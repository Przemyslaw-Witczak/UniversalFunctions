using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace.Logger
{
    /// <summary>
    /// Moja klasa loggera, wielowątkowa, Singleton
    /// </summary>
    /// <remarks>Idea, działania jest następująca. Program, podczas dodawania logow, dodaje je do kolejki, FiFo
    /// następnie, w kolejności są zapisywane do pliku, do którego jest otwarty jeden uchwyt.</remarks>
    public sealed class Logger : IDisposable
    {
        private readonly string MUTEX_GUID = "e1ffff8f-c91d-4188-9e82-c92ca5b1d057";
        private Mutex m_oLoggerMutex;

        /// <summary>
        /// Obiekt, kolejka logów
        /// </summary>
        private ConcurrentQueue<LogPositions> kolejkaFiFo;
        /// <summary>
        /// Prawda, gdy logi są zapisywane do pliku
        /// </summary>
        private bool logsAreSaving;
        
        /// <summary>
        /// Zmienna prywatna, będąca instancją singletona 
        /// </summary>
        private static volatile Logger instance;
        private static object syncRoot = new Object();

        /// <summary>
        /// Zdarzenie dopisania do kolejki logów
        /// </summary>
        private event LogAddedEventHandler OnLogAdd;

        /// <summary>
        /// Czy wysyłać powiadomienie email
        /// </summary>
        public bool SendEmail;
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        private Logger() 
        {
            m_oLoggerMutex = new Mutex(false, MUTEX_GUID);

            kolejkaFiFo = new ConcurrentQueue<LogPositions>();
            SendEmail = false;
            logsAreSaving = false;
            OnLogAdd = new LogAddedEventHandler(OnLogAddEvent);
        }

        /// <summary>
        /// Właściwość publiczna zwracająca instancję loggera
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Logger();
                    }
                }

                return instance;
            }
        }

        #region LOGI
        /// <summary>
        /// Dodaj log
        /// </summary>
        /// <param name="Komunikat">Treść komunikatu</param>
        /// <param name="WyslijMail">Czy wysyłka maila?</param>
        public void Loguj(string Komunikat, bool WyslijMail)
        {
            LogPositions log = new LogPositions(Komunikat);

            kolejkaFiFo.Enqueue(log);

            OnLogAdd(this, new LogAddedEventArgs("Dodano log"));
            string czas = string.Empty;
            DateTime czas_time = DateTime.Now;
            czas = Convert.ToString(czas_time) + "." + czas_time.Millisecond.ToString();

            try
            {
                if (WyslijMail && SendEmail && !string.IsNullOrEmpty(KlientEmailUstawienia.Instance.adresat))
                {

                    try
                    {
                        //Task t = Task.Factory.StartNew(() =>
                        {
                            Loguj("Wysyłanie maila do: " + KlientEmailUstawienia.Instance.adresat, false);
                            KlientEmail mail = new KlientEmail(KlientEmailUstawienia.Instance);
                            mail.SendMail(czas + "; " + Komunikat, "LOGER - Error log");
                        };//);


                    }
                    catch (Exception)
                    {
                        Loguj("Błąd wysyłania email'a", false);
                    }
                }
               
                
            }
            catch (Exception e)
            {
                AddEventLog("Błąd podczas zapisywania do kolejki " + e.ToString());
            }

        }

        /// <summary>
        /// Dodaj log
        /// </summary>
        /// <param name="Komunikat"></param>
        public void Loguj(string Komunikat)
        {
            Loguj(Komunikat, true);
        }
        #endregion

        /// <summary>
        /// Metoda wywoływana podczas zdarzenia dodania logów do kolejki
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnLogAddEvent(object source, LogAddedEventArgs e)
        {
            if (!logsAreSaving)
            {
                logsAreSaving = true;
                SaveLog();
            }
        }

        /// <summary>
        /// Zapisuje stan kolejki logów, do pliku, oraz usuwa zapisane elementy
        /// </summary>
        private void SaveLog()
        {
            m_oLoggerMutex.WaitOne();
            {
                try
                {
                    using(StreamWriter file = new StreamWriter("logi_" + Application.ProductName + ".txt", true, System.Text.Encoding.Default))
                    {
                        LogPositions log;
                        while (kolejkaFiFo.TryDequeue(out log))
                        {
                            file.WriteLine(log.GetLogValue());
                        }
                    }
                }
                catch (Exception e)
                {
                    AddEventLog("Błąd zapisywania do pliku, logów: " + e.ToString());
                }
                
            }
            logsAreSaving = false;
            m_oLoggerMutex.ReleaseMutex();
            
        }

        /// <summary>
        /// Dodaje log do systemowego dziennika zdarzeń
        /// </summary>
        /// <param name="komunikat"></param>
        public void AddEventLog(string komunikat)
        {
            string sSource;
            string sLog;

            sSource = Application.ProductName;
            sLog = "Logger";

            try
            {

                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                EventLog.WriteEntry(sSource, komunikat);
            }
            catch(Exception)
            {
                //Już nie ma gdzie logować, skoro nawet tu się wysypało
            }

        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {                                    
                    m_oLoggerMutex.Dispose();
                }

                disposedValue = true;
            }
        }

        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
