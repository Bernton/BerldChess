using BerldChess.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormPgnLoader : Form
    {
        public FormPgnLoader()
        {
            InitializeComponent();
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            Icon = Resources.PawnRush;
        }
    }
}
