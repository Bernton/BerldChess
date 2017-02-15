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
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this._slowTimer = new System.Windows.Forms.Timer(this.components);
            this._engineTimer = new System.Windows.Forms.Timer(this.components);
            this._timerAutoCheck = new System.Windows.Forms.Timer(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFENToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.engineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.engineTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiPVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cheatModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.squareColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appearanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterPiecesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridBorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToLatestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideArrowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemLatest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemForward = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBack = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStart = new System.Windows.Forms.ToolStripMenuItem();
            this._labelShowEval = new System.Windows.Forms.Label();
            this._labelEval = new System.Windows.Forms.Label();
            this._panelRight = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerOuter)).BeginInit();
            this._splitContainerOuter.Panel2.SuspendLayout();
            this._splitContainerOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.menuStrip.SuspendLayout();
            this._panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // _splitContainerOuter
            // 
            this._splitContainerOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._splitContainerOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._splitContainerOuter.ForeColor = System.Drawing.SystemColors.ControlText;
            this._splitContainerOuter.Location = new System.Drawing.Point(-1, 34);
            this._splitContainerOuter.Name = "_splitContainerOuter";
            this._splitContainerOuter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainerOuter.Panel2
            // 
            this._splitContainerOuter.Panel2.Controls.Add(this._dataGridView);
            this._splitContainerOuter.Panel2Collapsed = true;
            this._splitContainerOuter.Size = new System.Drawing.Size(604, 473);
            this._splitContainerOuter.SplitterDistance = 448;
            this._splitContainerOuter.SplitterWidth = 3;
            this._splitContainerOuter.TabIndex = 0;
            this._splitContainerOuter.TabStop = false;
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
            this._dataGridView.Size = new System.Drawing.Size(148, 44);
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
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Font = new System.Drawing.Font("Arial Unicode MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameToolStripMenuItem,
            this.engineToolStripMenuItem,
            this.appearanceToolStripMenuItem,
            this.navigationToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripMenuItemLatest,
            this.toolStripMenuItemForward,
            this.toolStripMenuItemBack,
            this.toolStripMenuItemStart});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(836, 33);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.copyFENToClipboardToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(62, 29);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeyDisplayString = "N";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.newToolStripMenuItem.Tag = "";
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.OnNewClick);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeyDisplayString = "L";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.loadToolStripMenuItem.Tag = "";
            this.loadToolStripMenuItem.Text = "Load from FEN";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.OnButtonLoadFenClick);
            // 
            // copyFENToClipboardToolStripMenuItem
            // 
            this.copyFENToClipboardToolStripMenuItem.Name = "copyFENToClipboardToolStripMenuItem";
            this.copyFENToClipboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.copyFENToClipboardToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.copyFENToClipboardToolStripMenuItem.Text = "Copy FEN To Clipboard";
            this.copyFENToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyFENToClipboardToolStripMenuItem_Click);
            // 
            // engineToolStripMenuItem
            // 
            this.engineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.engineTimeToolStripMenuItem,
            this.multiPVToolStripMenuItem,
            this.playNowToolStripMenuItem,
            this.autoplayToolStripMenuItem,
            this.cheatModeToolStripMenuItem});
            this.engineToolStripMenuItem.Name = "engineToolStripMenuItem";
            this.engineToolStripMenuItem.Size = new System.Drawing.Size(66, 29);
            this.engineToolStripMenuItem.Text = "Engine";
            // 
            // engineTimeToolStripMenuItem
            // 
            this.engineTimeToolStripMenuItem.Name = "engineTimeToolStripMenuItem";
            this.engineTimeToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.engineTimeToolStripMenuItem.Text = "Engine Time";
            this.engineTimeToolStripMenuItem.Click += new System.EventHandler(this.engineTimeToolStripMenuItem_Click);
            // 
            // multiPVToolStripMenuItem
            // 
            this.multiPVToolStripMenuItem.Name = "multiPVToolStripMenuItem";
            this.multiPVToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.multiPVToolStripMenuItem.Text = "MultiPV";
            this.multiPVToolStripMenuItem.Click += new System.EventHandler(this.multiPVToolStripMenuItem_Click);
            // 
            // playNowToolStripMenuItem
            // 
            this.playNowToolStripMenuItem.Name = "playNowToolStripMenuItem";
            this.playNowToolStripMenuItem.ShortcutKeyDisplayString = "C";
            this.playNowToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.playNowToolStripMenuItem.Text = "Play Now";
            this.playNowToolStripMenuItem.Click += new System.EventHandler(this.OnButtonComputerMoveClick);
            // 
            // autoplayToolStripMenuItem
            // 
            this.autoplayToolStripMenuItem.Name = "autoplayToolStripMenuItem";
            this.autoplayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.autoplayToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.autoplayToolStripMenuItem.Text = "Autoplay";
            this.autoplayToolStripMenuItem.Click += new System.EventHandler(this.OnButtonAutoPlayClick);
            // 
            // cheatModeToolStripMenuItem
            // 
            this.cheatModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animTimeToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.squareColorsToolStripMenuItem,
            this.autoMoveToolStripMenuItem,
            this.checkAutoToolStripMenuItem});
            this.cheatModeToolStripMenuItem.Name = "cheatModeToolStripMenuItem";
            this.cheatModeToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.cheatModeToolStripMenuItem.Text = "Cheat Mode";
            this.cheatModeToolStripMenuItem.Click += new System.EventHandler(this.cheatModeToolStripMenuItem_Click);
            // 
            // animTimeToolStripMenuItem
            // 
            this.animTimeToolStripMenuItem.Name = "animTimeToolStripMenuItem";
            this.animTimeToolStripMenuItem.Size = new System.Drawing.Size(172, 24);
            this.animTimeToolStripMenuItem.Text = "Anim Time";
            this.animTimeToolStripMenuItem.Click += new System.EventHandler(this.animTimeToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(172, 24);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.OnButtonResetClick);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(172, 24);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.OnButtonUpdateRecClick);
            // 
            // squareColorsToolStripMenuItem
            // 
            this.squareColorsToolStripMenuItem.Name = "squareColorsToolStripMenuItem";
            this.squareColorsToolStripMenuItem.Size = new System.Drawing.Size(172, 24);
            this.squareColorsToolStripMenuItem.Text = "Square Colors";
            this.squareColorsToolStripMenuItem.Click += new System.EventHandler(this.OnButtonColorDialogClick);
            // 
            // autoMoveToolStripMenuItem
            // 
            this.autoMoveToolStripMenuItem.Name = "autoMoveToolStripMenuItem";
            this.autoMoveToolStripMenuItem.ShortcutKeyDisplayString = "A";
            this.autoMoveToolStripMenuItem.Size = new System.Drawing.Size(172, 24);
            this.autoMoveToolStripMenuItem.Text = "Auto Move";
            this.autoMoveToolStripMenuItem.Click += new System.EventHandler(this.OnButtonMoveRecClick);
            // 
            // checkAutoToolStripMenuItem
            // 
            this.checkAutoToolStripMenuItem.Name = "checkAutoToolStripMenuItem";
            this.checkAutoToolStripMenuItem.Size = new System.Drawing.Size(172, 24);
            this.checkAutoToolStripMenuItem.Text = "Check Auto";
            this.checkAutoToolStripMenuItem.Click += new System.EventHandler(this.checkAutoToolStripMenuItem_Click);
            // 
            // appearanceToolStripMenuItem
            // 
            this.appearanceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alterPiecesToolStripMenuItem,
            this.gridBorderToolStripMenuItem});
            this.appearanceToolStripMenuItem.Name = "appearanceToolStripMenuItem";
            this.appearanceToolStripMenuItem.Size = new System.Drawing.Size(100, 29);
            this.appearanceToolStripMenuItem.Text = "Appearance";
            // 
            // alterPiecesToolStripMenuItem
            // 
            this.alterPiecesToolStripMenuItem.Name = "alterPiecesToolStripMenuItem";
            this.alterPiecesToolStripMenuItem.Size = new System.Drawing.Size(157, 24);
            this.alterPiecesToolStripMenuItem.Text = "Alter Pieces";
            this.alterPiecesToolStripMenuItem.Click += new System.EventHandler(this.OnButtonAlterPiecesClick);
            // 
            // gridBorderToolStripMenuItem
            // 
            this.gridBorderToolStripMenuItem.Name = "gridBorderToolStripMenuItem";
            this.gridBorderToolStripMenuItem.Size = new System.Drawing.Size(157, 24);
            this.gridBorderToolStripMenuItem.Text = "Grid Border";
            this.gridBorderToolStripMenuItem.Click += new System.EventHandler(this.OnCheckBoxGridBorderCheckedChanged);
            // 
            // navigationToolStripMenuItem
            // 
            this.navigationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goBackToolStripMenuItem,
            this.goForwardToolStripMenuItem,
            this.goToStartToolStripMenuItem,
            this.goToLatestToolStripMenuItem});
            this.navigationToolStripMenuItem.Name = "navigationToolStripMenuItem";
            this.navigationToolStripMenuItem.Size = new System.Drawing.Size(90, 29);
            this.navigationToolStripMenuItem.Text = "Navigation";
            // 
            // goBackToolStripMenuItem
            // 
            this.goBackToolStripMenuItem.Name = "goBackToolStripMenuItem";
            this.goBackToolStripMenuItem.ShortcutKeyDisplayString = "Left";
            this.goBackToolStripMenuItem.Size = new System.Drawing.Size(194, 24);
            this.goBackToolStripMenuItem.Text = "Go back";
            this.goBackToolStripMenuItem.Click += new System.EventHandler(this.OnButtonBackClick);
            // 
            // goForwardToolStripMenuItem
            // 
            this.goForwardToolStripMenuItem.Name = "goForwardToolStripMenuItem";
            this.goForwardToolStripMenuItem.ShortcutKeyDisplayString = "Right";
            this.goForwardToolStripMenuItem.Size = new System.Drawing.Size(194, 24);
            this.goForwardToolStripMenuItem.Text = "Go forward";
            this.goForwardToolStripMenuItem.Click += new System.EventHandler(this.OnButtonForwardClick);
            // 
            // goToStartToolStripMenuItem
            // 
            this.goToStartToolStripMenuItem.Name = "goToStartToolStripMenuItem";
            this.goToStartToolStripMenuItem.ShortcutKeyDisplayString = "Down";
            this.goToStartToolStripMenuItem.Size = new System.Drawing.Size(194, 24);
            this.goToStartToolStripMenuItem.Text = "Go to start";
            this.goToStartToolStripMenuItem.Click += new System.EventHandler(this.OnButtonStartClick);
            // 
            // goToLatestToolStripMenuItem
            // 
            this.goToLatestToolStripMenuItem.Name = "goToLatestToolStripMenuItem";
            this.goToLatestToolStripMenuItem.ShortcutKeyDisplayString = "Up";
            this.goToLatestToolStripMenuItem.Size = new System.Drawing.Size(194, 24);
            this.goToLatestToolStripMenuItem.Text = "Go to latest";
            this.goToLatestToolStripMenuItem.Click += new System.EventHandler(this.OnButtonLatestClick);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flipBoardToolStripMenuItem,
            this.soundToolStripMenuItem,
            this.hideOutputToolStripMenuItem,
            this.hideArrowsToolStripMenuItem,
            this.localModeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(72, 29);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // flipBoardToolStripMenuItem
            // 
            this.flipBoardToolStripMenuItem.Name = "flipBoardToolStripMenuItem";
            this.flipBoardToolStripMenuItem.ShortcutKeyDisplayString = "F";
            this.flipBoardToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.flipBoardToolStripMenuItem.Text = "Flip Board";
            this.flipBoardToolStripMenuItem.Click += new System.EventHandler(this.OnCheckBoxFlippedCheckedChanged);
            // 
            // soundToolStripMenuItem
            // 
            this.soundToolStripMenuItem.Name = "soundToolStripMenuItem";
            this.soundToolStripMenuItem.ShortcutKeyDisplayString = "S";
            this.soundToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.soundToolStripMenuItem.Text = "Sound";
            this.soundToolStripMenuItem.Click += new System.EventHandler(this.soundToolStripMenuItem_Click);
            // 
            // hideOutputToolStripMenuItem
            // 
            this.hideOutputToolStripMenuItem.Name = "hideOutputToolStripMenuItem";
            this.hideOutputToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.hideOutputToolStripMenuItem.Text = "Hide Output";
            this.hideOutputToolStripMenuItem.Click += new System.EventHandler(this.OnCheckBoxHideOutputCheckedChanged);
            // 
            // hideArrowsToolStripMenuItem
            // 
            this.hideArrowsToolStripMenuItem.Name = "hideArrowsToolStripMenuItem";
            this.hideArrowsToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.hideArrowsToolStripMenuItem.Text = "Hide Arrows";
            this.hideArrowsToolStripMenuItem.Click += new System.EventHandler(this.OnCheckBoxHideArrowsCheckedChanged);
            // 
            // localModeToolStripMenuItem
            // 
            this.localModeToolStripMenuItem.Name = "localModeToolStripMenuItem";
            this.localModeToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.localModeToolStripMenuItem.Text = "Local Mode";
            this.localModeToolStripMenuItem.Click += new System.EventHandler(this.OnCheckBoxPlayModeCheckedChanged);
            // 
            // toolStripMenuItemLatest
            // 
            this.toolStripMenuItemLatest.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItemLatest.Font = new System.Drawing.Font("Agency FB", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItemLatest.Name = "toolStripMenuItemLatest";
            this.toolStripMenuItemLatest.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripMenuItemLatest.Size = new System.Drawing.Size(40, 29);
            this.toolStripMenuItemLatest.Text = ">>";
            this.toolStripMenuItemLatest.Click += new System.EventHandler(this.OnButtonLatestClick);
            // 
            // toolStripMenuItemForward
            // 
            this.toolStripMenuItemForward.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItemForward.Font = new System.Drawing.Font("Agency FB", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItemForward.Name = "toolStripMenuItemForward";
            this.toolStripMenuItemForward.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripMenuItemForward.Size = new System.Drawing.Size(32, 29);
            this.toolStripMenuItemForward.Text = ">";
            this.toolStripMenuItemForward.Click += new System.EventHandler(this.OnButtonForwardClick);
            // 
            // toolStripMenuItemBack
            // 
            this.toolStripMenuItemBack.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItemBack.Font = new System.Drawing.Font("Agency FB", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItemBack.Name = "toolStripMenuItemBack";
            this.toolStripMenuItemBack.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripMenuItemBack.Size = new System.Drawing.Size(32, 29);
            this.toolStripMenuItemBack.Text = "<";
            this.toolStripMenuItemBack.Click += new System.EventHandler(this.OnButtonBackClick);
            // 
            // toolStripMenuItemStart
            // 
            this.toolStripMenuItemStart.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItemStart.Font = new System.Drawing.Font("Agency FB", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItemStart.Name = "toolStripMenuItemStart";
            this.toolStripMenuItemStart.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripMenuItemStart.Size = new System.Drawing.Size(40, 29);
            this.toolStripMenuItemStart.Text = "<<";
            this.toolStripMenuItemStart.Click += new System.EventHandler(this.OnButtonStartClick);
            // 
            // _labelShowEval
            // 
            this._labelShowEval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labelShowEval.AutoSize = true;
            this._labelShowEval.BackColor = System.Drawing.SystemColors.Control;
            this._labelShowEval.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelShowEval.Location = new System.Drawing.Point(64, 26);
            this._labelShowEval.Name = "_labelShowEval";
            this._labelShowEval.Size = new System.Drawing.Size(80, 18);
            this._labelShowEval.TabIndex = 2;
            this._labelShowEval.Text = "Evaluation:";
            // 
            // _labelEval
            // 
            this._labelEval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labelEval.BackColor = System.Drawing.SystemColors.Control;
            this._labelEval.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelEval.Location = new System.Drawing.Point(6, 44);
            this._labelEval.Name = "_labelEval";
            this._labelEval.Size = new System.Drawing.Size(223, 39);
            this._labelEval.TabIndex = 3;
            this._labelEval.Text = "+0.00";
            this._labelEval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _panelRight
            // 
            this._panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelRight.Controls.Add(this._labelEval);
            this._panelRight.Controls.Add(this._labelShowEval);
            this._panelRight.Location = new System.Drawing.Point(602, 34);
            this._panelRight.Name = "_panelRight";
            this._panelRight.Size = new System.Drawing.Size(234, 473);
            this._panelRight.TabIndex = 4;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 507);
            this.Controls.Add(this._splitContainerOuter);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this._panelRight);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(852, 546);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BerldChess Version X";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormMainClosing);
            this.Load += new System.EventHandler(this.OnFormMainLoad);
            this._splitContainerOuter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerOuter)).EndInit();
            this._splitContainerOuter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this._panelRight.ResumeLayout(false);
            this._panelRight.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitContainerOuter;
        private System.Windows.Forms.DataGridView _dataGridView;
        private System.Windows.Forms.Timer _slowTimer;
        private System.Windows.Forms.Timer _engineTimer;
        private System.Windows.Forms.Timer _timerAutoCheck;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem engineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem engineTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiPVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cheatModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appearanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alterPiecesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridBorderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem navigationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goForwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToLatestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideArrowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem squareColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoMoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAutoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLatest;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemForward;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBack;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStart;
        private System.Windows.Forms.ToolStripMenuItem copyFENToClipboardToolStripMenuItem;
        private System.Windows.Forms.Label _labelShowEval;
        private System.Windows.Forms.Label _labelEval;
        private System.Windows.Forms.Panel _panelRight;
    }
}

