﻿using NoMansTrade.App.ViewModels;
using NoMansTrade.Core;
using NoMansTrade.Core.Model;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoMansTrade.App.Commands
{
    internal class AnalyzeImageCommand : ICommand, IDisposable
    {
        private readonly DirectoryImages mImages;
        private readonly Locations mLocations;
        private readonly Settings mSettings;

        public event EventHandler? CanExecuteChanged;

        public AnalyzeImageCommand(DirectoryImages images, Locations locations, Settings settings)
        {
            mImages = images;
            mLocations = locations;
            mSettings = settings;

            images.Current.PropertyChanged += this.ImagesCurrent_PropertyChanged;
        }

        public bool CanExecute(object parameter)
        {
            return mImages.Current.Value != null;
        }

        public void Execute(object parameter)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            Task.Run(async () =>
            {
                var locations = ((SKRectI items, SKRectI location))parameter;

                var fullImage = SKImage.FromEncodedData(mImages.Current.Value!.FilePath);

                using var itemsImageStream = new MemoryStream();

                fullImage.Subset(locations.items)
                    .Encode(SKEncodedImageFormat.Png, 100)
                    .SaveTo(itemsImageStream);

                itemsImageStream.Position = 0;

                using var ocr = new AzureOcr(mSettings.ApiKey.Value, mSettings.ApiEndPoint.Value);
                var itemsText = ocr.ExtractTextAsync(itemsImageStream);

                using var locationImageStream = new MemoryStream();

                fullImage.Subset(locations.location)
                    .Encode(SKEncodedImageFormat.Png, 100)
                    .SaveTo(locationImageStream);

                locationImageStream.Position = 0;

                var locationText = await ocr.ExtractTextAsync(locationImageStream);

                var itemsResult = AzureOcr.ParseItems(await itemsText);

                await dispatcher.BeginInvoke(() =>
                {
                    mLocations.AddLocation(locationText, itemsResult.items, itemsResult.isBuying);
                });
            });
        }

        private void ImagesCurrent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            mImages.Current.PropertyChanged -= this.ImagesCurrent_PropertyChanged;
        }
    }
}