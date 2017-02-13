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
            this._checkBoxDefault = new System.Windows.Forms.CheckBox();
            this._buttonPreview = new System.Windows.Forms.Button();
            this._textBoxFontChars = new System.Windows.Forms.TextBox();
            this._labelFontChars = new System.Windows.Forms.Label();
            this._textBoxSizeFactor = new System.Windows.Forms.TextBox();
            this._labelSize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _checkBoxUnicodeFont
            // 
            this._checkBoxUnicodeFont.AutoSize = true;
            this._checkBoxUnicodeFont.Location = new System.Drawing.Point(12, 80);
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
            this._comboBoxFont.Location = new System.Drawing.Point(12, 53);
            this._comboBoxFont.Name = "_comboBoxFont";
            this._comboBoxFont.Size = new System.Drawing.Size(259, 21);
            this._comboBoxFont.TabIndex = 3;
            this._comboBoxFont.TextChanged += new System.EventHandler(this.OnComboBoxFontTextChanged);
            // 
            // _checkBoxDefault
            // 
            this._checkBoxDefault.AutoSize = true;
            this._checkBoxDefault.Location = new System.Drawing.Point(12, 12);
            this._checkBoxDefault.Name = "_checkBoxDefault";
            this._checkBoxDefault.Size = new System.Drawing.Size(82, 17);
            this._checkBoxDefault.TabIndex = 4;
            this._checkBoxDefault.Text = "Use Default";
            this._checkBoxDefault.UseVisualStyleBackColor = true;
            this._checkBoxDefault.CheckedChanged += new System.EventHandler(this.OnCheckBoxDefaultCheckedChanged);
            // 
            // _buttonPreview
            // 
            this._buttonPreview.Location = new System.Drawing.Point(12, 192);
            this._buttonPreview.Name = "_buttonPreview";
            this._buttonPreview.Size = new System.Drawing.Size(101, 29);
            this._buttonPreview.TabIndex = 5;
            this._buttonPreview.Text = "Apply";
            this._buttonPreview.UseVisualStyleBackColor = true;
            this._buttonPreview.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _textBoxFontChars
            // 
            this._textBoxFontChars.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxFontChars.Location = new System.Drawing.Point(12, 135);
            this._textBoxFontChars.Name = "_textBoxFontChars";
            this._textBoxFontChars.Size = new System.Drawing.Size(259, 35);
            this._textBoxFontChars.TabIndex = 6;
            this._textBoxFontChars.Tag = "kqbnrplwvmto";
            this._textBoxFontChars.Text = "kqbnrplwvmto";
            // 
            // _labelFontChars
            // 
            this._labelFontChars.AutoSize = true;
            this._labelFontChars.Location = new System.Drawing.Point(9, 119);
            this._labelFontChars.Name = "_labelFontChars";
            this._labelFontChars.Size = new System.Drawing.Size(266, 13);
            this._labelFontChars.TabIndex = 7;
            this._labelFontChars.Text = "Chess Font Characters (Order: K,Q,B,N,R,P,k,q,b,n,r,p)";
            // 
            // _textBoxSizeFactor
            // 
            this._textBoxSizeFactor.Location = new System.Drawing.Point(196, 10);
            this._textBoxSizeFactor.Name = "_textBoxSizeFactor";
            this._textBoxSizeFactor.Size = new System.Drawing.Size(79, 20);
            this._textBoxSizeFactor.TabIndex = 8;
            this._textBoxSizeFactor.Text = "0.9";
            // 
            // _labelSize
            // 
            this._labelSize.AutoSize = true;
            this._labelSize.Location = new System.Drawing.Point(163, 13);
            this._labelSize.Name = "_labelSize";
            this._labelSize.Size = new System.Drawing.Size(30, 13);
            this._labelSize.TabIndex = 9;
            this._labelSize.Text = "Size:";
            // 
            // PieceSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 233);
            this.Controls.Add(this._labelSize);
            this.Controls.Add(this._textBoxSizeFactor);
            this.Controls.Add(this._labelFontChars);
            this.Controls.Add(this._textBoxFontChars);
            this.Controls.Add(this._buttonPreview);
            this.Controls.Add(this._checkBoxDefault);
            this.Controls.Add(this._comboBoxFont);
            this.Controls.Add(this._checkBoxUnicodeFont);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PieceSelectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alter Piece Design";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _checkBoxUnicodeFont;
        private System.Windows.Forms.ComboBox _comboBoxFont;
        private System.Windows.Forms.CheckBox _checkBoxDefault;
        private System.Windows.Forms.Button _buttonPreview;
        private System.Windows.Forms.TextBox _textBoxFontChars;
        private System.Windows.Forms.Label _labelFontChars;
        private System.Windows.Forms.TextBox _textBoxSizeFactor;
        private System.Windows.Forms.Label _labelSize;
    }
}