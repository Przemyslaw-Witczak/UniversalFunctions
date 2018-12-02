using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializerNamespace
{

    /// <summary>
    /// Enum określający typy wartości przyjmowanych przez parametry
    /// </summary>
    public enum ParameterValueType
    {
        /// <summary>
        /// null
        /// </summary>
        UnSetValue = 0,
        StringValue = 1,
        IntegerValue = 2,
        BooleanValue = 3,
        DateValue = 4,
        TimeStampValue = 5,
        TimeValue = 6,
        DecimalValue = 7,
        DoubleValue = 8,
        ByteValue = 9,
        CharValue = 10,
        ObjectValue = 11
    }
}
