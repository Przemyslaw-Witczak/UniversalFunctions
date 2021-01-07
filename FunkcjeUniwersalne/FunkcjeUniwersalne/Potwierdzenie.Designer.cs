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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnTak = new System.Windows.Forms.Button();
            this.btnNie = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(191, 128);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnTak
            // 
            this.btnTak.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnTak.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnTak.Location = new System.Drawing.Point(93, 128);
            this.btnTak.Name = "btnTak";
            this.btnTak.Size = new System.Drawing.Size(75, 23);
            this.btnTak.TabIndex = 1;
            this.btnTak.Text = "TAK";
            this.btnTak.UseVisualStyleBackColor = true;
            // 
            // btnNie
            // 
            this.btnNie.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnNie.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNie.Location = new System.Drawing.Point(292, 128);
            this.btnNie.Name = "btnNie";
            this.btnNie.Size = new System.Drawing.Size(75, 23);
            this.btnNie.TabIndex = 2;
            this.btnNie.Text = "NIE";
            this.btnNie.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(40, 17);
            this.label1.MaximumSize = new System.Drawing.Size(389, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(40, 69);
            this.label2.MaximumSize = new System.Drawing.Size(800, 600);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // Potwierdzenie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 10);
            this.AutoSize = true;
            this.CancelButton = this.btnNie;
            this.ClientSize = new System.Drawing.Size(472, 164);
            this.Controls.Add(this.btnNie);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnTak);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Potwierdzenie";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Komunikat";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnTak;
        private System.Windows.Forms.Button btnNie;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}