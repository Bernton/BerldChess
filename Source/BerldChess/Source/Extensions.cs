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

            while (size.Width < factorWidth && size.Height < factorHeight )
            {
                fontSize++;
                size = TextRenderer.MeasureText(control.Text, new Font(control.Font.FontFamily, fontSize));
            }

            control.Font = new Font(control.Font.FontFamily, fontSize);
        }
    }
}
