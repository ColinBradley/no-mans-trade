using System;
using System.Collections.ObjectModel;

namespace NoMansTrade.Core.Model
{
    public class Location
    {
        public string Name { get; set; } = "";

        public ObservableCollection<Item> Buying { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> Selling { get; } = new ObservableCollection<Item>();

        public DateTime LastUpdate { get; set; }
    }
}
