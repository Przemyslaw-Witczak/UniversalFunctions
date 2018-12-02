using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FbKlientNameSpace
{
    [Serializable]
    public class cParametryKonfiguracyjne
    {
        public cParametryKonfiguracyjne()
        {
            DbUser = "SYSDBA";
            DbPassword = "masterkey";
            DbServer = "127.0.0.1";
            DbPort = 3050;
            Dialect = 3;
            Charset = "NONE";
            Role ="";
            ConnectionLifeTime = 15;
            Pooling = true;
            MinPoolSize = 0;
            MaxPoolSize = 50;
            PacketSize = 8192;
            Embedded = false;
            QueriesLog = false;
        }

        #region PolaKonfiguracyjne
        [XmlAttribute]
        public string DataBasePath;
        [XmlAttribute]
        public string DbUser;
        [XmlAttribute]
        public string DbPassword;
        [XmlAttribute]
        public string DbServer;
        [XmlAttribute]
        public int DbPort;
        [XmlAttribute]
        public int Dialect;
        [XmlAttribute]
        public string Charset;
        [XmlAttribute]
        public string Role;
        [XmlAttribute]
        public int ConnectionLifeTime;
        [XmlAttribute]
        public bool Pooling;
        [XmlAttribute]
        public int MinPoolSize;
        [XmlAttribute]
        public int MaxPoolSize;
        [XmlAttribute]
        public int PacketSize;
        [XmlAttribute]
        public bool Embedded;
        [XmlAttribute]
        public bool QueriesLog;
    #endregion
    }
}
