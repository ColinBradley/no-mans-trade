using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NoMansTrade.Core.Model
{
    public class Locations : ObservableCollection<Location>
    {
        private readonly Dictionary<string, Location> mLocationsByName = new Dictionary<string, Location>(StringComparer.OrdinalIgnoreCase);

        public void AddLocation(string name, Item[] items, bool isBuying)
        {
            if (!mLocationsByName.TryGetValue(name, out var instance))
            {
                instance = new Location()
                {
                    Name = name,
                    LastUpdate = DateTime.UtcNow,
                };

                if (isBuying)
                {
                    foreach (var item in items)
                    {
                        instance.Buying.Add(item);
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        instance.Selling.Add(item);
                    }
                }

                mLocationsByName.Add(name, instance);

                this.Add(instance);

                return;
            }

            instance.LastUpdate = DateTime.UtcNow;

            if (isBuying)
            {
                MergeItems(instance.Buying, items);
            }
            else
            {
                MergeItems(instance.Selling, items);
            }
        }

        private static void MergeItems(IList<Item> existingItems, Item[] newItems)
        {
            foreach (var item in newItems)
            {
                var matching = existingItems.FirstOrDefault(i => string.Equals(i.Name, item.Name, StringComparison.OrdinalIgnoreCase));
                if (matching == null)
                {
                    existingItems.Add(item);
                    continue;
                }

                matching.LastUpdate = item.LastUpdate;
                matching.Price = item.Price;
                matching.PriceDifferencePercentage = item.PriceDifferencePercentage;
                matching.Quantity = item.Quantity;
            }
        }
    }
}
