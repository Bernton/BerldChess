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
using System.Windows.Forms;
using System.Xml.Serialization;
using WindowsInput;

namespace BerldChess.View
{
    public partial class FormMain : Form
    {
        #region Fields

        private bool _isMoveTry = false;
        private bool _isAutoPlay = false;
        private volatile bool _evaluationEnabled = true;
        private volatile bool _updateAfterAnimation = false;
        private int _pvOneReference;
        private int _engineTime = 300;
        private int _animationTime = 300;
        private ChessPlayer _computerPlayer = ChessPlayer.None;
        private InfoType[] _columnOrder;
        private Random _random = new Random();
        private Bitmap _comparisonSnap = null;
        private Stopwatch _timeSinceLastMove = new Stopwatch();
        private ChessPanel _chessPanel;
        private SoundPlayer _movePlayer = new SoundPlayer(Resources.Move);
        private SoundPlayer _castlingPlayer = new SoundPlayer(Resources.Castling);
        private SoundPlayer _capturePlayer = new SoundPlayer(Resources.Capture);
        private SoundPlayer _illegalPlayer = new SoundPlayer(Resources.Ilegal);
        private InputSimulator _inputSimulator = new InputSimulator();
        private FormMainViewModel _vm;
        private PieceSelectionDialog _pieceDialog;
        private BoardSettingDialog _boardDialog;

        #endregion

        #region Constructors

        public FormMain()
        {
            InitializeComponent();
            SetDoubleBuffered(_panelEvaluationChart);
            SetDoubleBuffered(_dataGridViewMoves);

            InitializeWindow();
            InitializeViewModel();
            InitializeChessBoard();

            foreach (Control control in GetAll(this))
            {
                control.Tag = control.BackColor;
            }

            Tag = BackColor;

            LoadXml();

            if (SerializedInfo.Instance.ChessFonts.Count == 0)
            {
                SerializedInfo.Instance.ChessFonts.Add(new ChessFont
                {
                    Name = "Default",
                    FontFamily = "",
                    SizeFactor = 1.05,
                    IsUnicode = false,
                    PieceCharacters = ""
                });
            }

            _pieceDialog = new PieceSelectionDialog(SerializedInfo.Instance.ChessFonts, SerializedInfo.Instance.SelectedFontIndex);
            _pieceDialog.FontSelected += OnDialogFontSelected;

            _boardDialog = new BoardSettingDialog();
            _boardDialog.BoardSettingAltered += OnBoardSettingAltered;

            _columnOrder = new InfoType[]
            {
                InfoType.MultiPV,
                InfoType.Depth,
                InfoType.Score,
                InfoType.Time,
                InfoType.Nodes,
                InfoType.NPS,
                InfoType.PV
            };

            ResetDataGridViewColumns(_columnOrder);
            ResetDataGridViewRows(SerializedInfo.Instance.MultiPV);

            _vm.Engine.Query($"setoption name MultiPV value {SerializedInfo.Instance.MultiPV}");
            _vm.Engine.Query("stop");
            _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

            Recognizer.SearchBoard(SerializedInfo.Instance.EngineLightSquare, SerializedInfo.Instance.EngineDarkSquare);
        }

        #endregion

        #region Initialization Methods

        private void InitializeWindow()
        {
            Icon = Resources.PawnRush;
            Text = string.Format("BerldChess Version {0}", Assembly.GetEntryAssembly().GetName().Version.ToString(3));

            _movePlayer.Load();
            _capturePlayer.Load();
            _castlingPlayer.Load();
            _illegalPlayer.Load();
        }

