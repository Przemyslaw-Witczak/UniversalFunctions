using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseUniversalFunctions
{
    /// <summary>
    /// Atrybut do nadawania filtrom typu DictionaryListItem, pomaga powiązać z listą wartości w celu odczytania pozycji wyszukiwania
    /// </summary>
    public class DictionaryListItemValuesAttribute : Attribute
    {
        /// <summary>
        /// Nazwa listy z pozycjami słownikowymi powiązanymi z polem opisanym atrybutem
        /// </summary>
        public readonly string DictionaryListName;
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dictionaryListName">Nazwa słownika</param>
        public DictionaryListItemValuesAttribute(string dictionaryListName)
        {
            DictionaryListName = dictionaryListName;
        }
    }
}
