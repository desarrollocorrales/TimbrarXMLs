using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Utilerias_FTP
{
    public class MetodosFTP
    {
        public List<string> LstArchivos;
        private byte[] downloadedData;

        /// <summary>
        /// Conectarse al FTP y descargar la lista de archivos del servidor
        /// </summary>
        /// <param name="FTPAddress">URL PATH del Servidor FTP hata la carpeta donde se encuentran los archivos a descargar</param>
        /// <param name="username">Nombre de usuario del FTP</param>
        /// <param name="password">Contraseña del FTP</param>
        public void ObtenerListaDeArchivos(string FTPAddress, string username, string password)
        {
            LstArchivos = new List<string>();

            try
            {
                FtpWebRequest request = FtpWebRequest.Create(FTPAddress) as FtpWebRequest;

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(username, password);

                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                //Leer la respuesta del servidor
                FtpWebResponse response = request.GetResponse() as FtpWebResponse;
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string nombre = string.Empty;

                while (!reader.EndOfStream)
                {
                    nombre = reader.ReadLine();
                    if (nombre.Contains(".xml"))
                        LstArchivos.Add(nombre);
                }

                //Limpiar Flujos
                reader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                Logger.AgregarLog(string.Format("Excepcion: {0}", e.GetType()));
                Logger.AgregarLog(string.Format("  Mensaje: {0}", e.Message));
            }

            username = string.Empty;
            password = string.Empty;
        }

        /// <summary>
        /// Descarga UN archivo del servidor FTP especificado
        /// </summary>
        /// <param name="FTPAddress">URL PATH del Servidor FTP hasta donde se encuantra el archivo a descargar</param>
        /// <param name="filename">Nombre y extension del archivo a descargar</param>
        /// <param name="username">Nombre de usuario para autenticacion FTP</param>
        /// <param name="password">Contraseña para autenticacion FTP</param>
        public bool DescargarArchivo(string FTPAddress, string filename, string username, string password)
        {
            bool exito = false;
            downloadedData = new byte[0];

            try
            {
                //Crear la Peticion FTP (objeto FtpWebRequest);
                FtpWebRequest request = FtpWebRequest.Create(FTPAddress + filename) as FtpWebRequest;

                //Obtener el archivo
                request = FtpWebRequest.Create(FTPAddress + filename) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false; //close the connection when done

                //Streams
                FtpWebResponse response = request.GetResponse() as FtpWebResponse;
                Stream reader = response.GetResponseStream();

                //Descargar a memoria
                MemoryStream memStream = new MemoryStream();
                byte[] buffer = new byte[1024]; //Descarga en paquetes

                while (true)
                {
                    //Leer datos
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        //Nada fue leido, descarga finalizada
                        break;
                    }
                    else
                    {
                        //Escribir datos descargados
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }

                //Convertir datos descargados a un arreglo de bytes
                downloadedData = memStream.ToArray();

                //Limpiar
                reader.Close();
                memStream.Close();
                response.Close();

                Logger.AgregarLog(string.Format(" -   Archivo descargado correctamente: {0}", filename));
                exito = true;
            }
            catch (Exception e)
            {
                Logger.AgregarLog(string.Format("Excepcion: {0}", e.GetType()));
                Logger.AgregarLog(string.Format("  Mensaje: {0}", e.Message));
            }

            username = string.Empty;
            password = string.Empty;

            return exito;
        }

        /// <summary>
        /// Guarda un arreglo de bytes a un archivo
        /// </summary>
        /// <param name="Path">Path de la carpeta loscal donde se guardará el archivo</param>
        /// <param name="FileName">Nombre y extension del archivo a guardar</param>
        public void GuardarArchivo(string Path, string FileName)
        {
            if (downloadedData != null && downloadedData.Length != 0)
            {
                //Escribir el arreglo de bytes a un archivo
                FileStream newFile = new FileStream(Path + FileName, FileMode.Create);
                newFile.Write(downloadedData, 0, downloadedData.Length);
                newFile.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FTPAddress"></param>
        /// <param name="filename"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public bool SubirArchivo(string FTPAddress, FileInfo filename, string username, string password)
        {
            bool bExito = false;

            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Credentials = new System.Net.NetworkCredential(username, password);
                    client.UploadFile(FTPAddress + filename.Name, "STOR", filename.FullName);
                }

                Logger.AgregarLog(string.Format("Archivo subido correctamente a {0}", FTPAddress + filename.Name));
                bExito = true;
            }
            catch (Exception e)
            {
                Logger.AgregarLog(string.Format("Excepcion: {0}", e.GetType()));
                Logger.AgregarLog(string.Format("  Mensaje: {0}", e.Message));
            }

            return bExito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FTPAddress"></param>
        /// <param name="filename"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void EliminarArchivo(string FTPAddress, string filename, string username, string password)
        {
            try
            {
                FtpWebRequest request = FtpWebRequest.Create(FTPAddress + filename) as FtpWebRequest;
                request = FtpWebRequest.Create(FTPAddress + filename) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Logger.AgregarLog(string.Format(" -   Archivo eliminado correctamente: {0}", filename));
                response.Close();
            }
            catch (Exception e)
            {
                Logger.AgregarLog(string.Format("Excepcion: {0}", e.GetType()));
                Logger.AgregarLog(string.Format("  Mensaje: {0}", e.Message));
            }
        }

        /// <summary>
        /// Prueba una ruta FTP y responde con un TRUE si la ruta ES VALIDA o un FALSE si la ruta NO ES VALIDA
        /// </summary>
        /// <param name="url">Direccion a probar</param>
        /// <param name="username">Usuario FTP</param>
        /// <param name="password">Contraseña FTP</param>
        /// <returns>Respuesta de si la ruta es valida o invalida</returns>
        public bool ProbarConexion(string url, string username, string password)
        {
            bool bExito = false;
            WebResponse response;
            FtpWebRequest requestDir;

            try
            {
                requestDir = (FtpWebRequest)FtpWebRequest.Create(url);
                requestDir.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                requestDir.Credentials = new NetworkCredential(username, password);

                response = requestDir.GetResponse();

                bExito = true;
            }
            catch
            {

            }

            return bExito;
        }
    }
}
