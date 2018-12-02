using System;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using MojeFunkcjeUniwersalneNameSpace.Logger;
using FilesServer.Package;
using MojeFunkcjeUniwersalneNameSpace;

public  class SynchronousSocketListener 
{
        
    //private  const int   portNum = Convert.ToInt32(MySetup.Instance.GetParam("Connection", "Port", "1983"));
    private static int portNum = 1983;
    private  static  ArrayList ClientSockets  ;
    private  static   bool ContinueReclaim =  true;
    private  static   Thread ThreadReclaim ;
  
    /// <summary>
    /// Wyœwietla komunikat w konsoli i loguje do pliku
    /// </summary>
    /// <param name="message"></param>
    private static void ShowMessage(string message)
    {     
        Console.WriteLine(message);
        Logger.Instance.Loguj(message);
    }

    public  static  void StartListening() 
    {

        ClientSockets = new ArrayList() ;
    
        ThreadReclaim = new Thread( new ThreadStart(Reclaim) );
        ThreadReclaim.Start() ;
    
        TcpListener listener = new TcpListener(portNum);
        try 
        {
            listener.Start();
        
            int TestingCycle = 100 ; 
            int ClientNbr = 0 ;
        
            // Start listening for connections.
              
            ShowMessage("Waiting for a connection...");
            while ( TestingCycle > 0 ) 
            {
                      
                TcpClient handler = listener.AcceptTcpClient();
                        
                if (  handler != null)  
                {
                    ShowMessage(String.Format("Client#{0} accepted!", ++ClientNbr)) ;
                    // An incoming connection needs to be processed.
                    lock( ClientSockets.SyncRoot ) 
                    {
                        int i = ClientSockets.Add( new ClientHandler(handler) ) ;
                        ((ClientHandler) ClientSockets[i]).Start() ;
                    }
                    --TestingCycle ;
                }
                else 
                    break;                
            }
            listener.Stop();
              
            ContinueReclaim = false ;
            ThreadReclaim.Join() ;
              
            foreach ( Object Client in ClientSockets ) 
            {
                ( (ClientHandler) Client ).Stop() ;
            }
              
        } catch (Exception e) 
        {
            ShowMessage(e.ToString());
        }
        
        ShowMessage("Listening ends...");
        //Console.Read();
    
    }

    private static void Reclaim()  
    {
        while (ContinueReclaim) 
        {
            lock( ClientSockets.SyncRoot ) 
            {
                for (   int x = ClientSockets.Count-1 ; x >= 0 ; x-- )  
                {
                    Object Client = ClientSockets[x] ;
                    if ( !( ( ClientHandler ) Client ).Alive )  
                    {
                        ClientSockets.Remove( Client )  ;
                        ShowMessage("A client left") ;
                    }
                }
            }
            Thread.Sleep(200) ;
        }         
    }
  
    public  static  int Main(String[] args) 
    {
        portNum = Convert.ToInt32(MySetup.Instance.GetParam("Connection", "Port", "1983"));
        ShowMessage("Files Server Starts");
        StartListening();
        ShowMessage("Files Server Stops");
        return 0;
    }
}

class ClientHandler 
{

    TcpClient ClientSocket ;
    bool ContinueProcess = false ;
    Thread ClientThread ;

    /// <summary>
    /// Wyœwietla komunikat w konsoli i loguje do pliku
    /// </summary>
    /// <param name="message"></param>
    private static void ShowMessage(string message)
    {     
        Console.WriteLine(message);
        Logger.Instance.Loguj(message);
    }

    public ClientHandler (TcpClient ClientSocket)
    {
        this.ClientSocket = ClientSocket ;
    }

    public void Start()
    {
        ContinueProcess = true ;
        ClientThread = new Thread ( new ThreadStart(Process) ) ;
        ClientThread.Start() ;
    }

