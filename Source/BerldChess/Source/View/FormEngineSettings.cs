using BerldChess.Model;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormEngineSettings : Form
    {
        #region Fields

        private EngineList _engineList;

        #endregion

        #region Events

        public event Action EngineSelected;

        #endregion

        #region Constructors

        public FormEngineSettings(EngineList engineList)
        {
            InitializeComponent();
            _listBoxSettings.SetDoubleBuffered();
            _engineList = engineList;
            UpdateUI(_engineList.SelectedIndex);
            InitializeRadioButtons();
        }

        #endregion

        #region Event Methods

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            if (!_engineList.SettingAvailable)
            {
                return;
            }

            EngineSetting setting = _engineList.Settings[_listBoxSettings.SelectedIndex];
            string name = _textBoxName.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            bool nameChanged = name != setting.Name;

            if (nameChanged && _listBoxSettings.Items.Contains(name))
            {
                MessageBox.Show(this, "Setting with this name already exists.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nameChanged)
            {
                setting.Name = name;
            }

            setting.ExecutablePath = _comboBoxPath.Text;
            setting.Arguments = _textBoxArguments.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            _engineList.TryAddPath(setting.ExecutablePath);
            EngineSelected?.Invoke();

            UpdateUI(_listBoxSettings.SelectedIndex);
        }

        private void OnButtonAddNewClick(object sender, EventArgs e)
        {
            EngineSetting setting = new EngineSetting();
            string name = _textBoxName.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "Name is missing.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            setting.Name = name;
            setting.ExecutablePath = _comboBoxPath.Text;
            setting.Arguments = _textBoxArguments.Text.Split('\n');

            _engineList.TryAddPath(setting.ExecutablePath);
            _engineList.Settings.Add(setting);

            UpdateUI(_listBoxSettings.SelectedIndex);

            _listBoxSettings.SelectedIndex = _listBoxSettings.Items.Count - 1;
        }

        private void OnButtonRemoveClick(object sender, EventArgs e)
        {
            if (!_engineList.SettingAvailable)
            {
                return;
            }

            int selected = _listBoxSettings.SelectedIndex;

            _engineList.Settings.RemoveAt(selected);

            if (_engineList.SelectedIndex == selected)
            {
                _engineList.SelectedIndex = 0;
            }

            if (_engineList.SelectedOpponentIndex == selected)
            {
                _engineList.SelectedOpponentIndex = 0;
            }

            EngineSelected?.Invoke();

            UpdateUI(_listBoxSettings.SelectedIndex);

            if (selected - 1 >= 0)
            {
                _listBoxSettings.SelectedIndex = selected - 1;
            }
        }

        private void OnListBoxSettingsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxSettings.SelectedIndex != -1)
            {
                OpenEngineConfig(_listBoxSettings.SelectedIndex);
            }
        }

        private void OnComboBoxEngine1SelectedIndexChanged(object sender, EventArgs e)
        {
            _engineList.SelectedIndex = _comboBoxEngine1.SelectedIndex;
            EngineSelected?.Invoke();
        }

        private void OnComboBoxEngine2SelectedIndexChanged(object sender, EventArgs e)
        {
            _engineList.SelectedOpponentIndex = _comboBoxEngine2.SelectedIndex;
            EngineSelected?.Invoke();
        }

        private void OnButtonPathDialogClick(object sender, EventArgs e)
        {
            string initialDirectory = null;

            for (int i = 0; i < SerializedInfo.Instance.EngineList.LastPaths.Count; i++)
            {
                string path = SerializedInfo.Instance.EngineList.LastPaths[i];

                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    initialDirectory = fileInfo.Directory.FullName;
                    break;
                }
                else if (Directory.Exists(path))
                {
                    initialDirectory = path;
                    break;
                }
            }

            OpenFileDialog fileDialog = new OpenFileDialog();

            if (initialDirectory != null)
            {
                fileDialog.InitialDirectory = initialDirectory;
            }

            fileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _comboBoxPath.Text = fileDialog.FileName;
            }
        }

        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Checked && radioButton.Tag != null)
            {
                SetUIToEngineMode((EngineMode)(int)radioButton.Tag);

                SerializedInfo.Instance.EngineMode = (EngineMode)(int)radioButton.Tag;
                EngineSelected?.Invoke();
            }
        }

        #endregion

        #region Other Methods

        private void InitializeRadioButtons()
        {
            _radioButtonDisabled.Tag = 0;
            _radioButtonAnalysis.Tag = 1;
            _radioButtonCompetitive.Tag = 2;

            switch (SerializedInfo.Instance.EngineMode)
            {
                case EngineMode.Disabled:
                    _radioButtonDisabled.Checked = true;
                    break;
                case EngineMode.Analysis:
                    _radioButtonAnalysis.Checked = true;
                    break;
                case EngineMode.Competitive:
                    _radioButtonCompetitive.Checked = true;
                    break;
            }
        }

        private void OpenEngineConfig(int index)
        {
            EngineSetting setting = _engineList.Settings[index];

            _textBoxName.Text = setting.Name;
            _textBoxArguments.Text = string.Join(Environment.NewLine, setting.Arguments);
            _comboBoxPath.Text = setting.ExecutablePath;
        }

        private void UpdateUI(int initialSelect)
        {
            _listBoxSettings.Items.Clear();
            _comboBoxEngine1.Items.Clear();
            _comboBoxEngine2.Items.Clear();
            _comboBoxPath.Items.Clear();

            for (int i = 0; i < _engineList.LastPaths.Count; i++)
            {
                _comboBoxPath.Items.Insert(0, _engineList.LastPaths[i]);
            }

            for (int i = 0; i < _engineList.Settings.Count; i++)
            {
                _listBoxSettings.Items.Add(_engineList.Settings[i].Name);

                _comboBoxEngine1.Items.Add(_engineList.Settings[i].Name);
                _comboBoxEngine2.Items.Add(_engineList.Settings[i].Name);
            }

            if (initialSelect != -1 && _listBoxSettings.Items.Count > 0)
            {
                if (initialSelect >= _listBoxSettings.Items.Count)
                {
                    initialSelect = _listBoxSettings.Items.Count - 1;
                }

                _listBoxSettings.SelectedIndex = initialSelect;
                OpenEngineConfig(initialSelect);
            }

            TrySelect(_comboBoxEngine1, _engineList.SelectedIndex);
            TrySelect(_comboBoxEngine2, _engineList.SelectedOpponentIndex);
        }

        private void SetUIToEngineMode(EngineMode mode)
        {
            switch (mode)
            {
                case EngineMode.Disabled:

                    _comboBoxEngine1.Visible = false;
                    _comboBoxEngine2.Visible = false;
                    _labelEngine1.Visible = false;
                    _labelEngine2.Visible = false;

                    break;

                case EngineMode.Analysis:

                    _comboBoxEngine1.Visible = true;
                    _labelEngine1.Text = "Analysis Engine";
                    _labelEngine1.Visible = true;

                    _comboBoxEngine2.Visible = false;
                    _labelEngine2.Visible = false;

                    break;

                case EngineMode.Competitive:

                    _comboBoxEngine1.Visible = true;
                    _labelEngine1.Text = "Engine 1";
                    _labelEngine1.Visible = true;

                    _comboBoxEngine2.Visible = true;
                    _labelEngine2.Visible = true;

                    break;
            }
        }

        private void TrySelect(ComboBox comboBox, int selectedIndex)
        {
            if (comboBox.Items.Count > 0)
            {
                if (selectedIndex < comboBox.Items.Count && selectedIndex >= 0)
                {
                    comboBox.SelectedIndex = selectedIndex;
                }
                else
                {
                    comboBox.SelectedIndex = 0;
                }
            }
        }

        #endregion
    }
}
