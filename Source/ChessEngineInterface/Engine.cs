using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ChessEngineInterface
{
    public class Engine : IDisposable
    {
        #region Fields

        private long _currentIndex = 0;
        private volatile string _currentOutput;
        private Process _engine;
        private StreamWriter _writer;
        private AutoResetEvent _isreadyEvent;
        private AutoResetEvent _stoppedEvent;

        #endregion

        #region Properties

        public int TimeoutDuration { get; set; } = 1000;
        public string Description { get; private set; } = "";
        public string ExecutablePath { get; } = "";

        public bool IsReady
        {
            get
            {
                _isreadyEvent.Reset();

                Query("isready");

                return _isreadyEvent.WaitOne(TimeoutDuration);
            }
        }

        #endregion

        #region Events

        public event OutputReceived OutputDataReceived;
        public event OutputInfoReceived OutputDataInfoReceived;
        public event EvaluationReceived EvaluationReceived;
        public event Action EngineStopped;

        #endregion

        #region Constructors

        public Engine(string exectuablePath)
        {
            if (string.IsNullOrWhiteSpace(exectuablePath))
            {
                throw new ArgumentException($"Parameter '{nameof(exectuablePath)}' must not be null or empty");
            }

            FileInfo exectuableInfo = new FileInfo(exectuablePath);

            if (!exectuableInfo.Exists)
            {
                throw new ArgumentException("Given path doesn't exist");
            }

            if (exectuableInfo.Extension != ".exe")
            {
                throw new ArgumentException("Given path doesn't lead to an executable file '.exe'");
            }

            ExecutablePath = exectuablePath;
            _currentOutput = string.Empty;
            _isreadyEvent = new AutoResetEvent(false);
            _stoppedEvent = new AutoResetEvent(false);

            InitializeEngine();

            if (!IsReady)
            {
                throw new ArgumentException("Given executable not compatible or not responding");
            }
        }

        #endregion

        #region Methods

        public void RequestStop()
        {
            Query("stop");
        }

        public void Query(string input)
        {
            _writer.WriteLine(input);
            _writer.Flush();
        }

        private void InitializeEngine()
        {
            _engine = new Process();
            _engine.StartInfo.FileName = ExecutablePath;
            _engine.StartInfo.UseShellExecute = false;
            _engine.StartInfo.CreateNoWindow = true;
            _engine.StartInfo.RedirectStandardInput = true;
            _engine.StartInfo.RedirectStandardOutput = true;
            _engine.OutputDataReceived += OnOutputDataReceived;
            _engine.Start();
            _engine.BeginOutputReadLine();

            _writer = _engine.StandardInput;
            _writer.WriteLine("uci");
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            if (Description == "")
            {
                Description = e.Data;
                return;
            }

            if (e.Data.Contains(" "))
            {
                string typeIdentifier = e.Data.Substring(0, e.Data.IndexOf(' '));

                bool validIdentifier = false;
                string[] names = Enum.GetNames(typeof(OutputType));

                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i].ToLowerInvariant() == typeIdentifier)
                    {
                        validIdentifier = true;
                        break;
                    }
                }

                if (validIdentifier)
                {
                    typeIdentifier = e.Data.Substring(0, e.Data.IndexOf(' '));
                    string data = e.Data.Substring(e.Data.IndexOf(' ') + 1);

                    if (typeIdentifier == "bestmove")
                    {
                        EngineStopped?.Invoke();
                    }

                    if (OutputDataReceived != null)
                    {
                        OutputDataReceived(data, (OutputType)Enum.Parse(typeof(OutputType), typeIdentifier, true));
                    }

                    if (typeIdentifier == "info")
                    {
                        string[] infoComponents = data.Split(' ');
                        string[] infoTypeNames = Enum.GetNames(typeof(InfoType));

                        List<InfoType> types = new List<InfoType>();
                        List<string> values = new List<string>();

                        bool isCurrMove = true;

                        for (int i = 0; i < infoComponents.Length; i++)
                        {
                            if (infoComponents[i] == "pv")
                            {
                                isCurrMove = false;
                                break;
                            }
                        }

                        int currentType = -1;
                        string currentInfoData = "";

                        for (int i = 0; i < infoComponents.Length; i++)
                        {
                            bool infoType = false;

                            for (int typeI = 0; typeI < infoTypeNames.Length; typeI++)
                            {
                                if (infoComponents[i] == infoTypeNames[typeI].ToLowerInvariant())
                                {
                                    if (i != 0)
                                    {
                                        OutputDataInfoReceived?.Invoke(_currentIndex, isCurrMove, currentInfoData, (InfoType)currentType);

                                        if (!isCurrMove)
                                        {
                                            types.Add((InfoType)currentType);
                                            values.Add(currentInfoData);
                                        }
                                    }

                                    currentType = typeI;
                                    currentInfoData = "";
                                    infoType = true;
                                    break;
                                }
                            }

                            if (!infoType)
                            {
                                currentInfoData += $"{infoComponents[i]} ";
                            }
                        }

                        OutputDataInfoReceived?.Invoke(_currentIndex, isCurrMove, currentInfoData, (InfoType)currentType);

                        if (!isCurrMove)
                        {
                            types.Add((InfoType)currentType);
                            values.Add(currentInfoData);

                            Evaluation evaluation = new Evaluation(types.ToArray(), values.ToArray());
                            EvaluationReceived?.Invoke(evaluation);
                        }
                    }

                    _currentIndex++;
                }
            }

            if (e.Data == "readyok")
            {
                _isreadyEvent.Set();
            }
        }

        public void Dispose()
        {
            _engine.Kill();
            _engine = null;
        }

        #endregion
    }
}
