using System;
using System.Collections.ObjectModel;

namespace NoMansTrade.Core.Model
{
    public class Location
    {
        public string Name { get; set; } = "";

        public ObservableCollection<Item> Buying { get; set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> Selling { get; set; } = new ObservableCollection<Item>();

        public DateTime LastUpdate { get; set; }
    }
}
