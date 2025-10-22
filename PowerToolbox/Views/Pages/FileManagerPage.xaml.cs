using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件管理页面
    /// </summary>
    public sealed partial class FileManagerPage : Page, INotifyPropertyChanged
    {
        private readonly string ChangeRuleString = ResourceService.FileManagerResource.GetString("ChangeRule");
        private readonly string NameChangeRule1String = ResourceService.FileManagerResource.GetString("NameChangeRule1");
        private readonly string NameChangeRule2String = ResourceService.FileManagerResource.GetString("NameChangeRule2");
        private readonly string NameChangeRule3String = ResourceService.FileManagerResource.GetString("NameChangeRule3");
        private readonly string NameChangeRule4String = ResourceService.FileManagerResource.GetString("NameChangeRule4");
        private readonly string NameChangeOriginalName1String = ResourceService.FileManagerResource.GetString("NameChangeOriginalName1");
        private readonly string NameChangeOriginalName2String = ResourceService.FileManagerResource.GetString("NameChangeOriginalName2");
        private readonly string NameChangeOriginalName3String = ResourceService.FileManagerResource.GetString("NameChangeOriginalName3");
        private readonly string NameChangeOriginalName4String = ResourceService.FileManagerResource.GetString("NameChangeOriginalName4");
        private readonly string NameChangeList1ChangedName1String = ResourceService.FileManagerResource.GetString("NameChangeList1ChangedName1");
        private readonly string NameChangeList1ChangedName2String = ResourceService.FileManagerResource.GetString("NameChangeList1ChangedName2");
        private readonly string NameChangeList1ChangedName3String = ResourceService.FileManagerResource.GetString("NameChangeList1ChangedName3");
        private readonly string NameChangeList1ChangedName4String = ResourceService.FileManagerResource.GetString("NameChangeList1ChangedName4");
        private readonly string NameChangeList2ChangedName1String = ResourceService.FileManagerResource.GetString("NameChangeList2ChangedName1");
        private readonly string NameChangeList2ChangedName2String = ResourceService.FileManagerResource.GetString("NameChangeList2ChangedName2");
        private readonly string NameChangeList2ChangedName3String = ResourceService.FileManagerResource.GetString("NameChangeList2ChangedName3");
        private readonly string NameChangeList2ChangedName4String = ResourceService.FileManagerResource.GetString("NameChangeList2ChangedName4");
        private readonly string NameChangeList3ChangedName1String = ResourceService.FileManagerResource.GetString("NameChangeList3ChangedName1");
        private readonly string NameChangeList3ChangedName2String = ResourceService.FileManagerResource.GetString("NameChangeList3ChangedName2");
        private readonly string NameChangeList3ChangedName3String = ResourceService.FileManagerResource.GetString("NameChangeList3ChangedName3");
        private readonly string NameChangeList3ChangedName4String = ResourceService.FileManagerResource.GetString("NameChangeList3ChangedName4");
        private readonly string NameChangeList4ChangedName1String = ResourceService.FileManagerResource.GetString("NameChangeList4ChangedName1");
        private readonly string NameChangeList4ChangedName2String = ResourceService.FileManagerResource.GetString("NameChangeList4ChangedName2");
        private readonly string NameChangeList4ChangedName3String = ResourceService.FileManagerResource.GetString("NameChangeList4ChangedName3");
        private readonly string NameChangeList4ChangedName4String = ResourceService.FileManagerResource.GetString("NameChangeList4ChangedName4");

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

        private int _currentIndex = 0;

        public int CurrentIndex
        {
            get { return _currentIndex; }

            set
            {
                if (!Equals(_currentIndex, value))
                {
                    _currentIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));
                }
            }
        }

        private List<Type> PageList { get; } = [typeof(FileNamePage), typeof(ExtensionNamePage), typeof(UpperAndLowerCasePage), typeof(FilePropertiesPage)];

        private List<OldAndNewNameModel> NameChangeList { get; } =
        [
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        ];

        private List<string> NameChangeRuleList { get; } = [];

        private Dictionary<int, List<OldAndNewNameModel>> NameChangeDict { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileManagerPage()
        {
            InitializeComponent();
            NameChangeRuleList.Add(NameChangeRule1String);
            NameChangeRuleList.Add(NameChangeRule2String);
            NameChangeRuleList.Add(NameChangeRule3String);
            NameChangeRuleList.Add(NameChangeRule4String);

            NameChangeDict.Add(0,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList1ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList1ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList1ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList1ChangedName4String },
            ]);
            NameChangeDict.Add(1,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList2ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList2ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList2ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList2ChangedName4String },
            ]);
            NameChangeDict.Add(2,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList3ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList3ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList3ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList3ChangedName4String },
            ]);
            NameChangeDict.Add(3,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList4ChangedName1String },
                new(){ OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList4ChangedName2String },
                new(){ OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList4ChangedName3String },
                new(){ OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList4ChangedName4String },
            ]);

            CurrentIndex = 0;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面时发生的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            FileManagerFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0]);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：文件管理页面——挂载的事件

        /// <summary>
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is Type pageType)
            {
                int index = PageList.IndexOf(pageType);
                int currentIndex = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

                if (index is 0 && !Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), PageList[1]))
                {
                    NavigateTo(PageList[1], null, index > currentIndex);
                }
                else if (index is 2 && !Equals(GetCurrentPageType(), PageList[2]))
                {
                    NavigateTo(PageList[2], null, index > currentIndex);
                }
                else if (index is 3 && !Equals(GetCurrentPageType(), PageList[3]))
                {
                    NavigateTo(PageList[3], null, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 导航完成后发生的事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < FileManagerSelctorBar.Items.Count)
            {
                SelectedItem = FileManagerSelctorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < FileManagerSelctorBar.Items.Count)
            {
                SelectedItem = FileManagerSelctorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }

            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileManagerPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        /// <summary>
        /// 关闭改名示例提示
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (FileManagerSplitView.IsPaneOpen)
            {
                FileManagerSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 向前导航
        /// </summary>
        private void OnForwardNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex is 0 ? 3 : CurrentIndex - 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 向后导航
        /// </summary>
        private void OnNextNavigateClicked(object sender, RoutedEventArgs args)
        {
            CurrentIndex = CurrentIndex is 3 ? 0 : CurrentIndex + 1;

            for (int index = 0; index < NameChangeList.Count; index++)
            {
                NameChangeList[index].OriginalFileName = NameChangeDict[CurrentIndex][index].OriginalFileName;
                NameChangeList[index].NewFileName = NameChangeDict[CurrentIndex][index].NewFileName;
            }
        }

        #endregion 第二部分：文件管理页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    FileManagerFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                FileManagerFrame.Navigate(navigationPageType, parameter);
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
            return FileManagerFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return FileManagerFrame.Content;
        }

        /// <summary>
        /// 显示使用说明
        /// </summary>
        public void ShowUseInstruction()
        {
            if (!FileManagerSplitView.IsPaneOpen)
            {
                FileManagerSplitView.IsPaneOpen = true;
            }
        }

        private string GetChangeRule(int index)
        {
            return string.Format(ChangeRuleString, NameChangeRuleList[index]);
        }
    }
}
