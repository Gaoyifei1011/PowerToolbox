using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Models;
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
    /// 数据校验与加密页面
    /// </summary>
    internal sealed partial class DataVerifyEncryptPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DataDecryptString = ResourceService.DataVerifyEncryptResource.GetString("DataDecrypt");
        private readonly string DataEncryptString = ResourceService.DataVerifyEncryptResource.GetString("DataEncrypt");
        private readonly string DataVerifyString = ResourceService.DataVerifyEncryptResource.GetString("DataVerify");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private int _selectedIndex;

        private int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedIndex)));
                }
            }
        }

        private List<RadioButtonItemModel> DataVerifyEncryptSelectorRadioList { get; } = [];

        private List<Type> PageList { get; } = [typeof(DataVerifyPage), typeof(DataEncryptPage), typeof(DataDecryptPage)];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal DataVerifyEncryptPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面时发生的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DataVerifyEncryptFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], this);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && !Equals(SelectedIndex, radioButtons.SelectedIndex))
            {
                SelectedIndex = radioButtons.SelectedIndex;
            }

            if (SelectedIndex >= 0 && SelectedIndex < DataVerifyEncryptSelectorRadioList.Count)
            {
                int index = SelectedIndex;
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
                else if (index is 2 && !Equals(currentPage, PageList[2]))
                {
                    NavigateTo(PageList[2], null, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 导航失败后发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < DataVerifyEncryptSelectorRadioList.Count)
            {
                SelectedIndex = index;
            }

            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyEncryptPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            DataVerifyEncryptSelectorRadioList.Add(new() { SelectorName = DataVerifyString });
            DataVerifyEncryptSelectorRadioList.Add(new() { SelectorName = DataEncryptString });
            DataVerifyEncryptSelectorRadioList.Add(new() { SelectorName = DataDecryptString });
        }

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                DataVerifyEncryptFrame.ContentTransitions = slideDirection.HasValue ? slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection : SuppressNavigationTransitionCollection;

                // 导航到该项目对应的页面
                DataVerifyEncryptFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyEncryptPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        internal Type GetCurrentPageType()
        {
            return DataVerifyEncryptFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        internal object GetFrameContent()
        {
            return DataVerifyEncryptFrame.Content;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
