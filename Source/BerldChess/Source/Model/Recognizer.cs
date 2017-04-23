using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BerldChess.Model
{
    public static class Recognizer
    {
        #region Fields

        private const int SideLength = 8;
        private static Color _darkSquare;
        private static Color _lightSquare;
        private static Bitmap _lastBoardSnap;

        #endregion

        #region Properties

        public static bool BoardFound { get; private set; } = false;
        public static int ScreenIndex { get; private set; } = -1;
        public static int MinimumSize { get; set; } = 32;
        public static Point BoardLocation { get; private set; }
        public static Size BoardSize { get; private set; }
        public static SizeF FieldSize { get; set; }
        public static PointF CenterPixelLocation { get; set; } = new PointF(0.5F, 0.73F);

        #endregion

        #region Methods

        public static void UpdateBoardImage()
        {
            if (BoardFound)
            {
                _lastBoardSnap = GetBoardSnap();
            }
        }

        public static void UpdateBoardImage(Bitmap image)
        {
            if (BoardFound)
            {
                Debug.WriteLine("# Update #");
                _lastBoardSnap = image;
            }
        }

        public static bool SearchBoard(Color lightSquareColor, Color darkSquareColor)
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (SearchBoard(GetScreenshot(Screen.AllScreens[i]), darkSquareColor, lightSquareColor))
                {
                    if (BoardSize.Width < MinimumSize || BoardSize.Height < MinimumSize)
                    {
                        return false;
                    }

                    BoardFound = true;
                    ScreenIndex = i;
                    _darkSquare = darkSquareColor;
                    _lightSquare = lightSquareColor;
                    _lastBoardSnap = GetBoardSnap();
                    return true;
                }
            }

            return false;
        }

        public static Point[] GetChangedSquares(Bitmap boardSnap)
        {
            if (BoardFound)
            {
                List<Point> changedSquares = new List<Point>();

                double fieldWidth = boardSnap.Width / 8.0;
                double fieldHeight = boardSnap.Height / 8.0;

                unsafe
                {
                    BitmapData _data = boardSnap.LockBits(new Rectangle(0, 0, boardSnap.Width, boardSnap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    BitmapData _lastData = _lastBoardSnap.LockBits(new Rectangle(0, 0, _lastBoardSnap.Width, _lastBoardSnap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    byte* dataScan0 = (byte*)_data.Scan0;
                    byte* lastDataScan0 = (byte*)_lastData.Scan0;

                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            int borderX = Round((x * fieldWidth + 4));
                            int borderY = Round((y * fieldHeight + 4));
                            int centerX = Round((x * fieldWidth + (fieldWidth / 2.0)));
                            int centerY = Round((y * fieldHeight + fieldHeight * 0.73));

                            int borderColor = GetPixel(dataScan0, _data.Stride, borderX, borderY);
                            int centerColor = GetPixel(dataScan0, _data.Stride, centerX, centerY);
                            bool same = borderColor == centerColor;

                            int lastBorderColor = GetPixel(lastDataScan0, _lastData.Stride, borderX, borderY);
                            int lastCenterColor = GetPixel(lastDataScan0, _lastData.Stride, centerX, centerY);
                            bool lastSame = lastBorderColor == lastCenterColor;

                            if (same != lastSame)
                            {
                                changedSquares.Add(new Point(x, y));
                            }
                            else if (same == false && centerColor != lastCenterColor)
                            {
                                changedSquares.Add(new Point(x, y));
                            }
                        }
                    }

                    boardSnap.UnlockBits(_data);
                    _lastBoardSnap.UnlockBits(_lastData);
                }

                return changedSquares.ToArray();
            }

            return null;
        }

        private unsafe static int GetPixel(byte* scan0, int stride, int x, int y)
        {
            byte* colPointer = scan0;
            colPointer += y * stride + x * 3;

            return colPointer[0] * 255 * 255 + colPointer[1] * 255 + (int)colPointer[2];
        }

        public static Bitmap GetBoardSnap()
        {
            if (BoardFound)
            {
                Screen screen = Screen.AllScreens[ScreenIndex];
                Bitmap screenshot = new Bitmap(BoardSize.Width, BoardSize.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(screenshot);
                g.CopyFromScreen(screen.Bounds.X + BoardLocation.X, screen.Bounds.Y + BoardLocation.Y, 0, 0, BoardSize, CopyPixelOperation.SourceCopy);
                return screenshot;
            }

            return null;
        }

        public static Bitmap GetScreenshot(Screen screen)
        {
            Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(screenshot);
            g.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return screenshot;
        }

        private static bool SearchBoard(Bitmap image, Color darkSquareColor, Color lightSquareColor)
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
            int initSuccTol = 4;
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
                                initSuccTol = (int)Math.Ceiling(boardWidth / 10.0);
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
                BoardLocation = new Point(boardX, boardY);
                BoardSize = new Size(boardWidth, boardHeight);
                FieldSize = new SizeF(BoardSize.Width / (float)SideLength, BoardSize.Height / (float)SideLength);
            }

            return heightFound;
        }

        private static bool IsSameColor(Color color1, Color color2, int tolerance)
        {
            return tolerance >= Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }

        private static int Round(double number)
        {
            return (int)Math.Round(number, 0);
        }

        #endregion
    }
}
