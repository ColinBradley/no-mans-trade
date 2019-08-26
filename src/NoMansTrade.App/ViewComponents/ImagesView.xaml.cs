using NoMansTrade.App.Controls;
using SkiaSharp;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NoMansTrade.App.ViewComponents
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ImagesView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ImagesView()
        {
            this.InitializeComponent();

            this.SizeChanged += this.ImagesView_SizeChanged;

            ItemsRectangle.Changed += this.Rectangle_Changed;
            LocationRectangle.Changed += this.Rectangle_Changed;
        }

        public (SKRectI, SKRectI) ImageLocations
        {
            get
            {
                if (ImageDisplay == null || ImageDisplay.Source == null)
                {
                    return (new SKRectI(), new SKRectI());
                }

                var imageSizeRatio = ImageDisplay.Source.Width / ImageDisplay.ActualWidth;
                var imageTop = (((FrameworkElement)ImageDisplay.Parent).ActualHeight - ImageDisplay.ActualHeight) / 2;

                var itemsRectangle = GetImageRectangle(imageSizeRatio, imageTop, ItemsRectangle);
                var locationRectangle = GetImageRectangle(imageSizeRatio, imageTop, LocationRectangle);

                return (itemsRectangle, locationRectangle);
            }
        }


        private static SKRectI GetImageRectangle(double imageSizeRatio, double imageTop, DragSelectionBox selectionRectangle)
        {
            return new SKRectI(
                Convert.ToInt32(Canvas.GetLeft(selectionRectangle) * imageSizeRatio),
                Convert.ToInt32((Canvas.GetTop(selectionRectangle) - imageTop) * imageSizeRatio),
                Convert.ToInt32((Canvas.GetLeft(selectionRectangle) + selectionRectangle.ActualWidth) * imageSizeRatio),
                Convert.ToInt32((Canvas.GetTop(selectionRectangle) - imageTop + selectionRectangle.ActualHeight) * imageSizeRatio));
        }

        private void Rectangle_Changed(DragSelectionBox sender)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ImageLocations)));
        }

        private void ImagesView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ImageLocations)));
        }
    }
}
