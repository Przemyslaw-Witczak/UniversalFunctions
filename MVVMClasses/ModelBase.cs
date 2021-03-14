using System.ComponentModel;

namespace MVVMClasses
{
    /// <summary>
    /// Klasa bazowa, dziedziczy i implementuje NotifyPropertyChanged
    /// </summary>
    public class ModelBase : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
