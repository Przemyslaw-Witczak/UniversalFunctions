using System;
using System.Xml.Serialization;

namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Klasa odpowiada parametrowi konfiguracji
    /// </summary>
    [Serializable]
    public class cParametr : IDisposable
    {
      
        /// <summary>
        /// Nazwa parametru
        /// </summary>
        [XmlAttribute("Nazwa")]
        public string Parametr;

        /// <summary>
        /// Wartość parametru
        /// </summary>
        [XmlAttribute]
        public string Wartosc;

        /// <summary>
        /// Czy zapis zmiany parametru
        /// </summary>
        [XmlIgnore]
        public bool Zmiana;

        protected bool _disposed;

        public cParametr()
        {
            Parametr = string.Empty;
            Wartosc = string.Empty;
            Zmiana = false;
            _disposed = false;
        }

        ~cParametr()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (!_disposed)
            {
                if (dispose)
                {
                    // Miejsce do zwalniania zasobów zarządzalnych                               
                    //_objectToDispose.Dispose();
                    _disposed = true;
                }
                //Miejsce do zwalniania zasobów niezarządzalnych 
            }


        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}