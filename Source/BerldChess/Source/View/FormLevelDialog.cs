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

            _textBoxPlies.Text = _level.Plies.ToString();
            _textBoxTimePerMove.Text = TimeSpan.FromMilliseconds(_level.TimePerMove).TotalSeconds.ToString();
            _textBoxTotalTimeMinutes.Text = totalTime.TotalMinutes.ToString();
            _textBoxTotalTimeSeconds.Text = totalTime.Seconds.ToString();
            _textBoxIncrement.Text = TimeSpan.FromMilliseconds(_level.Increment).TotalSeconds.ToString();
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
            int plies;
            double timePerMove;
            int totalMinutes;
            int totalSeconds;
            double increment;

            if (int.TryParse(_textBoxPlies.Text, out plies))
            {
                _level.Plies = plies;
            }

            if(double.TryParse(_textBoxTimePerMove.Text, out timePerMove))
            {
                _level.TimePerMove = (int)TimeSpan.FromSeconds(timePerMove).TotalMilliseconds;
            }

            if(int.TryParse(_textBoxTotalTimeMinutes.Text, out totalMinutes) &&
                int.TryParse(_textBoxTotalTimeSeconds.Text, out totalSeconds))
            {
                TimeSpan totalTime = new TimeSpan(0, totalMinutes, totalSeconds);
                _level.TotalTime = (int)totalTime.TotalMilliseconds;
            }

            if(double.TryParse(_textBoxIncrement.Text, out increment))
            {
                _level.Increment = (int)TimeSpan.FromSeconds(increment).TotalMilliseconds;
            }

            _level.Nodes = (int)_numericNodes.Value;
            _level.SelectedLevelType = _selectedLevelType;

            DialogResult = DialogResult.OK;
        }
    }
}
