using BerldChess.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BerldChess.View
{
    /// <summary>
    /// Compact always-visible scoreboard for competitive engine mode: names, running score,
    /// W/D/L, Elo difference with error margin and LOS, game number, and a strip of per-game
    /// result dots. Fed a <see cref="MatchStatistics"/> via <see cref="Update"/>.
    /// </summary>
    public sealed class CompetitiveScoreboard : UserControl
    {
        private readonly Label _labelNames = new Label();
        private readonly Label _labelScore = new Label();
        private readonly Label _labelWdl = new Label();
        private readonly Label _labelElo = new Label();
        private readonly Panel _dotsPanel = new Panel();

        private readonly SolidBrush _winBrush = new SolidBrush(Color.FromArgb(45, 164, 78));
        private readonly SolidBrush _drawBrush = new SolidBrush(Color.FromArgb(182, 188, 196));
        private readonly SolidBrush _lossBrush = new SolidBrush(Color.FromArgb(209, 65, 75));

        private IReadOnlyList<MatchOutcome> _results;

        public CompetitiveScoreboard()
        {
            // Auto-size to content so the control is exactly tall enough at any DPI (a fixed
            // height clips/overlaps the rows when fonts scale up).
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(10, 8, 10, 10);

            _labelNames.AutoSize = true;
            _labelNames.Margin = new Padding(0, 0, 0, 2);
            _labelNames.ForeColor = Color.Gray;
            _labelNames.Font = new Font("Segoe UI", 8.25F);

            _labelScore.AutoSize = true;
            _labelScore.Margin = new Padding(0);
            _labelScore.Font = new Font("Segoe UI", 15F, FontStyle.Bold);

            _labelWdl.AutoSize = true;
            _labelWdl.Margin = new Padding(0, 2, 0, 0);
            _labelWdl.Font = new Font("Segoe UI", 9F);

            _labelElo.AutoSize = true;
            _labelElo.Margin = new Padding(0, 1, 0, 0);
            _labelElo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            _dotsPanel.Dock = DockStyle.Fill;
            _dotsPanel.Height = 28;
            _dotsPanel.Margin = new Padding(0, 6, 0, 0);
            _dotsPanel.Paint += OnDotsPaint;

            var table = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 5
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            table.Controls.Add(_labelNames, 0, 0);
            table.Controls.Add(_labelScore, 0, 1);
            table.Controls.Add(_labelWdl, 0, 2);
            table.Controls.Add(_labelElo, 0, 3);
            table.Controls.Add(_dotsPanel, 0, 4);

            Controls.Add(table);
        }

        /// <summary>Refreshes the scoreboard. Pass a null target for an endless match.</summary>
        public void Update(string engine1, string engine2, MatchStatistics stats, int? target)
        {
            _labelNames.Text = $"{engine1}  vs  {engine2}";
            _labelScore.Text = $"{FormatPoints(stats.Score)} – {FormatPoints(stats.Games - stats.Score)}";

            string game = target.HasValue ? $"Game {stats.Games} / {target.Value}" : $"Game {stats.Games}";
            _labelWdl.Text = $"W {stats.Wins}  ·  D {stats.Draws}  ·  L {stats.Losses}      {game}";

            _labelElo.Text = $"{FormatElo(stats)}   ·   LOS {stats.LikelihoodOfSuperiority * 100.0:0}%";

            _results = stats.Results;
            _dotsPanel.Invalidate();
        }

        private void OnDotsPaint(object sender, PaintEventArgs e)
        {
            if (_results == null || _results.Count == 0)
            {
                return;
            }

            const int size = 11;
            const int gap = 3;

            int perRow = Math.Max(1, (_dotsPanel.Width + gap) / (size + gap));
            int rows = Math.Max(1, (_dotsPanel.Height + gap) / (size + gap));
            int capacity = perRow * rows;

            // Show the most recent results if they don't all fit.
            int start = Math.Max(0, _results.Count - capacity);

            for (int i = start; i < _results.Count; i++)
            {
                int index = i - start;
                int x = index % perRow * (size + gap);
                int y = index / perRow * (size + gap);

                e.Graphics.FillRectangle(BrushFor(_results[i]), x, y, size, size);
            }
        }

        private SolidBrush BrushFor(MatchOutcome outcome)
        {
            switch (outcome)
            {
                case MatchOutcome.Win: return _winBrush;
                case MatchOutcome.Loss: return _lossBrush;
                default: return _drawBrush;
            }
        }

        private static string FormatPoints(double points)
        {
            int whole = (int)points;
            bool half = points - whole >= 0.5;

            if (half)
            {
                return whole == 0 ? "½" : $"{whole}½";
            }

            return whole.ToString();
        }

        private static string FormatElo(MatchStatistics stats)
        {
            double elo = stats.Elo;

            if (double.IsPositiveInfinity(elo))
            {
                return "+∞ Elo";
            }

            if (double.IsNegativeInfinity(elo))
            {
                return "−∞ Elo";
            }

            string value = elo.ToString("+0;−0;0");

            return stats.Games >= 2
                ? $"{value} ± {stats.EloErrorMargin:0} Elo"
                : $"{value} Elo";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _winBrush.Dispose();
                _drawBrush.Dispose();
                _lossBrush.Dispose();
                _labelNames.Font.Dispose();
                _labelScore.Font.Dispose();
                _labelWdl.Font.Dispose();
                _labelElo.Font.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
