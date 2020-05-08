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
        private const string ITEMS_EXAMPLE_1 = "De-Scented Pheromone Bottle\nPrice: -13.3%\nBuy for # 910\nProduced Locally\n1 / 24\nNeutron Microscope\nPrice: -9.9%\nBuy for @ 5,676\nProduced Locally\n1 / 29\nMicroprocessor\nDemand: +0.0%\nBuy for @ 19,000\n1 / 39\nWiring Loom\nDemand: +3.9%\nBuy for # 57,121\n1 / 11\nOxygen Capsule\nDemand: +4.3%\nBuy for # 438\n1 / 35\nUnstable Plasma\nDemand: +0.3%\nBuy for\n# 6,920\n1 / 10";

        [TestMethod]
        public void Basic_1()
        {
            var (isBuying, items) = AzureOcr.ParseItems(ITEMS_EXAMPLE_1);

            Assert.AreEqual(true, isBuying);
            Assert.AreEqual(6, items.Length);
        }
    }
}
