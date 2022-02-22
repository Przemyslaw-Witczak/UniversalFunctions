using FbClientBase.Abstract;
using FbClientBase.Extensions;
using FbClientBaseNameSpace;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FbKlientNameSpace
{
    /// <summary>
    /// Wrapper do połączenia z bazą danych Firebird SQL Server
    /// </summary>
    /// <remarks>
    /// Autor: Przemysław Witczak,
    /// Klasa wykorzystuje bibliotekę Firebird .Net Provider, jednak opakowuje ją w funkcje znane z komponentów Borland C++ oraz ODBC
    /// </remarks>
    public class FbKlient : FbAbstractClient, IDisposable, ISqlClient
    {
        #region Zmienne publiczne i prywatne       
        
        /// <summary>
        /// Nadrzędny formularz wykorzystywany do prawidłowego wyświetlania komunikatów, oraz zmiany kursora
        /// </summary>
        private Form ParentForm;

        public object GetDataTable()
        {
            StringBuilder commandString = new StringBuilder();
            FbDataAdapter da;
            DataTable dt = null;

            try
            {
                if (PodlaczDoBazy())
                {
                    da = new FbDataAdapter();
                    dt = new DataTable();

                    //getCommandLog(commandString, Commands[QueryId]);
                    GetCurrentCommand().Connection = DataBaseConnection;
                    GetCurrentCommand().Transaction = Transaction;
                    da.SelectCommand = GetCurrentCommand();
                    da.Fill(dt);

                    CommitTransaction();
                }
                else
                {
                    ExceptionLogOrMessage("Problem z połączeniem do bazy danych !");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Problem z wykonaniem zapytania: " + commandString.ToString() + ex.Message.ToString());
            }

            return null ?? dt;
        }

        /// <summary>
        /// Obiekt połączenia do bazy danych Firebird
        /// </summary>
        private FbConnection DataBaseConnection;
        /// <summary>
        /// Transakcja
        /// </summary>
        private FbTransaction Transaction;
        /// <summary>
        /// Lista komend(zapytań) wysyłanych w transakcji
        /// </summary>
        private List<FbCommand> Commands;
        protected override FbCommand GetCurrentCommand()
        {
            return Commands[QueryId];
        }
        /// <summary>
        /// Odpowiedź - rekordset
        /// </summary>
        protected override FbDataReader GetCurrentResponse()
        {            
            if (Responses == null || Responses.Count == 0 || responseId < 0)
            {
                    
                return null;
            }                
            return Responses[responseId];
        
        }

        public List<FbDataReader> Responses;

        private bool DataBaseOpened;
        private int queryId;

        /// <summary>
        /// Indeks komendy, domyślna wartość 0
        /// </summary>
        public int QueryId
        {
            set
            {
                if (queryId != value)
                {
                    queryId = value;
                    if (queryId >= CommandsCount)
                    {
                        CommandsCount = queryId + 1;
                    }
                    else if (queryId>-1 && queryId < commandsCount)
                    {
                        CommandsCount = queryId + 1;
                    }
                }
            }
            get
            {
                return queryId;
            }
        }

        private int commandsCount = 1;

        FbCommand CreateCommand()
        {
            var newCommand = new FbCommand();
            newCommand.Connection = DataBaseConnection;
            newCommand.Transaction = Transaction;
            return newCommand;
        }
        /// <summary>
        /// Liczba komend wykonywanych w aktualnej transakcji, domyślnie 1, nie ma konieczności inkrementowania tej zmiennej jeżeli zmieniana jest QueryId
        /// </summary>
        public int CommandsCount
        {
            private set
            {
                if (commandsCount != value)
                {

                    if (value > commandsCount) //zwiększenie
                    {
                        for (int i = Commands.Count; i < value; i++)
                        {
                            var cmd = CreateCommand();
                            Commands.Add(cmd);
                        }
                    }
                    else if (value < commandsCount) //zmniejszenie
                    {
                        //for (int i=Commands.Count;i>=value;i--)
                        while (Commands.Count > value)
                        {
                            Commands[Commands.Count - 1].Dispose();
                            Commands.RemoveAt(Commands.Count - 1);
                        }
                    }

                    commandsCount = value;
                }

            }
            get { return commandsCount; }
        }

        private int responseId;
        /// <summary>
        /// Indeks odppowiedzi, domyślna wartość 0
        /// </summary>
        public int ResponseId
        {
            get { return responseId; }
            set
            {
                if (Responses == null || value > Responses.Count )
                {
                    throw new Exception("ResponseId wykracza po za liczbę zapytań !! ResponseId=" + responseId.ToString() + " <= Count=" + Responses.Count.ToString());
                }

                if (responseId != value)
                {
                    responseId = value;
                }

            }
        }

        private bool _isDisposed;
        private readonly string _connectionString;

        #endregion

        #region Konstruktor i destruktor
        private void konstruktor()
        {
            ParentForm = null;
            DataBaseConnection = null;
            Transaction = null;
            Commands = new List<FbCommand>();
            
            Responses = new List<FbDataReader>();

            DataBaseOpened = false;
            QueryId = -1;

            _isDisposed = false;
            Loguj("FbKlient__Konstruktor");
        }
        /// <summary>
        /// Konstruktor obiektu do komunikacji z bazą danych Firebird SQL Server, może być wykorzystywany w aplikacjach wielowątkowych i asynchronicznych
        /// </summary>
        /// <param name="connectionString">Opcjonalny connectionstring na potrzeby aplikacji webowych, kiedy nie można stworzyć klasy konfiguracyjnej.</param>
        public FbKlient(string connectionString="")  //konstruktor
        {
            _connectionString = connectionString;
            konstruktor();
        }

        /// <summary>
        /// Konstruktor obiektu do komunikacji z bazą danych Firebird SQL Server, dla okien nie asynchroniczny
        /// </summary>
        /// <param name="ParentForM">Rodzic, do wyświetlania komunikatów</param>
        public FbKlient(Form ParentForM)  //konstruktor
        {
            konstruktor();
            ParentForm = ParentForM;
        }

        /// <summary>
        /// Destruktor
        /// </summary>
        ~FbKlient()
        {
            Loguj("FbKlient__Destruktor");
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            Loguj("FbKlient__Dispose(bool)");
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Miejsce do zwalniania zasobów zarządzalnych                               
                    DataBaseClose();
                    //_objectToDispose.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                }
                //Miejsce do zwalniania zasobów niezarządzalnych 
            }
            _isDisposed = true;

        }

        public void Dispose()
        {
            Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        #endregion        

        #region Konfiguracja klienta
        /// <summary>
        /// Otwiera okno konfiguracji połączenia z bazą danych
        /// </summary>
        public void ConfigureDataBase()
        {
            using (frmUstawienia konfiguracja = new frmUstawienia())
            {
                konfiguracja.ShowDialog();
            }
        }

        /// <summary>
        /// Ustawia konfigurację wg podanego parametru,
        /// </summary>
        /// <param name="prekonfiguracja">Obiekt zawierający parametry połączenia do bazy danych</param>
        public void ConfigureDataBase(cParametryKonfiguracyjne prekonfiguracja)
        {
            using (frmUstawienia konfiguracja = new frmUstawienia())
            {
                konfiguracja.ZapiszParametry(prekonfiguracja);
            }

        }
        #endregion

        protected delegate void setMainFormCursorDelegate(Cursor cursor);
        private void setMainFormCursor(Cursor cursor)
        {
            ParentForm.Cursor = cursor;
            ParentForm.Refresh();
        }

        /// <summary>
        /// Ta metoda ze względu na wskaźnik do formularza może generować problemy w aplikacjach wielowątkowych podczas odwoływania się do formularza i zmiany kursora.
        /// </summary>
        /// <returns></returns>
        private bool PodlaczDoBazy()
        {
            Loguj("FbKlient__PodlaczDoBazy");
            if (DataBaseOpened)
            {
                Loguj("Już połączono z bazą danych");

                return true;
            }

            if (ParentForm != null)
            {
                if (ParentForm.IsHandleCreated)
                {
                    ParentForm.Invoke(new setMainFormCursorDelegate(setMainFormCursor), Cursors.WaitCursor); //To generuje problem w aplikacjach wielowątkowych
                }
            }

            string ConnectionString;
            ConnectionString = GetConnectionString();

            try
            {
                Loguj("Łączę do bazy: " + ConnectionString);
                DataBaseConnection = new FbConnection(ConnectionString);
                Loguj("Utworzono obiekt połączeniowy");
                DataBaseConnection.Open();
                Loguj("Obiekt połączeniowy otwarty");
                Transaction = DataBaseConnection.BeginTransaction();

                Loguj("Otworzyłem połączenie z bazą i tranzakcję!");
                DataBaseOpened = true;

                return true;
            }
            catch (Exception e)
            {
                ExceptionLogOrMessage($"There is an error while connecting to database: '{ConnectionString}'!\n{e}");
                DataBaseOpened = false;

                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }

                if (DataBaseConnection != null)
                {
                    DataBaseConnection.Dispose();
                    DataBaseConnection = null;
                    //GC.WaitForPendingFinalizers();
                }
                //throw new Exception("There is an error while connecting to database: " + Properties.Settings.Default.DataBasePath + "!\n" + e.ToString());
                return false;
            }

        }

        private string GetConnectionString()
        {
            //return String.Format("DataSource={3};Database={0};User={1};Password={2};Dialect=3;Port={4};Dialect={5};" +
            //                                    "Charset={6};Role={7};Connection lifetime={8};Pooling={9};" +
            //                                    "MinPoolSize={10};MaxPoolSize={11};Packet Size={12};ServerType={13}",
            //                                 Properties.Settings.Default.DataBasePath,
            //                                 Properties.Settings.Default.DbUser,
            //                                 Properties.Settings.Default.DbPassword,
            //                                 Properties.Settings.Default.DbServer,
            //                                 Properties.Settings.Default.DbPort,
            //                                 Properties.Settings.Default.Dialect,
            //                                 Properties.Settings.Default.Charset,
            //                                 Properties.Settings.Default.Role,
            //                                 Properties.Settings.Default.ConnectionLifeTime.ToString(),
            //                                 Properties.Settings.Default.Pooling.ToString(),
            //                                 Properties.Settings.Default.MinPoolSize.ToString(),
            //                                 Properties.Settings.Default.MaxPoolSize.ToString(),
            //                                 Properties.Settings.Default.PacketSize.ToString(),
            //                                 Convert.ToInt16(Properties.Settings.Default.Embedded).ToString()

            //                                 );
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;
            return $"DataSource={ConfigurationManager.AppSettings["DataBaseIp"]};" +
                $"Database={ConfigurationManager.AppSettings["Database"]};" +
                $"User={ConfigurationManager.AppSettings["DataBaseUsr"]};" +
                $"Password={ConfigurationManager.AppSettings["DataBasePwd"]};" +            
                $"Port={ConfigurationManager.AppSettings["DataBasePort"]};" +
                $"Dialect={ConfigurationManager.AppSettings["DataBaseDialect"]};" +
                $"Charset={ConfigurationManager.AppSettings["DataBaseCharset"]};" +
                $"Role={ConfigurationManager.AppSettings["DataBaseRole"]};" +
                $"Connection lifetime={ConfigurationManager.AppSettings["DataBaseConnection_lifetime"]};" +
                $"Pooling={ConfigurationManager.AppSettings["DataBasePooling"]};" +
                $"MinPoolSize={ConfigurationManager.AppSettings["DataBaseMinPoolSize"]};" +
                $"MaxPoolSize={ConfigurationManager.AppSettings["DataBaseMaxPoolSize"]};" +
                $"Packet Size={ConfigurationManager.AppSettings["DataBasePacket_Size"]};" +
                $"ServerType={ConfigurationManager.AppSettings["DataBaseServerType"]}";
        }

        private void CommitTransaction()
        {
            Loguj("FbKlient__CommitTransaction");
            if (Transaction != null)
            {
                try
                {
                    Transaction.Commit();
                }
                catch(Exception)
                {
                    Loguj("Error in FbKlient__CommitTransaction while try Transaction.Commit();");
                }
                Transaction.Dispose();
                Loguj("FbKlient__CommitTransaction__Commiting transaction");
                Transaction = null;

                //GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Metoda zamykająca transakcję
        /// </summary>
        public void DataBaseClose()
        {
            Loguj("FbKlient__DataBaseClose");
            ResponseClose();

            CommandsClose();

            CommitTransaction();
            if (DataBaseConnection != null)
            {
                DataBaseConnection.Close();
                DataBaseConnection.Dispose();
                DataBaseConnection = null;
                //GC.WaitForPendingFinalizers();
            }
            DataBaseOpened = false;
            QueryId = -1;

            if (ParentForm != null)
            {
                if (ParentForm.IsHandleCreated)
                {
                    ParentForm.Invoke(new setMainFormCursorDelegate(setMainFormCursor), Cursors.Default);
                }
            }
            Loguj("FbKlient__DataBaseClose-Koniec");
        }
        private void ResponseClose()
        {
            Loguj("FbKlient__ResponseClose");
            //QueryId = 0;
            if (Responses == null)
            {
                return;
            }

            foreach (FbDataReader response in Responses)
            {
                if (response != null)
                {
                    if (response.IsClosed == false)
                    {
                        response.Close();
                    }

                    response.Dispose();

                    //GC.WaitForPendingFinalizers();
                    //MyShowMessage("Response Close");
                }
            }
            Responses.Clear();
            Loguj("FbKlient__ResponseClose-Koniec");
        }
        private void CommandsClose()
        {
            Loguj("FbKlient__CommandsClose");
            if (Commands != null)
            {
                for (int i = 0; i < Commands.Count; i++)
                {
                    Commands[i].Dispose();
                    Commands[i] = null;
                    GC.WaitForPendingFinalizers();
                }
                Commands.Clear();
                Commands = null;
            }
            Loguj("FbKlient__CommandsClose-Koniec");
        }

        /// <summary>
        /// Metoda dodająca zapytanie, jednorazowe wywołanie, jedna komenda
        /// </summary>
        /// <param name="SqlString">Zapytanie SQL</param>
        public void AddSQL(String SqlString)
        {
            Loguj("FbKlient__AddSQL");
            SqlString = SqlString.Replace(":", "@");
            PodlaczDoBazy();
            try
            {
                if (Commands.Count < 1)
                {
                    var cmd = CreateCommand();
                    cmd.CommandText = SqlString;
                    Commands.Add(cmd);
                    QueryId = 0;
                }
                else
                {
                    GetCurrentCommand().CommandText += SqlString;
                }
                Loguj("FbKlient__AddSQL-Koniec");
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while trying AddSQL: " + ex.Message);
            }

        }

        
        #region Wykonaj zapytanie do bazy
        /// <summary>
        /// Metoda wykonuje zapytanie SQL
        /// </summary>
        /// <param name="WithResponse">Czy zwraca rekordy</param>
        private void Execute(bool WithResponse)
        {
            StringBuilder commandString = new StringBuilder();
            try
            {
                ResponseClose();
                if (PodlaczDoBazy())
                {
                    if (WithResponse == true)
                    {
                        foreach (var command in Commands)
                        {
                            command.GetCommandLog(commandString);
                            Responses.Add(command.ExecuteReader());
                            //Thread.Sleep(200);
                        }
                        if (Responses.Count > 0)
                        {
                            ResponseId = 0;                            
                        }
                    }
                    else
                    {
                        foreach (var command in Commands)
                        {
                            if (!String.IsNullOrEmpty(command.CommandText))
                            {
                                command.GetCommandLog(commandString);
                                command.ExecuteNonQuery();
                            }
                        }
                        CommitTransaction();
                        CommandsClose();
                        DataBaseClose();
                    }
                }
                else
                {
                    ResponseId = -1;
                    Responses = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage($"Execute: Problem z wykonaniem zapytania: {commandString}, Exception={ex}");
            }
        }

        /// <summary>
        /// Wykonuje zapytanie SQL ze zwracaniem rekordów
        /// </summary>
        public void Execute()
        {
            Execute(true);
        }
        /// <summary>
        /// Wykonaj zapytanie SQL bez wyników
        /// </summary>
        public void ExecuteNonQuery()
        {
            Execute(false);
        }

        /// <summary>
        /// Metoda do wykonywania skryptów aktualizacyjnych
        /// </summary>
        /// <param name="script">Treść skryptu</param>
        public void ExecuteScript(string script)
        {
          
            try
            {
                
                FirebirdSql.Data.Isql.FbScript fbScript = new FirebirdSql.Data.Isql.FbScript(script);
                fbScript.Parse();
                //ToDo: Sprawdzić możliwości rolbacku transakcji w przypadku błędów w skryptach
                var connection = new FbConnection(GetConnectionString());                
                FirebirdSql.Data.Isql.FbBatchExecution fbe = new FirebirdSql.Data.Isql.FbBatchExecution(connection);
                fbe.AppendSqlStatements(fbScript);                
                string scriptParsed = fbScript.ToString();
                    
                fbe.Execute(true);                                                        
                
            }
            catch (Exception ex)
            {
                string messageErr = string.Empty;
                try
                {
                    if (Transaction != null)
                    {
                        Transaction.Rollback();
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }
                catch(Exception)
                {
                    messageErr = "Nie udało się wycofać transakcji. ";
                }
                throw new Exception(string.Format("Problem z wykonaniem skryptu, {0} {1}",messageErr, ex.Message.ToString()));

            }
        }
        #endregion

        

        /// <summary>
        /// Odczytaj następny rekord
        /// </summary>
        /// <returns>Czy rekord istnieje</returns>
        public bool Read()
        {
            Loguj("FbKlient__Read");
            bool returned_value = false;
            if (GetCurrentResponse() != null)
            {
                try
                {
                    returned_value = GetCurrentResponse().Read();
                    if (!returned_value && Responses.Count - 1 == ResponseId)
                    {
                        DataBaseClose();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogOrMessage("Error in sql read: " + ex.Message);
                }
            }
            return returned_value;
        }
        


        #region MetodySystemowe        
        protected override void ExceptionLogOrMessage(string message)
        {
            Loguj(message);
            if (ParentForm != null)
            {
                MessageBox.Show(message);
            }
            else
            {
                throw new Exception(message);
            }
        }

        private void Loguj(string message)
        {
            if (ConfigurationManager.AppSettings["LogQueries"] == "true" && DebugLog != null)
            {
                DebugLog(message);
            }
        }       
        
        #endregion
    }
}
