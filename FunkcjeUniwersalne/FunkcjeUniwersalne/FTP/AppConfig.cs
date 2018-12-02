using System;

namespace MojeFunkcjeUniwersalneNameSpace.FTP
{
    /// <summary>
    /// Klasa domyślnych ustawień dla klasy FTP
    /// </summary>
    internal class AppConfig
    {
        public static bool FtpUsePassive
        {
            get
            {
                return Convert.ToBoolean(MySetup.Instance.GetParam("FTP", "FtpUsePassive", Convert.ToString(false)));
            }

            internal set
            {
                MySetup.Instance.SetParam("FTP", "FtpUsePassive", Convert.ToString(value));
            }
        }
            
        public static bool FtpUseSSL
        {
            get
            {
                return Convert.ToBoolean(MySetup.Instance.GetParam("FTP", "FtpUseSSL", Convert.ToString(false)));
            }

            internal set
            {
                MySetup.Instance.SetParam("FTP", "FtpUseSSL", Convert.ToString(value));
            }
        }
    }
}