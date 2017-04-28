using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilerias_FTP
{
    public class Logger
    {
        /// <summary>
        /// Agrega una linea vacia al Log
        /// </summary>
        /// <param name="tipoLog">Tipo que determina a que archivo se asignará el texto LogFTP o LogATS</param>
        public static void AgregarLog()
        {
            string path = Environment.CurrentDirectory + "\\Logs\\" + DateTime.Today.ToString("dd_MM_yyyy") + ".txt";
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Agrega una linea de texto al Log
        /// </summary>
        /// <param name="mensaje">Texto a agregar al log</param>
        /// <param name="tipoLog">Tipo que determina a que archivo se asignará el texto LogFTP o LogATS</param>
        public static void AgregarLog(string mensaje)
        {
            string path = Environment.CurrentDirectory + "\\Logs\\" + DateTime.Today.ToString("dd_MM_yyyy") + ".txt";
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                writer.WriteLine(mensaje);
            }
        }
     
    }
}
