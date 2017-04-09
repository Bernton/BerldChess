using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BerldChess.Source.Model
{
    public static class Extensions
    {
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
            if(isAdd)
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
