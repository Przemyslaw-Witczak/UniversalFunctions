
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using MojeFunkcjeUniwersalneNameSpace.Logger;


namespace MojeFunkcjeUniwersalneNameSpace.FTP
{
    /// <summary>
    /// Klasa - wrapper do połączeń nieszyfrowanych FTP
    /// </summary>
    public class Ftp
    {
        private string _ftpHost;
        private string _ftpLogin;
        private string _ftpPassword;
        private bool _ftpUsePassive;
        private int _ftpTimeOut;
        

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ftpHost"></param>
        /// <param name="ftpLogin"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="ftpUsePassive"></param>
        /// <param name="ftpTimeOut"></param>
        public Ftp(string ftpHost, string ftpLogin, string ftpPassword, bool ftpUsePassive, int ftpTimeOut)
        {
            _ftpHost = ftpHost;
            _ftpLogin = ftpLogin;
            _ftpPassword = ftpPassword;
            _ftpUsePassive = ftpUsePassive;
            _ftpTimeOut = ftpTimeOut;
            OnTransferEvent = null;
            Log("I","Utworzono obiekt FTP");
        }

        public event FtpTransferEventHandler OnTransferEvent;

        public void Log(string level, string message, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(level))
            {
                sb.Append(level);
                sb.Append(" ");
            }

            sb.Append(message);

            if (ex!=null)
            {
                sb.Append(" Exception: ");
                sb.Append(ex.ToString());
            }            
            
            Logger.Logger.Instance.Loguj(sb.ToString());            
        }

        public void Log(string level, string message)
        {
            Log(level, message, null);
        }

        /// <summary>
        /// Pobiera listę plików i katalogow wskazanej lokalizacji ftp
        /// </summary>
        /// <param name="path">ścieżka w strukturze ftp, np. /kat1/kat2</param>
        /// <param name="details">true jesli ma zwracac w linii detale o plikach/katalogach</param>
        /// <returns>lista typu string</returns>
        public List<string> GetFilesList(string path, bool details)
        {
            
            List<string> downloadFiles = new List<string>();

            FtpWebRequest ftp;
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                Log("I", string.Format("FTP: GetFilesList: '{0}'", path));
                string address = protocol(false) + _ftpHost + path;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(address));
                ftp.Timeout = _ftpTimeOut;

                ftp.KeepAlive = false;
                ftp.UseBinary = true;
                ftp.Credentials = new NetworkCredential(_ftpLogin, _ftpPassword);                               
                ftp.Method = details ? WebRequestMethods.Ftp.ListDirectoryDetails : WebRequestMethods.Ftp.ListDirectory;
                ftp.UsePassive = AppConfig.FtpUsePassive;
                ftp.EnableSsl = false;
                ftp.Proxy = null;
                ftp.AuthenticationLevel = AuthenticationLevel.MutualAuthRequired;
                
