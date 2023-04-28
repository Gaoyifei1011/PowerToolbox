﻿using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace FileRenamer.Converters.Controls
{
    /// <summary>
    /// 适应性网格视图高度值计算转换器
    /// </summary>
    public class AdaptiveHeightValueConverter : IValueConverter
    {
        private Thickness thickness = new Thickness(0, 0, 4, 4);

        public Thickness DefaultItemMargin
        {
            get { return thickness; }
            set { thickness = value; }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not null)
            {
                var gridView = (GridView)parameter;
                if (gridView is null)
                {
                    return value;
                }

                double height = System.Convert.ToDouble(value);

                Thickness padding = gridView.Padding;
                Thickness margin = GetItemMargin(gridView, DefaultItemMargin);
                height = height + margin.Top + margin.Bottom + padding.Top + padding.Bottom;

                return height;
            }

            return double.NaN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }

        public static Thickness GetItemMargin(GridView view, Thickness fallback = default)
        {
            Setter setter = view.ItemContainerStyle?.Setters.OfType<Setter>().FirstOrDefault(s => s.Property == FrameworkElement.MarginProperty);
            if (setter is not null)
            {
                return (Thickness)setter.Value;
            }
            else
            {
                if (view.Items.Count > 0)
                {
                    var container = (GridViewItem)view.ContainerFromIndex(0);
                    if (container is not null)
                    {
                        return container.Margin;
                    }
                }

                // 使用GridViewItem的默认厚度
                return fallback;
            }
        }
    }
}
