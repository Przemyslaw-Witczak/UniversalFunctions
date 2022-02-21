using BinarySerializer;
using BinarySerializerNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializerTests
{
    public class TestClass
    {
        [BinarySerializer]
        public int IntProperty { get; set; }

        [BinarySerializer]
        public string StringProperty { get; set;}

        [BinarySerializerAttribute]
        public bool BoolProperty { get; set; }

        [BinarySerializerAttribute]
        public Decimal DecProperty2 { get; set; }

        public TestClass()
        {
            
        }
    }
}
