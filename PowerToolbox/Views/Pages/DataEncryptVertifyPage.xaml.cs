using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据加密校验页面
    /// </summary>
    public sealed partial class DataEncryptVertifyPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;

        private Microsoft.UI.Xaml.Controls.NavigationViewItem _selectedItem;

        public Microsoft.UI.Xaml.Controls.NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            new KeyValuePair<string, Type>("DataEncrypt",typeof(DataEncryptPage)),
            new KeyValuePair<string, Type>("DataVertify", typeof(DataVertifyPage)),
        ];

        private List<NavigationModel> NavigationItemList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataEncryptVertifyPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面时发生的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DataEncryptVertifyFrame.ContentTransitions = SuppressNavigationTransitionCollection;
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：数据加密校验页面——挂载的事件

        /// <summary>
        /// 导航控件加载完成后初始化内容
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                if (sender is Microsoft.UI.Xaml.Controls.NavigationView navigationView)
                {
                    foreach (object menuItem in navigationView.MenuItems)
                    {
                        if (menuItem is Microsoft.UI.Xaml.Controls.NavigationViewItem navigationViewItem && navigationViewItem.Tag is string tag)
                        {
                            int tagIndex = PageList.FindIndex(item => string.Equals(item.Key, tag));

                            NavigationItemList.Add(new NavigationModel()
                            {
                                NavigationTag = PageList[tagIndex].Key,
                                NavigationItem = navigationViewItem,
                                NavigationPage = PageList[tagIndex].Value,
                            });
                        }
                    }
                }

                SelectedItem = NavigationItemList[0].NavigationItem;
                NavigateTo(PageList[0].Value);
            }
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItemBase navigationViewItem && navigationViewItem.Tag is string tag)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => string.Equals(item.NavigationTag, tag, StringComparison.OrdinalIgnoreCase));

                if (navigationItem.NavigationPage is not null && !Equals(SelectedItem, navigationItem.NavigationItem))
                {
                    int selectedIndex = sender.MenuItems.IndexOf(SelectedItem);
                    int invokedIndex = sender.MenuItems.IndexOf(navigationItem.NavigationItem);
                    NavigateTo(navigationItem.NavigationPage, null, invokedIndex > selectedIndex);
                }
            }
        }

        /// <summary>
        /// 导航完成后发生的事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            try
            {
                Type currentPageType = GetCurrentPageType();
                foreach (NavigationModel navigationItem in NavigationItemList)
                {
                    if (navigationItem.NavigationPage is not null && Equals(navigationItem.NavigationPage, currentPageType))
                    {
                        SelectedItem = navigationItem.NavigationItem;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DataEncryptVertifyPage), nameof(OnNavigated), 1, e);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;

            try
            {
                Type currentPageType = GetCurrentPageType();
                foreach (NavigationModel navigationItem in NavigationItemList)
                {
                    if (navigationItem.NavigationPage is not null && Equals(navigationItem.NavigationPage, currentPageType))
                    {
                        SelectedItem = navigationItem.NavigationItem;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DataEncryptVertifyPage), nameof(OnNavigationFailed), 1, e);
            }
        }

        #endregion 第二部分：数据加密校验页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (NavigationItemList.Find(item => Equals(item.NavigationPage, navigationPageType)) is NavigationModel navigationItem)
                {
                    if (slideDirection.HasValue)
                    {
                        DataEncryptVertifyFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                    }

                    // 导航到该项目对应的页面
                    DataEncryptVertifyFrame.Navigate(navigationItem.NavigationPage, parameter);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileManagerPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return DataEncryptVertifyFrame.CurrentSourcePageType;
        }
    }
}
