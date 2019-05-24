using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MojeFunkcjeRozszerzajace;

namespace MojeFunkcjeRozszerzajace.Tests
{
    [TestClass()]
    public class FileVersionInfoExtensionsTests
    {
        
    }
}

namespace FunkcjeUniwersalneTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int lowerValue = 2;
            int greaterValue = 3;
            Assert.IsTrue(lowerValue.EqualsReturnEnum(greaterValue) ==FileVersionInfoExtensions.EqualityEnum.Lower);
            Assert.IsTrue(greaterValue.EqualsReturnEnum(lowerValue) == FileVersionInfoExtensions.EqualityEnum.Greater);
            Assert.IsTrue(greaterValue.EqualsReturnEnum(greaterValue) == FileVersionInfoExtensions.EqualityEnum.Equal);
        }
    }
}
