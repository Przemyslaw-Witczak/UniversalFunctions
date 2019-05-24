namespace Aktualizacje
{
    /// <summary>
    /// Skrypt
    /// </summary>
    public class AktualizacjaSkrypt
    {
        /// <summary>
        /// Wersja od której obowiązuje skrypt, od wersji 1.0.7.3 jest to wersja docelowa
        /// </summary>
        public string Wersja;

        /// <summary>
        /// Skrypt SQL zawierający aktualizację bazy danych
        /// </summary>
        public string Skrypt;

        public AktualizacjaSkrypt()
        {
            Wersja = string.Empty;
            Skrypt = string.Empty;
        }
    }
}