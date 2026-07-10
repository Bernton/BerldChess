using System;
using System.Drawing;

namespace BerldChess.View
{
    public class MoveArrow
    {
        public bool HasStartCircle { get; set; }
        public bool IsAbovePieces { get; set; }
        public double ThicknessPercent { get; set; }
        public string Move { get; set; }
        public Color Color { get; set; }

        public MoveArrow(string move, double thickness, Color color, bool abovePieces = true, bool hasStartCircle = true)
        {
            if (move == null || move.Length < 4)
            {
                throw new ArgumentException("Move must have a length of at least 4.", nameof(move));
            }

            // An arrow is defined by its from/to squares; ignore any promotion suffix (e.g. "a7a8q").
            Move = move.Substring(0, 4);
            ThicknessPercent = thickness;
            Color = color;
            IsAbovePieces = abovePieces;
            HasStartCircle = hasStartCircle;
        }
    }
}