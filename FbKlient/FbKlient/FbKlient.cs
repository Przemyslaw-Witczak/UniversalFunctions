using System;


using System.Windows.Forms;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using MojeFunkcjeUniwersalneNameSpace.Logger;
using System.Configuration;
using ISqlKlientNameSpace;
//using System.Data.SqlClient;

namespace FbKlientNameSpace
{
    /// <summary>
    /// Wrapper do połączenia z bazą danych Firebird SQL Server
    /// </summary>
    /// <remarks>
    /// Autor: Przemysław Witczak,
    /// Klasa wykorzystuje bibliotekę Firebird .Net Provider, jednak opakowuje ją w funkcje znane z komponentów Borland C++ oraz ODBC
    /// </remarks>
    public class FbKlient : IDisposable, ISqlKlient
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
                    Commands[QueryId].Connection = DataBaseConnection;
                    Commands[QueryId].Transaction = Transaction;
                    da.SelectCommand = Commands[QueryId];
                    da.Fill(dt);

                    CommitTransaction();
                }
                else
                {
                    MyShowMessage("Problem z połączeniem do bazy danych !");
                }
            }
            catch (Exception ex)
            {
                MyShowMessage("Problem z wykonaniem zapytania: " + commandString.ToString() + ex.Message.ToString());
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
        /// <summary>
        /// Odpowiedź - rekordset
        /// </summary>
        public FbDataReader Response;
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
                            FbCommand myCommand = new FbCommand();
                            myCommand.Connection = DataBaseConnection;
                            myCommand.Transaction = Transaction;

                            Commands.Add(myCommand);
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
                if (responseId != value)
                {
                    responseId = value;
                }

                if (Responses != null && Responses.Count > responseId)
                {
                    Response = Responses[responseId];
                }
                else
                {
                    throw new Exception("ResponseId wykracza po za liczbę zapytań !! ResponseId=" + responseId.ToString() + " <= Count=" + Responses.Count.ToString());
                }
            }
        }

        private bool _isDisposed;

        #endregion

        #region Konstruktor i destruktor
        private void konstruktor()
        {
            ParentForm = null;
            DataBaseConnection = null;
            Transaction = null;
            Commands = new List<FbCommand>();
            Response = null;
            Responses = new List<FbDataReader>();

            DataBaseOpened = false;
            QueryId = -1;

            _isDisposed = false;
            Loguj("FbKlient__Konstruktor");
        }
        /// <summary>
        /// Konstruktor obiektu do komunikacji z bazą danych Firebird SQL Server
        /// </summary>
        public FbKlient()  //konstruktor
        {
            konstruktor();

        }

        /// <summary>
        /// Konstruktor obiektu do komunikacji z bazą danych Firebird SQL Server
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

        private void Loguj(string message)
        {
            if (ConfigurationManager.AppSettings["LogQueries"]=="true")
            {
                Logger.Instance.Loguj(message, false);
            }
        }

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
                    ParentForm.Invoke(new setMainFormCursorDelegate(setMainFormCursor), Cursors.WaitCursor); //TODO: To generuje problem w aplikacjach wielowątkowych
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
                MyShowMessage($"There is an error while connecting to database: '{ConnectionString}'!\n{e}");
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

        private static string GetConnectionString()
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
                    Commands.Add(new FbCommand(SqlString, DataBaseConnection, Transaction));
                    QueryId = 0;
                }
                else
                {
                    Commands[QueryId].CommandText += SqlString;
                }
                Loguj("FbKlient__AddSQL-Koniec");
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while trying AddSQL: " + ex.Message);
            }

        }

        /// <summary>
        /// Metoda do przekazywania wartości parametrów do zapytania
        /// </summary>
        /// <param name="paramName">Nazwa parametru poprzedzona, może być poprzedzona znakiem '@' lub ':'</param>
        /// <param name="Typ">Typ danych</param>
        /// <returns></returns>
        public FbParameter ParamByName(String paramName, FbDbType paramType)
        {
            Loguj("FbKlient__ParamByName('" + paramName + "' as " + paramType.GetType().Name + ")");
            try
            {
                paramName = paramName.Replace(":", "@");

                if (!paramName.Contains("@"))
                {
                    paramName = "@" + paramName;
                }

                FbParameter returned_param = null;
                foreach (FbParameter param in Commands[queryId].Parameters)
                {
                    if (param.ParameterName.Equals(paramName))
                    {
                        returned_param = param;
                        returned_param.FbDbType = paramType;
                        break;
                    }
                }

                if (returned_param == null)
                {
                    returned_param = Commands[QueryId].Parameters.Add(paramName, paramType);
                }

                Loguj("FbKlient__ParamByName-Koniec");
                return returned_param;
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while setting parameter " + paramName + " " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Przypisuje parametrowi, wartość NULL
        /// </summary>
        /// <param name="paramName"></param>
        public void SetNull(String paramName)
        {
            try
            {
                ParamByName(paramName, FbDbType.SmallInt).Value = DBNull.Value;
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while setting parameter " + paramName + " to null: " + ex.Message);
            }

        }
        #region Wykonaj zapytanie do bazy
        /// <summary>
        /// Metoda wykonuje zapytanie SQL
        /// </summary>
        /// <param name="WithResponse">Czy zwraca rekordy</param>
        private void Execute(bool WithResponse)
        {
            //ToDo: Logować problematyczne zapytanie do komunikatu błędu
            try
            {
                ResponseClose();
                if (PodlaczDoBazy())
                {
                    if (WithResponse == true)
                    {
                        foreach (FbCommand Command in Commands)
                        {
                            Responses.Add(Command.ExecuteReader());
                            //Thread.Sleep(200);
                        }
                        if (Responses.Count > 0)
                        {
                            Response = Responses[0];
                        }
                    }
                    else
                    {
                        foreach (FbCommand Command in Commands)
                        {
                            if (!String.IsNullOrEmpty(Command.CommandText))
                            {
                                Command.ExecuteNonQuery();
                            }
                        }
                        CommitTransaction();
                        CommandsClose();
                        DataBaseClose();
                    }
                }
                else
                {
                    Response = null;
                    Responses = null;
                }
            }
            catch (Exception ex)
            {
                MyShowMessage($"Problem z wykonaniem {Commands.Count} zapytań(nia): {ex}");
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

                    FirebirdSql.Data.Isql.FbBatchExecution fbe = new FirebirdSql.Data.Isql.FbBatchExecution(new FbConnection(GetConnectionString()), fbScript);                                      
                    string scriptParsed = fbScript.ToString();
                    //for (int i = 0; i < fbScript.Results.Count; i++)
                    //    fbe.SqlStatements.Add(fbScript.Results[i]);
                    //fbe.SqlStatements.Add(scriptParsed);
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

        private int GetFieldIndex(String FieldName)
        {
            Loguj("FbKlient__GetFieldIndex '" + FieldName + "'");
            try
            {
                int returned_value = Response.GetOrdinal(FieldName);
                if (returned_value < 0)
                {
                    MyShowMessage("Can not find field " + FieldName);
                }

                return returned_value;
            }
            catch (Exception ex)
            {
                MyShowMessage("Error in GetFieldIndex '" + FieldName + "' " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Odczytaj następny rekord
        /// </summary>
        /// <returns>Czy rekord istnieje</returns>
        public bool Read()
        {
            Loguj("FbKlient__Read");
            bool returned_value = false;
            if (Response != null)
            {
                try
                {
                    returned_value = Response.Read();
                    if (!returned_value && Responses.Count - 1 == ResponseId)
                    {
                        DataBaseClose();
                    }
                }
                catch (Exception ex)
                {
                    MyShowMessage("Error in sql read: " + ex.Message);
                }
            }
            return returned_value;
        }
        #region Pobieranie danych z klienta
        /// <summary>
        /// Zwraca wartość bool odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public bool GetBoolean(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to bool !");
                }

                return Response.GetBoolean(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting bool value from '" + FieldName + "' !\n" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Zwraca wartość byte odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public byte GetByte(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to byte !");
                }

                return Response.GetByte(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting byte value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Char odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public char GetChar(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to char !");
                }

                return Response.GetChar(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting byte value from '" + FieldName + "' !\n" + ex.Message);
            }
            return Convert.ToChar(0);
        }

        /// <summary>
        /// Zwraca wartość DateTime odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public DateTime GetDateTime(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to DateTime !");
                }

                return Response.GetDateTime(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting DateTime value from '" + FieldName + "' !\n" + ex.Message);
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Zwraca wartość Decimal odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public decimal GetDecimal(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Decimal !");
                }

                return Response.GetDecimal(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting Decimal value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Double odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public double GetDouble(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Double !");
                }

                return Response.GetDouble(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting Double value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Int16 odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public short GetInt16(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int16 !");
                }

                return Response.GetInt16(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting Int16 value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Int32 odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public int GetInt32(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int32 !");
                }

                return Response.GetInt32(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting Int32 value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Int64 odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public long GetInt64(String FieldName)
        {
            try
            {
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int64 !");
                }

                return Response.GetInt64(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting Int64 value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość String odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public string GetString(String FieldName)
        {
            try
            {
                //if (Response.IsDBNull(GetFieldIndex(FieldName)))
                //    throw new Exception("'" + FieldName + "' has NULL value, can't convert to string !");
                return Response.GetString(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting string value from '" + FieldName + "' !\n" + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// Zwraca wartość Object odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public object GetValue(String FieldName)
        {
            try
            {
                return Response.GetValue(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while getting value from '" + FieldName + "' !\n" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Zwraca informację czy pole odczytane z bazy danych ma wartość NULL czy inną
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Prawda/Fałsz</returns>
        public bool IsDBNull(String FieldName)
        {
            try
            {
                return Response.IsDBNull(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while checking if '" + FieldName + "' is null!\n" + ex.Message);
            }
            return false;
        }
        #region Załączniki
        /// <summary>
        /// Metoda pobiera załącznik z bazy danych z parametru blob o podanej nazwie i zapisuje w podanej ścieżce
        /// </summary>
        /// <param name="FieldName">Nazwa parametru</param>
        /// <param name="FileName">Ścieżka do pliku gdzie zapisać</param>
        public void GetFile(String FieldName, String FileName)
        {
                       
            try
            {
                //using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write)) // Writes the BLOB to a file
                Stream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);//) // Writes the BLOB to a file
                {
                    GetIntoStream(FieldName, ref fs);
                    fs.Close();
                }
            }
            catch
            {
                MyShowMessage("Error while writing blob parameter " + FieldName + " with file " + FileName);
            }

        }

        /// <summary>
        /// Metoda pobiera parametr o podanej nazwie i zapisuje w strumieniu
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="fs"></param>
        public void GetIntoStream(string FieldName, ref Stream fs)
        {
            //using (BinaryWriter bw = new BinaryWriter(fs)) // Streams the BLOB to the FileStream object.
            BinaryWriter bw = new BinaryWriter(fs);
            {
                int bufferSize = 100;                   // Size of the BLOB buffer.
                byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
                long retval;                            // The bytes returned from GetBytes.
                long startIndex = 0;                    // The starting position in the BLOB output.
                                                        // Reset the starting byte for the new BLOB.
                startIndex = 0;

                // Read the bytes into outbyte[] and retain the number of bytes returned.
                retval = Response.GetBytes(GetFieldIndex(FieldName), startIndex, outbyte, 0, bufferSize);

                // Continue reading and writing while there are bytes beyond the size of the buffer.
                while (retval == bufferSize)
                {
                    bw.Write(outbyte);
                    bw.Flush();

                    // Reposition the start index to the end of the last buffer and fill the buffer.
                    startIndex += bufferSize;
                    retval = Response.GetBytes(GetFieldIndex(FieldName), startIndex, outbyte, 0, bufferSize);
                }

                // Write the remaining buffer.
                bw.Write(outbyte, 0, (int)retval);
                bw.Flush();
                

                outbyte = null;
            }
        }

        /// <summary>
        /// Dodaje plik jako obiekt blob
        /// </summary>
        /// <param name="ParamName">nazwa parametru zapytania</param>
        /// <param name="FileName">ścieżka do pliku</param>
        public void SetFile(String ParamName, String FileName)
        {
            try
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    SetStreamParameter(ParamName, fs);
                }
            }
            catch
            {
                MyShowMessage($"Error while setting blob parameter {ParamName} with file {FileName} in SetFile");
            }
        }

        private void SetStreamParameter(string ParamName, Stream fs)
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] photo = br.ReadBytes((int)fs.Length);

                br.Close();

                fs.Close();

                Commands[QueryId].Parameters.Add(ParamName, FbDbType.Binary, photo.Length).Value = photo;
            }
        }

        /// <summary>
        /// Dodaje strumień jako obiekt blob
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="memoryStream"></param>
        public void SetFromStream(string ParamName, Stream memoryStream)
        {
            try
            {
                memoryStream.Position = 0;
                SetStreamParameter(ParamName, memoryStream);
            }
            catch
            {
                MyShowMessage($"Error while setting blob parameter {ParamName} in SetStream");
            }
        }
        #endregion


        #endregion


        #region MetodySystemowe
        void MyShowMessage(string message)
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
        #endregion
    }
}
