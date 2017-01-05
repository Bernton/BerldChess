using BerldChess.Properties;
using ChessDotNet;
using System;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class PromotionDialog : Form
    {
        #region Fields

        private const int CP_NOCLOSE_BUTTON = 0x200;

        #endregion

        #region Properties

        public char PromotionCharacter { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        #endregion

        #region Constructors

        public PromotionDialog(ChessPlayer owner)
        {
            InitializeComponent();
            Icon = Resources.PawnRush;

            switch (owner)
            {
                case ChessPlayer.Black:

                    _buttonQueen.BackgroundImage = Resources.PieceQs;
                    _buttonRook.BackgroundImage = Resources.PieceRs;
                    _buttonBishop.BackgroundImage = Resources.PieceBs;
                    _buttonKnight.BackgroundImage = Resources.PieceNs;
                    break;

                case ChessPlayer.None:
                case ChessPlayer.White:

                    _buttonQueen.BackgroundImage = Resources.PieceQ;
                    _buttonRook.BackgroundImage = Resources.PieceR;
                    _buttonBishop.BackgroundImage = Resources.PieceB;
                    _buttonKnight.BackgroundImage = Resources.PieceN;
                    break;
            }
        }

        #endregion

        #region Event Methods

        private void OnButtonClick(object sender, EventArgs e)
        {
            PromotionCharacter = char.Parse((string)((Control)sender).Tag);
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}
