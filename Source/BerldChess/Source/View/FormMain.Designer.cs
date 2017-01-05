namespace BerldChess.View
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this._splitContainerOuter = new System.Windows.Forms.SplitContainer();
            this._splitContainerInner = new System.Windows.Forms.SplitContainer();
            this._buttonNew = new System.Windows.Forms.Button();
            this._labelEvaluation = new System.Windows.Forms.Label();
            this._checkBoxLocalMode = new System.Windows.Forms.CheckBox();
            this._labelCPStatus = new System.Windows.Forms.Label();
            this._checkBoxHideOutput = new System.Windows.Forms.CheckBox();
            this._checkBoxHideArrows = new System.Windows.Forms.CheckBox();
            this._checkBoxFlipped = new System.Windows.Forms.CheckBox();
            this._checkBoxGridBorder = new System.Windows.Forms.CheckBox();
            this._buttonLatest = new System.Windows.Forms.Button();
            this._buttonStart = new System.Windows.Forms.Button();
            this._buttonForward = new System.Windows.Forms.Button();
            this._buttonBack = new System.Windows.Forms.Button();
            this._buttonLoadFen = new System.Windows.Forms.Button();
            this._labelFEN = new System.Windows.Forms.Label();
            this._textBoxFen = new System.Windows.Forms.TextBox();
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this._arrowInvalidateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerOuter)).BeginInit();
            this._splitContainerOuter.Panel1.SuspendLayout();
            this._splitContainerOuter.Panel2.SuspendLayout();
            this._splitContainerOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerInner)).BeginInit();
            this._splitContainerInner.Panel2.SuspendLayout();
            this._splitContainerInner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _splitContainerOuter
            // 
            this._splitContainerOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._splitContainerOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerOuter.ForeColor = System.Drawing.SystemColors.ControlText;
            this._splitContainerOuter.Location = new System.Drawing.Point(0, 0);
            this._splitContainerOuter.Name = "_splitContainerOuter";
            this._splitContainerOuter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainerOuter.Panel1
            // 
            this._splitContainerOuter.Panel1.Controls.Add(this._splitContainerInner);
            // 
            // _splitContainerOuter.Panel2
            // 
            this._splitContainerOuter.Panel2.Controls.Add(this._dataGridView);
            this._splitContainerOuter.Size = new System.Drawing.Size(836, 529);
            this._splitContainerOuter.SplitterDistance = 405;
            this._splitContainerOuter.SplitterWidth = 3;
            this._splitContainerOuter.TabIndex = 0;
            this._splitContainerOuter.TabStop = false;
            // 
            // _splitContainerInner
            // 
            this._splitContainerInner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._splitContainerInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerInner.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this._splitContainerInner.Location = new System.Drawing.Point(0, 0);
            this._splitContainerInner.Name = "_splitContainerInner";
            // 
            // _splitContainerInner.Panel1
            // 
            this._splitContainerInner.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // _splitContainerInner.Panel2
            // 
            this._splitContainerInner.Panel2.Controls.Add(this._buttonNew);
            this._splitContainerInner.Panel2.Controls.Add(this._labelEvaluation);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxLocalMode);
            this._splitContainerInner.Panel2.Controls.Add(this._labelCPStatus);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxHideOutput);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxHideArrows);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxFlipped);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxGridBorder);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonLatest);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonStart);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonForward);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonBack);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonLoadFen);
            this._splitContainerInner.Panel2.Controls.Add(this._labelFEN);
            this._splitContainerInner.Panel2.Controls.Add(this._textBoxFen);
            this._splitContainerInner.Panel2MinSize = 200;
            this._splitContainerInner.Size = new System.Drawing.Size(836, 405);
            this._splitContainerInner.SplitterDistance = 632;
            this._splitContainerInner.SplitterWidth = 3;
            this._splitContainerInner.TabIndex = 0;
            this._splitContainerInner.TabStop = false;
            // 
            // _buttonNew
            // 
            this._buttonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonNew.Location = new System.Drawing.Point(118, 66);
            this._buttonNew.Name = "_buttonNew";
            this._buttonNew.Size = new System.Drawing.Size(75, 23);
            this._buttonNew.TabIndex = 2;
            this._buttonNew.Text = "New";
            this._buttonNew.UseVisualStyleBackColor = true;
            this._buttonNew.Click += new System.EventHandler(this.OnButtonNewClick);
            // 
            // _labelEvaluation
            // 
            this._labelEvaluation.AutoSize = true;
            this._labelEvaluation.Location = new System.Drawing.Point(15, 228);
            this._labelEvaluation.Name = "_labelEvaluation";
            this._labelEvaluation.Size = new System.Drawing.Size(57, 13);
            this._labelEvaluation.TabIndex = 13;
            this._labelEvaluation.Text = "Evaluation";
            // 
            // _checkBoxLocalMode
            // 
            this._checkBoxLocalMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxLocalMode.AutoSize = true;
            this._checkBoxLocalMode.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxLocalMode.Location = new System.Drawing.Point(97, 326);
            this._checkBoxLocalMode.Name = "_checkBoxLocalMode";
            this._checkBoxLocalMode.Size = new System.Drawing.Size(81, 17);
            this._checkBoxLocalMode.TabIndex = 9;
            this._checkBoxLocalMode.Text = "Local mode";
            this._checkBoxLocalMode.UseVisualStyleBackColor = false;
            this._checkBoxLocalMode.CheckedChanged += new System.EventHandler(this.OnCheckBoxPlayModeCheckedChanged);
            // 
            // _labelCPStatus
            // 
            this._labelCPStatus.AutoSize = true;
            this._labelCPStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelCPStatus.Location = new System.Drawing.Point(12, 244);
            this._labelCPStatus.Name = "_labelCPStatus";
            this._labelCPStatus.Size = new System.Drawing.Size(69, 31);
            this._labelCPStatus.TabIndex = 11;
            this._labelCPStatus.Text = "cp 0";
            // 
            // _checkBoxHideOutput
            // 
            this._checkBoxHideOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxHideOutput.AutoSize = true;
            this._checkBoxHideOutput.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxHideOutput.Location = new System.Drawing.Point(97, 280);
            this._checkBoxHideOutput.Name = "_checkBoxHideOutput";
            this._checkBoxHideOutput.Size = new System.Drawing.Size(83, 17);
            this._checkBoxHideOutput.TabIndex = 7;
            this._checkBoxHideOutput.Text = "Hide Output";
            this._checkBoxHideOutput.UseVisualStyleBackColor = false;
            this._checkBoxHideOutput.CheckedChanged += new System.EventHandler(this.OnCheckBoxHideOutputCheckedChanged);
            // 
            // _checkBoxHideArrows
            // 
            this._checkBoxHideArrows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxHideArrows.AutoSize = true;
            this._checkBoxHideArrows.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxHideArrows.Location = new System.Drawing.Point(97, 303);
            this._checkBoxHideArrows.Name = "_checkBoxHideArrows";
            this._checkBoxHideArrows.Size = new System.Drawing.Size(83, 17);
            this._checkBoxHideArrows.TabIndex = 8;
            this._checkBoxHideArrows.Text = "Hide Arrows";
            this._checkBoxHideArrows.UseVisualStyleBackColor = false;
            this._checkBoxHideArrows.CheckedChanged += new System.EventHandler(this.OnCheckBoxHideArrowsCheckedChanged);
            // 
            // _checkBoxFlipped
            // 
            this._checkBoxFlipped.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxFlipped.AutoSize = true;
            this._checkBoxFlipped.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxFlipped.Location = new System.Drawing.Point(97, 349);
            this._checkBoxFlipped.Name = "_checkBoxFlipped";
            this._checkBoxFlipped.Size = new System.Drawing.Size(100, 17);
            this._checkBoxFlipped.TabIndex = 10;
            this._checkBoxFlipped.Text = "Black at bottom";
            this._checkBoxFlipped.UseVisualStyleBackColor = false;
            this._checkBoxFlipped.CheckedChanged += new System.EventHandler(this.OnCheckBoxFlippedCheckedChanged);
            // 
            // _checkBoxGridBorder
            // 
            this._checkBoxGridBorder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxGridBorder.AutoSize = true;
            this._checkBoxGridBorder.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxGridBorder.Location = new System.Drawing.Point(97, 372);
            this._checkBoxGridBorder.Name = "_checkBoxGridBorder";
            this._checkBoxGridBorder.Size = new System.Drawing.Size(79, 17);
            this._checkBoxGridBorder.TabIndex = 11;
            this._checkBoxGridBorder.Text = "Grid Border";
            this._checkBoxGridBorder.UseVisualStyleBackColor = false;
            this._checkBoxGridBorder.CheckedChanged += new System.EventHandler(this.OnCheckBoxGridBorderCheckedChanged);
            // 
            // _buttonLatest
            // 
            this._buttonLatest.Location = new System.Drawing.Point(6, 176);
            this._buttonLatest.Name = "_buttonLatest";
            this._buttonLatest.Size = new System.Drawing.Size(75, 25);
            this._buttonLatest.TabIndex = 6;
            this._buttonLatest.Text = "Go to latest";
            this._buttonLatest.UseVisualStyleBackColor = true;
            this._buttonLatest.Click += new System.EventHandler(this.OnButtonLatestClick);
            // 
            // _buttonStart
            // 
            this._buttonStart.Location = new System.Drawing.Point(6, 145);
            this._buttonStart.Name = "_buttonStart";
            this._buttonStart.Size = new System.Drawing.Size(75, 25);
            this._buttonStart.TabIndex = 5;
            this._buttonStart.Text = "Go to start";
            this._buttonStart.UseVisualStyleBackColor = true;
            this._buttonStart.Click += new System.EventHandler(this.OnButtonStartClick);
            // 
            // _buttonForward
            // 
            this._buttonForward.Location = new System.Drawing.Point(6, 114);
            this._buttonForward.Name = "_buttonForward";
            this._buttonForward.Size = new System.Drawing.Size(75, 25);
            this._buttonForward.TabIndex = 4;
            this._buttonForward.Text = "Go forward";
            this._buttonForward.UseVisualStyleBackColor = true;
            this._buttonForward.Click += new System.EventHandler(this.OnButtonForwardClick);
            // 
            // _buttonBack
            // 
            this._buttonBack.Location = new System.Drawing.Point(6, 83);
            this._buttonBack.Name = "_buttonBack";
            this._buttonBack.Size = new System.Drawing.Size(75, 25);
            this._buttonBack.TabIndex = 3;
            this._buttonBack.Text = "Go back";
            this._buttonBack.UseVisualStyleBackColor = true;
            this._buttonBack.Click += new System.EventHandler(this.OnButtonBackClick);
            // 
            // _buttonLoadFen
            // 
            this._buttonLoadFen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonLoadFen.Location = new System.Drawing.Point(118, 37);
            this._buttonLoadFen.Name = "_buttonLoadFen";
            this._buttonLoadFen.Size = new System.Drawing.Size(75, 23);
            this._buttonLoadFen.TabIndex = 1;
            this._buttonLoadFen.Text = "Load";
            this._buttonLoadFen.UseVisualStyleBackColor = true;
            this._buttonLoadFen.Click += new System.EventHandler(this.OnButtonLoadFenClick);
            // 
            // _labelFEN
            // 
            this._labelFEN.AutoSize = true;
            this._labelFEN.Location = new System.Drawing.Point(3, 14);
            this._labelFEN.Name = "_labelFEN";
            this._labelFEN.Size = new System.Drawing.Size(25, 13);
            this._labelFEN.TabIndex = 1;
            this._labelFEN.Text = "Fen";
            // 
            // _textBoxFen
            // 
            this._textBoxFen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxFen.Location = new System.Drawing.Point(38, 11);
            this._textBoxFen.Name = "_textBoxFen";
            this._textBoxFen.Size = new System.Drawing.Size(153, 20);
            this._textBoxFen.TabIndex = 0;
            this._textBoxFen.WordWrap = false;
            // 
            // _dataGridView
            // 
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dataGridView.Location = new System.Drawing.Point(0, 0);
            this._dataGridView.MultiSelect = false;
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.ReadOnly = true;
            this._dataGridView.RowHeadersVisible = false;
            this._dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridView.ShowCellErrors = false;
            this._dataGridView.ShowEditingIcon = false;
            this._dataGridView.ShowRowErrors = false;
            this._dataGridView.Size = new System.Drawing.Size(834, 119);
            this._dataGridView.TabIndex = 0;
            // 
            // _arrowInvalidateTimer
            // 
            this._arrowInvalidateTimer.Enabled = true;
            this._arrowInvalidateTimer.Tick += new System.EventHandler(this.OnArrowInvalidateTimerTick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 529);
            this.Controls.Add(this._splitContainerOuter);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(650, 450);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BerldChess Version X";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormMainClosing);
            this._splitContainerOuter.Panel1.ResumeLayout(false);
            this._splitContainerOuter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerOuter)).EndInit();
            this._splitContainerOuter.ResumeLayout(false);
            this._splitContainerInner.Panel2.ResumeLayout(false);
            this._splitContainerInner.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerInner)).EndInit();
            this._splitContainerInner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitContainerOuter;
        private System.Windows.Forms.SplitContainer _splitContainerInner;
        private System.Windows.Forms.DataGridView _dataGridView;
        private System.Windows.Forms.Button _buttonLoadFen;
        private System.Windows.Forms.Label _labelFEN;
        private System.Windows.Forms.TextBox _textBoxFen;
        private System.Windows.Forms.Button _buttonLatest;
        private System.Windows.Forms.Button _buttonStart;
        private System.Windows.Forms.Button _buttonForward;
        private System.Windows.Forms.Button _buttonBack;
        private System.Windows.Forms.CheckBox _checkBoxGridBorder;
        private System.Windows.Forms.CheckBox _checkBoxFlipped;
        private System.Windows.Forms.CheckBox _checkBoxHideArrows;
        private System.Windows.Forms.CheckBox _checkBoxHideOutput;
        private System.Windows.Forms.Timer _arrowInvalidateTimer;
        private System.Windows.Forms.Label _labelCPStatus;
        private System.Windows.Forms.CheckBox _checkBoxLocalMode;
        private System.Windows.Forms.Label _labelEvaluation;
        private System.Windows.Forms.Button _buttonNew;
    }
}

