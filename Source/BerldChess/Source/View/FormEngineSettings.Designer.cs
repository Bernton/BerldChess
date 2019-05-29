namespace BerldChess.View
{
    partial class FormEngineSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEngineSettings));
            this._listBoxSettings = new System.Windows.Forms.ListBox();
            this._labelArguments = new BerldChess.View.SmoothLabel();
            this._textBoxArguments = new System.Windows.Forms.TextBox();
            this._labelName = new BerldChess.View.SmoothLabel();
            this._textBoxName = new System.Windows.Forms.TextBox();
            this._labelPath = new BerldChess.View.SmoothLabel();
            this._comboBoxPath = new System.Windows.Forms.ComboBox();
            this._buttonAddNew = new System.Windows.Forms.Button();
            this._buttonApply = new System.Windows.Forms.Button();
            this._buttonRemove = new System.Windows.Forms.Button();
            this._radioButtonDisabled = new System.Windows.Forms.RadioButton();
            this._radioButtonAnalysis = new System.Windows.Forms.RadioButton();
            this._radioButtonCompetitive = new System.Windows.Forms.RadioButton();
            this._comboBoxEngine1 = new System.Windows.Forms.ComboBox();
            this._comboBoxEngine2 = new System.Windows.Forms.ComboBox();
            this._labelEngine1 = new BerldChess.View.SmoothLabel();
            this._labelEngine2 = new BerldChess.View.SmoothLabel();
            this._buttonPathDialog = new System.Windows.Forms.Button();
            this._groupBoxProperties = new System.Windows.Forms.GroupBox();
            this._groupBoxEngineMode = new System.Windows.Forms.GroupBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this._groupBoxProperties.SuspendLayout();
            this._groupBoxEngineMode.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // _listBoxSettings
            // 
            this._listBoxSettings.BackColor = System.Drawing.SystemColors.Window;
            this._listBoxSettings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._listBoxSettings.FormattingEnabled = true;
            this._listBoxSettings.ItemHeight = 25;
            this._listBoxSettings.Location = new System.Drawing.Point(14, 30);
            this._listBoxSettings.Margin = new System.Windows.Forms.Padding(0);
            this._listBoxSettings.Name = "_listBoxSettings";
            this._listBoxSettings.Size = new System.Drawing.Size(256, 354);
            this._listBoxSettings.TabIndex = 0;
            this._listBoxSettings.SelectedIndexChanged += new System.EventHandler(this.OnListBoxSettingsSelectedIndexChanged);
            // 
            // _labelArguments
            // 
            this._labelArguments.AutoSize = true;
            this._labelArguments.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this._labelArguments.Location = new System.Drawing.Point(9, 149);
            this._labelArguments.Name = "_labelArguments";
            this._labelArguments.Size = new System.Drawing.Size(81, 20);
            this._labelArguments.TabIndex = 19;
            this._labelArguments.Text = "Arguments";
            // 
            // _textBoxArguments
            // 
            this._textBoxArguments.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxArguments.Location = new System.Drawing.Point(13, 172);
            this._textBoxArguments.Multiline = true;
            this._textBoxArguments.Name = "_textBoxArguments";
            this._textBoxArguments.Size = new System.Drawing.Size(356, 168);
            this._textBoxArguments.TabIndex = 3;
            this._textBoxArguments.Tag = "kqbnrplwvmto";
            // 
            // _labelName
            // 
            this._labelName.AutoSize = true;
            this._labelName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelName.Location = new System.Drawing.Point(10, 24);
            this._labelName.Name = "_labelName";
            this._labelName.Size = new System.Drawing.Size(49, 20);
            this._labelName.TabIndex = 21;
            this._labelName.Text = "Name";
            // 
            // _textBoxName
            // 
            this._textBoxName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxName.Location = new System.Drawing.Point(13, 47);
            this._textBoxName.Name = "_textBoxName";
            this._textBoxName.Size = new System.Drawing.Size(356, 27);
            this._textBoxName.TabIndex = 0;
            // 
            // _labelPath
            // 
            this._labelPath.AutoSize = true;
            this._labelPath.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelPath.Location = new System.Drawing.Point(9, 88);
            this._labelPath.Name = "_labelPath";
            this._labelPath.Size = new System.Drawing.Size(64, 20);
            this._labelPath.TabIndex = 22;
            this._labelPath.Text = "File Path";
            // 
            // _comboBoxPath
            // 
            this._comboBoxPath.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._comboBoxPath.FormattingEnabled = true;
            this._comboBoxPath.Location = new System.Drawing.Point(13, 111);
            this._comboBoxPath.Name = "_comboBoxPath";
            this._comboBoxPath.Size = new System.Drawing.Size(316, 28);
            this._comboBoxPath.TabIndex = 1;
            // 
            // _buttonAddNew
            // 
            this._buttonAddNew.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonAddNew.Location = new System.Drawing.Point(133, 355);
            this._buttonAddNew.Name = "_buttonAddNew";
            this._buttonAddNew.Size = new System.Drawing.Size(115, 30);
            this._buttonAddNew.TabIndex = 5;
            this._buttonAddNew.Text = "Add New";
            this._buttonAddNew.UseVisualStyleBackColor = true;
            this._buttonAddNew.Click += new System.EventHandler(this.OnButtonAddNewClick);
            // 
            // _buttonApply
            // 
            this._buttonApply.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonApply.Location = new System.Drawing.Point(11, 355);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(115, 30);
            this._buttonApply.TabIndex = 4;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _buttonRemove
            // 
            this._buttonRemove.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonRemove.Location = new System.Drawing.Point(254, 355);
            this._buttonRemove.Name = "_buttonRemove";
            this._buttonRemove.Size = new System.Drawing.Size(115, 30);
            this._buttonRemove.TabIndex = 6;
            this._buttonRemove.Text = "Remove";
            this._buttonRemove.UseVisualStyleBackColor = true;
            this._buttonRemove.Click += new System.EventHandler(this.OnButtonRemoveClick);
            // 
            // _radioButtonDisabled
            // 
            this._radioButtonDisabled.AutoSize = true;
            this._radioButtonDisabled.Location = new System.Drawing.Point(35, 34);
            this._radioButtonDisabled.Name = "_radioButtonDisabled";
            this._radioButtonDisabled.Size = new System.Drawing.Size(88, 25);
            this._radioButtonDisabled.TabIndex = 7;
            this._radioButtonDisabled.TabStop = true;
            this._radioButtonDisabled.Text = "Disabled";
            this._radioButtonDisabled.UseVisualStyleBackColor = true;
            this._radioButtonDisabled.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // _radioButtonAnalysis
            // 
            this._radioButtonAnalysis.AutoSize = true;
            this._radioButtonAnalysis.Location = new System.Drawing.Point(35, 65);
            this._radioButtonAnalysis.Name = "_radioButtonAnalysis";
            this._radioButtonAnalysis.Size = new System.Drawing.Size(85, 25);
            this._radioButtonAnalysis.TabIndex = 8;
            this._radioButtonAnalysis.TabStop = true;
            this._radioButtonAnalysis.Text = "Analysis";
            this._radioButtonAnalysis.UseVisualStyleBackColor = true;
            this._radioButtonAnalysis.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // _radioButtonCompetitive
            // 
            this._radioButtonCompetitive.AutoSize = true;
            this._radioButtonCompetitive.Location = new System.Drawing.Point(35, 96);
            this._radioButtonCompetitive.Name = "_radioButtonCompetitive";
            this._radioButtonCompetitive.Size = new System.Drawing.Size(112, 25);
            this._radioButtonCompetitive.TabIndex = 9;
            this._radioButtonCompetitive.TabStop = true;
            this._radioButtonCompetitive.Text = "Competitive";
            this._radioButtonCompetitive.UseVisualStyleBackColor = true;
            this._radioButtonCompetitive.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // _comboBoxEngine1
            // 
            this._comboBoxEngine1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._comboBoxEngine1.FormattingEnabled = true;
            this._comboBoxEngine1.ItemHeight = 21;
            this._comboBoxEngine1.Location = new System.Drawing.Point(300, 42);
            this._comboBoxEngine1.Name = "_comboBoxEngine1";
            this._comboBoxEngine1.Size = new System.Drawing.Size(343, 29);
            this._comboBoxEngine1.TabIndex = 10;
            this._comboBoxEngine1.SelectedIndexChanged += new System.EventHandler(this.OnComboBoxEngine1SelectedIndexChanged);
            // 
            // _comboBoxEngine2
            // 
            this._comboBoxEngine2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._comboBoxEngine2.FormattingEnabled = true;
            this._comboBoxEngine2.ItemHeight = 21;
            this._comboBoxEngine2.Location = new System.Drawing.Point(300, 99);
            this._comboBoxEngine2.Name = "_comboBoxEngine2";
            this._comboBoxEngine2.Size = new System.Drawing.Size(343, 29);
            this._comboBoxEngine2.TabIndex = 11;
            this._comboBoxEngine2.SelectedIndexChanged += new System.EventHandler(this.OnComboBoxEngine2SelectedIndexChanged);
            // 
            // _labelEngine1
            // 
            this._labelEngine1.AutoSize = true;
            this._labelEngine1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelEngine1.Location = new System.Drawing.Point(297, 18);
            this._labelEngine1.Name = "_labelEngine1";
            this._labelEngine1.Size = new System.Drawing.Size(70, 21);
            this._labelEngine1.TabIndex = 32;
            this._labelEngine1.Text = "Engine 1";
            // 
            // _labelEngine2
            // 
            this._labelEngine2.AutoSize = true;
            this._labelEngine2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelEngine2.Location = new System.Drawing.Point(297, 76);
            this._labelEngine2.Name = "_labelEngine2";
            this._labelEngine2.Size = new System.Drawing.Size(70, 21);
            this._labelEngine2.TabIndex = 33;
            this._labelEngine2.Text = "Engine 2";
            // 
            // _buttonPathDialog
            // 
            this._buttonPathDialog.Location = new System.Drawing.Point(337, 110);
            this._buttonPathDialog.Name = "_buttonPathDialog";
            this._buttonPathDialog.Size = new System.Drawing.Size(32, 30);
            this._buttonPathDialog.TabIndex = 2;
            this._buttonPathDialog.Text = "...";
            this._buttonPathDialog.UseVisualStyleBackColor = true;
            this._buttonPathDialog.Click += new System.EventHandler(this.OnButtonPathDialogClick);
            // 
            // _groupBoxProperties
            // 
            this._groupBoxProperties.Controls.Add(this._labelName);
            this._groupBoxProperties.Controls.Add(this._buttonPathDialog);
            this._groupBoxProperties.Controls.Add(this._comboBoxPath);
            this._groupBoxProperties.Controls.Add(this._labelPath);
            this._groupBoxProperties.Controls.Add(this._textBoxName);
            this._groupBoxProperties.Controls.Add(this._textBoxArguments);
            this._groupBoxProperties.Controls.Add(this._labelArguments);
            this._groupBoxProperties.Controls.Add(this._buttonRemove);
            this._groupBoxProperties.Controls.Add(this._buttonApply);
            this._groupBoxProperties.Controls.Add(this._buttonAddNew);
            this._groupBoxProperties.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._groupBoxProperties.Location = new System.Drawing.Point(312, 3);
            this._groupBoxProperties.Name = "_groupBoxProperties";
            this._groupBoxProperties.Size = new System.Drawing.Size(381, 396);
            this._groupBoxProperties.TabIndex = 35;
            this._groupBoxProperties.TabStop = false;
            this._groupBoxProperties.Text = "Properties";
            // 
            // _groupBoxEngineMode
            // 
            this._groupBoxEngineMode.Controls.Add(this._radioButtonDisabled);
            this._groupBoxEngineMode.Controls.Add(this._radioButtonAnalysis);
            this._groupBoxEngineMode.Controls.Add(this._labelEngine2);
            this._groupBoxEngineMode.Controls.Add(this._radioButtonCompetitive);
            this._groupBoxEngineMode.Controls.Add(this._labelEngine1);
            this._groupBoxEngineMode.Controls.Add(this._comboBoxEngine1);
            this._groupBoxEngineMode.Controls.Add(this._comboBoxEngine2);
            this._groupBoxEngineMode.Location = new System.Drawing.Point(12, 403);
            this._groupBoxEngineMode.Name = "_groupBoxEngineMode";
            this._groupBoxEngineMode.Size = new System.Drawing.Size(681, 138);
            this._groupBoxEngineMode.TabIndex = 36;
            this._groupBoxEngineMode.TabStop = false;
            this._groupBoxEngineMode.Text = "Engine Mode";
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this._listBoxSettings);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 3);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(284, 396);
            this.groupBoxSettings.TabIndex = 37;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // FormEngineSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 553);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this._groupBoxEngineMode);
            this.Controls.Add(this._groupBoxProperties);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEngineSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Engine Settings";
            this._groupBoxProperties.ResumeLayout(false);
            this._groupBoxProperties.PerformLayout();
            this._groupBoxEngineMode.ResumeLayout(false);
            this._groupBoxEngineMode.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _listBoxSettings;
        private BerldChess.View.SmoothLabel _labelArguments;
        private System.Windows.Forms.TextBox _textBoxArguments;
        private BerldChess.View.SmoothLabel _labelName;
        private System.Windows.Forms.TextBox _textBoxName;
        private BerldChess.View.SmoothLabel _labelPath;
        private System.Windows.Forms.ComboBox _comboBoxPath;
        private System.Windows.Forms.Button _buttonAddNew;
        private System.Windows.Forms.Button _buttonApply;
        private System.Windows.Forms.Button _buttonRemove;
        private System.Windows.Forms.RadioButton _radioButtonDisabled;
        private System.Windows.Forms.RadioButton _radioButtonAnalysis;
        private System.Windows.Forms.RadioButton _radioButtonCompetitive;
        private System.Windows.Forms.ComboBox _comboBoxEngine1;
        private System.Windows.Forms.ComboBox _comboBoxEngine2;
        private BerldChess.View.SmoothLabel _labelEngine1;
        private BerldChess.View.SmoothLabel _labelEngine2;
        private System.Windows.Forms.Button _buttonPathDialog;
        private System.Windows.Forms.GroupBox _groupBoxProperties;
        private System.Windows.Forms.GroupBox _groupBoxEngineMode;
        private System.Windows.Forms.GroupBox groupBoxSettings;
    }
}