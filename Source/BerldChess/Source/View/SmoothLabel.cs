// Type: BerldChess.View.SmoothLabel
// Assembly: BerldChess, Version=1.7.7088.41946, Culture=neutral, PublicKeyToken=null
// MVID: CE1A7504-EDE2-473D-8504-A5A5C388C662
// Assembly location: C:\Users\User\AppData\Local\Apps\2.0\B42VM5KK.YGV\2GDWEP3Q.8Q8\berl..tion_0000000000000000_0001.0007_66bb90a13acccc7d\BerldChess.exe

using System.Drawing.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public class SmoothLabel : Label
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = (double)this.Font.Size > 15.0 ? TextRenderingHint.AntiAliasGridFit : TextRenderingHint.SystemDefault;
            base.OnPaint(e);
        }
    }
}
