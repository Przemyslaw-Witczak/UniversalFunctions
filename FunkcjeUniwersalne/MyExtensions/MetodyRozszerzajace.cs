using MojeFunkcjeUniwersalneNameSpace.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DowiExtensionsNameSpace
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

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (length < 0)
            {
                return string.Empty;
            }

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
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
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
            strValue = string.Format("{0:#" + decimalSeparator + "0.000}", value);
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
            {
                formatedValue = $"{value} B";
            }
            else if (value / 1024 < 1024)
            {
                formatedValue = $"{value / 1024} kB";
            }
            else if (value / 1024 / 1024 < 1024)
            {
                formatedValue = $"{(value / 1024f / 1024f).ToString("F")} MB";
            }
            else if (value / 1024 / 1024 / 1024 < 1024)
            {
                formatedValue = $"{(value / 1024f / 1024f / 1024f).ToString("F")} GB";
            }
            else
            {
                formatedValue = $"{(value / 1024f / 1024f / 1024f / 1024f).ToString("F")} TB";
            }

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
            {
                return "TAK";
            }
            else
            {
                return "NIE";
            }
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
            {
                forma.MdiParent = parentForm;
            }
            else
            {
                forma.MdiParent = parentForm.MdiParent;
            }

            forma.Show();
            forma.WindowState = FormWindowState.Maximized;
        }

        public static List<Control> GetAllControlsRecursive(this Form form)
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
                {
                    outputList.Add(kontrolka);
                }
                else if (kontrolka is TabPage)
                {
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                }
                else if (kontrolka is TabControl)
                {
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                }
                else if (kontrolka is Panel || kontrolka is SplitterPanel)
                {
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                }
                else if (kontrolka is GroupBox)
                {
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                }
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
                {
                    outputList.AddRange(GetAllControls(kontrolka.Controls));
                }
            }
            return outputList;
        }

        public static void ShowFormOnPanel(this Form forma, Panel destination)
        {
            forma.FormBorderStyle = FormBorderStyle.None;
            var formHandle = forma.Handle;
            SafeNativeMethods.SetParent(formHandle, destination.Handle);
            RefreshSizeCppForm(formHandle, destination);
            SetWindowParameters(formHandle, destination);
            if (forma is MdiChildFormBase)
                (forma as MdiChildFormBase).MenuStripPointer.Visible = false;
            forma.Show();
            RefreshSizeCppForm(forma.Handle, destination);
            destination.Focus();
        }

        /// <summary>
        /// Dostosowuje rozmiar okna modułu C++ do kontrolki, w której jest wyświetlane.
        /// </summary>
        /// <param name="cppFormHWND">Uchyt do okna modułu C++.</param>
        /// <param name="parentControl">Konrolka typu panel, w której wyświetlane jest okno C++.</param>
        /// <remarks>W przypadku osadzenia okna borland C++ oraz zmiany wielkości okna Shell aplikacji
        /// metoda powoduje dostosowanie wielkości okna borlandowego do bierzącego rozmiaru okna Shell.</remarks>
        public static void RefreshSizeCppForm(IntPtr cppFormHWND, object parentControl)
        {
            Panel panel = (Panel)parentControl;
            SafeNativeMethods.SetWindowPos(cppFormHWND, new IntPtr(0), 0, 0, panel.Width, panel.Height, 0);
        }

        /// <summary>
        /// Metoda ustawia parametry okna w celu poprawy odblokowywania głownego okna i przechwytywania zdarzeń
        /// </summary>
        /// <param name="cppFormHandle">Uchwyt do okna modułu C++</param>
        /// <param name="panel">Kontrolka typu panel w której wyświetlane jest okno C++</param>
        private static unsafe void SetWindowParameters(IntPtr cppFormHandle, Panel panel)
        {
            const int GWL_EXSTYLE = 0x14;
            const int WS_EX_CONTROLPARENT = 0x00010000;

            const int GWL_STYLE = -16;
            const uint WS_POPUP = 0x80000000;
            const uint WS_CHILD = 0x40000000;
            const uint WS_TABSTOP = 0x00010000;

            uint style = (uint)SafeNativeMethods.GetWindowLong(cppFormHandle, GWL_STYLE);
            if (style == 0)
            {
                throw new Exception("Error in SetWindowParameters");
            }

            style = style & ~WS_POPUP | WS_CHILD | WS_TABSTOP;
            SafeNativeMethods.SetWindowLong(cppFormHandle, GWL_STYLE, (int)style);

            //Nie dokońca wiadomo po co to
            uint style2 = (uint)SafeNativeMethods.GetWindowLong(panel.Handle, GWL_EXSTYLE);
            style2 = style2 | WS_EX_CONTROLPARENT;
            SafeNativeMethods.SetWindowLong(panel.Handle, GWL_EXSTYLE, (int)style2);
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
            int halfWay = dataGridView.DisplayedRowCount(false) / 2;
            if (dataGridView.SelectedRows.Count > 0 &&
                (dataGridView.FirstDisplayedScrollingRowIndex + halfWay > dataGridView.SelectedRows[0].Index ||
                dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.DisplayedRowCount(false) - halfWay <= dataGridView.SelectedRows[0].Index))
            {
                int targetRow = dataGridView.SelectedRows[0].Index;

                targetRow = Math.Max(targetRow - halfWay, 0);
                dataGridView.FirstDisplayedScrollingRowIndex = targetRow;

            }
        }


        public static DataGridViewRow AddRow(this DataGridViewRowCollection dataGridViewRowCollection, params string[] columnsValues)
        {
            int rowIndex = -1;
            rowIndex = dataGridViewRowCollection.Add(columnsValues);
            if (rowIndex > -1)
            {
                return dataGridViewRowCollection[rowIndex];
            }
            else
            {
                return null;
            }
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



        public static IEnumerable<TreeNode> GetAllChildren(TreeNode Parent)
        {
            return Parent.Nodes.Cast<TreeNode>().Concat(
                   Parent.Nodes.Cast<TreeNode>().SelectMany(GetAllChildren));
        }


    }

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
