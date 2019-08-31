using NoMansTrade.App.ViewModels;
using NoMansTrade.Core;
using NoMansTrade.Core.Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoMansTrade.App.Commands
{
    internal class BuildRecommmendationsCommand : ICommand
    {
        private readonly Locations mLocations;
        private readonly Recommendations mRecommendationsViewModel;

        public event EventHandler? CanExecuteChanged;

        public BuildRecommmendationsCommand(Locations locations, Recommendations recommendations)
        {
            mLocations = locations;
            mRecommendationsViewModel = recommendations;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var buying = new Dictionary<string, List<(Item item, Location location)>>(StringComparer.OrdinalIgnoreCase);
            var selling = new Dictionary<string, List<(Item item, Location location)>>(StringComparer.OrdinalIgnoreCase);

            foreach (var location in mLocations)
            {
                foreach (var buyable in location.Buying)
                {
                    if (buyable.PriceDifferencePercentage > 0)
                    {
                        continue;
                    }

                    if (!buying.TryGetValue(buyable.Name, out var items))
                    {
                        items = new List<(Item item, Location location)>();
                        buying.Add(buyable.Name, items);
                    }

                    items.Add((buyable, location));
                }

                foreach (var sellable in location.Selling)
                {
                    if (sellable.PriceDifferencePercentage < 0)
                    {
                        continue;
                    }

                    if (!selling.TryGetValue(sellable.Name, out var items))
                    {
                        items = new List<(Item item, Location location)>();
                        selling.Add(sellable.Name, items);
                    }

                    items.Add((sellable, location));
                }
            }

            var recommendations = new List<Recommendation>();
            foreach (var buyable in buying.Values)
            {
                var bestBuyable = buyable.OrderBy(b => b.item.PriceDifferencePercentage).First();
                if (!selling.TryGetValue(bestBuyable.item.Name, out var sellables))
                {
                    continue;
                }

                var bestSellable = sellables.OrderBy(s => s.item.PriceDifferencePercentage).Last();

                recommendations.Add(new Recommendation(bestBuyable, bestSellable));
            }

            mRecommendationsViewModel.SetRecommendations(recommendations.ToArray());
        }
    }
}
