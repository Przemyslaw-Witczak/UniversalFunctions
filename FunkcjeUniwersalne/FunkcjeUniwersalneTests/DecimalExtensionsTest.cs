using DowiExtensionsNameSpace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MojeFunkcjeUniwersalneNameSpace;
using System;

namespace FunkcjeUniwersalneTests
{
    [TestClass]
    public class DecimalExtensionsTest
    {
        [TestMethod]
        public void TestDecimalConversionAndConvertBack()
        {
            Decimal value0dot003 = 0.123M;
            var stringValue0dot003 = value0dot003.ToString3Places();            
            var convertedBack = FunkcjeUniwersalne.Instance.FormatujStringNaDecimal(stringValue0dot003);

            Decimal value0dot002 = 0.12M;
            var stringValueToDecimal2 = value0dot002.ToString2Places();
            var convertedBack2 = FunkcjeUniwersalne.Instance.FormatujStringNaDecimal(stringValueToDecimal2);

            Assert.AreEqual(value0dot003, convertedBack);
            Assert.AreEqual(value0dot002, convertedBack2);

            // Additional checks for formatting of currency values
            Decimal currencyValue = 1234.5M;
            //format to currency using current culture
            var currencyString = currencyValue.ToString("C");
            var convertedBackCurrency = FunkcjeUniwersalne.Instance.FormatujStringNaDecimal(currencyString);
            Assert.AreEqual(currencyValue, convertedBackCurrency);

            //Check if value with dot as decimal separator is handled correctly
            var dotDecimalString = "1234.56";
            var convertedBackDot = FunkcjeUniwersalne.Instance.FormatujStringNaDecimal(dotDecimalString);
            Assert.AreEqual(1234.56M, convertedBackDot);
        }
    }
}
