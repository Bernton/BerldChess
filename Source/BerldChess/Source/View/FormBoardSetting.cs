using BerldChess.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormBoardSetting : Form
    {
        private bool _inititalGradient;
        private bool _initialGrid;
        private bool _initialDarkMode;
        private bool _initialHighlightBorder;
        private bool _applied = false;

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

            if(colorDialog.ShowDialog() == DialogResult.OK)
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
            if(!_applied)
            {
                SerializedInfo.Instance.BoardDarkSquare = _initialDarkSquare;
                SerializedInfo.Instance.BoardLightSquare = _initialLightSquare;
                SerializedInfo.Instance.Gradient = _inititalGradient;
                SerializedInfo.Instance.DisplayGridBorder = _initialGrid;
                SerializedInfo.Instance.DarkMode = _initialDarkMode;
                SerializedInfo.Instance.BorderHighlight = _initialHighlightBorder;
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
            _inititalGradient = SerializedInfo.Instance.Gradient;
            _initialGrid = SerializedInfo.Instance.DisplayGridBorder;
            _initialDarkMode = SerializedInfo.Instance.DarkMode;
            _initialHighlightBorder = SerializedInfo.Instance.BorderHighlight;

            _buttonDarkSquare.BackColor = _initialDarkSquare;
            _buttonLightSquare.BackColor = _initialLightSquare;
            _checkBoxGridBorder.Checked = _initialGrid;
            _checkBoxGradient.Checked = _inititalGradient;
            _checkBoxDarkMode.Checked = _initialDarkMode;
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
    }
}
