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

            PropertyInfo propertyInfo = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfo.SetValue(control, true, null);
        }

        public static void FitFont(this Control control, double widthFactor, double heightFactor)
        {
            ChangeFontSize(control, GetFitFontSize(control, widthFactor, heightFactor));
        }

        public static void ChangeFontSize(Control control, int fontSize)
        {
            control.Font = new Font(control.Font.FontFamily, fontSize, control.Font.Style);
        }

        public static int GetFitFontSize(Control control, double widthFactor, double heightFactor)
        {
            int fontSize = 1;
            Font currentFont = new Font(control.Font.FontFamily, fontSize);

            while (TextRenderer.MeasureText(control.Text, currentFont).Width / widthFactor < control.Width &&
                TextRenderer.MeasureText(control.Text, currentFont).Height / heightFactor < control.Height)
            {
                fontSize++;
                currentFont = new Font(control.Font.FontFamily, fontSize);
            }

            return fontSize;
        }
    }
}
