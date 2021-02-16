using DataBaseUniversalFunctions.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DataBaseUniversalFunctions.Abstract
{
    /// <summary>
    /// Klasa bazowa filtrów wyszukiwania
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class SearchFieldsParametersBase    
    {
        /// <summary>
        /// Formatka na której są filtry
        /// </summary>
        private readonly Form _parentForm;
        /// <summary>
        /// Metoda czyści wartości pól filtrów klasy implementującej klasę bazową
        /// </summary>
        public void ClearFields()
        {
            try
            {
                var properties = this.GetType().GetFields();

                foreach (var property in from property in properties
                                         let oldValue = property.GetValue(this)
                                         where oldValue == null
                                         select property)
                {
                    property.SetValue(this, GetDefaultValue(property));
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error in SearchFieldsParametersBase.ClearFields on {this.ToString()}!");
            }

        }

        /// <summary>
        /// Metoda zwracająca wartości domyślne filtrów w zależności od typu parametru
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object GetDefaultValue(System.Reflection.FieldInfo value)
        {
            try
            {
                if (value.FieldType == typeof(string))
                    return "";

                if (value.FieldType == typeof(bool))
                    return false;

                if (value.FieldType == typeof(int))
                    return 0;

                if (value.FieldType == typeof(decimal))
                    return 0;

                if (value.FieldType == typeof(DateTime))
                    return DateTime.FromBinary(0);
            }
            catch (Exception)
            {
                throw new Exception("Error in SearchFieldsParametersBase.GetDefaultValue!");
            }

            return null;
        }

        /// <summary>
        /// Konstruktor klasy bazowej
        /// </summary>
        public SearchFieldsParametersBase(Form parentForm)
        {
            _parentForm = parentForm;
            ClearFields();
        }

        /// <summary>
        /// Ustawia wartości dla obiektu filtrów dziedziczącego po klasie bazowej filtrów, pobiera wartości z kontrolek formularza, opisanych atrybutem
        /// </summary>
        /// <param name="searchForm"></param>
        public void SetFieldsValuesFrom(Form searchForm)
        {
            Type myType = this.GetType();
            var searchFormFields = GetSearchFieldsFrom(searchForm);
            Debug.WriteLine($"Formatka: Type={myType.Name}, znaleziono {searchFormFields.Count} kontrolek opisanych parametrem.");
            foreach (var searchFormField in searchFormFields)
            {                 
                var newValue = GetControlValue(searchFormField, out FieldInfo searchField, out _);

                if (newValue != null)
                {
                    if (searchField.FieldType == typeof(int))
                    {
                        if (newValue.GetType() == typeof(DictionaryListItem) && searchField.FieldType == typeof(int))
                            searchField.SetValue(this, (newValue as DictionaryListItem).Identity);
                        else
                            searchField.SetValue(this, Convert.ToInt32(newValue));
                    }                                            
                    else
                        searchField.SetValue(this, newValue);
                }
                else if (newValue == null && (searchField.FieldType == typeof(int)))
                    searchField.SetValue(this, -1);

            }
        }

        /// <summary>
        /// Metoda zwraca kontrolki formularza, które posiadają atrybut: SearchFieldAttribute
        /// </summary>
        /// <param name="searchForm">Formularz</param>
        /// <returns></returns>
        private List<FieldInfo> GetSearchFieldsFrom(Form searchForm)
        {
            Type formType = searchForm.GetType();
            
            var allControls2 = searchForm.GetType().GetFields();

            if (allControls2.Count() == 0)
                return null;
            List<FieldInfo> returnedProperties = new List<FieldInfo>();
            foreach (var control in allControls2)
            {
                var attributes = control.GetCustomAttributes(typeof(SearchFieldAttribute), false);
                if (attributes.Count()>0)
                    returnedProperties.Add(control);    
            }
            return returnedProperties;
        }

        public string ToString(Form searchForm)
        {
            var query = new StringBuilder();
            var controls = GetSearchFieldsFrom(searchForm);
            foreach(var control in controls)
            {
                var fieldValue = GetControlValue(control, out _, out string stringValue);                
                if (!string.IsNullOrEmpty(stringValue))
                    query.Append($"{stringValue}; "); 
            }
            if (query.Length == 0)
                return "Wszystko";
            return query.ToString();
        }

        /// <summary>
        /// Zwraca wartość kontrolki
        /// </summary>
        /// <param name="searchFormField">Pole formularza, opisane atrybutem wyszukiwania</param>
        /// <param name="searchField">Pole filtru do przypisania wartości</param>
        /// <param name="displayValue">Pole tekstowe z wartością filtru wyszukiwania i etykietą</param>
        /// <returns></returns>
        private object GetControlValue(FieldInfo searchFormField, out FieldInfo searchField, out string displayValue)
        {
            displayValue = string.Empty;
            var attributes = searchFormField.GetCustomAttributes(typeof(SearchFieldAttribute), false);
            SearchFieldAttribute attribute = (attributes[0] as SearchFieldAttribute);
            var searchFieldNameAsignedToControl = attribute.PropertyName;
            Type myType = this.GetType();
            searchField = myType.GetField(searchFieldNameAsignedToControl);
            var control = searchFormField.GetValue(_parentForm);
            object newValue = null;

            Debug.WriteLine($"Kontrolka: {control.GetType().Name}:{(control as Control).Name} -> {searchField.Name}");
            
            if (control is ComboBox)
            {
                if ((control as ComboBox).SelectedIndex > -1)
                {
                    displayValue = $"{attribute.QueryDisplayLabel}: {(control as ComboBox).Text}";
                    if (attribute.GetStringValueOfComboBox)
                    {
                        newValue = (control as ComboBox).SelectedItem;                      
                    }
                    else if (string.IsNullOrEmpty(attribute.ListOfIndexesName))
                    {
                        newValue = (control as ComboBox).SelectedIndex;                        
                    }
                    else
                    {
                        List<int> listOfIndexes = (_parentForm.GetType()?.GetField(attribute.ListOfIndexesName)?.GetValue(_parentForm)) as List<int>;
                        if (listOfIndexes != null)
                        {
                            newValue = listOfIndexes[(control as ComboBox).SelectedIndex];                            
                        }
                    }
                }
                else
                {
                    if (attribute.GetStringValueOfComboBox)
                        newValue = null;
                    else
                        newValue = -1;
                }
            }
            else if (control is TextBox && !string.IsNullOrEmpty((control as TextBox).Text))
            {
                newValue = (control as TextBox).Text.Trim().ToUpper();
                displayValue = $"{attribute.QueryDisplayLabel}: {newValue}";
            }
            else if (control is DateTimePicker && (control as DateTimePicker).Checked)
            {
                newValue = (control as DateTimePicker).Value;                
                displayValue = $"{attribute.QueryDisplayLabel}: {(control as DateTimePicker).Value.ToShortDateString()}";
            }
            else if (control is CheckBox)
            {
                newValue = (control as CheckBox).Checked;
                if ((bool)newValue == true)
                    displayValue = $"{attribute.QueryDisplayLabel}";
            }
            else if (control is CheckedListBox)
            {
                newValue = new List<DictionaryListItem>();
                var checkedValues = (control as CheckedListBox).CheckedItems.OfType<DictionaryListItem>();
                if (checkedValues.Count() > 0)
                {
                    (newValue as List<DictionaryListItem>).AddRange(checkedValues);
                    displayValue = $"{attribute.QueryDisplayLabel}: {string.Join(", ", (newValue as List<DictionaryListItem>))}";
                }
            }
            //else
            //{
            //    throw new Exception($"Nie obsłużono filtru wyszukiwania dla kontrolki {control.GetType()} w {myType}!");
            //}
            Debug.WriteLine($"Filtr wyszukiwania: {displayValue}");
            return newValue;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
