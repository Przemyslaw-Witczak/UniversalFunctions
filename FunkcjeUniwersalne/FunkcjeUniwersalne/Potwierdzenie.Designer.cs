namespace MojeFunkcjeUniwersalneNameSpace
{
    partial class Potwierdzenie
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
            this.bnlBtny = new System.Windows.Forms.Panel();
            this.btnNie = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnTak = new System.Windows.Forms.Button();
            this.pnlTopLine = new System.Windows.Forms.Panel();
            this.lblFirstLine = new System.Windows.Forms.Label();
            this.pnlSecondLine = new System.Windows.Forms.Panel();
            this.txtMultiline = new System.Windows.Forms.TextBox();
            this.lblSecondLine = new System.Windows.Forms.Label();
            this.bnlBtny.SuspendLayout();
            this.pnlTopLine.SuspendLayout();
            this.pnlSecondLine.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnlBtny
            // 
            this.bnlBtny.Controls.Add(this.btnNie);
            this.bnlBtny.Controls.Add(this.btnOk);
            this.bnlBtny.Controls.Add(this.btnTak);
            this.bnlBtny.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnlBtny.Location = new System.Drawing.Point(0, 131);
            this.bnlBtny.Name = "bnlBtny";
            this.bnlBtny.Size = new System.Drawing.Size(472, 46);
            this.bnlBtny.TabIndex = 6;
            // 
            // btnNie
            // 
            this.btnNie.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnNie.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNie.Location = new System.Drawing.Point(300, 11);
            this.btnNie.Name = "btnNie";
            this.btnNie.Size = new System.Drawing.Size(75, 23);
            this.btnNie.TabIndex = 5;
            this.btnNie.Text = "NIE";
            this.btnNie.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(199, 11);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnTak
            // 
            this.btnTak.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnTak.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnTak.Location = new System.Drawing.Point(101, 11);
            this.btnTak.Name = "btnTak";
            this.btnTak.Size = new System.Drawing.Size(75, 23);
            this.btnTak.TabIndex = 4;
            this.btnTak.Text = "TAK";
            this.btnTak.UseVisualStyleBackColor = true;
            // 
            // pnlTopLine
            // 
            this.pnlTopLine.Controls.Add(this.lblFirstLine);
            this.pnlTopLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopLine.Location = new System.Drawing.Point(0, 0);
            this.pnlTopLine.Name = "pnlTopLine";
            this.pnlTopLine.Size = new System.Drawing.Size(472, 63);
            this.pnlTopLine.TabIndex = 7;
            // 
            // lblFirstLine
            // 
            this.lblFirstLine.AutoSize = true;
            this.lblFirstLine.BackColor = System.Drawing.Color.Transparent;
            this.lblFirstLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblFirstLine.Location = new System.Drawing.Point(40, 9);
            this.lblFirstLine.MaximumSize = new System.Drawing.Size(389, 0);
            this.lblFirstLine.Name = "lblFirstLine";
            this.lblFirstLine.Size = new System.Drawing.Size(44, 16);
            this.lblFirstLine.TabIndex = 4;
            this.lblFirstLine.Text = "label1";
            // 
            // pnlSecondLine
            // 
            this.pnlSecondLine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSecondLine.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSecondLine.Controls.Add(this.txtMultiline);
            this.pnlSecondLine.Controls.Add(this.lblSecondLine);
            this.pnlSecondLine.Location = new System.Drawing.Point(0, 63);
            this.pnlSecondLine.Name = "pnlSecondLine";
            this.pnlSecondLine.Size = new System.Drawing.Size(472, 66);
            this.pnlSecondLine.TabIndex = 8;
            // 
            // txtMultiline
            // 
            this.txtMultiline.Location = new System.Drawing.Point(43, 0);
            this.txtMultiline.Multiline = true;
            this.txtMultiline.Name = "txtMultiline";
            this.txtMultiline.ReadOnly = true;
            this.txtMultiline.Size = new System.Drawing.Size(410, 111);
            this.txtMultiline.TabIndex = 6;
            // 
            // lblSecondLine
            // 
            this.lblSecondLine.AutoSize = true;
            this.lblSecondLine.BackColor = System.Drawing.Color.Transparent;
            this.lblSecondLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSecondLine.Location = new System.Drawing.Point(40, 19);
            this.lblSecondLine.MaximumSize = new System.Drawing.Size(800, 600);
            this.lblSecondLine.Name = "lblSecondLine";
            this.lblSecondLine.Size = new System.Drawing.Size(44, 16);
            this.lblSecondLine.TabIndex = 5;
            this.lblSecondLine.Text = "label2";
            // 
            // Potwierdzenie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 10);
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(472, 177);
            this.Controls.Add(this.pnlSecondLine);
            this.Controls.Add(this.pnlTopLine);
            this.Controls.Add(this.bnlBtny);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Potwierdzenie";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Komunikat";
            this.TopMost = true;
            this.bnlBtny.ResumeLayout(false);
            this.pnlTopLine.ResumeLayout(false);
            this.pnlTopLine.PerformLayout();
            this.pnlSecondLine.ResumeLayout(false);
            this.pnlSecondLine.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel bnlBtny;
        private System.Windows.Forms.Button btnNie;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnTak;
        private System.Windows.Forms.Panel pnlTopLine;
        private System.Windows.Forms.Label lblFirstLine;
        private System.Windows.Forms.Panel pnlSecondLine;
        private System.Windows.Forms.TextBox txtMultiline;
        private System.Windows.Forms.Label lblSecondLine;
    }
}