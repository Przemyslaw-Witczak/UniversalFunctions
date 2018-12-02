using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MojeFunkcjeUniwersalneNameSpace.Logger
{
    /// <summary>
    /// Zawartość kolejki logów do zapisu.
    /// </summary>
    public class LogPositions
    {
        public bool IsWritten;
        public string LogValue;
        public DateTime LogTimeStamp;
        public LogPositions(string komunikat = "")
        {
            IsWritten = false;
            LogValue = komunikat;
            LogTimeStamp = DateTime.Now;
        }

        public string GetLogValue()
        {
            string czas = string.Empty;// string.Format("{0:yyyy-MM-dd hh:mm:ss.fff}", DateTime.Now);
            czas = Convert.ToString(LogTimeStamp) + "." + LogTimeStamp.Millisecond.ToString();
            czas += ";" + LogValue;
            return czas;    
        }
    }
}
