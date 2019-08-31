using NoMansTrade.App.Commands;
using NoMansTrade.App.Support;
using NoMansTrade.Core.Model;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoMansTrade.App.ViewModels
{
    internal class DirectoryImages : System.IDisposable
    {
        private readonly ObservableCollection<Image> mImagesSource = new ObservableCollection<Image>();
        private readonly Dispatcher mDispatcher;
        private readonly Settings mSettings;

        private FileSystemWatcher? mWatcher;

        public DirectoryImages(Locations locations, Settings settings)
        {
            mDispatcher = Dispatcher.CurrentDispatcher;
            mSettings = settings;

            this.Images = new ReadOnlyObservableCollection<Image>(mImagesSource);
            this.Current = new ObservableProperty<Image?>(mImagesSource.FirstOrDefault());

            ((INotifyCollectionChanged)this.Images).CollectionChanged += this.Images_CollectionChanged;

            this.NextImage = new NextImageCommand(this);
            this.PreviousImage = new PreviousImageCommand(this);
            this.AnalyzeImage = new AnalyzeImageCommand(this, locations, settings);

            mSettings.ScanDirectory.PropertyChanged += this.ScanDirectory_PropertyChanged;
        }

        private void ScanDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Initialize();
        }

        public ObservableProperty<Image?> Current { get; private set; }

        public ObservableProperty<SKRectI> ItemsRectangle { get; } = new ObservableProperty<SKRectI>(new SKRectI(200, 50, 400, 200));

        public ObservableProperty<SKRectI> LocationRectangle { get; } = new ObservableProperty<SKRectI>(new SKRectI(50, 250, 500, 350));

        internal void SetAnalyzedImages(string[] analyzedNames)
        {
            foreach (var name in analyzedNames)
            {
                var image = this.Images.FirstOrDefault(i => i.FilePath.EndsWith(name));
                if (image == null)
                {
                    continue;
                }

                image.IsAnalyzed.Value = true;
            }
        }

        public ReadOnlyObservableCollection<Image> Images { get; }

        public ICommand NextImage { get; }

        public ICommand PreviousImage { get; }

        public ICommand AnalyzeImage { get; }

        public Task Initialize()
        {
            if (mWatcher != null)
            {
                mWatcher.Dispose();
            }

            mWatcher = new FileSystemWatcher(mSettings.ScanDirectory.Value, "*.png");
            mWatcher.Created += this.mWatcher_Changed;
            mWatcher.Deleted += this.mWatcher_Changed;
            mWatcher.Changed += this.mWatcher_Changed;
            mWatcher.EnableRaisingEvents = true;

            return this.ReadImagesFromPath();
        }

        private async void mWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            await this.ReadImagesFromPath();
        }

        private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Current.Value = mImagesSource.LastOrDefault();
        }

        private async Task ReadImagesFromPath()
        {
            lock (mImagesSource)
            {
                var unusedImages = mImagesSource.ToDictionary(i => i.FilePath);

                foreach (var filePath in Directory.GetFiles(mSettings.ScanDirectory.Value, "*.png", SearchOption.TopDirectoryOnly))
                {
                    if (unusedImages.Remove(filePath))
                    {
                        // Already loaded
                        continue;
                    }

                    Image? image = null;
                    mDispatcher.Invoke(() => image = new Image(filePath));

                    mImagesSource.Add(image!);
                }

                foreach (var unusedImage in unusedImages.Values)
                {
                    mImagesSource.Remove(unusedImage);
                }

                var newOrder = mImagesSource.OrderBy(i => i.Date).ToArray();
                for (var index = 0; index < newOrder.Length; index++)
                {
                    mImagesSource[index] = newOrder[index];
                }
            }
        }

        public void Dispose()
        {
            if (mWatcher != null)
            {
                mWatcher.Dispose();
            }
        }
    }
}
