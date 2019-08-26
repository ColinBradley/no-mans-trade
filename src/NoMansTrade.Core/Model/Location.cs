using NoMansTrade.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;

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
