namespace DataBaseUniversalFunctions.Model
{
    /// <summary>
    /// Klasa opisująca wartość słownikową wyświetlaną w polu kombi lub na liście
    /// </summary>
    public class DictionaryListItem
    {
        /// <summary>
        /// Indeks, klucz główny z tabeli słownikowej, zamiennie z IdentityKey
        /// </summary>
        public int Identity { get; set; }

        /// <summary>
        /// Indeks, klucz główny z tabeli słownikowej, zamiennie z Identity
        /// </summary>
        public string IdentityKey { get; set; }

        /// <summary>
        /// Wartość wyświetlana na liście w polu kombi
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Wskaźnik na dodatkowe dane..
        /// </summary>
        public object AdditionalData { get; set; }

        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Tworzy nowy obiekt na podstawie klucza i wartości
        /// </summary>
        /// <param name="identity">Indeks z tabeli słownikowej</param>
        /// <param name="value">Nazwa wartości</param>
        /// <returns></returns>
        public static DictionaryListItem Create(int identity, string value)
        {
            return new DictionaryListItem()
            {
                Identity = identity,
                Value = value
            };
        }

        /// <summary>
        /// Tworzy nowy obiekt na podstawie klucza i wartości
        /// </summary>
        /// <param name="identity">Klucz będący ciągiem znaków z tabeli słownikowej</param>
        /// <param name="value">Nazwa wartości</param>
        /// <returns></returns>
        public static DictionaryListItem Create(string identity, string value)
        {
            return new DictionaryListItem()
            {
                IdentityKey = identity,
                Value = value
            };
        }
    }
}
