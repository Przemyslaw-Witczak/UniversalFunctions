using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MVVMClasses.Models;

namespace MVVMClasses
{
    /// <summary>
    /// Interaction logic for MvvmMessageBox.xaml
    /// </summary>
    public partial class MvvmMessageBox : Window
    {
        public MvvmMessageBox()
        {
            InitializeComponent();            
        }

        public MvvmMessageBoxModel ViewModel
        {
            get { return (MvvmMessageBoxModel) this.DataContext; }
            set
            {
                DataContext = value;
                ViewModel.CloseViewWithOKResult += (() => this.DialogResult = true);
                ViewModel.CloseViewWithFalseResult += (() => this.DialogResult = false);
            }
        }
    }
}
