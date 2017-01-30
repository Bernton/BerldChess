using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BerldChess.Model
{
    public static class Recognizer
    {

        public static Bitmap[] Pieces { get; private set; } = new Bitmap[12];
        public static int ScreenIndex { get; private set; }
        private static Point? _boardLocation = null;
        private static Size? _boardSize = null;

        private static Color _lightColor;
        private static Color _darkColor;

        private static Bitmap _lastBoardSnap;

        public static double FieldSize { get; private set; } = -1;

        public static bool BoardFound { get; private set; } = false;

        public static Point BoardLocation
        {
            get
            {
                if (_boardLocation != null)
                {
                    return (Point)_boardLocation;
                }
                else
                {
                    throw new InvalidOperationException("No board found yet.");
                }
            }
        }

        public static Size BoardSize
        {
            get
            {
                if (_boardSize != null)
                {
                    return (Size)_boardSize;
                }
                else
                {
                    throw new InvalidOperationException("No board found yet.");
                }
            }
        }

        public static Bitmap GetBoardSnap()
        {
            if (BoardFound)
            {
                Bitmap screenShot = GetScreenshot(Screen.AllScreens[ScreenIndex]);
                Rectangle cloneRectangle = new Rectangle(BoardLocation, BoardSize);
                return screenShot.Clone(cloneRectangle, screenShot.PixelFormat);
            }

            return null;
        }

        private static Bitmap GetSectionClone(Bitmap source, Rectangle section)
        {
            return source.Clone(section, source.PixelFormat);
        }

        public static void UpdateBoardImage()
        {
            _lastBoardSnap = GetBoardSnap();
        }

        public static bool SearchBoard(Color lightSquareColor, Color darkSquareColor)
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (SearchBoard(GetScreenshot(Screen.AllScreens[i]), lightSquareColor, darkSquareColor))
                {
                    _lightColor = lightSquareColor;
                    _darkColor = darkSquareColor;

                    ScreenIndex = i;
                    BoardFound = true;
                    _lastBoardSnap = GetBoardSnap();
                    Pieces = GetPieceImages(_lastBoardSnap);
                    return true;
                }
            }

            return false;
        }

        public static Point[] GetChangedSquares()
        {
            List<Point> changedSquares = new List<Point>();
            Bitmap boardSnap = GetBoardSnap();

            double fieldWidth = boardSnap.Width / 8.0;
            double fieldHeight = boardSnap.Height / 8.0;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Color borderColor = boardSnap.GetPixel(Round((x * fieldWidth + 4)), Round((y * fieldHeight + 4)));
                    Color centerColor = boardSnap.GetPixel(Round((x * fieldWidth + (fieldWidth / 2.0))), Round((y * fieldHeight + fieldHeight * 0.73)));
                    bool same = borderColor.ToArgb() == centerColor.ToArgb();

                    Color lastBorderColor = _lastBoardSnap.GetPixel(Round((x * fieldWidth + 4)), Round((y * fieldHeight + 4)));
                    Color lastCenterColor = _lastBoardSnap.GetPixel(Round((x * fieldWidth + (fieldWidth / 2.0))), Round((y * fieldHeight + fieldHeight * 0.73)));
                    bool lastSame = lastBorderColor.ToArgb() == lastCenterColor.ToArgb();

                    if (same != lastSame)
                    {
                        changedSquares.Add(new Point(x, y));
                    }
                    else if (same == false && centerColor.ToArgb() != lastCenterColor.ToArgb())
                    {
                        changedSquares.Add(new Point(x, y));
                    }
                }
            }

            return changedSquares.ToArray();
        }

        private static Bitmap[] GetPieceImages(Bitmap board)
        {
            Bitmap[] pieces = new Bitmap[12];

            double fieldWidth = board.Width / 8.0;
            double fieldHeight = board.Height / 8.0;

            pieces[0] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 4), Round(fieldHeight * 7), (int)fieldWidth, (int)fieldHeight));
            pieces[1] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 3), Round(fieldHeight * 7), (int)fieldWidth, (int)fieldHeight));
            pieces[2] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 0), Round(fieldHeight * 7), (int)fieldWidth, (int)fieldHeight));
            pieces[3] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 2), Round(fieldHeight * 7), (int)fieldWidth, (int)fieldHeight));
            pieces[4] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 1), Round(fieldHeight * 7), (int)fieldWidth, (int)fieldHeight));
            pieces[5] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 0), Round(fieldHeight * 6), (int)fieldWidth, (int)fieldHeight));
            pieces[6] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 4), Round(fieldHeight * 0), (int)fieldWidth, (int)fieldHeight));
            pieces[7] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 3), Round(fieldHeight * 0), (int)fieldWidth, (int)fieldHeight));
            pieces[8] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 0), Round(fieldHeight * 0), (int)fieldWidth, (int)fieldHeight));
            pieces[9] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 2), Round(fieldHeight * 0), (int)fieldWidth, (int)fieldHeight));
            pieces[10] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 1), Round(fieldHeight * 0), (int)fieldWidth, (int)fieldHeight));
            pieces[11] = GetSectionClone(board, new Rectangle(Round(fieldWidth * 0), Round(fieldHeight * 1), (int)fieldWidth, (int)fieldHeight));

            return pieces;
        }

        private static int Round(double number)
        {
            return (int)Math.Round(number, 0);
        }

        private static bool SearchBoard(Bitmap image, Color lightSquareColor, Color darkSquareColor)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            bool matchFound = false;
            bool widthFound = false;
            bool heightFound = false;
            int boardX = -1;
            int boardY = -1;
            int boardWidth = 0;
            int boardHeight = 0;
            int matchTolerance = 0;
            int initSuccTol = -1;
            int successivelyTolerance = -1;
            int imageStride = imageData.Stride;
            int imageWidth = image.Width * 3;
            int imageHeight = image.Height;

            Rectangle location = Rectangle.Empty;

            unsafe
            {
                byte* imagePointer = (byte*)(void*)imageData.Scan0;
                byte* startPointer = null;

                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixelColor = Color.FromArgb(imagePointer[2], imagePointer[1], imagePointer[0]);

                        if (IsSameColor(pixelColor, lightSquareColor, matchTolerance) || IsSameColor(pixelColor, darkSquareColor, matchTolerance))
                        {
                            if (successivelyTolerance > 0)
                            {
                                successivelyTolerance = initSuccTol;
                            }

                            if (!matchFound)
                            {
                                boardX = x;
                                boardY = y;
                                startPointer = imagePointer;
                                matchFound = true;
                            }
                            else if (!widthFound)
                            {
                                boardWidth++;
                            }
                            else
                            {
                                boardHeight++;
                            }
                        }
                        else
                        {
                            if (successivelyTolerance == -1 && matchFound)
                            {
                                initSuccTol = (int)Math.Ceiling(boardWidth / 40.0);
                                successivelyTolerance = initSuccTol;
                            }

                            if (successivelyTolerance > 0)
                            {
                                successivelyTolerance--;

                                if (widthFound)
                                {
                                    boardHeight++;
                                }
                                else
                                {
                                    boardWidth++;
                                }
                            }


                            if (widthFound && successivelyTolerance == 0)
                            {
                                heightFound = true;
                                boardHeight -= initSuccTol;
                                break;
                            }
                            else if (matchFound && successivelyTolerance == 0)
                            {
                                widthFound = true;
                                boardWidth -= initSuccTol;
                                successivelyTolerance = -1;

                                imagePointer = startPointer;
                                imagePointer -= imageWidth;
                                x = boardX;
                                y = boardY;
                                break;
                            }
                        }

                        if (!widthFound)
                        {
                            imagePointer += 3;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (widthFound)
                    {
                        imagePointer += imageWidth;
                    }

                    if (heightFound)
                    {
                        break;
                    }
                }
            }

            image.UnlockBits(imageData);

            if (heightFound)
            {
                _boardLocation = new Point(boardX, boardY);
                _boardSize = new Size(boardWidth, boardHeight);
            }

            return heightFound;
        }

        private static bool IsSameColor(Color color1, Color color2, int tolerance)
        {
            return tolerance >= Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }

        private static Bitmap GetScreenshot(Screen screen)
        {
            Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(screenshot);
            g.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return screenshot;
        }

        private static Rectangle SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {
            BitmapData smallData =
              smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bigData =
              bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int smallStride = smallData.Stride;
            int bigStride = bigData.Stride;

            int bigWidth = bigBmp.Width;
            int bigHeight = bigBmp.Height - smallBmp.Height + 1;
            int smallWidth = smallBmp.Width * 3;
            int smallHeight = smallBmp.Height;

            Rectangle location = Rectangle.Empty;
            int margin = Convert.ToInt32(255.0 * tolerance);

            unsafe
            {
                byte* pSmall = (byte*)(void*)smallData.Scan0;
                byte* pBig = (byte*)(void*)bigData.Scan0;

                int smallOffset = smallStride - smallBmp.Width * 3;
                int bigOffset = bigStride - bigBmp.Width * 3;

                bool matchFound = true;

                for (int y = 0; y < bigHeight; y++)
                {
                    for (int x = 0; x < bigWidth; x++)
                    {
                        byte* pBigBackup = pBig;
                        byte* pSmallBackup = pSmall;

                        //Look for the small picture.
                        for (int i = 0; i < smallHeight; i++)
                        {
                            int j = 0;
                            matchFound = true;
                            for (j = 0; j < smallWidth; j++)
                            {
                                //With tolerance: pSmall value should be between margins.
                                int inf = pBig[0] - margin;
                                int sup = pBig[0] + margin;
                                if (sup < pSmall[0] || inf > pSmall[0])
                                {
                                    matchFound = false;
                                    break;
                                }

                                pBig++;
                                pSmall++;
                            }

                            if (!matchFound) break;

                            //We restore the pointers.
                            pSmall = pSmallBackup;
                            pBig = pBigBackup;

                            //Next rows of the small and big pictures.
                            pSmall += smallStride * (1 + i);
                            pBig += bigStride * (1 + i);
                        }

                        //If match found, we return.
                        if (matchFound)
                        {
                            location.X = x;
                            location.Y = y;
                            location.Width = smallBmp.Width;
                            location.Height = smallBmp.Height;
                            break;
                        }
                        //If no match found, we restore the pointers and continue.
                        else
                        {
                            pBig = pBigBackup;
                            pSmall = pSmallBackup;
                            pBig += 3;
                        }
                    }

                    if (matchFound) break;

                    pBig += bigOffset;
                }
            }

            bigBmp.UnlockBits(bigData);
            smallBmp.UnlockBits(smallData);

            return location;
        }
    }
}
