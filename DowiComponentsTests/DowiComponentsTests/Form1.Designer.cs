﻿namespace DowiComponentsTests
{
    partial class Form1
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
            this.dowiComboBox1 = new DowiComboBoxNameSpace.DowiComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dowiComboBox1
            // 
            this.dowiComboBox1.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.dowiComboBox1.DropDownWidth = 365;
            this.dowiComboBox1.FormattingEnabled = true;
            this.dowiComboBox1.InputValidiator = null;
            this.dowiComboBox1.Items.AddRange(new object[] {
            "Krótka\");",
            "dowiComboBox1.Items.Add(\"Długa pozycja\");",
            "dowiComboBox1.Items.Add(\"Bardzo długa pozycja\");",
            "dowiComboBox1.Items.Add(\"Naj najdłuższa bardzo długa pozycja"});
            this.dowiComboBox1.Location = new System.Drawing.Point(37, 26);
            this.dowiComboBox1.Name = "dowiComboBox1";
            this.dowiComboBox1.Size = new System.Drawing.Size(121, 21);
            this.dowiComboBox1.TabIndex = 0;
            this.dowiComboBox1.ValueInteraction = DowiComboBoxNameSpace.ValueInteractionType.InSet;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(37, 71);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 129);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dowiComboBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DowiComboBoxNameSpace.DowiComboBox dowiComboBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
    }
}

