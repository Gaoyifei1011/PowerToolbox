using FileRenamer.Converters.Controls;
using System;
using System.Windows.Input;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace FileRenamer.Views.CustomControls.Collections
{
    /// <summary>
    /// ����Ӧ���ڿ�ȵ�����ؼ�
    /// </summary>
    public partial class AdaptiveGridView : GridView
    {
        private bool _isLoaded;

        private ScrollMode _savedVerticalScrollMode;

        private ScrollMode _savedHorizontalScrollMode;

        private ScrollBarVisibility _savedVerticalScrollBarVisibility;

        private ScrollBarVisibility _savedHorizontalScrollBarVisibility;

        private Orientation _savedOrientation;

        private bool _needToRestoreScrollStates;

        private bool _needContainerMarginForLayout;

        /// <summary>
        /// ��ȡ������ÿ�����������
        /// </summary>
        public double DesiredWidth
        {
            get { return (double)GetValue(DesiredWidthProperty); }
            set { SetValue(DesiredWidthProperty, value); }
        }

        public static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.Register("DesiredWidth", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN, DesiredWidthChanged));

        /// <summary>
        /// ��ȡ�������ڵ������� IsItemClickEnabled ����Ϊ true ʱҪִ�е�����
        /// </summary>
        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(AdaptiveGridView), new PropertyMetadata(null));

        /// <summary>
        /// ��ȡ������������ÿ����ĸ߶�
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN));

        /// <summary>
        /// ��ȡ������������ÿ����Ŀ��
        /// </summary>
        private double ItemWidth
        {
            set { SetValue(ItemWidthProperty, value); }
        }

        private static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN));

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ�Ƿ�ֻӦ��ʾһ��
        /// </summary>
        public bool OneRowModeEnabled
        {
            get { return (bool)GetValue(OneRowModeEnabledProperty); }
            set { SetValue(OneRowModeEnabledProperty, value); }
        }

        public static readonly DependencyProperty OneRowModeEnabledProperty =
            DependencyProperty.Register("OneRowModeEnabled", typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(false, delegate (DependencyObject o, DependencyPropertyChangedEventArgs args)
            {
                OnOneRowModeEnabledChanged(o, args.NewValue);
            }));

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ�ؼ��Ƿ�Ӧ�����������������һ��
        /// </summary>
        public bool StretchContentForSingleRow
        {
            get { return (bool)GetValue(StretchContentForSingleRowProperty); }
            set { SetValue(StretchContentForSingleRowProperty, value); }
        }

        public static readonly DependencyProperty StretchContentForSingleRowProperty =
            DependencyProperty.Register("StretchContentForSingleRow", typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(true, OnStretchContentForSingleRowPropertyChanged));

        public AdaptiveGridView()
        {
            IsTabStop = false;
            SizeChanged += OnSizeChanged;
            ItemClick += OnItemClick;
            Items.VectorChanged += ItemsOnVectorChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            UseLayoutRounding = false;
        }

        ~AdaptiveGridView()
        {
            SizeChanged -= OnSizeChanged;
            ItemClick -= OnItemClick;
            Items.VectorChanged -= ItemsOnVectorChanged;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }

        /// <summary>׼��ָ����Ԫ������ʾָ�����</summary>
        /// <param name="obj">������ʾָ�����Ԫ�ء�</param>
        /// <param name="item">Ҫ��ʾ���</param>
        protected override void PrepareContainerForItemOverride(DependencyObject obj, object item)
        {
            base.PrepareContainerForItemOverride(obj, item);
            if (obj is FrameworkElement frameworkElement)
            {
                Binding binding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemHeight"),
                    Mode = BindingMode.TwoWay
                };
                Binding binding2 = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemWidth"),
                    Mode = BindingMode.TwoWay
                };
                frameworkElement.SetBinding(HeightProperty, binding);
                frameworkElement.SetBinding(WidthProperty, binding2);
            }

            if (obj is ContentControl contentControl)
            {
                contentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                contentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            }

            if (_needContainerMarginForLayout)
            {
                _needContainerMarginForLayout = false;
                RecalculateLayout(ActualWidth);
            }
        }

        /// <summary>����������Ŀ�ȡ�</summary>
        /// <param name="containerWidth">�����ؼ��Ŀ�ȡ�</param>
        /// <returns>�������Ŀ��ȡ�</returns>
        private double CalculateItemWidth(double containerWidth)
        {
            if (double.IsNaN(DesiredWidth))
            {
                return DesiredWidth;
            }

            int num = CalculateColumns(containerWidth, DesiredWidth);
            if (Items is not null && Items.Count > 0 && Items.Count < num && StretchContentForSingleRow)
            {
                num = Items.Count;
            }

            Thickness thickness = default;
            Thickness itemMargin = AdaptiveHeightValueConverter.GetItemMargin(this, thickness);
            if (itemMargin.Equals(thickness))
            {
                _needContainerMarginForLayout = true;
            }

            return containerWidth / num - itemMargin.Left - itemMargin.Right;
        }

        /// <summary>
        /// ÿ��Ӧ�ó��������ڲ����̣����ؽ����ִ��ݣ����� ApplyTemplate ʱ���á�����˵������ζ����Ӧ������ʾ UI Ԫ��֮ǰ���ø÷�������д�˷�����Ӱ�����Ĭ�Ϻ�ģ���߼���
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnOneRowModeEnabledChanged(this, OneRowModeEnabled);
        }

        private void ItemsOnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
        {
            if (!double.IsNaN(ActualWidth))
            {
                RecalculateLayout(ActualWidth);
            }
        }

        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            ICommand itemClickCommand = ItemClickCommand;
            if (itemClickCommand is not null && itemClickCommand.CanExecute(args.ClickedItem))
            {
                itemClickCommand.Execute(args.ClickedItem);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (HorizontalAlignment is not HorizontalAlignment.Stretch)
            {
                int num = CalculateColumns(args.PreviousSize.Width, DesiredWidth);
                int num2 = CalculateColumns(args.NewSize.Width, DesiredWidth);
                if (num != num2)
                {
                    RecalculateLayout(args.NewSize.Width);
                }
            }
            else if (args.PreviousSize.Width != args.NewSize.Width)
            {
                RecalculateLayout(args.NewSize.Width);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            _isLoaded = true;
            DetermineOneRowMode();
        }

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            _isLoaded = false;
        }

        private void DetermineOneRowMode()
        {
            if (!_isLoaded)
            {
                return;
            }

            ItemsWrapGrid itemsWrapGrid = ItemsPanelRoot as ItemsWrapGrid;
            if (OneRowModeEnabled)
            {
                Binding binding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemHeight"),
                    Converter = new AdaptiveHeightValueConverter(),
                    ConverterParameter = this
                };
                if (itemsWrapGrid is not null)
                {
                    _savedOrientation = itemsWrapGrid.Orientation;
                    itemsWrapGrid.Orientation = Orientation.Vertical;
                }

                SetBinding(MaxHeightProperty, binding);
                _savedHorizontalScrollMode = ScrollViewer.GetHorizontalScrollMode(this);
                _savedVerticalScrollMode = ScrollViewer.GetVerticalScrollMode(this);
                _savedHorizontalScrollBarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(this);
                _savedVerticalScrollBarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(this);
                _needToRestoreScrollStates = true;
                ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Disabled);
                ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Hidden);
                ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Visible);
                ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Enabled);
                return;
            }

            ClearValue(MaxHeightProperty);
            if (_needToRestoreScrollStates)
            {
                _needToRestoreScrollStates = false;
                if (itemsWrapGrid is not null)
                {
                    itemsWrapGrid.Orientation = _savedOrientation;
                }

                ScrollViewer.SetVerticalScrollMode(this, _savedVerticalScrollMode);
                ScrollViewer.SetVerticalScrollBarVisibility(this, _savedVerticalScrollBarVisibility);
                ScrollViewer.SetHorizontalScrollBarVisibility(this, _savedHorizontalScrollBarVisibility);
                ScrollViewer.SetHorizontalScrollMode(this, _savedHorizontalScrollMode);
            }
        }

        private void RecalculateLayout(double containerWidth)
        {
            Panel itemsPanelRoot = ItemsPanelRoot;
            double num = itemsPanelRoot is not null ? itemsPanelRoot.Margin.Left + itemsPanelRoot.Margin.Right : 0.0;
            double num2 = Padding.Left + Padding.Right;
            double num3 = BorderThickness.Left + BorderThickness.Right;
            containerWidth = containerWidth - num2 - num - num3;
            if (containerWidth > 0.0)
            {
                double d = CalculateItemWidth(containerWidth);
                ItemWidth = Math.Floor(d);
            }
        }

        private static void OnOneRowModeEnabledChanged(DependencyObject d, object newValue)
        {
            (d as AdaptiveGridView).DetermineOneRowMode();
        }

        private static void DesiredWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            AdaptiveGridView obj = d as AdaptiveGridView;
            obj.RecalculateLayout(obj.ActualWidth);
        }

        private static void OnStretchContentForSingleRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            AdaptiveGridView obj = d as AdaptiveGridView;
            obj.RecalculateLayout(obj.ActualWidth);
        }

        private int CalculateColumns(double containerWidth, double itemWidth)
        {
            int num = (int)Math.Round(containerWidth / itemWidth);
            if (num is 0)
            {
                num = 1;
            }

            return num;
        }
    }
}
