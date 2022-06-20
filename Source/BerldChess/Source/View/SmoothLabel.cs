using System.Windows.Forms;
using static System.Drawing.Text.TextRenderingHint;

namespace BerldChess.View
{
    public class SmoothLabel : Label
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = Font.Size > 15 ? AntiAliasGridFit : SystemDefault;
            base.OnPaint(e);
        }
    }
}
