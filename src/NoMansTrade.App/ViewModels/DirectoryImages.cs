using NoMansTrade.App.Commands;
using NoMansTrade.App.Support;
using NoMansTrade.Core.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NoMansTrade.App.ViewModels
{
    internal class DirectoryImages
    {
        private readonly ObservableCollection<Image> mImagesSource = new ObservableCollection<Image>();
        private readonly Settings mSettings;

        private FileSystemWatcher? mWatcher;

        public DirectoryImages(Locations locations, Settings settings)
        {
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
        
        public ReadOnlyObservableCollection<Image> Images { get; }

        public ICommand NextImage { get; }

        public ICommand PreviousImage { get; }

        public ICommand AnalyzeImage { get; }

        public void Initialize()
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

            this.ReadImagesFromPath();
        }

        private void mWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.ReadImagesFromPath();
        }

        private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Current.Value = mImagesSource.LastOrDefault();
        }

        private void ReadImagesFromPath()
        {
            var unusedImages = mImagesSource.ToDictionary(i => i.FilePath);

            foreach (var filePath in Directory.GetFiles(mSettings.ScanDirectory.Value, "*.png", SearchOption.TopDirectoryOnly))
            {
                if (unusedImages.Remove(filePath))
                {
                    // Already loaded
                    continue;
                }

                var image = new Image(filePath);
                mImagesSource.Add(image);
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
}
