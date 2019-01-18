using ISqlKlientNameSpace;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLWrapper
{
    public class MySqlKlient : IDisposable, ISqlKlient
    {

        public int CommandsCount => throw new NotImplementedException();

        public int QueryId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ResponseId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        
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

        private static string GetConnectionString()
        {            
            return $"User = {ConfigurationManager.AppSettings["MySqlUserName"]}; Password = {ConfigurationManager.AppSettings["MySqlUserPassword"]};Database={ConfigurationManager.AppSettings["MySqlDataBase"]};Server={ConfigurationManager.AppSettings["MySqlServer"]};Port={ConfigurationManager.AppSettings["MySqlPort"]};";
        }

        public void AddSQL(string SqlString)
        {
            throw new NotImplementedException();
        }

        public void ConfigureDataBase()
        {
            throw new NotImplementedException();
        }

        public void DataBaseClose()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void ExecuteNonQuery()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public long GetInt64(string FieldName)
        {
            throw new NotImplementedException();
        }

        public string GetString(string FieldName)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetNull(string paramName)
        {
            throw new NotImplementedException();
        }
        
    }
}
