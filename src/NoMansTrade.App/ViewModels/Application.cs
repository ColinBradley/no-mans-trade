using NoMansTrade.App.Support;
using NoMansTrade.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoMansTrade.App.ViewModels
{
    internal class Application
    {
        public Application()
        {
            this.Images = new DirectoryImages(this.Directory.Value, this.Locations, this.Settings);
        }

        public ObservableProperty<string> Directory { get; } 
            = new ObservableProperty<string>(
                Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    @"Pictures/Screenshots"));

        public DirectoryImages Images { get; }

        public Locations Locations { get; } = new Locations();


        public Settings Settings { get; } = new Settings();

        public void Initialize()
        {
            this.Images.Initialize();
        }
    }
}
