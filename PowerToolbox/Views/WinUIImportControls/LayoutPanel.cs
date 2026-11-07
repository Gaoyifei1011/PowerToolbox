using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    internal class LayoutPanel : Panel
    {
        private LayoutPanelLayoutContext _layoutContext;
        private Layout _layout;
        private object _layoutState;

        internal object LayoutState
        {
            get { return _layoutState; }
            set { _layoutState = value; }
        }

        internal Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        internal static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(nameof(Layout), typeof(Layout), typeof(LayoutPanel), new(null, OnLayoutChanged));

        internal Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        internal static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(LayoutPanel), new(null));

        internal Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        internal static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(LayoutPanel), new(new()));

        internal Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        internal static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(LayoutPanel), new(new(), OnPaddingChanged));

        internal CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        internal static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(LayoutPanel), new(new()));

        internal LayoutPanel()
        { }

        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness padding = Padding;
            Thickness border = BorderThickness;

            double horizontalPadding = padding.Left + padding.Right + border.Left + border.Right;
            double verticalPadding = padding.Top + padding.Bottom + border.Top + border.Bottom;

            Size adjustedSize = new(Math.Max(0, availableSize.Width - horizontalPadding), Math.Max(0, availableSize.Height - verticalPadding));
            Layout layout = _layout;

            if (layout is not null)
            {
                Size desiredSize = layout.Measure(_layoutContext, adjustedSize);
                return new(desiredSize.Width + horizontalPadding, desiredSize.Height + verticalPadding);
            }

            Size result = new();

            foreach (UIElement child in Children)
            {
                child.Measure(adjustedSize);
                Size childSize = child.DesiredSize;
                result.Width = Math.Max(result.Width, childSize.Width);
                result.Height = Math.Max(result.Height, childSize.Height);
            }

            result.Width += horizontalPadding;
            result.Height += verticalPadding;

            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Thickness padding = Padding;
            Thickness border = BorderThickness;

            double horizontalPadding = padding.Left + padding.Right + border.Left + border.Right;
            double verticalPadding = padding.Top + padding.Bottom + border.Top + border.Bottom;
            double leftAdjustment = padding.Left + border.Left;
            double topAdjustment = padding.Top + border.Top;
            Size adjustedSize = new(Math.Max(0, finalSize.Width - horizontalPadding), Math.Max(0, finalSize.Height - verticalPadding));
            Layout layout = _layout;

            if (layout is not null)
            {
                Size layoutSize = layout.Arrange(_layoutContext, adjustedSize);

                layoutSize.Width += horizontalPadding;
                layoutSize.Height += verticalPadding;

                if (leftAdjustment is not 0 || topAdjustment is not 0)
                {
                    foreach (UIElement child in Children)
                    {
                        if (child is FrameworkElement element)
                        {
                            Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
                            layoutSlot.X += leftAdjustment;
                            layoutSlot.Y += topAdjustment;
                            element.Arrange(layoutSlot);
                        }
                    }
                }
                return layoutSize;
            }

            Rect arrangeRect = new(leftAdjustment, topAdjustment, adjustedSize.Width, adjustedSize.Height);

            foreach (UIElement child in Children)
            {
                child.Arrange(arrangeRect);
            }

            return finalSize;
        }

        private static void OnLayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LayoutPanel panel = (LayoutPanel)sender;
            panel.OnLayoutChanged((Layout)args.OldValue, (Layout)args.NewValue);
        }

        private void OnLayoutChanged(Layout oldLayout, Layout newLayout)
        {
            _layoutContext ??= new LayoutPanelLayoutContext(this);

            if (oldLayout is not null)
            {
                oldLayout.MeasureInvalidated -= OnLayoutMeasureInvalidated;
                oldLayout.ArrangeInvalidated -= OnLayoutArrangeInvalidated;
                oldLayout.UninitializeForContext(_layoutContext);
            }

            _layout = newLayout;

            if (newLayout is not null)
            {
                newLayout.InitializeForContext(_layoutContext);
                newLayout.MeasureInvalidated += OnLayoutMeasureInvalidated;
                newLayout.ArrangeInvalidated += OnLayoutArrangeInvalidated;
            }

            InvalidateMeasure();
        }

        private void OnLayoutMeasureInvalidated(Layout sender, object args)
        {
            InvalidateMeasure();
        }

        private void OnLayoutArrangeInvalidated(Layout sender, object args)
        {
            InvalidateArrange();
        }

        private static void OnPaddingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((LayoutPanel)sender).InvalidateMeasure();
        }
    }
}
