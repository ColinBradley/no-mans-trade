using System;
using System.Collections.Generic;
using System.Text;

namespace NoMansTrade.Core.Model
{
    public class Item
    {
        public string Name { get; set; } = "";

        public int Price { get; set; }

        public int Quantity { get; set; }

        public double PriceDifferencePercentage { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
