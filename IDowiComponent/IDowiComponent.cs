using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDowiComponentNamespace
{
    /// <summary>
    /// Interfejst customowego komponentu
    /// </summary>
    public interface IDowiComponent
    {
        /// <summary>
        /// Właściwość pozwalająca określić czy komponent jest wypełniony
        /// </summary>
        bool HasValue { get; set; }
    }
}
