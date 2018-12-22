using BerldChess.Model;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormPieceSettings : Form
    {
        private List<ChessFont> _chessFonts;

        public event Action FontSelected;

        #region Constructors

        public FormPieceSettings(List<ChessFont> chessFonts, int selectedIndex)
        {
            _chessFonts = chessFonts;

            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            InitializeComponent();
            _listBoxSettings.SetDoubleBuffered();
            UpdateList(selectedIndex);

            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
            {
                _comboBoxFont.Items.Add(fonts.Families[i].Name);
            }
        }

        #endregion

        #region Methods

        private void UpdateList(int initialSelect)
        {
            _listBoxSettings.Items.Clear();

            for (int i = 0; i < _chessFonts.Count; i++)
            {
                _listBoxSettings.Items.Add(_chessFonts[i].Name);
            }

            _listBoxSettings.SelectedIndex = initialSelect;
            OpenFontConfig(_listBoxSettings.SelectedIndex);
        }

        private void OnListBoxConfigsSelectedIndexChanged(object sender, EventArgs e)
        {
            OpenFontConfig(_listBoxSettings.SelectedIndex);
        }

        private void OpenFontConfig(int index)
        {
            if (index < 0)
            {
                index = 0;
            }

            bool notDefault = index > 2;

            _panelFontSettings.Enabled = notDefault;
            _textBoxName.Enabled = notDefault;
            _buttonRemove.Enabled = notDefault;

            ChessFont font = (ChessFont)_chessFonts[index];

            _textBoxName.Text = font.Name;
            _textBoxSizeFactor.Text = font.SizeFactor.ToString();
            _textBoxFontChars.Text = font.PieceCharacters;
            _checkBoxUnicodeFont.Checked = font.IsUnicode;
            _comboBoxFont.Text = font.FontFamily;

            SerializedInfo.Instance.SelectedFontIndex = index;
            FontSelected?.Invoke();
        }

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            double factor;
            string name = _textBoxName.Text;
            ChessFont font = (ChessFont)_chessFonts[_listBoxSettings.SelectedIndex];
            bool nameChanged = name != font.Name;

            if (nameChanged && _listBoxSettings.Items.Contains(name))
            {
                MessageBox.Show(this, "Setting with this name already exists.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (double.TryParse(_textBoxSizeFactor.Text, out factor))
            {
                font.SizeFactor = factor;
            }

            if (_listBoxSettings.SelectedIndex != 0)
            {
                if (nameChanged)
                {
                    font.Name = name;
                    UpdateList(_listBoxSettings.SelectedIndex);
                }

                font.IsUnicode = _checkBoxUnicodeFont.Checked;
                font.FontFamily = _comboBoxFont.Text;
                font.PieceCharacters = _textBoxFontChars.Text;
            }

            FontSelected?.Invoke();
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
            if (FontExists(_comboBoxFont.Text, FontStyle.Regular) && _comboBoxFont.Text != "")
            {
                _textBoxFontChars.Font = new Font(_comboBoxFont.Text, _textBoxFontChars.Font.Size);

                if (((ChessFont)_chessFonts[_listBoxSettings.SelectedIndex]).PieceCharacters == "")
                {
                    _textBoxFontChars.Text = (string)_textBoxFontChars.Tag;
                }
            }
            else
            {
                _textBoxFontChars.Text = "";
            }
        }

        private void OnButtonRemoveClick(object sender, EventArgs e)
        {
            int selected = _listBoxSettings.SelectedIndex;

            SerializedInfo.Instance.ChessFonts.RemoveAt(selected);

            _listBoxSettings.Items.RemoveAt(selected);
            _listBoxSettings.SelectedIndex = selected - 1;
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

        private void OnButtonAddNewClick(object sender, EventArgs e)
        {
            double factor;
            ChessFont newFont = new ChessFont();

            string name = _textBoxName.Text;

            bool warn = false;

            while (_listBoxSettings.Items.Contains(name))
            {
                if (warn)
                {
                    MessageBox.Show(this, "Setting with this name already exists.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    warn = true;
                }

                string input = Interaction.InputBox("Enter new name for setting:", "BerldChess - Setting name");

                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }
                else
                {
                    name = input;
                }
            }

            if (double.TryParse(_textBoxSizeFactor.Text, out factor))
            {
                newFont.SizeFactor = factor;
            }
            else
            {
                newFont.SizeFactor = 1;
            }

            newFont.Name = name;
            newFont.IsUnicode = _checkBoxUnicodeFont.Checked;
            newFont.FontFamily = _comboBoxFont.Text;
            newFont.PieceCharacters = _textBoxFontChars.Text;

            SerializedInfo.Instance.ChessFonts.Add(newFont);
            SerializedInfo.Instance.SelectedFontIndex = SerializedInfo.Instance.ChessFonts.Count - 1;

            _listBoxSettings.Items.Add(newFont.Name);
            _listBoxSettings.SelectedIndex = _listBoxSettings.Items.Count - 1;

            FontSelected?.Invoke();
        }



        #endregion
    }
}
