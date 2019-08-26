using NoMansTrade.App.Support;
using System;
using System.IO;

namespace NoMansTrade.App.ViewModels
{
    internal class Settings
    {
        public ObservableProperty<string> ScanDirectory { get; } = 
            new ObservableProperty<string>(
                Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    @"Pictures\Screenshots"));

        public ObservableProperty<string> ApiKey { get; } = new ObservableProperty<string>("");

        public ObservableProperty<string> ApiEndPoint { get; } = new ObservableProperty<string>("");

        public ObservableProperty<bool> AutoScanNewImages { get; } = new ObservableProperty<bool>(false);
    }
}
