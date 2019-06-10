using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MojeFunkcjeRozszerzajace
{

    public static class StringExtensions
    {
        /// <summary>
        /// Zwraca liczbę znaków w ciągu
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int WordCount(this string str)
        { return str.Split(new char[] { ' ' }).Length; }

        public static string SubStringEx(this string value, int startIndex, int length)
        {
            if (length + startIndex > value.Length)
            {
                length = value.Length - startIndex;
            }

            if (startIndex < 0) startIndex = 0;
            if (length < 0) return string.Empty;
            
            
            try
            {
                

                return value.Substring(startIndex, length);

            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new Exception($"SubStringEx::{e.Message}");
            }

        }
    }

    public static class ByteExtensions
    {
        public static string ToString2(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

    }

    public static class DecimalExtensions
    {
        public static string ToString3Places(this decimal value)
        {
            string strValue;
            //strValue = String.Format("{0:#,#.000}",  value);
            string decimalSeparator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator; ;
            //strValue = string.Format("{0:#,0.000}", value);
            strValue = string.Format("{0:#"+ decimalSeparator + "0.000}", value);
            return strValue;
        }

        public static string ToString2Places(this decimal value)
        {
            string strValue;
            //strValue = String.Format("{0:#,#.000}",  value);
            strValue = string.Format("{0:#,0.00}", value);
            return strValue;
        }
    }

    public static class LongExtensions
    {
        public static string ToFileSizeString(this long value)
        {
            string formatedValue;

            if (value < 1024)
                formatedValue = $"{value} B";
            else if (value / 1024 < 1024)
                formatedValue = $"{(value / 1024)} kB";
            else if (value / 1024 / 1024 < 1024)
                formatedValue = $"{(value / 1024f / 1024f).ToString("F")} MB";
            else if (value / 1024 / 1024 / 1024 < 1024)
                formatedValue = $"{(value / 1024f / 1024f / 1024f).ToString("F")} GB";
            else 
                formatedValue = $"{(value / 1024f / 1024f / 1024f / 1024f).ToString("F")} TB";


            return formatedValue;
        }

    }

    /// <summary>
    /// Klasa metod rozszerzających wartości boolowskie
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        /// Zwraca wartość TAK lub NIE
        /// </summary>
        /// <param name="value">Wartość konwertowana</param>
        /// <returns></returns>
        public static string TakNie(this bool value)
        {
            if (value)
                return "TAK";
            else
                return "NIE";
        }
    }

    /// <summary>
    /// Klasa metod rozszerzających formularze
    /// </summary>
    public static class FormExtensions
    {
        /// <summary>
        /// Wyświetla okno jako MDI Child zmaksymalizowane
        /// </summary>
        /// <param name="forma">Forma</param>
        /// <param name="parentForm">Rodzic</param>
        public static void ShowAsMaximized(this Form forma, Form parentForm)
        {
            if (parentForm.IsMdiContainer)
                forma.MdiParent = parentForm;
            else
                forma.MdiParent = parentForm.MdiParent;
            forma.Show();        
            forma.WindowState = FormWindowState.Maximized;
        }

        public static List<Control>GetAllControlsRecursive(this Form form)
        {
            var outputList = new List<Control>();
            outputList.AddRange(GetAllControls(form.Controls));
            return outputList;
        }

        private static List<Control> GetAllControls(Control.ControlCollection ParentControls)
        {
            var outputList = new List<Control>();
            foreach (Control kontrolka in ParentControls)
            {
                if (kontrolka is DataGridView
                  || kontrolka is TextBox
                  || kontrolka is RadioButton
                  || kontrolka is CheckBox
                  || kontrolka is DateTimePicker
                  //|| kontrolka is Splitter
                  || kontrolka is ComboBox
                  || kontrolka is CheckedListBox
                     )
                    outputList.Add(kontrolka);
                else if (kontrolka is TabPage)
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                else if (kontrolka is TabControl)
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                else if (kontrolka is Panel || kontrolka is SplitterPanel)
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                else if (kontrolka is GroupBox)
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                else if (kontrolka is SplitContainer)
                {
                    outputList.Add(kontrolka);
                    outputList.AddRange(GetAllControls((kontrolka as SplitContainer).Panel1.Controls));
                    outputList.AddRange(GetAllControls((kontrolka as SplitContainer).Panel2.Controls));
                }
                else if (kontrolka is Splitter)
                {
                    outputList.Add(kontrolka);
                }
                else if (kontrolka is UserControl)
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
            }
            return outputList;
        }
    }


    /// <summary>
    /// Klasa metod rozszerzających dla komponentu DataGridView
    /// </summary>
    public static class DataGridViewExtensions
    {

        /// <summary>
        /// Metoda ustawiająca zaznaczony wiersz w połowie listy wyświetlanych wierszy
        /// </summary>
        /// <param name="dataGridView"></param>
        public static void ScrollGrid(this DataGridView dataGridView)
        {
            int halfWay = (dataGridView.DisplayedRowCount(false) / 2);
            if (dataGridView.SelectedRows.Count>0 &&                 
                (dataGridView.FirstDisplayedScrollingRowIndex + halfWay > dataGridView.SelectedRows[0].Index ||
                (dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.DisplayedRowCount(false) - halfWay) <= dataGridView.SelectedRows[0].Index))
            {
                int targetRow = dataGridView.SelectedRows[0].Index;

                targetRow = Math.Max(targetRow - halfWay, 0);
                dataGridView.FirstDisplayedScrollingRowIndex = targetRow;

            }
        }


        public static DataGridViewRow AddRow(this DataGridViewRowCollection dataGridViewRowCollection, params string [] columnsValues)
        {
            int rowIndex = -1;
            rowIndex = dataGridViewRowCollection.Add(columnsValues);
            if (rowIndex > -1)
                return dataGridViewRowCollection[rowIndex];
            else
                return null;
        }
    }

    /// <summary>
    /// Klasa metod rozszerzających dla komponentu TreeView
    /// </summary>
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Metoda zwraca listę gałęzi obiektu TreeView
        /// </summary>
        /// <param name="_self">Obiekt TreeView</param>
        /// <returns>Lista gałęzi</returns>
        public static List<TreeNode> GetAllNodes(this TreeNodeCollection _self)
        {
            List<TreeNode> result = new List<TreeNode>();
            foreach (TreeNode child in _self)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }

        /// <summary>
        /// Metoda zwraca listę gałęzi obiektu gałęzi
        /// </summary>
        /// <param name="_self">Gałąź</param>
        /// <returns>Lista gałęzi</returns>
        public static List<TreeNode> GetAllNodes(this TreeNode _self)
        {
            List<TreeNode> result = new List<TreeNode>();
            result.Add(_self);
            foreach (TreeNode child in _self.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }
    }

    public static class EnumExtensions
    {
        /// <summary>
        /// Pobiera nazwę z atrybutu Description dla Enum'a
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {

            var enumFields =
                value.GetType().GetFields(
                    BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);

            var currentField = enumFields.First(x => x.Name.Equals(value.ToString()));

            var currentFieldsDescriptionAttributes = currentField.GetCustomAttributes(typeof(DescriptionAttribute), true);
            List<Attribute> fieldsDescriptionAttributes = null;
            if (currentFieldsDescriptionAttributes!=null)
                fieldsDescriptionAttributes = (currentFieldsDescriptionAttributes as IList<Attribute>).ToList();
            //var fieldsDescriptionAttributes = (currentFieldsDescriptionAttributes as IList<Attribute>) ?? currentFieldsDescriptionAttributes.ToList();
            if (fieldsDescriptionAttributes!=null)
                return fieldsDescriptionAttributes.Count() != 0 ? ((DescriptionAttribute)fieldsDescriptionAttributes.First()).Description : value.ToString();
            return String.Empty;
        }

        public static int GetEnumValue(this Enum value)
        {

            var enumFields =
                value.GetType().GetFields(
                    BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);

            var currentField = enumFields.First(x => x.Name.Equals(value.ToString()));

            var currentFieldValue = currentField.GetValue(value);
            return (int)currentFieldValue;
        }
              
    }

    public static class ListExtensions
    {
        public static string ToDelimitedString<T>(this IEnumerable<T> source)
        {
            return source.ToDelimitedString(x => x.ToString(), CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> source, Func<T, string> converter)
        {
            return source.ToDelimitedString(converter, CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> source, string separator)
        {
            return source.ToDelimitedString(x => x.ToString(), separator);
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> source, Func<T, string> converter, string separator)
        {
            return string.Join(separator, source.Select(converter).ToArray());
        }
    }
}
