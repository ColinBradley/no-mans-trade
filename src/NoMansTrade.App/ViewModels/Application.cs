using Newtonsoft.Json;
using NoMansTrade.App.Storage;
using NoMansTrade.Core.Model;
using NoMansTrade.Core.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NoMansTrade.App.ViewModels
{
    internal class Application
    {
        static Application()
        {
            JsonSerialization.Settings.Converters.Add(new SKRectIJsonConverter());
        }

        public Application()
        {
            this.Images = new DirectoryImages(this.Locations, this.Settings);
            this.Recommendations = new Recommendations(this.Locations);
        }

        public DirectoryImages Images { get; }

        public LocationCollection Locations { get; } = new LocationCollection();

        public Settings Settings { get; } = new Settings();

        public Recommendations Recommendations { get; }

        public async Task Load()
        {
            if (!File.Exists("./config.json"))
            {
                return;
            }

            try
            {
                var config = JsonConvert.DeserializeObject<ApplicationConfiguration>(await File.ReadAllTextAsync("./config.json").ConfigureAwait(true), JsonSerialization.Settings);

                this.Settings.ApiKey.Value = config.ApiKey;
                this.Settings.ApiEndPoint.Value = config.ApiEndPoint;
                this.Settings.AutoScanNewImages.Value = config.AutoScanNewImages;
                this.Settings.ScanDirectory.Value = config.ScanDirectory;

                foreach (var location in config.Locations)
                {
                    this.Locations.AddLocation(location);
                }

                this.Images.Initialize();
                this.Images.SetAnalyzedImages(config.AnalyzedNames);
                this.Images.ItemsRectangle.Value = config.ItemsRectangle;
                this.Images.LocationRectangle.Value = config.LocationRectangle;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error loading config");
            }
        }

        public async Task Save()
        {
            var config = new ApplicationConfiguration
            {
                ApiKey = this.Settings.ApiKey.Value,
                ApiEndPoint = this.Settings.ApiEndPoint.Value,
                AutoScanNewImages = this.Settings.AutoScanNewImages.Value,
                ScanDirectory = this.Settings.ScanDirectory.Value,
                ItemsRectangle = this.Images.ItemsRectangle.Value,
                LocationRectangle = this.Images.LocationRectangle.Value,
                Locations = this.Locations,
                AnalyzedNames = this.Images.Images.Where(i => i.IsAnalyzed.Value).Select(i => Path.GetFileName(i.FilePath)).ToArray()
            };

            try
            {
                await File.WriteAllTextAsync("./config.json", JsonConvert.SerializeObject(config, JsonSerialization.Settings)).ConfigureAwait(false);
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error saving config");
            }
        }
    }
}
