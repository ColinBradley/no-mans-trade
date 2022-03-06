using NoMansTrade.App.Commands;
using NoMansTrade.App.Support;
using NoMansTrade.Core.Model;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoMansTrade.App.ViewModels
{
    internal class DirectoryImages : System.IDisposable
    {
        private readonly ObservableCollection<Image> mImagesSource = new();
        private readonly Dispatcher mDispatcher;
        private readonly Settings mSettings;

        private readonly Action mReadImagesFromPathDebounced;

        private FileSystemWatcher? mWatcher;

        private bool mIsInitializing = false;

        public DirectoryImages(LocationCollection locations, Settings settings)
        {
            mDispatcher = Dispatcher.CurrentDispatcher;
            mSettings = settings;

            this.Images = new ReadOnlyObservableCollection<Image>(mImagesSource);
            this.Current = new ObservableProperty<Image?>(mImagesSource.FirstOrDefault());


            this.NextImage = new NextImageCommand(this);
            this.PreviousImage = new PreviousImageCommand(this);
            this.AnalyzeImage = new AnalyzeImageCommand(this, locations, settings);

            mReadImagesFromPathDebounced = CreateDebounce(this.ReadImagesFromPath);
        }

        public void Initiliaze()
        {
            this.Load();

            this.Current.Value = mImagesSource.LastOrDefault();

            ((INotifyCollectionChanged)this.Images).CollectionChanged += this.Images_CollectionChanged;
            mSettings.ScanDirectory.PropertyChanged += this.ScanDirectory_PropertyChanged;
        }

        private void ScanDirectory_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Load();
        }

        public ObservableProperty<Image?> Current { get; private set; }

        public ObservableProperty<SKRectI> ItemsRectangle { get; } = new(new SKRectI(200, 50, 400, 200));

        public ObservableProperty<SKRectI> LocationRectangle { get; } = new(new SKRectI(50, 250, 500, 350));

        internal void SetAnalyzedImages(string[] analyzedNames)
        {
            foreach (var name in analyzedNames)
            {
                var image = this.Images.FirstOrDefault(i => i.FilePath.EndsWith(name, System.StringComparison.OrdinalIgnoreCase));
                if (image == null)
                {
                    continue;
                }

                image.IsAnalyzed.Value = true;
            }
        }

        public ReadOnlyObservableCollection<Image> Images { get; }

        public NextImageCommand NextImage { get; }

        public PreviousImageCommand PreviousImage { get; }

        public AnalyzeImageCommand AnalyzeImage { get; }

        private void Load()
        {
            try
            {
                mIsInitializing = true;

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
            finally
            {
                mIsInitializing = false;
            }
        }

        private void mWatcher_Changed(object? sender, FileSystemEventArgs e)
        {
            // Use a debouncer because at this point a file might not be fully written to
            // Or someone might be pasting in loads of files? Doesn't really matter - debouncing is lovely
            mReadImagesFromPathDebounced();
        }

        private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.Current.Value = mImagesSource.LastOrDefault();

            if (!mIsInitializing && mSettings.AutoScanNewImages.Value && this.AnalyzeImage.CanExecute())
            {
                _ = this.AnalyzeImage.Execute();
            }
        }

        private void ReadImagesFromPath()
        {
            lock (mImagesSource)
            {
                var unusedImages = mImagesSource.ToDictionary(i => i.FilePath);

                // Image has to be created on main thread because it has ObservableProperties.
                // Could just pass them the right dispatcher... but that's faff
                mDispatcher.Invoke(() =>
                {
                    foreach (var filePath in Directory.GetFiles(mSettings.ScanDirectory.Value, "*.png", SearchOption.TopDirectoryOnly))
                    {
                        if (unusedImages.Remove(filePath))
                        {
                            // Already loaded
                            continue;
                        }

                        mImagesSource.Add(new Image(filePath));
                    }
                });

                foreach (var unusedImage in unusedImages.Values)
                {
                    _ = mImagesSource.Remove(unusedImage);
                }

                var newOrder = mImagesSource.OrderBy(i => i.Date).ToArray();
                for (var index = 0; index < newOrder.Length; index++)
                {
                    if (mImagesSource[index] == newOrder[index])
                    {
                        // Avoid setting the same value, as that raises a replace event ¬_¬
                        continue;
                    }

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

        private static Action CreateDebounce(Action func, int milliseconds = 500)
        {
            CancellationTokenSource? cancelTokenSource = null;

            return () =>
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource = new CancellationTokenSource();

                _ = Task.Delay(milliseconds, cancelTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            func();
                        }
                    }, TaskScheduler.Default);
            };
        }
    }
}
