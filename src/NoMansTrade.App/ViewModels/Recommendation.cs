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

            this.ProfitAmount = (this.SellItem.Price * this.BuyItem.Quantity) - (this.BuyItem.Price * this.BuyItem.Quantity);
            this.Profit = $"{this.ProfitAmount.ToString("N", System.Globalization.CultureInfo.CurrentCulture)} Units";
        }

        public Item BuyItem { get; }

        public Location BuyLocation { get; }

        public Item SellItem { get; }

        public Location SellLocation { get; }

        public int ProfitAmount { get; }

        public string Profit { get; }
    }
}
