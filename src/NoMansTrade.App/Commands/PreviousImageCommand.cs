using NoMansTrade.App.ViewModels;
using System;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoMansTrade.App.Commands
{
    internal class PreviousImageCommand : ICommand, IDisposable
    {
        private readonly Dispatcher mDispatcher;
        private readonly DirectoryImages mImages;

        public event EventHandler? CanExecuteChanged;

        public PreviousImageCommand(DirectoryImages images)
        {
            mDispatcher = Dispatcher.CurrentDispatcher;

            mImages = images;
            ((INotifyCollectionChanged)images.Images).CollectionChanged += this.mImagesImages_CollectionChanged;
        }

        public bool CanExecute(object? parameter)
        {
            return mImages.Images.IndexOf(mImages.Current.Value!) > 0;
        }

        public void Execute(object? parameter)
        {
            mImages.Current.Value = mImages.Images[mImages.Images.IndexOf(mImages.Current.Value!) - 1];
        }

        private void mImagesImages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _ = mDispatcher.BeginInvoke(
                () => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        public void Dispose()
        {
            ((INotifyCollectionChanged)mImages.Images).CollectionChanged -= this.mImagesImages_CollectionChanged;
        }
    }
}
