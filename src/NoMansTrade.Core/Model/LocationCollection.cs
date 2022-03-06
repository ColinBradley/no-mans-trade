using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NoMansTrade.Core.Model
{
    public class LocationCollection : ObservableCollection<Location>
    {
        private readonly Dictionary<string, Location> mLocationsByName = new(StringComparer.OrdinalIgnoreCase);

        public void AddLocation(string name, Item[] items, bool isBuying)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

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

        public void AddLocation(Location location)
        {
            if (!mLocationsByName.TryGetValue(location.Name, out var instance))
            {
                mLocationsByName.Add(location.Name, location);

                this.Add(location);

                return;
            }

            MergeItems(instance.Buying, location.Buying.ToArray());
            MergeItems(instance.Selling, location.Selling.ToArray());
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
                matching.Quantity = item.Quantity;
            }
        }
    }
}
