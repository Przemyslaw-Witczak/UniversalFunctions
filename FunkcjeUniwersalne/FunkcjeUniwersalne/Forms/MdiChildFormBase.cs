using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace.Forms
{
    public class MdiChildFormBase : Form
    {
        public MenuStrip menuStrip;

        public MdiChildFormBase()
        {
            menuStrip = new MenuStrip();
        }
    }
}