                using (response = ftp.GetResponse())
                {
                    using (reader = new StreamReader(response.GetResponseStream()))
                    {
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            downloadFiles.Add(line);
                            line = reader.ReadLine();
                        }
                        reader.Close();
                        response.Close();
                    }
                }
                return downloadFiles;
            }
            catch (Exception ex)
            {
                try
                {
                    Log("I","FTP: GetFilesList: Błąd w metodzie GetFilesList", ex);
                    reader.Close();
                    response.Close();
                }
                catch (Exception)
                {
                    Log("I","FTP: GetFilesList: Błąd podczas obsługi wyjątku w metodzie GetFilesList", ex);
                }
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Tworzy katalog
        /// </summary>
        /// <param name="dirName">ścieżka do katalogu, np. /kat1/kat2/nowykat</param>
        public void CreateDirectory(string dirName)
        {
            FtpWebRequest reqFTP;
            FtpWebResponse response = null;
            Stream ftpStream = null;
            try
            {
                Debug(string.Format("- FTP: CreateDirectory: '{0}'", dirName));
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(protocol(AppConfig.FtpUseSSL) + _ftpHost + "/" + dirName));
                reqFTP.Timeout = _ftpTimeOut;
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(_ftpLogin, _ftpPassword);
                reqFTP.UsePassive = AppConfig.FtpUsePassive;

                Debug("- Tworze katalog: '" + dirName + "'");
                using (response = (FtpWebResponse)reqFTP.GetResponse())
                {
                    ftpStream = response.GetResponseStream();
                }
                response.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    Log("I","FTP: CreateDirectory: Błąd w metodzie CreateDirectory", ex);
                    ftpStream.Close();
                    response.Close();
                }
                catch (Exception ex2)
                {
                    Log("I","FTP: CreateDirectory: Błąd podczas obsługi wyjątku w metodzie GetFilesList", ex2);
                }
                throw new Exception(ex.Message, ex);
            }
        }

        private void Debug(string v)
        {
#if DEBUG
            Log("DEBUG", v, null);
#endif
        }

        /// <summary>
        /// Zwraca informacje czy plik o takiej nazwie już istnieje
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool CheckFileExist(string filePath)
        {
            try
            {
                Debug("- Weryfikuje czy na koncie FTP znajduje się plik '" + filePath + "'");                
                string directory = Path.GetDirectoryName(filePath);
                if (directory != null)
                    directory = directory.Replace("\\", "/");
                else
                    throw new Exception($"Nieprawidłowa ścieżka pliku:'{filePath}'");
                filePath = filePath.TrimStart('/');
                Debug("- Pobieram zawartosc katalogu: '" + directory + "'");
                List<string> fileList = GetFilesList(directory, false);
                Debug("- W sprawdzanym katalogu znajduje sie " + fileList.Count.ToString() + " plików");
                if (fileList.Contains(filePath))
                {
                    Debug("- Znaleziono plik: '" + filePath + "')");
                    return true;
                }
                else
                {
                    Debug("- Nie odnaleziono pliku: '" + filePath + "')");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error("- Wystąpił błąd podczas weryfikacji czy plik znajduje się na koncie FTP");
                throw ex;
            }
        }

        private void Error(string v)
        {            
            Log("ERROR", v);
        }

        /// <summary>
        /// Sprawdza istnienie katalogów i jeśli ich brak - tworzy je
        /// </summary>
        /// <param name="path">Ścieżka od której zaczyna się sprawdzanie</param>
        /// <param name="dirs">Lista typu string kolejnych katalogów wgłąb</param>
        public void CheckDirectories(string path, List<string> dirs)
        {
            try
            {
                Debug(string.Format("- FTP: CheckDirectories, path='{0}', dirs.Count='{1}'", path, dirs.Count));
                string root = path;
                foreach (string dir in dirs)
                {
                    Debug(string.Format("- FTP: CheckDirectories, sprawdzam czy pod ścieżką '{0}' znajduje się katalog '{1}'", root, dir));
                    List<string> files = GetFilesList(root, false);

                    root = Path.Combine(root, dir);
                    foreach (string str in files)
                    {
                        Debug("- Plik: '" + str + "'");
                    }
                  
                    if (!files.Contains(dir))
                    {
                        Debug(string.Format("- Tworzę katalog '{0}'", root));
                        CreateDirectory(root);
                    }
                    else
                    {
                        Debug(string.Format("- FTP: CheckDirectories: Odnaleziono katalog '{0}'", dir));
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error", "Wystąpił błąd podczas sprawdzania czy na koncie FTP występuje katalog", ex);
                throw ex;
            }
        }

        public void Upload(string destination, byte[] buff)
        {
            string uri = protocol(AppConfig.FtpUseSSL) + _ftpHost + "/" + destination;
            
            FtpWebRequest reqFTP;
            Stream strm = null;
            try
            {
                Debug("- Przygotowanie do wyslania pliku: '" + destination + "' pod ścieżkę '" + uri + "'");

                Debug("- Tworzenie obiektu polaczenia FTP");
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Timeout = _ftpTimeOut;
                reqFTP.Credentials = new NetworkCredential(_ftpLogin, _ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.EnableSsl = AppConfig.FtpUseSSL;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = AppConfig.FtpUsePassive;

                if (AppConfig.FtpUseSSL)
                {
                    Debug("- Delegat certyfikacji FTP");
                    //ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);                 
                }
                reqFTP.ContentLength = buff.Length;
                Debug("- Zapis źródła danych na koncie FTP");
                using (strm = reqFTP.GetRequestStream())
                {
                    strm.Write(buff, 0, buff.Length);
                    strm.Close();
                }

                Debug("- Zakończono wysyłkę FTP");
            }
            catch (Exception ex)
            {
                try
                {
                    Log("Error", "Wystąpił błąd podczas wysyłki pliku na konto FTP", ex);

                    strm.Close();
                }
                catch (Exception ex2)
                {
                    Log("Error", "Nie można zamknąć strumienia danych!", ex2);
                }
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Dokonuje uploadu pliku
        /// </summary>
        /// <param name="destination">Ścieżka do pliku w strukturze ftp, np. /kat1/nowypliki.txt</param>
        /// <param name="source">Ścieżka lokalna do pliku</param>
        public void Upload(string destination, Stream source)
        {            
            string uri = protocol(AppConfig.FtpUseSSL) + _ftpHost + "/" + destination;
            
            FtpWebRequest reqFTP;
            Stream strm = null;
            try
            {
                Debug("- Przygotowanie do wyslania pliku: '" + destination + "' pod ścieżkę '" + uri + "'");
         
                Debug("- Tworzenie obiektu polaczenia FTP");
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.Timeout = _ftpTimeOut;
                reqFTP.Credentials = new NetworkCredential(_ftpLogin, _ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.EnableSsl = AppConfig.FtpUseSSL;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = AppConfig.FtpUsePassive;
                
                if (AppConfig.FtpUseSSL)
                {
                    Debug("- Delegat certyfikacji FTP");
                    //ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);               
                }

                Debug("- Rozmiar źródła danych: '" + source.Length + "'");
                reqFTP.ContentLength = source.Length;

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                
                Debug("- Zapis źródła danych na koncie FTP");
                
                using (strm = reqFTP.GetRequestStream())
                {
                    contentLen = source.Read(buff, 0, buffLength);

                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = source.Read(buff, 0, buffLength);
                    }

                    strm.Close();
                }

                Debug("- Zakończono wysyłkę FTP");
            }
            catch (Exception ex)
            {
                try
                {
                    Log("Error", "Wystąpił błąd podczas wysyłki pliku na konto FTP", ex);
                    strm.Close();
                }
                catch (Exception ex2)
                {
                    Log("Error", "Nie można zamknąć strumienia danych!", ex2);
                }
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Metoda pobiera z serwera FTP plik
        /// </summary>
        /// <param name="remotePath">Ścieżka do zdalnego pliku</param>
        /// <param name="localPath">Ścieżka do lokalnego pliku</param>
        /// <returns></returns>
        public bool Download(string remotePath, string localPath)
        {
            if (remotePath.ToCharArray()[0] != '/')
                remotePath = "/" + remotePath;
            try
            {
                OnTransferEvent?.Invoke(this, new FtpTransferEventArgs(0));
                FtpWebRequest reqFTP;
                //filePath: The full path where the file is to be created.
                //fileName: Name of the file to be createdNeed not name on
                //          the FTP server. name name()
#region Weryfikacja katalogu docelowego
                try
                {
                    //Zakomentowane: MBT 3615
                    //if (!Directory.Exists("R:"))
                    //{
                    //    string SubstMountPassword = @"trooper";
                    //    string SubstMountLogin = @"gb\cakwnioskixml";
                    //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    //    proc.StartInfo = new System.Diagnostics.ProcessStartInfo("net", @"use R: \\zaltrp01\zasob " + SubstMountPassword + " /user:" + SubstMountLogin);
                    //    proc.Start();
                    //    proc.WaitForExit();
                    //} 

                    string directoryPath = Path.GetDirectoryName(localPath);
                    if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex2)
                {
                    Log("Warn", $"Nie udało się zweryfikować katalogu docelowego: '{localPath}'", ex2);
                }
#endregion

                FileStream outputStream = new FileStream(localPath, FileMode.Create);
                string downloadFile = "ftp://" + _ftpHost +  remotePath;
                Uri uri = new Uri(downloadFile);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(_ftpLogin, _ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                //StreamReader readStream = new StreamReader(ftpStream);
                long cl = response.ContentLength;
                if (cl < 0)
                {
                    Log("Debug", $"Wycinać rozmiar pliku z komunikatu {response.BannerMessage}");
                    Log("Debug", $"Wycinać rozmiar pliku z komunikatu {response.StatusDescription}");

                    int openingBracket = response.StatusDescription.IndexOf("(");
                    int closingBracket = response.StatusDescription.IndexOf(")");                     
                    var temporarySubstring = response.StatusDescription.Substring(openingBracket+1, closingBracket - openingBracket);
                    var fileSize = temporarySubstring.Substring(0, temporarySubstring.IndexOf(" "));
                    try
                    {
                        cl = Convert.ToInt64(fileSize);
                    }
                    catch (Exception e)
                    {
                        Log("Error", $"Nie udało się przekonwertować rozmiaru pliku z komunikatu");
                    }
                }
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                long readen = readCount;
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    readen += readCount;
                    int value = (int)(readen*100/cl);
                    if (value < 0) value = 0;
                    OnTransferEvent?.Invoke(this, new FtpTransferEventArgs(value));                    
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
                OnTransferEvent?.Invoke(this, new FtpTransferEventArgs(100));
                return true;
            }
            catch (Exception ex)
            {
                Log("Error", string.Format("FTP: Wystąpił błąd podczas pobierania pliku '{0}' do lokalizacji '{1}'", remotePath, localPath), ex);
                throw ex;
            }
        }

        private string protocol(bool ssl)
        {
            string returned_value = "ftp://";
            //if (ssl)
            //    returned_value = "sftp://";
            return returned_value;
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }


    /// <summary>
    /// Delegat metody zdarzenia wywoływanego podczas transferu przez FTP
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void FtpTransferEventHandler(object source, FtpTransferEventArgs e);

    /// <summary>
    /// Atrybuty zdarzenia transferu ftp
    /// </summary>
    public class FtpTransferEventArgs : EventArgs
    {
        private int percentValue;
        public FtpTransferEventArgs(int eventValue)
        {
            percentValue = eventValue;
        }
        public int GetInfo()
        {
            return percentValue;
        }
    }
}
