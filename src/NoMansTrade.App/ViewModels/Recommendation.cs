using System;
using System.Collections.Generic;
using System.Text;
using NoMansTrade.Core.Model;

namespace NoMansTrade.App.ViewModels
{
    public class Recommendation
    {
        public Recommendation((Item item, Location location) bestBuyable, (Item item, Location location) bestSellable)
        {
            this.BuyItem = bestBuyable.item;
            this.BuyLocation = bestBuyable.location;
            this.SellItem = bestSellable.item;
            this.SellLocation = bestSellable.location;
        }

        public Item BuyItem { get; }

        public Location BuyLocation { get; }

        public Item SellItem { get; }

        public Location SellLocation { get; }
    }
}
