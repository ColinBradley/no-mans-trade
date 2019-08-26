using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

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

        public event Action<DragSelectionBox>? Changed;

        public DragSelectionBox()
        {
            this.InitializeComponent();

            MoveThumb.DragDelta += this.MoveThumb_DragDelta;
            TopLeftThumb.DragDelta += this.TopLeftThumb_DragDelta;
            TopRightThumb.DragDelta += this.TopRightThumb_DragDelta;
            BottomRightThumb.DragDelta += this.BottomRightThumb_DragDelta;
            BottomLeftThumb.DragDelta += this.BottomLeftThumb_DragDelta;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            ((FrameworkElement)this.Parent).SizeChanged += this.Parent_SizeChanged;
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0)
            {
                // Initial sizing
                return;
            }

            // Really, we want to adjust based on how the image changes, not how the parent changed... but it doesn't really matter
            var horizontalDelta = e.NewSize.Width / e.PreviousSize.Width;
            var verticalDelta = e.NewSize.Height - e.PreviousSize.Height;

            // Assume height is always the largest dimension
            Canvas.SetLeft(this, Canvas.GetLeft(this) * horizontalDelta);
            Canvas.SetTop(this, Canvas.GetTop(this) + (verticalDelta / 2));

            // Height typically doesn't change much
            this.Width *= horizontalDelta;
        }

        public Brush Color
        {
            get { return (Brush)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Changed?.Invoke(this);
        }

        private void BottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            this.Height += e.VerticalChange;

            this.Width -= e.HorizontalChange;

            this.Changed?.Invoke(this);
        }

        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width += e.HorizontalChange;
            this.Height += e.VerticalChange;

            this.Changed?.Invoke(this);
        }

        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width += e.HorizontalChange;
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Height -= e.VerticalChange;

            this.Changed?.Invoke(this);
        }

        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

            this.Width -= e.HorizontalChange;
            this.Height -= e.VerticalChange;

            this.Changed?.Invoke(this);
        }
    }
}
