namespace BerldChess.View
{
    partial class FormLevelDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLevelDialog));
            this._groupBoxMode = new System.Windows.Forms.GroupBox();
            this._radioButtonNodes = new System.Windows.Forms.RadioButton();
            this._radioButtonInfinite = new System.Windows.Forms.RadioButton();
            this._radioButtonTotalTime = new System.Windows.Forms.RadioButton();
            this._radioButtonTimePerMove = new System.Windows.Forms.RadioButton();
            this._radioButtonFixedDepth = new System.Windows.Forms.RadioButton();
            this._groupBoxModeSetting = new System.Windows.Forms.GroupBox();
            this._panelFixedDepth = new System.Windows.Forms.Panel();
            this._textBoxPlies = new System.Windows.Forms.TextBox();
            this._labelPlies = new BerldChess.View.SmoothLabel();
            this._panelTotalTime = new System.Windows.Forms.Panel();
            this._textBoxIncrement = new System.Windows.Forms.TextBox();
            this._textBoxTotalTimeSeconds = new System.Windows.Forms.TextBox();
            this._textBoxTotalTimeMinutes = new System.Windows.Forms.TextBox();
            this._labelIncrementSeconds = new BerldChess.View.SmoothLabel();
            this._labelIncrement = new BerldChess.View.SmoothLabel();
            this._labelTotalTimeSeconds = new BerldChess.View.SmoothLabel();
            this._labelTotalTimeMinutes = new BerldChess.View.SmoothLabel();
            this._labelTotalTime = new BerldChess.View.SmoothLabel();
            this._panelNodes = new System.Windows.Forms.Panel();
            this._numericNodes = new System.Windows.Forms.NumericUpDown();
            this._labelNodes = new BerldChess.View.SmoothLabel();
            this._panelTimePerMove = new System.Windows.Forms.Panel();
            this._textBoxTimePerMove = new System.Windows.Forms.TextBox();
            this._labelTimePerMoveSeconds = new BerldChess.View.SmoothLabel();
            this._labelTimePerMove = new BerldChess.View.SmoothLabel();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._buttonApply = new System.Windows.Forms.Button();
            this._groupBoxMode.SuspendLayout();
            this._groupBoxModeSetting.SuspendLayout();
            this._panelFixedDepth.SuspendLayout();
            this._panelTotalTime.SuspendLayout();
            this._panelNodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numericNodes)).BeginInit();
            this._panelTimePerMove.SuspendLayout();
            this.SuspendLayout();
            // 
            // _groupBoxMode
            // 
            this._groupBoxMode.Controls.Add(this._radioButtonNodes);
            this._groupBoxMode.Controls.Add(this._radioButtonInfinite);
            this._groupBoxMode.Controls.Add(this._radioButtonTotalTime);
            this._groupBoxMode.Controls.Add(this._radioButtonTimePerMove);
            this._groupBoxMode.Controls.Add(this._radioButtonFixedDepth);
            this._groupBoxMode.Location = new System.Drawing.Point(13, 14);
            this._groupBoxMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._groupBoxMode.Name = "_groupBoxMode";
            this._groupBoxMode.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._groupBoxMode.Size = new System.Drawing.Size(224, 276);
            this._groupBoxMode.TabIndex = 0;
            this._groupBoxMode.TabStop = false;
            this._groupBoxMode.Text = "Mode";
            // 
            // _radioButtonNodes
            // 
            this._radioButtonNodes.AutoSize = true;
            this._radioButtonNodes.Location = new System.Drawing.Point(22, 160);
            this._radioButtonNodes.Name = "_radioButtonNodes";
            this._radioButtonNodes.Size = new System.Drawing.Size(73, 25);
            this._radioButtonNodes.TabIndex = 4;
            this._radioButtonNodes.TabStop = true;
            this._radioButtonNodes.Text = "Nodes";
            this._radioButtonNodes.UseVisualStyleBackColor = true;
            this._radioButtonNodes.CheckedChanged += new System.EventHandler(this.RadioButtonLevelsCheckedChanged);
            // 
            // _radioButtonInfinite
            // 
            this._radioButtonInfinite.AutoSize = true;
            this._radioButtonInfinite.Location = new System.Drawing.Point(22, 129);
            this._radioButtonInfinite.Name = "_radioButtonInfinite";
            this._radioButtonInfinite.Size = new System.Drawing.Size(76, 25);
            this._radioButtonInfinite.TabIndex = 3;
            this._radioButtonInfinite.TabStop = true;
            this._radioButtonInfinite.Text = "Infinite";
            this._radioButtonInfinite.UseVisualStyleBackColor = true;
            this._radioButtonInfinite.CheckedChanged += new System.EventHandler(this.RadioButtonLevelsCheckedChanged);
            // 
            // _radioButtonTotalTime
            // 
            this._radioButtonTotalTime.AutoSize = true;
            this._radioButtonTotalTime.Location = new System.Drawing.Point(22, 98);
            this._radioButtonTotalTime.Name = "_radioButtonTotalTime";
            this._radioButtonTotalTime.Size = new System.Drawing.Size(97, 25);
            this._radioButtonTotalTime.TabIndex = 2;
            this._radioButtonTotalTime.TabStop = true;
            this._radioButtonTotalTime.Text = "Total time";
            this._radioButtonTotalTime.UseVisualStyleBackColor = true;
            this._radioButtonTotalTime.CheckedChanged += new System.EventHandler(this.RadioButtonLevelsCheckedChanged);
            // 
            // _radioButtonTimePerMove
            // 
            this._radioButtonTimePerMove.AutoSize = true;
            this._radioButtonTimePerMove.Location = new System.Drawing.Point(22, 67);
            this._radioButtonTimePerMove.Name = "_radioButtonTimePerMove";
            this._radioButtonTimePerMove.Size = new System.Drawing.Size(132, 25);
            this._radioButtonTimePerMove.TabIndex = 1;
            this._radioButtonTimePerMove.TabStop = true;
            this._radioButtonTimePerMove.Text = "Time per move";
            this._radioButtonTimePerMove.UseVisualStyleBackColor = true;
            this._radioButtonTimePerMove.CheckedChanged += new System.EventHandler(this.RadioButtonLevelsCheckedChanged);
            // 
            // _radioButtonFixedDepth
            // 
            this._radioButtonFixedDepth.AutoSize = true;
            this._radioButtonFixedDepth.Location = new System.Drawing.Point(22, 36);
            this._radioButtonFixedDepth.Name = "_radioButtonFixedDepth";
            this._radioButtonFixedDepth.Size = new System.Drawing.Size(157, 25);
            this._radioButtonFixedDepth.TabIndex = 0;
            this._radioButtonFixedDepth.TabStop = true;
            this._radioButtonFixedDepth.Text = "Fixed search depth";
            this._radioButtonFixedDepth.UseVisualStyleBackColor = true;
            this._radioButtonFixedDepth.CheckedChanged += new System.EventHandler(this.RadioButtonLevelsCheckedChanged);
            // 
            // _groupBoxModeSetting
            // 
            this._groupBoxModeSetting.Controls.Add(this._panelNodes);
            this._groupBoxModeSetting.Controls.Add(this._panelTimePerMove);
            this._groupBoxModeSetting.Controls.Add(this._panelFixedDepth);
            this._groupBoxModeSetting.Controls.Add(this._panelTotalTime);
            this._groupBoxModeSetting.Location = new System.Drawing.Point(263, 14);
            this._groupBoxModeSetting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._groupBoxModeSetting.Name = "_groupBoxModeSetting";
            this._groupBoxModeSetting.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._groupBoxModeSetting.Size = new System.Drawing.Size(275, 276);
            this._groupBoxModeSetting.TabIndex = 1;
            this._groupBoxModeSetting.TabStop = false;
            this._groupBoxModeSetting.Text = "Mode Name";
            // 
            // _panelFixedDepth
            // 
            this._panelFixedDepth.Controls.Add(this._textBoxPlies);
            this._panelFixedDepth.Controls.Add(this._labelPlies);
            this._panelFixedDepth.Location = new System.Drawing.Point(7, 23);
            this._panelFixedDepth.Name = "_panelFixedDepth";
            this._panelFixedDepth.Size = new System.Drawing.Size(261, 245);
            this._panelFixedDepth.TabIndex = 5;
            // 
            // _textBoxPlies
            // 
            this._textBoxPlies.Location = new System.Drawing.Point(12, 43);
            this._textBoxPlies.Name = "_textBoxPlies";
            this._textBoxPlies.Size = new System.Drawing.Size(130, 29);
            this._textBoxPlies.TabIndex = 6;
            // 
            // _labelPlies
            // 
            this._labelPlies.AutoSize = true;
            this._labelPlies.Location = new System.Drawing.Point(8, 13);
            this._labelPlies.Name = "_labelPlies";
            this._labelPlies.Size = new System.Drawing.Size(134, 21);
            this._labelPlies.TabIndex = 1;
            this._labelPlies.Text = "Half moves (Plies)";
            // 
            // _panelTotalTime
            // 
            this._panelTotalTime.Controls.Add(this._textBoxIncrement);
            this._panelTotalTime.Controls.Add(this._textBoxTotalTimeSeconds);
            this._panelTotalTime.Controls.Add(this._textBoxTotalTimeMinutes);
            this._panelTotalTime.Controls.Add(this._labelIncrementSeconds);
            this._panelTotalTime.Controls.Add(this._labelIncrement);
            this._panelTotalTime.Controls.Add(this._labelTotalTimeSeconds);
            this._panelTotalTime.Controls.Add(this._labelTotalTimeMinutes);
            this._panelTotalTime.Controls.Add(this._labelTotalTime);
            this._panelTotalTime.Location = new System.Drawing.Point(7, 23);
            this._panelTotalTime.Name = "_panelTotalTime";
            this._panelTotalTime.Size = new System.Drawing.Size(261, 245);
            this._panelTotalTime.TabIndex = 7;
            // 
            // _textBoxIncrement
            // 
            this._textBoxIncrement.Location = new System.Drawing.Point(12, 162);
            this._textBoxIncrement.Name = "_textBoxIncrement";
            this._textBoxIncrement.Size = new System.Drawing.Size(130, 29);
            this._textBoxIncrement.TabIndex = 15;
            // 
            // _textBoxTotalTimeSeconds
            // 
            this._textBoxTotalTimeSeconds.Location = new System.Drawing.Point(12, 81);
            this._textBoxTotalTimeSeconds.Name = "_textBoxTotalTimeSeconds";
            this._textBoxTotalTimeSeconds.Size = new System.Drawing.Size(130, 29);
            this._textBoxTotalTimeSeconds.TabIndex = 14;
            // 
            // _textBoxTotalTimeMinutes
            // 
            this._textBoxTotalTimeMinutes.Location = new System.Drawing.Point(12, 39);
            this._textBoxTotalTimeMinutes.Name = "_textBoxTotalTimeMinutes";
            this._textBoxTotalTimeMinutes.Size = new System.Drawing.Size(130, 29);
            this._textBoxTotalTimeMinutes.TabIndex = 13;
            // 
            // _labelIncrementSeconds
            // 
            this._labelIncrementSeconds.AutoSize = true;
            this._labelIncrementSeconds.Location = new System.Drawing.Point(148, 165);
            this._labelIncrementSeconds.Name = "_labelIncrementSeconds";
            this._labelIncrementSeconds.Size = new System.Drawing.Size(66, 21);
            this._labelIncrementSeconds.TabIndex = 12;
            this._labelIncrementSeconds.Text = "seconds";
            // 
            // _labelIncrement
            // 
            this._labelIncrement.AutoSize = true;
            this._labelIncrement.Location = new System.Drawing.Point(8, 132);
            this._labelIncrement.Name = "_labelIncrement";
            this._labelIncrement.Size = new System.Drawing.Size(80, 21);
            this._labelIncrement.TabIndex = 10;
            this._labelIncrement.Text = "Increment";
            // 
            // _labelTotalTimeSeconds
            // 
            this._labelTotalTimeSeconds.AutoSize = true;
            this._labelTotalTimeSeconds.Location = new System.Drawing.Point(148, 84);
            this._labelTotalTimeSeconds.Name = "_labelTotalTimeSeconds";
            this._labelTotalTimeSeconds.Size = new System.Drawing.Size(66, 21);
            this._labelTotalTimeSeconds.TabIndex = 9;
            this._labelTotalTimeSeconds.Text = "seconds";
            // 
            // _labelTotalTimeMinutes
            // 
            this._labelTotalTimeMinutes.AutoSize = true;
            this._labelTotalTimeMinutes.Location = new System.Drawing.Point(148, 46);
            this._labelTotalTimeMinutes.Name = "_labelTotalTimeMinutes";
            this._labelTotalTimeMinutes.Size = new System.Drawing.Size(66, 21);
            this._labelTotalTimeMinutes.TabIndex = 6;
            this._labelTotalTimeMinutes.Text = "minutes";
            // 
            // _labelTotalTime
            // 
            this._labelTotalTime.AutoSize = true;
            this._labelTotalTime.Location = new System.Drawing.Point(8, 13);
            this._labelTotalTime.Name = "_labelTotalTime";
            this._labelTotalTime.Size = new System.Drawing.Size(82, 21);
            this._labelTotalTime.TabIndex = 1;
            this._labelTotalTime.Text = "Total Time";
            // 
            // _panelNodes
            // 
            this._panelNodes.Controls.Add(this._numericNodes);
            this._panelNodes.Controls.Add(this._labelNodes);
            this._panelNodes.Location = new System.Drawing.Point(7, 23);
            this._panelNodes.Name = "_panelNodes";
            this._panelNodes.Size = new System.Drawing.Size(261, 245);
            this._panelNodes.TabIndex = 8;
            // 
            // _numericNodes
            // 
            this._numericNodes.Increment = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this._numericNodes.Location = new System.Drawing.Point(12, 40);
            this._numericNodes.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this._numericNodes.Name = "_numericNodes";
            this._numericNodes.Size = new System.Drawing.Size(184, 29);
            this._numericNodes.TabIndex = 5;
            this._numericNodes.ThousandsSeparator = true;
            // 
            // _labelNodes
            // 
            this._labelNodes.AutoSize = true;
            this._labelNodes.Location = new System.Drawing.Point(8, 13);
            this._labelNodes.Name = "_labelNodes";
            this._labelNodes.Size = new System.Drawing.Size(55, 21);
            this._labelNodes.TabIndex = 1;
            this._labelNodes.Text = "Nodes";
            // 
            // _panelTimePerMove
            // 
            this._panelTimePerMove.Controls.Add(this._textBoxTimePerMove);
            this._panelTimePerMove.Controls.Add(this._labelTimePerMoveSeconds);
            this._panelTimePerMove.Controls.Add(this._labelTimePerMove);
            this._panelTimePerMove.Location = new System.Drawing.Point(7, 23);
            this._panelTimePerMove.Name = "_panelTimePerMove";
            this._panelTimePerMove.Size = new System.Drawing.Size(261, 245);
            this._panelTimePerMove.TabIndex = 6;
            // 
            // _textBoxTimePerMove
            // 
            this._textBoxTimePerMove.Location = new System.Drawing.Point(12, 42);
            this._textBoxTimePerMove.Name = "_textBoxTimePerMove";
            this._textBoxTimePerMove.Size = new System.Drawing.Size(130, 29);
            this._textBoxTimePerMove.TabIndex = 5;
            // 
            // _labelTimePerMoveSeconds
            // 
            this._labelTimePerMoveSeconds.AutoSize = true;
            this._labelTimePerMoveSeconds.Location = new System.Drawing.Point(148, 45);
            this._labelTimePerMoveSeconds.Name = "_labelTimePerMoveSeconds";
            this._labelTimePerMoveSeconds.Size = new System.Drawing.Size(66, 21);
            this._labelTimePerMoveSeconds.TabIndex = 4;
            this._labelTimePerMoveSeconds.Text = "seconds";
            // 
            // _labelTimePerMove
            // 
            this._labelTimePerMove.AutoSize = true;
            this._labelTimePerMove.Location = new System.Drawing.Point(8, 13);
            this._labelTimePerMove.Name = "_labelTimePerMove";
            this._labelTimePerMove.Size = new System.Drawing.Size(84, 21);
            this._labelTimePerMove.TabIndex = 3;
            this._labelTimePerMove.Text = "Move time";
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Location = new System.Drawing.Point(163, 312);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(134, 30);
            this._buttonCancel.TabIndex = 3;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this.OnButtonCancelClick);
            // 
            // _buttonApply
            // 
            this._buttonApply.Location = new System.Drawing.Point(13, 312);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(135, 30);
            this._buttonApply.TabIndex = 4;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // FormLevelDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 354);
            this.Controls.Add(this._buttonApply);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._groupBoxModeSetting);
            this.Controls.Add(this._groupBoxMode);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "FormLevelDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Adjust Level";
            this._groupBoxMode.ResumeLayout(false);
            this._groupBoxMode.PerformLayout();
            this._groupBoxModeSetting.ResumeLayout(false);
            this._panelFixedDepth.ResumeLayout(false);
            this._panelFixedDepth.PerformLayout();
            this._panelTotalTime.ResumeLayout(false);
            this._panelTotalTime.PerformLayout();
            this._panelNodes.ResumeLayout(false);
            this._panelNodes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numericNodes)).EndInit();
            this._panelTimePerMove.ResumeLayout(false);
            this._panelTimePerMove.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _groupBoxMode;
        private System.Windows.Forms.RadioButton _radioButtonNodes;
        private System.Windows.Forms.RadioButton _radioButtonInfinite;
        private System.Windows.Forms.RadioButton _radioButtonTotalTime;
        private System.Windows.Forms.RadioButton _radioButtonTimePerMove;
        private System.Windows.Forms.RadioButton _radioButtonFixedDepth;
        private System.Windows.Forms.GroupBox _groupBoxModeSetting;
        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.Button _buttonApply;
        private System.Windows.Forms.Panel _panelFixedDepth;
        private System.Windows.Forms.Panel _panelTimePerMove;
        private BerldChess.View.SmoothLabel _labelTimePerMoveSeconds;
        private BerldChess.View.SmoothLabel _labelTimePerMove;
        private BerldChess.View.SmoothLabel _labelPlies;
        private System.Windows.Forms.Panel _panelTotalTime;
        private BerldChess.View.SmoothLabel _labelIncrementSeconds;
        private BerldChess.View.SmoothLabel _labelIncrement;
        private BerldChess.View.SmoothLabel _labelTotalTimeSeconds;
        private BerldChess.View.SmoothLabel _labelTotalTimeMinutes;
        private BerldChess.View.SmoothLabel _labelTotalTime;
        private System.Windows.Forms.Panel _panelNodes;
        private System.Windows.Forms.NumericUpDown _numericNodes;
        private BerldChess.View.SmoothLabel _labelNodes;
        private System.Windows.Forms.TextBox _textBoxIncrement;
        private System.Windows.Forms.TextBox _textBoxTotalTimeSeconds;
        private System.Windows.Forms.TextBox _textBoxTotalTimeMinutes;
        private System.Windows.Forms.TextBox _textBoxTimePerMove;
        private System.Windows.Forms.TextBox _textBoxPlies;
    }
}