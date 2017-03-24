namespace BerldChess.View
{
    partial class PieceSelectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PieceSelectionDialog));
            this._checkBoxUnicodeFont = new System.Windows.Forms.CheckBox();
            this._comboBoxFont = new System.Windows.Forms.ComboBox();
            this._buttonApply = new System.Windows.Forms.Button();
            this._textBoxFontChars = new System.Windows.Forms.TextBox();
            this._labelFontChars = new System.Windows.Forms.Label();
            this._textBoxSizeFactor = new System.Windows.Forms.TextBox();
            this._labelSize = new System.Windows.Forms.Label();
            this._buttonRemove = new System.Windows.Forms.Button();
            this._labelConfigs = new System.Windows.Forms.Label();
            this._listBoxConfigs = new System.Windows.Forms.ListBox();
            this._textBoxName = new System.Windows.Forms.TextBox();
            this._labelName = new System.Windows.Forms.Label();
            this._buttonAddNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _checkBoxUnicodeFont
            // 
            this._checkBoxUnicodeFont.AutoSize = true;
            this._checkBoxUnicodeFont.Location = new System.Drawing.Point(210, 84);
            this._checkBoxUnicodeFont.Name = "_checkBoxUnicodeFont";
            this._checkBoxUnicodeFont.Size = new System.Drawing.Size(101, 17);
            this._checkBoxUnicodeFont.TabIndex = 0;
            this._checkBoxUnicodeFont.Text = "Is Unicode Font";
            this._checkBoxUnicodeFont.UseVisualStyleBackColor = true;
            this._checkBoxUnicodeFont.CheckedChanged += new System.EventHandler(this.OnCheckBoxUnicodeFontCheckedChanged);
            // 
            // _comboBoxFont
            // 
            this._comboBoxFont.FormattingEnabled = true;
            this._comboBoxFont.Location = new System.Drawing.Point(209, 56);
            this._comboBoxFont.Name = "_comboBoxFont";
            this._comboBoxFont.Size = new System.Drawing.Size(259, 21);
            this._comboBoxFont.TabIndex = 3;
            this._comboBoxFont.TextChanged += new System.EventHandler(this.OnComboBoxFontTextChanged);
            // 
            // _buttonApply
            // 
            this._buttonApply.Location = new System.Drawing.Point(210, 196);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(89, 29);
            this._buttonApply.TabIndex = 5;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _textBoxFontChars
            // 
            this._textBoxFontChars.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxFontChars.Location = new System.Drawing.Point(210, 139);
            this._textBoxFontChars.Multiline = true;
            this._textBoxFontChars.Name = "_textBoxFontChars";
            this._textBoxFontChars.Size = new System.Drawing.Size(259, 39);
            this._textBoxFontChars.TabIndex = 6;
            this._textBoxFontChars.Tag = "kqbnrplwvmto";
            this._textBoxFontChars.Text = "kqbnrplwvmto";
            this._textBoxFontChars.WordWrap = false;
            // 
            // _labelFontChars
            // 
            this._labelFontChars.AutoSize = true;
            this._labelFontChars.Location = new System.Drawing.Point(207, 123);
            this._labelFontChars.Name = "_labelFontChars";
            this._labelFontChars.Size = new System.Drawing.Size(266, 13);
            this._labelFontChars.TabIndex = 7;
            this._labelFontChars.Text = "Chess Font Characters (Order: K,Q,B,N,R,P,k,q,b,n,r,p)";
            // 
            // _textBoxSizeFactor
            // 
            this._textBoxSizeFactor.Location = new System.Drawing.Point(390, 81);
            this._textBoxSizeFactor.Name = "_textBoxSizeFactor";
            this._textBoxSizeFactor.Size = new System.Drawing.Size(79, 20);
            this._textBoxSizeFactor.TabIndex = 8;
            this._textBoxSizeFactor.Text = "0.9";
            // 
            // _labelSize
            // 
            this._labelSize.AutoSize = true;
            this._labelSize.Location = new System.Drawing.Point(357, 84);
            this._labelSize.Name = "_labelSize";
            this._labelSize.Size = new System.Drawing.Size(30, 13);
            this._labelSize.TabIndex = 9;
            this._labelSize.Text = "Size:";
            // 
            // _buttonRemove
            // 
            this._buttonRemove.Location = new System.Drawing.Point(390, 196);
            this._buttonRemove.Name = "_buttonRemove";
            this._buttonRemove.Size = new System.Drawing.Size(79, 29);
            this._buttonRemove.TabIndex = 10;
            this._buttonRemove.Text = "Remove";
            this._buttonRemove.UseVisualStyleBackColor = true;
            this._buttonRemove.Click += new System.EventHandler(this.OnButtonCloseClick);
            // 
            // _labelConfigs
            // 
            this._labelConfigs.AutoSize = true;
            this._labelConfigs.Location = new System.Drawing.Point(12, 18);
            this._labelConfigs.Name = "_labelConfigs";
            this._labelConfigs.Size = new System.Drawing.Size(98, 13);
            this._labelConfigs.TabIndex = 11;
            this._labelConfigs.Text = "Font Configurations";
            // 
            // _listBoxConfigs
            // 
            this._listBoxConfigs.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._listBoxConfigs.FormattingEnabled = true;
            this._listBoxConfigs.ItemHeight = 18;
            this._listBoxConfigs.Location = new System.Drawing.Point(12, 39);
            this._listBoxConfigs.Name = "_listBoxConfigs";
            this._listBoxConfigs.Size = new System.Drawing.Size(182, 184);
            this._listBoxConfigs.TabIndex = 12;
            this._listBoxConfigs.SelectedIndexChanged += new System.EventHandler(this.OnListBoxConfigsSelectedIndexChanged);
            // 
            // _textBoxName
            // 
            this._textBoxName.Location = new System.Drawing.Point(328, 30);
            this._textBoxName.Name = "_textBoxName";
            this._textBoxName.Size = new System.Drawing.Size(139, 20);
            this._textBoxName.TabIndex = 13;
            // 
            // _labelName
            // 
            this._labelName.AutoSize = true;
            this._labelName.Location = new System.Drawing.Point(207, 31);
            this._labelName.Name = "_labelName";
            this._labelName.Size = new System.Drawing.Size(115, 13);
            this._labelName.TabIndex = 14;
            this._labelName.Text = "Name of Configuration:";
            // 
            // _buttonAddNew
            // 
            this._buttonAddNew.Location = new System.Drawing.Point(305, 196);
            this._buttonAddNew.Name = "_buttonAddNew";
            this._buttonAddNew.Size = new System.Drawing.Size(79, 29);
            this._buttonAddNew.TabIndex = 15;
            this._buttonAddNew.Text = "Add New";
            this._buttonAddNew.UseVisualStyleBackColor = true;
            this._buttonAddNew.Click += new System.EventHandler(this.OnButtonAddNewClick);
            // 
            // PieceSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 241);
            this.Controls.Add(this._buttonAddNew);
            this.Controls.Add(this._labelName);
            this.Controls.Add(this._textBoxName);
            this.Controls.Add(this._listBoxConfigs);
            this.Controls.Add(this._labelConfigs);
            this.Controls.Add(this._buttonRemove);
            this.Controls.Add(this._labelSize);
            this.Controls.Add(this._textBoxSizeFactor);
            this.Controls.Add(this._labelFontChars);
            this.Controls.Add(this._textBoxFontChars);
            this.Controls.Add(this._buttonApply);
            this.Controls.Add(this._comboBoxFont);
            this.Controls.Add(this._checkBoxUnicodeFont);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PieceSelectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Piece Settings";
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
        private System.Windows.Forms.ListBox _listBoxConfigs;
        private System.Windows.Forms.TextBox _textBoxName;
        private System.Windows.Forms.Label _labelName;
        private System.Windows.Forms.Button _buttonAddNew;
    }
}