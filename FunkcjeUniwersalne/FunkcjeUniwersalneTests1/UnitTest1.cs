using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FunkcjeUniwersalneTests1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int firstValue = 2;
            int secondValue = 3;
            Assert.IsTrue(firstValue.Equals());
        }
    }
}
