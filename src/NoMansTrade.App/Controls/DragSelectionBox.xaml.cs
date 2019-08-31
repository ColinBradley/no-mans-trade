using SkiaSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NoMansTrade.App.Controls
{
    /// <summary>
    /// Interaction logic for DragSelectionBox.xaml
    /// </summary>
    public partial class DragSelectionBox : UserControl
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                "Color",
                typeof(Brush),
                typeof(DragSelectionBox),
                new PropertyMetadata(new BrushConverter().ConvertFromString("DodgerBlue"))
            );

        public static readonly DependencyProperty RectangleProperty =
            DependencyProperty.Register(
                "Rectangle",
                typeof(SKRectI),
                typeof(DragSelectionBox),
                new PropertyMetadata(new SKRectI(100, 100, 200, 200), (s,e) => ((DragSelectionBox)s).Rectangle_Changed())
            );

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(string),
                typeof(DragSelectionBox),
                new PropertyMetadata("")
            );

        public DragSelectionBox()
        {
            this.InitializeComponent();

            MoveThumb.DragDelta += this.MoveThumb_DragDelta;
            TopLeftThumb.DragDelta += this.TopLeftThumb_DragDelta;
            TopRightThumb.DragDelta += this.TopRightThumb_DragDelta;
            BottomRightThumb.DragDelta += this.BottomRightThumb_DragDelta;
            BottomLeftThumb.DragDelta += this.BottomLeftThumb_DragDelta;
        }

        public Brush Color
        {
            get { return (Brush)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public SKRectI Rectangle
        {
            get { return (SKRectI)this.GetValue(RectangleProperty); }
            set { this.SetValue(RectangleProperty, value); }
        }

        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        private void Rectangle_Changed()
        {
            Canvas.SetTop(this, this.Rectangle.Top);
            Canvas.SetLeft(this, this.Rectangle.Left);

            this.Width = this.Rectangle.Width;
            this.Height = this.Rectangle.Height;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.SetRectangle();
        }

        private void BottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);

            this.Height = Math.Max(this.Height + e.VerticalChange, 0);
            this.Width = Math.Max(this.Width - e.HorizontalChange, 0);

            this.SetRectangle();
        }

        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width = Math.Max(this.Width + e.HorizontalChange, 0);
            this.Height = Math.Max(this.Height + e.VerticalChange, 0);

            this.SetRectangle();
        }

        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Width = Math.Max(this.Width + e.HorizontalChange, 0);
            this.Height = Math.Max(this.Height - e.VerticalChange, 0);

            this.SetRectangle();
        }

        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Width = Math.Max(this.Width - e.HorizontalChange, 0);
            this.Height = Math.Max(this.Height - e.VerticalChange, 0);

            this.SetRectangle();
        }

        private void SetRectangle()
        {
            this.Rectangle = new SKRectI(
                Convert.ToInt32(Canvas.GetLeft(this)),
                Convert.ToInt32(Canvas.GetTop(this)),
                Convert.ToInt32(Canvas.GetLeft(this) + this.Width),
                Convert.ToInt32(Canvas.GetTop(this) + this.Height));
        }
    }
}
