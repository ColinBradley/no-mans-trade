using System;

namespace NoMansTrade.Core.Model
{
    public class Item
    {
        public string Name { get; set; } = "";

        public int Price { get; set; }

        public int Quantity { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
