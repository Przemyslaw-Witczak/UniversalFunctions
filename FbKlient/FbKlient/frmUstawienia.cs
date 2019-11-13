using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace FbKlientNameSpace
{
    public partial class frmUstawienia : Form
    {
        public cParametryKonfiguracyjne Parametry;
        bool _nieZapisuj = false;


        public frmUstawienia(bool nieZapisuj=false)
        {
            InitializeComponent();
            _nieZapisuj = nieZapisuj;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmUstawienia_Shown(object sender, EventArgs e)
        {
            //textBox1.Text = Properties.Settings.Default.DataBasePath;
            //textBox2.Text = Properties.Settings.Default.DbUser;
            //textBox3.Text = Properties.Settings.Default.DbPassword;
            //textBox4.Text = Properties.Settings.Default.DbServer;
            //textBox5.Text = Properties.Settings.Default.DbPort.ToString();

            //textBox6.Text = Properties.Settings.Default.Dialect.ToString();
            //textBox7.Text = Properties.Settings.Default.Charset;
            //textBox8.Text = Properties.Settings.Default.Role;
            //textBox9.Text = Properties.Settings.Default.ConnectionLifeTime.ToString();
            //checkBox1.Checked = Properties.Settings.Default.Pooling;
            //numericUpDown1.Value = Properties.Settings.Default.MinPoolSize;
            //numericUpDown2.Value = Properties.Settings.Default.MaxPoolSize;
            //textBox10.Text = Properties.Settings.Default.PacketSize.ToString();
            //checkBox2.Checked = Properties.Settings.Default.Embedded;
            //chkQueriesLog.Checked = Properties.Settings.Default.LogQueries;
            if (Parametry == null)
            {
                return;
            }

            textBox1.Text = Parametry.DataBasePath;
            textBox2.Text = Parametry.DbUser;
            textBox3.Text = Parametry.DbPassword;
            textBox4.Text = Parametry.DbServer;
            textBox5.Text = Parametry.DbPort.ToString();

            textBox6.Text = Parametry.Dialect.ToString();
            textBox7.Text = Parametry.Charset;
            textBox8.Text = Parametry.Role;
            textBox9.Text = Parametry.ConnectionLifeTime.ToString();
            checkBox1.Checked = Parametry.Pooling;
            numericUpDown1.Value = Parametry.MinPoolSize;
            numericUpDown2.Value = Parametry.MaxPoolSize;
            textBox10.Text = Parametry.PacketSize.ToString();
            checkBox2.Checked = Parametry.Embedded;
            chkQueriesLog.Checked = Parametry.QueriesLog;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("File not exists :" + textBox1.Text);
                this.DialogResult = 0;
            }*/
            if (textBox4.Text.Length == 0
                || textBox5.Text.Length == 0)
            {
                this.DialogResult = DialogResult.Cancel;
            }

            Parametry = new cParametryKonfiguracyjne();
            
            try
            {
                Parametry.DataBasePath = textBox1.Text;
                Parametry.DbUser = textBox2.Text;
                Parametry.DbPassword = textBox3.Text;
                Parametry.DbServer = textBox4.Text;

                Parametry.DbPort = Convert.ToInt32(textBox5.Text);

                Parametry.Dialect = Convert.ToInt32(textBox6.Text);
                Parametry.Charset = textBox7.Text;
                Parametry.Role = textBox8.Text;
                Parametry.ConnectionLifeTime = Convert.ToInt32(textBox9.Text);
                Parametry.Pooling = checkBox1.Checked;
                Parametry.MinPoolSize = Convert.ToInt32(numericUpDown1.Value);
                Parametry.MaxPoolSize = Convert.ToInt32(numericUpDown2.Value);
                Parametry.PacketSize = Convert.ToInt32(textBox10.Text);
                Parametry.Embedded = checkBox2.Checked;
                Parametry.QueriesLog = chkQueriesLog.Checked;
                ZapiszParametry(Parametry);
            }
            catch 
            {
                MessageBox.Show("Nie można zapisać ustawień !!");
                this.DialogResult = DialogResult.Cancel;
            }
           
        }

        public void ZapiszParametry(cParametryKonfiguracyjne ParametrY)
        {
            if (_nieZapisuj)
            {
                return;
            }
            //ToDo: Przerobić na zapisywanie parametrów do appconfig.xml
            ConfigurationManager.AppSettings["DataBaseIp"] = ParametrY.DbServer;
            ConfigurationManager.AppSettings["Database"] = ParametrY.DataBasePath;
            ConfigurationManager.AppSettings["DataBaseUsr"] = ParametrY.DbUser;
            ConfigurationManager.AppSettings["DataBasePwd"] = ParametrY.DbPassword;
            ConfigurationManager.AppSettings["DataBaseIp"] = ParametrY.DbServer;

            ConfigurationManager.AppSettings["DataBasePort"] = ParametrY.DbPort.ToString();

            ConfigurationManager.AppSettings["DataBaseDialect"] = ParametrY.Dialect.ToString();
            ConfigurationManager.AppSettings["DataBaseCharset"] = ParametrY.Charset;
            ConfigurationManager.AppSettings["DataBaseRole"] = ParametrY.Role;
            ConfigurationManager.AppSettings["DataBaseConnection_lifetime"] = ParametrY.ConnectionLifeTime.ToString();
            ConfigurationManager.AppSettings["DataBasePooling"] = ParametrY.Pooling.ToString();
            ConfigurationManager.AppSettings["DataBaseMinPoolSize"] = ParametrY.MinPoolSize.ToString();
            ConfigurationManager.AppSettings["DataBaseMaxPoolSize"] = ParametrY.MaxPoolSize.ToString();
            ConfigurationManager.AppSettings["DataBasePacket_Size"] = ParametrY.PacketSize.ToString();
            ConfigurationManager.AppSettings["DataBaseServerType"] = Convert.ToInt16(ParametrY.Embedded).ToString();
            ConfigurationManager.AppSettings["LogQueries"] = ParametrY.QueriesLog.ToString();
            //Properties.Settings.Default.Save();
        }
    }
}
