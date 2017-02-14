using System;
using System.Drawing;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormSquareColorDialog : Form
    {
        #region Fields

        private int _imageIndex = 0;
        private Bitmap[] _images;

        #endregion

        #region Properties

        public Color? DarkSquareColor { get; private set; } = null;
        public Color? LightSquareColor { get; private set; } = null;

        #endregion

        #region Constructors

        public FormSquareColorDialog(Bitmap[] images)
        {
            if (images == null)
            {
                throw new ArgumentException("'images' must not be null.");
            }

            InitializeComponent();

            _images = images;
            pictureBox.Image = _images[_imageIndex];
        }

        #endregion

        #region Methods

        private void OnPictureBoxMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _imageIndex++;

                if (_imageIndex >= _images.Length)
                {
                    _imageIndex = 0;
                }

                pictureBox.Image = _images[_imageIndex];
            }
            else
            {
                int x = (int)((double)e.X / pictureBox.Width * _images[_imageIndex].Width);
                int y = (int)((double)e.Y / pictureBox.Height * _images[_imageIndex].Height);

                if (DarkSquareColor == null)
                {
                    DarkSquareColor = _images[_imageIndex].GetPixel(x, y);
                    Text = "Square Color Dialog - Select Light Square";
                }
                else
                {
                    LightSquareColor = _images[_imageIndex].GetPixel(x, y);
                    DialogResult = DialogResult.OK;
                }
            }
        }

        #endregion
    }
}
