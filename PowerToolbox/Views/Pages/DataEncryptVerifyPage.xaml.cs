using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据加密校验页面
    /// </summary>
    public sealed partial class DataEncryptVerifyPage : Page, INotifyPropertyChanged
    {
        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
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

        private List<Type> PageList { get; } = [typeof(DataEncryptPage), typeof(DataVerifyPage)];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataEncryptVerifyPage()
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
            DataEncryptVerifyFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                SelectedItem = DataEncryptVerifySelctorBar.Items[0];
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：数据加密校验页面——挂载的事件

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectedItem = sender.SelectedItem;
            int index = sender.Items.IndexOf(SelectedItem);
            Type currentPage = GetCurrentPageType();
            int currentIndex = PageList.FindIndex(item => Equals(item, currentPage));

            if (index is 0)
            {
                if (currentPage is null)
                {
                    NavigateTo(PageList[0]);
                }
                else if (!Equals(currentPage, PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
                }
            }
            else if (index is 1 && !Equals(currentPage, PageList[1]))
            {
                NavigateTo(PageList[1], null, index > currentIndex);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < DataEncryptVerifySelctorBar.Items.Count)
            {
                SelectedItem = DataEncryptVerifySelctorBar.Items[index];
            }

            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptVerifyPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        #endregion 第二部分：数据加密校验页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    DataEncryptVerifyFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                DataEncryptVerifyFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptVerifyPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return DataEncryptVerifyFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return DataEncryptVerifyFrame.Content;
        }
    }
}
