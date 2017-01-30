namespace BerldChess.View
{
    partial class PromotionDialog
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
            this._buttonQueen = new System.Windows.Forms.Button();
            this._buttonRook = new System.Windows.Forms.Button();
            this._buttonBishop = new System.Windows.Forms.Button();
            this._buttonKnight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonQueen
            // 
            this._buttonQueen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._buttonQueen.Location = new System.Drawing.Point(12, 12);
            this._buttonQueen.Name = "_buttonQueen";
            this._buttonQueen.Size = new System.Drawing.Size(105, 109);
            this._buttonQueen.TabIndex = 0;
            this._buttonQueen.Tag = "Q";
            this._buttonQueen.UseVisualStyleBackColor = true;
            this._buttonQueen.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // _buttonRook
            // 
            this._buttonRook.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._buttonRook.Location = new System.Drawing.Point(123, 12);
            this._buttonRook.Name = "_buttonRook";
            this._buttonRook.Size = new System.Drawing.Size(105, 109);
            this._buttonRook.TabIndex = 1;
            this._buttonRook.Tag = "R";
            this._buttonRook.UseVisualStyleBackColor = true;
            this._buttonRook.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // _buttonBishop
            // 
            this._buttonBishop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._buttonBishop.Location = new System.Drawing.Point(234, 12);
            this._buttonBishop.Name = "_buttonBishop";
            this._buttonBishop.Size = new System.Drawing.Size(105, 109);
            this._buttonBishop.TabIndex = 2;
            this._buttonBishop.Tag = "B";
            this._buttonBishop.UseVisualStyleBackColor = true;
            this._buttonBishop.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // _buttonKnight
            // 
            this._buttonKnight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._buttonKnight.Location = new System.Drawing.Point(345, 12);
            this._buttonKnight.Name = "_buttonKnight";
            this._buttonKnight.Size = new System.Drawing.Size(105, 109);
            this._buttonKnight.TabIndex = 3;
            this._buttonKnight.Tag = "N";
            this._buttonKnight.UseVisualStyleBackColor = true;
            this._buttonKnight.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // PromotionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 132);
            this.Controls.Add(this._buttonKnight);
            this.Controls.Add(this._buttonBishop);
            this.Controls.Add(this._buttonRook);
            this.Controls.Add(this._buttonQueen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromotionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Promotion";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonQueen;
        private System.Windows.Forms.Button _buttonRook;
        private System.Windows.Forms.Button _buttonBishop;
        private System.Windows.Forms.Button _buttonKnight;
    }
}