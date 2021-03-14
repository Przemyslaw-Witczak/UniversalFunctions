using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FunkcjeUniwersalne.Wpf
{
    /// <summary>
    /// Metody rozszerzające okna WPF
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Zwróć listę kontrolek z okna WPF
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static List<Control> GetAllControlsRecursive(this DependencyObject depObj)
        {
            var outputList = new List<Control>();
            outputList.AddRange(GetAllControls(depObj));
            return outputList;
        }

        /// <summary>
        /// Zwraca rekurencyjnie listę kontrolek okna, lub rodzica WPF
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        private static List<Control> GetAllControls(DependencyObject depObj)
        {
            var outputList = new List<Control>();
            if (depObj != null)
            {

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if ((child is TextBox)
                        || (child is DatePicker)
                        || (child is ComboBox)
                        || (child is CheckBox)
                        || (child is ListBox)
                        || (child is RadioButton))
                    {
                        outputList.Add(child as Control);
                    }
                    else
                    {
                        outputList.AddRange(GetAllControls(child));
                    }
                }
            }
            return outputList;
        }

        
    }
}
