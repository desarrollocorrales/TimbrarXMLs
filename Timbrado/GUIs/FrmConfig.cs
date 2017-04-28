using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace Timbrado.GUIs
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != null)
            {
                string s =  Utilerias_XML.XML_Controller.getXmlBienFormado(openFileDialog1.FileName);
                textBox1.Text = s;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
