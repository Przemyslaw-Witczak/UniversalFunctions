using System;

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

        public SearchFieldAttribute(string propertyName, string parameterName = "", string listOfIndexesName = "", bool getStringValueOfComboBox = false)
        {
            PropertyName = propertyName;
            ParameterName = parameterName;
            ListOfIndexesName = listOfIndexesName;
            GetStringValueOfComboBox = getStringValueOfComboBox;
    }

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
