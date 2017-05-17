using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BerldChess.View
{
    [System.ComponentModel.DesignerCategory("")]
    internal partial class FormProgressDialog : Form
    {
        #region Fields

        private IContainer components = null;
        private ProgressBar _progressBar;
        private Label _labelStatus;

        #endregion

        #region Properties

        public Label StatusLabel
        {
            get
            {
                return _labelStatus;
            }
        }

        public ProgressBar ProgressBar
        {
            get
            {
                return _progressBar;
            }
        }

        #endregion

        #region Constructors

        internal FormProgressDialog()
        {
            _progressBar = new ProgressBar();
            _labelStatus = new Label();
            SuspendLayout();

            _progressBar.Location = new Point(12, 39);
            _progressBar.MarqueeAnimationSpeed = 50;
            _progressBar.Size = new Size(335, 25);
            _progressBar.Step = 1;

            _labelStatus.Location = new Point(12, 9);
            _labelStatus.Size = new Size(335, 27);
            _labelStatus.Text = "Processing...";
            _labelStatus.TextAlign = ContentAlignment.MiddleLeft;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(359, 76);
            Controls.Add(_labelStatus);
            Controls.Add(_progressBar);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MaximumSize = new Size(375, 115);
            MinimizeBox = false;
            MinimumSize = new Size(375, 115);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Progress";
            ResumeLayout(false);
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
