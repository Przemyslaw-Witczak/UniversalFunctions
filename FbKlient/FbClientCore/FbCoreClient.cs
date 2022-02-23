using FbClientBase.Abstract;
using FbClientBase.Extensions;
using FbClientBaseNameSpace;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Text;

namespace FbCoreClientNameSpace
{
    public class FbCoreClient : FbAbstractClient, IDisposable, ISqlSimpleClient
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly FbConnection _connection;
        private FbTransaction _transaction;
        private FbCommand _command;
        private FbDataReader _reader;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_transaction != null)
                        _transaction.Commit();

                    // TODO: dispose managed state (managed objects)
                    if (_connection !=null)
                    {

                        if (_connection.State == System.Data.ConnectionState.Open)
                            _connection.Close();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FbDataBaseService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Konstruktor klasy fasady połączenia do bazy danych Firebird dla aplikacji .Net Core
        /// </summary>
        /// <param name="ConnectionString">Ciąg połaczeniowy do bazy danych</param>
        public FbCoreClient(string ConnectionString)
        {
            _connection = new FbConnection(ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// <inheritdoc cref="ISqlSimpleClient.AddSQL(string)"/>
        /// </summary>        
        public void AddSQL(string Query)
        {
            Query = Query.Replace(":", "@");

            if (_command == null)
                _command = new FbCommand(Query, _connection, _transaction);
            else
                _command.CommandText += Query;

        }        

        #region Wykonaj zapytanie do bazy
        /// <summary>
        /// Metoda wykonuje zapytanie SQL
        /// </summary>
        /// <param name="WithResponse">Czy zwraca rekordy</param>
        private void Execute(bool WithResponse)
        {
            var commandString = new StringBuilder();
            if (_command == null)
                throw new Exception("There is no query set to execute!");
            try
            {                
                _command.GetCommandLog(commandString);
                if (WithResponse == true)
                {                                      
                    _reader = _command.ExecuteReader();                                            
                }
                else
                {                                                                                
                    _command.ExecuteNonQuery();                                        
                    CommitTransaction();
                    CommandsClose();
                    DataBaseClose();
                }                
                
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Execute: Problem z wykonaniem zapytania: {commandString}.");
                throw;
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
        #endregion

        /// <summary>
        /// Metoda zamykająca transakcję
        /// </summary>
        public void DataBaseClose()
        {
            logger.Debug("FbKlient__DataBaseClose");
            ResponseClose();

            CommandsClose();

            CommitTransaction();
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();                                
            }

            logger.Debug("FbKlient__DataBaseClose-Koniec");
        }
        private void ResponseClose()
        {
            logger.Debug("FbKlient__ResponseClose");
            
            if (_reader == null)
            {
                return;
            }                        
            else
            {
                if (_reader.IsClosed == false)
                {
                    _reader.Close();
                }

                _reader.Dispose();                
            }                        
            logger.Debug("FbKlient__ResponseClose-Koniec");
        }
        private void CommandsClose()
        {
            logger.Debug("FbKlient__CommandsClose");
            if (_command != null)
            {

                _command.Dispose();
                _command = null;
                GC.WaitForPendingFinalizers();                
            }
            logger.Debug("FbKlient__CommandsClose-Koniec");
        }
        private void CommitTransaction()
        {
            logger.Debug("FbKlient__CommitTransaction");
            if (_transaction != null)
            {
                try
                {
                    _transaction.Commit();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error in FbKlient__CommitTransaction while try Transaction.Commit();");
                }
                _transaction.Dispose();
                _transaction = null;
            }
        }                

        /// <summary>        
        /// <inheritdoc cref="ISqlSimpleClient.Read"/>
        /// </summary>        
        public bool Read()
        {
            logger.Debug("FbKlient__Read");
            bool returned_value = false;
            if (_reader != null)
            {
                try
                {
                    returned_value = _reader.Read();
                    if (!returned_value)
                    {
                        DataBaseClose();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error in sql read.");
                    throw;
                }
            }
            return returned_value;
        }

        /// <summary>
        /// <inheritdoc cref="FbClientBase.GetCurrentCommand"/>
        /// </summary>        
        protected override FbCommand GetCurrentCommand()
        {
            return _command;
        }

        /// <summary>
        /// <inheritdoc cref="FbClientBase.GetCurrentResponse"/>
        /// </summary>        
        protected override FbDataReader GetCurrentResponse()
        {
            return _reader;
        }

        /// <summary>
        /// <inheritdoc cref="FbClientBase.ExceptionLogOrMessage(string)"/>
        /// </summary>        
        protected override void ExceptionLogOrMessage(string message)
        {
            logger?.Debug(message);
        }
    }
}
