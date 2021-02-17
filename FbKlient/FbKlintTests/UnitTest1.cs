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
        public void TestMethod1()
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
    }
}
