using BerldChess.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BerldChess.View
{
    public class PieceImageProvider
    {
        public static Bitmap[] PieceImages { get; set; }

        public const int ColorCount = 2;
        public const int PieceCount = 6;

        static PieceImageProvider()
        {
            PieceImages = new Bitmap[12];

            double pieceImageWidth = Resources.ChessPiecesSprite.Width / (double)PieceCount;
            double pieceImageHeight = Resources.ChessPiecesSprite.Height / (double)ColorCount;

            int roundedWidth = Round(pieceImageWidth);
            int roundedHeight = Round(pieceImageHeight);

            int x;
            int y;


            for (int colorI = 0; colorI < 2; colorI++)
            {
                for (int pieceI = 0; pieceI < 6; pieceI++)
                {
                    x = Round(pieceI * pieceImageWidth);
                    y = Round(colorI * pieceImageHeight);

                    PieceImages[colorI * 6 + pieceI] = CropImage(Resources.ChessPiecesSprite, new Rectangle(x, y, roundedWidth, roundedHeight));
                }
            }
        }

        public static Bitmap GetFromFEN(char FENCharacter)
        {
            switch (FENCharacter)
            {
                case 'K':
                    return PieceImages[0];
                case 'Q':
                    return PieceImages[1];
                case 'B':
                    return PieceImages[2];
                case 'N':
                    return PieceImages[3];
                case 'R':
                    return PieceImages[4];
                case 'P':
                    return PieceImages[5];
                case 'k':
                    return PieceImages[6];
                case 'q':
                    return PieceImages[7];
                case 'b':
                    return PieceImages[8];
                case 'n':
                    return PieceImages[9];
                case 'r':
                    return PieceImages[10];
                case 'p':
                    return PieceImages[11];
            }

            throw new ArgumentException("Invalid char.");
        }

        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            Bitmap bitmap = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            return bitmap;
        }

        private static int Round(double value)
        {
            return (int)Math.Round(value, 0);
        }
    }
}
