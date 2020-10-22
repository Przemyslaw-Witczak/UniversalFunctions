using System;
using System.Reflection;

namespace DataBaseUniversalFunctions
{
    /// <summary>
    /// Atrybut opisujący pole TextBox, ComboBox, DateTimePicker, CheckListBox w celu powiązania z parametrem zapytania bazodanowego
    /// </summary>
    public class SearchFieldAttribute : Attribute
    {
        public readonly string PropertyName;
        public readonly string ParameterName;
        public readonly string ListOfIndexesName;
        public readonly bool GetStringValueOfComboBox;
        public readonly string QueryDisplayLabel;

        /// <summary>
        /// Kontrolka formularza..
        /// </summary>
        public FieldInfo FormField;

        /// <summary>
        /// Pełny konstruktor, ale zawierający wartości domyślne pól
        /// </summary>
        /// <param name="propertyName">Nazwa właściwości z klasy filrtów</param>
        /// <param name="parameterName">Nazwa parametru w zapytaniu</param>
        /// <param name="listOfIndexesName">Lista indeksów dla pola ComboBox i w przyszłości CheckListBox, być może do usunięcia</param>
        /// <param name="getStringValueOfComboBox">Czy z pola ComboBox pobrać wartość string.. czyli obiekt Dictionary</param>
        /// <param name="queryDisplayLabel">Etykieta do wyświetlania na kwerendzie</param>
        public SearchFieldAttribute(string propertyName, string parameterName = "", string listOfIndexesName = "", bool getStringValueOfComboBox = false, string queryDisplayLabel = "")
        {
            PropertyName = propertyName;
            ParameterName = parameterName;
            ListOfIndexesName = listOfIndexesName;
            GetStringValueOfComboBox = getStringValueOfComboBox;
            QueryDisplayLabel = queryDisplayLabel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="parameterName"></param>
        public SearchFieldAttribute(string propertyName, string parameterName) : this(propertyName, parameterName, string.Empty)
        {

        }

        public SearchFieldAttribute(string propertyName) : this(propertyName, propertyName, string.Empty)
        {

        }

        public SearchFieldAttribute()
        {
        }
    }
}
