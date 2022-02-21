using System.Windows;
using System.Windows.Controls;

namespace MVVMClasses.Views
{
    /// <summary>
    /// Interaction logic for CommitCancelView.xaml
    /// </summary>
    public partial class CommitCancelView : Window
    {
        public CommitCancelView(UserControl viewFiltry)
        {
            InitializeComponent();
            PanelFiltrow = viewFiltry;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.dialogresult = true;
        }
    }
}
