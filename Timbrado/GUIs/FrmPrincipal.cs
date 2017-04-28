using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Utilerias_FTP;
using System.Linq;

namespace Timbrado.GUIs
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string smensaje = string.Empty;
            try
            {
                XmlDocument xml = new XmlDocument();
                //xml.Load(txbXMLPath.Text);
                byte[] array = Encoding.UTF8.GetBytes(xml.InnerXml.ToString());
                byte[] xmlTimbrado;
                wsTimbrado.TimbrarV3 servicio = new wsTimbrado.TimbrarV3();
                int i = servicio.getTimbreCFDI("AAA010101AAA", "PWD", array, out smensaje, out xmlTimbrado);
                //textBox2.Text = "Error: " + i + " Mensaje: " + smensaje;
                if (i == 0)
                {
                  //  textBox2.Text = Encoding.UTF8.GetString(xmlTimbrado);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + Environment.NewLine + smensaje); 
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            ofdBuscar.ShowDialog();
            if (ofdBuscar.FileName!= null)
            {
                //txbXMLPath.Text = ofdBuscar.FileName;
                StreamReader sr = new StreamReader(ofdBuscar.FileName);
                //textBox1.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            lblAccion.Text = string.Empty;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            Iniciar();
        }
        private void Iniciar()
        {
            List<string> lstArchivos = DescargarArchivos();

            //Timbrar Archivos            
            wsTimbrado.TimbrarV3 servicio = new wsTimbrado.TimbrarV3();
            byte[] arrayXML = null;
            byte[] xmlTimbrado = null;
            string smensaje = string.Empty;

            foreach (string fileName in lstArchivos)
            {
                arrayXML = null;
                xmlTimbrado = null;
                smensaje = string.Empty;

                //Leer el XML y guardarlo en un string.
                StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\XML\\" + fileName, Encoding.UTF8);
                string s = sr.ReadToEnd();
                sr.Close();

                s = Utilerias_XML.XML_Controller.getXmlBienFormado(Environment.CurrentDirectory + "\\XML\\" + fileName);

                //Leer el XML y guardarlo en un string.
                StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\XML\\" + fileName, false);
                sw.WriteLine(s);
                sw.Close();

                arrayXML = Encoding.UTF8.GetBytes(s);
                
                int i = servicio.getTimbreCFDI("AAA010101AAA", "PWD", arrayXML, out smensaje, out xmlTimbrado);

                if (i == 0)
                {
                    string xml = Encoding.UTF8.GetString(xmlTimbrado);
                    sw = new StreamWriter(Environment.CurrentDirectory + "\\CFDI\\cfdi_" + fileName, false, Encoding.UTF8);
                    sw.Write(xml);
                    sw.Close();

                    xml = Utilerias_XML.XML_Controller.getXmlBienFormado(Environment.CurrentDirectory + "\\CFDI\\cfdi_" + fileName);

                    sw = new StreamWriter(Environment.CurrentDirectory + "\\CFDI\\cfdi_" + fileName, false, Encoding.UTF8);
                    sw.Write(xml);
                    sw.Close();
                    
                    lblAccion.Text = "Archivo " + fileName + " timbrado con exito!";
                    Logger.AgregarLog("Archivo " + fileName + " timbrado con exito!");
                    Application.DoEvents();
                }
                else
                {
                    lblAccion.Text = "Error al timbrar archivo " + fileName + ". Error: " + smensaje;
                    Logger.AgregarLog("Error al timbrar archivo " + fileName + ". Error: " + smensaje);
                    Application.DoEvents();
                }
            }

            lblAccion.Text = "Archivos Terminados";
            Application.DoEvents();
        }

        private List<string> DescargarArchivos()
        {
            MetodosFTP ftp = new MetodosFTP();

            //Obtener la lista de archivos a procesar
            lblAccion.Text = "Obteniendo lista de Archivos";
            Application.DoEvents();

            ftp.ObtenerListaDeArchivos("ftp://loscorrales.com.mx/public_ftp/xml/", "corrales", "teincom.2013");

            //Descargar los archivos
            foreach (string fileName in ftp.LstArchivos)
            {
                lblAccion.Text = "Descargando archivo " + fileName;
                Application.DoEvents();
                ftp.DescargarArchivo("ftp://loscorrales.com.mx/public_ftp/xml/", fileName, "corrales", "teincom.2013");
                ftp.GuardarArchivo(Environment.CurrentDirectory + "\\XML\\", fileName);
            }

            return ftp.LstArchivos;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            new FrmConfig().ShowDialog();
        }
    }
}
