using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Utilerias_XML
{
    public static class XML_Controller
    {
        /// <summary>
        /// Obtiene un XML bien formado a partir de un string.
        /// </summary>
        /// <param name="sXmlPath">Cadena con la estructura del XML</param>
        /// <returns>Regresa una cadena de caracteres con el XML bien formado</returns>
        public static string getXmlBienFormado(string sXmlPath)
        {
            string sXmlFormated = string.Empty;

            XmlDocument xml = new XmlDocument();
            xml.Load(sXmlPath);

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace,
                Encoding = new UTF8Encoding(false)
            };

            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    xml.Save(writer);
                    sXmlFormated = Encoding.UTF8.GetString(ms.ToArray());
                }
            }

            return sXmlFormated;
        }
    }
}
