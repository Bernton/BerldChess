using BerldChess.View;
using System;
using System.Windows.Forms;

namespace BerldChess
{
    public static class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}