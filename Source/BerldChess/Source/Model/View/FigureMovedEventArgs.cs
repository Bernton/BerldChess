using System;
using System.Drawing;

namespace BerldChess.View
{
    public class FigureMovedEventArgs : EventArgs
    {
        public Point Position { get; }
        public Point NewPosition { get; }

        public FigureMovedEventArgs(Point position, Point newPosition) : base()
        {
            Position = position;
            NewPosition = newPosition;
        }
    }
}