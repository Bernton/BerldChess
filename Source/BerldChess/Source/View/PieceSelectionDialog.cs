using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class PieceSelectionDialog : Form
    {
        #region Fields

        private bool _isUnicode;
        private double _sizeFactor;
        private string _fontFamily;
        private string _fontChars;

        #endregion

        #region Properties & Events

        public bool IsUnicode
        {
            get
            {
                return _isUnicode;
            }

            set
            {
                _isUnicode = value;
                _checkBoxUnicodeFont.Checked = _isUnicode;
            }
        }

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

        public string FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                _fontFamily = value;
                _comboBoxFont.Text = _fontFamily;

                if (_fontFamily == "")
                {
                    _checkBoxDefault.Checked = true;
                }
                else
                {
                    _checkBoxDefault.Checked = false;
                }
            }
        }

        public string FontChars
        {
            get
            {
                return _fontChars;
            }
            set
            {
                _fontChars = value;
                _textBoxFontChars.Text = _fontChars;
            }
        }

        public event Action FontSelected;

        #endregion

        #region Constructors

        public PieceSelectionDialog()
        {
            InitializeComponent();

            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
            {
                _comboBoxFont.Items.Add(fonts.Families[i].Name);
            }
        }

        #endregion

        #region Methods

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            double factor;

            if (double.TryParse(_textBoxSizeFactor.Text, out factor))
            {
                _sizeFactor = factor;
            }

            if (_checkBoxDefault.Checked)
            {
                FontFamily = "";
            }
            else if (FontExists(_comboBoxFont.Text, FontStyle.Regular))
            {
                _isUnicode = _checkBoxUnicodeFont.Checked;
                _fontFamily = _comboBoxFont.Text;
                _fontChars = _textBoxFontChars.Text;
            }

            FontSelected?.Invoke();
        }

        private void OnCheckBoxDefaultCheckedChanged(object sender, EventArgs e)
        {
            if (_checkBoxDefault.Checked)
            {
                _checkBoxUnicodeFont.Enabled = false;
                _comboBoxFont.Enabled = false;
                _labelFontChars.Enabled = false;
                _textBoxFontChars.Enabled = false;
            }
            else
            {
                _checkBoxUnicodeFont.Enabled = true;
                _comboBoxFont.Enabled = true;
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
            if (FontExists(_comboBoxFont.Text, FontStyle.Regular))
            {
                _textBoxFontChars.Font = new Font(_comboBoxFont.Text, _textBoxFontChars.Font.Size);
                _textBoxFontChars.Text = (string)_textBoxFontChars.Tag;
            }
            else
            {
                _textBoxFontChars.Text = "";
            }
        }

        private void OnButtonCloseClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private bool FontExists(string fontFamilyName, FontStyle fontStyle)
        {
            bool result;

            try
            {
                using (FontFamily family = new FontFamily(fontFamilyName))
                {
                    result = family.IsStyleAvailable(fontStyle);
                }
            }
            catch (ArgumentException)
            {
                result = false;
            }

            return result;
        }

        #endregion
    }
}
