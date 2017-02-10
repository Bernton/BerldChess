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
            this._buttonAlterPieces = new System.Windows.Forms.Button();
            this._labelAnimTime = new System.Windows.Forms.Label();
            this._textBoxAnimTime = new System.Windows.Forms.TextBox();
            this._checkBoxCheckAuto = new System.Windows.Forms.CheckBox();
            this._buttonReset = new System.Windows.Forms.Button();
            this._buttonColorDialog = new System.Windows.Forms.Button();
            this._buttonUpdateRec = new System.Windows.Forms.Button();
            this._buttonMoveRec = new System.Windows.Forms.Button();
            this._checkBoxSound = new System.Windows.Forms.CheckBox();
            this._buttonAutoPlay = new System.Windows.Forms.Button();
            this._buttonApply = new System.Windows.Forms.Button();
            this._labelMultiPV = new System.Windows.Forms.Label();
            this._textBoxMultiPV = new System.Windows.Forms.TextBox();
            this._labelComputerTime = new System.Windows.Forms.Label();
            this._textBoxEngineTime = new System.Windows.Forms.TextBox();
            this._checkBoxCheatMode = new System.Windows.Forms.CheckBox();
            this._buttonComputerMove = new System.Windows.Forms.Button();
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
            this._slowTimer = new System.Windows.Forms.Timer(this.components);
            this._engineTimer = new System.Windows.Forms.Timer(this.components);
            this._timerAutoCheck = new System.Windows.Forms.Timer(this.components);
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
            this._splitContainerOuter.Size = new System.Drawing.Size(836, 507);
            this._splitContainerOuter.SplitterDistance = 461;
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
            this._splitContainerInner.Panel2.Controls.Add(this._buttonAlterPieces);
            this._splitContainerInner.Panel2.Controls.Add(this._labelAnimTime);
            this._splitContainerInner.Panel2.Controls.Add(this._textBoxAnimTime);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxCheckAuto);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonReset);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonColorDialog);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonUpdateRec);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonMoveRec);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxSound);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonAutoPlay);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonApply);
            this._splitContainerInner.Panel2.Controls.Add(this._labelMultiPV);
            this._splitContainerInner.Panel2.Controls.Add(this._textBoxMultiPV);
            this._splitContainerInner.Panel2.Controls.Add(this._labelComputerTime);
            this._splitContainerInner.Panel2.Controls.Add(this._textBoxEngineTime);
            this._splitContainerInner.Panel2.Controls.Add(this._checkBoxCheatMode);
            this._splitContainerInner.Panel2.Controls.Add(this._buttonComputerMove);
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
            this._splitContainerInner.Size = new System.Drawing.Size(836, 461);
            this._splitContainerInner.SplitterDistance = 616;
            this._splitContainerInner.SplitterWidth = 3;
            this._splitContainerInner.TabIndex = 0;
            this._splitContainerInner.TabStop = false;
            // 
            // _buttonAlterPieces
            // 
            this._buttonAlterPieces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonAlterPieces.Location = new System.Drawing.Point(125, 254);
            this._buttonAlterPieces.Name = "_buttonAlterPieces";
            this._buttonAlterPieces.Size = new System.Drawing.Size(81, 23);
            this._buttonAlterPieces.TabIndex = 30;
            this._buttonAlterPieces.Text = "Alter Pieces";
            this._buttonAlterPieces.UseVisualStyleBackColor = true;
            this._buttonAlterPieces.Click += new System.EventHandler(this.OnButtonAlterPiecesClick);
            // 
            // _labelAnimTime
            // 
            this._labelAnimTime.AutoSize = true;
            this._labelAnimTime.Location = new System.Drawing.Point(12, 347);
            this._labelAnimTime.Name = "_labelAnimTime";
            this._labelAnimTime.Size = new System.Drawing.Size(79, 13);
            this._labelAnimTime.TabIndex = 29;
            this._labelAnimTime.Text = "Animation Time";
            // 
            // _textBoxAnimTime
            // 
            this._textBoxAnimTime.Location = new System.Drawing.Point(28, 363);
            this._textBoxAnimTime.Name = "_textBoxAnimTime";
            this._textBoxAnimTime.Size = new System.Drawing.Size(59, 20);
            this._textBoxAnimTime.TabIndex = 28;
            this._textBoxAnimTime.Text = "300";
            this._textBoxAnimTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._textBoxAnimTime.TextChanged += new System.EventHandler(this.OnTextBoxAnimTimeTextChanged);
            // 
            // _checkBoxCheckAuto
            // 
            this._checkBoxCheckAuto.AutoSize = true;
            this._checkBoxCheckAuto.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxCheckAuto.Location = new System.Drawing.Point(11, 316);
            this._checkBoxCheckAuto.Name = "_checkBoxCheckAuto";
            this._checkBoxCheckAuto.Size = new System.Drawing.Size(82, 17);
            this._checkBoxCheckAuto.TabIndex = 27;
            this._checkBoxCheckAuto.Text = "Check Auto";
            this._checkBoxCheckAuto.UseVisualStyleBackColor = false;
            // 
            // _buttonReset
            // 
            this._buttonReset.Location = new System.Drawing.Point(9, 229);
            this._buttonReset.Name = "_buttonReset";
            this._buttonReset.Size = new System.Drawing.Size(54, 23);
            this._buttonReset.TabIndex = 26;
            this._buttonReset.Text = "Reset";
            this._buttonReset.UseVisualStyleBackColor = true;
            this._buttonReset.Click += new System.EventHandler(this.OnButtonResetClick);
            // 
            // _buttonColorDialog
            // 
            this._buttonColorDialog.Location = new System.Drawing.Point(69, 229);
            this._buttonColorDialog.Name = "_buttonColorDialog";
            this._buttonColorDialog.Size = new System.Drawing.Size(22, 23);
            this._buttonColorDialog.TabIndex = 25;
            this._buttonColorDialog.Text = "..";
            this._buttonColorDialog.UseVisualStyleBackColor = true;
            this._buttonColorDialog.Click += new System.EventHandler(this.OnButtonColorDialogClick);
            // 
            // _buttonUpdateRec
            // 
            this._buttonUpdateRec.Location = new System.Drawing.Point(9, 258);
            this._buttonUpdateRec.Name = "_buttonUpdateRec";
            this._buttonUpdateRec.Size = new System.Drawing.Size(82, 23);
            this._buttonUpdateRec.TabIndex = 24;
            this._buttonUpdateRec.Text = "Update";
            this._buttonUpdateRec.UseVisualStyleBackColor = true;
            this._buttonUpdateRec.Click += new System.EventHandler(this.OnButtonUpdateRecClick);
            // 
            // _buttonMoveRec
            // 
            this._buttonMoveRec.Location = new System.Drawing.Point(9, 287);
            this._buttonMoveRec.Name = "_buttonMoveRec";
            this._buttonMoveRec.Size = new System.Drawing.Size(82, 23);
            this._buttonMoveRec.TabIndex = 23;
            this._buttonMoveRec.Text = "Auto Move";
            this._buttonMoveRec.UseVisualStyleBackColor = true;
            this._buttonMoveRec.Click += new System.EventHandler(this.OnButtonMoveRecClick);
            // 
            // _checkBoxSound
            // 
            this._checkBoxSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxSound.AutoSize = true;
            this._checkBoxSound.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxSound.Checked = true;
            this._checkBoxSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxSound.Location = new System.Drawing.Point(117, 430);
            this._checkBoxSound.Name = "_checkBoxSound";
            this._checkBoxSound.Size = new System.Drawing.Size(57, 17);
            this._checkBoxSound.TabIndex = 22;
            this._checkBoxSound.Text = "Sound";
            this._checkBoxSound.UseVisualStyleBackColor = false;
            // 
            // _buttonAutoPlay
            // 
            this._buttonAutoPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonAutoPlay.Location = new System.Drawing.Point(103, 98);
            this._buttonAutoPlay.Name = "_buttonAutoPlay";
            this._buttonAutoPlay.Size = new System.Drawing.Size(20, 25);
            this._buttonAutoPlay.TabIndex = 21;
            this._buttonAutoPlay.Text = "A";
            this._buttonAutoPlay.UseVisualStyleBackColor = true;
            this._buttonAutoPlay.Click += new System.EventHandler(this.OnButtonAutoPlayClick);
            // 
            // _buttonApply
            // 
            this._buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonApply.Location = new System.Drawing.Point(146, 222);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(59, 23);
            this._buttonApply.TabIndex = 20;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _labelMultiPV
            // 
            this._labelMultiPV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labelMultiPV.AutoSize = true;
            this._labelMultiPV.Location = new System.Drawing.Point(158, 180);
            this._labelMultiPV.Name = "_labelMultiPV";
            this._labelMultiPV.Size = new System.Drawing.Size(46, 13);
            this._labelMultiPV.TabIndex = 19;
            this._labelMultiPV.Text = "Multi PV";
            // 
            // _textBoxMultiPV
            // 
            this._textBoxMultiPV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxMultiPV.Location = new System.Drawing.Point(163, 196);
            this._textBoxMultiPV.Name = "_textBoxMultiPV";
            this._textBoxMultiPV.Size = new System.Drawing.Size(41, 20);
            this._textBoxMultiPV.TabIndex = 18;
            this._textBoxMultiPV.Text = "250";
            this._textBoxMultiPV.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _labelComputerTime
            // 
            this._labelComputerTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labelComputerTime.AutoSize = true;
            this._labelComputerTime.Location = new System.Drawing.Point(138, 134);
            this._labelComputerTime.Name = "_labelComputerTime";
            this._labelComputerTime.Size = new System.Drawing.Size(66, 13);
            this._labelComputerTime.TabIndex = 17;
            this._labelComputerTime.Text = "Engine Time";
            // 
            // _textBoxEngineTime
            // 
            this._textBoxEngineTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxEngineTime.Location = new System.Drawing.Point(147, 150);
            this._textBoxEngineTime.Name = "_textBoxEngineTime";
            this._textBoxEngineTime.Size = new System.Drawing.Size(57, 20);
            this._textBoxEngineTime.TabIndex = 16;
            this._textBoxEngineTime.Text = "300";
            this._textBoxEngineTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._textBoxEngineTime.TextChanged += new System.EventHandler(this.OnTextBoxEngineTimeTextChanged);
            // 
            // _checkBoxCheatMode
            // 
            this._checkBoxCheatMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxCheatMode.AutoSize = true;
            this._checkBoxCheatMode.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxCheatMode.Location = new System.Drawing.Point(117, 305);
            this._checkBoxCheatMode.Name = "_checkBoxCheatMode";
            this._checkBoxCheatMode.Size = new System.Drawing.Size(84, 17);
            this._checkBoxCheatMode.TabIndex = 15;
            this._checkBoxCheatMode.Text = "Cheat Mode";
            this._checkBoxCheatMode.UseVisualStyleBackColor = false;
            this._checkBoxCheatMode.CheckedChanged += new System.EventHandler(this.OnCheckBoxCheatModeCheckedChanged);
            // 
            // _buttonComputerMove
            // 
            this._buttonComputerMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonComputerMove.Location = new System.Drawing.Point(129, 98);
            this._buttonComputerMove.Name = "_buttonComputerMove";
            this._buttonComputerMove.Size = new System.Drawing.Size(75, 25);
            this._buttonComputerMove.TabIndex = 14;
            this._buttonComputerMove.Text = "Engine";
            this._buttonComputerMove.UseVisualStyleBackColor = true;
            this._buttonComputerMove.Click += new System.EventHandler(this.OnButtonComputerMoveClick);
            // 
            // _buttonNew
            // 
            this._buttonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonNew.Location = new System.Drawing.Point(129, 67);
            this._buttonNew.Name = "_buttonNew";
            this._buttonNew.Size = new System.Drawing.Size(75, 25);
            this._buttonNew.TabIndex = 2;
            this._buttonNew.Text = "New";
            this._buttonNew.UseVisualStyleBackColor = true;
            this._buttonNew.Click += new System.EventHandler(this.OnButtonNewClick);
            // 
            // _labelEvaluation
            // 
            this._labelEvaluation.AutoSize = true;
            this._labelEvaluation.Location = new System.Drawing.Point(6, 168);
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
            this._checkBoxLocalMode.Location = new System.Drawing.Point(117, 367);
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
            this._labelCPStatus.Location = new System.Drawing.Point(3, 184);
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
            this._checkBoxHideOutput.Location = new System.Drawing.Point(117, 325);
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
            this._checkBoxHideArrows.Location = new System.Drawing.Point(117, 346);
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
            this._checkBoxFlipped.Location = new System.Drawing.Point(117, 388);
            this._checkBoxFlipped.Name = "_checkBoxFlipped";
            this._checkBoxFlipped.Size = new System.Drawing.Size(60, 17);
            this._checkBoxFlipped.TabIndex = 10;
            this._checkBoxFlipped.Text = "Flipped";
            this._checkBoxFlipped.UseVisualStyleBackColor = false;
            this._checkBoxFlipped.CheckedChanged += new System.EventHandler(this.OnCheckBoxFlippedCheckedChanged);
            // 
            // _checkBoxGridBorder
            // 
            this._checkBoxGridBorder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxGridBorder.AutoSize = true;
            this._checkBoxGridBorder.BackColor = System.Drawing.SystemColors.Control;
            this._checkBoxGridBorder.Location = new System.Drawing.Point(117, 409);
            this._checkBoxGridBorder.Name = "_checkBoxGridBorder";
            this._checkBoxGridBorder.Size = new System.Drawing.Size(79, 17);
            this._checkBoxGridBorder.TabIndex = 11;
            this._checkBoxGridBorder.Text = "Grid Border";
            this._checkBoxGridBorder.UseVisualStyleBackColor = false;
            this._checkBoxGridBorder.CheckedChanged += new System.EventHandler(this.OnCheckBoxGridBorderCheckedChanged);
            // 
            // _buttonLatest
            // 
            this._buttonLatest.Location = new System.Drawing.Point(6, 129);
            this._buttonLatest.Name = "_buttonLatest";
            this._buttonLatest.Size = new System.Drawing.Size(75, 25);
            this._buttonLatest.TabIndex = 6;
            this._buttonLatest.Text = "Go to latest";
            this._buttonLatest.UseVisualStyleBackColor = true;
            this._buttonLatest.Click += new System.EventHandler(this.OnButtonLatestClick);
            // 
            // _buttonStart
            // 
            this._buttonStart.Location = new System.Drawing.Point(6, 98);
            this._buttonStart.Name = "_buttonStart";
            this._buttonStart.Size = new System.Drawing.Size(75, 25);
            this._buttonStart.TabIndex = 5;
            this._buttonStart.Text = "Go to start";
            this._buttonStart.UseVisualStyleBackColor = true;
            this._buttonStart.Click += new System.EventHandler(this.OnButtonStartClick);
            // 
            // _buttonForward
            // 
            this._buttonForward.Location = new System.Drawing.Point(6, 67);
            this._buttonForward.Name = "_buttonForward";
            this._buttonForward.Size = new System.Drawing.Size(75, 25);
            this._buttonForward.TabIndex = 4;
            this._buttonForward.Text = "Go forward";
            this._buttonForward.UseVisualStyleBackColor = true;
            this._buttonForward.Click += new System.EventHandler(this.OnButtonForwardClick);
            // 
            // _buttonBack
            // 
            this._buttonBack.Location = new System.Drawing.Point(6, 36);
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
            this._buttonLoadFen.Location = new System.Drawing.Point(129, 36);
            this._buttonLoadFen.Name = "_buttonLoadFen";
            this._buttonLoadFen.Size = new System.Drawing.Size(75, 25);
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
            this._textBoxFen.Location = new System.Drawing.Point(34, 10);
            this._textBoxFen.Name = "_textBoxFen";
            this._textBoxFen.Size = new System.Drawing.Size(170, 20);
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
            this._dataGridView.Size = new System.Drawing.Size(834, 41);
            this._dataGridView.TabIndex = 0;
            // 
            // _slowTimer
            // 
            this._slowTimer.Enabled = true;
            this._slowTimer.Tick += new System.EventHandler(this.OnSlowTimerTick);
            // 
            // _engineTimer
            // 
            this._engineTimer.Interval = 10;
            this._engineTimer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // _timerAutoCheck
            // 
            this._timerAutoCheck.Enabled = true;
            this._timerAutoCheck.Interval = 400;
            this._timerAutoCheck.Tick += new System.EventHandler(this.OnTimerAutoCheckTick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 507);
            this.Controls.Add(this._splitContainerOuter);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(852, 546);
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
        private System.Windows.Forms.Timer _slowTimer;
        private System.Windows.Forms.Label _labelCPStatus;
        private System.Windows.Forms.CheckBox _checkBoxLocalMode;
        private System.Windows.Forms.Label _labelEvaluation;
        private System.Windows.Forms.Button _buttonNew;
        private System.Windows.Forms.Button _buttonComputerMove;
        private System.Windows.Forms.Timer _engineTimer;
        private System.Windows.Forms.CheckBox _checkBoxCheatMode;
        private System.Windows.Forms.TextBox _textBoxEngineTime;
        private System.Windows.Forms.Label _labelComputerTime;
        private System.Windows.Forms.Label _labelMultiPV;
        private System.Windows.Forms.TextBox _textBoxMultiPV;
        private System.Windows.Forms.Button _buttonApply;
        private System.Windows.Forms.Button _buttonAutoPlay;
        private System.Windows.Forms.CheckBox _checkBoxSound;
        private System.Windows.Forms.Button _buttonMoveRec;
        private System.Windows.Forms.Button _buttonUpdateRec;
        private System.Windows.Forms.Button _buttonColorDialog;
        private System.Windows.Forms.Button _buttonReset;
        private System.Windows.Forms.CheckBox _checkBoxCheckAuto;
        private System.Windows.Forms.Timer _timerAutoCheck;
        private System.Windows.Forms.Label _labelAnimTime;
        private System.Windows.Forms.TextBox _textBoxAnimTime;
        private System.Windows.Forms.Button _buttonAlterPieces;
    }
}

