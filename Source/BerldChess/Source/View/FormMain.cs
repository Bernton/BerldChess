using BerldChess.Model;
using BerldChess.Properties;
using BerldChess.ViewModel;
using ChessDotNet;
using ChessDotNet.Pieces;
using ChessEngineInterface;
using ilf.pgn.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using WindowsInput;
using Color = System.Drawing.Color;
using File = System.IO.File;
using Move = ilf.pgn.Data.Move;
using MoveType = ChessDotNet.MoveType;

namespace BerldChess.View
{
    public partial class FormMain : Form
    {
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
                Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare,
                    SerializedInfo.Instance.EngineDarkSquare);
            }
        }

        #endregion

        private void OnMenuItemLevelClick(object sender, EventArgs e)
        {
            var levelDialog = new FormLevelDialog(SerializedInfo.Instance.Level);
            if (levelDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

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

            if (int.TryParse(input, out var time))
            {
                StartTimeAnalysis(time);
            }
        }

        private void SetLevel()
        {
            switch (SerializedInfo.Instance.Level.SelectedLevelType)
            {
                case LevelType.FixedDepth:
                case LevelType.TimePerMove:
                case LevelType.Infinite:
                case LevelType.Nodes:

                    foreach (var playerTime in _playerTimes)
                    {
                        playerTime.Reset();
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

        #region Fields

        private bool _totalChartView;
        private volatile bool _evaluationEnabled = true;
        private volatile bool _updateAfterAnimation;
        private volatile bool _analyzingMode = true;
        private volatile bool _movePlayed = true;
        private int _animationTime = 300;
        private int _draws;
        private const int CentipawnTolerance = 100;
        private const int ChartShownPlies = 25;
        private const double DefaultSizeFactor = 0.91;
        private ChessPlayer _computerPlayer = ChessPlayer.None;
        private InfoType[] _columnOrder;
        private readonly Random _random = new Random();
        private readonly Color _lastMoveArrowColor = Color.FromArgb(100, 30, 10, 200);
        private readonly Color _darkModeColor = Color.FromArgb(49, 46, 43);
        private readonly Color _selectionColor = Color.FromArgb(160, 177, 199, 208);
        private Bitmap _comparisonSnap;
        private ChessPanel _chessPanel;
        private readonly Stopwatch[] _playerTimes = { new Stopwatch(), new Stopwatch() };
        private SoundEngine _soundEngine;
        private readonly InputSimulator _inputSimulator = new InputSimulator();
        private PieceMovedEventArgs _moveOnHold;
        private CancellationTokenSource _analysisTaskTokenSource;
        private readonly FormMainViewModel _vm;
        private FormPieceSettings _pieceDialog;
        private FormBoardSetting _boardDialog;
        private FormEngineSettings _engineDialog;
        private int[] _engineWins = new int[2];
        private string[] _pgnImportNames;
        private readonly string[] _enginePaths = new string[2];
        private volatile Evaluation[] _evaluations;
        private Control[] _engineViewElements;
        private bool _switched;
        private readonly List<MoveArrow> _engineEvalArrows = new List<MoveArrow>();
        private MoveArrow _lastMoveArrow;

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
                    _engineEvalArrows.Clear();

                    break;

                case EngineMode.Analysis:

                    SetEngineViewElementsVisible(SetupEngine(SerializedInfo.Instance.EngineList.SelectedSetting, 0,
                        true));
                    _evaluationEnabled = true;
                    break;

                case EngineMode.Competitive:

                    _switched = false;
                    _engineWins = new int[2];
                    _draws = 0;

                    var isValidEngine = SetupEngine(SerializedInfo.Instance.EngineList.SelectedSetting, 0, false);

                    if (SerializedInfo.Instance.EngineList.SelectedSetting != null)
                    {
                        _enginePaths[0] = SerializedInfo.Instance.EngineList.SelectedSetting.ExecutablePath;
                    }

                    if (!SetupEngine(SerializedInfo.Instance.EngineList.SelectedOpponentSetting, 1, false))
                    {
                        isValidEngine = false;
                    }

                    if (SerializedInfo.Instance.EngineList.SelectedOpponentSetting != null)
                    {
                        _enginePaths[1] = SerializedInfo.Instance.EngineList.SelectedOpponentSetting.ExecutablePath;
                    }

                    if (isValidEngine)
                    {
                        _vm.EngineInfos[(int)_vm.Game.WhoseTurn].Engine.Query($"position fen {_vm.Game.GetFen()}");
                        _vm.EngineInfos[(int)_vm.Game.WhoseTurn].Engine.Query("go infinite");
                    }

                    SetEngineViewElementsVisible(isValidEngine);

                    if (!isValidEngine)
                    {
                        ClearEngines();
                    }

                    _evaluationEnabled = true;
                    break;
            }

            _chessPanel.Invalidate();
        }

        private void InitializeChessBoard()
        {
            _chessPanel = new ChessPanel
            {
                Cursor = Cursors.Hand,
                BackColor = SystemColors.Control,
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = new Size(787, 403),
                Game = _vm.Game
            };

            _chessPanel.Select();
            _chessPanel.PieceMoved += OnPieceMoved;
            _splitContainerBoard.Panel1.Controls.Add(_chessPanel);
        }

        private void InitializeEvaluationGrid()
        {
            _columnOrder = new[]
            {
                InfoType.MultiPV,
                InfoType.Depth,
                InfoType.Score,
                InfoType.Time,
                InfoType.Nodes,
                InfoType.NPS,
                InfoType.TBHits,
                InfoType.PV
            };

            ResetEvaluationGridColumns(_columnOrder);
            ResetEvaluationData(SerializedInfo.Instance.MultiPv);
            ResetEvaluationGridRows(SerializedInfo.Instance.MultiPv);
        }

        private void InitializeFormAndControls()
        {
            Icon = Resources.PawnRush;
            Text = $@"BerldChess Version {Assembly.GetEntryAssembly().GetName().Version.ToString(2)}";

            _soundEngine = new SoundEngine();
            _soundEngine.Load();

            Tag = BackColor;

            foreach (var control in GetAllChildControls(this))
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
            _dataGridViewMoves.DefaultCellStyle.Font = new Font(_dataGridViewMoves.DefaultCellStyle.Font.FontFamily,
                _dataGridViewMoves.Width / 22.5F);
        }

        private void OnBestMoveFound(string move)
        {
            if (!_movePlayed)
            {
                try
                {
                    Invoke((MethodInvoker)delegate { PlayMove(move); });
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

            _engineEvalArrows.Clear();
            _chessPanel.Invalidate();

            var isFinishedPosition = false;
            string evaluationText = null;
            DrawReason drawReason = 0;

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    if (!_switched && _vm.Game.WhoseTurn == ChessPlayer.White ||
                        _switched && _vm.Game.WhoseTurn == ChessPlayer.Black)
                    {
                        _engineWins[1]++;
                    }
                    else
                    {
                        _engineWins[0]++;
                    }
                }

                evaluationText = "Checkmate";
                isFinishedPosition = true;
            }
            else if (_vm.Game.IsStalemated(_vm.Game.WhoseTurn))
            {
                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    _draws++;
                }

                evaluationText = "Stalemate";
                isFinishedPosition = true;
            }
            else if (_vm.Game.CheckIfDraw(ref drawReason))
            {
                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    _draws++;
                }

                evaluationText = GetFormattedDrawReason(drawReason);
                isFinishedPosition = true;
            }

            if (isFinishedPosition)
            {
                foreach (var playerTime in _playerTimes)
                {
                    playerTime.Stop();
                }

                SetExtendedInfoEnabled(false);
                _labelEvaluation.Text = evaluationText;
                _labelEvaluation.ForeColor = _menuItemNew.ForeColor;

                if (SerializedInfo.Instance.EngineMode != EngineMode.Competitive)
                {
                    return;
                }

                ResetGame(new ChessGame());
                OnMenuItemAutoPlayClick(null, null);
            }
            else
            {
                var engineIndex = 0;

                if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
                {
                    var onTurn = _vm.Game.WhoseTurn;
                    engineIndex = (int)onTurn;
                }

                _vm.EngineInfos[engineIndex].Engine.Query($"position fen {_vm.Game.GetFen()}");

                if (_analyzingMode && (_computerPlayer == ChessPlayer.None || _computerPlayer != _vm.Game.WhoseTurn))
                {
                    _vm.EngineInfos[engineIndex].Engine.Query("go infinite");
                }
                else
                {
                    switch (SerializedInfo.Instance.Level.SelectedLevelType)
                    {
                        case LevelType.FixedDepth:
                            _vm.EngineInfos[engineIndex].Engine.Query($"go depth {SerializedInfo.Instance.Level.Plies}");
                            break;

                        case LevelType.TimePerMove:
                            _vm.EngineInfos[engineIndex].Engine.Query($"go movetime {SerializedInfo.Instance.Level.TimePerMove}");
                            break;

                        case LevelType.TotalTime:
                            var increment = SerializedInfo.Instance.Level.Increment;
                            _vm.EngineInfos[engineIndex]
                                .Engine.Query(
                                    $"go wtime {GetRemainingTime(0)} btime {GetRemainingTime(1)} winc {increment} binc {increment}");
                            break;

                        case LevelType.Infinite:
                            _vm.EngineInfos[engineIndex].Engine.Query("go infinite");
                            break;

                        case LevelType.Nodes:
                            _vm.EngineInfos[engineIndex].Engine.Query($"go nodes {SerializedInfo.Instance.Level.Nodes}");
                            break;
                    }
                }

                _movePlayed = false;
                _evaluationEnabled = true;
            }
        }

        private void OnDialogFontSelected()
        {
            var currentFont = SerializedInfo.Instance.SelectedChessFont;
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

            ApplyLastMoveHighlight();

            if (SerializedInfo.Instance.UseImages)
            {
                TryApplyBoardImages();
            }

            _chessPanel.InvalidateRender();
            SetDarkMode(this, SerializedInfo.Instance.DarkMode);
        }

        private void AddMoveArrow(string move)
        {
            _lastMoveArrow = new MoveArrow(move, 2.2, _lastMoveArrowColor, false, false);
            UpdateArrows();
        }

        private void AddLastMoveArrow()
        {
            var currentMoveString = _vm.CurrentPly.Move;

            if (!string.IsNullOrEmpty(currentMoveString))
            {
                AddMoveArrow(currentMoveString);
            }
        }

        private void RemoveLastMoveArrow()
        {
            if (_lastMoveArrow != null)
            {
                _chessPanel.Arrows.Remove(_lastMoveArrow);
            }

            _lastMoveArrow = null;
        }

        private void ApplyLastMoveHighlight()
        {
            if (SerializedInfo.Instance.ArrowHighlight)
            {
                _chessPanel.HighlightedSquares.Clear();
                AddLastMoveArrow();
            }
            else
            {
                RemoveLastMoveArrow();

                if (_chessPanel.HighlightedSquares.Count == 0)
                {
                    var currentMoveString = _vm.CurrentPly.Move;

                    if (!string.IsNullOrEmpty(currentMoveString))
                    {
                        var squareLocations = _chessPanel.GetRelPositionsFromMoveString(currentMoveString);
                        _chessPanel.HighlightedSquares.AddRange(squareLocations);
                    }
                }
            }

            if (SerializedInfo.Instance.NoHighlight)
            {
                _chessPanel.NoHighlight = true;
            }
            else if (SerializedInfo.Instance.BorderHighlight)
            {
                _chessPanel.NoHighlight = false;
                _chessPanel.BorderHighlight = true;
            }
            else
            {
                _chessPanel.NoHighlight = false;
                _chessPanel.BorderHighlight = false;
            }
        }

        private void TryApplyBoardImages()
        {
            var lightSquareImage = TryGetImage(SerializedInfo.Instance.LightSquarePath);
            var darkSquareImage = TryGetImage(SerializedInfo.Instance.DarkSquarePath);

            _chessPanel.DarkSquareImage = darkSquareImage;
            _chessPanel.LightSquareImage = lightSquareImage;
        }

        private static Image TryGetImage(string path)
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
            var playingEngine = GetPlayingEngine();

            if (PlayMove(e))
            {
                playingEngine?.RequestStop();
            }
        }

        private void OnFormMainLoad(object sender, EventArgs e)
        {
            _menuItemMultiPv.Text = $@"MultiPv [{SerializedInfo.Instance.MultiPv}]";
            _menuItemAnimationTime.Text = $@"Animation Time [{_animationTime}]";
            _menuItemClickDelay.Text = $@"Click Delay [{SerializedInfo.Instance.ClickDelay}]";

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
                UpdateUi();
            }

            _panelEvaluationChart.Invalidate();
            UpdateArrows();
            _chessPanel.Invalidate();
        }

        private void UpdateArrows()
        {
            _chessPanel.Arrows.Clear();

            if (!_menuItemHideArrows.Checked)
            {
                _chessPanel.Arrows.AddRange(_engineEvalArrows);
            }

            if (_lastMoveArrow != null)
            {
                _chessPanel.Arrows.Add(_lastMoveArrow);
            }
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
            var input = Interaction.InputBox("Enter Position FEN:", "BerldChess - FEN");

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
                MessageBox.Show(ex.Message, @"Berld Chess", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
            if (_menuItemLocalMode.Checked)
            {
                return;
            }

            _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
            _chessPanel.Invalidate();
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
                _chessPanel.IsFlipped = _vm.Game.WhoseTurn != ChessPlayer.White;
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
            if (GetPlayingEngine() == null)
            {
                return;
            }

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

        private void OnMenuItemAutoPlayClick(object sender, EventArgs e)
        {
            if (GetPlayingEngine() == null)
            {
                return;
            }

            _analyzingMode = !_analyzingMode;
            _computerPlayer = _vm.Game.WhoseTurn;

            _movePlayed = true;
            GetPlayingEngine().RequestStop();
        }

        private void OnMenuItemAutoMoveClick(object sender, EventArgs e)
        {
            if (_moveOnHold == null)
            {
                return;
            }

            var currentImage = Recognizer.GetBoardSnap();

            if (PlayMove(_moveOnHold, 'q', true))
            {
                _movePlayed = true;
                GetPlayingEngine().RequestStop();
            }

            Recognizer.UpdateBoardImage(currentImage);

            _moveOnHold = null;
        }

        private void OnMenuItemUpdateClick(object sender, EventArgs e)
        {
            if (!Recognizer.BoardFound)
            {
                if (!Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare,
                    SerializedInfo.Instance.EngineDarkSquare))
                {
                    MessageBox.Show(@"No board found!", @"BerldChess", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            else
            {
                Recognizer.UpdateBoardImage();
            }
        }

        private void OnMenuItemSquareColorsClick(object sender, EventArgs e)
        {
            var images = new Bitmap[Screen.AllScreens.Length];

            for (var i = 0; i < images.Length; i++)
            {
                images[i] = Recognizer.GetScreenshot(Screen.AllScreens[i]);
            }

            var squareColorDialog = new FormSquareColor(images);
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
            if (!Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare,
                SerializedInfo.Instance.EngineDarkSquare))
            {
                MessageBox.Show(@"No board found!", @"BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var input = Interaction.InputBox("Enter MultiPv:", "BerldChess - MultiPv");

            foreach (var engineInfo in _vm.EngineInfos)
            {
                if (engineInfo == null)
                {
                    continue;
                }

                if (!int.TryParse(input, out var multiPv))
                {
                    continue;
                }

                if (multiPv <= 0 || multiPv > 250)
                {
                    continue;
                }

                _evaluationEnabled = false;
                _movePlayed = true;

                SerializedInfo.Instance.MultiPv = multiPv;
                _menuItemMultiPv.Text = $@"MultiPv [{SerializedInfo.Instance.MultiPv}]";
                engineInfo.Engine.Query($"setoption name MultiPv value {SerializedInfo.Instance.MultiPv}");
                engineInfo.Engine.RequestStop();
                _chessPanel.Invalidate();

                ResetEvaluationData(multiPv);
                ResetEvaluationGridRows(SerializedInfo.Instance.MultiPv);
                _evaluationEnabled = true;
            }
        }

        private void OnMenuItemDepthAnalysisClick(object sender, EventArgs e)
        {
            if (GetPlayingEngine() == null)
            {
                return;
            }

            var input = Interaction.InputBox("Enter Depth:", "BerldChess - Depth Analysis");

            if (int.TryParse(input, out var depth))
            {
                StartDepthAnalysis(depth);
            }
        }

        private void StartTimeAnalysis(int milliseconds)
        {
            if (SerializedInfo.Instance.EngineMode == EngineMode.Disabled || !_labelEvaluation.Visible)
            {
                return;
            }

            if (_vm.PlyList.Count <= 0)
            {
                return;
            }

            var dialog = new ProgressDialog($"Time analysis ({milliseconds} ms/ply)", "Analyzing...");
            dialog.ClosedByInterface += OnDepthAnalysisCancel;

            _analysisTaskTokenSource?.Cancel();
            _analysisTaskTokenSource = new CancellationTokenSource();

            var analysisTask = new Task(() =>
            {
                for (var i = _vm.NavigationIndex; i < _vm.PlyList.Count; i++)
                {
                    var uiI = i;

                    Invoke((MethodInvoker)delegate
                   {
                       Navigate(uiI);
                       dialog.StatusText = $"Analyzing ply {uiI}...";
                       dialog.ProgressBarPercentage = (int)Math.Ceiling((double)uiI / _vm.PlyList.Count * 100.0);
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

            if (_vm.PlyList.Count <= 0)
            {
                return;
            }

            var dialog = new ProgressDialog($"Analysis to depth {depth}", "Analyzing...");
            dialog.ClosedByInterface += OnDepthAnalysisCancel;

            _analysisTaskTokenSource?.Cancel();
            _analysisTaskTokenSource = new CancellationTokenSource();

            var analysisTask = new Task(() =>
            {
                for (var i = _vm.NavigationIndex; i < _vm.PlyList.Count; i++)
                {
                    var uiI = i;

                    Invoke((MethodInvoker)delegate
                   {
                       Navigate(uiI);
                       dialog.StatusText = $"Analyzing ply {uiI}...";
                       dialog.ProgressBarPercentage = (int)Math.Ceiling((double)uiI / _vm.PlyList.Count * 100.0);
                   });

                    while (_vm.PlyList[i].EvaluationDepth < depth && !_vm.GameFinished)
                    {
                        Thread.Sleep(10);

                        if (!_analysisTaskTokenSource.Token.IsCancellationRequested)
                        {
                            continue;
                        }

                        Invoke((MethodInvoker)delegate { dialog.Hide(); });
                        return;
                    }
                }

                Invoke((MethodInvoker)delegate { dialog.Hide(); });
            }, _analysisTaskTokenSource.Token);

            analysisTask.Start();
            dialog.Show();
        }

        private void OnDepthAnalysisCancel(object sender, EventArgs e)
        {
            _analysisTaskTokenSource.Cancel();
        }

        private void OnMenuItemCheatModeCheckedChanged(object sender, EventArgs e)
        {
            foreach (ToolStripDropDownItem item in _menuItemCheatMode.DropDownItems)
            {
                item.Enabled = _menuItemCheatMode.Checked;
            }
        }

        private void OnMenuItemAnimationTimeClick(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Enter Animation Time:", "BerldChess - Animation Time");

            if (!int.TryParse(input, out var animTime))
            {
                return;
            }

            _animationTime = animTime;
            _menuItemAnimationTime.Text = $@"Anim Time [{_animationTime}]";
        }

        private void OnMenuItemCopyFenClick(object sender, EventArgs e)
        {
            Clipboard.SetText(_vm.CurrentPly.Fen);
        }

        private void OnMenuItemClickDelayClick(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Enter Click Delay:", "BerldChess - Click Delay");

            if (!int.TryParse(input, out var clickDelay))
            {
                return;
            }

            SerializedInfo.Instance.ClickDelay = clickDelay;
            _menuItemClickDelay.Text = $@"Click Delay [{clickDelay}]";
        }

        private void OnMenuItemLoadPgnClick(object sender, EventArgs e)
        {
            var pgnLoader = new FormPgnLoader(SerializedInfo.Instance.PgnAnalysis,
                SerializedInfo.Instance.PgnAnalysisDepth);

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
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.Default;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            var gridPen = new Pen(_panelEvaluationChart.ForeColor.AlterRgb(200, !SerializedInfo.Instance.DarkMode), 1);
            var middleLinePen =
                new Pen(_panelEvaluationChart.ForeColor.AlterRgb(120, !SerializedInfo.Instance.DarkMode), 1);
            var chartBorderPen = new Pen(_panelEvaluationChart.ForeColor, 1);

            var labelFont = new Font("Segoe UI", 11);

            var fontBrush = new SolidBrush(_panelEvaluationChart.ForeColor);
            var chartLightBrush = new SolidBrush(Color.White);
            var chartDarkBrush = new SolidBrush(Color.FromArgb(69, 66, 63));
            var selectionBrush = new SolidBrush(_selectionColor);

            int peak;
            var chartYMiddle = (int)(_panelEvaluationChart.Height / 2.0);

            var positions = _vm.PlyList.ToList();

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

            var rowCount = peak * 2;
            const int lineCount = 6;

            var xPart = _panelEvaluationChart.Width / (double)lineCount;
            var yPart = _panelEvaluationChart.Height / (double)rowCount;

            for (var i = 1; i < rowCount; i++)
            {
                g.DrawLine(gridPen, 0, (float)(yPart * i), _panelEvaluationChart.Width, (float)(yPart * i));
            }

            for (var i = 1; i < lineCount; i++)
            {
                g.DrawLine(gridPen, (float)(xPart * i), 0, (float)(xPart * i), _panelEvaluationChart.Height);
            }

            var lineWidth = (float)_panelEvaluationChart.Width / lineCount;
            var yCapHeight = _panelEvaluationChart.Height - labelFont.Size * 2.25F;

            for (var i = 1; i <= lineCount; i++)
            {
                if (i % 2 == 1)
                {
                    continue;
                }

                var valueX = (int)((positions.Count - 1) / (double)lineCount * i);

                if (!_totalChartView && _vm.PlyList.Count > ChartShownPlies)
                {
                    valueX += _vm.PlyList.Count - ChartShownPlies;
                }

                var text = valueX.ToString();
                var size = g.MeasureString(text, labelFont);
                size.Width *= 2.25F;

                g.DrawString(text, labelFont, fontBrush, lineWidth * i - size.Width / 2, yCapHeight);
            }

            for (var i = 1; i < rowCount; i++)
            {
                if (peak > 5 && i % 2 == 1)
                {
                    continue;
                }

                if (i == rowCount / 2)
                {
                    continue;
                }

                var valueY = peak - i;
                var text = valueY.ToString("+0;-0");
                var offSet = g.MeasureString(text, labelFont).Height / 1.99F;

                g.DrawString(text, labelFont, fontBrush, labelFont.Size / 2,
                    (int)(_panelEvaluationChart.Height / (double)rowCount * i) - offSet);
            }

            g.DrawLine(middleLinePen, 0, chartYMiddle, _panelEvaluationChart.Width, chartYMiddle);

            var yUnitLength = _panelEvaluationChart.Height / (double)rowCount;
            var xUnitLength = (double)(_panelEvaluationChart.Width - 3) / positions.Count;

            if (peak != 0)
            {
                for (var i = 0; i < positions.Count; i++)
                {
                    var evaluation = positions[i].Evaluation;

                    var x = (float)(i * xUnitLength);
                    int y;

                    var height = Round(Math.Abs(evaluation * yUnitLength));
                    var width = (float)((i + 1) * xUnitLength - i * xUnitLength);

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
            var navigationIndex = e.RowIndex * 2 + e.ColumnIndex + 1;

            if (navigationIndex >= _vm.PlyList.Count || navigationIndex == _vm.NavigationIndex)
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
            var source = (Control)sender;
            source.FitFont(0.9, 0.9);
        }

        private void OnLabelEvaluationTextChanged(object sender, EventArgs e)
        {
            _labelEvaluation.FitFont(0.9, 0.98);
        }

        private string GetFormattedEngineScore()
        {
            EngineList engineList = SerializedInfo.Instance.EngineList;

            string playerName = engineList.SelectedSetting.Name;
            string opponentName = engineList.SelectedOpponentSetting.Name;

            int playerWins = _engineWins[0];
            int opponentWins = _engineWins[1];

            int draws = _draws;
            int games = playerWins + opponentWins + draws;

            return
                $"{playerName}: {playerWins} ({ToFormattedPercentage(playerWins, games)})\n\n" +
                $"{opponentName}: {opponentWins} ({ToFormattedPercentage(opponentWins, games)})\n\n" +
                $"Draws: {draws} ({ToFormattedPercentage(draws, games)})";
        }

        private string ToFormattedPercentage(int value, int divisor)
        {
            if (divisor == 0)
            {
                return "0%";
            }

            return $"{Math.Round(value / (double)divisor * 100, 1)}%";
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V)
            {
                string formattedEngineWinDrawLoss = GetFormattedEngineScore();

                MessageBox.Show(formattedEngineWinDrawLoss, "BerldChess - Score");
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.T)
            {
                var plies = "";

                for (var i = 0; i < _vm.PlyList.Count; i++)
                {
                    plies += i + 1 + " " + _vm.PlyList[i].Evaluation + " D" + _vm.PlyList[i].EvaluationDepth + " | ";
                }

                MessageBox.Show(plies, @"BerldChess - Plies");
            }

            foreach (var menuItem in GetMenuItems(_menuStripMain))
            {
                if (string.IsNullOrEmpty(menuItem.ShortcutKeyDisplayString))
                {
                    continue;
                }

                var key = (Keys)Enum.Parse(typeof(Keys), menuItem.ShortcutKeyDisplayString);

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

            ApplyLastMoveHighlight();

            _menuItemHideOutput.Checked = SerializedInfo.Instance.HideOutput;
            _menuItemHideArrows.Checked = SerializedInfo.Instance.HideArrows;
            _menuItemFilterArrows.Checked = SerializedInfo.Instance.FilterArrows;

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

            _pieceDialog = new FormPieceSettings(SerializedInfo.Instance.ChessFonts,
                SerializedInfo.Instance.SelectedFontIndex);
            _pieceDialog.FontSelected += OnDialogFontSelected;

            _boardDialog = new FormBoardSetting();
            _boardDialog.BoardSettingAltered += OnBoardSettingAltered;

            _chessPanel.Invalidate();
        }

        private void SerializeInfo()
        {
            try
            {
                SerializedInfo.Instance.Bounds = WindowState == FormWindowState.Maximized ? RestoreBounds : Bounds;
                SerializedInfo.Instance.SplitterDistance = _splitContainerMain.SplitterDistance;
                SerializedInfo.Instance.IsMaximized = WindowState == FormWindowState.Maximized;
                SerializedInfo.Instance.BoardFlipped = _menuItemFlipBoard.Checked;
                SerializedInfo.Instance.HideArrows = _menuItemHideArrows.Checked;
                SerializedInfo.Instance.FilterArrows = _menuItemFilterArrows.Checked;
                SerializedInfo.Instance.HideOutput = _menuItemHideOutput.Checked;
                SerializedInfo.Instance.LocalMode = _menuItemLocalMode.Checked;
                SerializedInfo.Instance.CheatMode = _menuItemCheatMode.Checked;
                SerializedInfo.Instance.Sound = _menuIllegalSound.Checked;
                SerializedInfo.Instance.AutoCheck = _menuItemCheckAuto.Checked;
                SerializedInfo.Instance.AnimationTime = _animationTime;
                SerializedInfo.Instance.DisplayLegalMoves = _menuItemDisplayLegalMoves.Checked;
                SerializedInfo.Instance.IllegalSound = _menuItemIllegalSound.Checked;
                SerializedInfo.Instance.DisplayCoordinates = _menuItemDisplayCoordinates.Checked;

                var selectedSetting = SerializedInfo.Instance.EngineList.SelectedSetting;
                var selectedOpponentSetting = SerializedInfo.Instance.EngineList.SelectedOpponentSetting;

                SerializedInfo.Instance.EngineList.Settings =
                    SerializedInfo.Instance.EngineList.Settings.OrderBy(c => c.Name).ToList();

                SerializedInfo.Instance.EngineList.SelectedIndex =
                    SerializedInfo.Instance.EngineList.Settings.IndexOf(selectedSetting);
                SerializedInfo.Instance.EngineList.SelectedOpponentIndex =
                    SerializedInfo.Instance.EngineList.Settings.IndexOf(selectedOpponentSetting);

                if (SerializedInfo.Instance.ChessFonts.Count > 3)
                {
                    var fontDefault = SerializedInfo.Instance.ChessFonts[0];
                    var fontSimple = SerializedInfo.Instance.ChessFonts[1];
                    var fontPixel = SerializedInfo.Instance.ChessFonts[2];

                    var selectedFont = SerializedInfo.Instance.SelectedChessFont;

                    SerializedInfo.Instance.ChessFonts.RemoveRange(0, 3);

                    SerializedInfo.Instance.ChessFonts =
                        SerializedInfo.Instance.ChessFonts.OrderBy(c => c.Name).ToList();

                    SerializedInfo.Instance.ChessFonts.Insert(0, fontPixel);
                    SerializedInfo.Instance.ChessFonts.Insert(0, fontSimple);
                    SerializedInfo.Instance.ChessFonts.Insert(0, fontDefault);

                    SerializedInfo.Instance.SelectedFontIndex =
                        SerializedInfo.Instance.ChessFonts.IndexOf(selectedFont);
                }

                var serializer = new XmlSerializer(typeof(SerializedInfo));
                var fileStream = new FileStream(FormMainViewModel.ConfigurationFileName, FileMode.Create);
                serializer.Serialize(fileStream, SerializedInfo.Instance);
                fileStream.Dispose();

                SerializedInfo.Instance.Bounds = WindowState == FormWindowState.Maximized ? RestoreBounds : Bounds;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static void DeserializeInfo()
        {
            try
            {
                if (!File.Exists(FormMainViewModel.ConfigurationFileName))
                {
                    return;
                }

                var serializer = new XmlSerializer(typeof(SerializedInfo));
                var fileStream = new FileStream(FormMainViewModel.ConfigurationFileName, FileMode.Open);

                SerializedInfo.Instance = (SerializedInfo)serializer.Deserialize(fileStream);
                fileStream.Dispose();
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

            var currentImage = Recognizer.GetBoardSnap();
            var areSame = Recognizer.CompareBitmaps(currentImage, _comparisonSnap);

            if (_updateAfterAnimation && areSame)
            {
                _updateAfterAnimation = false;
                Recognizer.UpdateBoardImage(currentImage);
                return;
            }

            if (areSame)
            {
                var changedSquares = Recognizer.GetChangedSquares(currentImage);

                if (changedSquares == null || changedSquares.Length == 0)
                {
                    return;
                }

                var source = Point.Empty;
                var destination = Point.Empty;

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
                    var piece = _chessPanel.Board[changedSquares[0].Y][changedSquares[0].X];

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

                var args = new PieceMovedEventArgs(source, destination);


                if (_menuItemCheckAuto.Checked)
                {
                    if (PlayMove(args, 'q', true))
                    {
                        _movePlayed = true;
                        GetPlayingEngine().RequestStop();
                    }

                    Recognizer.UpdateBoardImage(currentImage);
                }
                else
                {
                    _moveOnHold = args;
                }
            }

            _comparisonSnap = currentImage;
        }

        private void ClearEngines()
        {
            for (var i = 0; i < _vm.EngineInfos.Length; i++)
            {
                if (_vm.EngineInfos[i] != null)
                {
                    _vm.EngineInfos[i].Engine.Dispose();
                    _vm.EngineInfos[i] = null;
                }
            }
        }

        private void EmptyEvaluationGrid()
        {
            for (var rowI = 0; rowI < _dataGridViewEvaluation.Rows.Count; rowI++)
            {
                for (var cellI = 0; cellI < _dataGridViewEvaluation.Rows[rowI].Cells.Count; cellI++)
                {
                    _dataGridViewEvaluation.Rows[rowI].Cells[cellI].Value = "";
                }
            }
        }

        private void UpdateUi()
        {
            UpdateWindowText();

            if (_evaluations?[0] == null)
            {
                return;
            }

            var pawnValue = _evaluations[0][InfoType.Score];
            var depth = int.Parse(_evaluations[0][InfoType.Depth]);

            ProcessPawnValue(pawnValue, out var centipawn, out var isMate);

            _labelDepth.Text = depth.ToString();

            var tbHits = long.Parse(_evaluations[0][InfoType.TBHits]);

            if (tbHits > 0)
            {
                _labelShowEvaluation.Text = @"Score [" + FormatNumber(tbHits) + @" TBHits]";
            }
            else
            {
                _labelShowEvaluation.Text = @"Score";
            }

            _labelNodes.Text = FormatNumber(long.Parse(_evaluations[0][InfoType.Nodes]));

            _labelTime.Text = GetFormattedEngineInfo(InfoType.Time, _evaluations[0][InfoType.Time]);
            _labelNPS.Text = ToKiloFormat(int.Parse(_evaluations[0][InfoType.NPS]));
            SetExtendedInfoEnabled(true);

            double whitePawnEvaluation;

            if (isMate)
            {
                var mateValue = centipawn;

                if (mateValue > 0)
                {
                    _labelEvaluation.Text = $@"Mate in {mateValue}";
                    _labelEvaluation.ForeColor = Color.Green;
                }
                else
                {
                    _labelEvaluation.Text = $@"Mated in {Math.Abs(mateValue)}";
                    _labelEvaluation.ForeColor = Color.Red;
                }

                if (_vm.Game.WhoseTurn == ChessPlayer.Black && mateValue > 0 ||
                    _vm.Game.WhoseTurn == ChessPlayer.White && mateValue < 0)
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

                _labelEvaluation.Text = Math.Abs(whitePawnEvaluation) < 0.01
                    ? " 0.00"
                    : whitePawnEvaluation.ToString("+0.00;-0.00");
                _labelEvaluation.ForeColor = CalculateEvaluationColor(-(centipawn / 100.0));
            }

            if (!_vm.GameFinished)
            {
                if (FillPlyList(depth, whitePawnEvaluation, isMate))
                {
                    UpdateMoveGridCell(isMate, whitePawnEvaluation, depth);
                }
            }

            for (var iPv = 0; iPv < _evaluations.Length; iPv++)
            {
                if (_evaluations[iPv] == null)
                {
                    continue;
                }

                for (var iColumn = 0; iColumn < _columnOrder.Length; iColumn++)
                {
                    _dataGridViewEvaluation.Rows[iPv].Cells[iColumn].Value =
                        GetFormattedEngineInfo(_columnOrder[iColumn], _evaluations[iPv][_columnOrder[iColumn]]);
                }
            }

            if (_menuItemHideArrows.Checked)
            {
                return;
            }

            UpdateEngineArrows();
        }

        private void UpdateEngineArrows()
        {
            if (_evaluations is null || _evaluations.Length == 0)
            {
                return;
            }

            _engineEvalArrows.Clear();

            int mainPvValue = default;
            bool mainPvIsMate = default;

            for (var iPv = 0; iPv < _evaluations.Length; iPv++)
            {
                Evaluation pvEvaluation = _evaluations[iPv];

                if (pvEvaluation == null)
                {
                    continue;
                }

                bool isMainPv = iPv == 0;
                string pawnValue = _evaluations[iPv][InfoType.Score];
                ProcessPawnValue(pawnValue, out var value, out var isMate);

                if (isMainPv)
                {
                    mainPvValue = value;
                    mainPvIsMate = isMate;
                }

                double distanceFromReference = GetDistanceFromReference(value, isMate, mainPvValue, mainPvIsMate);

                if (_menuItemFilterArrows.Checked &&
                    distanceFromReference > 2.0)
                {
                    continue;
                }

                string move = _evaluations[iPv][InfoType.PV].Substring(0, 4);
                Color color = isMainPv ? Color.DarkBlue : GetGreenRedGradient(1 - distanceFromReference);
                _engineEvalArrows.Add(new MoveArrow(move, 0.9, color));
            }
        }

        private static double GetDistanceFromReference(int value, bool isMate, int referenceValue, bool referenceIsMate)
        {
            double distanceFromReference;

            if (referenceIsMate && referenceValue >= 0)
            {
                if (isMate && value >= 0)
                {
                    distanceFromReference = 0.4 * (value - referenceValue);
                }
                else
                {
                    distanceFromReference = int.MaxValue;
                }
            }
            else if (referenceIsMate && referenceValue < 0)
            {
                distanceFromReference = 0.4 * (value - referenceValue);
            }
            else if (isMate)
            {
                distanceFromReference = int.MaxValue;
            }
            else
            {
                distanceFromReference = (referenceValue - value) / 50.0;
            }

            return distanceFromReference;
        }

        private static Color GetGreenRedGradient(double value)
        {
            if (Math.Abs(value) > 1)
            {
                value = Math.Sign(value);
            }

            int red;
            int green;

            if (value >= 0)
            {
                red = Round(255 * (1 - value));
                green = 255;
            }
            else
            {
                red = 255;
                green = Round(255 * (1 + value));
            }

            return Color.FromArgb(red, green, 0);
        }

        private long GetRemainingTime(int index)
        {
            var time = SerializedInfo.Instance.Level.TotalTime - (int)_playerTimes[index].Elapsed.TotalMilliseconds +
                       (_vm.PlyList.Count - 1) / 2 * SerializedInfo.Instance.Level.Increment;

            if (time < 1)
            {
                time = 1;
            }

            return time;
        }

        private void UpdateWindowText()
        {
            var text = $"BerldChess Version {Assembly.GetEntryAssembly().GetName().Version.ToString(2)}";

            if (_pgnImportNames == null)
            {
                foreach (var engineInfo in _vm.EngineInfos)
                {
                    if (engineInfo == null)
                    {
                        continue;
                    }

                    text += $" | {engineInfo.Setting.Name}";
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
                    _soundEngine.PlayCapture();
                }
                else if (_vm.Game.Moves[_vm.Game.Moves.Count - 1].Castling != CastlingType.None)
                {
                    _soundEngine.PlayCastling();
                }
                else
                {
                    _soundEngine.PlayMove();
                }
            }
        }

        private void PlayMove(string move)
        {
            var points = _chessPanel.GetRelPositionsFromMoveString(move);
            var moveArgs = new PieceMovedEventArgs(points[0], points[1]);

            char? promotion = null;

            if (move.Length > 4 && move[4] != ' ')
            {
                promotion = move[4];
            }

            if (_menuItemCheatMode.Checked)
            {
                if (Recognizer.BoardFound)
                {
                    Point initialCursorPosition = Cursor.Position;
                    Screen screen = Screen.AllScreens[Recognizer.ScreenIndex];

                    double screenWidth = screen.WorkingArea.Width;
                    double screenHeight = screen.WorkingArea.Height;
                    double fieldWidth = Recognizer.BoardSize.Width / 8.0;
                    double fieldHeight = Recognizer.BoardSize.Height / 8.0;
                    double boardX = screen.Bounds.Left + Recognizer.BoardLocation.X;
                    double boardY = screen.Bounds.Top + Recognizer.BoardLocation.Y;
                    double max = ushort.MaxValue;

                    double moveX = max * ((boardX + fieldWidth * (points[0].X + 0.45)) / screenWidth);
                    double moveY = max * ((boardY + fieldHeight * (points[0].Y + 0.45)) / screenHeight);
                    _inputSimulator.Mouse.MoveMouseTo(moveX, moveY);
                    _inputSimulator.Mouse.LeftButtonClick();

                    int clickDelay = SerializedInfo.Instance.ClickDelay;
                    Thread.Sleep(_random.Next((int)(clickDelay / 2.5), clickDelay));

                    moveX = max * ((boardX + fieldWidth * (points[1].X + 0.45)) / screenWidth);
                    moveY = max * ((boardY + fieldHeight * (points[1].Y + 0.45)) / screenHeight);
                    _inputSimulator.Mouse.MoveMouseTo(moveX, moveY);
                    _inputSimulator.Mouse.LeftButtonClick();

                    moveX = (int)(max * initialCursorPosition.X / screenWidth);
                    moveY = (int)Math.Round(max * initialCursorPosition.Y / screenHeight * 0.97, 0);
                    _inputSimulator.Mouse.MoveMouseTo(moveX, moveY);
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

        private void PlayShortMove(Move move, bool isWhite)
        {
            var pieces = _vm.Game.GetBoard();

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

                var args = new PieceMovedEventArgs(position, newPosition);

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

                var args = new PieceMovedEventArgs(position, newPosition);

                PlayMove(args);
                return;
            }

            var searchedPiece = move.Piece.GetFenPieceChar(isWhite);

            var originY = -1;
            var originX = -1;
            var found = false;

            if (move.OriginRank != null)
            {
                originY = Invert((int)move.OriginRank - 1, 7);
            }

            if (move.OriginFile != null)
            {
                originX = (int)move.OriginFile - 1;
            }

            for (var y = 0; y < pieces.Length; y++)
            {
                if (originY != -1 && y != originY)
                {
                    continue;
                }

                for (var x = 0; x < pieces.Length; x++)
                {
                    if (originX != -1 && x != originX)
                    {
                        continue;
                    }

                    if (pieces[y][x] == null || pieces[y][x].GetFENLetter() != searchedPiece)
                    {
                        continue;
                    }

                    var position = new Point(x, y);
                    var newPosition = new Point((int)move.TargetSquare.File - 1,
                        Invert(move.TargetSquare.Rank - 1, 7));

                    if (!pieces[y][x].IsLegalMove(
                        new ChessDotNet.Move(new BoardPosition((ChessFile)x, Invert(y, 7) + 1),
                            new BoardPosition(move.TargetSquare.ToString()),
                            isWhite ? ChessPlayer.White : ChessPlayer.Black, move.PromotedPiece.GetPieceChar()),
                        _vm.Game))
                    {
                        continue;
                    }

                    var args = new PieceMovedEventArgs(position, newPosition);

                    PlayMove(args, move.PromotedPiece.GetPieceChar(), silent: true);
                    found = true;
                    break;
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
                double whitePawnEvaluation;
                var depth = int.Parse(_evaluations[0][InfoType.Depth]);

                ProcessPawnValue(_evaluations[0][InfoType.Score], out var centipawn, out var isMate);

                if (isMate)
                {
                    if (_vm.Game.WhoseTurn == ChessPlayer.White && centipawn > 0 ||
                        _vm.Game.WhoseTurn == ChessPlayer.Black && centipawn < 0)
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

        private void LoadGame(Game game)
        {
            ResetGame(game.Tags.ContainsKey("FEN") ? new ChessGame(game.Tags["FEN"]) : new ChessGame());

            _pgnImportNames = new[]
            {
                game.WhitePlayer,
                game.BlackPlayer
            };

            foreach (var moveText in game.MoveText)
            {
                switch (moveText.Type)
                {
                    case MoveTextEntryType.MovePair:
                        var movePair = (MovePairEntry)moveText;

                        PlayShortMove(movePair.White, true);
                        PlayShortMove(movePair.Black, false);
                        break;

                    case MoveTextEntryType.SingleMove:
                        var singleMove = (HalfMoveEntry)moveText;

                        var isWhite = _vm.Game.WhoseTurn == ChessPlayer.White;

                        PlayShortMove(singleMove.Move, isWhite);
                        break;
                }
            }
        }

        private void ProcessEvaluation(Evaluation evaluation)
        {
            var multiPv = evaluation.Types.Contains(InfoType.MultiPV) ? int.Parse(evaluation[InfoType.MultiPV]) : 1;

            if (multiPv > SerializedInfo.Instance.MultiPv)
            {
                return;
            }

            for (var i = 0; i < _columnOrder.Length; i++)
            {
                if (multiPv - 1 >= 0 && multiPv - 1 < _evaluations.Length)
                {
                    _evaluations[multiPv - 1] = evaluation;
                }
            }
        }

        private void UpdateMoveGridCell(bool isMate, double whitePawnEvaluation, int depth)
        {
            var cellIndex = _vm.NavigationIndex - 1;
            var rowIndex = cellIndex / 2;
            var columnIndex = cellIndex % 2;

            if (cellIndex < 0 || _dataGridViewMoves.Rows.Count <= rowIndex ||
                _dataGridViewMoves.Rows[rowIndex].Cells.Count <= columnIndex)
            {
                return;
            }

            string formatEvaluation;

            if (isMate)
            {
                formatEvaluation = "#";
            }
            else if (Math.Abs(whitePawnEvaluation) < 0.01)
            {
                formatEvaluation = "0.00";
            }
            else
            {
                formatEvaluation = whitePawnEvaluation.ToString("+0.00;-0.00");
            }

            var currentValue = (string)_dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value;

            if (currentValue != null && depth > 0)
            {
                _dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value =
                    $"{currentValue.Substring(0, currentValue.IndexOf('[') + 1)}{formatEvaluation} D{depth}]";
            }
        }

        private static void ProcessPawnValue(string pawnValue, out int centipawn, out bool isMate)
        {
            isMate = false;

            if (int.TryParse(pawnValue.Substring(3), out centipawn))
            {
                return;
            }

            if (int.TryParse(pawnValue.Substring(3, pawnValue.IndexOf(' ', 3) - 3), out centipawn))
            {
                return;
            }

            if (int.TryParse(pawnValue.Substring(5, pawnValue.IndexOf(' ', 5) - 5), out centipawn))
            {
                isMate = true;
            }
            else
            {
                throw new ArgumentException("Invalid pawn value format.");
            }
        }

        private void ResetGame(ChessGame newGame)
        {
            var wasFinished = _vm.GameFinished;

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
            RemoveLastMoveArrow();
            _chessPanel.ClearIndicators();
            _dataGridViewMoves.Rows.Clear();
            _movePlayed = true;

            ResetEvaluationData(SerializedInfo.Instance.MultiPv);

            if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
            {
                var temp = _vm.EngineInfos[0];

                _vm.EngineInfos[0] = _vm.EngineInfos[1];
                _vm.EngineInfos[1] = temp;

                _switched = !_switched;
            }

            foreach (var engineInfo in _vm.EngineInfos)
            {
                if (engineInfo == null)
                {
                    continue;
                }

                if (!wasFinished)
                {
                    engineInfo.Engine.RequestStop();
                }

                engineInfo.Engine.Query("ucinewgame");
            }

            if (wasFinished)
            {
                OnEngineStopped();
            }

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _vm.Game.WhoseTurn != ChessPlayer.White;
            }
            else
            {
                _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
            }

            _chessPanel.Invalidate();
        }

        private void SetDarkMode(Control control, bool darkMode)
        {
            var controls = GetAllChildControls(control).ToList();
            controls.Add(this);

            var menuItems = GetMenuItems(_menuStripMain).ToList();

            foreach (var child in controls)
            {
                if (darkMode)
                {
                    child.BackColor = _darkModeColor;
                    child.ForeColor = Color.White;
                }
                else
                {
                    child.BackColor = (Color)child.Tag;
                    child.ForeColor = Color.Black;
                }

                if (!(child is DataGridView))
                {
                    continue;
                }

                var grid = (DataGridView)child;

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

            foreach (var menuItem in menuItems)
            {
                if (darkMode)
                {
                    menuItem.BackColor = _darkModeColor;
                    menuItem.ForeColor = Color.White;
                }
                else
                {
                    menuItem.BackColor = SystemColors.Control;
                    menuItem.ForeColor = Color.Black;
                }
            }
        }

        private void ResetEvaluationData(int multiPv)
        {
            _evaluations = new Evaluation[multiPv];
        }

        private void ResetEvaluationGridRows(int newRowCount)
        {
            _dataGridViewEvaluation.Rows.Clear();

            for (var i = 0; i < newRowCount - 1; i++)
            {
                var row = new DataGridViewRow();

                for (var iCell = 0; iCell < _dataGridViewEvaluation.Columns.Count; iCell++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = string.Empty });
                }

                _dataGridViewEvaluation.Rows.Add(row);
            }
        }

        private void ResetEvaluationGridColumns(InfoType[] infoTypes)
        {
            _dataGridViewEvaluation.Rows.Clear();
            _dataGridViewEvaluation.Columns.Clear();

            foreach (var infoType in infoTypes)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    AutoSizeMode = infoType == InfoType.PV
                        ? DataGridViewAutoSizeColumnMode.Fill
                        : DataGridViewAutoSizeColumnMode.NotSet,
                    HeaderText = infoType.ToString()
                };


                _dataGridViewEvaluation.Columns.Add(column);
            }
        }

        private void SetEngineViewElementsVisible(bool visible)
        {
            foreach (var engineViewElement in _engineViewElements)
            {
                engineViewElement.Visible = visible;
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

            var wasFinished = _vm.GameFinished;
            _evaluationEnabled = false;

            for (var i = 0; i < _evaluations.Length; i++)
            {
                _evaluations[i] = null;
            }

            _analyzingMode = true;

            _computerPlayer = ChessPlayer.None;

            _vm.NavigationIndex = navigation;

            _vm.Game = new ChessGame(_vm.CurrentPly.Fen);

            _chessPanel.Game = _vm.Game;
            _chessPanel.HighlightedSquares.Clear();
            RemoveLastMoveArrow();

            var currentMoveString = _vm.CurrentPly.Move;

            if (!string.IsNullOrEmpty(currentMoveString))
            {
                if (SerializedInfo.Instance.ArrowHighlight)
                {
                    AddMoveArrow(currentMoveString);
                }
                else
                {
                    var squareLocations = _chessPanel.GetRelPositionsFromMoveString(currentMoveString);
                    _chessPanel.HighlightedSquares.AddRange(squareLocations);
                }
            }

            SelectMovesCell(navigation);

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _vm.NavigationIndex % 2 == 1;
                _chessPanel.Invalidate();
            }

            _movePlayed = true;

            foreach (var engineInfo in _vm.EngineInfos)
            {
                if (engineInfo == null)
                {
                    continue;
                }

                if (wasFinished)
                {
                    OnEngineStopped();
                }
                else
                {
                    engineInfo.Engine.RequestStop();
                }
            }
        }

        private void SelectMovesCell(int navIndex)
        {
            for (var i = 0; i < _dataGridViewMoves.SelectedCells.Count; i++)
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

            if (navIndex + 1 >= _vm.PlyList.Count)
            {
                navIndex = _vm.PlyList.Count - 2;
            }

            _dataGridViewMoves.Rows[navIndex / 2].Cells[navIndex % 2].Selected = true;
            _dataGridViewMoves.CurrentCell = _dataGridViewMoves.Rows[navIndex / 2].Cells[navIndex % 2];
        }

        private void UpdateMoveGrid(string formattedMove)
        {
            var cellCount = _dataGridViewMoves.GetCellCount(DataGridViewElementStates.None);
            var plyCount = _vm.PlyList.Count;

            for (var i = cellCount; i >= plyCount; i--)
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
                var row = new DataGridViewRow();
                var whiteMove = new DataGridViewTextBoxCell();
                var blackMove = new DataGridViewTextBoxCell();

                whiteMove.Value = plyCount / 2 + ". " + formattedMove;

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

        private static void AddMenuItems(ToolStripMenuItem menuItem, ICollection<ToolStripMenuItem> menuItems)
        {
            menuItems.Add(menuItem);

            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem stripMenuItem)
                {
                    AddMenuItems(stripMenuItem, menuItems);
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

        private bool PlayMove(PieceMovedEventArgs args, char? promotion = null, bool cheatMove = false,
            bool silent = false)
        {
            var sourcePosition = new BoardPosition((ChessFile)args.Position.X, Invert(args.Position.Y - 1, 7));
            var destinationPosition =
                new BoardPosition((ChessFile)args.NewPosition.X, Invert(args.NewPosition.Y - 1, 7));
            var movingPiece = _vm.Game.GetPieceAt(sourcePosition);

            if (movingPiece == null)
            {
                return false;
            }

            if (promotion == null)
            {
                promotion = CheckPromotion(sourcePosition, destinationPosition, movingPiece);
            }

            var move = new ChessDotNet.Move(sourcePosition, destinationPosition, movingPiece.Owner, promotion);

            if (!_vm.Game.IsValidMove(move))
            {
                if (_menuItemIllegalSound.Checked && !cheatMove && !silent && _menuIllegalSound.Checked)
                {
                    _soundEngine.PlayIllegal();
                }

                return false;
            }

            if (cheatMove)
            {
                Recognizer.UpdateBoardImage();
            }

            _evaluationEnabled = false;
            _engineEvalArrows.Clear();

            var validMoves = _vm.Game.GetValidMoves(_vm.Game.WhoseTurn);
            var moveType = _vm.Game.ApplyMove(move, true);

            if (SerializedInfo.Instance.Level.SelectedLevelType == LevelType.TotalTime)
            {
                if (_vm.Game.WhoseTurn == ChessPlayer.White)
                {
                    _playerTimes[0].Start();
                    _playerTimes[1].Stop();
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

            if (SerializedInfo.Instance.ArrowHighlight)
            {
                RemoveLastMoveArrow();
                AddMoveArrow(move.ToString(""));
            }
            else
            {
                _chessPanel.HighlightedSquares.Clear();
                _chessPanel.HighlightedSquares.Add(args.Position);
                _chessPanel.HighlightedSquares.Add(args.NewPosition);
            }

            var formattedMove = GetFormattedMove(move, moveType, validMoves);
            AddMoveToPlyList(move.ToString(""), formattedMove);
            UpdateMoveGrid(formattedMove);

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                _vm.LatestPly.Evaluation = _vm.Game.IsCheckmated(ChessPlayer.White) ? -120 : 120;
            }

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = !_chessPanel.IsFlipped;
            }

            for (var i = 0; i < _evaluations.Length; i++)
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

                    foreach (var argument in engineSetting.Arguments)
                    {
                        engine.Query(argument);
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

                _vm.EngineInfos[index] = new Source.Model.EngineInfo()
                {
                    Engine = engine,
                    Setting = engineSetting
                };

                return true;
            }

            return false;
        }

        private char CheckPromotion(BoardPosition source, BoardPosition destination, ChessPiece movingPiece)
        {
            if (_vm.Game.WhoseTurn != _computerPlayer)
            {
                if (movingPiece is Pawn &&
                    source.Rank == (movingPiece.Owner == ChessPlayer.White ? 7 : 2) &&
                    destination.Rank == (movingPiece.Owner == ChessPlayer.White ? 8 : 1))
                {
                    var dialog = new FormPromotion(movingPiece.Owner);
                    dialog.ShowDialog();

                    return dialog.PromotionCharacter;
                }
            }

            return 'q';
        }

        private static int Round(double number)
        {
            return (int)Math.Round(number, 0);
        }

        private int Invert(int value, int range)
        {
            return Math.Abs(value - range);
        }

        private string GetFormattedMove(ChessDotNet.Move move, MoveType moveType,
            IReadOnlyList<ChessDotNet.Move> legalMoves)
        {
            var piece = _vm.Game.GetPieceAt(move.NewPosition);
            var pieceCharacter = piece.GetFENLetter().ToString().ToUpperInvariant()[0];
            var formatMove = "";

            if (moveType == (MoveType.Castling | MoveType.Move))
            {
                formatMove = move.NewPosition.File == ChessFile.G ? "O-O" : "O-O-O";
                return formatMove;
            }

            if (pieceCharacter != 'P' && moveType != (MoveType.Move | MoveType.Promotion) &&
                moveType != (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += pieceCharacter;

                foreach (var legalMove in legalMoves)
                {
                    var legalPiece = _vm.Game.GetPieceAt(legalMove.OriginalPosition);

                    if (!legalMove.NewPosition.Equals(move.NewPosition) ||
                        legalMove.OriginalPosition.Equals(move.OriginalPosition) ||
                        legalPiece.GetFENLetter() != piece.GetFENLetter())
                    {
                        continue;
                    }

                    if (legalMove.OriginalPosition.File != move.OriginalPosition.File)
                    {
                        formatMove += move.OriginalPosition.File.ToString().ToLowerInvariant();
                    }
                    else
                    {
                        formatMove += move.OriginalPosition.Rank.ToString();
                    }
                }
            }
            else if (moveType == (MoveType.Move | MoveType.Capture) ||
                     moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += move.OriginalPosition.File.ToString().ToLowerInvariant();
            }

            if (moveType == (MoveType.Move | MoveType.Capture) ||
                moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += 'x';
            }

            formatMove += move.NewPosition.ToString();

            if (moveType == (MoveType.Move | MoveType.Promotion) ||
                moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                Debug.Assert(move.Promotion != null, "move.Promotion != null");

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

        private static string ToKiloFormat(long number)
        {
            if (number >= 10000)
            {
                return (number / 1000D).ToString("#,##0") + " k";
            }

            return number.ToString("#");
        }

        private static string GetFormattedEngineInfo(InfoType type, string data)
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

        private static string GetFormattedDrawReason(DrawReason drawReason)
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

        private static string FormatNumber(long number)
        {
            if (number >= 100000000000)
            {
                return (number / 1000000000).ToString("#,0 B");
            }

            if (number >= 10000000000)
            {
                return (number / 1000000000).ToString("0.#") + " B";
            }

            if (number >= 100000000)
            {
                return (number / 1000000).ToString("#,0 M");
            }

            if (number >= 10000000)
            {
                return (number / 1000000).ToString("0.#") + " M";
            }

            if (number >= 100000)
            {
                return (number / 1000).ToString("#,0 k");
            }

            if (number >= 10000)
            {
                return (number / 1000).ToString("0.#") + " k";
            }

            return number.ToString("#,0");
        }

        private static Color CalculateEvaluationColor(double evaluation)
        {
            const double range = 3;

            if (evaluation > range)
            {
                evaluation = range;
            }
            else if (evaluation < -range)
            {
                evaluation = -range;
            }

            var fraction = evaluation / range * 0.5;
            var x = 0.5 + fraction;

            if (SerializedInfo.Instance.DarkMode)
            {
                return Color.FromArgb(Math.Max((int)(x * 255) + 70, 255), Math.Max((int)((1 - x) * 255) + 70, 255),
                    0);
            }

            return Color.FromArgb((int)(x * 255), (int)((1 - x) * 255), 0);
        }

        private Engine GetPlayingEngine()
        {
            if (SerializedInfo.Instance.EngineMode == EngineMode.Competitive)
            {
                return _vm.EngineInfos[(int)_vm.Game.WhoseTurn].Engine;
            }

            return _vm.EngineInfos[0].Engine;
        }

        private static IEnumerable<Control> GetAllChildControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            var controlArray = controls as Control[] ?? controls.ToArray();
            return controlArray.SelectMany(GetAllChildControls).Concat(controlArray);
        }

        private static IEnumerable<ToolStripMenuItem> GetMenuItems(ToolStrip menuStrip)
        {
            var allMenuItems = new List<ToolStripMenuItem>();

            foreach (var item in menuStrip.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    AddMenuItems(menuItem, allMenuItems);
                }
            }

            return allMenuItems;
        }

        #endregion
    }
}