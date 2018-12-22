using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BerldChess
{
    public static class Extensions
    {
        public static void SetDoubleBuffered(this Control control)
        {
            if (SystemInformation.TerminalServerSession)
            {
                return;
            }

            PropertyInfo doubleBufferedProperty = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            doubleBufferedProperty.SetValue(control, true, null);
        }

        public static void FitFont(this Control control, double widthFactor, double heightFactor)
        {
            int fontSize = 1;
            Size size = TextRenderer.MeasureText(control.Text, new Font(control.Font.FontFamily, fontSize));

            double factorWidth = control.Width * widthFactor;
            double factorHeight = control.Height * heightFactor;

            while (size.Width < factorWidth && size.Height < factorHeight && size.Width != 0 && size.Height != 0)
            {
                fontSize++;
                size = TextRenderer.MeasureText(control.Text, new Font(control.Font.FontFamily, fontSize));
            }

            control.Font = new Font(control.Font.FontFamily, fontSize);
        }

        public static char GetPieceChar(this ilf.pgn.Data.PieceType? type)
        {
            if (type == null)
            {
                return 'q';
            }

            switch (type)
            {
                case ilf.pgn.Data.PieceType.Queen:
                    return 'q';

                case ilf.pgn.Data.PieceType.Rook:
                    return 'r';

                case ilf.pgn.Data.PieceType.Bishop:
                    return 'b';

                case ilf.pgn.Data.PieceType.Knight:
                    return 'n';
            }

            return 'q';
        }

        public static char GetFenPieceChar(this ilf.pgn.Data.PieceType? type, bool isWhite)
        {
            switch (type)
            {
                case ilf.pgn.Data.PieceType.King:

                    if (isWhite)
                    {
                        return 'K';
                    }
                    else
                    {
                        return 'k';
                    }

                case ilf.pgn.Data.PieceType.Bishop:

                    if (isWhite)
                    {
                        return 'B';
                    }
                    else
                    {
                        return 'b';
                    }

                case ilf.pgn.Data.PieceType.Knight:

                    if (isWhite)
                    {
                        return 'N';
                    }
                    else
                    {
                        return 'n';
                    }

                case ilf.pgn.Data.PieceType.Pawn:

                    if (isWhite)
                    {
                        return 'P';
                    }
                    else
                    {
                        return 'p';
                    }

                case ilf.pgn.Data.PieceType.Queen:

                    if (isWhite)
                    {
                        return 'Q';
                    }
                    else
                    {
                        return 'q';
                    }

                case ilf.pgn.Data.PieceType.Rook:

                    if (isWhite)
                    {
                        return 'R';
                    }
                    else
                    {
                        return 'r';
                    }
            }

            throw new ArgumentException("PieceType invalid.");
        }

        public static int Cap(this int number, int cap)
        {
            if (number > cap)
            {
                return cap;
            }
            else
            {
                return number;
            }
        }

        public static int BotCap(this int number, int cap)
        {
            if (number < cap)
            {
                return cap;
            }
            else
            {
                return number;
            }
        }

        public static Color OperateAll(this Color color, int number, bool isAdd)
        {
            if (isAdd)
            {
                return Color.FromArgb((color.R + number).Cap(255), (color.G + number).Cap(255), (color.B + number).Cap(255));
            }
            else
            {
                return Color.FromArgb((color.R - number).BotCap(0), (color.G - number).BotCap(0), (color.B - number).BotCap(0));
            }
        }
    }
}