        private void InitializeViewModel()
        {
            _vm = new FormMainViewModel();
            _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen()));
            _vm.Engine.EvaluationReceived += OnEvaluationReceived;
            _vm.Engine.EngineStopped += OnEngineStopped;
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
            _splitContainerMain.Panel1.Controls.Add(_chessPanel);
        }

        #endregion

        #region Event Methods

        private void OnEngineStopped()
        {
            EmptyEvaluation();
            _chessPanel.Arrows.Clear();
            _chessPanel.Invalidate();

            _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                Invoke((MethodInvoker)delegate
                {
                    _labelEvaluation.Text = "Checkmate";
                    _labelEvaluation.ForeColor = _menuItemNew.ForeColor;
                });

                EmptyEvaluation();
            }
            else if (_vm.Game.IsStalemated(_vm.Game.WhoseTurn))
            {
                Invoke((MethodInvoker)delegate
                {
                    _labelEvaluation.Text = "Stalemate";
                    _labelEvaluation.ForeColor = _menuItemNew.ForeColor;
                });

                EmptyEvaluation();
            }
            else if (_vm.Game.IsDraw())
            {
                if (IsHandleCreated)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        _labelEvaluation.Text = "Draw";
                        _labelEvaluation.ForeColor = _menuItemNew.ForeColor;
                    });

                    EmptyEvaluation();
                }
            }
            else
            {
                _vm.Engine.Query("go infinite");
                _evaluationEnabled = true;
            }
        }

        private void OnDialogFontSelected()
        {
            ChessFont font = SerializedInfo.Instance.ChessFonts[SerializedInfo.Instance.SelectedFontIndex];

            _chessPanel.IsUnicodeFont = font.IsUnicode;
            _chessPanel.ChessFontChars = font.PieceCharacters;
            _chessPanel.PieceSizeFactor = font.SizeFactor;
            _chessPanel.PieceFontFamily = font.FontFamily;
        }

        private void OnBoardSettingAltered()
        {
            _chessPanel.DarkSquare = SerializedInfo.Instance.BoardDarkSquare;
            _chessPanel.LightSquare = SerializedInfo.Instance.BoardLightSquare;
            _chessPanel.DisplayGridBorders = SerializedInfo.Instance.DisplayGridBorder;
            SetDarkMode(this, SerializedInfo.Instance.DarkMode);
            _chessPanel.Invalidate();
        }

        private void OnEvaluationReceived(Evaluation evaluation)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            try
            {
                int multiPv = int.Parse(evaluation[InfoType.MultiPV]);

                Invoke((MethodInvoker)delegate
                {
                    if (_columnOrder.Length != _dataGridViewEvaluation.Columns.Count)
                    {
                        ResetDataGridViewColumns(_columnOrder);
                        ResetDataGridViewRows(SerializedInfo.Instance.MultiPV);
                    }

                    for (int i = 0; i < evaluation.Types.Length; i++)
                    {
                        int columnIndex = -1;

                        for (int iOrder = 0; iOrder < _columnOrder.Length; iOrder++)
                        {
                            if (_columnOrder[iOrder] == evaluation.Types[i])
                            {
                                columnIndex = iOrder;
                                break;
                            }
                        }

                        if (columnIndex != -1)
                        {
                            if (_dataGridViewEvaluation.Rows.Count >= multiPv)
                            {
                                _dataGridViewEvaluation.Rows[multiPv - 1].Cells[columnIndex].Value = GetFormattedEngineInfo(evaluation.Types[i], evaluation[i]);
                            }
                        }
                    }

                    bool isMate = false;
                    string pawnValue = evaluation[InfoType.Score];
                    int mateNumber = -1;
                    int centipawn = -20000;

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
                                throw new InvalidOperationException("Can't convert score.");
                            }
                        }
                    }

                    if (isMate)
                    {
                        mateNumber = centipawn;

                        if (centipawn > 0)
                        {
                            centipawn += 12000 + Math.Abs(multiPv - SerializedInfo.Instance.MultiPV) * 25;
                        }
                        else
                        {
                            centipawn -= 12000 - Math.Abs(multiPv - SerializedInfo.Instance.MultiPV) * 25;
                        }
                    }

                    if (multiPv == 1)
                    {
                        double whiteEvaluation;

                        if (isMate)
                        {
                            if (mateNumber > 0)
                            {
                                _labelEvaluation.Text = $"Mate in {mateNumber}";
                                _labelEvaluation.ForeColor = Color.Green;

                            }
                            else
                            {
                                _labelEvaluation.Text = $"Mated in {Math.Abs(mateNumber)}";
                                _labelEvaluation.ForeColor = Color.Red;
                            }

                            if (_vm.Game.WhoseTurn == ChessPlayer.Black && mateNumber > 0 || _vm.Game.WhoseTurn == ChessPlayer.White && mateNumber < 0)
                            {
                                whiteEvaluation = -120;
                            }
                            else
                            {
                                whiteEvaluation = 120;
                            }
                        }
                        else
                        {
                            if (_vm.Game.WhoseTurn == ChessPlayer.Black)
                            {
                                whiteEvaluation = -(centipawn / 100.0);
                            }
                            else
                            {
                                whiteEvaluation = centipawn / 100.0;
                            }

                            if (whiteEvaluation == 0)
                            {
                                _labelEvaluation.Text = " 0.00";
                            }
                            else
                            {
                                _labelEvaluation.Text = whiteEvaluation.ToString("+0.00;-0.00");
                            }

                            _labelEvaluation.ForeColor = CalculateEvaluationColor(-(centipawn / 100.0));
                        }

                        if (_evaluationEnabled && _vm.PositionHistory.Count > 1 && _vm.PositionHistory.Count > _vm.NavigationIndex)
                        {
                            int depth = int.Parse(evaluation[InfoType.Depth]);

                            if (depth >= _vm.PositionHistory[_vm.NavigationIndex].EvaluationDepth)
                            {
                                _vm.PositionHistory[_vm.NavigationIndex].Evaluation = whiteEvaluation;
                                _vm.PositionHistory[_vm.NavigationIndex].EvaluationDepth = depth;

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
                                    else if (whiteEvaluation == 0)
                                    {
                                        formatEvaluation = " 0.00";
                                    }
                                    else
                                    {
                                        formatEvaluation = whiteEvaluation.ToString("+0.00;-0.00");
                                    }

                                    string currentValue = (string)_dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value;
                                    _dataGridViewMoves.Rows[rowIndex].Cells[columnIndex].Value = $"{currentValue.Substring(0, currentValue.IndexOf('(') + 1)}{formatEvaluation} D{depth})";
                                }
                            }
                        }
                    }

                    if (_menuItemHideArrows.Checked)
                    {
                        return;
                    }

                    if (multiPv == 1 || (!isMate && _pvOneReference - centipawn < 150) || _chessPanel.Arrows.Count < 3)
                    {
                        if (multiPv == 1)
                        {
                            _pvOneReference = centipawn;
                        }

                        if (_chessPanel.Arrows.Count < multiPv)
                        {
                            _chessPanel.Arrows.Add(new Arrow((evaluation[InfoType.PV]).Substring(0, 4), 0.9, GetReferenceColor(centipawn, _pvOneReference)));
                        }
                        else
                        {
                            _chessPanel.Arrows[multiPv - 1].Move = (evaluation[InfoType.PV]).Substring(0, 4);
                            _chessPanel.Arrows[multiPv - 1].Color = GetReferenceColor(centipawn, _pvOneReference);
                        }

                        if (multiPv == 1)
                        {
                            _chessPanel.Arrows[multiPv - 1].Color = Color.DarkBlue;
                        }
                    }
                    else
                    {
                        if (_chessPanel.Arrows.Count >= multiPv)
                        {
                            _chessPanel.Arrows.RemoveRange(multiPv, _chessPanel.Arrows.Count - multiPv);
                        }
                    }
                });
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void OnPieceMoved(object sender, FigureMovedEventArgs e)
        {
            BoardPosition startPosition = new BoardPosition((ChessFile)e.Position.X, Invert(e.Position.Y - 1, 7));
            BoardPosition endPosition = new BoardPosition((ChessFile)e.NewPosition.X, Invert(e.NewPosition.Y - 1, 7));
            ChessPiece movingPiece = _vm.Game.GetPieceAt(startPosition);

            char promotion = 'q';

            if (_vm.Game.WhoseTurn != _computerPlayer)
            {
                if (movingPiece is Pawn && startPosition.Rank == (movingPiece.Owner == ChessPlayer.White ? 7 : 2) && endPosition.Rank == (movingPiece.Owner == ChessPlayer.White ? 8 : 1))
                {
                    PromotionDialog dialog = new PromotionDialog(movingPiece.Owner);
                    dialog.ShowDialog();

                    promotion = dialog.PromotionCharacter;
                }
            }

            if (movingPiece == null)
            {
                return;
            }

            Move move = new Move(startPosition, endPosition, movingPiece.Owner, promotion);
            MoveType moveType;

            if (_vm.Game.IsValidMove(move))
            {
                if (_isMoveTry)
                {
                    Recognizer.UpdateBoardImage();
                    _isMoveTry = false;
                }

                ReadOnlyCollection<Move> legalMoves = _vm.Game.GetValidMoves(_vm.Game.WhoseTurn);

                if (_menuItemLocalMode.Checked)
                {
                    _chessPanel.IsFlipped = !_chessPanel.IsFlipped;
                    _chessPanel.Invalidate();
                }

                moveType = _vm.Game.ApplyMove(move, true);

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

                _chessPanel.HighlighedSquares.Clear();
                _chessPanel.HighlighedSquares.Add(e.Position);
                _chessPanel.HighlighedSquares.Add(e.NewPosition);

                if (_vm.NavigationIndex != _vm.PositionHistory.Count - 1)
                {
                    int countCache = _vm.PositionHistory.Count;

                    _vm.PositionHistory.RemoveRange(_vm.NavigationIndex + 1, _vm.PositionHistory.Count - _vm.NavigationIndex - 1);

                    for (int i = countCache - 2; i >= _vm.NavigationIndex; i--)
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
                }

                _evaluationEnabled = false;

                _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen(), 0.0, move.ToString("")));

                int count = _vm.PositionHistory.Count;
                string displayEval = _labelEvaluation.Text;

                if (displayEval.Substring(0, 4) == "Mate")
                {
                    displayEval = "#";
                }

                if (count % 2 == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell whiteMove = new DataGridViewTextBoxCell();
                    DataGridViewTextBoxCell blackMove = new DataGridViewTextBoxCell();

                    whiteMove.Value = (count / 2) + ". " + GetFormattedMove(move, moveType, legalMoves) + "\n(" + displayEval + ")";

                    row.Cells.Add(whiteMove);
                    row.Cells.Add(blackMove);

                    row.Height = 40;
                    _dataGridViewMoves.Rows.Add(row);
                }
                else
                {
                    _dataGridViewMoves.Rows[count / 2 - 1].Cells[1].Value = GetFormattedMove(move, moveType, legalMoves) + "\n(" + displayEval + ")";
                }

                _vm.NavigationIndex++;
                SelectMovesCell(_vm.NavigationIndex);

                if (_vm.Game.IsCheckmated(ChessPlayer.Black) || _vm.Game.IsCheckmated(ChessPlayer.White))
                {
                    _labelEvaluation.Text = "Checkmate";
                }
                else if (_vm.Game.IsStalemated(ChessPlayer.Black) || _vm.Game.IsStalemated(ChessPlayer.White))
                {
                    _labelEvaluation.Text = "Stalemate";
                }
                else if (_vm.Game.IsDraw())
                {
                    _labelEvaluation.Text = "Draw";
                }

                _timeSinceLastMove.Restart();

                _vm.Engine.RequestStop();
                _chessPanel.Invalidate();
            }
            else
            {
                if (!_isMoveTry && _menuItemSound.Checked)
                {
                    _illegalPlayer.Play();
                }
            }
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

        private void OnTimerEngineTick(object sender, EventArgs e)
        {
            if (_computerPlayer != ChessPlayer.None)
            {
                if (_vm.Game.WhoseTurn == _computerPlayer)
                {
                    if (!IsFinishedPosition())
                    {
                        if (_timeSinceLastMove.ElapsedMilliseconds > _engineTime && _dataGridViewEvaluation.Rows.Count > 0)
                        {
                            if ((string)_dataGridViewEvaluation.Rows[0].Cells[6].Value == "")
                            {
                                return;
                            }

                            string move = ((string)_dataGridViewEvaluation.Rows[0].Cells[6].Value).Substring(0, 5);

                            Point[] positions = _chessPanel.GetRelPositionsFromMoveString((move));
                            FigureMovedEventArgs args = new FigureMovedEventArgs(positions[0], positions[1]);

                            OnPieceMoved(null, args);

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

                                    _inputSimulator.Mouse.MoveMouseTo(max + (int)(max * (Recognizer.BoardLocation.X + fW * (positions[0].X + 0.45)) / pW), (int)(max * (Recognizer.BoardLocation.Y + fW * (positions[0].Y + 0.45)) / pH));
                                    _inputSimulator.Mouse.LeftButtonClick();
                                    Thread.Sleep(_random.Next((int)(SerializedInfo.Instance.ClickDelay / 2.5), SerializedInfo.Instance.ClickDelay));
                                    _inputSimulator.Mouse.MoveMouseTo(max + (int)(max * (Recognizer.BoardLocation.X + fH * (positions[1].X + 0.45)) / pW), (int)(max * (Recognizer.BoardLocation.Y + fH * (positions[1].Y + 0.45)) / pH));
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
                        }
                    }
                }
            }
        }

        private void OnTimerValidationTick(object sender, EventArgs e)
        {
            _panelEvaluationChart.Invalidate();
            _chessPanel.Invalidate();
        }

        private void OnTimerAutoCheckTick(object sender, EventArgs e)
        {
            if (_isAutoPlay && IsFinishedPosition())
            {
                _isAutoPlay = false;
            }

            if (_isAutoPlay)
            {
                _computerPlayer = _vm.Game.WhoseTurn;
                if (!_timeSinceLastMove.IsRunning)
                {
                    _timeSinceLastMove.Restart();
                }
                _timerEngine.Enabled = true;
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

            _isAutoPlay = false;
            _timeSinceLastMove.Reset();
            _computerPlayer = ChessPlayer.None;
            _timerEngine.Enabled = false;

            Recognizer.UpdateBoardImage();

            bool wasFinished = IsFinishedPosition();

            try
            {
                _vm.Game = new ChessGame(input);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Berld Chess", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Debug.WriteLine(ex.ToString());
                return;
            }

            _chessPanel.Game = _vm.Game;
            _vm.PositionHistory.Clear();

            _vm.PositionHistory.Add(new ChessPosition(input));
            _chessPanel.HighlighedSquares.Clear();
            _vm.NavigationIndex = 0;

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
            }


            if (wasFinished)
            {
                OnEngineStopped();
            }
            else
            {
                _vm.Engine.RequestStop();
            }

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
            if (_vm.NavigationIndex != _vm.PositionHistory.Count - 1)
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
            Navigate(_vm.PositionHistory.Count - 1);
        }

        private void OnFormMainClosing(object sender, FormClosingEventArgs e)
        {
            SaveXml();
            _vm.Engine.Dispose();
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
            _splitContainerMain.Panel2Collapsed = _menuItemHideOutput.Checked;
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
            _isAutoPlay = false;
            _timeSinceLastMove.Reset();
            _computerPlayer = ChessPlayer.None;
            _timerEngine.Enabled = false;

            Recognizer.UpdateBoardImage();

            bool wasFinished = IsFinishedPosition();

            _vm.Game = new ChessGame();

            _chessPanel.Game = _vm.Game;
            _vm.PositionHistory.Clear();
            _dataGridViewMoves.Rows.Clear();
            _chessPanel.HighlighedSquares.Clear();
            _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen()));
            _vm.NavigationIndex = 0;
            _vm.Engine.Query("ucinewgame");

            if (_menuItemLocalMode.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = _menuItemFlipBoard.Checked;
            }

            if (wasFinished)
            {
                OnEngineStopped();
            }
            else
            {
                _vm.Engine.RequestStop();
            }

            _chessPanel.Invalidate();
        }

        private void OnMenuItemComputerMoveClick(object sender, EventArgs e)
        {
            _computerPlayer = _vm.Game.WhoseTurn;
            _timeSinceLastMove.Restart();
            _timerEngine.Enabled = true;
        }

        private void OnMenuItemAutoPlayClick(object sender, EventArgs e)
        {
            _menuItemCheatMode.Checked = false;

            _isAutoPlay = !_isAutoPlay;
            _menuItemAutoPlay.Checked = _isAutoPlay;
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

            FormSquareColorDialog squareColorDialog = new FormSquareColorDialog(images);
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
            string input = Interaction.InputBox("Enter MultiPV:", "BerldChess - MultiPV");
            int multiPV;

            if (int.TryParse(input, out multiPV))
            {
                if (multiPV > 0 && multiPV <= 250)
                {
                    SerializedInfo.Instance.MultiPV = multiPV;
                    _menuItemMultiPv.Text = $"MultiPV [{SerializedInfo.Instance.MultiPV}]";
                    _vm.Engine.Query($"setoption name MultiPV value {SerializedInfo.Instance.MultiPV}");
                    _vm.Engine.Query("stop");
                    _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

                    _chessPanel.Invalidate();
                    ResetDataGridViewRows(multiPV);
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
            Clipboard.SetText(_vm.PositionHistory[_vm.NavigationIndex].FEN);
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
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            Pen gridPen = new Pen(_panelEvaluationChart.ForeColor.OperateAll(200, !SerializedInfo.Instance.DarkMode), 1);
            Pen middleLinePen = new Pen(_panelEvaluationChart.ForeColor.OperateAll(120, !SerializedInfo.Instance.DarkMode), 1);
            Pen chartLinePen = new Pen(_panelEvaluationChart.ForeColor, 1);

            Font font = new Font("Segoe UI", 10);
            SolidBrush fontBrush = new SolidBrush(_panelEvaluationChart.ForeColor);
            int chartYMiddle = Round(_panelEvaluationChart.Height / 2.0);

            int peak;
            double highestValue = _vm.PositionHistory.Max(c => c.Evaluation);
            double lowestValue = _vm.PositionHistory.Min(c => c.Evaluation);

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
            else if (peak > 10)
            {
                peak = 10;
            }

            int rowCount = peak * 2;
            int lineCount = 6;

            double xPart = _panelEvaluationChart.Width / (double)lineCount;
            double yPart = _panelEvaluationChart.Height / (double)rowCount;

            for (int i = 1; i < rowCount; i++)
            {
                g.DrawLine(gridPen, 0, Round(yPart * i), _panelEvaluationChart.Width, Round(yPart * i));
            }

            for (int i = 1; i < lineCount; i++)
            {
                g.DrawLine(gridPen, Round(xPart * i), 0, Round(xPart * i), _panelEvaluationChart.Height);
            }

            for (int i = 1; i <= lineCount; i++)
            {
                if (i % 3 != 0)
                {
                    continue;
                }

                int valueX = (int)((_vm.PositionHistory.Count - 1) / (double)lineCount * i);

                if (valueX > 2 || i == lineCount)
                {
                    string output = valueX.ToString();
                    int offSet = (int)((g.MeasureString(output, font).Width) / lineCount * i);

                    g.DrawString(output, font, fontBrush, (_panelEvaluationChart.Width / lineCount * i) - offSet, _panelEvaluationChart.Height - font.Size * 2);
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
                string output = valueY.ToString();

                int offSet = Round(g.MeasureString(output, font).Height * 1.1 / 2);

                g.DrawString(output, font, fontBrush, font.Size / 2, Round(_panelEvaluationChart.Height / (double)rowCount * i) - offSet);
            }

            g.DrawLine(middleLinePen, 0, chartYMiddle, _panelEvaluationChart.Width, chartYMiddle);

            double upperBound = 10;
            double lowerBound = -9.7;

            double yUnitLength = _panelEvaluationChart.Height / (double)rowCount;
            double xUnitLength = (double)_panelEvaluationChart.Width / (_vm.PositionHistory.Count - 1);

            if (peak != 0)
            {
                for (int i = 0; i < _vm.PositionHistory.Count - 1; i++)
                {
                    double startValue = _vm.PositionHistory[i].Evaluation;
                    double endValue = _vm.PositionHistory[i + 1].Evaluation;

                    if (startValue > upperBound)
                    {
                        startValue = upperBound;
                    }
                    else if (startValue < lowerBound)
                    {
                        startValue = lowerBound;
                    }

                    if (endValue > upperBound)
                    {
                        endValue = upperBound;
                    }
                    else if (endValue < lowerBound)
                    {
                        endValue = lowerBound;
                    }

                    g.DrawLine(chartLinePen, Round(i * xUnitLength), -Round(startValue * yUnitLength) + chartYMiddle, Round((i + 1) * xUnitLength), -Round(endValue * yUnitLength) + chartYMiddle);
                }
            }
        }

        private void OnDataGridViewMovesKeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void OnDataGridViewMovesCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Navigate(e.RowIndex * 2 + e.ColumnIndex + 1);
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
                }
            }
        }

        #endregion

        #region Other Methods

        private void SetDarkMode(Control control, bool darkMode)
        {
            List<Control> controls = GetAll(control).ToList();
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

                if(controls[i] is DataGridView)
                {
                    DataGridView grid = (DataGridView)controls[i];

                    if(darkMode)
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

        public IEnumerable<Control> GetAll(Control control)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl)).Concat(controls);
        }

        private void LoadXml()
        {
            try
            {
                if (File.Exists(FormMainViewModel.ConfigurationFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializedInfo));
                    FileStream fileStream = new FileStream(FormMainViewModel.ConfigurationFileName, FileMode.Open);

                    SerializedInfo.Instance = (SerializedInfo)serializer.Deserialize(fileStream);
                    fileStream.Dispose();

                    Bounds = SerializedInfo.Instance.Bounds;

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
                        _chessPanel.PieceSizeFactor = 1.05;
                    }

                    _chessPanel.DarkSquare = SerializedInfo.Instance.BoardDarkSquare;
                    _chessPanel.LightSquare = SerializedInfo.Instance.BoardLightSquare;

                    _menuItemHideOutput.Checked = SerializedInfo.Instance.HideOutput;
                    _menuItemHideArrows.Checked = SerializedInfo.Instance.HideArrows;

                    _splitContainerMain.Panel2Collapsed = _menuItemHideOutput.Checked;

                    _chessPanel.DisplayGridBorders = SerializedInfo.Instance.DisplayGridBorder;
                    _menuItemFlipBoard.Checked = SerializedInfo.Instance.BoardFlipped;
                    _menuItemLocalMode.Checked = SerializedInfo.Instance.LocalMode;
                    _menuItemCheatMode.Checked = SerializedInfo.Instance.CheatMode;
                    _menuItemCheckAuto.Checked = SerializedInfo.Instance.AutoCheck;
                    _engineTime = SerializedInfo.Instance.EngineTime;
                    _menuItemSound.Checked = SerializedInfo.Instance.Sound;
                    _animationTime = SerializedInfo.Instance.AnimationTime;

                    SetDarkMode(this, SerializedInfo.Instance.DarkMode);
                }
            }
            catch (Exception ex)
            {
                File.Delete(FormMainViewModel.ConfigurationFileName);
                Debug.WriteLine(ex.ToString());
            }

            _chessPanel.Invalidate();
        }

        private void SaveXml()
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

        private void ResetDataGridViewRows(int newRowCount)
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

        private void ResetDataGridViewColumns(InfoType[] columns)
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

        private void Navigate(int navIndex)
        {
            if (navIndex != _vm.NavigationIndex && navIndex < _vm.PositionHistory.Count)
            {
                SelectMovesCell(navIndex);

                _isAutoPlay = false;
                _timeSinceLastMove.Reset();
                _computerPlayer = ChessPlayer.None;
                _timerEngine.Enabled = false;

                bool wasFinished = IsFinishedPosition();

                _chessPanel.HighlighedSquares.Clear();
                _vm.Game = new ChessGame(_vm.PositionHistory[navIndex].FEN);
                _chessPanel.Game = _vm.Game;

                string previousMove = _vm.PositionHistory[navIndex].Move;

                if (previousMove != null)
                {
                    Point[] squareLocations = _chessPanel.GetRelPositionsFromMoveString(previousMove);

                    _chessPanel.HighlighedSquares.AddRange(squareLocations);
                }


                _evaluationEnabled = false;

                _vm.NavigationIndex = navIndex;

                if (_menuItemLocalMode.Checked)
                {
                    _chessPanel.IsFlipped = _vm.NavigationIndex % 2 != 0;
                    _chessPanel.Invalidate();
                }

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
                    return TimeSpan.FromMilliseconds(int.Parse(data)).ToString(@"h\:mm\:ss");

                case InfoType.Nodes:
                case InfoType.NPS:
                    return GetKiloFormat(long.Parse(data));

                case InfoType.PV:
                    break;

                case InfoType.MultiPV:
                    break;

                case InfoType.Score:
                    break;
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

            double relativeDiff = difference / 70.0;

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
            bool areSame = AreSame(_currImg, _comparisonSnap);

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

                _isMoveTry = true;

                FigureMovedEventArgs args = new FigureMovedEventArgs(source, destination);
                OnPieceMoved(null, args);
            }

            _comparisonSnap = _currImg;
        }

        private bool AreSame(Bitmap bitmap1, Bitmap bitmap2)
        {
            bool equals = true;
            Rectangle rect = new Rectangle(0, 0, bitmap1.Width, bitmap1.Height);
            BitmapData bmpData1 = bitmap1.LockBits(rect, ImageLockMode.ReadOnly, bitmap1.PixelFormat);
            BitmapData bmpData2 = bitmap2.LockBits(rect, ImageLockMode.ReadOnly, bitmap2.PixelFormat);
            unsafe
            {
                byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                int width = rect.Width * 3; // for 24bpp pixel data
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
                    ptr1 += bmpData1.Stride - width;
                    ptr2 += bmpData2.Stride - width;
                }
            }
            bitmap1.UnlockBits(bmpData1);
            bitmap2.UnlockBits(bmpData2);

            return equals;
        }

        private bool IsFinishedPosition()
        {
            return _vm.Game.IsCheckmated(ChessPlayer.Black)
                || _vm.Game.IsCheckmated(ChessPlayer.White)
                || _vm.Game.IsStalemated(ChessPlayer.Black)
                || _vm.Game.IsStalemated(ChessPlayer.White)
                || _vm.Game.IsDraw();
        }

        private void SetDoubleBuffered(Control control)
        {
            if (SystemInformation.TerminalServerSession)
            {
                return;
            }

            PropertyInfo propertyInfo = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfo.SetValue(control, true, null);
        }

        #endregion
    }
}