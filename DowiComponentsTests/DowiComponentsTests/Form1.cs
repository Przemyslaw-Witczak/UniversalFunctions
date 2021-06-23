using MojeFunkcjeUniwersalneNameSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DowiComponentsTests
{
    public partial class Form1 : Form
    {
        private InputValidator inputValidiator;
        public Form1()
        {
            InitializeComponent();
            inputValidiator = new InputValidator(this);
            inputValidiator.RegisterComboBox(dowiComboBox1);
            inputValidiator.AllowOnlyIntegerValues(textBox1);
            inputValidiator.RegisterTextBox(textBox1);
            inputValidiator.DestinationControl = button1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dowiComboBox1.Items.Add("Krótka");
            dowiComboBox1.Items.Add("Długa pozycja");
            dowiComboBox1.Items.Add("Bardzo długa pozycja");
            dowiComboBox1.Items.Add("Naj najdłuższa bardzo długa pozycja");
            

            comboBox1.Items.Add("Krótka");
            comboBox1.Items.Add("Długa pozycja");
            comboBox1.Items.Add("Bardzo długa pozycja");
            comboBox1.Items.Add("Naj najdłuższa bardzo długa pozycja");

         
        }
    }
}
