namespace BerldChess.View
{
    partial class FormPgnLoader
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
            this._buttonLoad = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._textBoxPgnInput = new System.Windows.Forms.TextBox();
            this._labelPgnInput = new System.Windows.Forms.Label();
            this._buttonChooseFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonLoad
            // 
            this._buttonLoad.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonLoad.Location = new System.Drawing.Point(12, 268);
            this._buttonLoad.Name = "_buttonLoad";
            this._buttonLoad.Size = new System.Drawing.Size(150, 40);
            this._buttonLoad.TabIndex = 0;
            this._buttonLoad.Text = "Load";
            this._buttonLoad.UseVisualStyleBackColor = true;
            this._buttonLoad.Click += new System.EventHandler(this.OnButtonLoadClick);
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonCancel.Location = new System.Drawing.Point(312, 268);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(154, 40);
            this._buttonCancel.TabIndex = 1;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this.OnButtonCancelClick);
            // 
            // _textBoxPgnInput
            // 
            this._textBoxPgnInput.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxPgnInput.Location = new System.Drawing.Point(12, 58);
            this._textBoxPgnInput.Multiline = true;
            this._textBoxPgnInput.Name = "_textBoxPgnInput";
            this._textBoxPgnInput.Size = new System.Drawing.Size(454, 189);
            this._textBoxPgnInput.TabIndex = 2;
            // 
            // _labelPgnInput
            // 
            this._labelPgnInput.AutoSize = true;
            this._labelPgnInput.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelPgnInput.Location = new System.Drawing.Point(12, 16);
            this._labelPgnInput.Name = "_labelPgnInput";
            this._labelPgnInput.Size = new System.Drawing.Size(102, 25);
            this._labelPgnInput.TabIndex = 3;
            this._labelPgnInput.Text = "PGN-Input";
            // 
            // _buttonChooseFile
            // 
            this._buttonChooseFile.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonChooseFile.Location = new System.Drawing.Point(168, 268);
            this._buttonChooseFile.Name = "_buttonChooseFile";
            this._buttonChooseFile.Size = new System.Drawing.Size(138, 40);
            this._buttonChooseFile.TabIndex = 4;
            this._buttonChooseFile.Text = "Choose File";
            this._buttonChooseFile.Click += new System.EventHandler(this.OnButtonChooseFileClick);
            // 
            // FormPgnLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 320);
            this.Controls.Add(this._buttonChooseFile);
            this.Controls.Add(this._labelPgnInput);
            this.Controls.Add(this._textBoxPgnInput);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPgnLoader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load PGN";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _buttonLoad;
        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.TextBox _textBoxPgnInput;
        private System.Windows.Forms.Button _buttonChooseFile;
        private System.Windows.Forms.Label _labelPgnInput;
    }
}