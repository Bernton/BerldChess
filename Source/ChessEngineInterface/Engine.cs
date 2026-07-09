using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ChessEngineInterface
{
    /// <summary>
    /// Drives a UCI chess engine process, exposing typed commands and results instead of the raw
    /// UCI text protocol. The <see cref="EvaluationReceived"/> and <see cref="BestMoveFound"/>
    /// events are raised on a background thread, so marshal to the UI thread before touching UI.
    /// </summary>
    public sealed class Engine : IDisposable
    {
        private const int BootTimeoutMs = 10000;
        private const int ShutdownTimeoutMs = 1000;

        private readonly AutoResetEvent _uciOk = new AutoResetEvent(false);
        private readonly List<string> _options = new List<string>();
        private readonly object _writeLock = new object();
        private Process _process;
        private StreamWriter _writer;

        /// <summary>Starts the engine and completes the UCI handshake.</summary>
        public Engine(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw new ArgumentException("Executable path must not be null or white space.", nameof(executablePath));
            }

            var executable = new FileInfo(executablePath);

            if (!executable.Exists)
            {
                throw new ArgumentException("Given path does not exist.", nameof(executablePath));
            }

            if (!string.Equals(executable.Extension, ".exe", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Given path does not lead to an '.exe' file.", nameof(executablePath));
            }

            ExecutablePath = executablePath;
            Start();

            // Send "uci" and wait for "uciok" so Name/Author/Options are populated and later
            // commands are not sent before the engine has entered uci mode.
            _uciOk.Reset();
            Send("uci");
            _uciOk.WaitOne(BootTimeoutMs);
        }

        #region Identity

        public string ExecutablePath { get; }
        public string Name { get; private set; }
        public string Author { get; private set; }

        /// <summary>The raw "option ..." descriptors the engine advertised at startup.</summary>
        public IReadOnlyList<string> Options => _options.AsReadOnly();

        #endregion

        #region Events

        /// <summary>Raised for every principal-variation line the engine reports during a search.</summary>
        public event Action<Evaluation> EvaluationReceived;

        /// <summary>
        /// Raised when the engine finishes a search. The argument is the best move in long
        /// algebraic notation, or null when the engine reports a null move (terminal position).
        /// </summary>
        public event Action<string> BestMoveFound;

        #endregion

        #region Commands

        public void SetOption(string name, string value)
        {
            Send($"setoption name {name} value {value}");
        }

        public void SetOption(string name, int value)
        {
            SetOption(name, value.ToString());
        }

        /// <summary>Presses a "button"-type option (no value).</summary>
        public void SetOption(string name)
        {
            Send($"setoption name {name}");
        }

        public void NewGame()
        {
            Send("ucinewgame");
        }

        public void SetPosition(string fen)
        {
            Send($"position fen {fen}");
        }

        public void GoInfinite()
        {
            Send("go infinite");
        }

        public void GoDepth(int depth)
        {
            Send($"go depth {depth}");
        }

        public void GoMoveTime(long milliseconds)
        {
            Send($"go movetime {milliseconds}");
        }

        public void GoNodes(long nodes)
        {
            Send($"go nodes {nodes}");
        }

        public void GoTimeControl(long whiteTime, long blackTime, long whiteIncrement, long blackIncrement)
        {
            Send($"go wtime {whiteTime} btime {blackTime} winc {whiteIncrement} binc {blackIncrement}");
        }

        public void RequestStop()
        {
            Send("stop");
        }

        /// <summary>Sends a raw UCI command, for custom/engine-specific options the typed methods don't cover.</summary>
        public void SendCommand(string command)
        {
            Send(command);
        }

        #endregion

        #region Process I/O

        private void Start()
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = ExecutablePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };

            _process.OutputDataReceived += OnOutputDataReceived;
            _process.Start();
            _process.BeginOutputReadLine();

            // Process.StandardInput uses Console.InputEncoding, which can be UTF-8 and emit a BOM.
            // Accessing BaseStream flushes that stray BOM onto the pipe; write with a no-preamble
            // encoding and send one empty line first so the BOM sits on its own (ignored) line.
            _writer = new StreamWriter(_process.StandardInput.BaseStream, new UTF8Encoding(false)) { AutoFlush = true };
            _writer.WriteLine();
        }

        private void Send(string command)
        {
            // Serialize writes: commands come from the UI thread, while a "registration"
            // reply can be sent from the engine's output-reader thread.
            lock (_writeLock)
            {
                _writer.WriteLine(command);
            }
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            // UCI allows arbitrary white space between tokens.
            string[] tokens = e.Data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
            {
                return;
            }

            switch (tokens[0])
            {
                case "id":

                    if (tokens.Length >= 3)
                    {
                        string value = string.Join(" ", tokens, 2, tokens.Length - 2);

                        if (tokens[1] == "name")
                        {
                            Name = value;
                        }
                        else if (tokens[1] == "author")
                        {
                            Author = value;
                        }
                    }

                    break;

                case "uciok":

                    _uciOk.Set();
                    break;

                case "bestmove":

                    if (tokens.Length >= 2)
                    {
                        bool isNullMove = tokens[1] == "(none)" || tokens[1] == "0000";
                        BestMoveFound?.Invoke(isNullMove ? null : tokens[1]);
                    }

                    break;

                case "registration":

                    // The GUI must answer a registration error so the engine knows it can deal
                    // with registration; we don't register, so defer it.
                    if (tokens.Length >= 2 && tokens[1] == "error")
                    {
                        Send("register later");
                    }

                    break;

                case "info":

                    Evaluation evaluation = ParseInfo(tokens);

                    if (evaluation != null)
                    {
                        EvaluationReceived?.Invoke(evaluation);
                    }

                    break;

                case "option":

                    if (tokens.Length >= 3 && tokens[1] == "name")
                    {
                        string option = string.Join(" ", tokens, 2, tokens.Length - 2);

                        if (!_options.Contains(option))
                        {
                            _options.Add(option);
                        }
                    }

                    break;
            }
        }

        /// <summary>Parses an "info" line into an <see cref="Evaluation"/>, or null if it carries no pv.</summary>
        private static Evaluation ParseInfo(string[] tokens)
        {
            int depth = 0, selDepth = 0, multiPv = 1, time = 0;
            long nodes = 0, nps = 0, tbHits = 0;
            Score score = default;
            List<string> pv = null;

            for (int i = 1; i < tokens.Length; i++)
            {
                switch (tokens[i])
                {
                    case "depth": TryInt(tokens, ref i, out depth); break;
                    case "seldepth": TryInt(tokens, ref i, out selDepth); break;
                    case "multipv": TryInt(tokens, ref i, out multiPv); break;
                    case "time": TryInt(tokens, ref i, out time); break;
                    case "nodes": TryLong(tokens, ref i, out nodes); break;
                    case "nps": TryLong(tokens, ref i, out nps); break;
                    case "tbhits": TryLong(tokens, ref i, out tbHits); break;

                    case "score":

                        // "score cp <x>" | "score mate <y>" (optionally followed by lowerbound/upperbound)
                        if (i + 2 < tokens.Length)
                        {
                            bool isMate = tokens[i + 1] == "mate";
                            int.TryParse(tokens[i + 2], out int scoreValue);
                            score = new Score(scoreValue, isMate);
                            i += 2;
                        }

                        break;

                    case "pv":

                        pv = new List<string>();

                        for (int j = i + 1; j < tokens.Length; j++)
                        {
                            pv.Add(tokens[j]);
                        }

                        i = tokens.Length;
                        break;

                    case "string":

                        // The rest of an "info string" line is literal text; stop parsing fields.
                        i = tokens.Length;
                        break;
                }
            }

            // Only info lines carrying a principal variation are treated as evaluations;
            // currmove/string/etc. lines are ignored.
            if (pv == null)
            {
                return null;
            }

            return new Evaluation(multiPv, depth, selDepth, score, nodes, nps, time, tbHits, pv.AsReadOnly());
        }

        private static void TryInt(string[] tokens, ref int i, out int value)
        {
            value = 0;

            if (i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out value))
            {
                i++;
            }
        }

        private static void TryLong(string[] tokens, ref int i, out long value)
        {
            value = 0;

            if (i + 1 < tokens.Length && long.TryParse(tokens[i + 1], out value))
            {
                i++;
            }
        }

        #endregion

        public void Dispose()
        {
            try
            {
                if (_process != null && !_process.HasExited)
                {
                    // Ask the engine to quit and let it shut down gracefully; force-kill on timeout.
                    Send("quit");

                    if (!_process.WaitForExit(ShutdownTimeoutMs))
                    {
                        _process.Kill();
                    }
                }
            }
            catch
            {
            }

            try
            {
                _writer?.Dispose();

                if (_process != null)
                {
                    _process.OutputDataReceived -= OnOutputDataReceived;
                    _process.Dispose();
                }

                _uciOk.Dispose();
            }
            catch
            {
            }

            _process = null;
        }
    }
}
