﻿namespace DataBaseUniversalFunctions.Model
{
    /// <summary>
    /// Klasa opisująca wartość słownikową wyświetlaną w polu kombi lub na liście
    /// </summary>
    public class DictionaryListItem
    {
        /// <summary>
        /// Indeks, klucz główny z tabeli słownikowej
        /// </summary>
        public int Identity { get; set; }
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

    }
}
