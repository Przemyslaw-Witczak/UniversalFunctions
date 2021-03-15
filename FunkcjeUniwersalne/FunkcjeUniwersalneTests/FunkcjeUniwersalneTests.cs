using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MojeFunkcjeUniwersalneNameSpace.Tests
{
    [TestClass()]
    public class FunkcjeUniwersalneTests
    {
        [TestMethod()]
        public void FormatujDecimalSeparatorTest()
        {
            string finalDecimalValue = $"1{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}23";
            Assert.AreEqual(finalDecimalValue, FunkcjeUniwersalne.Instance.FormatujDecimalSeparator("1.23"));

            Assert.AreEqual(finalDecimalValue, FunkcjeUniwersalne.Instance.FormatujDecimalSeparator("1,23"));
        }
        
        [TestMethod()]
        public void FormatujStringNaDecimalTest()
        {
            decimal kwotaOczekiwana = 1.23M;
            
            decimal kwota = FunkcjeUniwersalne.Instance.FormatujStringNaDecimal(Convert.ToString(kwotaOczekiwana));
            Assert.AreEqual(kwotaOczekiwana, kwota);
            
        }
    }
}
