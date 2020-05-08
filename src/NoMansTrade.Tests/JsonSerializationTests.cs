using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoMansTrade.Core.Model;
using NoMansTrade.Core.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace NoMansTrade.Tests
{
    [TestClass]
    public class JsonSerializationTests
    {
        [TestMethod]
        public async Task Basic()
        {
            var updateTime = System.DateTime.UtcNow;
            var orignal = new[] {
                new Location() {
                    Name = "Dave",
                    LastUpdate = updateTime,
                },
            };

            orignal[0].Buying.Add(new Item()
            {
                Name = "Item 1",
                Price = 100,
                Quantity = 42,
                LastUpdate = updateTime
            });

            using var stream = new MemoryStream();

            JsonSerialization.Store(orignal, stream);

            stream.Position = 0;

            var parsed = await JsonSerialization.Parse(stream);

            Assert.AreEqual(orignal.Length, parsed.Length);
            Assert.AreEqual(orignal[0].LastUpdate, parsed[0].LastUpdate);
            Assert.AreEqual(orignal[0].Buying.Count, parsed[0].Buying.Count);
        }
    }
}
