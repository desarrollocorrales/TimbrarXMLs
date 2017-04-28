using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Web;

namespace TimbradoSolucionFactible
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofd.ShowDialog();
            if (ofd.FileName != null)
            {
                StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default);
                textBox1.Text = sr.ReadToEnd();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool produccion = false;
            string prod_endpoint = "TimbradoEndpoint_PRODUCCION";
            string test_endpoint = "TimbradoEndpoint_TESTING";

            //Si recibe error 417 deberá descomentar la linea a continuación
            //System.Net.ServicePointManager.Expect100Continue = false;

            //El paquete o namespace en el que se encuentran las clases
            //será el que se define al agregar la referencia al WebService,
            //en este ejemplo es: com.sf.ws.Timbrado
            wsTimbrar.TimbradoPortTypeClient portClient = null;
            portClient = (produccion) ? new wsTimbrar.TimbradoPortTypeClient(prod_endpoint) : portClient = new wsTimbrar.TimbradoPortTypeClient(test_endpoint);

            try
            {
                textBox3.Text = string.Empty;
                byte[] bytes = Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(ofd.FileName));
                textBox3.Paste("Sending request...");
                textBox3.Paste(Environment.NewLine);
                textBox3.Paste("EndPoint = " + portClient.Endpoint.Address);
                textBox3.Paste(Environment.NewLine);
                Application.DoEvents();
                wsTimbrar.CFDICertificacion response = portClient.timbrar("testing@solucionfactible.com", "timbrado.SF.16672", bytes, false);

                textBox3.Paste("Información de la transacción");
                textBox3.Paste(Environment.NewLine);
                textBox3.Paste(response.status.ToString());
                textBox3.Paste(Environment.NewLine);
                textBox3.Paste(response.mensaje);
                textBox3.Paste(Environment.NewLine);
                textBox3.Paste("Resultados recibidos" + response.resultados.Length);
                textBox3.Paste(Environment.NewLine);
                Application.DoEvents();
                wsTimbrar.CFDIResultadoCertificacion[] resultados = response.resultados;

                textBox2.Text = string.Empty;                
                textBox2.Paste(resultados[0].mensaje);
                textBox2.Paste(Encoding.UTF8.GetString(resultados[0].cfdiTimbrado));
                //Clases a usar en cancelación:
                //com.sf.ws.Timbrado.CFDICancelacion
                //com.sf.ws.Timbrado.CFDIResultadoCancelacion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
