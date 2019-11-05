using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DataBaseUniversalFunctions.Abstract
{
    /// <summary>
    /// Klasa bazowa filtrów wyszukiwania
    /// </summary>
    public class SearchFieldsParametersBase    
    {
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
        public SearchFieldsParametersBase()
        {
            ClearFields();
        }

        public void SetFieldsValuesFrom(Form searchForm)
        {
            var searchFormFields = GetSearchFieldsFrom(searchForm);
            foreach(var searchFormField in searchFormFields)
            {
                var attributes = searchFormField.GetCustomAttributes(typeof(SearchFieldAttribute), false);
                SearchFieldAttribute attribute = (attributes[0] as SearchFieldAttribute);
                var searchFieldNameAsignedToControl = attribute.PropertyName;
                Type myType = this.GetType();
                FieldInfo searchField = myType.GetField(searchFieldNameAsignedToControl);
                var control = searchFormField.GetValue(searchForm);
                object newValue = null;
                if (control is ComboBox)
                {
                    if ((control as ComboBox).SelectedIndex > -1)
                    {
                        if (string.IsNullOrEmpty(attribute.ListOfIndexesName))
                            newValue = (control as ComboBox).SelectedIndex;
                        else
                        {
                            List<int> listOfIndexes = (searchForm.GetType()?.GetField(attribute.ListOfIndexesName)?.GetValue(searchForm)) as List<int>;
                            if (listOfIndexes != null)
                                newValue = listOfIndexes[(control as ComboBox).SelectedIndex];

                        }
                    }
                    else
                    {
                        newValue = -1;
                    }
                }
                else if (control is TextBox && !string.IsNullOrEmpty((control as TextBox).Text))
                {
                    newValue = (control as TextBox).Text.Trim().ToUpper();
                }
                else if (control is DateTimePicker && (control as DateTimePicker).Checked)
                {
                    newValue = (control as DateTimePicker).Value;
                }
                if (newValue!=null)
                    searchField.SetValue(this, newValue);
            }
        }

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
    }
}
