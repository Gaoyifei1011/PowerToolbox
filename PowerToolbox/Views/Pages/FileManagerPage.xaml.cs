using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件管理页面
    /// </summary>
    internal sealed partial class FileManagerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ChangeRuleString = ResourceService.FileManagerResource.GetString("ChangeRule");
        private readonly string ExtensionNameString = ResourceService.FileManagerResource.GetString("ExtensionName");
        private readonly string FileNameString = ResourceService.FileManagerResource.GetString("FileName");
        private readonly string FilePropertiesString = ResourceService.FileManagerResource.GetString("FileProperties");
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
        private readonly string UpperAndLowerCaseString = ResourceService.FileManagerResource.GetString("UpperAndLowerCase");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private int SelectedIndex
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedIndex)));
                }
            }
        }

        private int SelectedPageIndex
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedPageIndex)));
                }
            }
        }

        private StringCollection FileManagerSelectorRadioCollection { get; } = [];

        private ReadOnlyCollection<Type> PageCollection { get; } = new([typeof(FileNamePage), typeof(ExtensionNamePage), typeof(UpperAndLowerCasePage), typeof(FilePropertiesPage)]);

        private ReadOnlyCollection<OldAndNewNameModel> NameChangeCollection { get; } = new(
        [
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
            new(){ OriginalFileName = string.Empty, NewFileName = string.Empty },
        ]);

        private StringCollection NameChangeRuleCollection { get; } = [];

        private Dictionary<int, List<OldAndNewNameModel>> NameChangeDict { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal FileManagerPage()
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
            FileManagerFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageCollection[0], this);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 点击选择器栏选中项发生变化后发生的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && !Equals(SelectedIndex, radioButtons.SelectedIndex))
            {
                SelectedIndex = radioButtons.SelectedIndex;
            }

            if (SelectedIndex >= 0 && SelectedIndex < FileManagerSelectorRadioCollection.Count)
            {
                int index = SelectedIndex;
                Type currentPage = GetCurrentPageType();
                int currentIndex = PageCollection.IndexOf(PageCollection.FirstOrDefault(item => Equals(item, currentPage)));

                if (index is 0)
                {
                    if (currentPage is null)
                    {
                        NavigateTo(PageCollection[0]);
                    }
                    else if (!Equals(currentPage, PageCollection[0]))
                    {
                        NavigateTo(PageCollection[0], null, index > currentIndex);
                    }
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), PageCollection[1]))
                {
                    NavigateTo(PageCollection[1], null, index > currentIndex);
                }
                else if (index is 2 && !Equals(GetCurrentPageType(), PageCollection[2]))
                {
                    NavigateTo(PageCollection[2], null, index > currentIndex);
                }
                else if (index is 3 && !Equals(GetCurrentPageType(), PageCollection[3]))
                {
                    NavigateTo(PageCollection[3], null, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 导航失败后发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageCollection.IndexOf(PageCollection.FirstOrDefault(item => Equals(item, GetCurrentPageType())));

            if (index >= 0 && index < FileManagerSelectorRadioCollection.Count)
            {
                SelectedIndex = index;
            }

            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileManagerPage), nameof(OnNavigationFailed), 1, args.Exception);
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
        /// 改名示例导航器选中项发生变化后触发的事件
        /// </summary>
        private void OnSelecteedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
        {
            SelectedPageIndex = sender.SelectedPageIndex;

            for (int index = 0; index < NameChangeCollection.Count; index++)
            {
                NameChangeCollection[index].OriginalFileName = NameChangeDict[SelectedPageIndex][index].OriginalFileName;
                NameChangeCollection[index].NewFileName = NameChangeDict[SelectedPageIndex][index].NewFileName;
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            FileManagerSelectorRadioCollection.Add(FileNameString);
            FileManagerSelectorRadioCollection.Add(ExtensionNameString);
            FileManagerSelectorRadioCollection.Add(UpperAndLowerCaseString);
            FileManagerSelectorRadioCollection.Add(FilePropertiesString);

            NameChangeRuleCollection.Add(NameChangeRule1String);
            NameChangeRuleCollection.Add(NameChangeRule2String);
            NameChangeRuleCollection.Add(NameChangeRule3String);
            NameChangeRuleCollection.Add(NameChangeRule4String);

            NameChangeDict.Add(0,
            [
                new() { OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList1ChangedName1String },
                new() { OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList1ChangedName2String },
                new() { OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList1ChangedName3String },
                new() { OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList1ChangedName4String },
            ]);
            NameChangeDict.Add(1,
            [
                new(){ OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList2ChangedName1String },
                new() { OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList2ChangedName2String },
                new() { OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList2ChangedName3String },
                new() { OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList2ChangedName4String },
            ]);
            NameChangeDict.Add(2,
            [
                new() { OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList3ChangedName1String },
                new() { OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList3ChangedName2String },
                new() { OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList3ChangedName3String },
                new() { OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList3ChangedName4String },
            ]);
            NameChangeDict.Add(3,
            [
                new() { OriginalFileName = NameChangeOriginalName1String, NewFileName = NameChangeList4ChangedName1String },
                new() { OriginalFileName = NameChangeOriginalName2String, NewFileName = NameChangeList4ChangedName2String },
                new() { OriginalFileName = NameChangeOriginalName3String, NewFileName = NameChangeList4ChangedName3String },
                new() { OriginalFileName = NameChangeOriginalName4String, NewFileName = NameChangeList4ChangedName4String },
            ]);

            SelectedPageIndex = 0;

            for (int index = 0; index < NameChangeCollection.Count; index++)
            {
                NameChangeCollection[index].OriginalFileName = NameChangeDict[SelectedPageIndex][index].OriginalFileName;
                NameChangeCollection[index].NewFileName = NameChangeDict[SelectedPageIndex][index].NewFileName;
            }
        }

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                FileManagerFrame.ContentTransitions = slideDirection.HasValue ? slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection : SuppressNavigationTransitionCollection;

                // 导航到该项目对应的页面
                FileManagerFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileManagerPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        private Type GetCurrentPageType()
        {
            return FileManagerFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        internal object GetFrameContent()
        {
            return FileManagerFrame.Content;
        }

        /// <summary>
        /// 显示使用说明
        /// </summary>
        internal void ShowUseInstruction()
        {
            if (!FileManagerSplitView.IsPaneOpen)
            {
                FileManagerSplitView.IsPaneOpen = true;
            }
        }

        private string GetChangeRule(int index)
        {
            return string.Format(ChangeRuleString, NameChangeRuleCollection[index]);
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
