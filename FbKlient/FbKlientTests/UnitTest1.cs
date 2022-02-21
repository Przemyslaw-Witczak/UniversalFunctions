using FbKlientNameSpace;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;

namespace FbKlintTests
{
    [TestClass]
    public class UnitTest1
    {
        cParametryKonfiguracyjne konfig = null;
        void ConfigureConnection()
        {
            konfig = new cParametryKonfiguracyjne
            {
                DataBasePath = ConfigurationManager.AppSettings.Get("Database"),
                DbUser = ConfigurationManager.AppSettings.Get("DataBaseUsr"),
                DbPassword = ConfigurationManager.AppSettings.Get("DataBasePwd"),
                DbServer = ConfigurationManager.AppSettings.Get("DataBaseIp"),
                DbPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DataBasePort")),
                Embedded = Convert.ToBoolean(Convert.ToInt16(ConfigurationManager.AppSettings.Get("DataBaseServerType"))),
                Charset = "WIN1250",
                Dialect = 3,
                Pooling = false,
                PacketSize = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DataBasePacket_Size"))
            };
            FbKlient klient = new FbKlient();
            klient.ConfigureDataBase(konfig);
        }
        [TestMethod]
        public void MultipleInserts()
        {
            ConfigureConnection();
            using (FbKlient client = new FbKlient())
            {
                for (int queryId = 0; queryId < 100; queryId++)
                {
                    client.QueryId = queryId;
                    client.AddSQL("insert into tabela_insertow(data_wpisu, komunikat) values (:data, :komunikat)");
                    var komunikat = $"Wstawiam queryId={queryId}";
                    client.ParamByName("data", FbDbType.Date).Value = DateTime.Now;
                    client.ParamByName("komunikat", FbDbType.VarChar).Value = komunikat;
                }
                client.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void DeleteInserts()
        {
            ConfigureConnection();
            using (FbKlient client = new FbKlient())
            {                
                client.AddSQL("delete from tabela_insertow");                                    
                client.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void MultipleProcedureExecutesWithReturningValues()
        {
            ConfigureConnection();
            using (FbKlient client = new FbKlient())
            {
                for (int queryId = 0; queryId < 100; queryId++)
                {
                    client.QueryId = queryId;
                    client.AddSQL("SELECT * FROM PROC_RETURN_STRING(:komunikat_we)");
                    var komunikat = $"Wykonuję procedurę w queryId={queryId}";                    
                    client.ParamByName("komunikat_we", FbDbType.VarChar).Value = komunikat;
                }
                client.Execute();
                
                for (int responseId = 0; responseId < 100; responseId++)
                {
                    client.ResponseId = responseId;
                    while(client.Read())
                    {
                        var odpowiedzZBazyDanych = client.GetString("wyjscie");
                        var dataOdpowiedzi = client.GetDateTime("data_wy");
                        Console.WriteLine($"[{dataOdpowiedzi}]: Odpowiedź z bazy danych :{odpowiedzZBazyDanych}");
                    }
                }
            }
        }
    }
}
