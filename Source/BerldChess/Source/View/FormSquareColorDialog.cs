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
    public partial class FormSquareColorDialog : Form
    {
        private int _imageI = 0;
        private Bitmap[] _images;

        public Color? DarkSquareColor { get; private set; } = null;
        public Color? LightSquareColor { get; set; } = null;

        public FormSquareColorDialog(Bitmap[] images)
        {
            if(images == null)
            {
                throw new ArgumentException("'images' must not be null.");
            }

            InitializeComponent();

            _images = images;

            pictureBox.Image = _images[_imageI];
        }

        private void OnPictureBoxMouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                _imageI++;
                pictureBox.Image = _images[_imageI % _images.Length];
            }
            else
            {
                int x = (int)((double)e.X / pictureBox.Width * _images[_imageI].Width);
                int y = (int)((double)e.Y / pictureBox.Height * _images[_imageI].Height);

                if(DarkSquareColor == null)
                {
                    DarkSquareColor = _images[_imageI].GetPixel(x, y);
                    Text = "Square Color Dialog - Light Square";
                }
                else
                {
                    LightSquareColor = _images[_imageI].GetPixel(x, y);
                    DialogResult = DialogResult.OK;
                }
            }
        }
    }
}
