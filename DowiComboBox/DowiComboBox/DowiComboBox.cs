using IDowiComponentNamespace;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DowiComboBoxNameSpace
{
    public partial class DowiComboBox: ComboBox, IDowiComponent
    {
        //Store the default width to perform check in UpdateDropDownWidth.
        private int initialDropDownWidth = 0;
        private const UInt32 WM_CTLCOLORLISTBOX = 0x0134;
        public AutoScaleMode AutoScaleMode { get; set; }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private const int SWP_NOSIZE = 0x1;


        public DowiComboBox()
        {
            InitializeComponent();

            initialDropDownWidth = this.DropDownWidth;

            this.HandleCreated += new EventHandler(BetterComboBox_HandleCreated);


        }

        private void UpdateDropDownWidth()
        {
            //Create a GDI+ drawing surface to measure string widths
            System.Drawing.Graphics ds = this.CreateGraphics();

            //Float to hold largest single item width
            float maxWidth = 0;

            //Iterate over each item, measuring the maximum width
            //of the DisplayMember strings
            foreach (object item in this.Items)
            {
                maxWidth = Math.Max(maxWidth, ds.MeasureString(item.ToString(), this.Font).Width);
            }

            //Add a buffer for some white space
            //around the text
            maxWidth += 30;

            //round maxWidth and cast to an int
            int newWidth = (int)Decimal.Round((decimal)maxWidth, 0);

            //If the width is bigger than the screen, ensure
            //we stay within the bounds of the screen
            if (newWidth > Screen.GetWorkingArea(this).Width)
            {
                newWidth = Screen.GetWorkingArea(this).Width;
            }

            //Only change the default width if it's smaller
            //than the newly calculated width
            if (newWidth > initialDropDownWidth)
            {
                this.DropDownWidth = newWidth;
            }

            //Clean up the drawing surface
            ds.Dispose();
        }

        private void BetterComboBox_HandleCreated(object sender, EventArgs e)
        {
            UpdateDropDownWidth();
        }

        protected override void WndProc(ref Message m)
        {

            if (m.Msg == WM_CTLCOLORLISTBOX)
            {
                // Make sure we are inbounds of the screen
                int left = this.PointToScreen(new Point(0, 0)).X;

                //Only do this if the dropdown is going off right edge of screen
                if (this.DropDownWidth > Screen.PrimaryScreen.WorkingArea.Width - left)
                {
                    // Get the current combo position and size
                    Rectangle comboRect = this.RectangleToScreen(this.ClientRectangle);

                    int dropHeight = 0;
                    int topOfDropDown = 0;
                    int leftOfDropDown = 0;

                    //Calculate dropped list height
                    for (int i = 0; (i < this.Items.Count && i < this.MaxDropDownItems); i++)
                    {
                        dropHeight += this.ItemHeight;
                    }

                    //Set top position of the dropped list if 
                    //it goes off the bottom of the screen
                    if (dropHeight > Screen.PrimaryScreen.WorkingArea.Height -
                       this.PointToScreen(new Point(0, 0)).Y)
                    {
                        topOfDropDown = comboRect.Top - dropHeight - 2;
                    }
                    else
                    {
                        topOfDropDown = comboRect.Bottom;
                    }

                    //Calculate shifted left position
                    leftOfDropDown = comboRect.Left - (this.DropDownWidth -
                       (Screen.PrimaryScreen.WorkingArea.Width - left));

                    // Postioning/sizing the drop-down
                    //SetWindowPos(HWND hWnd,
                    //      HWND hWndInsertAfter,
                    //      int X,
                    //      int Y,
                    //      int cx,
                    //      int cy,
                    //      UINT uFlags);
                    //when using the SWP_NOSIZE flag, cx and cy params are ignored
                    SetWindowPos(m.LParam,
                       IntPtr.Zero,
                       leftOfDropDown,
                       topOfDropDown,
                       0,
                       0,
                       SWP_NOSIZE);
                }
            }



            base.WndProc(ref m);
        }
    }
}
