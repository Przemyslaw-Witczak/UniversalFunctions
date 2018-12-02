using FilesServer.Package;
using MojeFunkcjeUniwersalneNameSpace;
using MojeFunkcjeUniwersalneNameSpace.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilesServer
{
    public partial class frmSerwer : Form
    {
        bool SerwerPracuje;

        Thread serwerMainThread;
        Socket listener;

        public Thread SerwerMainThread
        {
            get
            {
                return serwerMainThread;
            }

            set
            {
                serwerMainThread = value;
            }
        }

        public struct ThreadParams
        {
            public int Port;
            public int MaximumConnections;
        };

        public frmSerwer()
        {
            InitializeComponent();
            serwerMainThread = null;
            SerwerPracuje = false;
            listener = null;
            btnUruchomZatrzymaj.Text = "Uruchom serwer";
        }


        #region Uruchomienie i zamkniecie watka
        void UruchomWatek()
        {
            btnUruchomZatrzymaj.Text = "Zatrzymaj serwer";
            ThreadParams konfig = new ThreadParams();
            konfig.Port = Convert.ToInt32(MySetup.Instance.GetParam("Polaczenie", "Port", "1983"));
            konfig.MaximumConnections = Convert.ToInt32(MySetup.Instance.GetParam("Polaczenie", "MaximumConnections", "10"));
            try
            {
                if (serwerMainThread==null)
                {
                    serwerMainThread = new Thread(new ParameterizedThreadStart(Listen));
                    serwerMainThread.Start(konfig);
                }
                
            }
            catch (Exception e)
            {              
                Logger.Instance.Loguj("Błąd podczas uruchamiania listnera " + e.ToString());
            }
        }

        void ZakaczWatek()
        {
            try
            {
                if (listener != null)
                    listener.Close();
            }
            catch (Exception e)
            {
                Logger.Instance.Loguj("Błąd podczas zamykania listnera " + e.ToString());
            }

            try
            {
                if (serwerMainThread != null && serwerMainThread.ThreadState == ThreadState.Stopped)
                    serwerMainThread = null;

                if (serwerMainThread != null && serwerMainThread.IsAlive)
                {
                    serwerMainThread.Abort();
                    serwerMainThread.Join();
                    serwerMainThread = null;
                    btnUruchomZatrzymaj.Text = "Uruchom serwer";
                }

            }
            catch (Exception e)
            {
                Logger.Instance.Loguj("Błąd podczas zabijania wątku " + e.ToString());
            }
            while (serwerMainThread!=null && serwerMainThread.IsAlive)
                Logger.Instance.Loguj("Czekam na zatrztymanie wątku");

        }
        #endregion


        public void Listen(object threadKonfigObject)
        {
            try
            {

                ThreadParams threadKonfig = (ThreadParams)threadKonfigObject;
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Any, threadKonfig.Port));
                listener.Listen(threadKonfig.MaximumConnections);
                while (true)
                {
                    using (Socket serverSocket = listener.Accept())
                    {

                        string receivedValue = string.Empty;
                        while (true)
                        {
#pragma warning disable CS0612 // 'SendReceiveFunctions.ReceiveNetPackage(Socket)' is obsolete
                            NetPackage pakietRequest = SendReceiveFunctions.ReceiveNetPackage(serverSocket);
#pragma warning restore CS0612 // 'SendReceiveFunctions.ReceiveNetPackage(Socket)' is obsolete

                            if (!pakietRequest.IsBinaryData)
                                MessageBox.Show(pakietRequest.StringData);
                            else
                            {
                                string destination = Path.Combine(MySetup.Instance.GetParam("Konfiguracja", "KatalogZalacznikow", "D:\\ZALACZNIKI"), pakietRequest.FileName);
                                if (pakietRequest.Command == NetPackageCommandType.Upload)
                                {
                                    using (FileStream plik = File.Create(destination))
                                    {
#pragma warning disable CS0612 // 'NetPackage.Data' is obsolete
                                        pakietRequest.Data.CopyTo(plik);
#pragma warning restore CS0612 // 'NetPackage.Data' is obsolete

                                        plik.Close();
                                    }
                                }
                                else if (pakietRequest.Command == NetPackageCommandType.Delete)
                                    File.Delete(destination);
                                else if (pakietRequest.Command == NetPackageCommandType.Download)
                                {

                                }


                            }
                                                      
                            #region respond
                            NetPackage pakietRespond = new NetPackage();
                            pakietRespond = new NetPackage();
                            pakietRespond.Command = NetPackageCommandType.Upload;
                            pakietRespond.StringData = "OK";
#pragma warning disable CS0612 // 'NetPackage.Serialize()' is obsolete
                            Stream respondStream = pakietRespond.Serialize();
#pragma warning restore CS0612 // 'NetPackage.Serialize()' is obsolete

#pragma warning disable CS0612 // 'SendReceiveFunctions.SendNetPackage(NetPackage, Socket)' is obsolete
                            SendReceiveFunctions.SendNetPackage(pakietRespond, serverSocket);
#pragma warning restore CS0612 // 'SendReceiveFunctions.SendNetPackage(NetPackage, Socket)' is obsolete
                            #endregion
                            serverSocket.Disconnect(true);

                            serverSocket.Close();
                            break;
                        }
                    }
                }
                
            }
            catch (ThreadAbortException abortException)
            {
                Logger.Instance.Loguj("Wątek otrzymał ThreadAbortException " + abortException.Message.ToString());
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
                Logger.Instance.Loguj("Error in Listen: " + e.Message);
            }

        }

        private void btnUruchomZatrzymaj_Click(object sender, EventArgs e)
        {
            if (!SerwerPracuje)
                UruchomWatek();
            else
                ZakaczWatek();

            SerwerPracuje = serwerMainThread != null;
        }
    }
}
