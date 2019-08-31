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
            this.Height += e.VerticalChange;

            this.Width -= e.HorizontalChange;

            this.SetRectangle();
        }

        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width += e.HorizontalChange;
            this.Height += e.VerticalChange;

            this.SetRectangle();
        }

        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width += e.HorizontalChange;
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Height -= e.VerticalChange;

            this.SetRectangle();
        }

        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Width -= e.HorizontalChange;
            this.Height -= e.VerticalChange;

            this.SetRectangle();
        }

        private void SetRectangle()
        {
            this.Rectangle = new SKRectI(
                Convert.ToInt32(Canvas.GetLeft(this)),
                Convert.ToInt32(Canvas.GetTop(this)),
                Convert.ToInt32(Canvas.GetLeft(this) + this.ActualWidth),
                Convert.ToInt32(Canvas.GetTop(this) + this.ActualHeight));
        }
    }
}
