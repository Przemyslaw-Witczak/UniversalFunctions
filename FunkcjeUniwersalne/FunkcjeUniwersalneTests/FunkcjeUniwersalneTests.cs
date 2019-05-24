using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MojeFunkcjeUniwersalneNameSpace;

namespace FunkcjeUniwersalneTests
{
    [TestClass()]
    public class FunkcjeUniwersalneTests
    {
        [TestMethod()]
        public void FormatujDecimalSeparatorTest()
        {
            string finalDecimalValue = $"1{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}23";
            Assert.AreEqual(finalDecimalValue, MojeFunkcjeUniwersalneNameSpace.FunkcjeUniwersalne.Instance.FormatujDecimalSeparator("1.23"));

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
