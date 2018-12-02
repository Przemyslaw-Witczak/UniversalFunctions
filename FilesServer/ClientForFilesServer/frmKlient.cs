using FilesServer.Package;
using MojeFunkcjeUniwersalneNameSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientForFilesServer
{
    public partial class frmKlient : Form
    {
        Socket socket = null;
        public frmKlient()
        {
            InitializeComponent();
        }

        private void btnWyslijTekst_Click(object sender, EventArgs e)
        {
            NetPackage package = new NetPackage();
            package.StringData = textBox1.Text;
            package.IsBinaryData = false;
            package.Command = NetPackageCommandType.Upload;

            ComunicateFunctions comunicate = new ComunicateFunctions();
            NetPackage pakietRespond = comunicate.SendPackageBySocket(MySetup.Instance.GetParam("Polaczenie", "Serwer", "127.0.0.1"), 1983, package, string.Empty);

            MessageBox.Show(pakietRespond.StringData);
        }

        private void btnWyslijPlik_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("Plik '" + textBox1.Text + "' nie istnieje!");
                return;
            }            

            NetPackage package = new NetPackage();
            package.FileName =textBox1.Text;
            package.Command = NetPackageCommandType.Upload;

            ComunicateFunctions comunicate = new ComunicateFunctions();
            NetPackage pakietRespond = comunicate.SendPackageBySocket(MySetup.Instance.GetParam("Polaczenie", "Serwer", "127.0.0.1"), 1983, package, string.Empty);                      

            MessageBox.Show(pakietRespond.StringData);
            
        }

        private void btnWybierzPlik_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                    textBox1.Text = openFile.FileName;
            }
        }

        private void btnUsunPlik_Click(object sender, EventArgs e)
        {
            NetPackage package = new NetPackage();
            package.FileName = textBox1.Text;
            package.Command = NetPackageCommandType.Delete;

            ComunicateFunctions comunicate = new ComunicateFunctions();
            NetPackage pakietRespond = comunicate.SendPackageBySocket(MySetup.Instance.GetParam("Polaczenie", "Serwer", "127.0.0.1"), 1983, package, string.Empty);

            MessageBox.Show(pakietRespond.StringData);
        }        

        private void btnPobierz_Click(object sender, EventArgs e)
        {
                         
            NetPackage package = new NetPackage();
            package.FileName = textBox1.Text;
            package.Command = NetPackageCommandType.Download;
            string dirName = string.Empty;
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = package.FileName;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                    dirName = Path.GetDirectoryName(saveDialog.FileName);
            }

            ComunicateFunctions comunicate = new ComunicateFunctions();
            NetPackage pakietRespond = comunicate.SendPackageBySocket(MySetup.Instance.GetParam("Polaczenie", "Serwer", "127.0.0.1"), 1983, package, dirName);
            if (!pakietRespond.IsBinaryData)
                MessageBox.Show(pakietRespond.StringData);
            else
                MessageBox.Show("File saved");
        }
    }
}
