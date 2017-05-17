using BerldChess.Model;
using BerldChess.Properties;
using ilf.pgn;
using ilf.pgn.Data;
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
    public partial class FormPgnLoader : Form
    {
        public Game PgnLoadedGame { get; set; } = null;
        public bool Analysis { get; set; }
        public int Depth { get; set; }

        public FormPgnLoader(bool analysis, int depth)
        {
            InitializeComponent();
            InitializeWindow();

            _checkBoxAnalysis.Checked = analysis;
            _textBoxDepth.Enabled = analysis;

            if(analysis)
            {
                _textBoxDepth.Text = depth.ToString();
            }
        }

        private void InitializeWindow()
        {
            Icon = Resources.PawnRush;
        }

        private void OnButtonLoadClick(object sender, EventArgs e)
        {
            try
            {
                PgnReader reader = new PgnReader();
                Database database = reader.ReadFromString(_textBoxPgnInput.Text);

                if (database.Games.Count > 0)
                {
                    Game firstGame = database.Games[0];
                    PgnLoadedGame = firstGame;

                    Analysis = _checkBoxAnalysis.Checked;

                    int parseDepth;
                    bool parsed = int.TryParse(_textBoxDepth.Text, out parseDepth);

                    if (parsed)
                    {
                        Depth = Math.Abs(parseDepth);
                    }
                    else
                    {
                        Analysis = false;
                    }

                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("No valid game found.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with reading input.\n\n" + ex.Message, "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnButtonChooseFileClick(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = SerializedInfo.Instance.LastPgnDir;
            fileDialog.Filter = "pgn files (*.pgn)|*.pgn";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SerializedInfo.Instance.LastPgnDir = fileDialog.FileName;

                try
                {
                    PgnReader reader = new PgnReader();
                    Database database = reader.ReadFromFile(fileDialog.FileName);

                    if (database.Games.Count > 0)
                    {
                        Game firstGame = database.Games[0];
                        PgnLoadedGame = firstGame;

                        Analysis = _checkBoxAnalysis.Checked;

                        int parseDepth;
                        bool parsed = int.TryParse(_textBoxDepth.Text, out parseDepth);

                        if (parsed)
                        {
                            Depth = Math.Abs(parseDepth);
                        }
                        else
                        {
                            Analysis = false;
                        }

                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("No valid game found.", "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error with reading input.\n\n" + ex.Message, "BerldChess", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnButtonCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OnCheckBoxAnalysisCheckedChanged(object sender, EventArgs e)
        {
            _textBoxDepth.Enabled = _checkBoxAnalysis.Checked;
        }
    }
}
