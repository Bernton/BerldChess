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
using System.Drawing.Imaging;
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

        private volatile bool _evalUpdate = true;
        private bool _isMoveTry = false;
        private bool _isAutoPlay = false;
        private volatile bool _updateAfterAnimation = false;
        private int _multiPV1Reference;
        private int _engineTime = 300;
        private int _animationTime = 300;
        private ChessPlayer _computerPlayer = ChessPlayer.None;
        private InfoType[] _columnOrder;
        private Random _random = new Random();
        private FormMainViewModel _vm;
        private PieceSelectionDialog _pieceDialog;
        private ChessPanel _chessPanel;
        private SoundPlayer _movePlayer = new SoundPlayer(Resources.Move);
        private SoundPlayer _castlingPlayer = new SoundPlayer(Resources.Castling);
        private SoundPlayer _capturePlayer = new SoundPlayer(Resources.Capture);
        private SoundPlayer _illegalPlayer = new SoundPlayer(Resources.Ilegal);
        private InputSimulator _inputSimulator = new InputSimulator();
        private Stopwatch _timeSinceLastMove = new Stopwatch();
        private Bitmap _animationTestImage = null;

        #endregion

        #region Constructors

        public FormMain()
        {
            InitializeComponent();
            SetDoubleBuffered(_panelEvalChart);
            SetDoubleBuffered(_dataGridViewMoves);
            InitializeWindow();
            InitializeViewModel();
            InitializeChessBoard();
            LoadXMLConfiguration();

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

            ResetDataGridColumn(_columnOrder);
            ResetDataGridRows(SerializedInfo.Instance.MultiPV);

            _vm.Engine.Query($"setoption name MultiPV value {SerializedInfo.Instance.MultiPV}");
            _vm.Engine.Query("stop");
            _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

            Recognizer.SearchBoard(SerializedInfo.Instance.LightSquare, SerializedInfo.Instance.DarkSquare);
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
            _splitContainerOuter.Panel1.Controls.Add(_chessPanel);
        }

        #endregion

        #region Event Methods

        private void OnEngineStopped()
        {
            EmptyDataGrid();
            _chessPanel.Arrows.Clear();
            _chessPanel.Invalidate();

            _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                Invoke((MethodInvoker)delegate
                {
                    _labelEval.Text = "Checkmate";
                    _labelEval.ForeColor = Color.Black;
                });

                EmptyDataGrid();
            }
            else if (_vm.Game.IsStalemated(_vm.Game.WhoseTurn))
            {
                Invoke((MethodInvoker)delegate
                {
                    _labelEval.Text = "Stalemate";
                    _labelEval.ForeColor = Color.Black;
                });

                EmptyDataGrid();
            }
            else if (_vm.Game.IsDraw())
            {
                if (IsHandleCreated)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        _labelEval.Text = "Draw";
                        _labelEval.ForeColor = Color.Black;
                    });

                    EmptyDataGrid();
                }
            }
            else
            {
                _vm.Engine.Query("go infinite");
                _evalUpdate = true;
            }
        }

        private void OnEvaluationReceived(Evaluation evaluation)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            try
            {
                int multiPV = int.Parse(evaluation[InfoType.MultiPV]);

                Invoke((MethodInvoker)delegate
                {
                    if (_columnOrder.Length != _dataGridViewEval.Columns.Count)
                    {
                        ResetDataGridColumn(_columnOrder);
                        ResetDataGridRows(SerializedInfo.Instance.MultiPV);
                    }

                    int columnIndex;

                    for (int iEval = 0; iEval < evaluation.Types.Length; iEval++)
                    {
                        columnIndex = -1;

                        for (int iOrder = 0; iOrder < _columnOrder.Length; iOrder++)
                        {
                            if (_columnOrder[iOrder] == evaluation.Types[iEval])
                            {
                                columnIndex = iOrder;
                                break;
                            }
                        }

                        if (columnIndex != -1)
                        {
                            if (_dataGridViewEval.Rows.Count >= multiPV)
                            {
                                _dataGridViewEval.Rows[multiPV - 1].Cells[columnIndex].Value = FormatEngineInfo(evaluation.Types[iEval], evaluation[iEval]);
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
                                throw new Exception("Can't convert score");
                            }
                        }
                    }

                    if (isMate)
                    {
                        mateNumber = centipawn;

                        if (centipawn > 0)
                        {
                            centipawn += 12000 + Math.Abs(multiPV - SerializedInfo.Instance.MultiPV) * 10;
                        }
                        else
                        {
                            centipawn -= 12000 - Math.Abs(multiPV - SerializedInfo.Instance.MultiPV) * 10;
                        }
                    }

                    if (multiPV == 1)
                    {
                        if (isMate)
                        {
                            if (mateNumber > 0)
                            {
                                _labelEval.Text = $"Mate in {mateNumber}";
                                _labelEval.ForeColor = Color.Green;
                            }
                            else
                            {
                                _labelEval.Text = $"Mated in {Math.Abs(mateNumber)}";
                                _labelEval.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            double eval;

                            if (_vm.Game.WhoseTurn == ChessPlayer.Black)
                            {
                                eval = (-(double)centipawn / 100.0);
                            }
                            else
                            {
                                eval = ((double)centipawn / 100.0);
                            }

                            if (_vm.PositionHistory.Count > _vm.NavIndex && _vm.PositionHistory.Count > 1 && _evalUpdate)
                            {
                                int depth = int.Parse(evaluation[InfoType.Depth]);
                                if (depth >= _vm.PositionHistory[_vm.NavIndex].EvaluationDepth)
                                {
                                    _vm.PositionHistory[_vm.NavIndex].Evaluation = eval;
                                    _vm.PositionHistory[_vm.NavIndex].EvaluationDepth = int.Parse(evaluation[InfoType.Depth]);

                                    int gridIndex = _vm.NavIndex - 1;

                                    if (gridIndex >= 0 && _dataGridViewMoves.Rows.Count > gridIndex / 2 && _dataGridViewMoves.Rows[gridIndex / 2].Cells.Count > gridIndex % 2)
                                    {
                                        string newEvalPart;

                                        if (isMate)
                                        {
                                            newEvalPart = "#";
                                        }
                                        else if(eval == 0)
                                        {
                                            newEvalPart = "Even";
                                        }
                                        else
                                        {
                                            newEvalPart = eval.ToString("+0.00;-0.00");
                                        }

                                        string currentValue = (string)_dataGridViewMoves.Rows[gridIndex / 2].Cells[gridIndex % 2].Value;
                                        _dataGridViewMoves.Rows[gridIndex / 2].Cells[gridIndex % 2].Value = currentValue.Substring(0, currentValue.IndexOf('(') + 1) + newEvalPart + " D" + depth + ")";
                                    }
                                }
                            }

                            if (eval == 0)
                            {
                                _labelEval.Text = "Even";
                            }
                            else
                            {
                                _labelEval.Text = eval.ToString("+0.00;-0.00");
                            }

                            _labelEval.ForeColor = CalculateEvaluationColor(-(centipawn / 100.0));
                        }
                    }

                    if (hideArrowsToolStripMenuItem.Checked)
                    {
                        return;
                    }

                    if ((!isMate && _multiPV1Reference - centipawn < 150) || multiPV == 1 || _chessPanel.Arrows.Count < 3)
                    {
                        if (multiPV == 1)
                        {
                            _multiPV1Reference = centipawn;
                        }

                        if (_chessPanel.Arrows.Count < multiPV)
                        {
                            _chessPanel.Arrows.Add(new Arrow((evaluation[InfoType.PV]).Substring(0, 4), 0.9, GetReferenceColor(centipawn, _multiPV1Reference)));
                        }
                        else
                        {
                            _chessPanel.Arrows[multiPV - 1].Move = (evaluation[InfoType.PV]).Substring(0, 4);
                            _chessPanel.Arrows[multiPV - 1].Color = GetReferenceColor(centipawn, _multiPV1Reference);
                        }

                        if (multiPV == 1)
                        {
                            _chessPanel.Arrows[multiPV - 1].Color = Color.DarkBlue;
                        }
                    }
                    else
                    {
                        if (_chessPanel.Arrows.Count >= multiPV)
                        {
                            _chessPanel.Arrows.RemoveRange(multiPV, _chessPanel.Arrows.Count - multiPV);
                        }
                    }
                });
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_computerPlayer != ChessPlayer.None)
            {
                if (_vm.Game.WhoseTurn == _computerPlayer)
                {
                    if (!IsFinishedPosition())
                    {
                        if (_timeSinceLastMove.ElapsedMilliseconds > _engineTime && _dataGridViewEval.Rows.Count > 0)
                        {
                            if ((string)_dataGridViewEval.Rows[0].Cells[6].Value == "")
                            {
                                return;
                            }

                            string move = ((string)_dataGridViewEval.Rows[0].Cells[6].Value).Substring(0, 5);

                            Point[] positions = _chessPanel.GetRelPositionsFromMoveString((move));
                            FigureMovedEventArgs args = new FigureMovedEventArgs(positions[0], positions[1]);

                            OnPieceMoved(null, args);

                            if (cheatModeToolStripMenuItem.Checked)
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

                                    if (checkAutoToolStripMenuItem.Checked)
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

                if (localModeToolStripMenuItem.Checked)
                {
                    _chessPanel.IsFlipped = !_chessPanel.IsFlipped;
                    _chessPanel.Invalidate();
                }

                moveType = _vm.Game.ApplyMove(move, true);

                if (soundToolStripMenuItem.Checked)
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

                if (_vm.NavIndex != _vm.PositionHistory.Count - 1)
                {
                    int countCache = _vm.PositionHistory.Count;

                    _vm.PositionHistory.RemoveRange(_vm.NavIndex + 1, _vm.PositionHistory.Count - _vm.NavIndex - 1);

                    for (int i = countCache - 2; i >= _vm.NavIndex; i--)
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

                _evalUpdate = false;

                _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen(), 0.0, move.ToString("")));

                int count = _vm.PositionHistory.Count;
                string displayEval = _labelEval.Text;

                if (displayEval.Substring(0, 4) == "Mate")
                {
                    displayEval = "#";
                }

                if (count % 2 == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell whiteMove = new DataGridViewTextBoxCell();
                    DataGridViewTextBoxCell blackMove = new DataGridViewTextBoxCell();

                    whiteMove.Value = (count / 2) + ". " + GetFormatMove(move, moveType, legalMoves) + "\n(" + displayEval + ")";

                    row.Cells.Add(whiteMove);
                    row.Cells.Add(blackMove);

                    row.Height = 40;
                    _dataGridViewMoves.Rows.Add(row);
                }
                else
                {
                    _dataGridViewMoves.Rows[count / 2 - 1].Cells[1].Value = GetFormatMove(move, moveType, legalMoves) + "\n(" + displayEval + ")";
                }

                _vm.NavIndex++;
                SelectCell(_vm.NavIndex);

                if (_vm.Game.IsCheckmated(ChessPlayer.Black) || _vm.Game.IsCheckmated(ChessPlayer.White))
                {
                    _labelEval.Text = "Checkmate";
                }
                else if (_vm.Game.IsStalemated(ChessPlayer.Black) || _vm.Game.IsStalemated(ChessPlayer.White))
                {
                    _labelEval.Text = "Stalemate";
                }
                else if (_vm.Game.IsDraw())
                {
                    _labelEval.Text = "Draw";
                }

                _timeSinceLastMove.Restart();

                _vm.Engine.RequestStop();
                _chessPanel.Invalidate();
            }
            else
            {
                if (!_isMoveTry && soundToolStripMenuItem.Checked)
                {
                    _illegalPlayer.Play();
                }
            }
        }

        private void OnButtonLoadFenClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Position FEN:", "BerldChess - FEN");

            if (input == "")
            {
                return;
            }

            _isAutoPlay = false;
            _timeSinceLastMove.Reset();
            _computerPlayer = ChessPlayer.None;
            _engineTimer.Enabled = false;

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
            _vm.NavIndex = 0;

            if (localModeToolStripMenuItem.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = flipBoardToolStripMenuItem.Checked;
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

        private void OnButtonBackClick(object sender, EventArgs e)
        {
            if (_vm.NavIndex > 0)
            {
                NavigateGame(_vm.NavIndex - 1);
            }
        }

        private void OnButtonForwardClick(object sender, EventArgs e)
        {
            if (_vm.NavIndex != _vm.PositionHistory.Count - 1)
            {
                NavigateGame(_vm.NavIndex + 1);
            }
        }

        private void OnButtonStartClick(object sender, EventArgs e)
        {
            NavigateGame(0);
        }

        private void OnButtonLatestClick(object sender, EventArgs e)
        {
            NavigateGame(_vm.PositionHistory.Count - 1);
        }

        private void OnFormMainClosing(object sender, FormClosingEventArgs e)
        {
            SaveXMLConfiguration();
            _vm.Engine.Dispose();
        }

        private void OnCheckBoxGridBorderCheckedChanged(object sender, EventArgs e)
        {
            gridBorderToolStripMenuItem.Checked = !gridBorderToolStripMenuItem.Checked;
            _chessPanel.DisplayGridBorders = gridBorderToolStripMenuItem.Checked;
            _chessPanel.Invalidate();
        }

        private void OnCheckBoxFlippedCheckedChanged(object sender, EventArgs e)
        {
            flipBoardToolStripMenuItem.Checked = !flipBoardToolStripMenuItem.Checked;

            if (!localModeToolStripMenuItem.Checked)
            {
                _chessPanel.IsFlipped = flipBoardToolStripMenuItem.Checked;
                _chessPanel.Invalidate();
            }
        }

        private void OnCheckBoxHideArrowsCheckedChanged(object sender, EventArgs e)
        {
            hideArrowsToolStripMenuItem.Checked = !hideArrowsToolStripMenuItem.Checked;

            if (hideArrowsToolStripMenuItem.Checked && _chessPanel != null && _chessPanel.Arrows != null)
            {
                _chessPanel.Arrows.Clear();
                _chessPanel.Invalidate();
            }
        }

        private void OnCheckBoxHideOutputCheckedChanged(object sender, EventArgs e)
        {
            hideOutputToolStripMenuItem.Checked = !hideOutputToolStripMenuItem.Checked;
            _splitContainerOuter.Panel2Collapsed = hideOutputToolStripMenuItem.Checked;
        }

        private void OnSlowTimerTick(object sender, EventArgs e)
        {
            _panelEvalChart.Invalidate();
            _chessPanel.Invalidate();
        }

        private void OnCheckBoxPlayModeCheckedChanged(object sender, EventArgs e)
        {
            localModeToolStripMenuItem.Checked = !localModeToolStripMenuItem.Checked;
            flipBoardToolStripMenuItem.Enabled = !localModeToolStripMenuItem.Checked;

            if (localModeToolStripMenuItem.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = flipBoardToolStripMenuItem.Checked;
            }

            _chessPanel.Invalidate();
        }

        private void OnNewClick(object sender, EventArgs e)
        {
            _isAutoPlay = false;
            _timeSinceLastMove.Reset();
            _computerPlayer = ChessPlayer.None;
            _engineTimer.Enabled = false;

            Recognizer.UpdateBoardImage();

            bool wasFinished = IsFinishedPosition();

            _vm.Game = new ChessGame();

            _chessPanel.Game = _vm.Game;
            _vm.PositionHistory.Clear();
            _dataGridViewMoves.Rows.Clear();
            _chessPanel.HighlighedSquares.Clear();
            _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen()));
            _vm.NavIndex = 0;
            _vm.Engine.Query("ucinewgame");

            if (localModeToolStripMenuItem.Checked)
            {
                _chessPanel.IsFlipped = !(_vm.Game.WhoseTurn == ChessPlayer.White);
            }
            else
            {
                _chessPanel.IsFlipped = flipBoardToolStripMenuItem.Checked;
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

        private void OnButtonComputerMoveClick(object sender, EventArgs e)
        {
            _computerPlayer = _vm.Game.WhoseTurn;
            _timeSinceLastMove.Restart();
            _engineTimer.Enabled = true;
        }

        private void OnButtonAutoPlayClick(object sender, EventArgs e)
        {
            cheatModeToolStripMenuItem.Checked = false;

            _isAutoPlay = !_isAutoPlay;
            autoplayToolStripMenuItem.Checked = _isAutoPlay;
        }

        private void OnButtonMoveRecClick(object sender, EventArgs e)
        {
            AutoMove();
        }

        private void OnButtonUpdateRecClick(object sender, EventArgs e)
        {
            if (!Recognizer.BoardFound)
            {
                if (!Recognizer.SearchBoard(SerializedInfo.Instance.LightSquare, SerializedInfo.Instance.DarkSquare))
                {
                    MessageBox.Show("No board found!", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                Recognizer.UpdateBoardImage();
            }
        }

        private void OnButtonColorDialogClick(object sender, EventArgs e)
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
                SerializedInfo.Instance.DarkSquare = (Color)squareColorDialog.DarkSquareColor;
            }

            if (squareColorDialog.LightSquareColor != null)
            {
                SerializedInfo.Instance.LightSquare = (Color)squareColorDialog.LightSquareColor;
            }
        }

        private void OnButtonResetClick(object sender, EventArgs e)
        {
            if (!Recognizer.SearchBoard(SerializedInfo.Instance.LightSquare, SerializedInfo.Instance.DarkSquare))
            {
                MessageBox.Show("No board found!", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                _engineTimer.Enabled = true;
            }

            if (checkAutoToolStripMenuItem.Checked && cheatModeToolStripMenuItem.Checked)
            {
                AutoMove();
            }
        }

        private void OnButtonAlterPiecesClick(object sender, EventArgs e)
        {
            _pieceDialog.ShowDialog();
        }

        private void OnDialogFontSelected()
        {
            ChessFont font = SerializedInfo.Instance.ChessFonts[SerializedInfo.Instance.SelectedFontIndex];

            _chessPanel.IsUnicodeFont = font.IsUnicode;
            _chessPanel.ChessFontChars = font.PieceCharacters;
            _chessPanel.PieceSizeFactor = font.SizeFactor;
            _chessPanel.PieceFontFamily = font.FontFamily;
        }

        private void OnFormMainLoad(object sender, EventArgs e)
        {
            engineTimeToolStripMenuItem.Text = $"Enginetime [{_engineTime}]";
            multiPVToolStripMenuItem.Text = $"MultiPV [{SerializedInfo.Instance.MultiPV}]";
            animTimeToolStripMenuItem.Text = $"Anim Time [{_animationTime}]";
            clickDelayToolStripMenuItem.Text = $"Click Delay [{SerializedInfo.Instance.ClickDelay}]";

            if (SerializedInfo.Instance.IsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void OnSoundToolStripMenuItemClick(object sender, EventArgs e)
        {
            soundToolStripMenuItem.Checked = !soundToolStripMenuItem.Checked;
        }

        private void OnEngineTimeToolStripMenuItemClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Engine Time:", "BerldChess - Engine Time");
            int engineTime;

            if (int.TryParse(input, out engineTime))
            {
                _engineTime = engineTime;
                engineTimeToolStripMenuItem.Text = $"Enginetime [{_engineTime}]";
            }
        }

        private void OnMultiPVToolStripMenuItemClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter MultiPV:", "BerldChess - MultiPV");
            int multiPV;

            if (int.TryParse(input, out multiPV))
            {
                if (multiPV > 0 && multiPV <= 250)
                {
                    SerializedInfo.Instance.MultiPV = multiPV;
                    multiPVToolStripMenuItem.Text = $"MultiPV [{SerializedInfo.Instance.MultiPV}]";
                    _vm.Engine.Query($"setoption name MultiPV value {SerializedInfo.Instance.MultiPV}");
                    _vm.Engine.Query("stop");
                    _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

                    _chessPanel.Invalidate();
                    ResetDataGridRows(multiPV);
                }
            }
        }

        private void OnCheatModeToolStripMenuItemClick(object sender, EventArgs e)
        {
            cheatModeToolStripMenuItem.Checked = !cheatModeToolStripMenuItem.Checked;

            foreach (ToolStripDropDownItem item in cheatModeToolStripMenuItem.DropDownItems)
            {
                if (cheatModeToolStripMenuItem.Checked)
                {
                    item.Enabled = true;
                }
                else
                {
                    item.Enabled = false;
                }
            }
        }

        private void OnAnimTimeToolStripMenuItemClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Animation Time:", "BerldChess - Animation Time");
            int animTime;

            if (int.TryParse(input, out animTime))
            {
                _animationTime = animTime;
                animTimeToolStripMenuItem.Text = $"Anim Time [{_animationTime}]";
            }
        }

        private void OnCheckAutoToolStripMenuItemClick(object sender, EventArgs e)
        {
            checkAutoToolStripMenuItem.Checked = !checkAutoToolStripMenuItem.Checked;
        }

        private void OnCopyFENToClipboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            Clipboard.SetText(_vm.PositionHistory[_vm.NavIndex].FEN);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            foreach (ToolStripMenuItem item in GetItems(menuStrip))
            {
                if (string.IsNullOrEmpty(item.ShortcutKeyDisplayString))
                {
                    continue;
                }

                Keys key = (Keys)Enum.Parse(typeof(Keys), item.ShortcutKeyDisplayString);

                if (e.KeyCode == key)
                {
                    item.PerformClick();
                }
            }
        }

        private void OnPanelEvalChartPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            Pen gridPen = new Pen(Color.LightGray, 1);
            Pen middleLinePen = new Pen(Color.Gray, 1);
            Pen chartLinePen = new Pen(Color.Black, 1);

            Font font = new Font("Segue UI", 11);
            int chartYMiddle = Round(_panelEvalChart.Height / 2.0);

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

            double xPart = _panelEvalChart.Width / (double)lineCount;
            double yPart = _panelEvalChart.Height / (double)rowCount;

            for (int i = 1; i < rowCount; i++)
            {
                g.DrawLine(gridPen, 0, Round(yPart * i), _panelEvalChart.Width, Round(yPart * i));
            }

            for (int i = 1; i < lineCount; i++)
            {
                g.DrawLine(gridPen, Round(xPart * i), 0, Round(xPart * i), _panelEvalChart.Height);
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

                    g.DrawString(output, font, Brushes.Black, (_panelEvalChart.Width / lineCount * i) - offSet, _panelEvalChart.Height - font.Size * 2);
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

                g.DrawString(output, font, Brushes.Black, font.Size / 2, Round(_panelEvalChart.Height / (double)rowCount * i) - offSet);
            }

            g.DrawLine(middleLinePen, 0, chartYMiddle, _panelEvalChart.Width, chartYMiddle);

            double upperBound = 10;
            double lowerBound = -9.7;

            double yUnitLength = _panelEvalChart.Height / (double)rowCount;
            double xUnitLength = (double)_panelEvalChart.Width / (_vm.PositionHistory.Count - 1);

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

        private void OnClickDelayToolStripMenuItemClick(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Click Delay:", "BerldChess -Click Delay");
            int clickDelay;

            if (int.TryParse(input, out clickDelay))
            {
                SerializedInfo.Instance.ClickDelay = clickDelay;
                clickDelayToolStripMenuItem.Text = $"Click Delay [{clickDelay}]";
            }
        }

        private void OnDataGridViewMovesKeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void OnDataGridViewMovesCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            NavigateGame(e.RowIndex * 2 + e.ColumnIndex + 1);
        }

        private void loadFromPGNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPgnLoader pgnLoader = new FormPgnLoader();
            pgnLoader.ShowDialog();
        }

        #endregion

        #region Other Methods

        private void LoadXMLConfiguration()
        {
            try
            {
                if (File.Exists(FormMainViewModel.ConfigFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializedInfo));
                    FileStream fileStream = new FileStream(FormMainViewModel.ConfigFileName, FileMode.Open);

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

                    hideOutputToolStripMenuItem.Checked = SerializedInfo.Instance.HideOutput;
                    hideArrowsToolStripMenuItem.Checked = SerializedInfo.Instance.HideArrows;
                    gridBorderToolStripMenuItem.Checked = SerializedInfo.Instance.DisplayGridBorder;
                    flipBoardToolStripMenuItem.Checked = SerializedInfo.Instance.BoardFlipped;
                    localModeToolStripMenuItem.Checked = SerializedInfo.Instance.LocalMode;
                    cheatModeToolStripMenuItem.Checked = SerializedInfo.Instance.CheatMode;
                    checkAutoToolStripMenuItem.Checked = SerializedInfo.Instance.AutoCheck;
                    _engineTime = SerializedInfo.Instance.EngineTime;
                    soundToolStripMenuItem.Checked = SerializedInfo.Instance.Sound;
                    _animationTime = SerializedInfo.Instance.AnimationTime;
                }
            }
            catch (Exception ex)
            {
                File.Delete(FormMainViewModel.ConfigFileName);
                Debug.WriteLine(ex.ToString());
            }

            _chessPanel.Invalidate();
        }

        private void SaveXMLConfiguration()
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
                SerializedInfo.Instance.DisplayGridBorder = gridBorderToolStripMenuItem.Checked;
                SerializedInfo.Instance.BoardFlipped = flipBoardToolStripMenuItem.Checked;
                SerializedInfo.Instance.HideArrows = hideArrowsToolStripMenuItem.Checked;
                SerializedInfo.Instance.HideOutput = hideOutputToolStripMenuItem.Checked;
                SerializedInfo.Instance.LocalMode = localModeToolStripMenuItem.Checked;
                SerializedInfo.Instance.CheatMode = cheatModeToolStripMenuItem.Checked;
                SerializedInfo.Instance.EngineTime = _engineTime;
                SerializedInfo.Instance.Sound = soundToolStripMenuItem.Checked;
                SerializedInfo.Instance.AutoCheck = checkAutoToolStripMenuItem.Checked;
                SerializedInfo.Instance.AnimationTime = _animationTime;

                XmlSerializer serializer = new XmlSerializer(typeof(SerializedInfo));
                FileStream fileStream = new FileStream(FormMainViewModel.ConfigFileName, FileMode.Create);
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

        private void ResetDataGridRows(int newRowCount)
        {
            _dataGridViewEval.Rows.Clear();

            for (int i = 0; i < newRowCount - 1; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                for (int iCell = 0; iCell < _dataGridViewEval.Columns.Count; iCell++)
                {
                    DataGridViewTextBoxCell textCell = new DataGridViewTextBoxCell();
                    textCell.Value = string.Empty;

                    row.Cells.Add(textCell);
                }

                _dataGridViewEval.Rows.Add(row);
            }
        }

        private void ResetDataGridColumn(InfoType[] columns)
        {
            _dataGridViewEval.Rows.Clear();
            _dataGridViewEval.Columns.Clear();

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
                _dataGridViewEval.Columns.Add(column);
            }
        }

        private string GetFormatMove(Move move, MoveType moveType, ReadOnlyCollection<Move> legalMoves)
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

            if (pieceCharacter != 'P' && moveType != (MoveType.Move |MoveType.Promotion) && moveType != (MoveType.Move | MoveType.Capture | MoveType.Promotion))
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
            else if(moveType == (MoveType.Move | MoveType.Capture) || moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += move.OriginalPosition.File.ToString().ToLowerInvariant();
            }

            //formatMove += move.OriginalPosition.ToString();

            if (moveType == (MoveType.Move | MoveType.Capture) || moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
            {
                formatMove += 'x';
            }

            formatMove += move.NewPosition.ToString();

            if(moveType == (MoveType.Move | MoveType.Promotion) || moveType == (MoveType.Move | MoveType.Capture | MoveType.Promotion))
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

        private void NavigateGame(int navIndex)
        {
            if (navIndex != _vm.NavIndex)
            {
                SelectCell(navIndex);

                _isAutoPlay = false;
                _timeSinceLastMove.Reset();
                _computerPlayer = ChessPlayer.None;
                _engineTimer.Enabled = false;

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


                _evalUpdate = false;

                _vm.NavIndex = navIndex;

                if (localModeToolStripMenuItem.Checked)
                {
                    _chessPanel.IsFlipped = _vm.NavIndex % 2 != 0;
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

        private void SelectCell(int navIndex)
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

        private string KiloFormat(long number)
        {
            if (number >= 10000)
            {
                return (number / 1000D).ToString("#,##0") + " k";
            }

            return number.ToString("#");
        }

        private string FormatEngineInfo(InfoType type, string data)
        {
            switch (type)
            {
                case InfoType.Time:
                    return TimeSpan.FromMilliseconds(int.Parse(data)).ToString(@"h\:mm\:ss");

                case InfoType.Nodes:
                case InfoType.NPS:
                    return KiloFormat(long.Parse(data));

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

            return Color.FromArgb((int)(x * 255), (int)((1 - x) * 255), 0);
        }

        private List<ToolStripMenuItem> GetItems(MenuStrip menuStrip)
        {
            List<ToolStripMenuItem> myItems = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem i in menuStrip.Items)
            {
                GetMenuItems(i, myItems);
            }
            return myItems;
        }

        private void GetMenuItems(ToolStripMenuItem item, List<ToolStripMenuItem> items)
        {
            items.Add(item);
            foreach (ToolStripItem i in item.DropDownItems)
            {
                if (i is ToolStripMenuItem)
                {
                    GetMenuItems((ToolStripMenuItem)i, items);
                }
            }
        }

        private void EmptyDataGrid()
        {
            for (int rowI = 0; rowI < _dataGridViewEval.Rows.Count; rowI++)
            {
                for (int cellI = 0; cellI < _dataGridViewEval.Rows[rowI].Cells.Count; cellI++)
                {
                    _dataGridViewEval.Rows[rowI].Cells[cellI].Value = "";
                }
            }
        }

        private void AutoMove()
        {
            if (_animationTestImage == null)
            {
                _animationTestImage = Recognizer.GetBoardSnap();
                return;
            }

            Bitmap _currImg = Recognizer.GetBoardSnap();
            bool areSame = AreSame(_currImg, _animationTestImage);

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

            _animationTestImage = _currImg;
        }

        private bool AreSame(Bitmap bmp1, Bitmap bmp2)
        {
            bool equals = true;
            Rectangle rect = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            BitmapData bmpData1 = bmp1.LockBits(rect, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(rect, ImageLockMode.ReadOnly, bmp2.PixelFormat);
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
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

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

        private void SetDoubleBuffered(Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        #endregion
    }
}