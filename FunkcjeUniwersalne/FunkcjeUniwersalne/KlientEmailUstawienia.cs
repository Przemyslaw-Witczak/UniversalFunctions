using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MojeFunkcjeUniwersalneNameSpace
{
    public sealed class KlientEmailUstawienia
    {
        public string nadawca;
        public string adresat;
        public string temat;
        public string serwer;
        public string login;
        public string haslo;//WfG0UuxgpaU4aFEjANlT
        public int port;

        private static volatile KlientEmailUstawienia instance;
        private static object syncRoot = new Object();
        public bool SendEmail = false;
        private KlientEmailUstawienia() 
        {
            
        }
        #region Konstruktor i destruktor

        public static KlientEmailUstawienia Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new KlientEmailUstawienia();
                    }
                }

                return instance;
            }
        }
        #endregion
        
    }
}
