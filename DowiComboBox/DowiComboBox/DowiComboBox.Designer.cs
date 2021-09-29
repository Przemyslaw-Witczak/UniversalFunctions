using DowiExtensionsNameSpace;
using IDowiComponentNamespace;
using IInputValidiatorNamespace;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DowiComboBoxNameSpace
{
    partial class DowiComboBox : IDowiComponent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ValueInteractionType ValueInteraction { get; set; }

        public IInputValidiator InputValidiator { get; set; }
        public bool HasValue { get; set; } = false;

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.DropDownStyle = ComboBoxStyle.DropDown;

            this.AutoCompleteMode = AutoCompleteMode.None;
            this.AutoCompleteSource = AutoCompleteSource.None;

            this.KeyPress += new KeyPressEventHandler(HandleKeyPress);
            this.TextChanged += new EventHandler(HandleTextChanged);
        }

        private void HandleTextChanged(object sender, EventArgs e)
        {
            if (this.SelectedIndex < 0)
            {
                this.HasValue = false;               
            }
            else
            {
                this.HasValue = true;                
            }
            InputValidiator?.Validate();
        }

        private void HandleKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("HandleKeyPress");
            ComboBox comboBox = sender as ComboBox;
            
            try
            {
               HasValue = true;
                //comboBox.BackColor = System.Drawing.Color.Green;
                string TmpStr;

                bool BackSpace = (e.KeyChar == (char)Keys.Back);
                if (BackSpace && comboBox.SelectionLength > 0)
                    TmpStr = comboBox.Text.SubStringEx(0, comboBox.SelectionStart) + comboBox.Text.SubStringEx(comboBox.SelectionLength + comboBox.SelectionStart + 1, 255);

                else if (BackSpace) // SelLength == 0
                    TmpStr = comboBox.Text.SubStringEx(0, comboBox.SelectionStart - 1) + comboBox.Text.SubStringEx(comboBox.SelectionStart + 1, 255);
                else //Key is a visible character
                    TmpStr = comboBox.Text.SubStringEx(0, comboBox.SelectionStart) + e.KeyChar + comboBox.Text.SubStringEx(comboBox.SelectionLength + comboBox.SelectionStart + 1, 255);
                if (string.IsNullOrEmpty(TmpStr))
                {
                    HasValue = false;
                    comboBox.SelectedIndex = -1;
                    Debug.WriteLine($"TmpStr=isEmpty");
                    InputValidiator?.Validate();
                    return;
                }
                Debug.WriteLine($"TmpStr={TmpStr}");
                // set SelSt to the current insertion point
                int SelSt = comboBox.SelectionStart;

                if (ValueInteraction == ValueInteractionType.InSet) //Sprawdzenie warunku czy tylko wybór, czy również dopisywanie
                    e.KeyChar = (char)0;


                if (BackSpace && SelSt > 0)
                    SelSt--;
                else if (!BackSpace)
                    SelSt++;

                if (SelSt == 0)
                {
                    HasValue = false;
                    Debug.WriteLine($"SelSt={SelSt}");
                    comboBox.Text = "";
                    comboBox.SelectedIndex = -1;
                    InputValidiator?.Validate();
                    return;
                }

                // Now that TmpStr is the currently typed string, see if we can locate a match

                bool Found = false;
                for (int i = 1; i <= comboBox.Items.Count; i++)
                {
                    string value = comboBox.Items[i - 1].ToString().SubStringEx(0, TmpStr.Length).ToUpper();
                    if (TmpStr.ToUpper() == value)
                    {
                        if (ValueInteraction == ValueInteractionType.InSetWrite)
                            e.KeyChar = (char)0;
                        comboBox.DroppedDown = false;
                        comboBox.Text = comboBox.Items[i - 1].ToString(); // update to the match that was found
                        comboBox.SelectedIndex = i - 1;
                        Found = true;
                        HasValue = true;
                        break;
                    }
                }

                if (Found) // select the untyped end of the string
                {
                    comboBox.SelectionStart = SelSt;
                    comboBox.SelectionLength = comboBox.Text.Length - SelSt;
                    Debug.WriteLine("SelectioStart=" + SelSt.ToString());
                }
                if (ValueInteraction==ValueInteractionType.InSet)
                    e.Handled = true;
                HasValue = true;
                //comboBox.BackColor = System.Drawing.Color.Blue;
                Debug.WriteLine($"{comboBox.Name} Selected ItemIndex={comboBox.SelectedIndex}");
                InputValidiator?.Validate();

            }
            catch (Exception ex)
            {
                //ShowMessage("Error in KeyPress: row:=" + IntToStr(row) + "; " + ex.Message);
                Debug.WriteLine("Error in KeyPress: row:=" + ex.Message);
            }
        }

        #endregion
    }
}
