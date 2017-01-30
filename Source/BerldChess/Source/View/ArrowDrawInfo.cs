using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BerldChess.View
{
    public class ArrowDrawInfo
    {
        public Arrow Arrow { get; set; }
        public Point[] Positions { get; set; }

        public int Length
        {
            get
            {
                if (Positions.Length > 1)
                {
                    return GetDistance(Math.Abs(Positions[0].X - Positions[1].X), Math.Abs(Positions[0].Y - Positions[1].Y));
                }

                return -1;
            }

        }

        private int GetDistance(int a, int b)
        {
            return (int)Math.Sqrt(a * a + b * b);
        }
    }
}
