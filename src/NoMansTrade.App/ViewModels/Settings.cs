using NoMansTrade.App.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoMansTrade.App.ViewModels
{
    internal class Settings
    {
        public ObservableProperty<string> ApiKey { get; } = new ObservableProperty<string>("");

        public ObservableProperty<string> ApiEndPoint { get; } = new ObservableProperty<string>("");
    }
}
