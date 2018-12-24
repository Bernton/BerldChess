using BerldChess.Model;
using BerldChess.Properties;
using BerldChess.ViewModel;
using ChessDotNet;
using ChessDotNet.Pieces;
using ChessEngineInterface;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using WindowsInput;

namespace BerldChess.View
{
    public partial class FormMain : Form
    {
        #region Fields

        private bool _totalChartView = false;
        private volatile bool _evaluationEnabled = true;
        private volatile bool _updateAfterAnimation = false;
        private volatile bool _analyzingMode = true;
        private volatile bool _movePlayed = true;
        private int _mainPvReference;
        private int _animationTime = 300;
        private int _draws = 0;
        private const int CentipawnTolerance = 85;
        private const int ChartShownPlies = 25;
        private const double DefaultSizeFactor = 0.82;
        private ChessPlayer _computerPlayer = ChessPlayer.None;
        private InfoType[] _columnOrder;
        private Random _random = new Random();
        private Color _darkModeColor = Color.FromArgb(49, 46, 43);
        private Color _selectionColor = Color.FromArgb(160, 177, 199, 208);
        private Bitmap _comparisonSnap = null;
        private ChessPanel _chessPanel;
        private Stopwatch[] _playerTimes = new Stopwatch[] { new Stopwatch(), new Stopwatch() };
        private SoundPlayer _movePlayer = new SoundPlayer(Resources.Move);
        private SoundPlayer _castlingPlayer = new SoundPlayer(Resources.Castling);
        private SoundPlayer _capturePlayer = new SoundPlayer(Resources.Capture);
        private SoundPlayer _illegalPlayer = new SoundPlayer(Resources.Ilegal);
        private InputSimulator _inputSimulator = new InputSimulator();
        private PieceMovedEventArgs _moveOnHold = null;
        private CancellationTokenSource _analysisTaskTokenSource;
        private FormMainViewModel _vm;
        private FormPieceSettings _pieceDialog;
        private FormBoardSetting _boardDialog;
        private FormEngineSettings _engineDialog;
        private int[] _engineWins = new int[2];
        private string[] _pgnImportNames = null;
        private string[] _enginePaths = new string[2];
        private volatile Evaluation[] _evaluations;
        private Control[] _engineViewElements;
        bool _switched = false;

        #endregion

        #region Constructors

        public FormMain()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            InitializeComponent();
            _panelEvaluationChart.SetDoubleBuffered();
            _dataGridViewMoves.SetDoubleBuffered();

            _vm = new FormMainViewModel();
            InitializeChessBoard();
            InitializeFormAndControls();

            DeserializeInfo();
            LoadInfo();

            InitializeEvaluationGrid();
            InitializeEngine();

