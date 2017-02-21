using BerldChess.Model;
using BerldChess.Properties;
using BerldChess.ViewModel;
using ChessDotNet;
using ChessDotNet.Pieces;
using ChessEngineInterface;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using WindowsInput;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BerldChess.View
{
    public partial class FormMain : Form
    {
        #region Fields

        private Bitmap _animTestImg = null;

        private Random _random = new Random();
        private PieceSelectionDialog _pieceDialog = new PieceSelectionDialog();
        private bool _moveTry = false;
        private SoundPlayer _movePlayer = new SoundPlayer(Resources.Move);
        private SoundPlayer _castlingPlayer = new SoundPlayer(Resources.Castling);
        private SoundPlayer _capturePlayer = new SoundPlayer(Resources.Capture);
        private SoundPlayer _illegalPlayer = new SoundPlayer(Resources.Ilegal);

        private bool _isAutoPlay = false;
        private int _engineTime = 300;
        private InputSimulator _inputSimulator = new InputSimulator();
        private Stopwatch _timeSinceLastMove = new Stopwatch();
        private ChessPlayer _computerPlayer = ChessPlayer.None;
        private int _multiPV1Reference;
        private int _animTime = 300;
        private ChessPanel _chessPanel;
        private FormMainViewModel _vm;
        private InfoType[] _columnOrder;

        private volatile bool _updateAfterAnim = false;

        #endregion

        #region Constructors

        public FormMain()
        {
            InitializeComponent();
            SetDoubleBuffered(_panelEvalChart);
            SetDoubleBuffered(_listViewMoves);
            InitializeWindow();
            InitializeViewModel();
            InitializeChessBoard();
            LoadXMLConfiguration();

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

                if (IsHandleCreated)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        //_textBoxFen.Text = _vm.Game.GetFen();
                    });
                }
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
                    if (_columnOrder.Length != _dataGridView.Columns.Count)
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
                            if (_dataGridView.Rows.Count >= multiPV)
                            {
                                _dataGridView.Rows[multiPV - 1].Cells[columnIndex].Value = FormatEngineInfo(evaluation.Types[iEval], evaluation[iEval]);
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
                            _chessPanel.Arrows.Add(new Arrow((evaluation[InfoType.PV]).Substring(0, 4), 1.25, GetReferenceColor(centipawn, _multiPV1Reference)));
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

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_computerPlayer != ChessPlayer.None)
            {
                if (_vm.Game.WhoseTurn == _computerPlayer)
                {
                    if (!IsFinishedPosition())
                    {
                        if (_timeSinceLastMove.ElapsedMilliseconds > _engineTime && _dataGridView.Rows.Count > 0)
                        {
                            if ((string)_dataGridView.Rows[0].Cells[6].Value == "")
                            {
                                return;
                            }

                            string move = ((string)_dataGridView.Rows[0].Cells[6].Value).Substring(0, 5);

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
                                    _updateAfterAnim = true;
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
                if (_moveTry)
                {
                    Recognizer.UpdateBoardImage();
                    _moveTry = false;
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
                            _listViewMoves.Items.RemoveAt(i / 2);
                        }
                        else
                        {
                            _listViewMoves.Items[i / 2].SubItems[2] = new ListViewItem.ListViewSubItem(_listViewMoves.Items[i / 2], "");
                        }
                    }
                }

                double eval = 0.00;

                if (_labelEval.Text == "Even" || _labelEval.Text == "Draw")
                {
                    eval = 0.00;
                }
                else if (_labelEval.Text.Substring(0, 5) == "Mated")
                {
                    if (_vm.Game.WhoseTurn == ChessPlayer.White)
                    {
                        eval = 120;
                    }
                    else
                    {
                        eval = -120;
                    }
                }
                else if (_labelEval.Text.Substring(0, 4) == "Mate")
                {
                    if (_vm.Game.WhoseTurn == ChessPlayer.White)
                    {
                        eval = -120;
                    }
                    else
                    {
                        eval = 120;
                    }
                }
                else
                {
                    eval = double.Parse(_labelEval.Text);
                }


                _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen(), eval, move.ToString("")));

                int count = _vm.PositionHistory.Count;

                if (count % 2 == 0)
                {
                    string[] values = new string[3];

                    values[0] = (count / 2) + ".";
                    values[1] = GetFormatMove(move, moveType);
                    values[2] = "";

                    _listViewMoves.Items.Add(new ListViewItem(values));
                    _listViewMoves.Items[_listViewMoves.Items.Count - 1].EnsureVisible();
                }
                else
                {
                    _listViewMoves.Items[count / 2 - 1].SubItems[2] = new ListViewItem.ListViewSubItem(_listViewMoves.Items[count / 2 - 1], GetFormatMove(move, moveType));
                    _listViewMoves.Items[count / 2 - 1].EnsureVisible();
                }

                if (localModeToolStripMenuItem.Checked)
                {
                    _chessPanel.IsFlipped = !_chessPanel.IsFlipped;
                    _chessPanel.Invalidate();
                }

                _vm.NavIndex++;

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
                if (!_moveTry && soundToolStripMenuItem.Checked)
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
            _listViewMoves.Items.Clear();

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
            _listViewMoves.Items.Clear();
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
            _pieceDialog.FontFamily = SerializedInfo.Instance.PieceFontFamily;
            _pieceDialog.IsUnicode = SerializedInfo.Instance.IsUnicodeFont;
            _pieceDialog.FontChars = SerializedInfo.Instance.ChessFontChars;
            _pieceDialog.SizeFactor = SerializedInfo.Instance.PieceSizeFactor;

            if (_pieceDialog.ShowDialog() == DialogResult.OK)
            {
                SerializedInfo.Instance.PieceSizeFactor = _pieceDialog.SizeFactor;
                SerializedInfo.Instance.ChessFontChars = _pieceDialog.FontChars;
                SerializedInfo.Instance.PieceFontFamily = _pieceDialog.FontFamily;
                SerializedInfo.Instance.IsUnicodeFont = _pieceDialog.IsUnicode;
                _chessPanel.PieceSizeFactor = SerializedInfo.Instance.PieceSizeFactor;
                _chessPanel.ChessFontChars = SerializedInfo.Instance.ChessFontChars;
                _chessPanel.IsUnicodeFont = SerializedInfo.Instance.IsUnicodeFont;
                _chessPanel.PieceFontFamily = SerializedInfo.Instance.PieceFontFamily;
                _chessPanel.Invalidate();
            }
        }

        private void OnDialogFontSelected()
        {
            SerializedInfo.Instance.PieceSizeFactor = _pieceDialog.SizeFactor;
            SerializedInfo.Instance.ChessFontChars = _pieceDialog.FontChars;
            SerializedInfo.Instance.PieceFontFamily = _pieceDialog.FontFamily;
            SerializedInfo.Instance.IsUnicodeFont = _pieceDialog.IsUnicode;
            _chessPanel.PieceSizeFactor = SerializedInfo.Instance.PieceSizeFactor;
            _chessPanel.ChessFontChars = SerializedInfo.Instance.ChessFontChars;
            _chessPanel.IsUnicodeFont = SerializedInfo.Instance.IsUnicodeFont;
            _chessPanel.PieceFontFamily = SerializedInfo.Instance.PieceFontFamily;
            _chessPanel.Invalidate();
        }

        private void OnFormMainLoad(object sender, EventArgs e)
        {
            engineTimeToolStripMenuItem.Text = $"Enginetime [{_engineTime}]";
            multiPVToolStripMenuItem.Text = $"MultiPV [{SerializedInfo.Instance.MultiPV}]";
            animTimeToolStripMenuItem.Text = $"Anim Time [{_animTime}]";
            clickDelayToolStripMenuItem.Text = $"Click Delay [{SerializedInfo.Instance.ClickDelay}]";

            if (SerializedInfo.Instance.IsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void soundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            soundToolStripMenuItem.Checked = !soundToolStripMenuItem.Checked;
        }

        private void engineTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Engine Time:", "BerldChess - Engine Time");
            int engineTime;

            if (int.TryParse(input, out engineTime))
            {
                _engineTime = engineTime;
                engineTimeToolStripMenuItem.Text = $"Enginetime [{_engineTime}]";
            }
        }

        private void multiPVToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void cheatModeToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void animTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Animation Time:", "BerldChess - Animation Time");
            int animTime;

            if (int.TryParse(input, out animTime))
            {
                _animTime = animTime;
                animTimeToolStripMenuItem.Text = $"Anim Time [{_animTime}]";
            }
        }

        private void checkAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkAutoToolStripMenuItem.Checked = !checkAutoToolStripMenuItem.Checked;
        }

        private void copyFENToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_vm.PositionHistory[_vm.NavIndex].FEN);
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
            Pen linePen = new Pen(Color.LightGray, 1);
            Pen middleLinePen = new Pen(Color.Gray, 1);
            Pen chartLinePen = new Pen(Color.Black, 1);

            int chartYMiddle = Round(_panelEvalChart.Height / 2.0);

            double peak;
            double highestValue = _vm.PositionHistory.Max(c => c.Evaluation);
            double lowestValue = _vm.PositionHistory.Min(c => c.Evaluation);

            if (highestValue > Math.Abs(lowestValue))
            {
                peak = highestValue;
            }
            else
            {
                peak = Math.Abs(lowestValue);
            }

            if (peak < 3)
            {
                peak = 3;
            }
            else if (peak > 10)
            {
                peak = 10;
            }

            double yUnitLength = _panelEvalChart.Height / peak / 2.0;
            double xUnitLength = (double)_panelEvalChart.Width / (_vm.PositionHistory.Count - 1);

            int xPart = Round(_panelEvalChart.Width / 10.0);
            int yPart = Round(_panelEvalChart.Height / 10.0);

            for (int i = 1; i < 10; i++)
            {
                g.DrawLine(linePen, xPart * i, 0, xPart * i, _panelEvalChart.Height);
                g.DrawLine(linePen, 0, yPart * i, _panelEvalChart.Width, yPart * i);
            }

            for (int i = 0; i <= 10; i++)
            {
                if (i % 2 == 1)
                {
                    continue;
                }

                int valueX = ((int)((_vm.PositionHistory.Count - 1) / 10.0 * i));

                if (valueX > 2 || i == 10)
                {
                    string output;

                    if (i == 10)
                    {
                        output = "Ply " + valueX;
                    }
                    else
                    {
                        output = valueX.ToString();
                    }

                    g.DrawString(output, _panelEvalChart.Font, Brushes.Black, _panelEvalChart.Width / 10 * i - g.MeasureString(output, _panelEvalChart.Font).Width, _panelEvalChart.Height - _panelEvalChart.Font.Size * 2);
                }
            }

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 1)
                {
                    continue;
                }

                int offSet = 0;

                double valueY = Math.Round((peak - ((peak / 5.0 * i))), 1);
                string displayed = valueY.ToString();

                offSet = Round(g.MeasureString(displayed, _panelEvalChart.Font).Height / 10.0 * i);
                g.DrawString(displayed, _panelEvalChart.Font, Brushes.Black, _panelEvalChart.Font.Size / 2, (_panelEvalChart.Height / 10 * i) - offSet);
            }

            g.DrawLine(middleLinePen, 0, chartYMiddle, _panelEvalChart.Width, chartYMiddle);

            double upperBound = 9.9;
            double lowerBound = -9.7;

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

        private void clickDelayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Click Delay:", "BerldChess -Click Delay");
            int clickDelay;

            if (int.TryParse(input, out clickDelay))
            {
                SerializedInfo.Instance.ClickDelay = clickDelay;
                clickDelayToolStripMenuItem.Text = $"Click Delay [{clickDelay}]";
            }
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
                    _chessPanel.IsUnicodeFont = SerializedInfo.Instance.IsUnicodeFont;
                    _chessPanel.PieceFontFamily = SerializedInfo.Instance.PieceFontFamily;
                    _chessPanel.ChessFontChars = SerializedInfo.Instance.ChessFontChars;
                    hideOutputToolStripMenuItem.Checked = SerializedInfo.Instance.HideOutput;
                    hideArrowsToolStripMenuItem.Checked = SerializedInfo.Instance.HideArrows;
                    gridBorderToolStripMenuItem.Checked = SerializedInfo.Instance.DisplayGridBorder;
                    flipBoardToolStripMenuItem.Checked = SerializedInfo.Instance.BoardFlipped;
                    localModeToolStripMenuItem.Checked = SerializedInfo.Instance.LocalMode;
                    cheatModeToolStripMenuItem.Checked = SerializedInfo.Instance.CheatMode;
                    checkAutoToolStripMenuItem.Checked = SerializedInfo.Instance.AutoCheck;
                    _engineTime = SerializedInfo.Instance.EngineTime;
                    soundToolStripMenuItem.Checked = SerializedInfo.Instance.Sound;
                    _animTime = SerializedInfo.Instance.AnimationTime;
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
                SerializedInfo.Instance.AnimationTime = _animTime;

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
            _dataGridView.Rows.Clear();

            for (int i = 0; i < newRowCount - 1; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                for (int iCell = 0; iCell < _dataGridView.Columns.Count; iCell++)
                {
                    DataGridViewTextBoxCell textCell = new DataGridViewTextBoxCell();
                    textCell.Value = string.Empty;

                    row.Cells.Add(textCell);
                }

                _dataGridView.Rows.Add(row);
            }
        }

        private void ResetDataGridColumn(InfoType[] columns)
        {
            _dataGridView.Rows.Clear();
            _dataGridView.Columns.Clear();

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
                _dataGridView.Columns.Add(column);
            }
        }

        private string GetFormatMove(Move move, MoveType moveType)
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

            if (pieceCharacter != 'P')
            {
                formatMove += pieceCharacter;
            }

            formatMove += move.OriginalPosition.ToString();

            if (moveType == MoveType.Move)
            {
                formatMove += '-';
            }
            else if (moveType == (MoveType.Move | MoveType.Capture))
            {
                formatMove += 'x';
            }

            formatMove += move.NewPosition.ToString();

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
            for (int rowI = 0; rowI < _dataGridView.Rows.Count; rowI++)
            {
                for (int cellI = 0; cellI < _dataGridView.Rows[rowI].Cells.Count; cellI++)
                {
                    _dataGridView.Rows[rowI].Cells[cellI].Value = "";
                }
            }
        }

        private void AutoMove()
        {
            if (_animTestImg == null)
            {
                _animTestImg = Recognizer.GetBoardSnap();
                return;
            }

            Bitmap _currImg = Recognizer.GetBoardSnap();
            bool areSame = AreSame(_currImg, _animTestImg);

            if (_updateAfterAnim && areSame)
            {
                _updateAfterAnim = false;
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

                _moveTry = true;

                FigureMovedEventArgs args = new FigureMovedEventArgs(source, destination);
                OnPieceMoved(null, args);
            }

            _animTestImg = _currImg;
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

        #endregion
    }
}