namespace BerldChess.View
{
    partial class FormBoardSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBoardSetting));
            this._checkBoxGridBorder = new System.Windows.Forms.CheckBox();
            this._labelDarkSquare = new System.Windows.Forms.Label();
            this._labelLightSquare = new System.Windows.Forms.Label();
            this._buttonDarkSquare = new System.Windows.Forms.Button();
            this._buttonLightSquare = new System.Windows.Forms.Button();
            this._buttonApply = new System.Windows.Forms.Button();
            this._checkBoxDarkMode = new System.Windows.Forms.CheckBox();
            this._checkBoxGradient = new System.Windows.Forms.CheckBox();
            this._checkBoxHighlightBorder = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _checkBoxGridBorder
            // 
            this._checkBoxGridBorder.AutoSize = true;
            this._checkBoxGridBorder.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkBoxGridBorder.Location = new System.Drawing.Point(164, 120);
            this._checkBoxGridBorder.Name = "_checkBoxGridBorder";
            this._checkBoxGridBorder.Size = new System.Drawing.Size(105, 24);
            this._checkBoxGridBorder.TabIndex = 0;
            this._checkBoxGridBorder.Text = "Grid Border";
            this._checkBoxGridBorder.UseVisualStyleBackColor = true;
            this._checkBoxGridBorder.CheckedChanged += new System.EventHandler(this.OnCheckBoxGridBorderCheckedChanged);
            // 
            // _labelDarkSquare
            // 
            this._labelDarkSquare.AutoSize = true;
            this._labelDarkSquare.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelDarkSquare.Location = new System.Drawing.Point(19, 22);
            this._labelDarkSquare.Name = "_labelDarkSquare";
            this._labelDarkSquare.Size = new System.Drawing.Size(133, 20);
            this._labelDarkSquare.TabIndex = 14;
            this._labelDarkSquare.Text = "Dark Square Color:";
            // 
            // _labelLightSquare
            // 
            this._labelLightSquare.AutoSize = true;
            this._labelLightSquare.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelLightSquare.Location = new System.Drawing.Point(19, 75);
            this._labelLightSquare.Name = "_labelLightSquare";
            this._labelLightSquare.Size = new System.Drawing.Size(135, 20);
            this._labelLightSquare.TabIndex = 17;
            this._labelLightSquare.Text = "Light Square Color:";
            // 
            // _buttonDarkSquare
            // 
            this._buttonDarkSquare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonDarkSquare.Location = new System.Drawing.Point(254, 16);
            this._buttonDarkSquare.Name = "_buttonDarkSquare";
            this._buttonDarkSquare.Size = new System.Drawing.Size(35, 35);
            this._buttonDarkSquare.TabIndex = 18;
            this._buttonDarkSquare.UseVisualStyleBackColor = true;
            this._buttonDarkSquare.Click += new System.EventHandler(this.OnButtonDarkSquareClick);
            // 
            // _buttonLightSquare
            // 
            this._buttonLightSquare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonLightSquare.Location = new System.Drawing.Point(254, 69);
            this._buttonLightSquare.Name = "_buttonLightSquare";
            this._buttonLightSquare.Size = new System.Drawing.Size(35, 35);
            this._buttonLightSquare.TabIndex = 19;
            this._buttonLightSquare.UseVisualStyleBackColor = true;
            this._buttonLightSquare.Click += new System.EventHandler(this.OnButtonLightSquareClick);
            // 
            // _buttonApply
            // 
            this._buttonApply.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonApply.Location = new System.Drawing.Point(12, 190);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(129, 33);
            this._buttonApply.TabIndex = 20;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = true;
            this._buttonApply.Click += new System.EventHandler(this.OnButtonApplyClick);
            // 
            // _checkBoxDarkMode
            // 
            this._checkBoxDarkMode.AutoSize = true;
            this._checkBoxDarkMode.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkBoxDarkMode.Location = new System.Drawing.Point(164, 146);
            this._checkBoxDarkMode.Name = "_checkBoxDarkMode";
            this._checkBoxDarkMode.Size = new System.Drawing.Size(102, 24);
            this._checkBoxDarkMode.TabIndex = 21;
            this._checkBoxDarkMode.Text = "Dark Mode";
            this._checkBoxDarkMode.UseVisualStyleBackColor = true;
            this._checkBoxDarkMode.CheckedChanged += new System.EventHandler(this.OnCheckBoxDarkModeCheckedChanged);
            // 
            // _checkBoxGradient
            // 
            this._checkBoxGradient.AutoSize = true;
            this._checkBoxGradient.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkBoxGradient.Location = new System.Drawing.Point(164, 172);
            this._checkBoxGradient.Name = "_checkBoxGradient";
            this._checkBoxGradient.Size = new System.Drawing.Size(85, 24);
            this._checkBoxGradient.TabIndex = 22;
            this._checkBoxGradient.Text = "Gradient";
            this._checkBoxGradient.UseVisualStyleBackColor = true;
            this._checkBoxGradient.CheckedChanged += new System.EventHandler(this.OnCheckBoxGradientCheckedChanged);
            // 
            // _checkBoxHighlightBorder
            // 
            this._checkBoxHighlightBorder.AutoSize = true;
            this._checkBoxHighlightBorder.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkBoxHighlightBorder.Location = new System.Drawing.Point(164, 199);
            this._checkBoxHighlightBorder.Name = "_checkBoxHighlightBorder";
            this._checkBoxHighlightBorder.Size = new System.Drawing.Size(139, 24);
            this._checkBoxHighlightBorder.TabIndex = 23;
            this._checkBoxHighlightBorder.Text = "Border Highlight";
            this._checkBoxHighlightBorder.UseVisualStyleBackColor = true;
            this._checkBoxHighlightBorder.CheckedChanged += new System.EventHandler(this.OnCheckBoxHighlightBorderCheckedChanged);
            // 
            // BoardSettingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 237);
            this.Controls.Add(this._checkBoxHighlightBorder);
            this.Controls.Add(this._checkBoxGradient);
            this.Controls.Add(this._checkBoxDarkMode);
            this.Controls.Add(this._buttonApply);
            this.Controls.Add(this._buttonLightSquare);
            this.Controls.Add(this._buttonDarkSquare);
            this.Controls.Add(this._labelLightSquare);
            this.Controls.Add(this._labelDarkSquare);
            this.Controls.Add(this._checkBoxGridBorder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BoardSettingDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Board Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnBoardSettingDialogFormClosing);
            this.Shown += new System.EventHandler(this.OnBoardSettingDialogShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _checkBoxGridBorder;
        private System.Windows.Forms.Label _labelDarkSquare;
        private System.Windows.Forms.Label _labelLightSquare;
        private System.Windows.Forms.Button _buttonDarkSquare;
        private System.Windows.Forms.Button _buttonLightSquare;
        private System.Windows.Forms.Button _buttonApply;
        private System.Windows.Forms.CheckBox _checkBoxDarkMode;
        private System.Windows.Forms.CheckBox _checkBoxGradient;
        private System.Windows.Forms.CheckBox _checkBoxHighlightBorder;
    }
}