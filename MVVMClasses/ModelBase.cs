using System;
using System.ComponentModel;
using System.Reflection;

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

        public static explicit operator ModelBase(PropertyInfo v)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
