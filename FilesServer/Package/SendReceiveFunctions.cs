using System;
using System.IO;
using System.Net.Sockets;

namespace FilesServer.Package
{
    /// <summary>
    /// Klasa statyczna zawierająca funkcje do transmisji pakietu
    /// </summary>
    public static class SendReceiveFunctions
    {
        #region Wysyłanie pakietu
        

        /// <summary>
        /// Wysyłka przez Socket, strumieniowanie pliku w locie
        /// </summary>
        /// <param name="package">Pakiet</param>
        /// <param name="socket">Gniazdo</param>          
        public static void SendNetPackage2(NetPackage package, Socket socket)
        {
                       
            if (File.Exists(package.FileName))
            {
                FileInfo file = new FileInfo(package.FileName);
                package.PackageLength = file.Length;
            }

            Stream stream = package.Serialize2();
            
            #region Wysyłam nazwę pliku i dane dodatkowe
            byte[] array = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(array, 0, (int)stream.Length);
            socket.Send(array);
            #endregion

            #region Wysyłam plik
            if (package.PackageLength > 0)
            { 
                using (FileStream fileIO = File.OpenRead(package.FileName))
                {
                    var buffer = new byte[1024 * 8];
                    int count = 0;
                    while ((count = fileIO.Read(buffer, 0, buffer.Length)) > 0)
                        socket.Send(buffer, count, SocketFlags.None);
                }
            }
            #endregion

        }

        
        public static void SendNetPackage2(NetPackage package, NetworkStream networkStream)
        {                                    
            if (File.Exists(package.FileNameWTimeStamp))
            {
                FileInfo file = new FileInfo(package.FileNameWTimeStamp);
                package.PackageLength = file.Length;
            }
            //else
            //    package.PackageLength = 0;

            Stream stream = package.Serialize2();

         
            #region Wysyłam nazwę pliku i dane dodatkowe
            byte[] array = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(array, 0, (int)stream.Length);
            networkStream.Write(array, 0, (int)stream.Length);
            #endregion

            #region Wysyłam plik
            if (package.PackageLength > 0 && package.IsBinaryData)
            {
                using (FileStream fileIO = File.OpenRead(package.FileNameWTimeStamp))
                {
                    var buffer = new byte[1024 * 8];
                    int count = 0;
                    while ((count = fileIO.Read(buffer, 0, buffer.Length)) > 0)
                        networkStream.Write(buffer, 0, count);
                }
            }            
            #endregion
        }
        #endregion


        #region Odbiór pakietu
       
        /// <summary>
        /// Klient odbiera dane
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="attachmentsPath"></param>
        /// <returns></returns>
        public static NetPackage ReceiveNetPackage2(Socket socket, string attachmentsPath)
        {
            NetworkStream networkStream = new NetworkStream(socket);

            NetPackage pakietRequest = null;

            pakietRequest = ReceiveNetPackage2(networkStream, attachmentsPath);
            
            return pakietRequest;
        }

       
        /// <summary>
        /// Odbiera bardzo duże pliki i zapisuje w podanym katalogu
        /// </summary>
        /// <param name="clientSocket">Gniazdo</param>
        /// <param name="attachmentsPath">Katalog docelowy, zapisania pliku</param>
        /// <returns>NetPackage</returns>
        public static NetPackage ReceiveNetPackage2(NetworkStream networkStream, string attachmentsPath)
        {
            int count = 0;
            long readen = 0;

            NetPackage pakietRequest = new NetPackage();
            try
            {
                               
                #region Odbieram rozmiar pliku, nazwę pliku i dane dodatkowe

                pakietRequest.Deserialize2(networkStream);

                #endregion

                #region Odbieram plik
                string destination = Path.Combine(attachmentsPath, pakietRequest.FileNameWTimeStamp);
                if (!string.IsNullOrEmpty(pakietRequest.FileNameWTimeStamp) && pakietRequest.Command==NetPackageCommandType.Upload)
                {
                    using (FileStream plik = File.Create(destination))
                    {

                        var buffer = new byte[1024 * 8];

                        while (readen < pakietRequest.PackageLength && (count = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            plik.Write(buffer, 0, count);
                            readen += count;
                        }

                        plik.Close();
                    }
                }
                #endregion

            }
            catch (IOException ex)
            {
                Console.WriteLine(string.Format("Error in ReceiveNetPackage: FileSize={0}, Readen={1}, Error={2}", pakietRequest.PackageLength.ToString(), readen.ToString(), ex.Message));
            }
           
            return pakietRequest;
        }

        #endregion


    }
}
