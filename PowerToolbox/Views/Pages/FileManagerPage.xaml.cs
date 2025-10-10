using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            new KeyValuePair<string, Type>("FileName",typeof(FileNamePage)),
            new KeyValuePair<string, Type>("ExtensionName", typeof(ExtensionNamePage)),
            new KeyValuePair<string, Type>("UpperAndLowerCase", typeof(UpperAndLowerCasePage)),
            new KeyValuePair<string, Type>("FileProperties", typeof(FilePropertiesPage)),
        ];

        private List<OldAndNewNameModel> NameChangeList { get; } =
        [
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        ];

        private List<string> NameChangeRuleList { get; } = [];

        private Dictionary<int, List<OldAndNewNameModel>> NameChangeDict { get; } = [];

        private List<NavigationModel> NavigationItemList { get; } = [];

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
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：文件管理页面——挂载的事件

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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileManagerPage), nameof(OnNavigated), 1, e);
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileManagerPage), nameof(OnNavigationFailed), 1, e);
            }
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
                if (NavigationItemList.Find(item => Equals(item.NavigationPage, navigationPageType)) is NavigationModel navigationItem)
                {
                    if (slideDirection.HasValue)
                    {
                        FileManagerFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                    }

                    // 导航到该项目对应的页面
                    FileManagerFrame.Navigate(navigationItem.NavigationPage, parameter);
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
