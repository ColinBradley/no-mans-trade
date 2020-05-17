using NoMansTrade.App.ViewModels;
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
        private readonly LocationCollection mLocations;
        private readonly Settings mSettings;
        private Image? mCurrentImageModel;

        public event EventHandler? CanExecuteChanged;

        public AnalyzeImageCommand(DirectoryImages images, LocationCollection locations, Settings settings)
        {
            mImages = images;
            mLocations = locations;
            mSettings = settings;

            images.Current.PropertyChanged += this.ImagesCurrent_PropertyChanged;
        }

        public bool CanExecute()
        {
            return mImages.Current.Value != null
                && !mImages.Current.Value.IsAnalyzing.Value
                && !mImages.Current.Value.IsAnalyzed.Value;
        }

        public void Execute()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            var imageViewModel = mImages.Current.Value!;

            imageViewModel.IsAnalyzing.Value = true;

            Task.Run(async () =>
            {
                var fullImage = SKImage.FromEncodedData(imageViewModel.FilePath);

                var attempts = 1;
                while (fullImage == null && attempts < 10)
                {
                    // Errored.. Image probably not fully written yet
                    await Task.Delay(attempts * 100).ConfigureAwait(false);

                    fullImage = SKImage.FromEncodedData(imageViewModel.FilePath);

                    attempts++;
                }

                if (fullImage == null)
                {
                    imageViewModel.IsAnalyzing.Value = false;
                    
                    // TODO: Report
                    return;
                }

                using var itemsImageStream = new MemoryStream();

                fullImage.Subset(mImages.ItemsRectangle.Value)
                    .Encode(SKEncodedImageFormat.Png, 100)
                    .SaveTo(itemsImageStream);

                itemsImageStream.Position = 0;

                using var ocr = new AzureOcr(mSettings.ApiKey.Value, mSettings.ApiEndPoint.Value);
                var itemsText = ocr.ExtractTextAsync(itemsImageStream);

                using var locationImageStream = new MemoryStream();

                fullImage.Subset(mImages.LocationRectangle.Value)
                    .Encode(SKEncodedImageFormat.Png, 100)
                    .SaveTo(locationImageStream);

                locationImageStream.Position = 0;

                try
                {
                    var locationText = await ocr.ExtractTextAsync(locationImageStream).ConfigureAwait(false);

                    var (isBuying, items) = AzureOcr.ParseItems(await itemsText.ConfigureAwait(false));

                    await dispatcher.BeginInvoke(() =>
                    {
                        imageViewModel.IsAnalyzing.Value = false;
                        imageViewModel.IsAnalyzed.Value = true;

                        mLocations.AddLocation(locationText, items, isBuying);
                    });
                }
                catch (TaskCanceledException)
                {
                    imageViewModel.IsAnalyzing.Value = false;
                    imageViewModel.IsAnalyzed.Value = false;
                }
            });
        }

        private void ImagesCurrent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            if (mCurrentImageModel != null)
            {
                mCurrentImageModel.IsAnalyzed.PropertyChanged -= this.IsAnalyzed_PropertyChanged;
                mCurrentImageModel.IsAnalyzing.PropertyChanged -= this.IsAnalyzed_PropertyChanged;
            }

            mCurrentImageModel = mImages.Current.Value;
            if (mCurrentImageModel == null)
            {
                return;
            }

            mCurrentImageModel.IsAnalyzed.PropertyChanged += this.IsAnalyzed_PropertyChanged;
            mCurrentImageModel.IsAnalyzing.PropertyChanged += this.IsAnalyzed_PropertyChanged;
        }

        private void IsAnalyzed_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            mImages.Current.PropertyChanged -= this.ImagesCurrent_PropertyChanged;

            if (mCurrentImageModel != null)
            {
                mCurrentImageModel.IsAnalyzed.PropertyChanged -= this.IsAnalyzed_PropertyChanged;
                mCurrentImageModel.IsAnalyzing.PropertyChanged -= this.IsAnalyzed_PropertyChanged;
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }
    }
}
