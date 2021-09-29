using DowiExtensionsNameSpace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MojeFunkcjeRozszerzajace.Tests
{
    [TestClass()]
    public class LongExtensionsTests
    {
        [TestMethod()]
        public void ToFileSizeStringTest()
        {
            long fileSize_363kb = 363687;
            Assert.AreEqual(fileSize_363kb.ToFileSizeString(), "355 kB");

            long fileSize_21MB = 21163416;
            Assert.AreEqual(fileSize_21MB.ToFileSizeString(), "20,18 MB");

            long fileSize_1GB = 1000967564;
            Assert.AreEqual(fileSize_1GB.ToFileSizeString(), "954,60 MB");

            long fileSize_10GB = 10009675640;
            Assert.AreEqual(fileSize_10GB.ToFileSizeString(), "9,32 GB");

            long fileSize_100GB = 100096756400;
            Assert.AreEqual(fileSize_100GB.ToFileSizeString(), "93,22 GB");

            long fileSize_1TB = 1000967564000;
            Assert.AreEqual(fileSize_1TB.ToFileSizeString(), "932,22 GB");

            long fileSize_10TB = 10009675640000;
            Assert.AreEqual(fileSize_10TB.ToFileSizeString(), "9,10 TB");
            
        }
    }
}