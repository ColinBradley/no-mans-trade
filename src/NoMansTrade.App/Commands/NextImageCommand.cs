using NoMansTrade.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Input;

namespace NoMansTrade.App.Commands
{
    internal class NextImageCommand : ICommand, IDisposable
    {
        private readonly DirectoryImages mImages;

        public event EventHandler? CanExecuteChanged;

        public NextImageCommand(DirectoryImages images)
        {
            mImages = images;
            ((INotifyCollectionChanged)images.Images).CollectionChanged += this.mImagesImages_CollectionChanged;
            images.Current.PropertyChanged += this.Current_PropertyChanged;
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return mImages.Images.IndexOf(mImages.Current.Value!) < (mImages.Images.Count - 1);
        }

        public void Execute(object parameter)
        {
            mImages.Current.Value = mImages.Images[mImages.Images.IndexOf(mImages.Current.Value!) + 1];
        }

        private void mImagesImages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            ((INotifyCollectionChanged)mImages.Images).CollectionChanged -= this.mImagesImages_CollectionChanged;
        }
    }
}
