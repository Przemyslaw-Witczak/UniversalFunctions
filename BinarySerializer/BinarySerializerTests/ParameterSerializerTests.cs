using BinarySerializerNamespace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BinarySerializer.Tests
{
    [TestClass()]
    public class ParameterSerializerTests
    {
        [TestMethod()]
        public void SerializeTest()
        {
            Parameter paramWe;
            Parameter paramWy;

            #region Serializacja parametru String
            paramWe = new Parameter("parametr1 - String");
            paramWe.AsString = "Wartość parametru string";

            MemoryStream stream = new MemoryStream();
            ParameterSerializer serializer = new ParameterSerializer();
            serializer.Serialize(paramWe, stream);

            stream.Position = 0;

            paramWy = serializer.Deserialize(stream);

            Assert.AreEqual(paramWe.Name, paramWy.Name);
            Assert.AreEqual(paramWe.ParameterTypeValue, paramWy.ParameterTypeValue);
            Assert.AreEqual(paramWe.AsValue, paramWy.AsValue);
            #endregion


            #region Serializacja parametru DateTime
            paramWe = new Parameter("parametr2 - DateTime");
            paramWe.AsTimeStamp = DateTime.Now;

            stream = new MemoryStream();
            serializer = new ParameterSerializer();
            serializer.Serialize(paramWe, stream);

            stream.Position = 0;

            paramWy = serializer.Deserialize(stream);

            Assert.AreEqual(paramWe.Name, paramWy.Name);
            Assert.AreEqual(paramWe.ParameterTypeValue, paramWy.ParameterTypeValue);
            Assert.AreEqual(paramWe.AsTimeStamp, paramWy.AsTimeStamp);
            #endregion

            #region Serializacja parametruTime
            paramWe = new Parameter("parametr3-Time");
            paramWe.AsTime = DateTime.Now;

            stream = new MemoryStream();
            serializer = new ParameterSerializer();
            serializer.Serialize(paramWe, stream);

            stream.Position = 0;

            paramWy = serializer.Deserialize(stream);

            Assert.AreEqual(paramWe.Name, paramWy.Name);
            Assert.AreEqual(paramWe.ParameterTypeValue, paramWy.ParameterTypeValue);
            Assert.AreEqual(paramWe.AsTime, paramWy.AsTime);
            #endregion
        }

       
    }
}