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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this._splitContainerBoard = new System.Windows.Forms.SplitContainer();
            this._dataGridViewEvaluation = new System.Windows.Forms.DataGridView();
            this._timerValidation = new System.Windows.Forms.Timer(this.components);
            this._timerEngine = new System.Windows.Forms.Timer(this.components);
            this._timerAutoCheck = new System.Windows.Forms.Timer(this.components);
            this._menuStripMain = new System.Windows.Forms.MenuStrip();
            this._menuItemGame = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemNew = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemLoadFen = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemCopyFen = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemLoadPgn = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemEngine = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemEngineTime = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemMultiPv = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemComputerMove = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemAutoPlay = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemDepthAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemCheatMode = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemAnimationTime = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemClickDelay = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemReset = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemSquareColors = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemAutoMove = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemCheckAuto = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemAppearance = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemAlterPieces = new System.Windows.Forms.ToolStripMenuItem();
            this.alterBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemFlipBoard = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemLocalMode = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemHideOutput = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemHideArrows = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemSound = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemNavigation = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemGoBack = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemGoForward = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemGoToStart = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemGoToLatest = new System.Windows.Forms.ToolStripMenuItem();
            this._labelShowEvaluation = new System.Windows.Forms.Label();
            this._labelEvaluation = new System.Windows.Forms.Label();
            this._panelRight = new System.Windows.Forms.Panel();
            this._tableLayoutPanelModules = new System.Windows.Forms.TableLayoutPanel();
            this._dataGridViewMoves = new System.Windows.Forms.DataGridView();
            this._whiteMove = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._blackMove = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._panelEvaluationChart = new System.Windows.Forms.Panel();
            this._splitContainerMain = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerBoard)).BeginInit();
            this._splitContainerBoard.Panel2.SuspendLayout();
            this._splitContainerBoard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewEvaluation)).BeginInit();
            this._menuStripMain.SuspendLayout();
            this._panelRight.SuspendLayout();
            this._tableLayoutPanelModules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewMoves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerMain)).BeginInit();
            this._splitContainerMain.Panel1.SuspendLayout();
            this._splitContainerMain.Panel2.SuspendLayout();
            this._splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // _splitContainerBoard
            // 
            this._splitContainerBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._splitContainerBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerBoard.ForeColor = System.Drawing.SystemColors.ControlText;
            this._splitContainerBoard.Location = new System.Drawing.Point(0, 0);
            this._splitContainerBoard.Name = "_splitContainerBoard";
            this._splitContainerBoard.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainerBoard.Panel2
            // 
            this._splitContainerBoard.Panel2.Controls.Add(this._dataGridViewEvaluation);
            this._splitContainerBoard.Size = new System.Drawing.Size(729, 498);
            this._splitContainerBoard.SplitterDistance = 466;
            this._splitContainerBoard.SplitterWidth = 3;
            this._splitContainerBoard.TabIndex = 0;
            this._splitContainerBoard.TabStop = false;
            // 
            // _dataGridViewEvaluation
            // 
            this._dataGridViewEvaluation.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dataGridViewEvaluation.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._dataGridViewEvaluation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridViewEvaluation.DefaultCellStyle = dataGridViewCellStyle2;
            this._dataGridViewEvaluation.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridViewEvaluation.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dataGridViewEvaluation.Location = new System.Drawing.Point(0, 0);
            this._dataGridViewEvaluation.MultiSelect = false;
            this._dataGridViewEvaluation.Name = "_dataGridViewEvaluation";
            this._dataGridViewEvaluation.ReadOnly = true;
            this._dataGridViewEvaluation.RowHeadersVisible = false;
            this._dataGridViewEvaluation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._dataGridViewEvaluation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridViewEvaluation.ShowCellErrors = false;
            this._dataGridViewEvaluation.ShowEditingIcon = false;
            this._dataGridViewEvaluation.ShowRowErrors = false;
            this._dataGridViewEvaluation.Size = new System.Drawing.Size(727, 27);
            this._dataGridViewEvaluation.TabIndex = 1;
            // 
            // _timerValidation
            // 
            this._timerValidation.Enabled = true;
            this._timerValidation.Tick += new System.EventHandler(this.OnTimerValidationTick);
            // 
            // _timerEngine
            // 
            this._timerEngine.Interval = 5;
            this._timerEngine.Tick += new System.EventHandler(this.OnTimerEngineTick);
            // 
            // _timerAutoCheck
            // 
            this._timerAutoCheck.Enabled = true;
            this._timerAutoCheck.Interval = 40;
            this._timerAutoCheck.Tick += new System.EventHandler(this.OnTimerAutoCheckTick);
            // 
            // _menuStripMain
            // 
            this._menuStripMain.BackColor = System.Drawing.SystemColors.Control;
            this._menuStripMain.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._menuStripMain.GripMargin = new System.Windows.Forms.Padding(2, 4, 0, 2);
            this._menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemGame,
            this._menuItemEngine,
            this._menuItemAppearance,
            this._menuItemOptions,
            this._menuItemNavigation});
            this._menuStripMain.Location = new System.Drawing.Point(0, 0);
            this._menuStripMain.Name = "_menuStripMain";
            this._menuStripMain.Padding = new System.Windows.Forms.Padding(4, 4, 0, 2);
            this._menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this._menuStripMain.Size = new System.Drawing.Size(969, 31);
            this._menuStripMain.TabIndex = 1;
            // 
            // _menuItemGame
            // 
            this._menuItemGame.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemNew,
            this._menuItemLoadFen,
            this._menuItemCopyFen,
            this._menuItemLoadPgn});
            this._menuItemGame.Name = "_menuItemGame";
            this._menuItemGame.Size = new System.Drawing.Size(63, 25);
            this._menuItemGame.Text = "Game";
            // 
            // _menuItemNew
            // 
            this._menuItemNew.Name = "_menuItemNew";
            this._menuItemNew.ShortcutKeyDisplayString = "N";
            this._menuItemNew.Size = new System.Drawing.Size(291, 26);
            this._menuItemNew.Tag = "";
            this._menuItemNew.Text = "New";
            this._menuItemNew.Click += new System.EventHandler(this.OnMenuItemNewClick);
            // 
            // _menuItemLoadFen
            // 
            this._menuItemLoadFen.Name = "_menuItemLoadFen";
            this._menuItemLoadFen.ShortcutKeyDisplayString = "L";
            this._menuItemLoadFen.Size = new System.Drawing.Size(291, 26);
            this._menuItemLoadFen.Tag = "";
            this._menuItemLoadFen.Text = "Load from FEN";
            this._menuItemLoadFen.Click += new System.EventHandler(this.OnMenuItemLoadFenClick);
            // 
            // _menuItemCopyFen
            // 
            this._menuItemCopyFen.Name = "_menuItemCopyFen";
            this._menuItemCopyFen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this._menuItemCopyFen.Size = new System.Drawing.Size(291, 26);
            this._menuItemCopyFen.Text = "Copy FEN To Clipboard";
            this._menuItemCopyFen.Click += new System.EventHandler(this.OnMenuItemCopyFenClick);
            // 
            // _menuItemLoadPgn
            // 
            this._menuItemLoadPgn.Name = "_menuItemLoadPgn";
            this._menuItemLoadPgn.Size = new System.Drawing.Size(291, 26);
            this._menuItemLoadPgn.Text = "Load from PGN";
            this._menuItemLoadPgn.Visible = false;
            this._menuItemLoadPgn.Click += new System.EventHandler(this.OnMenuItemLoadPgnClick);
            // 
            // _menuItemEngine
            // 
            this._menuItemEngine.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemEngineTime,
            this._menuItemMultiPv,
            this._menuItemComputerMove,
            this._menuItemAutoPlay,
            this._menuItemDepthAnalysis,
            this._menuItemCheatMode});
            this._menuItemEngine.Name = "_menuItemEngine";
            this._menuItemEngine.Size = new System.Drawing.Size(69, 25);
            this._menuItemEngine.Text = "Engine";
            // 
            // _menuItemEngineTime
            // 
            this._menuItemEngineTime.Name = "_menuItemEngineTime";
            this._menuItemEngineTime.Size = new System.Drawing.Size(220, 26);
            this._menuItemEngineTime.Text = "Engine Time";
            this._menuItemEngineTime.Click += new System.EventHandler(this.OnMenuItemEngineTimeClick);
            // 
            // _menuItemMultiPv
            // 
            this._menuItemMultiPv.Name = "_menuItemMultiPv";
            this._menuItemMultiPv.Size = new System.Drawing.Size(220, 26);
            this._menuItemMultiPv.Text = "MultiPV";
            this._menuItemMultiPv.Click += new System.EventHandler(this.OnMenuItemMultiPvClick);
            // 
            // _menuItemComputerMove
            // 
            this._menuItemComputerMove.Name = "_menuItemComputerMove";
            this._menuItemComputerMove.ShortcutKeyDisplayString = "C";
            this._menuItemComputerMove.Size = new System.Drawing.Size(220, 26);
            this._menuItemComputerMove.Text = "Play Now";
            this._menuItemComputerMove.Click += new System.EventHandler(this.OnMenuItemComputerMoveClick);
            // 
            // _menuItemAutoPlay
            // 
            this._menuItemAutoPlay.Name = "_menuItemAutoPlay";
            this._menuItemAutoPlay.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._menuItemAutoPlay.Size = new System.Drawing.Size(220, 26);
            this._menuItemAutoPlay.Text = "Auto Play";
            this._menuItemAutoPlay.Click += new System.EventHandler(this.OnMenuItemAutoPlayClick);
            // 
            // _menuItemDepthAnalysis
            // 
            this._menuItemDepthAnalysis.Name = "_menuItemDepthAnalysis";
            this._menuItemDepthAnalysis.Size = new System.Drawing.Size(220, 26);
            this._menuItemDepthAnalysis.Text = "Start Depth Analysis";
            this._menuItemDepthAnalysis.Click += new System.EventHandler(this.OnMenuItemDepthAnalysisClick);
            // 
            // _menuItemCheatMode
            // 
            this._menuItemCheatMode.CheckOnClick = true;
            this._menuItemCheatMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemAnimationTime,
            this._menuItemClickDelay,
            this._menuItemReset,
            this._menuItemUpdate,
            this._menuItemSquareColors,
            this._menuItemAutoMove,
            this._menuItemCheckAuto});
            this._menuItemCheatMode.Name = "_menuItemCheatMode";
            this._menuItemCheatMode.Size = new System.Drawing.Size(220, 26);
            this._menuItemCheatMode.Text = "Cheat Mode";
            this._menuItemCheatMode.CheckedChanged += new System.EventHandler(this.OnMenuItemCheatModeCheckedChanged);
            // 
            // _menuItemAnimationTime
            // 
            this._menuItemAnimationTime.Name = "_menuItemAnimationTime";
            this._menuItemAnimationTime.Size = new System.Drawing.Size(190, 26);
            this._menuItemAnimationTime.Text = "Animation Time";
            this._menuItemAnimationTime.Click += new System.EventHandler(this.OnMenuItemAnimationTimeClick);
            // 
            // _menuItemClickDelay
            // 
            this._menuItemClickDelay.Name = "_menuItemClickDelay";
            this._menuItemClickDelay.Size = new System.Drawing.Size(190, 26);
            this._menuItemClickDelay.Text = "Click Delay";
            this._menuItemClickDelay.Click += new System.EventHandler(this.OnMenuItemClickDelayClick);
            // 
            // _menuItemReset
            // 
            this._menuItemReset.Name = "_menuItemReset";
            this._menuItemReset.Size = new System.Drawing.Size(190, 26);
            this._menuItemReset.Text = "Reset";
            this._menuItemReset.Click += new System.EventHandler(this.OnMenuItemResetClick);
            // 
            // _menuItemUpdate
            // 
            this._menuItemUpdate.Name = "_menuItemUpdate";
            this._menuItemUpdate.Size = new System.Drawing.Size(190, 26);
            this._menuItemUpdate.Text = "Update";
            this._menuItemUpdate.Click += new System.EventHandler(this.OnMenuItemUpdateClick);
            // 
            // _menuItemSquareColors
            // 
            this._menuItemSquareColors.Name = "_menuItemSquareColors";
            this._menuItemSquareColors.Size = new System.Drawing.Size(190, 26);
            this._menuItemSquareColors.Text = "Square Colors";
            this._menuItemSquareColors.Click += new System.EventHandler(this.OnMenuItemSquareColorsClick);
            // 
            // _menuItemAutoMove
            // 
            this._menuItemAutoMove.Name = "_menuItemAutoMove";
            this._menuItemAutoMove.ShortcutKeyDisplayString = "A";
            this._menuItemAutoMove.Size = new System.Drawing.Size(190, 26);
            this._menuItemAutoMove.Text = "Auto Move";
            this._menuItemAutoMove.Click += new System.EventHandler(this.OnMenuItemAutoMoveClick);
            // 
            // _menuItemCheckAuto
            // 
            this._menuItemCheckAuto.CheckOnClick = true;
            this._menuItemCheckAuto.Name = "_menuItemCheckAuto";
            this._menuItemCheckAuto.Size = new System.Drawing.Size(190, 26);
            this._menuItemCheckAuto.Text = "Check Auto";
            // 
            // _menuItemAppearance
            // 
            this._menuItemAppearance.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemAlterPieces,
            this.alterBoardToolStripMenuItem});
            this._menuItemAppearance.Name = "_menuItemAppearance";
            this._menuItemAppearance.Size = new System.Drawing.Size(104, 25);
            this._menuItemAppearance.Text = "Appearance";
            // 
            // _menuItemAlterPieces
            // 
            this._menuItemAlterPieces.Name = "_menuItemAlterPieces";
            this._menuItemAlterPieces.Size = new System.Drawing.Size(123, 26);
            this._menuItemAlterPieces.Text = "Pieces";
            this._menuItemAlterPieces.Click += new System.EventHandler(this.OnMenuItemPiecesClick);
            // 
            // alterBoardToolStripMenuItem
            // 
            this.alterBoardToolStripMenuItem.Name = "alterBoardToolStripMenuItem";
            this.alterBoardToolStripMenuItem.Size = new System.Drawing.Size(123, 26);
            this.alterBoardToolStripMenuItem.Text = "Board";
            this.alterBoardToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemBoardClick);
            // 
            // _menuItemOptions
            // 
            this._menuItemOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemFlipBoard,
            this._menuItemLocalMode,
            this._menuItemHideOutput,
            this._menuItemHideArrows,
            this._menuItemSound});
            this._menuItemOptions.Name = "_menuItemOptions";
            this._menuItemOptions.Size = new System.Drawing.Size(77, 25);
            this._menuItemOptions.Text = "Options";
            // 
            // _menuItemFlipBoard
            // 
            this._menuItemFlipBoard.CheckOnClick = true;
            this._menuItemFlipBoard.Name = "_menuItemFlipBoard";
            this._menuItemFlipBoard.ShortcutKeyDisplayString = "F";
            this._menuItemFlipBoard.Size = new System.Drawing.Size(168, 26);
            this._menuItemFlipBoard.Text = "Flip Board";
            this._menuItemFlipBoard.CheckedChanged += new System.EventHandler(this.OnMenuItemFlipBoardCheckedChanged);
            // 
            // _menuItemLocalMode
            // 
            this._menuItemLocalMode.CheckOnClick = true;
            this._menuItemLocalMode.Name = "_menuItemLocalMode";
            this._menuItemLocalMode.Size = new System.Drawing.Size(168, 26);
            this._menuItemLocalMode.Text = "Local Mode";
            this._menuItemLocalMode.CheckedChanged += new System.EventHandler(this.OnMenuItemLocalModeCheckedChanged);
            // 
            // _menuItemHideOutput
            // 
            this._menuItemHideOutput.CheckOnClick = true;
            this._menuItemHideOutput.Name = "_menuItemHideOutput";
            this._menuItemHideOutput.Size = new System.Drawing.Size(168, 26);
            this._menuItemHideOutput.Text = "Hide Output";
            this._menuItemHideOutput.CheckedChanged += new System.EventHandler(this.OnMenuItemHideOutputCheckedChanged);
            // 
            // _menuItemHideArrows
            // 
            this._menuItemHideArrows.CheckOnClick = true;
            this._menuItemHideArrows.Name = "_menuItemHideArrows";
            this._menuItemHideArrows.Size = new System.Drawing.Size(168, 26);
            this._menuItemHideArrows.Text = "Hide Arrows";
            this._menuItemHideArrows.CheckedChanged += new System.EventHandler(this.OnMenuItemHideArrowsCheckedChanged);
            // 
            // _menuItemSound
            // 
            this._menuItemSound.CheckOnClick = true;
            this._menuItemSound.Name = "_menuItemSound";
            this._menuItemSound.ShortcutKeyDisplayString = "S";
            this._menuItemSound.Size = new System.Drawing.Size(168, 26);
            this._menuItemSound.Text = "Sound";
            // 
            // _menuItemNavigation
            // 
            this._menuItemNavigation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemGoBack,
            this._menuItemGoForward,
            this._menuItemGoToStart,
            this._menuItemGoToLatest});
            this._menuItemNavigation.Name = "_menuItemNavigation";
            this._menuItemNavigation.Size = new System.Drawing.Size(98, 25);
            this._menuItemNavigation.Text = "Navigation";
            // 
            // _menuItemGoBack
            // 
            this._menuItemGoBack.Name = "_menuItemGoBack";
            this._menuItemGoBack.ShortcutKeyDisplayString = "Left";
            this._menuItemGoBack.Size = new System.Drawing.Size(206, 26);
            this._menuItemGoBack.Text = "Go back";
            this._menuItemGoBack.Click += new System.EventHandler(this.OnMenuItemBackClick);
            // 
            // _menuItemGoForward
            // 
            this._menuItemGoForward.Name = "_menuItemGoForward";
            this._menuItemGoForward.ShortcutKeyDisplayString = "Right";
            this._menuItemGoForward.Size = new System.Drawing.Size(206, 26);
            this._menuItemGoForward.Text = "Go forward";
            this._menuItemGoForward.Click += new System.EventHandler(this.OnMenuItemForwardClick);
            // 
            // _menuItemGoToStart
            // 
            this._menuItemGoToStart.Name = "_menuItemGoToStart";
            this._menuItemGoToStart.ShortcutKeyDisplayString = "Down";
            this._menuItemGoToStart.Size = new System.Drawing.Size(206, 26);
            this._menuItemGoToStart.Text = "Go to start";
            this._menuItemGoToStart.Click += new System.EventHandler(this.OnMenuItemStartClick);
            // 
            // _menuItemGoToLatest
            // 
            this._menuItemGoToLatest.Name = "_menuItemGoToLatest";
            this._menuItemGoToLatest.ShortcutKeyDisplayString = "Up";
            this._menuItemGoToLatest.Size = new System.Drawing.Size(206, 26);
            this._menuItemGoToLatest.Text = "Go to latest";
            this._menuItemGoToLatest.Click += new System.EventHandler(this.OnMenuItemLatestClick);
            // 
            // _labelShowEvaluation
            // 
            this._labelShowEvaluation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._labelShowEvaluation.BackColor = System.Drawing.SystemColors.Control;
            this._labelShowEvaluation.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelShowEvaluation.Location = new System.Drawing.Point(-1, 10);
            this._labelShowEvaluation.Name = "_labelShowEvaluation";
            this._labelShowEvaluation.Size = new System.Drawing.Size(238, 21);
            this._labelShowEvaluation.TabIndex = 2;
            this._labelShowEvaluation.Text = "Evaluation:";
            this._labelShowEvaluation.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _labelEvaluation
            // 
            this._labelEvaluation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._labelEvaluation.BackColor = System.Drawing.SystemColors.Control;
            this._labelEvaluation.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelEvaluation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(0)))));
            this._labelEvaluation.Location = new System.Drawing.Point(3, 31);
            this._labelEvaluation.Name = "_labelEvaluation";
            this._labelEvaluation.Size = new System.Drawing.Size(231, 39);
            this._labelEvaluation.TabIndex = 3;
            this._labelEvaluation.Text = "+0.00";
            this._labelEvaluation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _panelRight
            // 
            this._panelRight.AutoScroll = true;
            this._panelRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelRight.Controls.Add(this._tableLayoutPanelModules);
            this._panelRight.Controls.Add(this._labelEvaluation);
            this._panelRight.Controls.Add(this._labelShowEvaluation);
            this._panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelRight.Location = new System.Drawing.Point(0, 0);
            this._panelRight.Name = "_panelRight";
            this._panelRight.Size = new System.Drawing.Size(238, 498);
            this._panelRight.TabIndex = 4;
            // 
            // _tableLayoutPanelModules
            // 
            this._tableLayoutPanelModules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanelModules.ColumnCount = 1;
            this._tableLayoutPanelModules.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelModules.Controls.Add(this._dataGridViewMoves, 0, 0);
            this._tableLayoutPanelModules.Controls.Add(this._panelEvaluationChart, 0, 2);
            this._tableLayoutPanelModules.Location = new System.Drawing.Point(20, 87);
            this._tableLayoutPanelModules.Margin = new System.Windows.Forms.Padding(0);
            this._tableLayoutPanelModules.Name = "_tableLayoutPanelModules";
            this._tableLayoutPanelModules.RowCount = 3;
            this._tableLayoutPanelModules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.41675F));
            this._tableLayoutPanelModules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.933535F));
            this._tableLayoutPanelModules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.64972F));
            this._tableLayoutPanelModules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableLayoutPanelModules.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableLayoutPanelModules.Size = new System.Drawing.Size(192, 336);
            this._tableLayoutPanelModules.TabIndex = 7;
            this._tableLayoutPanelModules.Resize += new System.EventHandler(this.OnTableLayoutPanelModulesResize);
            // 
            // _dataGridViewMoves
            // 
            this._dataGridViewMoves.AllowUserToAddRows = false;
            this._dataGridViewMoves.AllowUserToDeleteRows = false;
            this._dataGridViewMoves.AllowUserToResizeColumns = false;
            this._dataGridViewMoves.AllowUserToResizeRows = false;
            this._dataGridViewMoves.BackgroundColor = System.Drawing.Color.White;
            this._dataGridViewMoves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridViewMoves.ColumnHeadersVisible = false;
            this._dataGridViewMoves.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._whiteMove,
            this._blackMove});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dataGridViewMoves.DefaultCellStyle = dataGridViewCellStyle3;
            this._dataGridViewMoves.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridViewMoves.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dataGridViewMoves.GridColor = System.Drawing.SystemColors.Control;
            this._dataGridViewMoves.Location = new System.Drawing.Point(3, 3);
            this._dataGridViewMoves.MultiSelect = false;
            this._dataGridViewMoves.Name = "_dataGridViewMoves";
            this._dataGridViewMoves.RowHeadersVisible = false;
            this._dataGridViewMoves.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._dataGridViewMoves.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this._dataGridViewMoves.ShowCellErrors = false;
            this._dataGridViewMoves.ShowEditingIcon = false;
            this._dataGridViewMoves.ShowRowErrors = false;
            this._dataGridViewMoves.Size = new System.Drawing.Size(186, 153);
            this._dataGridViewMoves.TabIndex = 6;
            this._dataGridViewMoves.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.OnDataGridViewMovesCellMouseClick);
            this._dataGridViewMoves.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnDataGridViewMovesKeyDown);
            // 
            // _whiteMove
            // 
            this._whiteMove.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._whiteMove.FillWeight = 50F;
            this._whiteMove.HeaderText = "White Move";
            this._whiteMove.Name = "_whiteMove";
            // 
            // _blackMove
            // 
            this._blackMove.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._blackMove.FillWeight = 50F;
            this._blackMove.HeaderText = "Black Move";
            this._blackMove.Name = "_blackMove";
            // 
            // _panelEvaluationChart
            // 
            this._panelEvaluationChart.BackColor = System.Drawing.Color.White;
            this._panelEvaluationChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelEvaluationChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelEvaluationChart.Location = new System.Drawing.Point(3, 178);
            this._panelEvaluationChart.Name = "_panelEvaluationChart";
            this._panelEvaluationChart.Size = new System.Drawing.Size(186, 155);
            this._panelEvaluationChart.TabIndex = 5;
            this._panelEvaluationChart.Tag = "";
            this._panelEvaluationChart.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPanelEvaluationChartPaint);
            this._panelEvaluationChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnPanelEvaluationChartMouseClick);
            // 
            // _splitContainerMain
            // 
            this._splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this._splitContainerMain.Location = new System.Drawing.Point(0, 34);
            this._splitContainerMain.Name = "_splitContainerMain";
            // 
            // _splitContainerMain.Panel1
            // 
            this._splitContainerMain.Panel1.Controls.Add(this._splitContainerBoard);
            // 
            // _splitContainerMain.Panel2
            // 
            this._splitContainerMain.Panel2.Controls.Add(this._panelRight);
            this._splitContainerMain.Size = new System.Drawing.Size(969, 498);
            this._splitContainerMain.SplitterDistance = 729;
            this._splitContainerMain.SplitterWidth = 2;
            this._splitContainerMain.TabIndex = 5;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 532);
            this.Controls.Add(this._splitContainerMain);
            this.Controls.Add(this._menuStripMain);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this._menuStripMain;
            this.MinimumSize = new System.Drawing.Size(475, 300);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BerldChess Version X";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormMainClosing);
            this.Load += new System.EventHandler(this.OnFormMainLoad);
            this._splitContainerBoard.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerBoard)).EndInit();
            this._splitContainerBoard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewEvaluation)).EndInit();
            this._menuStripMain.ResumeLayout(false);
            this._menuStripMain.PerformLayout();
            this._panelRight.ResumeLayout(false);
            this._tableLayoutPanelModules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewMoves)).EndInit();
            this._splitContainerMain.Panel1.ResumeLayout(false);
            this._splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerMain)).EndInit();
            this._splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitContainerBoard;
        private System.Windows.Forms.Timer _timerValidation;
        private System.Windows.Forms.Timer _timerEngine;
        private System.Windows.Forms.Timer _timerAutoCheck;
        private System.Windows.Forms.MenuStrip _menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem _menuItemGame;
        private System.Windows.Forms.ToolStripMenuItem _menuItemNew;
        private System.Windows.Forms.ToolStripMenuItem _menuItemLoadFen;
        private System.Windows.Forms.ToolStripMenuItem _menuItemEngine;
        private System.Windows.Forms.ToolStripMenuItem _menuItemEngineTime;
        private System.Windows.Forms.ToolStripMenuItem _menuItemMultiPv;
        private System.Windows.Forms.ToolStripMenuItem _menuItemComputerMove;
        private System.Windows.Forms.ToolStripMenuItem _menuItemAutoPlay;
        private System.Windows.Forms.ToolStripMenuItem _menuItemCheatMode;
        private System.Windows.Forms.ToolStripMenuItem _menuItemAppearance;
        private System.Windows.Forms.ToolStripMenuItem _menuItemAlterPieces;
        private System.Windows.Forms.ToolStripMenuItem _menuItemNavigation;
        private System.Windows.Forms.ToolStripMenuItem _menuItemGoBack;
        private System.Windows.Forms.ToolStripMenuItem _menuItemGoForward;
        private System.Windows.Forms.ToolStripMenuItem _menuItemGoToStart;
        private System.Windows.Forms.ToolStripMenuItem _menuItemGoToLatest;
        private System.Windows.Forms.ToolStripMenuItem _menuItemOptions;
        private System.Windows.Forms.ToolStripMenuItem _menuItemFlipBoard;
        private System.Windows.Forms.ToolStripMenuItem _menuItemSound;
        private System.Windows.Forms.ToolStripMenuItem _menuItemHideOutput;
        private System.Windows.Forms.ToolStripMenuItem _menuItemHideArrows;
        private System.Windows.Forms.ToolStripMenuItem _menuItemLocalMode;
        private System.Windows.Forms.ToolStripMenuItem _menuItemReset;
        private System.Windows.Forms.ToolStripMenuItem _menuItemUpdate;
        private System.Windows.Forms.ToolStripMenuItem _menuItemSquareColors;
        private System.Windows.Forms.ToolStripMenuItem _menuItemAutoMove;
        private System.Windows.Forms.ToolStripMenuItem _menuItemCheckAuto;
        private System.Windows.Forms.ToolStripMenuItem _menuItemAnimationTime;
        private System.Windows.Forms.ToolStripMenuItem _menuItemCopyFen;
        private System.Windows.Forms.Label _labelShowEvaluation;
        private System.Windows.Forms.Label _labelEvaluation;
        private System.Windows.Forms.Panel _panelRight;
        private System.Windows.Forms.Panel _panelEvaluationChart;
        private System.Windows.Forms.ToolStripMenuItem _menuItemClickDelay;
        private System.Windows.Forms.DataGridView _dataGridViewMoves;
        private System.Windows.Forms.ToolStripMenuItem _menuItemLoadPgn;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelModules;
        private System.Windows.Forms.DataGridViewTextBoxColumn _whiteMove;
        private System.Windows.Forms.DataGridViewTextBoxColumn _blackMove;
        private System.Windows.Forms.ToolStripMenuItem alterBoardToolStripMenuItem;
        private System.Windows.Forms.SplitContainer _splitContainerMain;
        private System.Windows.Forms.DataGridView _dataGridViewEvaluation;
        private System.Windows.Forms.ToolStripMenuItem _menuItemDepthAnalysis;
    }
}

