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
            this._labelSettings = new System.Windows.Forms.Label();
            this._labelArguments = new System.Windows.Forms.Label();
            this._textBoxArguments = new System.Windows.Forms.TextBox();
            this._labelName = new System.Windows.Forms.Label();
            this._textBoxName = new System.Windows.Forms.TextBox();
            this._labelPath = new System.Windows.Forms.Label();
            this._comboBoxPath = new System.Windows.Forms.ComboBox();
            this._buttonAddNew = new System.Windows.Forms.Button();
            this._buttonApply = new System.Windows.Forms.Button();
            this._buttonRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _listBoxSettings
            // 
            this._listBoxSettings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._listBoxSettings.FormattingEnabled = true;
            this._listBoxSettings.ItemHeight = 25;
            this._listBoxSettings.Location = new System.Drawing.Point(13, 43);
            this._listBoxSettings.Name = "_listBoxSettings";
            this._listBoxSettings.Size = new System.Drawing.Size(239, 329);
            this._listBoxSettings.TabIndex = 14;
            this._listBoxSettings.SelectedIndexChanged += new System.EventHandler(this.OnListBoxSettingsSelectedIndexChanged);
            // 
            // _labelSettings
            // 
            this._labelSettings.AutoSize = true;
            this._labelSettings.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelSettings.Location = new System.Drawing.Point(9, 15);
            this._labelSettings.Name = "_labelSettings";
            this._labelSettings.Size = new System.Drawing.Size(117, 21);
            this._labelSettings.TabIndex = 13;
            this._labelSettings.Text = "Engine Settings";
            // 
            // _labelArguments
            // 
            this._labelArguments.AutoSize = true;
            this._labelArguments.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this._labelArguments.Location = new System.Drawing.Point(261, 157);
            this._labelArguments.Name = "_labelArguments";
            this._labelArguments.Size = new System.Drawing.Size(81, 20);
            this._labelArguments.TabIndex = 19;
            this._labelArguments.Text = "Arguments";
            // 
            // _textBoxArguments
            // 
            this._textBoxArguments.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxArguments.Location = new System.Drawing.Point(264, 178);
            this._textBoxArguments.Multiline = true;
            this._textBoxArguments.Name = "_textBoxArguments";
            this._textBoxArguments.Size = new System.Drawing.Size(311, 150);
            this._textBoxArguments.TabIndex = 18;
            this._textBoxArguments.Tag = "kqbnrplwvmto";
            // 
            // _labelName
            // 
            this._labelName.AutoSize = true;
            this._labelName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelName.Location = new System.Drawing.Point(261, 46);
            this._labelName.Name = "_labelName";
            this._labelName.Size = new System.Drawing.Size(127, 20);
            this._labelName.TabIndex = 21;
            this._labelName.Text = "Name of Settings:";
            // 
            // _textBoxName
            // 
            this._textBoxName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxName.Location = new System.Drawing.Point(466, 46);
            this._textBoxName.Name = "_textBoxName";
            this._textBoxName.Size = new System.Drawing.Size(109, 27);
            this._textBoxName.TabIndex = 20;
            // 
            // _labelPath
            // 
            this._labelPath.AutoSize = true;
            this._labelPath.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelPath.Location = new System.Drawing.Point(261, 87);
            this._labelPath.Name = "_labelPath";
            this._labelPath.Size = new System.Drawing.Size(131, 20);
            this._labelPath.TabIndex = 22;
            this._labelPath.Text = "Path to Executable";
            // 
            // _comboBoxPath
            // 
            this._comboBoxPath.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._comboBoxPath.FormattingEnabled = true;
            this._comboBoxPath.Location = new System.Drawing.Point(265, 110);
            this._comboBoxPath.Name = "_comboBoxPath";
            this._comboBoxPath.Size = new System.Drawing.Size(310, 28);
            this._comboBoxPath.TabIndex = 17;
            // 
            // _buttonAddNew
            // 
            this._buttonAddNew.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonAddNew.Location = new System.Drawing.Point(369, 342);
            this._buttonAddNew.Name = "_buttonAddNew";
            this._buttonAddNew.Size = new System.Drawing.Size(100, 30);
            this._buttonAddNew.TabIndex = 25;
            this._buttonAddNew.Text = "Add New";
            this._buttonAddNew.UseVisualStyleBackColor = true;
            this._buttonAddNew.Click += new System.EventHandler(this.OnButtonAddNewClick);
            // 
            // _buttonApply
            // 
            this._buttonApply.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonApply.Location = new System.Drawing.Point(263, 342);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(100, 30);
            this._buttonApply.TabIndex = 23;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _buttonRemove
            // 
            this._buttonRemove.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonRemove.Location = new System.Drawing.Point(475, 342);
            this._buttonRemove.Name = "_buttonRemove";
            this._buttonRemove.Size = new System.Drawing.Size(100, 30);
            this._buttonRemove.TabIndex = 24;
            this._buttonRemove.Text = "Remove";
            this._buttonRemove.UseVisualStyleBackColor = true;
            this._buttonRemove.Click += new System.EventHandler(this.OnButtonRemoveClick);
            // 
            // FormEngineSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 385);
            this.Controls.Add(this._buttonAddNew);
            this.Controls.Add(this._buttonApply);
            this.Controls.Add(this._buttonRemove);
            this.Controls.Add(this._labelArguments);
            this.Controls.Add(this._textBoxArguments);
            this.Controls.Add(this._labelName);
            this.Controls.Add(this._textBoxName);
            this.Controls.Add(this._labelPath);
            this.Controls.Add(this._comboBoxPath);
            this.Controls.Add(this._listBoxSettings);
            this.Controls.Add(this._labelSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEngineSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Engine Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox _listBoxSettings;
        private System.Windows.Forms.Label _labelSettings;
        private System.Windows.Forms.Label _labelArguments;
        private System.Windows.Forms.TextBox _textBoxArguments;
        private System.Windows.Forms.Label _labelName;
        private System.Windows.Forms.TextBox _textBoxName;
        private System.Windows.Forms.Label _labelPath;
        private System.Windows.Forms.ComboBox _comboBoxPath;
        private System.Windows.Forms.Button _buttonAddNew;
        private System.Windows.Forms.Button _buttonApply;
        private System.Windows.Forms.Button _buttonRemove;
    }
}