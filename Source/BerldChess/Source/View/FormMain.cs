using BerldChess.Model;
using BerldChess.Properties;
using BerldChess.Source.Model;
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
using System.Drawing.Imaging;
using System.Drawing.Text;
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
        private volatile bool _evalutionEnabled = true;
        private volatile bool _updateAfterAnimation = false;
        private volatile bool _analyzingMode = true;
        private volatile bool _movePlayed = true;
        private int _mainPvReference;
        private int _engineTime = 300;
        private int _animationTime = 300;
        private const int CentipawnTolerance = 85;
        private const int ChartShownPlies = 25;
        private const double DefaultSizeFactor = 0.82;
        private ChessPlayer _computerPlayer = ChessPlayer.None;
        private InfoType[] _columnOrder;
        private Random _random = new Random();
        private Bitmap _comparisonSnap = null;
        private ChessPanel _chessPanel;
        private SoundPlayer _movePlayer = new SoundPlayer(Resources.Move);
        private SoundPlayer _castlingPlayer = new SoundPlayer(Resources.Castling);
        private SoundPlayer _capturePlayer = new SoundPlayer(Resources.Capture);
        private SoundPlayer _illegalPlayer = new SoundPlayer(Resources.Ilegal);
        private InputSimulator _inputSimulator = new InputSimulator();
        private FormMainViewModel _vm;
        private FormPieceSettings _pieceDialog;
        private BoardSettingDialog _boardDialog;
        private FormEngineSettings _engineDialog;
        private volatile Evaluation[] _evaluations;
        private Control[] _engineViewElements;

        #endregion

        #region Constructors

        public FormMain()
        {
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
            ResetEvaluationData(SerializedInfo.Instance.MultiPV);
            ResetEvaluationGridRows(SerializedInfo.Instance.MultiPV);
        }

        private void InitializeEngine()
        {
            if (_vm.Engine != null)
            {
                _vm.Engine.Dispose();
                _vm.Engine = null;
            }

            if (SerializedInfo.Instance.EngineList.SettingAvailable)
            {
                EngineSetting setting = SerializedInfo.Instance.EngineList.SelectedSetting;

                if (File.Exists(setting.ExecutablePath))
                {
                    _vm.Engine = new Engine(setting.ExecutablePath);

                    for (int i = 0; i < setting.Arguments.Length; i++)
                    {
                        _vm.Engine.Query(setting.Arguments[i]);
                    }
                }
            }

            if (_vm.Engine != null)
            {
                _vm.Engine.EvaluationReceived += OnEvaluationReceived;
                _vm.Engine.BestMoveFound += OnBestMoveFound;
                _vm.Engine.Query($"setoption name MultiPV value {SerializedInfo.Instance.MultiPV}");
                _vm.Engine.Query("ucinewgame");
                _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");
                _vm.Engine.Query("go infinite");

                SetEngineViewElementsVisible(true);
            }
            else
            {
                SetEngineViewElementsVisible(false);
            }
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

        private void SetEngineViewElementsVisible(bool visible)
        {
            for (int i = 0; i < _engineViewElements.Length; i++)
            {
                _engineViewElements[i].Visible = visible;
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

        #endregion

        #region Event Methods

        private void OnBestMoveFound(string move)
        {
            if (!_movePlayed)
            {
                Invoke((MethodInvoker)delegate
                {
                    PlayMove(move);
                });
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

            EmptyEvaluation();

            _chessPanel.Arrows.Clear();
            _chessPanel.Invalidate();

            bool finishedPosition = false;
            string evaluationText = null;
            DrawReason drawReason = 0;

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                evaluationText = "Checkmate";
                finishedPosition = true;
            }
            else if (_vm.Game.IsStalemated(_vm.Game.WhoseTurn))
            {
                evaluationText = "Stalemate";
                finishedPosition = true;
            }
            else if (_vm.Game.CheckIfDraw(ref drawReason))
            {
                evaluationText = GetFormattedDrawReason(drawReason);
                finishedPosition = true;
            }

            if (finishedPosition)
            {
                SetExtendedInfoEnabled(false);
                _labelEvaluation.Text = evaluationText;
                _labelEvaluation.ForeColor = _menuItemNew.ForeColor;
            }
            else
            {
                _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

                if (_analyzingMode && (_computerPlayer == ChessPlayer.None || _computerPlayer != _vm.Game.WhoseTurn))
                {
                    _vm.Engine.Query("go infinite");
                }
                else
                {
                    _vm.Engine.Query($"go movetime {_engineTime}");
                }

                _movePlayed = false;
                _evalutionEnabled = true;
            }
        }

        private void OnDialogFontSelected()
        {
            ChessFont currentFont = SerializedInfo.Instance.SelectedChessFont;

            _chessPanel.IsUnicodeFont = currentFont.IsUnicode;
            _chessPanel.ChessFontChars = currentFont.PieceCharacters;
            _chessPanel.PieceSizeFactor = currentFont.SizeFactor;
            _chessPanel.PieceFontFamily = currentFont.FontFamily;

            _chessPanel.InvalidateRender();
        }

        private void OnBoardSettingAltered()
        {
            _chessPanel.Gradient = SerializedInfo.Instance.Gradient;
            _chessPanel.DarkSquare = SerializedInfo.Instance.BoardDarkSquare;
            _chessPanel.LightSquare = SerializedInfo.Instance.BoardLightSquare;
            _chessPanel.DisplayGridBorders = SerializedInfo.Instance.DisplayGridBorder;
            _chessPanel.InvalidateRender();

            SetDarkMode(this, SerializedInfo.Instance.DarkMode);
        }

        private void OnEvaluationReceived(Evaluation evaluation)
        {
            if (_evalutionEnabled)
            {
                ProcessEvaluation(evaluation);
            }
        }

        private void OnPieceMoved(object sender, PieceMovedEventArgs e)
        {
            if (PlayMove(e))
            {
                _vm.Engine?.RequestStop();
            }
        }

        private bool PlayMove(PieceMovedEventArgs args, char? promotion = null, bool cheatMove = false)
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
                if (!cheatMove && _menuItemSound.Checked)
                {
                    _illegalPlayer.Play();
                }

                return false;
            }

            if (cheatMove)
            {
                Recognizer.UpdateBoardImage();
            }

            _evalutionEnabled = false;

            ReadOnlyCollection<Move> validMoves = _vm.Game.GetValidMoves(_vm.Game.WhoseTurn);
            MoveType moveType = _vm.Game.ApplyMove(move, true);

            PlayMoveSound();

            _chessPanel.HighlighedSquares.Clear();
            _chessPanel.HighlighedSquares.Add(args.Position);
            _chessPanel.HighlighedSquares.Add(args.NewPosition);

            AddMoveToPlyList(move.ToString(""));
            UpdateMoveGrid(validMoves, move, moveType);

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

        private void AddMoveToPlyList(string moveString)
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

                FillPlyList(depth, whitePawnEvaluation);
                UpdateMoveGridCell(isMate, whitePawnEvaluation, depth);
            }

            _vm.PlyList.Add(new ChessPly(_vm.Game.GetFen(), 0.0, moveString));
            _vm.NavigationIndex++;
        }

        private void UpdateMoveGrid(ReadOnlyCollection<Move> validMoves, Move move, MoveType moveType)
        {
            int cellCount = _dataGridViewMoves.GetCellCount(DataGridViewElementStates.None);

            for (int i = cellCount - 2; i >= _vm.NavigationIndex; i--)
            {
                if (i % 2 == 0)
                {
                    _dataGridViewMoves.Rows.RemoveAt(i / 2);
                }
                else
                {
                    _dataGridViewMoves.Rows[i / 2].Cells[1].Value = "";
                }
            }

            int plyCount = _vm.PlyList.Count;
            string formattedEvaluation = _labelEvaluation.Text;

            if (formattedEvaluation.Substring(0, 4) == "Mate")
            {
                formattedEvaluation = "#";
            }

            if (plyCount % 2 == 0)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell whiteMove = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell blackMove = new DataGridViewTextBoxCell();

                whiteMove.Value = (plyCount / 2) + ". " + GetFormattedMove(move, moveType, validMoves);

                if (_vm.Engine != null)
                {
                    whiteMove.Value += "\n(" + formattedEvaluation + ")";
                }

                row.Cells.Add(whiteMove);
                row.Cells.Add(blackMove);

                row.Height = 40;
                _dataGridViewMoves.Rows.Add(row);
            }
            else
            {
                _dataGridViewMoves.Rows[plyCount / 2 - 1].Cells[1].Value = GetFormattedMove(move, moveType, validMoves);

                if (_vm.Engine != null)
                {
                    _dataGridViewMoves.Rows[plyCount / 2 - 1].Cells[1].Value += "\n(" + formattedEvaluation + ")";
                }
            }

            SelectMovesCell(_vm.NavigationIndex);
        }

        private void PlayMoveSound()
        {
            if (_menuItemSound.Checked)
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

        private void OnFormMainLoad(object sender, EventArgs e)
        {
            _menuItemEngineTime.Text = $"Enginetime [{_engineTime}]";
            _menuItemMultiPv.Text = $"MultiPV [{SerializedInfo.Instance.MultiPV}]";
            _menuItemAnimationTime.Text = $"Animation Time [{_animationTime}]";
            _menuItemClickDelay.Text = $"Click Delay [{SerializedInfo.Instance.ClickDelay}]";

            if (SerializedInfo.Instance.IsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void OnTimerValidationTick(object sender, EventArgs e)
        {
            if (!IsFinishedPosition() && _evalutionEnabled)
            {
                UpdateUI();
            }

            _panelEvaluationChart.Invalidate();
            _chessPanel.Invalidate();
        }

        private void UpdateUI()
        {
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
            _labelNodes.Text = FormatNumber(long.Parse(_evaluations[0][InfoType.Nodes]));
            _labelTime.Text = GetFormattedEngineInfo(InfoType.Time, _evaluations[0][InfoType.Time]);
            _labelNPS.Text = GetKiloFormat(int.Parse(_evaluations[0][InfoType.NPS]));
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

            FillPlyList(depth, whitePawnEvaluation);
            UpdateMoveGridCell(isMate, whitePawnEvaluation, depth);

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
                            _chessPanel.Arrows.Add(new Arrow((_evaluations[iPV][InfoType.PV]).Substring(0, 4), 0.9, GetReferenceColor(centipawn, _mainPvReference)));
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

        private void FillPlyList(int depth, double whitePawnEvaluation)
        {
            if (_vm.PlyList.Count > 0 && _vm.PlyList.Count > _vm.NavigationIndex)
            {
                if (depth >= _vm.CurrentPly.EvaluationDepth)
                {
                    _vm.CurrentPly.Evaluation = whitePawnEvaluation;
                    _vm.CurrentPly.EvaluationDepth = depth;
                }
            }
        }

        private void OnTimerAutoCheckTick(object sender, EventArgs e)
        {
            if (!_analyzingMode && IsFinishedPosition())
            {
                _analyzingMode = true;
            }

            if (!_analyzingMode)
            {
                _computerPlayer = _vm.Game.WhoseTurn;
            }

            if (_menuItemCheckAuto.Checked && _menuItemCheatMode.Checked)
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
            _vm.Engine?.Dispose();
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
            if (_vm.Engine != null)
            {
                if (_computerPlayer != ChessPlayer.None)
                {
                    _computerPlayer = ChessPlayer.None;
                    _vm.Engine.RequestStop();
                }
                else
                {
                    _computerPlayer = _vm.Game.WhoseTurn;
                    _vm.Engine.RequestStop();
                }
            }
        }

        private void OnMenuItemAutoPlayClick(object sender, EventArgs e)
        {
            if (_vm.Engine != null)
            {
                _analyzingMode = !_analyzingMode;
                _vm.Engine.RequestStop();
            }
        }

        private void OnMenuItemAutoMoveClick(object sender, EventArgs e)
        {
            CheckExternBoard();
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

        private void OnMenuItemEngineTimeClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Engine Time:", "BerldChess - Engine Time");
            int engineTime;

            if (int.TryParse(input, out engineTime))
            {
                _engineTime = engineTime;
                _menuItemEngineTime.Text = $"Enginetime [{_engineTime}]";
            }
        }

        private void OnMenuItemMultiPvClick(object sender, EventArgs e)
        {
            if (_vm.Engine != null)
            {
                string input = Interaction.InputBox("Enter MultiPV:", "BerldChess - MultiPV");
                int multiPV;

                if (int.TryParse(input, out multiPV))
                {
                    if (multiPV > 0 && multiPV <= 250)
                    {
                        _evalutionEnabled = false;

                        SerializedInfo.Instance.MultiPV = multiPV;
                        _menuItemMultiPv.Text = $"MultiPV [{SerializedInfo.Instance.MultiPV}]";
                        _vm.Engine.Query($"setoption name MultiPV value {SerializedInfo.Instance.MultiPV}");
                        _vm.Engine.Query("stop");
                        _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");
                        _chessPanel.Invalidate();

                        ResetEvaluationData(multiPV);
                        ResetEvaluationGridRows(SerializedInfo.Instance.MultiPV);

                        _evalutionEnabled = true;
                    }
                }
            }
        }

        private void OnMenuItemDepthAnalysisClick(object sender, EventArgs e)
        {
            if (_vm.Engine == null)
            {
                return;
            }

            string input = Interaction.InputBox("Enter Depth:", "BerldChess - Depth Analysis");
            int depth;

            if (int.TryParse(input, out depth))
            {
                if (depth > 60)
                {
                    depth = 60;
                }

                if (_vm.PlyList.Count > 0)
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    Task anaylsisTask = new Task(() =>
                    {
                        for (int i = _vm.PlyList.Count - 1; i >= 0; i--)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                Navigate(i);
                            });

                            while (_vm.PlyList[i].EvaluationDepth < depth)
                            {
                                Thread.Sleep(100);

                                if (source.Token.IsCancellationRequested)
                                {
                                    return;
                                }
                            }
                        }
                    });

                    anaylsisTask.Start();

                    MessageBox.Show(this, "Analysing to depth " + depth + "\n\n Press OK to cancel.", "Depth Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    source.Cancel();
                }
            }
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
            Clipboard.SetText(_vm.CurrentPly.FEN);
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
            FormPgnLoader pgnLoader = new FormPgnLoader();
            pgnLoader.ShowDialog();
        }

        private void OnPanelEvaluationChartPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.Default;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            Pen gridPen = new Pen(_panelEvaluationChart.ForeColor.OperateAll(200, !SerializedInfo.Instance.DarkMode), 1);
            Pen middleLinePen = new Pen(_panelEvaluationChart.ForeColor.OperateAll(120, !SerializedInfo.Instance.DarkMode), 1);
            Pen chartBorderPen = new Pen(_panelEvaluationChart.ForeColor, 1);

            Font labelFont = new Font("Segoe UI", 10);

            SolidBrush fontBrush = new SolidBrush(_panelEvaluationChart.ForeColor);
            SolidBrush chartLightBrush = new SolidBrush(Color.White);
            SolidBrush chartDarkBrush = new SolidBrush(Color.FromArgb(69, 66, 63));

            int peak;
            float chartYMiddle = _panelEvaluationChart.Height / 2.0F;

            List<ChessPly> positions = _vm.PlyList.ToList();

            if (!_totalChartView && positions.Count > ChartShownPlies)
            {
                positions.RemoveRange(0, positions.Count - ChartShownPlies);
            }

            double highestValue = positions.Max(c => c.Evaluation);
            double lowestValue = positions.Min(c => c.Evaluation);

            if (highestValue > Math.Abs(lowestValue))
            {
                peak = (int)Math.Ceiling(highestValue);
            }
            else
            {
                peak = (int)Math.Ceiling(Math.Abs(lowestValue));
            }

            if (peak % 2 == 1)
            {
                peak++;
            }

            if (peak < 3)
            {
                peak = 3;
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

                g.DrawString(text, labelFont, fontBrush, labelFont.Size / 2, Round(_panelEvaluationChart.Height / (double)rowCount * i) - offSet);
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
                    float y;

                    float height = (float)Math.Abs((evaluation * yUnitLength));
                    float width = (float)((i + 1) * xUnitLength - i * xUnitLength);

                    if (evaluation >= 0)
                    {
                        y = chartYMiddle - (float)(evaluation * yUnitLength);
                        g.FillRectangle(chartLightBrush, x, y, width, height);
                        g.DrawRectangle(chartBorderPen, x, y, width, height);
                    }
                    else
                    {
                        y = chartYMiddle;
                        g.FillRectangle(chartDarkBrush, x, y, width, height);
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

            if (navigationIndex >= _vm.PlyList.Count)
            {
                Navigate(_vm.PlyList.Count - 1, false);
            }
            else
            {
                Navigate(e.RowIndex * 2 + e.ColumnIndex + 1);
            }
        }

        private void OnTableLayoutPanelModulesResize(object sender, EventArgs e)
        {
            _tableLayoutPanelModules.Height = Round(_tableLayoutPanelModules.Width * 1.942196);
        }

        private void OnLabelTextValidate(object sender, EventArgs e)
        {
            Control source = (Control)sender;
            source.FitFont();
        }

        private void OnLabelEvaluationTextChanged(object sender, EventArgs e)
        {
            _labelEvaluation.FitFont();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
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

        private void OnMenuItemEngineSettingsClick(object sender, EventArgs e)
        {
            _engineDialog.ShowDialog();
        }

        #endregion

        #region Other Methods

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
                    _inputSimulator.Mouse.MoveMouseTo((int)(max * (double)currCurPos.X / (double)pW), (int)Math.Round((max * (double)currCurPos.Y / (double)pH * 0.97), 0));
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

            for (int i = 0; i < _columnOrder.Length; i++)
            {
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
                    formatEvaluation = " 0.00";
                }
                else
                {
                    formatEvaluation = whitePawnEvaluation.ToString("+0.00;-0.00");
                }

                string currentValue = (string)_dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value;

                if (currentValue != null && depth > 0)
                {
                    _dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value = $"{currentValue.Substring(0, currentValue.IndexOf('(') + 1)}{formatEvaluation} D{depth})";
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
            bool wasFinished = IsFinishedPosition();

            _evalutionEnabled = false;
            _analyzingMode = true;

            _computerPlayer = ChessPlayer.None;

            if (_menuItemCheatMode.Checked)
            {
                Recognizer.UpdateBoardImage();
            }

            _vm.Game = newGame;
            _vm.PlyList.Clear();
            _vm.PlyList.Add(new ChessPly(_vm.Game.GetFen()));
            _vm.NavigationIndex = 0;

            _chessPanel.Game = _vm.Game;
            _chessPanel.HighlighedSquares.Clear();

            _dataGridViewMoves.Rows.Clear();

            _movePlayed = true;

            if (_vm.Engine != null)
            {
                _vm.Engine.Query("ucinewgame");

                if (wasFinished)
                {
                    OnEngineStopped();
                }
                else
                {
                    _vm.Engine.RequestStop();
                }
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
                    controls[i].BackColor = Color.FromArgb(49, 46, 43);
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
                        grid.BackgroundColor = Color.FromArgb(49, 46, 43);
                        grid.DefaultCellStyle.BackColor = Color.FromArgb(49, 46, 43);
                        grid.DefaultCellStyle.ForeColor = Color.White;
                        grid.DefaultCellStyle.SelectionBackColor = SystemColors.ActiveCaption;
                        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        grid.BorderStyle = BorderStyle.FixedSingle;
                        grid.BackgroundColor = Color.White;
                        grid.DefaultCellStyle.BackColor = Color.White;
                        grid.DefaultCellStyle.ForeColor = Color.Black;
                        grid.DefaultCellStyle.SelectionBackColor = SystemColors.ActiveCaption;
                        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
                    }
                }
            }

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (darkMode)
                {
                    menuItems[i].BackColor = Color.FromArgb(49, 46, 43);
                    menuItems[i].ForeColor = Color.White;
                }
                else
                {
                    menuItems[i].BackColor = SystemColors.Control;
                    menuItems[i].ForeColor = Color.Black;
                }
            }
        }

        public IEnumerable<Control> GetAllChildControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllChildControls(ctrl)).Concat(controls);
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
                File.Delete(FormMainViewModel.ConfigurationFileName);
                Debug.WriteLine(ex.ToString());
            }
        }

        private void LoadInfo()
        {
            if (SerializedInfo.Instance.Bounds != null)
            {
                Bounds = (Rectangle)SerializedInfo.Instance.Bounds;
            }

            _chessPanel.PieceSizeFactor = SerializedInfo.Instance.PieceSizeFactor;

            if (SerializedInfo.Instance.ChessFonts.Count > 0 &&
                SerializedInfo.Instance.SelectedFontIndex > 0 &&
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

            _chessPanel.DarkSquare = SerializedInfo.Instance.BoardDarkSquare;
            _chessPanel.LightSquare = SerializedInfo.Instance.BoardLightSquare;
            _chessPanel.Gradient = SerializedInfo.Instance.Gradient;

            _menuItemHideOutput.Checked = SerializedInfo.Instance.HideOutput;
            _menuItemHideArrows.Checked = SerializedInfo.Instance.HideArrows;

            _splitContainerBoard.Panel2Collapsed = _menuItemHideOutput.Checked;

            _chessPanel.DisplayGridBorders = SerializedInfo.Instance.DisplayGridBorder;
            _menuItemFlipBoard.Checked = SerializedInfo.Instance.BoardFlipped;
            _menuItemLocalMode.Checked = SerializedInfo.Instance.LocalMode;
            _menuItemCheatMode.Checked = SerializedInfo.Instance.CheatMode;
            _menuItemCheckAuto.Checked = SerializedInfo.Instance.AutoCheck;
            _engineTime = SerializedInfo.Instance.EngineTime;
            _menuItemSound.Checked = SerializedInfo.Instance.Sound;
            _animationTime = SerializedInfo.Instance.AnimationTime;

            SetDarkMode(this, SerializedInfo.Instance.DarkMode);

            if (SerializedInfo.Instance.ChessFonts.Count == 0)
            {
                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Default1",
                    FontFamily = "Default1",
                    SizeFactor = DefaultSizeFactor,
                    IsUnicode = false,
                    PieceCharacters = ""
                });

                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Default2",
                    FontFamily = "Default2",
                    SizeFactor = 1,
                    IsUnicode = false,
                    PieceCharacters = ""
                });

                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Default3",
                    FontFamily = "Default3",
                    SizeFactor = 0.9,
                    IsUnicode = false,
                    PieceCharacters = ""
                });
            }

            _engineDialog = new FormEngineSettings(SerializedInfo.Instance.EngineList);
            _engineDialog.EngineSelected += OnEngineSelected;

            _pieceDialog = new FormPieceSettings(SerializedInfo.Instance.ChessFonts, SerializedInfo.Instance.SelectedFontIndex);
            _pieceDialog.FontSelected += OnDialogFontSelected;

            _boardDialog = new BoardSettingDialog();
            _boardDialog.BoardSettingAltered += OnBoardSettingAltered;

            _chessPanel.Invalidate();
        }

        private void OnEngineSelected()
        {
            InitializeEngine();
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

                SerializedInfo.Instance.IsMaximized = WindowState == FormWindowState.Maximized;
                SerializedInfo.Instance.BoardFlipped = _menuItemFlipBoard.Checked;
                SerializedInfo.Instance.HideArrows = _menuItemHideArrows.Checked;
                SerializedInfo.Instance.HideOutput = _menuItemHideOutput.Checked;
                SerializedInfo.Instance.LocalMode = _menuItemLocalMode.Checked;
                SerializedInfo.Instance.CheatMode = _menuItemCheatMode.Checked;
                SerializedInfo.Instance.EngineTime = _engineTime;
                SerializedInfo.Instance.Sound = _menuItemSound.Checked;
                SerializedInfo.Instance.AutoCheck = _menuItemCheckAuto.Checked;
                SerializedInfo.Instance.AnimationTime = _animationTime;

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

            //formatMove += move.OriginalPosition.ToString();

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

        private void Navigate(int navigation, bool validate = true)
        {
            if (validate)
            {
                if (navigation == _vm.NavigationIndex || navigation >= _vm.PlyList.Count)
                {
                    return;
                }
            }

            bool wasFinished = IsFinishedPosition();
            _evalutionEnabled = false;
            _analyzingMode = true;

            _computerPlayer = ChessPlayer.None;

            _vm.NavigationIndex = navigation;

            _vm.Game = new ChessGame(_vm.CurrentPly.FEN);

            _chessPanel.Game = _vm.Game;
            _chessPanel.HighlighedSquares.Clear();

            string currentMoveString = _vm.CurrentPly.Move;

            if (!string.IsNullOrEmpty(currentMoveString))
            {
                Point[] squareLocations = _chessPanel.GetRelPositionsFromMoveString(currentMoveString);
                _chessPanel.HighlighedSquares.AddRange(squareLocations);
            }

            SelectMovesCell(navigation);

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _vm.NavigationIndex % 2 == 1;
                _chessPanel.Invalidate();
            }

            _movePlayed = true;

            if (_vm.Engine != null)
            {
                if (wasFinished)
                {
                    OnEngineStopped();
                }
                else
                {
                    _vm.Engine.RequestStop();
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

            _dataGridViewMoves.Rows[navIndex / 2].Cells[navIndex % 2].Selected = true;
            _dataGridViewMoves.CurrentCell = _dataGridViewMoves.Rows[navIndex / 2].Cells[navIndex % 2];
        }

        private int Round(double number)
        {
            return (int)Math.Round(number, 0);
        }

        private int Invert(int value, int range)
        {
            return Math.Abs(value - range);
        }

        private string GetKiloFormat(long number)
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
                    return GetKiloFormat(long.Parse(data));
            }

            return data;
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
                return Color.FromArgb(((int)(x * 255) + 70).Cap(255), ((int)((1 - x) * 255) + 70).Cap(255), 0);
            }
            else
            {
                return Color.FromArgb((int)(x * 255), (int)((1 - x) * 255), 0);
            }
        }

        private List<ToolStripMenuItem> GetMenuItems(MenuStrip menuStrip)
        {
            List<ToolStripMenuItem> myItems = new List<ToolStripMenuItem>();

            foreach (ToolStripMenuItem menuItem in menuStrip.Items)
            {
                GetMenuItems(menuItem, myItems);
            }

            return myItems;
        }

        private void GetMenuItems(ToolStripMenuItem menuItem, List<ToolStripMenuItem> menuItems)
        {
            menuItems.Add(menuItem);

            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    GetMenuItems((ToolStripMenuItem)item, menuItems);
                }
            }
        }

        private void EmptyEvaluation()
        {
            for (int rowI = 0; rowI < _dataGridViewEvaluation.Rows.Count; rowI++)
            {
                for (int cellI = 0; cellI < _dataGridViewEvaluation.Rows[rowI].Cells.Count; cellI++)
                {
                    _dataGridViewEvaluation.Rows[rowI].Cells[cellI].Value = "";
                }
            }
        }

        private void CheckExternBoard()
        {
            if (_comparisonSnap == null)
            {
                _comparisonSnap = Recognizer.GetBoardSnap();
                return;
            }

            Bitmap _currImg = Recognizer.GetBoardSnap();
            bool areSame = CompareBitmaps(_currImg, _comparisonSnap);

            if (_updateAfterAnimation && areSame)
            {
                _updateAfterAnimation = false;
                Recognizer.UpdateBoardImage(_currImg);
                return;
            }

            if (areSame)
            {
                Point[] changedSquares = Recognizer.GetChangedSquares(_currImg);

                if (changedSquares == null || changedSquares.Length == 0)
                {
                    return;
                }
                else
                {
                    string msg = "Changed: ";

                    for (int i = 0; i < changedSquares.Length; i++)
                    {
                        msg += changedSquares[i].X + " " + changedSquares[i].Y + "  ";
                    }

                    Debug.WriteLine(msg);
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

                PlayMove(args, 'q', true);
                _vm.Engine?.RequestStop();
            }

            _comparisonSnap = _currImg;
        }

        private bool CompareBitmaps(Bitmap bitmap1, Bitmap bitmap2)
        {
            bool equals = true;

            Rectangle rect = new Rectangle(0, 0, bitmap1.Width, bitmap1.Height);
            BitmapData bitmapData1 = bitmap1.LockBits(rect, ImageLockMode.ReadOnly, bitmap1.PixelFormat);
            BitmapData bitmapData2 = bitmap2.LockBits(rect, ImageLockMode.ReadOnly, bitmap2.PixelFormat);

            unsafe
            {
                byte* ptr1 = (byte*)bitmapData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bitmapData2.Scan0.ToPointer();
                int width = rect.Width * 3;

                for (int y = 0; equals && y < rect.Height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            equals = false;
                            break;
                        }
                        ptr1++;
                        ptr2++;
                    }
                    ptr1 += bitmapData1.Stride - width;
                    ptr2 += bitmapData2.Stride - width;
                }
            }

            bitmap1.UnlockBits(bitmapData1);
            bitmap2.UnlockBits(bitmapData2);

            return equals;
        }

        private void SetExtendedInfoEnabled(bool active)
        {
            _tableLayoutPanelEvalInfos.Visible = active;
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

        private bool IsFinishedPosition()
        {
            return _vm.Game.IsCheckmated(_vm.Game.WhoseTurn) || _vm.Game.IsDraw;
        }


        #endregion


    }
}