    private  void Process()
    {
        
        // Data buffer for incoming data.
        byte[] bytes;

        if ( ClientSocket != null )
        {
            NetworkStream networkStream = ClientSocket.GetStream();
            ClientSocket.ReceiveTimeout = 1000 ; // 1000 miliseconds

	       while ( ContinueProcess )
            {
                #region request
                bytes = new byte[ClientSocket.ReceiveBufferSize];
                NetPackage pakietRequest = null;
                try
                {
                    
                    string destination = MySetup.Instance.GetParam("Konfiguracja", "KatalogZalacznikow", "D:\\ZALACZNIKI");
                    pakietRequest = SendReceiveFunctions.ReceiveNetPackage2(networkStream, destination);

                    if (!pakietRequest.IsBinaryData)
                        ShowMessage(pakietRequest.StringData);
                    else
                    {
                        if (string.IsNullOrEmpty(pakietRequest.FileNameWTimeStamp))
                        {
                            throw new Exception("No file name set!");
                            //ShowMessage("No file name set!");
                        }

                        destination = Path.Combine(MySetup.Instance.GetParam("Konfiguracja", "KatalogZalacznikow", "D:\\ZALACZNIKI"), pakietRequest.FileNameWTimeStamp);
                        if (pakietRequest.Command == NetPackageCommandType.Upload)
                        {
                            ShowMessage(string.Format("DONE-File '{0}' received.", pakietRequest.FileNameWTimeStamp));
                        }
                        else if (pakietRequest.Command == NetPackageCommandType.Delete)
                        {
                            File.Delete(destination);
                            ShowMessage(string.Format("DONE-File '{0}' deleted.", pakietRequest.FileNameWTimeStamp));
                        }
                        else if (pakietRequest.Command == NetPackageCommandType.Download)
                        {
                            ShowMessage(string.Format("DONE-File '{0}' sending.", pakietRequest.FileNameWTimeStamp));
                        }


                    }
                }
                catch  ( IOException ex)
                {
                    ShowMessage($"Error receiving DATA: {ex}");
                } // Timeout
                catch  ( SocketException se)
                {
                    ShowMessage( $"Conection is broken! {se}");
                    break ;
                }
                catch (Exception ex)
                {
                    ShowMessage($"Exception was thrown! {ex}");
                    break;
                }
                #endregion
                Thread.Sleep(200) ;

                #region respond
                NetPackage pakietRespond = new NetPackage();
                pakietRespond = new NetPackage();
                pakietRespond.Command = NetPackageCommandType.Upload;
                if (pakietRequest != null && pakietRequest.Command != NetPackageCommandType.Download)
                    pakietRespond.StringData = "OK";
                else
                {
                    pakietRespond.FileNameWTimeStamp = Path.Combine(MySetup.Instance.GetParam("Konfiguracja", "KatalogZalacznikow", "D:\\ZALACZNIKI"), pakietRequest.FileNameWTimeStamp);
                    pakietRespond.IsBinaryData = true;
                    if (File.Exists(pakietRespond.FileNameWTimeStamp))
                    {
                        FileInfo file = new FileInfo(pakietRespond.FileNameWTimeStamp);
                        pakietRespond.PackageLength = file.Length;
                    }
                    else
                    {
                        pakietRespond.StringData = string.Format("Plik '{0}' nie istnieje!", pakietRespond.FileNameWTimeStamp);
                    }
                    
                }
                SendReceiveFunctions.SendNetPackage2(pakietRespond, networkStream);
                //Thread.Sleep(20000);
                #endregion
                break;
            }
            networkStream.Close() ;
            ClientSocket.Close();			
        }
    }  

    public void Stop()
    {
        ContinueProcess = false ;
        if (ClientThread != null && ClientThread.IsAlive)
        {
            ClientThread.Join();
        }
	}
        
    public  bool Alive
    {
        get
        {
            return  ( ClientThread != null  && ClientThread.IsAlive  );
        }
    }
        
}  

