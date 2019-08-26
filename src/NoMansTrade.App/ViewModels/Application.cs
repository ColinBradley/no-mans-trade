using NoMansTrade.App.Support;
using NoMansTrade.Core.Model;
using System;
using System.IO;

namespace NoMansTrade.App.ViewModels
{
    internal class Application
    {
        public Application()
        {
            this.Images = new DirectoryImages(this.Locations, this.Settings);
        }

        public DirectoryImages Images { get; }

        public Locations Locations { get; } = new Locations();

        public Settings Settings { get; } = new Settings();

        public void Initialize()
        {
            this.Images.Initialize();
        }
    }
}
