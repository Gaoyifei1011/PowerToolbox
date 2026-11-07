using System;
using System.Collections.Generic;
using Windows.Foundation;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    internal class WrapPanel : Panel
    {
        private readonly List<Row> _rows = [];

        internal Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        internal static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(WrapPanel), new(Orientation.Horizontal, OnLayoutPropertyChanged));

        internal Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        internal static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(WrapPanel), new(new Thickness(), OnLayoutPropertyChanged));

        internal double ItemSpacing
        {
            get { return (double)GetValue(ItemSpacingProperty); }
            set { SetValue(ItemSpacingProperty, value); }
        }

        internal static readonly DependencyProperty ItemSpacingProperty = DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(WrapPanel), new(0.0, OnLayoutPropertyChanged));

        internal double LineSpacing
        {
            get { return (double)GetValue(LineSpacingProperty); }
            set { SetValue(LineSpacingProperty, value); }
        }

        internal static readonly DependencyProperty LineSpacingProperty = DependencyProperty.Register(nameof(LineSpacing), typeof(double), typeof(WrapPanel), new(0.0, OnLayoutPropertyChanged));

        internal WrapPanelItemsStretch ItemsStretch
        {
            get { return (WrapPanelItemsStretch)GetValue(ItemsStretchProperty); }
            set { SetValue(ItemsStretchProperty, value); }
        }

        internal static readonly DependencyProperty ItemsStretchProperty = DependencyProperty.Register(nameof(ItemsStretch), typeof(WrapPanelItemsStretch), typeof(WrapPanel), new(WrapPanelItemsStretch.None, OnLayoutPropertyChanged));

        private static void OnLayoutPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WrapPanel panel = (WrapPanel)sender;

            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness padding = Padding;

            Size childAvailableSize = new(availableSize.Width - padding.Left - padding.Right, availableSize.Height - padding.Top - padding.Bottom);

            foreach (UIElement child in Children)
            {
                child.Measure(childAvailableSize);
            }

            return UpdateRows(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Orientation orientation = Orientation;

            if ((orientation is Orientation.Horizontal && finalSize.Width < DesiredSize.Width) || (orientation is Orientation.Vertical && finalSize.Height < DesiredSize.Height))
            {
                UpdateRows(finalSize);
            }

            int childIndex = 0;

            foreach (Row row in _rows)
            {
                foreach (UvRect uvRect in row.ChildrenRects)
                {
                    while (childIndex < Children.Count && Children[childIndex].Visibility is Visibility.Collapsed)
                    {
                        childIndex++;
                    }

                    if (childIndex >= Children.Count)
                    {
                        break;
                    }

                    UIElement child = Children[childIndex];
                    childIndex++;

                    UvRect arrangeRect = new()
                    {
                        Position = uvRect.Position,
                        Size = new(uvRect.Size.U, row.Size.V)
                    };

                    Rect finalRect = arrangeRect.ToRect(orientation);
                    child.Arrange(finalRect);
                }
            }

            return finalSize;
        }

        private Size UpdateRows(Size availableSize)
        {
            _rows.Clear();

            Orientation orientation = Orientation;
            Thickness padding = Padding;
            UvMeasure paddingStart = new(orientation, padding.Left, padding.Top);
            UvMeasure paddingEnd = new(orientation, padding.Right, padding.Bottom);

            if (Children.Count is 0)
            {
                UvMeasure emptySize = paddingStart.Add(paddingEnd.U, paddingEnd.V);
                return emptySize.ToSize(orientation);
            }

            UvMeasure parentMeasure = new(orientation, availableSize.Width, availableSize.Height);
            UvMeasure spacingMeasure = new(ItemSpacing, LineSpacing);
            UvMeasure position = new(orientation, padding.Left, padding.Top);

            Row currentRow = new();
            UvMeasure finalMeasure = new(0, 0);

            void arrange(UIElement child, bool isLast)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    return;
                }

                UvMeasure desiredMeasure = new(orientation, child.DesiredSize);

                if ((desiredMeasure.U + position.U + paddingEnd.U) > parentMeasure.U)
                {
                    position.U = paddingStart.U;
                    position.V += currentRow.Size.V + spacingMeasure.V;
                    _rows.Add(currentRow);
                    currentRow = new();
                }

                if (isLast)
                {
                    desiredMeasure.U = parentMeasure.U - position.U;
                }

                currentRow.Add(position, desiredMeasure);
                position.U += desiredMeasure.U + spacingMeasure.U;
                finalMeasure.U = Math.Max(finalMeasure.U, position.U);
            }

            int lastIndex = Children.Count - 1;

            for (int i = 0; i < lastIndex; i++)
            {
                arrange(Children[i], false);
            }

            arrange(Children[lastIndex], ItemsStretch == WrapPanelItemsStretch.Last);

            if (currentRow.ChildrenRects.Count > 0)
            {
                _rows.Add(currentRow);
            }

            if (_rows.Count == 0)
            {
                UvMeasure emptySize = paddingStart.Add(paddingEnd.U, paddingEnd.V);
                return emptySize.ToSize(orientation);
            }

            UvRect lastRowRect = _rows[_rows.Count - 1].Rect();
            finalMeasure.V = lastRowRect.Position.V + lastRowRect.Size.V;
            UvMeasure finalRect = finalMeasure.Add(paddingEnd.U, paddingEnd.V);
            return finalRect.ToSize(orientation);
        }
    }
}