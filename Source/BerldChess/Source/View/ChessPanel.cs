﻿using BerldChess.Properties;
using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public class ChessPanel : Panel
    {
        #region Fields

        private bool _wasResized = true;
        private int _pieceDimension;
        private int _boardDimension;
        private double _fieldSize;
        private Point _boardLocation;
        private Point _movingPieceIndex = new Point(-1, -1);
        private Point _movingPoint = new Point(-1, -1);
        private Point _movingOffset = new Point(-1, -1);
        private List<Arrow> _arrows = new List<Arrow>();
        private ChessPiece[][] _board;
        private Bitmap[] _scaledPieceImages = new Bitmap[12];

        #endregion

        #region Properties

        public bool IsFlipped { get; set; } = false;
        public bool DisplayGridBorders { get; set; } = false;
        public ChessGame Game { get; set; } = null;
        public List<Point> HighlighedSquares { get; set; } = new List<Point>();

        public List<Arrow> Arrows
        {
            get
            {
                return _arrows;
            }

            set
            {
                _arrows = value;
            }
        }

        #endregion

        #region Events

        public event FigureMovedEventHandler PieceMoved;

        #endregion

        #region Constructors

        public ChessPanel()
        {
            DoubleBuffered = true;
        }

        #endregion

        #region Event Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Game == null)
            {
                return;
            }

            _board = Game.GetBoard();



            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Width > Height)
            {
                _boardLocation = new Point(Round((Width - Height) / 2.0), 0);
                _boardDimension = Height - 1;
            }
            else
            {
                _boardLocation = new Point(0, Round((Height - Width) / 2.0));
                _boardDimension = Width - 1;
            }

            _fieldSize = _boardDimension / (double)_board.Length;
            _pieceDimension = Round(_fieldSize * 1.05);

            if (_wasResized)
            {
                for (int i = 0; i < _scaledPieceImages.Length; i++)
                {
                    _scaledPieceImages[i] = ResizeImage(PieceImageProvider.PieceImages[i], _pieceDimension, _pieceDimension);
                }

                _wasResized = false;
            }

            SolidBrush figureBrush = null;

            int absX;
            int absY;

            int[] xLinePositions = new int[_board.Length + 1];
            int[] yLinePositions = new int[_board.Length + 1];

            int iPosition;
            for (int i = 0; i < _board.Length + 1; i++)
            {
                iPosition = Round(_fieldSize * i);
                xLinePositions[i] = iPosition + _boardLocation.X;
                yLinePositions[i] = iPosition + _boardLocation.Y;
            }

            Color evenSquare;
            Color oddSquare;

            if (!IsFlipped)
            {
                evenSquare = Color.FromArgb(220, 220, 220);
                oddSquare = Color.FromArgb(171, 171, 171);
            }
            else
            {
                evenSquare = Color.FromArgb(171, 171, 171);
                oddSquare = Color.FromArgb(220, 220, 220);
            }

            for (int y = 0; y < Game.BoardHeight; y++)
            {
                for (int x = 0; x < Game.BoardHeight; x++)
                {
                    bool isHighlighted = false;

                    for (int i = 0; i < HighlighedSquares.Count; i++)
                    {
                        if (HighlighedSquares[i].X == x && HighlighedSquares[i].Y == y && !IsFlipped ||
                            (HighlighedSquares[i].X == Invert(Game.BoardHeight - 1, x) && (HighlighedSquares[i].Y == Invert(Game.BoardHeight - 1, y) && IsFlipped)))
                        {
                            g.FillRectangle(new SolidBrush(Color.Yellow), xLinePositions[x], yLinePositions[y], xLinePositions[x + 1] - xLinePositions[x] + 1, xLinePositions[y + 1] - xLinePositions[y] + 1);
                            g.DrawRectangle(Pens.Black, xLinePositions[x], yLinePositions[y], xLinePositions[x + 1] - xLinePositions[x], xLinePositions[y + 1] - xLinePositions[y]);

                            isHighlighted = true;
                            continue;
                        }
                    }

                    if (isHighlighted)
                    {
                        continue;
                    }

                    if (y % 2 == 1)
                    {
                        if ((x + 1) % 2 == 0)
                        {
                            g.FillRectangle(new SolidBrush(evenSquare), xLinePositions[x], yLinePositions[y], xLinePositions[x + 1] - xLinePositions[x] + 1, xLinePositions[y + 1] - xLinePositions[y] + 1);
                            continue;
                        }
                    }
                    else
                    {
                        if (x % 2 == 0)
                        {
                            g.FillRectangle(new SolidBrush(evenSquare), xLinePositions[x], yLinePositions[y], xLinePositions[x + 1] - xLinePositions[x] + 1, xLinePositions[y + 1] - xLinePositions[y] + 1);
                            continue;
                        }
                    }

                    g.FillRectangle(new SolidBrush(oddSquare), xLinePositions[x], yLinePositions[y], xLinePositions[x + 1] - xLinePositions[x] + 1, xLinePositions[y + 1] - xLinePositions[y] + 1);
                }
            }

            if (DisplayGridBorders)
            {
                for (int i = 0; i < _board.Length + 1; i++)
                {
                    g.DrawLine(Pens.Black, _boardLocation.X, yLinePositions[i], _boardDimension + _boardLocation.X, yLinePositions[i]);
                    g.DrawLine(Pens.Black, xLinePositions[i], _boardLocation.Y, xLinePositions[i], _boardDimension + _boardLocation.Y);
                }
            }

            for (int y = 0; y < _board.Length; y++)
            {
                for (int x = 0; x < _board.Length; x++)
                {
                    if (_board[y][x] == null || (y == _movingPieceIndex.Y && x == _movingPieceIndex.X))
                    {
                        continue;
                    }

                    if (_board[y][x].Owner == ChessPlayer.White)
                    {
                        figureBrush = new SolidBrush(Color.White);
                    }
                    else
                    {
                        figureBrush = new SolidBrush(Color.Black);
                    }

                    if (!IsFlipped)
                    {
                        absX = Round(x * _fieldSize + _boardLocation.X + (_fieldSize - _pieceDimension) / 2.0);
                        absY = Round(y * _fieldSize + _boardLocation.Y + (_fieldSize - _pieceDimension) / 2.0);
                    }
                    else
                    {
                        absX = Round(Invert(Game.BoardHeight - 1, x) * _fieldSize + _boardLocation.X + (_fieldSize - _pieceDimension) / 2.0);
                        absY = Round(Invert(Game.BoardHeight - 1, y) * _fieldSize + _boardLocation.Y + (_fieldSize - _pieceDimension) / 2.0);
                    }

                    g.DrawImageUnscaled(_scaledPieceImages[GetPieceIndexFromFenChar(_board[y][x].GetFENLetter())], absX, absY);
                }
            }

            if (_arrows.Count > 0)
            {
                bool moving = _movingPieceIndex.X != -1 && _board[_movingPieceIndex.Y][_movingPieceIndex.X] != null;

                for (int i = _arrows.Count - 1; i >= 0; i--)
                {
                    Pen arrowPen = new Pen(Color.Black, Round(_arrows[i].ThicknessPercent / 100.0 * _boardDimension));
                    arrowPen.Brush = new SolidBrush(_arrows[i].Color);
                    arrowPen.EndCap = LineCap.ArrowAnchor;
                    arrowPen.StartCap = LineCap.RoundAnchor;
                    Point[] absPositions = GetAbsPositionsFromMoveString(_arrows[i].Move);

                    if (moving)
                    {
                        Point relArrowStartPos = GetRelPositionsFromMoveString(_arrows[i].Move)[0];

                        if (relArrowStartPos.X != _movingPieceIndex.X || relArrowStartPos.Y != _movingPieceIndex.Y)
                        {
                            continue;
                        }
                    }

                    g.DrawLine(arrowPen, absPositions[0], absPositions[1]);
                }
            }

            if (_movingPieceIndex.X != -1 && _board[_movingPieceIndex.Y][_movingPieceIndex.X] != null)
            {
                if (_board[_movingPieceIndex.Y][_movingPieceIndex.X].Owner == ChessPlayer.White)
                {
                    figureBrush = new SolidBrush(Color.White);
                }
                else
                {
                    figureBrush = new SolidBrush(Color.Black);
                }

                BoardPosition startPosition = new BoardPosition((ChessFile)_movingPieceIndex.X, Invert(Game.BoardHeight, _movingPieceIndex.Y));

                ReadOnlyCollection<Move> validMoves = Game.GetValidMoves(startPosition);

                for (int i = 0; i < validMoves.Count; i++)
                {
                    Move current = validMoves[i];

                    if (!IsFlipped)
                    {
                        g.FillEllipse(Brushes.LightCoral, Round(((int)current.NewPosition.File) * _fieldSize + _fieldSize * 0.4) + _boardLocation.X, Round(Invert(Game.BoardHeight, current.NewPosition.Rank) * _fieldSize + _fieldSize * 0.4) + _boardLocation.Y, Round(_fieldSize * 0.2), Round(_fieldSize * 0.2));

                    }
                    else
                    {
                        g.FillEllipse(Brushes.LightCoral, Round((Invert(Game.BoardHeight - 1, (int)current.NewPosition.File)) * _fieldSize + _fieldSize * 0.4) + _boardLocation.X, Round((current.NewPosition.Rank - 1) * _fieldSize + _fieldSize * 0.4) + _boardLocation.Y, Round(_fieldSize * 0.2), Round(_fieldSize * 0.2));
                    }
                }

                absX = _movingPoint.X - Round(_fieldSize / 2.0);
                absY = _movingPoint.Y - Round(_fieldSize / 2.0);

                g.DrawImageUnscaled(_scaledPieceImages[GetPieceIndexFromFenChar(_board[_movingPieceIndex.Y][_movingPieceIndex.X].GetFENLetter())], absX, absY);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int absoluteBoardX = e.X - _boardLocation.X;
            int absoluteBoardY = e.Y - _boardLocation.Y;

            int relativeBoardX = absoluteBoardX / (int)_fieldSize;
            int relativeBoardY = absoluteBoardY / (int)_fieldSize;

            Point pieceIndex;

            if(!IsFlipped)
            {
                pieceIndex = new Point(relativeBoardX, relativeBoardY);
            }
            else
            {
                pieceIndex = new Point(Invert(Game.BoardHeight - 1, relativeBoardX), Invert(Game.BoardHeight - 1, relativeBoardY));
            }

            if (relativeBoardX < 0 || relativeBoardX > 7 || relativeBoardY < 0 || relativeBoardY > 7 || _board[pieceIndex.Y][pieceIndex.X] == null)
            {
                return;
            }

            _movingPoint = new Point(Round(relativeBoardX * _fieldSize + _boardLocation.X + _fieldSize / 2.0 + (_fieldSize - _pieceDimension) / 2.0), Round(relativeBoardY * _fieldSize + _boardLocation.Y + _fieldSize / 2.0 + (_fieldSize - _pieceDimension) / 2.0));
            _movingOffset = new Point(_movingPoint.X - e.X, _movingPoint.Y - e.Y);
            _movingPieceIndex = pieceIndex;

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_movingPieceIndex.X != -1)
            {
                int absoluteBoardX = e.X - _boardLocation.X + _movingOffset.X;
                int absoluteBoardY = e.Y - _boardLocation.Y + _movingOffset.Y;

                int relativeBoardX;
                int relativeBoardY;

                if(!IsFlipped)
                {
                    relativeBoardX = absoluteBoardX / (int)_fieldSize;
                    relativeBoardY = absoluteBoardY / (int)_fieldSize;
                }
                else
                {
                    relativeBoardX = Invert(Game.BoardHeight - 1, absoluteBoardX / (int)_fieldSize);
                    relativeBoardY = Invert(Game.BoardHeight - 1, absoluteBoardY / (int)_fieldSize);
                }

                if (relativeBoardX < 0 || relativeBoardX > 7 || relativeBoardY < 0 || relativeBoardY > 7)
                {
                    _movingPieceIndex = new Point(-1, -1);
                    Invalidate();
                    return;
                }

                PieceMoved?.Invoke(this, new FigureMovedEventArgs(_movingPieceIndex, new Point(relativeBoardX, relativeBoardY)));

                _movingPieceIndex = new Point(-1, -1);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_movingPieceIndex.X != -1)
            {
                _movingPoint = new Point(e.X + _movingOffset.X, e.Y + _movingOffset.Y);
                Invalidate();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _wasResized = true;
            Invalidate();
        }

        #endregion

        #region Other Methods

        public Point[] GetRelPositionsFromMoveString(string move)
        {
            Point[] index = new Point[2];
            index[0] = new Point(move[0] - 97, Invert(Game.BoardHeight, int.Parse(move[1].ToString())));
            index[1] = new Point(move[2] - 97, Invert(Game.BoardHeight, int.Parse(move[3].ToString())));

            return index;
        }

        private Point[] GetAbsPositionsFromMoveString(string move)
        {
            Point[] index = new Point[2];

            double offSet = 0.49;

            if (!IsFlipped)
            {
                index[0] = new Point(Round(((move[0] - 97) + offSet) * _fieldSize) + _boardLocation.X, Round((Invert(Game.BoardHeight, int.Parse(move[1].ToString())) + offSet) * _fieldSize) + _boardLocation.Y);
                index[1] = new Point(Round(((move[2] - 97) + offSet) * _fieldSize) + _boardLocation.X, Round((Invert(Game.BoardHeight, int.Parse(move[3].ToString())) + offSet) * _fieldSize) + _boardLocation.Y);
            }
            else
            {
                index[0] = new Point(Round((Invert(Game.BoardHeight - 1, move[0] - 97) + offSet) * _fieldSize) + _boardLocation.X, Round((int.Parse(move[1].ToString()) - 1 + offSet) * _fieldSize) + _boardLocation.Y);
                index[1] = new Point(Round((Invert(Game.BoardHeight - 1, move[2] - 97) + offSet) * _fieldSize) + _boardLocation.X, Round((int.Parse(move[3].ToString()) - 1 + offSet) * _fieldSize) + _boardLocation.Y);
            }

            return index;
        }

        private int Round(double number)
        {
            return (int)Math.Round(number, 0);
        }

        private int GetPieceIndexFromFenChar(char fenCharacter)
        {
            switch (fenCharacter)
            {
                case 'K':
                    return 0;
                case 'k':
                    return 6;
                case 'Q':
                    return 1;
                case 'q':
                    return 7;
                case 'R':
                    return 4;
                case 'r':
                    return 10;
                case 'B':
                    return 2;
                case 'b':
                    return 8;
                case 'N':
                    return 3;
                case 'n':
                    return 9;
                case 'P':
                    return 5;
                case 'p':
                    return 11;
            }

            return -1;
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        private int Invert(int max, int value)
        {
            return Math.Abs(value - max);
        }

        #endregion
    }
}
