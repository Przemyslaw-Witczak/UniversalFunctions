using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Klasa przechodująca konfiguracje, z podziałem na sekcje, parametry oraz wartości
    /// </summary>
    /// <remarks>Klasa przechodująca konfiguracje, w wykorzystaniu podobna do plików INI, jednak zapis konfiguracji w plikach xml</remarks>
    [Serializable]    
    public class cKonfiguracja : IDisposable 
    {
        /// <summary>
        /// Nazwa sekcji
        /// </summary>
        [XmlAttribute("Nazwa")]
        public string Sekcja;
        
        /// <summary>
        /// Parametry
        /// </summary>
        [XmlElement("Parametr")]
        public List<cParametr> Parametry;
        
        /// <summary>
        /// Czy zapis zmiany parametru
        /// </summary>
        [XmlIgnore]
        public bool Zmiana;
        
        protected bool _disposed;

        public cKonfiguracja()
        {
            Sekcja = string.Empty;
            Parametry = new List<cParametr>();           
            Zmiana = false;
            _disposed = false;
        }

        ~cKonfiguracja()
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
                    foreach (cParametr parametr in Parametry)
                    {
                        parametr.Dispose();
                    }

                    Parametry.Clear();
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