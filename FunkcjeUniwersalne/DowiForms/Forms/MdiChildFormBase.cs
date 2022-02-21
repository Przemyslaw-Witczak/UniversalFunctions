using System;
using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace.Forms
{
    /// <summary>
    /// Klasa bazowa w celu ułatwienia rzutowań i odnajnowania menu głównego
    /// </summary>
    public class MdiChildFormBase : Form
    {
        /// <summary>
        /// Menu okna
        /// </summary>
        public MenuStrip MenuStripPointer;

        /// <summary>
        /// Action znajdź do wykorzystania w panelu osadzającym
        /// </summary>
        public Action ZnajdzAction;
      
    }
}
