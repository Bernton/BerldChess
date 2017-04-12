using BerldChess.Model;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class PieceSelectionDialog : Form
    {
        private List<ChessFont> _chessFonts;

        public event Action FontSelected;

        #region Constructors

        public PieceSelectionDialog(List<ChessFont> chessFonts, int selectedIndex)
        {
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            _chessFonts = chessFonts;

            InitializeComponent();

            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
            {
                _comboBoxFont.Items.Add(fonts.Families[i].Name);
            }

            for (int i = 0; i < _chessFonts.Count; i++)
            {
                _listBoxConfigs.Items.Add(_chessFonts[i].Name);
            }

            _listBoxConfigs.SelectedIndex = selectedIndex;
            OpenFontConfig(_listBoxConfigs.SelectedIndex);
        }

        #endregion

        #region Methods

        private void OnListBoxConfigsSelectedIndexChanged(object sender, EventArgs e)
        {
            OpenFontConfig(_listBoxConfigs.SelectedIndex);
        }

        private void OpenFontConfig(int index)
        {
            if (index < 0)
            {
                index = 0;
            }

            if (index == 0)
            {
                _buttonApply.Enabled = false;
                _buttonRemove.Enabled = false;
            }
            else
            {
                _buttonApply.Enabled = true;
                _buttonRemove.Enabled = true;
            }

            ChessFont font = _chessFonts[index];

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
            ChessFont font = _chessFonts[_listBoxConfigs.SelectedIndex];

            if (double.TryParse(_textBoxSizeFactor.Text, out factor))
            {
                font.SizeFactor = factor;
            }

            font.IsUnicode = _checkBoxUnicodeFont.Checked;
            font.FontFamily = _comboBoxFont.Text;
            font.PieceCharacters = _textBoxFontChars.Text;
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

                if (_chessFonts[_listBoxConfigs.SelectedIndex].PieceCharacters == "")
                {
                    _textBoxFontChars.Text = (string)_textBoxFontChars.Tag;
                }
            }
            else
            {
                _textBoxFontChars.Text = "";
            }
        }

        private void OnButtonCloseClick(object sender, EventArgs e)
        {
            int selected = _listBoxConfigs.SelectedIndex;

            SerializedInfo.Instance.ChessFonts.RemoveAt(selected);

            _listBoxConfigs.Items.RemoveAt(selected);
            _listBoxConfigs.SelectedIndex = selected - 1;
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

            while (_listBoxConfigs.Items.Contains(name))
            {
                MessageBox.Show(this, "Can't have 2 font configurations with the same name.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                string input = Interaction.InputBox("Enter new name:", "BerldChess - Name");

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

            newFont.Name = _textBoxName.Text;
            newFont.IsUnicode = _checkBoxUnicodeFont.Checked;
            newFont.FontFamily = _comboBoxFont.Text;
            newFont.PieceCharacters = _textBoxFontChars.Text;

            SerializedInfo.Instance.ChessFonts.Add(newFont);
            SerializedInfo.Instance.SelectedFontIndex = SerializedInfo.Instance.ChessFonts.Count - 1;

            _listBoxConfigs.Items.Add(newFont.Name);
            _listBoxConfigs.SelectedIndex = _listBoxConfigs.Items.Count - 1;

            FontSelected?.Invoke();
        }



        #endregion
    }
}