            if (_menuItemCheatMode.Checked)
            {
                Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare, SerializedInfo.Instance.EngineDarkSquare);
            }
        }

        #endregion

        #region Initialization Methods

        private void InitializeEngine()
        {
            ClearEngines();

            switch (SerializedInfo.Instance.EngineMode)
            {
                case EngineMode.Disabled:

                    _evaluationEnabled = false;
                    SetEngineViewElementsVisible(false);
                    _chessPanel.Arrows.Clear();
                    _chessPanel.Invalidate();

                    break;

                case EngineMode.Analysis:

                    SetEngineViewElementsVisible(SetupEngine(SerializedInfo.Instance.EngineList.SelectedSetting, 0, true));
                    _evaluationEnabled = true;
                    break;

                case EngineMode.Competitive:

                    _switched = false;
                    _engineWins = new int[2];
                    _draws = 0;

                    bool validEngines = true;

                    if (!SetupEngine(SerializedInfo.Instance.EngineList.SelectedSetting, 0, false))
                    {
                        validEngines = false;
                    }

                    if (SerializedInfo.Instance.EngineList.SelectedSetting != null)
                    {
                        _enginePaths[0] = SerializedInfo.Instance.EngineList.SelectedSetting.ExecutablePath;
                    }

                    if (!SetupEngine(SerializedInfo.Instance.EngineList.SelectedOpponentSetting, 1, false))
                    {
                        validEngines = false;
                    }

                    if (SerializedInfo.Instance.EngineList.SelectedOpponentSetting != null)
                    {
                        _enginePaths[1] = SerializedInfo.Instance.EngineList.SelectedOpponentSetting.ExecutablePath;
                    }

                    if (validEngines)
                    {
                        _vm.Engines[(int)_vm.Game.WhoseTurn].Query($"position fen {_vm.Game.GetFen()}");
                        _vm.Engines[(int)_vm.Game.WhoseTurn].Query("go infinite");
                    }

                    SetEngineViewElementsVisible(validEngines);

                    if (!validEngines)
                    {
                        ClearEngines();
                    }

                    _evaluationEnabled = true;
                    break;
            }
        }

        private void InitializeChessBoard()
        {
            _chessPanel = new ChessPanel();
            _chessPanel.Cursor = Cursors.Hand;
            _chessPanel.BackColor = SystemColors.Control;
            _chessPanel.Dock = DockStyle.Fill;
            _chessPanel.Location = new Point(0, 0);
            _chessPanel.Size = new Size(787, 403);
            _chessPanel.Game = _vm.Game;
            _chessPanel.Select();
            _chessPanel.PieceMoved += OnPieceMoved;
            _splitContainerBoard.Panel1.Controls.Add(_chessPanel);
        }

        private void InitializeEvaluationGrid()
        {
            _columnOrder = new InfoType[]
            {
                InfoType.MultiPV,
                InfoType.Depth,
                InfoType.Score,
                InfoType.Time,
                InfoType.Nodes,
                InfoType.NPS,
                InfoType.TBHits,
                InfoType.PV,
            };

            ResetEvaluationGridColumns(_columnOrder);
            ResetEvaluationData(SerializedInfo.Instance.MultiPv);
            ResetEvaluationGridRows(SerializedInfo.Instance.MultiPv);
        }

        private void InitializeFormAndControls()
        {
            Icon = Resources.PawnRush;
            Text = string.Format("BerldChess Version {0}", Assembly.GetEntryAssembly().GetName().Version.ToString(2));

            _movePlayer.Load();
            _capturePlayer.Load();
            _castlingPlayer.Load();
            _illegalPlayer.Load();

            Tag = BackColor;

            foreach (Control control in GetAllChildControls(this))
            {
                control.Tag = control.BackColor;
            }

            _engineViewElements = new Control[]
            {
                _tableLayoutPanelEvalInfos,
                _labelShowEvaluation,
                _labelEvaluation,
                _panelEvaluationChart
            };
        }

        #endregion

        #region Event Methods

        private void OnEngineSelected()
        {
            InitializeEngine();
        }

        private void OnDataGridViewMovesResize(object sender, EventArgs e)
        {
            _dataGridViewMoves.DefaultCellStyle.Font = new Font(_dataGridViewMoves.DefaultCellStyle.Font.FontFamily, _dataGridViewMoves.Width / 22.5F);
        }

        private void OnBestMoveFound(string move)
        {
            if (!_movePlayed)
            {
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        PlayMove(move);
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            OnEngineStopped();
        }

        private void OnEngineStopped()
        {
            if (InvokeRequired)
            {
                Invoke((Action)OnEngineStopped);
                return;
            }

            EmptyEvaluationGrid();

            _chessPanel.Arrows.Clear();
            _chessPanel.Invalidate();

            bool finishedPosition = false;
            string evaluationText = null;
            DrawReason drawReason = 0;

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {

                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    if ((!_switched && _vm.Game.WhoseTurn == ChessPlayer.White) || (_switched && _vm.Game.WhoseTurn == ChessPlayer.Black))
                    {
                        _engineWins[1]++;
                    }
                    else
                    {
                        _engineWins[0]++;
                    }
                }

                evaluationText = "Checkmate";
                finishedPosition = true;
            }
            else if (_vm.Game.IsStalemated(_vm.Game.WhoseTurn))
            {
                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    _draws++;
                }

                evaluationText = "Stalemate";
                finishedPosition = true;
            }
            else if (_vm.Game.CheckIfDraw(ref drawReason))
            {
                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    _draws++;
                }

                evaluationText = GetFormattedDrawReason(drawReason);
                finishedPosition = true;
            }

            if (finishedPosition)
            {
                for (int i = 0; i < _playerTimes.Length; i++)
                {
                    _playerTimes[i].Stop();
                }

                SetExtendedInfoEnabled(false);
                _labelEvaluation.Text = evaluationText;
                _labelEvaluation.ForeColor = _menuItemNew.ForeColor;

                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    ResetGame(new ChessGame());
                    OnMenuItemAutoPlayClick(null, null);
                }
            }
            else
            {
                int engineIndex = 0;

                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    ChessPlayer onTurn = _vm.Game.WhoseTurn;
                    engineIndex = (int)onTurn;
                }

                _vm.Engines[engineIndex].Query($"position fen {_vm.Game.GetFen()}");

                if (_analyzingMode && (_computerPlayer == ChessPlayer.None || _computerPlayer != _vm.Game.WhoseTurn))
                {
                    _vm.Engines[engineIndex].Query("go infinite");
                }
                else
                {
                    switch (SerializedInfo.Instance.Level.SelectedLevelType)
                    {
                        case LevelType.FixedDepth:

                            _vm.Engines[engineIndex].Query($"go depth {SerializedInfo.Instance.Level.Plies}");
                            break;

                        case LevelType.TimePerMove:

                            _vm.Engines[engineIndex].Query($"go movetime {SerializedInfo.Instance.Level.TimePerMove}");
                            break;

                        case LevelType.TotalTime:

                            long increment = SerializedInfo.Instance.Level.Increment;
                            _vm.Engines[engineIndex].Query($"go wtime {GetRemainingTime(0)} btime {GetRemainingTime(1)} winc {increment} binc{increment}");
                            break;

                        case LevelType.Infinite:

                            _vm.Engines[engineIndex].Query("go infinite");
                            break;

                        case LevelType.Nodes:

                            _vm.Engines[engineIndex].Query($"go nodes {SerializedInfo.Instance.Level.Nodes}");
                            break;
                    }
                }

                _movePlayed = false;
                _evaluationEnabled = true;
            }
        }

        private void OnDialogFontSelected()
        {
            ChessFont currentFont = (ChessFont)SerializedInfo.Instance.SelectedChessFont;
            _chessPanel.IsUnicodeFont = currentFont.IsUnicode;
            _chessPanel.ChessFontChars = currentFont.PieceCharacters;
            _chessPanel.PieceSizeFactor = currentFont.SizeFactor;
            _chessPanel.PieceFontFamily = currentFont.FontFamily;

            _chessPanel.InvalidateRender();
        }

        private void OnBoardSettingAltered()
        {
            _chessPanel.BorderHighlight = SerializedInfo.Instance.BorderHighlight;
            _chessPanel.Gradient = SerializedInfo.Instance.Gradient;
            _chessPanel.DarkSquare = SerializedInfo.Instance.BoardDarkSquare;
            _chessPanel.LightSquare = SerializedInfo.Instance.BoardLightSquare;
            _chessPanel.DisplayGridBorders = SerializedInfo.Instance.DisplayGridBorder;
            _chessPanel.IvoryMode = SerializedInfo.Instance.IvoryMode;
            _chessPanel.UseBoardImages = SerializedInfo.Instance.UseImages;

            if (SerializedInfo.Instance.UseImages)
            {
                TryApplyBoardImages();
            }

            _chessPanel.InvalidateRender();
            SetDarkMode(this, SerializedInfo.Instance.DarkMode);
        }

        private void TryApplyBoardImages()
        {
            Image lightSquareImage = TryGetImage(SerializedInfo.Instance.LightSquarePath);
            Image darkSquareImage = TryGetImage(SerializedInfo.Instance.DarkSquarePath);

            if (lightSquareImage != null && darkSquareImage != null)
            {
                _chessPanel.DarkSquareImage = darkSquareImage;
                _chessPanel.LightSquareImage = lightSquareImage;
            }
        }

        private Image TryGetImage(string path)
        {
            Image image;

            try
            {
                image = new Bitmap(path);
            }
            catch
            {
                image = null;
            }

            return image;
        }

        private void OnEvaluationReceived(Evaluation evaluation)
        {
            if (_evaluationEnabled)
            {
                ProcessEvaluation(evaluation);
            }
        }

        private void OnPieceMoved(object sender, PieceMovedEventArgs e)
        {
            Engine playingEngine = GetPlayingEngine();

            if (PlayMove(e))
            {
                playingEngine?.RequestStop();
            }
        }

        private void OnFormMainLoad(object sender, EventArgs e)
        {
            _menuItemMultiPv.Text = $"MultiPv [{SerializedInfo.Instance.MultiPv}]";
            _menuItemAnimationTime.Text = $"Animation Time [{_animationTime}]";
            _menuItemClickDelay.Text = $"Click Delay [{SerializedInfo.Instance.ClickDelay}]";

            if (SerializedInfo.Instance.IsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }

            if (SerializedInfo.Instance.SplitterDistance != null)
            {
                _splitContainerMain.SplitterDistance = (int)SerializedInfo.Instance.SplitterDistance;
            }
        }

        private void OnTimerValidationTick(object sender, EventArgs e)
        {
            if (SerializedInfo.Instance.Level.SelectedLevelType == LevelType.TotalTime)
            {
                var format = SerializedInfo.Instance.Level.TotalTime >= 3600000 ? @"h\:mm\:ss" : @"mm\:ss\.ff";

                _menuItemWhiteTime.Text = TimeSpan.FromMilliseconds(GetRemainingTime(0)).ToString(format);
                _menuItemBlackTime.Text = TimeSpan.FromMilliseconds(GetRemainingTime(1)).ToString(format);
            }

            if (!_vm.GameFinished && _evaluationEnabled)
            {
                UpdateUI();
            }

            _panelEvaluationChart.Invalidate();
            _chessPanel.Invalidate();
        }

        private void OnTimerAutoCheckTick(object sender, EventArgs e)
        {
            if (!_analyzingMode && _vm.GameFinished)
            {
                _analyzingMode = true;
            }

            if (!_analyzingMode)
            {
                _computerPlayer = _vm.Game.WhoseTurn;
            }

            if (_menuItemCheatMode.Checked)
            {
                CheckExternBoard();
            }
        }

        private void OnMenuItemLoadFenClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Position FEN:", "BerldChess - FEN");

            if (input == "")
            {
                return;
            }

            ChessGame gameFromFen;

            try
            {
                gameFromFen = new ChessGame(input);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Berld Chess", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Debug.WriteLine(ex.ToString());
                return;
            }

            ResetGame(gameFromFen);
        }

        private void OnMenuItemDisplayLegalMovesCheckedChanged(object sender, EventArgs e)
        {
            _chessPanel.DisplayLegalMoves = _menuItemDisplayLegalMoves.Checked;
            _chessPanel.Invalidate();
        }

        private void OnMenuItemDisplayCoordinatesCheckedChanged(object sender, EventArgs e)
        {
            _chessPanel.DisplayCoordinates = _menuItemDisplayCoordinates.Checked;
            _chessPanel.Invalidate();
        }

        private void OnMenuItemBackClick(object sender, EventArgs e)
        {
            if (_vm.NavigationIndex > 0)
            {
                Navigate(_vm.NavigationIndex - 1);
            }
        }

        private void OnMenuItemForwardClick(object sender, EventArgs e)
        {
            if (_vm.NavigationIndex != _vm.PlyList.Count - 1)
            {
                Navigate(_vm.NavigationIndex + 1);
            }
        }

        private void OnMenuItemStartClick(object sender, EventArgs e)
        {
            Navigate(0);
        }

        private void OnMenuItemLatestClick(object sender, EventArgs e)
        {
            Navigate(_vm.PlyList.Count - 1);
        }

        private void OnFormMainClosing(object sender, FormClosingEventArgs e)
        {
            SerializeInfo();
            ClearEngines();
        }

        private void OnMenuItemFlipBoardCheckedChanged(object sender, EventArgs e)
        {
            if (!_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
                _chessPanel.Invalidate();
            }
        }

        private void OnMenuItemHideArrowsCheckedChanged(object sender, EventArgs e)
        {
            if (_menuItemHideArrows.Checked && _chessPanel != null && _chessPanel.Arrows != null)
            {
                _chessPanel.Arrows.Clear();
                _chessPanel.Invalidate();
            }
        }

        private void OnMenuItemHideOutputCheckedChanged(object sender, EventArgs e)
        {
            _splitContainerBoard.Panel2Collapsed = _menuItemHideOutput.Checked;
        }

        private void OnMenuItemLocalModeCheckedChanged(object sender, EventArgs e)
        {
            _menuItemFlipBoard.Enabled = !_menuItemLocalMode.Checked;

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
            }

            _chessPanel.Invalidate();
        }

        private void OnMenuItemNewClick(object sender, EventArgs e)
        {
            ResetGame(new ChessGame());
        }

        private void OnMenuItemComputerMoveClick(object sender, EventArgs e)
        {
            if (GetPlayingEngine() != null)
            {
                if (_computerPlayer != ChessPlayer.None)
                {
                    _computerPlayer = ChessPlayer.None;
                    GetPlayingEngine().RequestStop();
                }
                else
                {
                    _computerPlayer = _vm.Game.WhoseTurn;
                    GetPlayingEngine().RequestStop();
                }
            }
        }

        private void OnMenuItemAutoPlayClick(object sender, EventArgs e)
        {
            if (GetPlayingEngine() != null)
            {
                _analyzingMode = !_analyzingMode;
                _computerPlayer = _vm.Game.WhoseTurn;

                _movePlayed = true;
                GetPlayingEngine().RequestStop();
            }
        }

        private void OnMenuItemAutoMoveClick(object sender, EventArgs e)
        {
            if (_moveOnHold != null)
            {
                Bitmap currentImage = Recognizer.GetBoardSnap();

                if (PlayMove(_moveOnHold, 'q', true))
                {
                    _movePlayed = true;
                    GetPlayingEngine().RequestStop();
                }

                Recognizer.UpdateBoardImage(currentImage);

                _moveOnHold = null;
            }
        }

        private void OnMenuItemUpdateClick(object sender, EventArgs e)
        {
            if (!Recognizer.BoardFound)
            {
                if (!Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare, SerializedInfo.Instance.EngineDarkSquare))
                {
                    MessageBox.Show("No board found!", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                Recognizer.UpdateBoardImage();
            }
        }

        private void OnMenuItemSquareColorsClick(object sender, EventArgs e)
        {
            Bitmap[] images = new Bitmap[Screen.AllScreens.Length];

            for (int i = 0; i < images.Length; i++)
            {
                images[i] = Recognizer.GetScreenshot(Screen.AllScreens[i]);
            }

            FormSquareColor squareColorDialog = new FormSquareColor(images);
            squareColorDialog.ShowDialog();

            if (squareColorDialog.DarkSquareColor != null)
            {
                SerializedInfo.Instance.EngineDarkSquare = (Color)squareColorDialog.DarkSquareColor;
            }

            if (squareColorDialog.LightSquareColor != null)
            {
                SerializedInfo.Instance.EngineLightSquare = (Color)squareColorDialog.LightSquareColor;
            }
        }

        private void OnMenuItemResetClick(object sender, EventArgs e)
        {
            if (!Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare, SerializedInfo.Instance.EngineDarkSquare))
            {
                MessageBox.Show("No board found!", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnMenuItemPiecesClick(object sender, EventArgs e)
        {
            _pieceDialog.ShowDialog();
        }

        private void OnMenuItemBoardClick(object sender, EventArgs e)
        {
            _boardDialog.ShowDialog();
        }

        private void OnMenuItemMultiPvClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter MultiPv:", "BerldChess - MultiPv");
            int multiPV;


            for (int i = 0; i < _vm.Engines.Length; i++)
            {
                if (_vm.Engines[i] != null)
                {
                    if (int.TryParse(input, out multiPV))
                    {
                        if (multiPV > 0 && multiPV <= 250)
                        {
                            _evaluationEnabled = false;
                            _movePlayed = true;

                            _menuItemMultiPv.Text = $"MultiPv [{SerializedInfo.Instance.MultiPv}]";
                            _vm.Engines[i].Query($"setoption name MultiPv value {SerializedInfo.Instance.MultiPv}");
                            _vm.Engines[i].RequestStop();
                            _chessPanel.Invalidate();

                            ResetEvaluationData(multiPV);
                            ResetEvaluationGridRows(SerializedInfo.Instance.MultiPv);
                            _evaluationEnabled = true;
                        }
                    }
                }
            }
        }

        private void OnMenuItemDepthAnalysisClick(object sender, EventArgs e)
        {
            if (GetPlayingEngine() == null)
            {
                return;
            }

            string input = Interaction.InputBox("Enter Depth:", "BerldChess - Depth Analysis");
            int depth;

            if (int.TryParse(input, out depth))
            {
                StartDepthAnalysis(depth);
            }
        }

        private void StartTimeAnalysis(int milliseconds)
        {
            if (SerializedInfo.Instance.EngineMode == EngineMode.Disabled || !_labelEvaluation.Visible) return;
            if (_vm.PlyList.Count <= 0) return;

            var dialog = new ProgressDialog($"Time analysis ({milliseconds} ms/ply)", "Analyzing...");
            dialog.ClosedByInterface += OnDepthAnalysisCancel;

            _analysisTaskTokenSource?.Cancel();
            _analysisTaskTokenSource = new CancellationTokenSource();

            var analysisTask = new Task(() =>
            {
                for (var i = _vm.NavigationIndex; i < _vm.PlyList.Count; i++)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        Navigate(i);
                    });

                    Invoke((MethodInvoker)delegate
                    {
                        dialog.StatusText = $"Analyzing ply {i}...";
                        dialog.ProgressBarPercentage = (int)Math.Ceiling((double)i / _vm.PlyList.Count * 100.0);
                    });

                    if (_analysisTaskTokenSource.Token.IsCancellationRequested)
                    {
                        Invoke((MethodInvoker)delegate { dialog.Hide(); });
                        return;
                    }

                    Thread.Sleep(milliseconds);
                }

                Invoke((MethodInvoker)delegate { dialog.Hide(); });
            }, _analysisTaskTokenSource.Token);

            analysisTask.Start();
            dialog.Show();
        }

        private void StartDepthAnalysis(int depth)
        {
            if (SerializedInfo.Instance.EngineMode == EngineMode.Disabled || !_labelEvaluation.Visible)
            {
                return;
            }

            if (_vm.PlyList.Count > 0)
            {

                ProgressDialog dialog = new ProgressDialog($"Analysis to depth {depth}", "Analyzing...", true);
                dialog.ClosedByInterface += OnDepthAnalysisCancel;

                _analysisTaskTokenSource?.Cancel();
                _analysisTaskTokenSource = new CancellationTokenSource();

                var analysisTask = new Task(() =>
                {
                    for (var i = _vm.NavigationIndex; i < _vm.PlyList.Count; i++)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            Navigate(i);
                        });

                        Invoke((MethodInvoker)delegate
                        {
                            dialog.StatusText = $"Analyzing ply {i}...";
                            dialog.ProgressBarPercentage = (int)Math.Ceiling((double)i / _vm.PlyList.Count * 100.0);
                        });

                        while (_vm.PlyList[i].EvaluationDepth < depth && !_vm.GameFinished)
                        {
                            Thread.Sleep(10);

                            if (_analysisTaskTokenSource.Token.IsCancellationRequested)
                            {
                                Invoke((MethodInvoker)delegate { dialog.Hide(); });
                                return;
                            }
                        }
                    }

                    Invoke((MethodInvoker)delegate { dialog.Hide(); });
                }, _analysisTaskTokenSource.Token);

                analysisTask.Start();
                dialog.Show();
            }
        }

        private void OnDepthAnalysisCancel(object sender, EventArgs e)
        {
            _analysisTaskTokenSource.Cancel();
        }

        private void OnMenuItemCheatModeCheckedChanged(object sender, EventArgs e)
        {
            foreach (ToolStripDropDownItem item in _menuItemCheatMode.DropDownItems)
            {
                if (_menuItemCheatMode.Checked)
                {
                    item.Enabled = true;
                }
                else
                {
                    item.Enabled = false;
                }
            }
        }

        private void OnMenuItemAnimationTimeClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Animation Time:", "BerldChess - Animation Time");
            int animTime;

            if (int.TryParse(input, out animTime))
            {
                _animationTime = animTime;
                _menuItemAnimationTime.Text = $"Anim Time [{_animationTime}]";
            }
        }

        private void OnMenuItemCopyFenClick(object sender, EventArgs e)
        {
            Clipboard.SetText(_vm.CurrentPly.Fen);
        }

        private void OnMenuItemClickDelayClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Click Delay:", "BerldChess -Click Delay");
            int clickDelay;

            if (int.TryParse(input, out clickDelay))
            {
                SerializedInfo.Instance.ClickDelay = clickDelay;
                _menuItemClickDelay.Text = $"Click Delay [{clickDelay}]";
            }
        }

        private void OnMenuItemLoadPgnClick(object sender, EventArgs e)
        {
            FormPgnLoader pgnLoader = new FormPgnLoader(SerializedInfo.Instance.PgnAnalysis, SerializedInfo.Instance.PgnAnalysisDepth);

            if (pgnLoader.ShowDialog() == DialogResult.OK)
            {
                SerializedInfo.Instance.PgnAnalysis = pgnLoader.Analysis;

                if (pgnLoader.Analysis)
                {
                    SerializedInfo.Instance.PgnAnalysisDepth = pgnLoader.Depth;
                }

                LoadGame(pgnLoader.PgnLoadedGame);

                if (pgnLoader.Analysis)
                {
                    StartDepthAnalysis(pgnLoader.Depth);
                }
            }
        }

        private void OnMenuItemEngineSettingsClick(object sender, EventArgs e)
        {
            _engineDialog.ShowDialog();
        }

        private void OnPanelEvaluationChartPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.Default;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            Pen gridPen = new Pen(_panelEvaluationChart.ForeColor.AlterRgb(200, !SerializedInfo.Instance.DarkMode), 1);
            Pen middleLinePen = new Pen(_panelEvaluationChart.ForeColor.AlterRgb(120, !SerializedInfo.Instance.DarkMode), 1);
            Pen chartBorderPen = new Pen(_panelEvaluationChart.ForeColor, 1);

            Font labelFont = new Font("Segoe UI", 10);

            SolidBrush fontBrush = new SolidBrush(_panelEvaluationChart.ForeColor);
            SolidBrush chartLightBrush = new SolidBrush(Color.White);
            SolidBrush chartDarkBrush = new SolidBrush(Color.FromArgb(69, 66, 63));
            SolidBrush selectionBrush = new SolidBrush(_selectionColor);

            int peak;
            int chartYMiddle = (int)(_panelEvaluationChart.Height / 2.0);

            List<ChessPly> positions = _vm.PlyList.ToList();

            while (positions.Count < 8)
            {
                positions.Add(new ChessPly(""));
            }

            if (!_totalChartView && positions.Count > ChartShownPlies)
            {
                positions.RemoveRange(0, positions.Count - ChartShownPlies);
            }

            int playStartIndex;

            if (_totalChartView)
            {
                playStartIndex = 0;
            }
            else
            {
                playStartIndex = _vm.PlyList.Count - positions.Count;
            }

            double highestValue = 0;
            double lowestValue = 0;

            foreach (var position in positions)
            {
                if (position.Evaluation > highestValue)
                {
                    highestValue = position.Evaluation;
                }
                else if (position.Evaluation < lowestValue)
                {
                    lowestValue = position.Evaluation;
                }
            }

            if (highestValue > Math.Abs(lowestValue))
            {
                peak = (int)Math.Ceiling(highestValue * 1.05);
            }
            else
            {
                peak = (int)Math.Ceiling(Math.Abs(lowestValue) * 1.05);
            }

            if (peak < 2)
            {
                peak = 2;
            }
            else if (peak > 8)
            {
                peak = 8;
            }

            int rowCount = peak * 2;
            int lineCount = 6;

            double xPart = _panelEvaluationChart.Width / (double)lineCount;
            double yPart = _panelEvaluationChart.Height / (double)rowCount;

            for (int i = 1; i < rowCount; i++)
            {
                g.DrawLine(gridPen, 0, (float)(yPart * i), _panelEvaluationChart.Width, (float)(yPart * i));
            }

            for (int i = 1; i < lineCount; i++)
            {
                g.DrawLine(gridPen, (float)(xPart * i), 0, (float)(xPart * i), _panelEvaluationChart.Height);
            }

            float lineWidth = (float)_panelEvaluationChart.Width / lineCount;

            for (int i = 1; i <= lineCount; i++)
            {
                if (i % 3 != 0)
                {
                    continue;
                }

                int valueX = (int)((positions.Count - 1) / (double)lineCount * i);

                if (!_totalChartView && _vm.PlyList.Count > ChartShownPlies)
                {
                    valueX += _vm.PlyList.Count - ChartShownPlies;
                }

                if (valueX > 2 || i == lineCount)
                {
                    string text = valueX.ToString();
                    float offSet = g.MeasureString(text, labelFont).Width / lineCount * i;

                    if (i == lineCount)
                    {
                        offSet += lineWidth * 0.15F;
                    }

                    g.DrawString(text, labelFont, fontBrush, lineWidth * i - offSet, _panelEvaluationChart.Height - labelFont.Size * 2.25F);
                }
            }

            for (int i = 1; i < rowCount; i++)
            {
                if (peak > 5 && i % 2 == 1)
                {
                    continue;
                }

                if (i == rowCount / 2)
                {
                    continue;
                }

                int valueY = peak - i;
                string text = valueY.ToString("+0;-0");
                float offSet = g.MeasureString(text, labelFont).Height / 2F;

                g.DrawString(text, labelFont, fontBrush, labelFont.Size / 2, (int)(_panelEvaluationChart.Height / (double)rowCount * i) - offSet);
            }

            g.DrawLine(middleLinePen, 0, chartYMiddle, _panelEvaluationChart.Width, chartYMiddle);

            double yUnitLength = _panelEvaluationChart.Height / (double)rowCount;
            double xUnitLength = (double)(_panelEvaluationChart.Width - 3) / positions.Count;

            if (peak != 0)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    double evaluation = positions[i].Evaluation;

                    float x = (float)(i * xUnitLength);
                    int y;

                    int height = Round(Math.Abs((evaluation * yUnitLength)));
                    float width = (float)(((i + 1) * xUnitLength - i * xUnitLength));

                    if (evaluation >= 0)
                    {
                        y = chartYMiddle - height;
                        g.FillRectangle(chartLightBrush, x, y, width, height);

                        if (i + playStartIndex == _vm.NavigationIndex)
                        {
                            g.FillRectangle(selectionBrush, x, y, width, height);
                        }

                        g.DrawRectangle(chartBorderPen, x, y, width, height);
                    }
                    else
                    {
                        y = chartYMiddle;
                        g.FillRectangle(chartDarkBrush, x, y, width, height);

                        if (i + playStartIndex == _vm.NavigationIndex)
                        {
                            g.FillRectangle(selectionBrush, x, y, width, height);
                        }

                        g.DrawRectangle(chartBorderPen, x, y, width, height);
                    }
                }
            }
        }

        private void OnPanelEvaluationChartMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_vm.PlyList.Count > 0)
                {
                    double xUnitLength;
                    int navIndex;

                    if (!_totalChartView && _vm.PlyList.Count > ChartShownPlies)
                    {
                        xUnitLength = (double)_panelEvaluationChart.Width / ChartShownPlies;
                        navIndex = (int)(e.X / xUnitLength) + (_vm.PlyList.Count - ChartShownPlies);
                    }
                    else
                    {
                        xUnitLength = (double)_panelEvaluationChart.Width / _vm.PlyList.Count;
                        navIndex = (int)(e.X / xUnitLength);
                    }

                    Navigate(navIndex);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                _totalChartView = !_totalChartView;
                _panelEvaluationChart.Invalidate();
            }
        }

        private void OnDataGridViewMovesKeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void OnDataGridViewMovesCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int navigationIndex = e.RowIndex * 2 + e.ColumnIndex + 1;

            if ((navigationIndex >= _vm.PlyList.Count || navigationIndex == _vm.NavigationIndex))
            {
                SelectMovesCell(navigationIndex);
                return;
            }

            Navigate(e.RowIndex * 2 + e.ColumnIndex + 1);
        }

        private void OnTableLayoutPanelModulesResize(object sender, EventArgs e)
        {
            _tableLayoutPanelModules.Height = Round(_tableLayoutPanelModules.Width * 1.942196);
        }

        private void OnLabelTextValidate(object sender, EventArgs e)
        {
            Control source = (Control)sender;
            source.FitFont(0.9, 0.9);
        }

        private void OnLabelEvaluationTextChanged(object sender, EventArgs e)
        {
            _labelEvaluation.FitFont(0.9, 0.98);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V)
            {
                MessageBox.Show($"{SerializedInfo.Instance.EngineList.SelectedSetting.Name}: {_engineWins[0].ToString()}\n\n{SerializedInfo.Instance.EngineList.SelectedOpponentSetting.Name}: {_engineWins[1].ToString()}\n\nDraws: {_draws.ToString()}", "BerldChess - Score");
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.T)
            {
                string plies = "";

                for (int i = 0; i < _vm.PlyList.Count; i++)
                {
                    plies += (i + 1).ToString() + " " + _vm.PlyList[i].Evaluation + " D" + _vm.PlyList[i].EvaluationDepth + " | ";
                }

                MessageBox.Show(plies, "BerldChess - Plies");
            }

            foreach (ToolStripMenuItem menuItem in GetMenuItems(_menuStripMain))
            {
                if (string.IsNullOrEmpty(menuItem.ShortcutKeyDisplayString))
                {
                    continue;
                }

                Keys key = (Keys)Enum.Parse(typeof(Keys), menuItem.ShortcutKeyDisplayString);

                if (e.KeyCode == key)
                {
                    menuItem.PerformClick();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        #endregion

        #region Other Methods

        private void LoadInfo()
        {
            if (SerializedInfo.Instance.Bounds != null)
            {
                Bounds = (Rectangle)SerializedInfo.Instance.Bounds;
            }

            _chessPanel.PieceSizeFactor = SerializedInfo.Instance.PieceSizeFactor;

            if (SerializedInfo.Instance.ChessFonts.Count > 0 &&
                SerializedInfo.Instance.SelectedFontIndex >= 0 &&
                SerializedInfo.Instance.SelectedFontIndex <
                SerializedInfo.Instance.ChessFonts.Count)
            {
                OnDialogFontSelected();
            }
            else
            {
                _chessPanel.PieceFontFamily = "";
                _chessPanel.PieceSizeFactor = DefaultSizeFactor;
            }

            _chessPanel.BorderHighlight = SerializedInfo.Instance.BorderHighlight;
            _chessPanel.DarkSquare = SerializedInfo.Instance.BoardDarkSquare;
            _chessPanel.LightSquare = SerializedInfo.Instance.BoardLightSquare;
            _chessPanel.Gradient = SerializedInfo.Instance.Gradient;
            _chessPanel.IvoryMode = SerializedInfo.Instance.IvoryMode;
            _chessPanel.DisplayLegalMoves = SerializedInfo.Instance.DisplayLegalMoves;
            _chessPanel.DisplayCoordinates = SerializedInfo.Instance.DisplayCoordinates;
            _chessPanel.UseBoardImages = SerializedInfo.Instance.UseImages;

            TryApplyBoardImages();

            _menuItemHideOutput.Checked = SerializedInfo.Instance.HideOutput;
            _menuItemHideArrows.Checked = SerializedInfo.Instance.HideArrows;

            _splitContainerBoard.Panel2Collapsed = _menuItemHideOutput.Checked;

            _chessPanel.DisplayGridBorders = SerializedInfo.Instance.DisplayGridBorder;
            _menuItemFlipBoard.Checked = SerializedInfo.Instance.BoardFlipped;
            _menuItemLocalMode.Checked = SerializedInfo.Instance.LocalMode;
            _menuItemDisplayCoordinates.Checked = SerializedInfo.Instance.DisplayCoordinates;
            _menuItemCheatMode.Checked = SerializedInfo.Instance.CheatMode;
            _menuItemCheckAuto.Checked = SerializedInfo.Instance.AutoCheck;
            _menuItemIllegalSound.Checked = SerializedInfo.Instance.IllegalSound;
            _menuItemDisplayLegalMoves.Checked = SerializedInfo.Instance.DisplayLegalMoves;
            _menuIllegalSound.Checked = SerializedInfo.Instance.Sound;
            _animationTime = SerializedInfo.Instance.AnimationTime;

            SetLevel();
            SetDarkMode(this, SerializedInfo.Instance.DarkMode);

            if (SerializedInfo.Instance.ChessFonts.Count == 0)
            {
                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Default",
                    FontFamily = "Default",
                    SizeFactor = DefaultSizeFactor,
                    IsUnicode = false,
                    PieceCharacters = ""
                });

                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Simple",
                    FontFamily = "Simple",
                    SizeFactor = 1,
                    IsUnicode = false,
                    PieceCharacters = ""
                });

                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Pixel",
                    FontFamily = "Pixel",
                    SizeFactor = 0.9,
                    IsUnicode = false,
                    PieceCharacters = ""
                });
            }
            else if (SerializedInfo.Instance.ChessFonts.Count > 2)
            {
                SerializedInfo.Instance.ChessFonts[0].Name = "Default";
                SerializedInfo.Instance.ChessFonts[0].FontFamily = "Default";
                SerializedInfo.Instance.ChessFonts[1].Name = "Simple";
                SerializedInfo.Instance.ChessFonts[1].FontFamily = "Simple";
                SerializedInfo.Instance.ChessFonts[2].Name = "Pixel";
                SerializedInfo.Instance.ChessFonts[2].FontFamily = "Pixel";
            }

            _engineDialog = new FormEngineSettings(SerializedInfo.Instance.EngineList);
            _engineDialog.EngineSelected += OnEngineSelected;

            _pieceDialog = new FormPieceSettings(SerializedInfo.Instance.ChessFonts, SerializedInfo.Instance.SelectedFontIndex);
            _pieceDialog.FontSelected += OnDialogFontSelected;

            _boardDialog = new FormBoardSetting();
            _boardDialog.BoardSettingAltered += OnBoardSettingAltered;

            _chessPanel.Invalidate();
        }

        private void SerializeInfo()
        {
            try
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    SerializedInfo.Instance.Bounds = RestoreBounds;
                }
                else
                {
                    SerializedInfo.Instance.Bounds = Bounds;
                }

                SerializedInfo.Instance.SplitterDistance = _splitContainerMain.SplitterDistance;

                SerializedInfo.Instance.IsMaximized = WindowState == FormWindowState.Maximized;
                SerializedInfo.Instance.BoardFlipped = _menuItemFlipBoard.Checked;
                SerializedInfo.Instance.HideArrows = _menuItemHideArrows.Checked;
                SerializedInfo.Instance.HideOutput = _menuItemHideOutput.Checked;
                SerializedInfo.Instance.LocalMode = _menuItemLocalMode.Checked;
                SerializedInfo.Instance.CheatMode = _menuItemCheatMode.Checked;

                SerializedInfo.Instance.Sound = _menuIllegalSound.Checked;
                SerializedInfo.Instance.AutoCheck = _menuItemCheckAuto.Checked;
                SerializedInfo.Instance.AnimationTime = _animationTime;
                SerializedInfo.Instance.DisplayLegalMoves = _menuItemDisplayLegalMoves.Checked;
                SerializedInfo.Instance.IllegalSound = _menuItemIllegalSound.Checked;
                SerializedInfo.Instance.DisplayCoordinates = _menuItemDisplayCoordinates.Checked;

                EngineSetting selectedSetting1 = SerializedInfo.Instance.EngineList.SelectedSetting;
                EngineSetting selectedSetting2 = SerializedInfo.Instance.EngineList.SelectedOpponentSetting;

                SerializedInfo.Instance.EngineList.Settings = SerializedInfo.Instance.EngineList.Settings.OrderBy(c => c.Name).ToList();

                SerializedInfo.Instance.EngineList.SelectedIndex = SerializedInfo.Instance.EngineList.Settings.IndexOf(selectedSetting1);
                SerializedInfo.Instance.EngineList.SelectedOpponentIndex = SerializedInfo.Instance.EngineList.Settings.IndexOf(selectedSetting2);

                if (SerializedInfo.Instance.ChessFonts.Count > 3)
                {
                    ChessFont fontDefault = SerializedInfo.Instance.ChessFonts[0];
                    ChessFont fontSimple = SerializedInfo.Instance.ChessFonts[1];
                    ChessFont fontPixel = SerializedInfo.Instance.ChessFonts[2];

                    ChessFont selectedFont = SerializedInfo.Instance.SelectedChessFont;

                    SerializedInfo.Instance.ChessFonts.RemoveRange(0, 3);

                    SerializedInfo.Instance.ChessFonts = SerializedInfo.Instance.ChessFonts.OrderBy(c => c.Name).ToList();

                    SerializedInfo.Instance.ChessFonts.Insert(0, fontPixel);
                    SerializedInfo.Instance.ChessFonts.Insert(0, fontSimple);
                    SerializedInfo.Instance.ChessFonts.Insert(0, fontDefault);

                    SerializedInfo.Instance.SelectedFontIndex = SerializedInfo.Instance.ChessFonts.IndexOf(selectedFont);
                }

                XmlSerializer serializer = new XmlSerializer(typeof(SerializedInfo));
                FileStream fileStream = new FileStream(FormMainViewModel.ConfigurationFileName, FileMode.Create);
                serializer.Serialize(fileStream, SerializedInfo.Instance);
                fileStream.Dispose();

                if (WindowState == FormWindowState.Maximized)
                {
                    SerializedInfo.Instance.Bounds = RestoreBounds;
                }
                else
                {
                    SerializedInfo.Instance.Bounds = Bounds;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void DeserializeInfo()
        {
            try
            {
                if (File.Exists(FormMainViewModel.ConfigurationFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializedInfo));
                    FileStream fileStream = new FileStream(FormMainViewModel.ConfigurationFileName, FileMode.Open);

                    SerializedInfo.Instance = (SerializedInfo)serializer.Deserialize(fileStream);
                    fileStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CheckExternBoard()
        {
            if (_comparisonSnap == null)
            {
                _comparisonSnap = Recognizer.GetBoardSnap();
                return;
            }

            Bitmap _currentImage = Recognizer.GetBoardSnap();
            bool areSame = Recognizer.CompareBitmaps(_currentImage, _comparisonSnap);

            if (_updateAfterAnimation && areSame)
            {
                _updateAfterAnimation = false;
                Recognizer.UpdateBoardImage(_currentImage);
                return;
            }

            if (areSame)
            {
                Point[] changedSquares = Recognizer.GetChangedSquares(_currentImage);

                if (changedSquares == null || changedSquares.Length == 0)
                {
                    return;
                }

                Point source = Point.Empty;
                Point destination = Point.Empty;

                if (changedSquares.Length == 4)
                {
                    if (changedSquares[0].X == 4)
                    {
                        source = changedSquares[0];
                        destination = changedSquares[2];

                    }
                    else
                    {
                        source = changedSquares[3];
                        destination = changedSquares[1];
                    }
                }
                else if (changedSquares.Length == 2)
                {
                    ChessPiece piece = _chessPanel.Board[changedSquares[0].Y][changedSquares[0].X];

                    if (piece != null && piece.Owner == _vm.Game.WhoseTurn)
                    {
                        source = changedSquares[0];
                        destination = changedSquares[1];
                    }
                    else
                    {
                        source = changedSquares[1];
                        destination = changedSquares[0];
                    }
                }

                PieceMovedEventArgs args = new PieceMovedEventArgs(source, destination);


                if (_menuItemCheckAuto.Checked)
                {
                    if (PlayMove(args, 'q', true))
                    {
                        _movePlayed = true;
                        GetPlayingEngine().RequestStop();
                    }

                    Recognizer.UpdateBoardImage(_currentImage);
                }
                else
                {
                    _moveOnHold = args;
                }
            }

            _comparisonSnap = _currentImage;
        }

        private void ClearEngines()
        {
            for (int i = 0; i < _vm.Engines.Length; i++)
            {
                if (_vm.Engines[i] != null)
                {
                    _vm.Engines[i].Dispose();
                    _vm.Engines[i] = null;
                }
            }
        }

        private void EmptyEvaluationGrid()
        {
            for (int rowI = 0; rowI < _dataGridViewEvaluation.Rows.Count; rowI++)
            {
                for (int cellI = 0; cellI < _dataGridViewEvaluation.Rows[rowI].Cells.Count; cellI++)
                {
                    _dataGridViewEvaluation.Rows[rowI].Cells[cellI].Value = "";
                }
            }
        }

        private void UpdateUI()
        {
            UpdateWindowText();

            if (_evaluations == null || _evaluations[0] == null)
            {
                return;
            }

            bool isMate;
            int centipawn;
            int mateValue = -1;
            string pawnValue = _evaluations[0][InfoType.Score];
            int depth = int.Parse(_evaluations[0][InfoType.Depth]);

            ProcessPawnValue(pawnValue, out centipawn, out isMate);

            _labelDepth.Text = depth.ToString();

            long tbHits = long.Parse(_evaluations[0][InfoType.TBHits]);

            if (tbHits > 0)
            {
                _labelShowEvaluation.Text = "Score [" + FormatNumber(tbHits) + " TBHits]";
            }
            else
            {
                _labelShowEvaluation.Text = "Score";
            }

            _labelNodes.Text = FormatNumber(long.Parse(_evaluations[0][InfoType.Nodes]));

            _labelTime.Text = GetFormattedEngineInfo(InfoType.Time, _evaluations[0][InfoType.Time]);
            _labelNPS.Text = ToKiloFormat(int.Parse(_evaluations[0][InfoType.NPS]));
            SetExtendedInfoEnabled(true);

            double whitePawnEvaluation;

            if (isMate)
            {
                mateValue = centipawn;

                if (mateValue > 0)
                {
                    _labelEvaluation.Text = $"Mate in {mateValue}";
                    _labelEvaluation.ForeColor = Color.Green;
                }
                else
                {
                    _labelEvaluation.Text = $"Mated in {Math.Abs(mateValue)}";
                    _labelEvaluation.ForeColor = Color.Red;
                }

                if (_vm.Game.WhoseTurn == ChessPlayer.Black && mateValue > 0 || _vm.Game.WhoseTurn == ChessPlayer.White && mateValue < 0)
                {
                    whitePawnEvaluation = -200;
                }
                else
                {
                    whitePawnEvaluation = 200;
                }
            }
            else
            {
                if (_vm.Game.WhoseTurn == ChessPlayer.Black)
                {
                    whitePawnEvaluation = -(centipawn / 100.0);
                }
                else
                {
                    whitePawnEvaluation = centipawn / 100.0;
                }

                if (whitePawnEvaluation == 0)
                {
                    _labelEvaluation.Text = " 0.00";
                }
                else
                {
                    _labelEvaluation.Text = whitePawnEvaluation.ToString("+0.00;-0.00");
                }

                _labelEvaluation.ForeColor = CalculateEvaluationColor(-(centipawn / 100.0));
            }

            if (!_vm.GameFinished)
            {
                if (FillPlyList(depth, whitePawnEvaluation, isMate))
                {
                    UpdateMoveGridCell(isMate, whitePawnEvaluation, depth);
                }
            }

            for (int iPV = 0; iPV < _evaluations.Length; iPV++)
            {
                if (_evaluations[iPV] == null)
                {
                    continue;
                }

                for (int iColumn = 0; iColumn < _columnOrder.Length; iColumn++)
                {
                    _dataGridViewEvaluation.Rows[iPV].Cells[iColumn].Value = GetFormattedEngineInfo(_columnOrder[iColumn], _evaluations[iPV][_columnOrder[iColumn]]);
                }
            }

            if (!_menuItemHideArrows.Checked)
            {
                for (int iPV = 0; iPV < _evaluations.Length; iPV++)
                {
                    if (_evaluations[iPV] == null)
                    {
                        continue;
                    }

                    pawnValue = _evaluations[iPV][InfoType.Score];
                    ProcessPawnValue(pawnValue, out centipawn, out isMate);

                    if (isMate)
                    {
                        centipawn = centipawn * 20 + 12000;
                    }

                    if (iPV == 0 || (!isMate && _mainPvReference - centipawn < CentipawnTolerance) || _chessPanel.Arrows.Count < 3)
                    {
                        if (iPV == 0)
                        {
                            _mainPvReference = centipawn;
                        }

                        if (_chessPanel.Arrows.Count <= iPV)
                        {
                            _chessPanel.Arrows.Add(new MoveArrow((_evaluations[iPV][InfoType.PV]).Substring(0, 4), 0.9, GetReferenceColor(centipawn, _mainPvReference)));
                        }
                        else
                        {
                            _chessPanel.Arrows[iPV].Move = (_evaluations[iPV][InfoType.PV]).Substring(0, 4);
                            _chessPanel.Arrows[iPV].Color = GetReferenceColor(centipawn, _mainPvReference);
                        }

                        if (iPV == 0)
                        {
                            _chessPanel.Arrows[iPV].Color = Color.DarkBlue;
                        }
                    }
                }
            }
        }

        private long GetRemainingTime(int index)
        {
            long time = SerializedInfo.Instance.Level.TotalTime - (int)_playerTimes[index].Elapsed.TotalMilliseconds + ((_vm.PlyList.Count - 1) / 2) * SerializedInfo.Instance.Level.Increment;

            if (time < 1)
            {
                time = 1;
            }

            return time;
        }

        private void UpdateWindowText()
        {
            string text = string.Format("BerldChess Version {0}", Assembly.GetEntryAssembly().GetName().Version.ToString(2));

            if (_pgnImportNames == null)
            {
                for (int i = 0; i < _vm.Engines.Length; i++)
                {
                    if (_vm.Engines[i] == null)
                    {
                        continue;
                    }

                    if (_vm.Engines[i].Name != null)
                    {
                        text += $" | {_vm.Engines[i].Name}";
                    }
                    else
                    {
                        text += $" | {_vm.Engines[i].ExecutablePath}";
                    }
                }
            }
            else
            {
                text += $" | {_pgnImportNames[0]} vs {_pgnImportNames[1]}";
            }

            Text = text;
        }

        private void PlayMoveSound()
        {
            if (_menuIllegalSound.Checked)
            {
                if (_vm.Game.Moves[_vm.Game.Moves.Count - 1].IsCapture)
                {
                    _capturePlayer.Play();
                }
                else if (_vm.Game.Moves[_vm.Game.Moves.Count - 1].Castling != CastlingType.None)
                {
                    _castlingPlayer.Play();
                }
                else
                {
                    _movePlayer.Play();
                }
            }
        }

        private void PlayMove(string move)
        {
            Point[] points = _chessPanel.GetRelPositionsFromMoveString(move);
            PieceMovedEventArgs moveArgs = new PieceMovedEventArgs(points[0], points[1]);

            char? promotion = null;

            if (move.Length > 4 && move[4] != ' ')
            {
                promotion = move[4];
            }

            if (_menuItemCheatMode.Checked)
            {
                if (Recognizer.BoardFound)
                {
                    double pW = Screen.PrimaryScreen.WorkingArea.Width;
                    double pH = Screen.PrimaryScreen.WorkingArea.Height;
                    double fW = Recognizer.BoardSize.Width / 8.0;
                    double fH = Recognizer.BoardSize.Height / 8.0;
                    double max = ushort.MaxValue;
                    Point currCurPos = Cursor.Position;

                    _inputSimulator.Mouse.MoveMouseTo(max + (int)(max * (Recognizer.BoardLocation.X + fW * (points[0].X + 0.45)) / pW), (int)(max * (Recognizer.BoardLocation.Y + fW * (points[0].Y + 0.45)) / pH));
                    _inputSimulator.Mouse.LeftButtonClick();
                    Thread.Sleep(_random.Next((int)(SerializedInfo.Instance.ClickDelay / 2.5), SerializedInfo.Instance.ClickDelay));
                    _inputSimulator.Mouse.MoveMouseTo(max + (int)(max * (Recognizer.BoardLocation.X + fH * (points[1].X + 0.45)) / pW), (int)(max * (Recognizer.BoardLocation.Y + fH * (points[1].Y + 0.45)) / pH));
                    _inputSimulator.Mouse.LeftButtonClick();
                    _inputSimulator.Mouse.MoveMouseTo((int)(max * currCurPos.X / pW), (int)Math.Round((max * currCurPos.Y / pH * 0.97), 0));
                    _inputSimulator.Mouse.LeftButtonClick();
                    Thread.Sleep(20);

                    if (_menuItemCheckAuto.Checked)
                    {
                        _updateAfterAnimation = true;
                    }
                }
            }

            PlayMove(moveArgs, promotion);
        }

        private void PlayShortMove(ilf.pgn.Data.Move move, bool isWhite)
        {
            ChessPiece[][] pieces = _vm.Game.GetBoard();
            string moveString = move.ToString();

            if (move.Type == ilf.pgn.Data.MoveType.CastleKingSide)
            {
                Point position;
                Point newPosition;

                if (isWhite)
                {
                    position = new Point(4, 7);
                    newPosition = new Point(6, 7);
                }
                else
                {
                    position = new Point(4, 0);
                    newPosition = new Point(6, 0);
                }

                PieceMovedEventArgs args = new PieceMovedEventArgs(position, newPosition);

                PlayMove(args);
                return;
            }

            if (move.Type == ilf.pgn.Data.MoveType.CastleQueenSide)
            {
                Point position;
                Point newPosition;

                if (isWhite)
                {
                    position = new Point(4, 7);
                    newPosition = new Point(2, 7);
                }
                else
                {
                    position = new Point(4, 0);
                    newPosition = new Point(2, 0);
                }

                PieceMovedEventArgs args = new PieceMovedEventArgs(position, newPosition);

                PlayMove(args);
                return;
            }

            char searchedPiece = move.Piece.GetFenPieceChar(isWhite);

            int originY = -1;
            int originX = -1;
            bool found = false;

            if (move.OriginRank != null)
            {
                originY = Invert((int)move.OriginRank - 1, 7);
            }

            if (move.OriginFile != null)
            {
                originX = (int)move.OriginFile - 1;
            }

            for (int y = 0; y < pieces.Length; y++)
            {
                if (originY != -1 && y != originY)
                {
                    continue;
                }

                for (int x = 0; x < pieces.Length; x++)
                {
                    if (originX != -1 && x != originX)
                    {
                        continue;
                    }

                    if (pieces[y][x] != null && pieces[y][x].GetFENLetter() == searchedPiece)
                    {
                        Point position = new Point(x, y);
                        Point newPosition = new Point((int)move.TargetSquare.File - 1, Invert(move.TargetSquare.Rank - 1, 7));

                        if (pieces[y][x].IsLegalMove(new Move(new BoardPosition((ChessFile)x, Invert(y, 7) + 1), new BoardPosition(move.TargetSquare.ToString()), isWhite ? ChessPlayer.White : ChessPlayer.Black, move.PromotedPiece.GetPieceChar()), _vm.Game))
                        {
                            PieceMovedEventArgs args = new PieceMovedEventArgs(position, newPosition);

                            PlayMove(args, move.PromotedPiece.GetPieceChar(), silent: true);
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    break;
                }
            }
        }

        private void AddMoveToPlyList(string moveString, string formattedMove)
        {
            if (_vm.NavigationIndex != _vm.PlyList.Count - 1)
            {
                _vm.PlyList.RemoveRange(_vm.NavigationIndex + 1, _vm.PlyList.Count - _vm.NavigationIndex - 1);
            }

            if (_evaluations != null && _evaluations.Length > 0 && _evaluations[0] != null)
            {
                int centipawn;
                bool isMate;
                double whitePawnEvaluation;
                int depth = int.Parse(_evaluations[0][InfoType.Depth]);

                ProcessPawnValue(_evaluations[0][InfoType.Score], out centipawn, out isMate);

                if (isMate)
                {
                    if (_vm.Game.WhoseTurn == ChessPlayer.White && centipawn > 0 || _vm.Game.WhoseTurn == ChessPlayer.Black && centipawn < 0)
                    {
                        whitePawnEvaluation = -200;
                    }
                    else
                    {
                        whitePawnEvaluation = 200;
                    }
                }
                else
                {
                    if (_vm.Game.WhoseTurn == ChessPlayer.White)
                    {
                        whitePawnEvaluation = -(centipawn / 100.0);
                    }
                    else
                    {
                        whitePawnEvaluation = centipawn / 100.0;
                    }
                }

                if (!_vm.GameFinished)
                {
                    if (FillPlyList(depth, whitePawnEvaluation, isMate))
                    {
                        UpdateMoveGridCell(isMate, whitePawnEvaluation, depth);
                    }
                }
            }

            _vm.PlyList.Add(new ChessPly(_vm.Game.GetFen(), 0.0, moveString, formattedMove));
            _vm.NavigationIndex++;
        }

        private void LoadGame(ilf.pgn.Data.Game game)
        {
            if (game.Tags.ContainsKey("FEN"))
            {
                ResetGame(new ChessGame(game.Tags["FEN"]));
            }
            else
            {
                ResetGame(new ChessGame());
            }

            _pgnImportNames = new string[]
            {
                game.WhitePlayer,
                game.BlackPlayer
            };

            for (int i = 0; i < game.MoveText.Count; i++)
            {
                switch (game.MoveText[i].Type)
                {
                    case ilf.pgn.Data.MoveTextEntryType.MovePair:
                        ilf.pgn.Data.MovePairEntry movePair = (ilf.pgn.Data.MovePairEntry)game.MoveText[i];

                        PlayShortMove(movePair.White, true);
                        PlayShortMove(movePair.Black, false);
                        break;

                    case ilf.pgn.Data.MoveTextEntryType.SingleMove:
                        ilf.pgn.Data.HalfMoveEntry singleMove = (ilf.pgn.Data.HalfMoveEntry)game.MoveText[i];

                        bool isWhite = false;

                        if (_vm.Game.WhoseTurn == ChessPlayer.White)
                        {
                            isWhite = true;
                        }

                        PlayShortMove(singleMove.Move, isWhite);
                        break;
                }
            }
        }

        private void ProcessEvaluation(Evaluation evaluation)
        {
            int multiPV;

            if (evaluation.Types.Contains(InfoType.MultiPV))
            {
                multiPV = int.Parse(evaluation[InfoType.MultiPV]);
            }
            else
            {
                multiPV = 1;
            }

            if (multiPV > SerializedInfo.Instance.MultiPv)
            {
                return;
            }

            for (int i = 0; i < _columnOrder.Length; i++)
            {
                if (multiPV - 1 >= 0 && multiPV - 1 < _evaluations.Length)
                    _evaluations[multiPV - 1] = evaluation;
            }
        }

        private void UpdateMoveGridCell(bool isMate, double whitePawnEvaluation, int depth)
        {
            int cellIndex = _vm.NavigationIndex - 1;
            int rowIndex = cellIndex / 2;
            int columnIndex = cellIndex % 2;

            if (cellIndex >= 0 && _dataGridViewMoves.Rows.Count > rowIndex && _dataGridViewMoves.Rows[rowIndex].Cells.Count > columnIndex)
            {
                string formatEvaluation;

                if (isMate)
                {
                    formatEvaluation = "#";
                }
                else if (whitePawnEvaluation == 0)
                {
                    formatEvaluation = "0.00";
                }
                else
                {
                    formatEvaluation = whitePawnEvaluation.ToString("+0.00;-0.00");
                }

                string currentValue = (string)_dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value;

                if (currentValue != null && depth > 0)
                {
                    _dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value = $"{currentValue.Substring(0, currentValue.IndexOf('[') + 1)}{formatEvaluation} D{depth}]";
                }
            }
        }

        private void ProcessPawnValue(string pawnValue, out int centipawn, out bool isMate)
        {
            isMate = false;

            if (!int.TryParse(pawnValue.Substring(3), out centipawn))
            {
                if (!int.TryParse(pawnValue.Substring(3, pawnValue.IndexOf(' ', 3) - 3), out centipawn))
                {
                    if (int.TryParse(pawnValue.Substring(5, pawnValue.IndexOf(' ', 5) - 5), out centipawn))
                    {
                        isMate = true;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid pawn value format.");
                    }
                }
            }
        }

        private void FillEvaluationGrid(int multiPV, Evaluation evaluation)
        {
            for (int iType = 0; iType < evaluation.Types.Length; iType++)
            {
                int columnIndex = -1;

                for (int iOrder = 0; iOrder < _columnOrder.Length; iOrder++)
                {
                    if (_columnOrder[iOrder] == evaluation.Types[iType])
                    {
                        columnIndex = iOrder;
                        break;
                    }
                }

                if (columnIndex != -1)
                {
                    if (_dataGridViewEvaluation.Rows.Count >= multiPV)
                    {
                        _dataGridViewEvaluation.Rows[multiPV - 1].Cells[columnIndex].Value = GetFormattedEngineInfo(evaluation.Types[iType], evaluation[iType]);
                    }
                }
            }
        }

        private void ResetGame(ChessGame newGame)
        {
            bool wasFinished = _vm.GameFinished;

            _evaluationEnabled = false;
            _analyzingMode = true;
            _computerPlayer = ChessPlayer.None;

            _pgnImportNames = null;

            _playerTimes[0].Reset();
            _playerTimes[1].Reset();

            if (_menuItemCheatMode.Checked)
            {
                Recognizer.UpdateBoardImage();
            }

            _vm.Game = newGame;
            _vm.PlyList.Clear();
            _vm.PlyList.Add(new ChessPly(_vm.Game.GetFen()));
            _vm.NavigationIndex = 0;
            _chessPanel.Game = _vm.Game;
            _chessPanel.HighlightedSquares.Clear();
            _chessPanel.ClearIndicators();
            _dataGridViewMoves.Rows.Clear();
            _movePlayed = true;

            ResetEvaluationData(SerializedInfo.Instance.MultiPv);

            if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
            {
                Engine temp = _vm.Engines[0];

                _vm.Engines[0] = _vm.Engines[1];
                _vm.Engines[1] = temp;

                _switched = !_switched;
            }

            for (int i = 0; i < _vm.Engines.Length; i++)
            {
                if (_vm.Engines[i] == null)
                {
                    continue;
                }

                if (!wasFinished)
                {
                    _vm.Engines[i].RequestStop();
                }

                _vm.Engines[i].Query("ucinewgame");
            }

            if (wasFinished)
            {
                OnEngineStopped();
            }

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
            }

            _chessPanel.Invalidate();
        }

        private void SetDarkMode(Control control, bool darkMode)
        {
            List<Control> controls = GetAllChildControls(control).ToList();
            controls.Add(this);

            List<ToolStripMenuItem> menuItems = GetMenuItems(_menuStripMain).ToList();

            for (int i = 0; i < controls.Count; i++)
            {
                if (darkMode)
                {
                    controls[i].BackColor = _darkModeColor;
                    controls[i].ForeColor = Color.White;
                }
                else
                {
                    controls[i].BackColor = (Color)controls[i].Tag;
                    controls[i].ForeColor = Color.Black;
                }

                if (controls[i] is DataGridView)
                {
                    DataGridView grid = (DataGridView)controls[i];

                    if (darkMode)
                    {
                        grid.BorderStyle = BorderStyle.Fixed3D;
                        grid.BackgroundColor = _darkModeColor;
                        grid.DefaultCellStyle.BackColor = _darkModeColor;
                        grid.DefaultCellStyle.ForeColor = Color.White;
                        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        grid.BorderStyle = BorderStyle.FixedSingle;
                        grid.BackgroundColor = Color.White;
                        grid.DefaultCellStyle.BackColor = Color.White;
                        grid.DefaultCellStyle.ForeColor = Color.Black;
                        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
                    }
                }
            }

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (darkMode)
                {
                    menuItems[i].BackColor = _darkModeColor;
                    menuItems[i].ForeColor = Color.White;
                }
                else
                {
                    menuItems[i].BackColor = SystemColors.Control;
                    menuItems[i].ForeColor = Color.Black;
                }
            }
        }

        private void ResetEvaluationData(int multiPV)
        {
            _evaluations = new Evaluation[multiPV];
        }

        private void ResetEvaluationGridRows(int newRowCount)
        {
            _dataGridViewEvaluation.Rows.Clear();

            for (int i = 0; i < newRowCount - 1; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                for (int iCell = 0; iCell < _dataGridViewEvaluation.Columns.Count; iCell++)
                {
                    DataGridViewTextBoxCell textCell = new DataGridViewTextBoxCell();
                    textCell.Value = string.Empty;

                    row.Cells.Add(textCell);
                }

                _dataGridViewEvaluation.Rows.Add(row);
            }
        }

        private void ResetEvaluationGridColumns(InfoType[] columns)
        {
            _dataGridViewEvaluation.Rows.Clear();
            _dataGridViewEvaluation.Columns.Clear();

            for (int i = 0; i < columns.Length; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();

                if (columns[i] == InfoType.PV)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                }

                column.HeaderText = columns[i].ToString();
                _dataGridViewEvaluation.Columns.Add(column);
            }
        }

        private void SetEngineViewElementsVisible(bool visible)
        {
            for (int i = 0; i < _engineViewElements.Length; i++)
            {
                _engineViewElements[i].Visible = visible;
            }
        }

        private void Navigate(int navigation, bool validate = true)
        {
            if (validate)
            {
                if (navigation == _vm.NavigationIndex || navigation >= _vm.PlyList.Count)
                {
                    return;
                }
            }

            bool wasFinished = _vm.GameFinished;
            _evaluationEnabled = false;
            _analyzingMode = true;

            _computerPlayer = ChessPlayer.None;

            _vm.NavigationIndex = navigation;

            _vm.Game = new ChessGame(_vm.CurrentPly.Fen);

            _chessPanel.Game = _vm.Game;
            _chessPanel.HighlightedSquares.Clear();

            string currentMoveString = _vm.CurrentPly.Move;

            if (!string.IsNullOrEmpty(currentMoveString))
            {
                Point[] squareLocations = _chessPanel.GetRelPositionsFromMoveString(currentMoveString);
                _chessPanel.HighlightedSquares.AddRange(squareLocations);
            }

            SelectMovesCell(navigation);

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _vm.NavigationIndex % 2 == 1;
                _chessPanel.Invalidate();
            }

            _movePlayed = true;

            for (int i = 0; i < _vm.Engines.Length; i++)
            {
                if (_vm.Engines[i] == null)
                {
                    continue;
                }

                if (wasFinished)
                {
                    OnEngineStopped();
                }
                else
                {
                    _vm.Engines[i].RequestStop();
                }
            }
        }

        private void SelectMovesCell(int navIndex)
        {
            for (int i = 0; i < _dataGridViewMoves.SelectedCells.Count; i++)
            {
                _dataGridViewMoves.SelectedCells[i].Selected = false;
            }

            navIndex--;

            if (navIndex < 0)
            {
                if (_dataGridViewMoves.Rows.Count > 0 && _dataGridViewMoves.Rows[0].Cells.Count > 0)
                {
                    _dataGridViewMoves.CurrentCell = _dataGridViewMoves.Rows[0].Cells[0];
                }

                return;
            }
            else if (navIndex + 1 >= _vm.PlyList.Count)
            {
                navIndex = _vm.PlyList.Count - 2;
            }

            _dataGridViewMoves.Rows[navIndex / 2].Cells[navIndex % 2].Selected = true;
            _dataGridViewMoves.CurrentCell = _dataGridViewMoves.Rows[navIndex / 2].Cells[navIndex % 2];
        }

        private void UpdateMoveGrid(string formattedMove)
        {
            int cellCount = _dataGridViewMoves.GetCellCount(DataGridViewElementStates.None);
            int plyCount = _vm.PlyList.Count;

            for (int i = cellCount; i >= plyCount; i--)
            {
                if (i % 2 == 0)
                {
                    _dataGridViewMoves.Rows.RemoveAt(i / 2 - 1);
                }
                else
                {
                    _dataGridViewMoves.Rows[i / 2 - 1].Cells[1].Value = "";
                }
            }

            if (plyCount % 2 == 0)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell whiteMove = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell blackMove = new DataGridViewTextBoxCell();

                whiteMove.Value = (plyCount / 2) + ". " + formattedMove;

                if (GetPlayingEngine() != null)
                {
                    whiteMove.Value += "\n[D0]";
                }

                row.Cells.Add(whiteMove);
                row.Cells.Add(blackMove);

                row.Height = 40;
                _dataGridViewMoves.Rows.Add(row);
            }
            else
            {
                _dataGridViewMoves.Rows[plyCount / 2 - 1].Cells[1].Value = formattedMove;

                if (GetPlayingEngine() != null)
                {
                    _dataGridViewMoves.Rows[plyCount / 2 - 1].Cells[1].Value += "\n[D0]";
                }
            }

            SelectMovesCell(_vm.NavigationIndex);
        }

        private void AddMenuItems(ToolStripMenuItem menuItem, List<ToolStripMenuItem> menuItems)
        {
            menuItems.Add(menuItem);

            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    AddMenuItems((ToolStripMenuItem)item, menuItems);
                }
            }
        }

        private void SetExtendedInfoEnabled(bool active)
        {
            _tableLayoutPanelEvalInfos.Visible = active;
        }

        private bool FillPlyList(int depth, double whitePawnEvaluation, bool isMate)
        {
            if (_vm.PlyList.Count > 0 && _vm.PlyList.Count > _vm.NavigationIndex)
            {
                if (depth >= _vm.CurrentPly.EvaluationDepth || isMate)
                {
                    _vm.CurrentPly.Evaluation = whitePawnEvaluation;
                    _vm.CurrentPly.EvaluationDepth = depth;

                    return true;
                }
            }

            return false;
        }

        private bool PlayMove(PieceMovedEventArgs args, char? promotion = null, bool cheatMove = false, bool silent = false)
        {
            BoardPosition sourcePosition = new BoardPosition((ChessFile)args.Position.X, Invert(args.Position.Y - 1, 7));
            BoardPosition destinationPosition = new BoardPosition((ChessFile)args.NewPosition.X, Invert(args.NewPosition.Y - 1, 7));
            ChessPiece movingPiece = _vm.Game.GetPieceAt(sourcePosition);

            if (movingPiece == null)
            {
                return false;
            }

            if (promotion == null)
            {
                promotion = CheckPromotion(sourcePosition, destinationPosition, movingPiece);
            }

            Move move = new Move(sourcePosition, destinationPosition, movingPiece.Owner, promotion);

            if (!_vm.Game.IsValidMove(move))
            {
                if (_menuItemIllegalSound.Checked && !cheatMove && !silent && _menuIllegalSound.Checked)
                {
                    _illegalPlayer.Play();
                }

                return false;
            }

            if (cheatMove)
            {
                Recognizer.UpdateBoardImage();
            }

            _evaluationEnabled = false;

            ReadOnlyCollection<Move> validMoves = _vm.Game.GetValidMoves(_vm.Game.WhoseTurn);
            MoveType moveType = _vm.Game.ApplyMove(move, true);

            if (SerializedInfo.Instance.Level.SelectedLevelType == LevelType.TotalTime)
            {
                if (_vm.Game.WhoseTurn == ChessPlayer.White)
                {
                    _playerTimes[0].Start();
                    _playerTimes[1].Stop();
                }
                else
                {
                    _playerTimes[1].Start();
                    _playerTimes[0].Stop();
                }
            }

            if (!silent)
            {
                PlayMoveSound();
            }

            _chessPanel.ClearIndicators();

            _chessPanel.HighlightedSquares.Clear();
            _chessPanel.HighlightedSquares.Add(args.Position);
            _chessPanel.HighlightedSquares.Add(args.NewPosition);

            string formattedMove = GetFormattedMove(move, moveType, validMoves);
            AddMoveToPlyList(move.ToString(""), formattedMove);
            UpdateMoveGrid(formattedMove);

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                _vm.LatestPly.Evaluation = _vm.Game.IsCheckmated(ChessPlayer.White) ? -120 : 120;
            }

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = !_chessPanel.IsFlipped;
                _chessPanel.Invalidate();
            }

            for (int i = 0; i < _evaluations.Length; i++)
            {
                _evaluations[i] = null;
            }

            _chessPanel.Invalidate();
            _movePlayed = true;

            return true;
        }

        private bool SetupEngine(EngineSetting engineSetting, int index, bool startAnalysis)
        {
            Engine engine = null;

            if (SerializedInfo.Instance.EngineList.SettingAvailable)
            {
                if (File.Exists(engineSetting.ExecutablePath))
                {
                    engine = new Engine(engineSetting.ExecutablePath);

                    for (int i = 0; i < engineSetting.Arguments.Length; i++)
                    {
                        engine.Query(engineSetting.Arguments[i]);
                    }
                }
            }

            if (engine != null)
            {
                engine.EvaluationReceived += OnEvaluationReceived;
                engine.BestMoveFound += OnBestMoveFound;
                engine.Query($"setoption name MultiPv value {SerializedInfo.Instance.MultiPv}");
                engine.Query("ucinewgame");

                if (startAnalysis)
                {
                    engine.Query($"position fen {_vm.Game.GetFen()}");
                    engine.Query("go infinite");
                }

                _vm.Engines[index] = engine;

                return true;
            }
            else
            {
                return false;
            }
        }

        private char CheckPromotion(BoardPosition source, BoardPosition destination, ChessPiece movingPiece)
        {
            if (_vm.Game.WhoseTurn != _computerPlayer)
            {
                if (movingPiece is Pawn &&
                    source.Rank == (movingPiece.Owner == ChessPlayer.White ? 7 : 2) &&
                    destination.Rank == (movingPiece.Owner == ChessPlayer.White ? 8 : 1))
                {
                    FormPromotion dialog = new FormPromotion(movingPiece.Owner);
                    dialog.ShowDialog();

                    return dialog.PromotionCharacter;
                }
            }

            return 'q';
        }

        private int Round(double number)
        {
            return (int)Math.Round(number, 0);
        }

        private int Invert(int value, int range)
        {
            return Math.Abs(value - range);
        }

        private string GetFormattedMove(Move move, MoveType moveType, ReadOnlyCollection<Move> legalMoves)
        {
            ChessPiece piece = _vm.Game.GetPieceAt(move.NewPosition);
            char pieceCharacter = piece.GetFENLetter().ToString().ToUpperInvariant()[0];
            string formatMove = "";

            if (moveType == (MoveType.Castling | MoveType.Move))
            {
                if (move.NewPosition.File == ChessFile.G)
                {
                    formatMove = "O-O";
                }
                else
                {
                    formatMove = "O-O-O";
                }

                return formatMove;
            }

            if (pieceCharacter != 'P' && moveType != (MoveType.Move | MoveType.Promotion) && moveType != (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += pieceCharacter;

                for (int i = 0; i < legalMoves.Count; i++)
                {
                    ChessPiece legalPiece = _vm.Game.GetPieceAt(legalMoves[i].OriginalPosition);

                    if (legalMoves[i].NewPosition.Equals(move.NewPosition) &&
                        !legalMoves[i].OriginalPosition.Equals(move.OriginalPosition) &&
                        legalPiece.GetFENLetter() == piece.GetFENLetter())
                    {

                        if (legalMoves[i].OriginalPosition.File != move.OriginalPosition.File)
                        {
                            formatMove += move.OriginalPosition.File.ToString().ToLowerInvariant();
                        }
                        else
                        {
                            formatMove += move.OriginalPosition.Rank.ToString();
                        }
                    }
                }
            }
            else if (moveType == (MoveType.Move | MoveType.Capture) || moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += move.OriginalPosition.File.ToString().ToLowerInvariant();
            }

            if (moveType == (MoveType.Move | MoveType.Capture) || moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += 'x';
            }

            formatMove += move.NewPosition.ToString();

            if (moveType == (MoveType.Move | MoveType.Promotion) || moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += "=" + ((char)move.Promotion).ToString().ToUpperInvariant();
            }

            if (_vm.Game.IsCheckmated(ChessPlayer.Black) || _vm.Game.IsCheckmated(ChessPlayer.White))
            {
                formatMove += '#';
            }
            else if (_vm.Game.IsInCheck(ChessPlayer.Black) || _vm.Game.IsInCheck(ChessPlayer.White))
            {
                formatMove += '+';
            }

            return formatMove;
        }

        private string ToKiloFormat(long number)
        {
            if (number >= 10000)
            {
                return (number / 1000D).ToString("#,##0") + " k";
            }

            return number.ToString("#");
        }

        private string GetFormattedEngineInfo(InfoType type, string data)
        {
            switch (type)
            {
                case InfoType.Time:
                    return TimeSpan.FromMilliseconds(int.Parse(data)).ToString(@"m\:ss");

                case InfoType.Nodes:
                case InfoType.NPS:
                case InfoType.TBHits:
                    return ToKiloFormat(long.Parse(data));
            }

            return data;
        }

        private string GetFormattedDrawReason(DrawReason drawReason)
        {
            switch (drawReason)
            {
                case DrawReason.Repetition:
                    return "Threefold Repetition";
                case DrawReason.FiftyMoveRule:
                    return "Fifty-Move Rule";
                case DrawReason.Stalemate:
                    return "Stalemate";
                case DrawReason.InsufficientMaterial:
                    return "Insufficient Material";
            }

            return null;
        }

        private string FormatNumber(long number)
        {
            if (number >= 100000000000)
                return (number / 1000000000).ToString("#,0 B");

            if (number >= 10000000000)
                return (number / 1000000000).ToString("0.#") + " B";

            if (number >= 100000000)
                return (number / 1000000).ToString("#,0 M");

            if (number >= 10000000)
                return (number / 1000000).ToString("0.#") + " M";

            if (number >= 100000)
                return (number / 1000).ToString("#,0 k");

            if (number >= 10000)
                return (number / 1000).ToString("0.#") + " k";

            return number.ToString("#,0");
        }

        private Color GetReferenceColor(int centiPawn, int reference)
        {
            if (centiPawn > reference)
            {
                centiPawn = reference;
            }

            int difference = reference - centiPawn;
            double relativeDiff = difference / 45.0;

            if (relativeDiff > 2)
            {
                relativeDiff = 2;
            }

            int red = 0;
            int green = 255;

            if (relativeDiff - 1 < 0)
            {
                red += (int)(relativeDiff * 255.0);
            }
            else
            {
                red = 255;
                green -= (int)((relativeDiff - 1) * 255.0);
            }

            return Color.FromArgb(red, green, 0);
        }

        private Color CalculateEvaluationColor(double evaluation)
        {
            double range = 3;

            if (evaluation > range)
            {
                evaluation = range;
            }
            else if (evaluation < -range)
            {
                evaluation = -range;
            }

            double fraction = evaluation / range * 0.5;
            double x = 0.5 + fraction;

            if (SerializedInfo.Instance.DarkMode)
            {
                return Color.FromArgb(Math.Max(((int)(x * 255) + 70), 255), Math.Max(((int)((1 - x) * 255) + 70), 255), 0);
            }
            else
            {
                return Color.FromArgb((int)(x * 255), (int)((1 - x) * 255), 0);
            }
        }

        private Engine GetPlayingEngine()
        {
            if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
            {
                return _vm.Engines[(int)_vm.Game.WhoseTurn];
            }
            else
            {
                return _vm.Engines[0];
            }
        }

        private IEnumerable<Control> GetAllChildControls(Control control)
        {
            IEnumerable<Control> controls = control.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllChildControls(c)).Concat(controls);
        }

        private List<ToolStripMenuItem> GetMenuItems(MenuStrip menuStrip)
        {
            List<ToolStripMenuItem> myItems = new List<ToolStripMenuItem>();

            foreach (var menuItem in menuStrip.Items)
            {
                if (menuItem is ToolStripMenuItem)
                {
                    AddMenuItems((ToolStripMenuItem)menuItem, myItems);
                }
            }

            return myItems;
        }

        #endregion

        private void SetLevel()
        {
            switch (SerializedInfo.Instance.Level.SelectedLevelType)
            {
                case LevelType.FixedDepth:
                case LevelType.TimePerMove:
                case LevelType.Infinite:
                case LevelType.Nodes:

                    for (int i = 0; i < _playerTimes.Length; i++)
                    {
                        _playerTimes[i].Reset();
                    }

                    _menuItemWhiteTime.Visible = false;
                    _menuItemBlackTime.Visible = false;
                    break;

                case LevelType.TotalTime:

                    _menuItemWhiteTime.Visible = true;
                    _menuItemBlackTime.Visible = true;
                    break;
            }
        }

        private void OnMenuItemLevelClick(object sender, EventArgs e)
        {
            var levelDialog = new FormLevelDialog(SerializedInfo.Instance.Level);
            if (levelDialog.ShowDialog() != DialogResult.OK) return;

            SerializedInfo.Instance.Level = levelDialog.Level;
            SetLevel();
        }

        private void OnMenuItemTimeAnalysisClick(object sender, EventArgs e)
        {
            if (GetPlayingEngine() == null)
            {
                return;
            }

            var input = Interaction.InputBox("Enter Time per ply in ms:", "BerldChess - Time Analysis");
            int time;

            if (int.TryParse(input, out time))
            {
                StartTimeAnalysis(time);
            }
        }
    }
}