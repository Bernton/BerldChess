using BerldChess.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormBoardSetting : Form
    {
        private bool _initialGradient;
        private bool _initialGrid;
        private bool _initialDarkMode;
        private bool _initialHighlightBorder;
        private bool _initialIvoryMode;
        private bool _initialUseImages;
        private bool _applied = false;

        private string _initialDarkSquarePath;
        private string _initialLightSquarePath;
        private Color _initialDarkSquare;
        private Color _initialLightSquare;

        public event Action BoardSettingAltered;

        #region Constructors

        public FormBoardSetting()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void OnButtonDarkSquareClick(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = _buttonDarkSquare.BackColor;

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                _buttonDarkSquare.BackColor = colorDialog.Color;

                SerializedInfo.Instance.BoardDarkSquare = colorDialog.Color;
                BoardSettingAltered?.Invoke();
            }
        }

        private void OnButtonLightSquareClick(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = _buttonLightSquare.BackColor;

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                _buttonLightSquare.BackColor = colorDialog.Color;

                SerializedInfo.Instance.BoardLightSquare = colorDialog.Color;
                BoardSettingAltered?.Invoke();
            }
        }

        private void OnCheckBoxGridBorderCheckedChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.DisplayGridBorder = _checkBoxGridBorder.Checked;
            BoardSettingAltered?.Invoke();
        }

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            _applied = true;
            Close();
        }

        private void OnBoardSettingDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_applied)
            {
                SerializedInfo.Instance.BoardDarkSquare = _initialDarkSquare;
                SerializedInfo.Instance.BoardLightSquare = _initialLightSquare;
                SerializedInfo.Instance.Gradient = _initialGradient;
                SerializedInfo.Instance.DisplayGridBorder = _initialGrid;
                SerializedInfo.Instance.DarkMode = _initialDarkMode;
                SerializedInfo.Instance.BorderHighlight = _initialHighlightBorder;
                SerializedInfo.Instance.IvoryMode = _initialIvoryMode;
                SerializedInfo.Instance.UseImages = _initialUseImages;
                SerializedInfo.Instance.DarkSquarePath = _initialDarkSquarePath;
                SerializedInfo.Instance.LightSquarePath = _initialLightSquarePath;
                BoardSettingAltered?.Invoke();
            }
            else
            {
                _applied = false;
            }
        }

        private void OnBoardSettingDialogShown(object sender, EventArgs e)
        {

            _initialDarkSquare = SerializedInfo.Instance.BoardDarkSquare;
            _initialLightSquare = SerializedInfo.Instance.BoardLightSquare;
            _initialGradient = SerializedInfo.Instance.Gradient;
            _initialGrid = SerializedInfo.Instance.DisplayGridBorder;
            _initialDarkMode = SerializedInfo.Instance.DarkMode;
            _initialHighlightBorder = SerializedInfo.Instance.BorderHighlight;
            _initialIvoryMode = SerializedInfo.Instance.IvoryMode;
            _initialUseImages = SerializedInfo.Instance.UseImages;
            _initialLightSquarePath = SerializedInfo.Instance.LightSquarePath;
            _initialDarkSquarePath = SerializedInfo.Instance.DarkSquarePath;

            _buttonDarkSquare.BackColor = _initialDarkSquare;
            _buttonLightSquare.BackColor = _initialLightSquare;
            _checkBoxGridBorder.Checked = _initialGrid;
            _checkBoxGradient.Checked = _initialGradient;
            _checkBoxDarkMode.Checked = _initialDarkMode;
            _checkBoxIvoryMode.Checked = _initialIvoryMode;
            _checkBoxUseImages.Checked = _initialUseImages;
            _textBoxDarkSquarePath.Text = _initialDarkSquarePath;
            _textBoxLightSquarePath.Text = _initialLightSquarePath;

            applyUseImageUI(_initialUseImages);
        }

        private void OnCheckBoxDarkModeCheckedChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.DarkMode = _checkBoxDarkMode.Checked;
            BoardSettingAltered?.Invoke();
        }

        private void OnCheckBoxGradientCheckedChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.Gradient = _checkBoxGradient.Checked;
            BoardSettingAltered?.Invoke();
        }

        private void OnCheckBoxHighlightBorderCheckedChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.BorderHighlight = _checkBoxHighlightBorder.Checked;
            BoardSettingAltered?.Invoke();
        }

        #endregion

        private void OnCheckBoxIvoryModeCheckedChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.IvoryMode = _checkBoxIvoryMode.Checked;
            BoardSettingAltered?.Invoke();
        }

        private void OnCheckBoxUseImagesCheckedChanged(object sender, EventArgs e)
        {
            bool useImages = _checkBoxUseImages.Checked;

            SerializedInfo.Instance.UseImages = useImages;
            BoardSettingAltered?.Invoke();

            applyUseImageUI(useImages);
        }

        private void applyUseImageUI(bool useImages)
        {
            _labelShowDarkSquarePath.Enabled = useImages;
            _labelShowLightSquarePath.Enabled = useImages;
            _labelShowDarkSquarePath.Enabled = useImages;
            _labelShowLightSquarePath.Enabled = useImages;

            _labelShowDarkSquareColor.Enabled = !useImages;
            _labelShowLightSquareColor.Enabled = !useImages;
            _buttonDarkSquare.Enabled = !useImages;
            _buttonLightSquare.Enabled = !useImages;
        }

        private void OnTextBoxDarkSquarePathTextChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.DarkSquarePath = _textBoxDarkSquarePath.Text;
            BoardSettingAltered?.Invoke();
        }

        private void OnTextBoxLightSquarePathTextChanged(object sender, EventArgs e)
        {
            SerializedInfo.Instance.LightSquarePath = _textBoxLightSquarePath.Text;
            BoardSettingAltered?.Invoke();
        }

        private void OnButtonDarkDialogClick(object sender, EventArgs e)
        {
            string path = RequestPath("BerldChess - Choose dark square image", _textBoxDarkSquarePath.Text);

            if (path != null)
            {
                _textBoxDarkSquarePath.Text = path;
            }
        }

        private void OnButtonLightDialogClick(object sender, EventArgs e)
        {
            string path = RequestPath("BerldChess - Choose light square image", _textBoxLightSquarePath.Text);

            if (path != null)
            {
                _textBoxLightSquarePath.Text = path;
            }
        }

        private string RequestPath(string title, string initialPath)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = initialPath,
                Multiselect = false,
                Title = title
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
