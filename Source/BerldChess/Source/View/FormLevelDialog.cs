using BerldChess.Model;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace BerldChess.View
{
    public partial class FormLevelDialog : Form
    {
        public Level Level { get; }

        private LevelType _selectedLevelType = LevelType.Infinite;
        private readonly Panel[] _panels;

        public FormLevelDialog(Level initialLevel)
        {
            Level = initialLevel;

            InitializeComponent();

            var radioButtons = new[]
            {
                _radioButtonFixedDepth,
                _radioButtonTimePerMove,
                _radioButtonTotalTime,
                _radioButtonInfinite,
                _radioButtonNodes
            };

            _panels = new[]
            {
                _panelFixedDepth,
                _panelTimePerMove,
                _panelTotalTime,
                null,
                _panelNodes,
            };

            for (var i = 0; i < radioButtons.Length; i++)
            {
                radioButtons[i].Tag = i;

                if (_panels[i] != null)
                {
                    _panels[i].Tag = i;
                }
            }

            var totalTime = TimeSpan.FromMilliseconds(Level.TotalTime);

            _textBoxPlies.Text = Level.Plies.ToString();
            _textBoxTimePerMove.Text = TimeSpan.FromMilliseconds(Level.TimePerMove).TotalSeconds.ToString(CultureInfo.CurrentCulture);
            _textBoxTotalTimeMinutes.Text = Math.Floor(totalTime.TotalMinutes).ToString(CultureInfo.CurrentCulture);
            _textBoxTotalTimeSeconds.Text = totalTime.Seconds.ToString();
            _textBoxIncrement.Text = TimeSpan.FromMilliseconds(Level.Increment).TotalSeconds.ToString(CultureInfo.CurrentCulture);
            _numericNodes.Value = Level.Nodes;

            radioButtons[(int)Level.SelectedLevelType].Checked = true;
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
                Level.Plies = plies;
            }

            if(double.TryParse(_textBoxTimePerMove.Text, out timePerMove))
            {
                Level.TimePerMove = (int)TimeSpan.FromSeconds(timePerMove).TotalMilliseconds;
            }

            if(int.TryParse(_textBoxTotalTimeMinutes.Text, out totalMinutes) &&
                int.TryParse(_textBoxTotalTimeSeconds.Text, out totalSeconds))
            {
                TimeSpan totalTime = new TimeSpan(0, totalMinutes, totalSeconds);
                Level.TotalTime = (int)totalTime.TotalMilliseconds;
            }

            if(double.TryParse(_textBoxIncrement.Text, out increment))
            {
                Level.Increment = (int)TimeSpan.FromSeconds(increment).TotalMilliseconds;
            }

            Level.Nodes = (int)_numericNodes.Value;
            Level.SelectedLevelType = _selectedLevelType;

            DialogResult = DialogResult.OK;
        }
    }
}
