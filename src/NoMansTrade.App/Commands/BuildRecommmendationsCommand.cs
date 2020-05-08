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
        private readonly LocationCollection mLocations;
        private readonly Recommendations mRecommendationsViewModel;

#pragma warning disable 67
        public event EventHandler? CanExecuteChanged;
#pragma warning restore 67

        public BuildRecommmendationsCommand(LocationCollection locations, Recommendations recommendations)
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
                    if (!buying.TryGetValue(buyable.Name, out var items))
                    {
                        items = new List<(Item item, Location location)>();
                        buying.Add(buyable.Name, items);
                    }

                    items.Add((buyable, location));
                }

                foreach (var sellable in location.Selling)
                {
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
                var bestBuyable = buyable.OrderBy(b => b.item.Price).First();
                if (!selling.TryGetValue(bestBuyable.item.Name, out var sellables))
                {
                    continue;
                }

                var bestSellable = sellables.OrderBy(s => s.item.Price).Last();

                var recommendation = new Recommendation(bestBuyable, bestSellable);
                if (recommendation.ProfitAmount <= 0)
                {
                    continue;
                }

                recommendations.Add(recommendation);
            }

            mRecommendationsViewModel.SetRecommendations(recommendations.ToArray());
        }
    }
}
