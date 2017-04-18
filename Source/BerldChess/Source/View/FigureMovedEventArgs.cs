﻿using System;
using System.Drawing;

namespace BerldChess.View
{
    public class PieceMovedEventArgs : EventArgs
    {
        public Point Position { get; }
        public Point NewPosition { get; }

        public PieceMovedEventArgs(Point position, Point newPosition) : base()
        {
            Position = position;
            NewPosition = newPosition;
        }
    }
}