using BerldChess.Model;
using BerldChess.Properties;
using BerldChess.ViewModel;
using ChessDotNet;
using ChessDotNet.Pieces;
using ChessEngineInterface;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BerldChess.View
{
    public partial class FormMain : Form
    {
        #region Fields

        private int _multiPV1Reference;
        private ChessPanel _chessPanel;
        private FormMainViewModel _vm;
        private InfoType[] _columnOrder;

        #endregion

        #region Constructors

        public FormMain()
        {
            InitializeComponent();
            InitializeWindow();
            InitializeViewModel();
            InitializeChessBoard();

            _textBoxFen.Text = _vm.Game.GetFen();
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
            ResetDataGridRows(FormMainViewModel.MultiPV);

            LoadXMLConfiguration();
        }

        #endregion

        #region Initialization Methods

        private void InitializeWindow()
        {
            Icon = Resources.PawnRush;
            Text = string.Format("BerldChess Version {0}", Assembly.GetEntryAssembly().GetName().Version.ToString(3));
            _splitContainerInner.SplitterWidth = 3;
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
            _chessPanel.Name = "_chessPanel";
            _chessPanel.Size = new Size(787, 403);
            _splitContainerInner.Panel1.Controls.Add(_chessPanel);
            _chessPanel.Game = _vm.Game;
            _chessPanel.Select();
            _chessPanel.PieceMoved += OnPieceMoved;
        }

        #endregion

        #region Event Methods

        private void OnEngineStopped()
        {
            _chessPanel.Arrows.Clear();
            _chessPanel.Invalidate();

            _vm.Engine.Query($"position fen {_vm.Game.GetFen()}");

            if (_vm.Game.IsCheckmated(_vm.Game.WhoseTurn))
            {
                Invoke((MethodInvoker)delegate
                {
                    _labelCPStatus.Text = "Checkmate";
                    _labelCPStatus.ForeColor = Color.Black;
                });

                EmptyDataGrid();
            }
            else if (_vm.Game.IsStalemated(_vm.Game.WhoseTurn))
            {
                Invoke((MethodInvoker)delegate
                {
                    _labelCPStatus.Text = "Stalemate";
                    _labelCPStatus.ForeColor = Color.Black;
                });

                EmptyDataGrid();
            }
            else
            {
                _vm.Engine.Query("go infinite");

                Invoke((MethodInvoker)delegate
                {
                    _textBoxFen.Text = _vm.Game.GetFen();
                });
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
                        ResetDataGridRows(FormMainViewModel.MultiPV);
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
                            _dataGridView.Rows[multiPV - 1].Cells[columnIndex].Value = FormatEngineInfo(evaluation.Types[iEval], evaluation[iEval]);
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
                            centipawn += 12000 + Math.Abs(multiPV - FormMainViewModel.MultiPV) * 10;
                        }
                        else
                        {
                            centipawn -= 12000 - Math.Abs(multiPV - FormMainViewModel.MultiPV) * 10;
                        }
                    }

                    if (multiPV == 1)
                    {
                        if (isMate)
                        {
                            if (mateNumber > 0)
                            {
                                _labelCPStatus.Text = $"Mate in {mateNumber}";
                                _labelCPStatus.ForeColor = Color.Green;
                            }
                            else
                            {
                                _labelCPStatus.Text = $"Mated in {Math.Abs(mateNumber)}";
                                _labelCPStatus.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            if (centipawn > 100)
                            {
                                _labelCPStatus.ForeColor = Color.Green;
                            }
                            else if (centipawn < -100)
                            {
                                _labelCPStatus.ForeColor = Color.Red;
                            }
                            else
                            {
                                _labelCPStatus.ForeColor = Color.Black;
                            }

                            _labelCPStatus.Text = ((double)centipawn / 100).ToString("0.##");
                        }
                    }

                    if (_checkBoxHideArrows.Checked)
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
                            _chessPanel.Arrows.Add(new Arrow((evaluation[InfoType.PV]).Substring(0, 4), 1.6, GetReferenceColor(centipawn, _multiPV1Reference)));
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

        private void OnPieceMoved(object sender, FigureMovedEventArgs e)
        {
            BoardPosition startPosition = new BoardPosition((ChessFile)e.Position.X, Invert(e.Position.Y - 1, 7));
            BoardPosition endPosition = new BoardPosition((ChessFile)e.NewPosition.X, Invert(e.NewPosition.Y - 1, 7));
            ChessPiece movingPiece = _vm.Game.GetPieceAt(startPosition);

            char promotion = 'q';

            if (movingPiece is Pawn && startPosition.Rank == (movingPiece.Owner == ChessPlayer.White ? 7 : 2) && endPosition.Rank == (movingPiece.Owner == ChessPlayer.White ? 8 : 1))
            {
                PromotionDialog dialog = new PromotionDialog(movingPiece.Owner);
                dialog.ShowDialog();

                promotion = dialog.PromotionCharacter;
            }

            Move move = new Move(startPosition, endPosition, movingPiece.Owner, promotion);
            MoveType moveType;

            if (_vm.Game.IsValidMove(move))
            {

                moveType = _vm.Game.ApplyMove(move, false);

                _chessPanel.HighlighedSquares.Clear();
                _chessPanel.HighlighedSquares.Add(e.Position);
                _chessPanel.HighlighedSquares.Add(e.NewPosition);

                if (_vm.NavIndex != _vm.PositionHistory.Count - 1)
                {
                    _vm.PositionHistory.RemoveRange(_vm.NavIndex + 1, _vm.PositionHistory.Count - _vm.NavIndex - 1);
                }

                _vm.PositionHistory.Add(new ChessPosition(_vm.Game.GetFen(), move.ToString("")));

                if (_checkBoxLocalMode.Checked)
                {
                    _chessPanel.IsFlipped = !_chessPanel.IsFlipped;
                    _chessPanel.Invalidate();
                }

                _vm.NavIndex++;

                if(_vm.Game.IsCheckmated(ChessPlayer.Black) || _vm.Game.IsCheckmated(ChessPlayer.White))
                {
                    _labelCPStatus.Text = "Checkmate";
                }
                else if(_vm.Game.IsStalemated(ChessPlayer.Black) || _vm.Game.IsStalemated(ChessPlayer.White))
                {
                    _labelCPStatus.Text = "Stalemate";
                }

                _vm.Engine.RequestStop();
                _chessPanel.Invalidate();
            }
        }

        private void OnButtonLoadFenClick(object sender, EventArgs e)
        {
            try
            {
                _vm.Game = new ChessGame(_textBoxFen.Text);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return;
            }

            _chessPanel.Game = _vm.Game;
            _vm.PositionHistory.Clear();
           
            _vm.PositionHistory.Add(new ChessPosition(_textBoxFen.Text));
            _chessPanel.HighlighedSquares.Clear();
            _vm.NavIndex = 0;
            _chessPanel.Invalidate();
            OnEngineStopped();
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
            _chessPanel.DisplayGridBorders = ((CheckBox)sender).Checked;
            _chessPanel.Invalidate();
        }

        private void OnCheckBoxFlippedCheckedChanged(object sender, EventArgs e)
        {
            if (!_checkBoxLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _checkBoxFlipped.Checked;
                _chessPanel.Invalidate();
            }
        }

        private void OnCheckBoxHideArrowsCheckedChanged(object sender, EventArgs e)
        {
            _arrowInvalidateTimer.Enabled = !_checkBoxHideArrows.Checked;

            if (_checkBoxHideArrows.Checked)
            {
                _chessPanel.Arrows.Clear();
            }

            _labelCPStatus.Visible = !(_checkBoxHideArrows.Checked && _checkBoxHideOutput.Checked);
            _labelEvaluation.Visible = !(_checkBoxHideArrows.Checked && _checkBoxHideOutput.Checked);
            _chessPanel.Invalidate();
        }

        private void OnCheckBoxHideOutputCheckedChanged(object sender, EventArgs e)
        {
            _splitContainerOuter.Panel2Collapsed = _checkBoxHideOutput.Checked;
            _labelCPStatus.Visible = !_checkBoxHideOutput.Checked;

            _labelCPStatus.Visible = !(_checkBoxHideArrows.Checked && _checkBoxHideOutput.Checked);
            _labelEvaluation.Visible = !(_checkBoxHideArrows.Checked && _checkBoxHideOutput.Checked);
        }

        private void OnArrowInvalidateTimerTick(object sender, EventArgs e)
        {
            _chessPanel.Invalidate();
        }

        private void OnCheckBoxPlayModeCheckedChanged(object sender, EventArgs e)
        {
            _checkBoxFlipped.Enabled = !_checkBoxLocalMode.Checked;

            if (_checkBoxLocalMode.Checked)
            {
                _chessPanel.IsFlipped = _vm.NavIndex % 2 != 0;
            }
            else
            {
                _chessPanel.IsFlipped = _checkBoxFlipped.Checked;
            }

            _chessPanel.Invalidate();
        }

        private void OnButtonNewClick(object sender, EventArgs e)
        {
            _vm.Game = new ChessGame();

            _chessPanel.Game = _vm.Game;
            _vm.PositionHistory.Clear();
            _chessPanel.HighlighedSquares.Clear();
            _vm.PositionHistory.Add(new ChessPosition(_textBoxFen.Text));
            _vm.NavIndex = 0;
            _chessPanel.Invalidate();
            _vm.Engine.Query("ucinewgame");
            OnEngineStopped();
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

                    if (SerializedInfo.Instance.IsMaximized)
                    {
                        WindowState = FormWindowState.Maximized;
                    }

                    _checkBoxHideOutput.Checked = SerializedInfo.Instance.HideOutput;
                    _checkBoxHideArrows.Checked = SerializedInfo.Instance.HideArrows;
                    _arrowInvalidateTimer.Enabled = !SerializedInfo.Instance.HideArrows;
                    _checkBoxGridBorder.Checked = SerializedInfo.Instance.DisplayGridBorder;
                    _checkBoxFlipped.Checked = SerializedInfo.Instance.BoardFlipped;
                    _checkBoxLocalMode.Checked = SerializedInfo.Instance.LocalMode;

                    _chessPanel.Invalidate();
                }
            }
            catch (Exception ex)
            {
                File.Delete(FormMainViewModel.ConfigFileName);
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SaveXMLConfiguration()
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
            SerializedInfo.Instance.DisplayGridBorder = _checkBoxGridBorder.Checked;
            SerializedInfo.Instance.BoardFlipped = _checkBoxFlipped.Checked;
            SerializedInfo.Instance.HideArrows = _checkBoxHideArrows.Checked;
            SerializedInfo.Instance.HideOutput = _checkBoxHideOutput.Checked;
            SerializedInfo.Instance.LocalMode = _checkBoxLocalMode.Checked;

            XmlSerializer serializer = new XmlSerializer(typeof(SerializedInfo));
            FileStream fileStream = new FileStream(FormMainViewModel.ConfigFileName, FileMode.Create);
            serializer.Serialize(fileStream, SerializedInfo.Instance);
            fileStream.Dispose();
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

        private void NavigateGame(int navIndex)
        {
            if (navIndex != _vm.NavIndex)
            {
                bool wasFinished = IsFinishedPosition();

                _chessPanel.HighlighedSquares.Clear();
                _vm.Game = new ChessGame(_vm.PositionHistory[navIndex].FEN);
                _chessPanel.Game = _vm.Game;

                string previousMove = _vm.PositionHistory[navIndex].PreviousMove;

                if (previousMove != null)
                {
                    Point[] squareLocations = _chessPanel.GetRelPositionsFromMoveString(previousMove);

                    _chessPanel.HighlighedSquares.AddRange(squareLocations);
                }

                _textBoxFen.Text = _vm.PositionHistory[navIndex].FEN;
                _vm.NavIndex = navIndex;

                if (_checkBoxLocalMode.Checked)
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

        private bool IsFinishedPosition()
        {
            return _vm.Game.IsCheckmated(ChessPlayer.Black)
                || _vm.Game.IsCheckmated(ChessPlayer.White)
                || _vm.Game.IsStalemated(ChessPlayer.Black)
                || _vm.Game.IsStalemated(ChessPlayer.White);
        }

        #endregion
    }
}