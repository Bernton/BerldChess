//**********************************************
// Project: Flood Fill Algorithms in C# & GDI+
// File Description: Flood Fill Class
//
// Copyright: Copyright 2003 by Justin Dunlap.
//    Any code herein can be used freely in your own 
//    applications, provided that:
//     * You agree that I am NOT to be held liable for
//       any damages caused by this code or its use.
//     * You give proper credit to me if you re-publish
//       this code.
//**********************************************
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BerldChess.Model
{
    using System;

    /// <summary>
    /// TODO - Add class summary
    /// </summary>
    /// <remarks>
    /// 	created by - J Dunlap
    /// 	created on - 7/2/2003 11:44:33 PM
    /// </remarks>
    public class FloodFiller : AbstractFloodFiller
    {
        private byte[] m_fillcolor = null;
        private byte[] deviation;

        /// <summary>
        /// Default constructor - initializes all fields to default values
        /// </summary>
        public FloodFiller()
        {
        }

        ///<summary>initializes the FloodFill operation</summary>
        public override void FloodFill(BitmapData bmpData, Point pt)
        {
            int ctr = timeGetTime();

            //Debug.WriteLine("*******Flood Fill******");

            //get the color's int value, and convert it from RGBA to BGRA format (as GDI+ uses BGRA)
            m_fillcolor = new byte[]
            {
                m_fillcolorcolor.B,
                m_fillcolorcolor.G,
                m_fillcolorcolor.R,
                m_fillcolorcolor.A
            };

            System.IntPtr Scan0 = bmpData.Scan0;

            unsafe
            {
                //resolve pointer
                byte* scan0 = (byte*)(void*)Scan0;
                //get the starting color
                //[loc += Y offset + X offset]
                int loc = CoordsToIndex(pt.X, pt.Y, bmpData.Stride);//((bmpData.Stride*(pt.Y-1))+(pt.X*4));
                int color = *((int*)(scan0 + loc));

                //create the array of bools that indicates whether each pixel
                //has been checked.  (Should be bitfield, but C# doesn't support bitfields.)
                PixelsChecked = new bool[bmpData.Width + 1, bmpData.Height + 1];


                LinearFloodFill4(scan0, pt.X, pt.Y, new Size(bmpData.Width, bmpData.Height), bmpData.Stride, (byte*)&color);

            }

            m_TimeBenchmark = timeGetTime() - ctr;
        }

        unsafe void LinearFloodFill4(byte* scan0, int x, int y, Size bmpsize, int stride, byte* startcolor)
        {
            //offset the pointer to the point passed in
            int* p = (int*)(scan0 + (CoordsToIndex(x, y, stride)));


            //FIND LEFT EDGE OF COLOR AREA
            int LFillLoc = x; //the location to check/fill on the left
            byte* ptr = (byte*)p; //the pointer to the current location
            while (true)
            {
                ptr[0] = m_fillcolor[0];    //fill with the color
                ptr[1] = m_fillcolor[1];
                ptr[2] = m_fillcolor[2];
                ptr[3] = m_fillcolor[3];

                PixelsChecked[LFillLoc, y] = true;
                LFillLoc--;              //de-increment counter
                ptr -= 4;                    //de-increment pointer
                if (LFillLoc <= 0 || !CheckPixel((byte*)ptr, startcolor) || (PixelsChecked[LFillLoc, y]))
                    break;               //exit loop if we're at edge of bitmap or color area
            }

            LFillLoc++;

            //FIND RIGHT EDGE OF COLOR AREA
            int RFillLoc = x; //the location to check/fill on the left
            ptr = (byte*)p;
            while (true)
            {
                ptr[0] = m_fillcolor[0];   
                ptr[1] = m_fillcolor[1];
                ptr[2] = m_fillcolor[2];
                ptr[3] = m_fillcolor[3];

                PixelsChecked[RFillLoc, y] = true;
                RFillLoc++;          //increment counter
                ptr += 4;                //increment pointer
                if (RFillLoc >= bmpsize.Width || !CheckPixel((byte*)ptr, startcolor) || (PixelsChecked[RFillLoc, y]))
                    break;           //exit loop if we're at edge of bitmap or color area

            }
            RFillLoc--;


            //START THE LOOP UPWARDS AND DOWNWARDS			
            ptr = (byte*)(scan0 + CoordsToIndex(LFillLoc, y, stride));
            for (int i = LFillLoc; i <= RFillLoc; i++)
            {
                //START LOOP UPWARDS
                //if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
                if (y > 0 && CheckPixel((byte*)(scan0 + CoordsToIndex(i, y - 1, stride)), startcolor) && (!(PixelsChecked[i, y - 1])))
                    LinearFloodFill4(scan0, i, y - 1, bmpsize, stride, startcolor);
                //START LOOP DOWNWARDS
                if (y < (bmpsize.Height - 1) && CheckPixel((byte*)(scan0 + CoordsToIndex(i, y + 1, stride)), startcolor) && (!(PixelsChecked[i, y + 1])))
                    LinearFloodFill4(scan0, i, y + 1, bmpsize, stride, startcolor);
                ptr += 4;
            }

        }

        unsafe bool CheckPixel(byte* px, byte* startcolor)
        {
            if (px[3] == 0 && startcolor[3] == 0)
            {
                return true;
            }

            bool ret = true;
            for (byte i = 0; i < 4; i++)
                ret &= (px[i] >= (startcolor[i] - m_Tolerance[i])) && px[i] <= (startcolor[i] + m_Tolerance[i]);

            return ret;
        }

        int CoordsToIndex(int x, int y, int stride)
        {
            return (stride * y) + (x * 4);
        }
    }

}
