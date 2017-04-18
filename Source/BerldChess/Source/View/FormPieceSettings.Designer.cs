namespace BerldChess.View
{
    partial class FormPieceSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPieceSettings));
            this._checkBoxUnicodeFont = new System.Windows.Forms.CheckBox();
            this._comboBoxFont = new System.Windows.Forms.ComboBox();
            this._buttonApply = new System.Windows.Forms.Button();
            this._textBoxFontChars = new System.Windows.Forms.TextBox();
            this._labelFontChars = new System.Windows.Forms.Label();
            this._textBoxSizeFactor = new System.Windows.Forms.TextBox();
            this._labelSize = new System.Windows.Forms.Label();
            this._buttonRemove = new System.Windows.Forms.Button();
            this._labelConfigs = new System.Windows.Forms.Label();
            this._listBoxSettings = new System.Windows.Forms.ListBox();
            this._textBoxName = new System.Windows.Forms.TextBox();
            this._labelName = new System.Windows.Forms.Label();
            this._buttonAddNew = new System.Windows.Forms.Button();
            this.labelFontFamily = new System.Windows.Forms.Label();
            this._panelSettings = new System.Windows.Forms.Panel();
            this._panelFontSettings = new System.Windows.Forms.Panel();
            this._panelSettings.SuspendLayout();
            this._panelFontSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // _checkBoxUnicodeFont
            // 
            this._checkBoxUnicodeFont.AutoSize = true;
            this._checkBoxUnicodeFont.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkBoxUnicodeFont.Location = new System.Drawing.Point(8, 70);
            this._checkBoxUnicodeFont.Name = "_checkBoxUnicodeFont";
            this._checkBoxUnicodeFont.Size = new System.Drawing.Size(130, 24);
            this._checkBoxUnicodeFont.TabIndex = 0;
            this._checkBoxUnicodeFont.Text = "Is Unicode Font";
            this._checkBoxUnicodeFont.UseVisualStyleBackColor = true;
            this._checkBoxUnicodeFont.CheckedChanged += new System.EventHandler(this.OnCheckBoxUnicodeFontCheckedChanged);
            // 
            // _comboBoxFont
            // 
            this._comboBoxFont.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._comboBoxFont.FormattingEnabled = true;
            this._comboBoxFont.Location = new System.Drawing.Point(8, 36);
            this._comboBoxFont.Name = "_comboBoxFont";
            this._comboBoxFont.Size = new System.Drawing.Size(306, 28);
            this._comboBoxFont.TabIndex = 3;
            this._comboBoxFont.TextChanged += new System.EventHandler(this.OnComboBoxFontTextChanged);
            // 
            // _buttonApply
            // 
            this._buttonApply.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonApply.Location = new System.Drawing.Point(11, 310);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(100, 30);
            this._buttonApply.TabIndex = 5;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _textBoxFontChars
            // 
            this._textBoxFontChars.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxFontChars.Location = new System.Drawing.Point(8, 124);
            this._textBoxFontChars.Multiline = true;
            this._textBoxFontChars.Name = "_textBoxFontChars";
            this._textBoxFontChars.Size = new System.Drawing.Size(312, 91);
            this._textBoxFontChars.TabIndex = 6;
            this._textBoxFontChars.Tag = "kqbnrplwvmto";
            this._textBoxFontChars.Text = "kqbnrplwvmto";
            // 
            // _labelFontChars
            // 
            this._labelFontChars.AutoSize = true;
            this._labelFontChars.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelFontChars.Location = new System.Drawing.Point(5, 104);
            this._labelFontChars.Name = "_labelFontChars";
            this._labelFontChars.Size = new System.Drawing.Size(316, 17);
            this._labelFontChars.TabIndex = 7;
            this._labelFontChars.Text = "Chess Font Characters (Order: K,Q,B,N,R,P,k,q,b,n,r,p)";
            // 
            // _textBoxSizeFactor
            // 
            this._textBoxSizeFactor.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxSizeFactor.Location = new System.Drawing.Point(198, 43);
            this._textBoxSizeFactor.Name = "_textBoxSizeFactor";
            this._textBoxSizeFactor.Size = new System.Drawing.Size(58, 27);
            this._textBoxSizeFactor.TabIndex = 8;
            this._textBoxSizeFactor.Text = "0.9";
            // 
            // _labelSize
            // 
            this._labelSize.AutoSize = true;
            this._labelSize.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelSize.Location = new System.Drawing.Point(7, 46);
            this._labelSize.Name = "_labelSize";
            this._labelSize.Size = new System.Drawing.Size(39, 20);
            this._labelSize.TabIndex = 9;
            this._labelSize.Text = "Size:";
            // 
            // _buttonRemove
            // 
            this._buttonRemove.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonRemove.Location = new System.Drawing.Point(223, 310);
            this._buttonRemove.Name = "_buttonRemove";
            this._buttonRemove.Size = new System.Drawing.Size(100, 30);
            this._buttonRemove.TabIndex = 10;
            this._buttonRemove.Text = "Remove";
            this._buttonRemove.UseVisualStyleBackColor = true;
            this._buttonRemove.Click += new System.EventHandler(this.OnButtonRemoveClick);
            // 
            // _labelConfigs
            // 
            this._labelConfigs.AutoSize = true;
            this._labelConfigs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelConfigs.Location = new System.Drawing.Point(8, 15);
            this._labelConfigs.Name = "_labelConfigs";
            this._labelConfigs.Size = new System.Drawing.Size(104, 21);
            this._labelConfigs.TabIndex = 11;
            this._labelConfigs.Text = "Font Settings:";
            // 
            // _listBoxSettings
            // 
            this._listBoxSettings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._listBoxSettings.FormattingEnabled = true;
            this._listBoxSettings.ItemHeight = 25;
            this._listBoxSettings.Location = new System.Drawing.Point(12, 43);
            this._listBoxSettings.Name = "_listBoxSettings";
            this._listBoxSettings.Size = new System.Drawing.Size(239, 329);
            this._listBoxSettings.TabIndex = 12;
            this._listBoxSettings.SelectedIndexChanged += new System.EventHandler(this.OnListBoxConfigsSelectedIndexChanged);
            // 
            // _textBoxName
            // 
            this._textBoxName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxName.Location = new System.Drawing.Point(198, 10);
            this._textBoxName.Name = "_textBoxName";
            this._textBoxName.Size = new System.Drawing.Size(119, 27);
            this._textBoxName.TabIndex = 13;
            this._textBoxName.Text = "Default";
            // 
            // _labelName
            // 
            this._labelName.AutoSize = true;
            this._labelName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelName.Location = new System.Drawing.Point(7, 13);
            this._labelName.Name = "_labelName";
            this._labelName.Size = new System.Drawing.Size(127, 20);
            this._labelName.TabIndex = 14;
            this._labelName.Text = "Name of Settings:";
            // 
            // _buttonAddNew
            // 
            this._buttonAddNew.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonAddNew.Location = new System.Drawing.Point(117, 310);
            this._buttonAddNew.Name = "_buttonAddNew";
            this._buttonAddNew.Size = new System.Drawing.Size(100, 30);
            this._buttonAddNew.TabIndex = 15;
            this._buttonAddNew.Text = "Add New";
            this._buttonAddNew.UseVisualStyleBackColor = true;
            this._buttonAddNew.Click += new System.EventHandler(this.OnButtonAddNewClick);
            // 
            // labelFontFamily
            // 
            this.labelFontFamily.AutoSize = true;
            this.labelFontFamily.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFontFamily.Location = new System.Drawing.Point(5, 16);
            this.labelFontFamily.Name = "labelFontFamily";
            this.labelFontFamily.Size = new System.Drawing.Size(73, 17);
            this.labelFontFamily.TabIndex = 16;
            this.labelFontFamily.Text = "Font Family";
            // 
            // _panelSettings
            // 
            this._panelSettings.Controls.Add(this._panelFontSettings);
            this._panelSettings.Controls.Add(this._labelName);
            this._panelSettings.Controls.Add(this._buttonAddNew);
            this._panelSettings.Controls.Add(this._buttonApply);
            this._panelSettings.Controls.Add(this._textBoxName);
            this._panelSettings.Controls.Add(this._textBoxSizeFactor);
            this._panelSettings.Controls.Add(this._buttonRemove);
            this._panelSettings.Controls.Add(this._labelSize);
            this._panelSettings.Location = new System.Drawing.Point(257, 31);
            this._panelSettings.Name = "_panelSettings";
            this._panelSettings.Size = new System.Drawing.Size(338, 353);
            this._panelSettings.TabIndex = 17;
            // 
            // _panelFontSettings
            // 
            this._panelFontSettings.Controls.Add(this.labelFontFamily);
            this._panelFontSettings.Controls.Add(this._labelFontChars);
            this._panelFontSettings.Controls.Add(this._checkBoxUnicodeFont);
            this._panelFontSettings.Controls.Add(this._textBoxFontChars);
            this._panelFontSettings.Controls.Add(this._comboBoxFont);
            this._panelFontSettings.Location = new System.Drawing.Point(0, 76);
            this._panelFontSettings.Name = "_panelFontSettings";
            this._panelFontSettings.Size = new System.Drawing.Size(338, 228);
            this._panelFontSettings.TabIndex = 18;
            // 
            // FormPieceSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 384);
            this.Controls.Add(this._panelSettings);
            this.Controls.Add(this._listBoxSettings);
            this.Controls.Add(this._labelConfigs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPieceSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Piece Settings";
            this._panelSettings.ResumeLayout(false);
            this._panelSettings.PerformLayout();
            this._panelFontSettings.ResumeLayout(false);
            this._panelFontSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _checkBoxUnicodeFont;
        private System.Windows.Forms.ComboBox _comboBoxFont;
        private System.Windows.Forms.Button _buttonApply;
        private System.Windows.Forms.TextBox _textBoxFontChars;
        private System.Windows.Forms.Label _labelFontChars;
        private System.Windows.Forms.TextBox _textBoxSizeFactor;
        private System.Windows.Forms.Label _labelSize;
        private System.Windows.Forms.Button _buttonRemove;
        private System.Windows.Forms.Label _labelConfigs;
        private System.Windows.Forms.ListBox _listBoxSettings;
        private System.Windows.Forms.TextBox _textBoxName;
        private System.Windows.Forms.Label _labelName;
        private System.Windows.Forms.Button _buttonAddNew;
        private System.Windows.Forms.Label labelFontFamily;
        private System.Windows.Forms.Panel _panelSettings;
        private System.Windows.Forms.Panel _panelFontSettings;
    }
}