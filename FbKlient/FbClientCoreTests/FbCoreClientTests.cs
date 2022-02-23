using FbCoreClientNameSpace;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace vl53l0xMeasurmentApp.Tests
{
    [TestClass()]
    public class FbCoreClientTests
    {
        private const string _connectionString = @"User=SYSDBA;Password=masterkey;Database=d:\Moje Programy GIT\Funkcje Uniwersalne\FbKlient\FbKlientTests\database\MSKLIENT_TESTS.FDB; DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=NONE;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;PacketSize=8192;ServerType = 0;";
        

        [TestMethod]
        public void DeleteInserts()
        {            
            using (var client = new FbCoreClient(_connectionString))
            {
                client.AddSQL("delete from tabela_insertow");
                client.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void SingleInsert()
        {
            using (var client = new FbCoreClient(_connectionString))
            {
                client.AddSQL("insert into tabela_insertow(data_wpisu, komunikat) values (@data, @komunikat)");
                var komunikat = $"Wstawiam jeden rekord {DateTime.Now}";
                client.ParamByName("data", FbDbType.Date).Value = DateTime.Now;
                client.ParamByName("komunikat", FbDbType.VarChar).Value = komunikat;

                client.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void SingleSelect()
        {
            using (var client = new FbCoreClient(_connectionString))
            {
                client.AddSQL("SELECT first 1 komunikat from tabela_insertow order by data_wpisu desc");
                client.Execute();
                while (client.Read())
                {
                    Debug.WriteLine(client.GetString("komunikat"));
                }
            }
        }
    }
}