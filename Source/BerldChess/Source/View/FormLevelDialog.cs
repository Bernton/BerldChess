using BerldChess.Model;
using System;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormLevelDialog : Form
    {
        public Level Level
        {
            get
            {
                return _level;
            }
        }

        private RadioButton[] _radioButtons;
        private Panel[] _panels;
        private Level _level;
        private LevelType _selectedLevelType = LevelType.Infinite;

        public FormLevelDialog(Level initialLevel)
        {
            _level = initialLevel;

            InitializeComponent();
            _radioButtons = new RadioButton[]
            {
                _radioButtonFixedDepth,
                _radioButtonTimePerMove,
                _radioButtonTotalTime,
                _radioButtonInfinite,
                _radioButtonNodes
            };

            _panels = new Panel[]
            {
                _panelFixedDepth,
                _panelTimePerMove,
                _panelTotalTime,
                null,
                _panelNodes,
            };

            for (int i = 0; i < _radioButtons.Length; i++)
            {
                _radioButtons[i].Tag = i;

                if (_panels[i] != null)
                {
                    _panels[i].Tag = i;
                }
            }

            TimeSpan totalTime = TimeSpan.FromMilliseconds(_level.TotalTime);

            _numericPlies.Value = _level.Plies;
            _numericTimePerMove.Value = (decimal)TimeSpan.FromMilliseconds(_level.TimePerMove).TotalSeconds;
            _numericTotalTimeMinutes.Value = (int)totalTime.TotalMinutes;
            _numericTotalTimeSeconds.Value = totalTime.Seconds;
            _numericIncrement.Value = (decimal)TimeSpan.FromMilliseconds(_level.Increment).TotalSeconds;
            _numericNodes.Value = _level.Nodes;

            _radioButtons[(int)_level.SelectedLevelType].Checked = true;
        }

        private void HidePanels()
        {
            for (int i = 0; i < _panels.Length; i++)
            {
                if (_panels[i] != null)
                {
                    _panels[i].Visible = false;
                }
            }
        }

        private void RadioButtonLevelsCheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Checked)
            {
                HidePanels();
                _groupBoxModeSetting.Text = radioButton.Text;
                _selectedLevelType = (LevelType)(int)radioButton.Tag;

                if (_panels[(int)radioButton.Tag] == null)
                {
                    return;
                }

                _panels[(int)radioButton.Tag].Visible = true;
            }
        }

        private void OnButtonCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OnButtonApplyClick(object sender, EventArgs e)
        {
            _level.Plies = (int)_numericPlies.Value;
            _level.TimePerMove = (int)TimeSpan.FromSeconds((double)_numericTimePerMove.Value).TotalMilliseconds;
            TimeSpan totalTime = new TimeSpan(0, (int)_numericTotalTimeMinutes.Value, (int)_numericTotalTimeSeconds.Value);
            _level.TotalTime = (int)totalTime.TotalMilliseconds;
            _level.Increment = (int)TimeSpan.FromSeconds((double)_numericIncrement.Value).TotalMilliseconds;
            _level.Nodes = (int)_numericNodes.Value;
            _level.SelectedLevelType = _selectedLevelType;

            DialogResult = DialogResult.OK;
        }
    }
}
