using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoMansTrade.Core;
using NoMansTrade.Core.Model;
using NoMansTrade.Core.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NoMansTrade.Tests
{
    [TestClass]
    public class OcrTextParsingTests
    {
        private const string ITEMS_EXAMPLE_1 = "Non - Stick Piston\nSell for # 0\n27.5%\n0 /0\nSix-Pronged Mesh Decoupler\n-21.2%\nSell for # 0\n0/0\nHolographic Crankshaft\nSell for # 0\n-16.1%\n0 /0\nDirt\nSell for\n+41.8%\n0/0\nUnrefined Pyrite Grease\nSell for\n+38.5%\n0 /0\nBromide Salt\nSell for # 0\n+29.9%\n0/0";
        private const string ITEMS_EXAMPLE_2 = "Polychromatic Zirconium\nSell for @ 0\n+21.6%\n0/0\nRe-latticed Arc Crystal\nSell for\n+16.0%\n0/0\nOxygen Capsule\nSell for\n-6.7%\n0/0\nRusted Metal\nSell for # 0\n-11.5%\n0/0\nResidual Goop\nSell for\n-7.5%\n0/0\nUranium\nSell for\n-7.8%\n0/0";
        private const string ITEMS_EXAMPLE_3 = "Non-Stick Piston\n-20.3%\nBuy for # 5,022\n1 / 73\nEnormous Metal Cog\n-25.9%\nBuy for # 777\n1 / 56\nSix-Pronged Mesh Decoupler\n-13.3%\nBuy for @ 13,655\n1 / 72\nHolographic Crankshaft\nBuy for # 29,088\n-7.7%\n1 / 43\nOxygen Capsule\n+11.9%\nBuy for # 430\n1 / 53\nUnstable Plasma\n+5.8%\nBuy for # 6,694\n1 / 71";

        [TestMethod]
        public void Basic_1()
        {
            var result = AzureOcr.ParseItems(ITEMS_EXAMPLE_1);

            Assert.AreEqual(false, result.isBuying);
            Assert.AreEqual(6, result.items.Length);
        }

        [TestMethod]
        public void Basic_2()
        {
            var result = AzureOcr.ParseItems(ITEMS_EXAMPLE_2);

            Assert.AreEqual(false, result.isBuying);
            Assert.AreEqual(6, result.items.Length);
        }

        [TestMethod]
        public void Basic_3()
        {
            var result = AzureOcr.ParseItems(ITEMS_EXAMPLE_3);

            Assert.AreEqual(true, result.isBuying);
            Assert.AreEqual(6, result.items.Length);
        }
    }
}
