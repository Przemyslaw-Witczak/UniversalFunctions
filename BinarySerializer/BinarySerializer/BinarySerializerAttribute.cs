using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializerNamespace
{
    /// <summary>
    /// Atrybut określający czy pole, lub wartość będzie serializowane
    /// </summary>
    public class BinarySerializerAttribute : Attribute
    {
        public BinarySerializerAttribute()
        {
            
        }
    }
}
