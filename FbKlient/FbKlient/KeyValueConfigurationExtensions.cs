using System.Configuration;

namespace FbKlientNameSpace
{
    /// <summary>
    /// Metody rozszerzające wykorzystywane przy zmianach w konfiguracji aplikacji
    /// </summary>
    public static class KeyValueConfigurationExtensions
    {
        /// <summary>
        /// Ustaw parametr konfiguracyjny lub dodaj
        /// </summary>
        /// <param name="collection">Konfiguracja aplikacji</param>
        /// <param name="key">Klucz</param>
        /// <param name="value">Nowa wartość</param>
        public static void SetKeyValue(this KeyValueConfigurationCollection collection, string key, string value)
        {
            if (collection[key] == null)
            {
                collection.Add(key, value.Trim());
            }
            else
            {
                collection[key].Value = value.Trim();
            }
        }
    }
}
