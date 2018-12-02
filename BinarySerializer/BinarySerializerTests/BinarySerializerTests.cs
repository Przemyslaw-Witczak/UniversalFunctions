using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinarySerializerTests;
using System.IO;

namespace BinarySerializerNamespace.Tests
{
    [TestClass()]
    public class BinarySerializerTests
    {
        [TestMethod()]
        public void SerializeTest()
        {
            TestClass serializedClass = new TestClass()
            {
                StringProperty = "Wartość zmiennej typu string",
                IntProperty = 123,
                DecProperty2 = 321.01M,
                BoolProperty = true
            };
            MemoryStream stream = new MemoryStream();
            

            BinarySerializer serialization = new BinarySerializer();
           
            serialization.Serialize(serializedClass, stream);

            stream.Position = 0;

            BinarySerializer deserialization = new BinarySerializer();
            TestClass deserializedClass = new TestClass();
            deserialization.Deserialize(deserializedClass, stream);

            Assert.AreEqual(serializedClass, deserializedClass);
        }
    }
}