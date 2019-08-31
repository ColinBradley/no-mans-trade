using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using NoMansTrade.App.Commands;
using NoMansTrade.Core.Model;

namespace NoMansTrade.App.ViewModels
{
    internal class Recommendations : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Recommendations(Locations locations)
        {
            this.BuildCommand = new BuildRecommmendationsCommand(locations, this);
        }

        public ICommand BuildCommand { get; }

        public Recommendation[] Items { get; private set; } = Array.Empty<Recommendation>();

        internal void SetRecommendations(Recommendation[] recomendations)
        {
            this.Items = recomendations;

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Items)));
        }
    }
}
