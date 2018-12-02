using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
