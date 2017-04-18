using BerldChess.Model;
using Microsoft.VisualBasic;
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
    public partial class FormEngineSettings : Form
    {
        private EngineList _engineList;

        public event Action EngineSelected;

        public FormEngineSettings(EngineList engineList)
        {
            _engineList = engineList;

            InitializeComponent();
            _listBoxSettings.SetDoubleBuffered();
            UpdateList(_engineList.SelectedIndex);

            for (int i = engineList.LastPaths.Count - 1; i >= 0; i--)
            {
                _comboBoxPath.Items.Add(engineList.LastPaths[i]);
            }
        }

        private void UpdateList(int initialSelect)
        {
            _listBoxSettings.Items.Clear();

            for (int i = 0; i < _engineList.Settings.Count; i++)
            {
                _listBoxSettings.Items.Add(_engineList.Settings[i].Name);
            }

            if (initialSelect != -1)
            {
                _listBoxSettings.SelectedIndex = initialSelect;
                OpenEngineConfig(initialSelect);
            }
        }

        private void OpenEngineConfig(int index)
        {
            EngineSetting setting = _engineList.Settings[index];

            _textBoxName.Text = setting.Name;
            _textBoxArguments.Text = string.Join("\n", setting.Arguments);
            _comboBoxPath.Text = setting.ExecutablePath;

            _engineList.SelectedIndex = index;
            EngineSelected?.Invoke();
        }

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            string name = _textBoxName.Text;
            EngineSetting setting = _engineList.SelectedSetting;

            if(setting == null)
            {
                return;
            }

            bool nameChanged = name != setting.Name;

            if(nameChanged && _listBoxSettings.Items.Contains(name))
            {
                MessageBox.Show(this, "Setting with this name already exists.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(nameChanged)
            {
                setting.Name = name;
                UpdateList(_listBoxSettings.SelectedIndex);
            }

            setting.ExecutablePath = _comboBoxPath.Text;
            setting.Arguments = _textBoxArguments.Text.Split('\n');

            EngineSelected?.Invoke();
        }

        private void OnButtonAddNewClick(object sender, EventArgs e)
        {
            EngineSetting setting = new EngineSetting();
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

            setting.Name = name;
            setting.ExecutablePath = _comboBoxPath.Text;
            setting.Arguments = _textBoxArguments.Text.Split('\n');

            _engineList.Settings.Add(setting);
            _engineList.SelectedIndex = _engineList.Settings.Count - 1;

            _listBoxSettings.Items.Add(setting.Name);
            _listBoxSettings.SelectedIndex = _listBoxSettings.Items.Count - 1;

            EngineSelected?.Invoke();
        }

        private void OnButtonRemoveClick(object sender, EventArgs e)
        {
            int selected = _listBoxSettings.SelectedIndex;

            _engineList.Settings.RemoveAt(selected);

            _listBoxSettings.Items.RemoveAt(selected);
            _listBoxSettings.SelectedIndex = selected - 1;
        }

        private void OnListBoxSettingsSelectedIndexChanged(object sender, EventArgs e)
        {
            OpenEngineConfig(_listBoxSettings.SelectedIndex);
        }
    }
}
