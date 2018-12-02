using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MojeFunkcjeUniwersalneNameSpace.Logger
{
    /// <summary>
    /// Delegat metody zdarzenia wywoływanego podczas dodawania loga
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void LogAddedEventHandler(object source, LogAddedEventArgs e);

    /// <summary>
    /// Atrybuty zdarzenia dodania logu
    /// </summary>
    public class LogAddedEventArgs : EventArgs
    {
        private string EventInfo;
        public LogAddedEventArgs(string Text)
        {
            EventInfo = Text;
        }
        public string GetInfo()
        {
            return EventInfo;
        }
    }
}
