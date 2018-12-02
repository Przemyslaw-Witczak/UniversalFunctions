using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FilesServer.Package
{
    /// <summary>
    /// Klasa metod komunikacji
    /// </summary>
    public class ComunicateFunctions
    {
        /// <summary>
        /// Funkcja wysyłająca komunikaty poprzez Socket
        /// </summary>
        /// <param name="address">Adres zdalnego komputera</param>
        /// <param name="port">Port na zdalnym komputerze</param>
        /// <returns>Zwraca odebrany pakiet</returns>
        public NetPackage SendPackageBySocket(string address, int port, NetPackage packageSend, string dirName)
        {
            NetPackage receivedPackage = null;
            try
            {
                
                IPAddress[] ipAddress = Dns.GetHostAddresses(address);
                IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], port);
                using (Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    clientSock.SendTimeout = 300000; //infinite //clientSock.SendTimeout = 300000;
                    clientSock.Connect(ipEnd);

                    #region Send request


                    SendReceiveFunctions.SendNetPackage2(packageSend, clientSock);
                    #endregion

                    #region Read respond                    

                    receivedPackage = SendReceiveFunctions.ReceiveNetPackage2(clientSock, dirName);

                    #endregion

                    clientSock.Disconnect(true);
                }
            }
            catch(Exception ex)
            {
                receivedPackage = new NetPackage();
                receivedPackage.StringData = ex.ToString();
            }
            return receivedPackage;
        }
        
    }
}
