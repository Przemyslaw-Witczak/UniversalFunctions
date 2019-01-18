using ISqlKlientNameSpace;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySQLWrapper
{
    public class MySqlKlient : IDisposable, ISqlKlient
    {

        public int CommandsCount => throw new NotImplementedException();

        public int QueryId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ResponseId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private string _dataBaseConnectionString;

        public MySqlKlient(string dataBaseConnectionString)
        {
            this._dataBaseConnectionString = dataBaseConnectionString;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MySqlKlient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
        private MySqlConnection _connection;
        private MySqlCommand _command;
        private MySqlDataReader _reader;

        private string GetConnectionString()
        {
            return _dataBaseConnectionString;
        }

        public void AddSQL(string SqlString)
        {
            _connection = new MySqlConnection(_dataBaseConnectionString);
            _connection.Open();
            _command = new MySqlCommand(SqlString, _connection);
            
        }

        public void ConfigureDataBase()
        {
            throw new NotImplementedException();
        }

        public void DataBaseClose()
        {
            throw new NotImplementedException();
        }

        public void ExecuteNonQuery()
        {
            Execute(false);
        }

        private void Execute(bool WithResponse)
        {
            try
            {
                //ResponseClose();
                //if (PodlaczDoBazy())
                {
                    if (WithResponse == true)
                    {
                        _reader = _command.ExecuteReader();                        
                    }
                    else
                    {
                        _command.ExecuteNonQuery();
                        _command.Dispose();
                        GC.WaitForPendingFinalizers();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MyShowMessage($"Problem z wykonaniem zapytania: {ex}");
            }
        }

        public bool GetBoolean(string FieldName)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(string FieldName)
        {
            throw new NotImplementedException();
        }

        public char GetChar(string FieldName)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(string FieldName)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(string FieldName)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(string FieldName)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(string FieldName)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(string FieldName)
        {
            return _reader.GetInt32(_reader.GetOrdinal(FieldName));
        }

        public long GetInt64(string FieldName)
        {
            throw new NotImplementedException();
        }

        public string GetString(string FieldName)
        {
            return _reader.GetString(_reader.GetOrdinal(FieldName));
        }

        public object GetValue(string FieldName)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(string FieldName)
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public void SetNull(string paramName)
        {
            ParamByName(paramName, MySqlDbType.Int16).Value = DBNull.Value;
        }

        public MySqlParameter ParamByName(String paramName, MySqlDbType paramType)
        {
            try
            {
                paramName = paramName.Replace(":", "@");

                if (!paramName.Contains("@"))
                    paramName = "@" + paramName;

                MySqlParameter returned_param = null;
                foreach (MySqlParameter param in _command.Parameters)
                {
                    if (param.ParameterName.Equals(paramName))
                    {
                        returned_param = param;
                        returned_param.MySqlDbType = paramType;
                        break;
                    }
                }

                if (returned_param == null)
                {
                    returned_param = _command.Parameters.Add(paramName, paramType);
                }

                return returned_param;
            }
            catch (Exception ex)
            {
                MyShowMessage("Error while setting parameter " + paramName + " " + ex.Message);
                return null;
            }
        }

        #region MetodySystemowe
        void MyShowMessage(string message)
        {            
            //if (ParentForm != null)
            {
                MessageBox.Show(message);
            }
            //else
            //{
            //    throw new Exception(message);
            //}
        }

        public void Execute()
        {
            Execute(true);
        }
        #endregion

    }
}
