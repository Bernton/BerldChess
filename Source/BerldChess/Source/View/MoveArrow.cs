using System;
using System.Drawing;

namespace BerldChess.View
{
    public class MoveArrow
    {
        public double ThicknessPercent { get; set; }
        public string Move { get; set; }
        public Color Color { get; set; }

        public MoveArrow(string move, double thickness, Color color)
        {
            if(move.Length != 4)
            {
                throw new ArgumentException("Move must have length of 4.");
            }

            Move = move;
            ThicknessPercent = thickness;
            Color = color;
        }
    }
}