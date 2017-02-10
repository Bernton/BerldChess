using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class PieceSelectionDialog : Form
    {
        public event Action FontSelected;

        private string _pieceFontFamily;
        private bool _isUnicodeFont;
        private string _chessFontChars;
        private double _sizeFactor;

        public double SizeFactor
        {
            get
            {
                return _sizeFactor;
            }
            set
            {
                _sizeFactor = value;
                _textBoxSizeFactor.Text = _sizeFactor.ToString();
            }
        }

        public string ChessFontChars
        {
            get
            {
                return _chessFontChars;
            }
            set
            {
                _chessFontChars = value;
                _textBoxFontChars.Text = value;
            }
        }

        public bool IsUnicodeFont
        {
            get
            {
                return _isUnicodeFont;
            }

            set
            {
                _isUnicodeFont = value;
                _checkBoxUnicodeFont.Checked = _isUnicodeFont;
            }
        }

        public string PieceFontFamily
        {
            get
            {
                return _pieceFontFamily;
            }
            set
            {
                _pieceFontFamily = value;
                _comboBoxFont.Text = _pieceFontFamily;

                if (_pieceFontFamily == "")
                {
                    _checkBoxDefault.Checked = true;
                }
                else
                {
                    _checkBoxDefault.Checked = false;
                }
            }
        }

        public PieceSelectionDialog()
        {
            InitializeComponent();

            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
            {
                _comboBoxFont.Items.Add(fonts.Families[i].Name);
            }
        }

        private void OnButtonApplyCloseClick(object sender, EventArgs e)
        {
            if (_checkBoxDefault.Checked)
            {
                PieceFontFamily = "";
                DialogResult = DialogResult.OK;
            }
            else if (DoesFontExist(_comboBoxFont.Text, FontStyle.Regular))
            {
                double factor;

                if(double.TryParse(_textBoxSizeFactor.Text, out factor))
                {
                    _sizeFactor = factor;
                }

                _isUnicodeFont = _checkBoxUnicodeFont.Checked;
                PieceFontFamily = _comboBoxFont.Text;
                ChessFontChars = _textBoxFontChars.Text;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid font.", "Berd Chess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            if (_checkBoxDefault.Checked)
            {
                PieceFontFamily = "";
                FontSelected?.Invoke();
            }
            else if (DoesFontExist(_comboBoxFont.Text, FontStyle.Regular))
            {
                double factor;

                if (double.TryParse(_textBoxSizeFactor.Text, out factor))
                {
                    _sizeFactor = factor;
                }

                _isUnicodeFont = _checkBoxUnicodeFont.Checked;
                PieceFontFamily = _comboBoxFont.Text;
                ChessFontChars = _textBoxFontChars.Text;
                FontSelected?.Invoke();
            }
        }

        private bool DoesFontExist(string fontFamilyName, FontStyle fontStyle)
        {
            bool result;

            try
            {
                using (FontFamily family = new FontFamily(fontFamilyName))
                    result = family.IsStyleAvailable(fontStyle);
            }
            catch (ArgumentException)
            {
                result = false;
            }

            return result;
        }

        private void OnCheckBoxDefaultCheckedChanged(object sender, EventArgs e)
        {
            if (_checkBoxDefault.Checked)
            {
                _comboBoxFont.Enabled = false;
                _checkBoxUnicodeFont.Enabled = false;
                _labelFontChars.Enabled = false;
                _textBoxFontChars.Enabled = false;
            }
            else
            {
                _comboBoxFont.Enabled = true;
                _checkBoxUnicodeFont.Enabled = true;
                _labelFontChars.Enabled = true;
                _textBoxFontChars.Enabled = true;
            }
        }

        private void OnCheckBoxUnicodeFontCheckedChanged(object sender, EventArgs e)
        {
            if (_checkBoxUnicodeFont.Checked)
            {
                _labelFontChars.Enabled = false;
                _textBoxFontChars.Enabled = false;
            }
            else
            {
                _labelFontChars.Enabled = true;
                _textBoxFontChars.Enabled = true;
            }
        }

        private void OnComboBoxFontTextChanged(object sender, EventArgs e)
        {
            if (DoesFontExist(_comboBoxFont.Text, FontStyle.Regular))
            {
                _textBoxFontChars.Font = new Font(_comboBoxFont.Text, _textBoxFontChars.Font.Size);
                _textBoxFontChars.Text = (string)_textBoxFontChars.Tag;
            }
            else
            {
                _textBoxFontChars.Text = "";
            }
        }
    }
}
