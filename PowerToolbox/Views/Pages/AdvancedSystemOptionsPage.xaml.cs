using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Services.Root;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项页面
    /// </summary>
    internal sealed partial class AdvancedSystemOptionsPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AdvancedSystemOptionsString = ResourceService.AdvancedSystemOptionsResource.GetString("AdvancedSystemOptions");
        private readonly string PersonalizationString = ResourceService.AdvancedSystemOptionsResource.GetString("Personalization");
        private readonly string RestartExplorerString = ResourceService.AdvancedSystemOptionsResource.GetString("RestartExplorer");
        private readonly string RestartingString = ResourceService.AdvancedSystemOptionsResource.GetString("Restarting");
        private readonly string SystemString = ResourceService.AdvancedSystemOptionsResource.GetString("System");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isAdvancedSettingsInfoWarning;

        internal bool IsAdvancedSettingsInfoWarning
        {
            get { return _isAdvancedSettingsInfoWarning; }

            set
            {
                if (!Equals(_isAdvancedSettingsInfoWarning, value))
                {
                    _isAdvancedSettingsInfoWarning = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsAdvancedSettingsInfoWarning)));
                }
            }
        }

        private bool _isRestartExplorerVisible;

        internal bool IsRestartExplorerVisible
        {
            get { return _isRestartExplorerVisible; }

            set
            {
                if (!Equals(_isRestartExplorerVisible, value))
                {
                    _isRestartExplorerVisible = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRestartExplorerVisible)));
                }
            }
        }

        private bool _isRestartPCVisible;

        internal bool IsRestartPCVisible
        {
            get { return _isRestartPCVisible; }

            set
            {
                if (!Equals(_isRestartPCVisible, value))
                {
                    _isRestartPCVisible = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRestartPCVisible)));
                }
            }
        }

        private bool _isRestarting;

        private bool IsRestarting
        {
            get { return _isRestarting; }

            set
            {
                if (!Equals(_isRestarting, value))
                {
                    _isRestarting = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRestarting)));
                }
            }
        }

        private string _restartExplorerText;

        private string RestartExplorerText
        {
            get { return _restartExplorerText; }

            set
            {
                if (!Equals(_restartExplorerText, value))
                {
                    _restartExplorerText = value;
                    PropertyChanged?.Invoke(this, new(nameof(RestartExplorerText)));
                }
            }
        }

        internal List<Type> PageList { get; } = [typeof(AdvancedSystemOptionsListPage), typeof(AdvancedSystemOptionsPersonalizationPage), typeof(AdvancedSystemOptionsSystemPage)];

        internal WinRTObservableCollection<DictionaryEntry> BreadCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal AdvancedSystemOptionsPage()
        {
            InitializeComponent();
            RestartExplorerText = RestartExplorerString;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            AdvancedSystemOptionsFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], this, null);
            }
        }

        #endregion 第四部分：命令调用处理

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is DictionaryEntry bread && BreadCollection.Count is 2 && Equals(bread.Key, BreadCollection[0].Key))
            {
                NavigateTo(PageList[0], this, false);
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (BreadCollection.Count is 0 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.Add(new()
                {
                    Key = "AdvancedSystemOptions",
                    Value = AdvancedSystemOptionsString
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                BreadCollection.Add(new()
                {
                    Key = "Personalization",
                    Value = PersonalizationString
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[2]))
            {
                BreadCollection.Add(new()
                {
                    Key = "System",
                    Value = SystemString
                });
            }
            else if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.RemoveAt(1);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
        }

        /// <summary>
        /// 重启资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            RestartExplorerText = RestartingString;
            IsRestarting = true;
            await RestartExplorerAsync();
            IsRestarting = false;
            RestartExplorerText = RestartExplorerString;
            IsRestartExplorerVisible = false;
            if (!IsRestartExplorerVisible && !IsRestartPCVisible)
            {
                IsAdvancedSettingsInfoWarning = false;
            }
        }

        /// <summary>
        /// 重启电脑
        /// </summary>
        private void OnRestartPCClicked(object sender, RoutedEventArgs args)
        {
            RestartPC();
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 页面向前导航
        /// </summary>
        internal void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                AdvancedSystemOptionsFrame.ContentTransitions = slideDirection.HasValue ? slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection : SuppressNavigationTransitionCollection;

                // 导航到该项目对应的页面
                AdvancedSystemOptionsFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        internal Type GetCurrentPageType()
        {
            return AdvancedSystemOptionsFrame.CurrentSourcePageType;
        }

        #endregion 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 重启资源管理器
        /// </summary>
        private async Task RestartExplorerAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    Process taskKillProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = "/IM explorer.exe /F",
                        Verb = "open",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    });
                    taskKillProcess.WaitForExit();
                    taskKillProcess.Dispose();
                    while (Process.GetProcessesByName("explorer").FirstOrDefault() is not null)
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Win32Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(RestartExplorerAsync), 1, e);
                }
                finally
                {
                    try
                    {
                        Process explorerProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Verb = "open",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        });
                        explorerProcess.Dispose();
                    }
                    catch (Win32Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(RestartExplorerAsync), 2, e);
                    }
                }
            });
        }

        /// <summary>
        /// 重启电脑
        /// </summary>
        private void RestartPC()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("shutdown.exe", "-r -f -t 0");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRestartPCClicked), 1, e);
                }
            });
        }
    }
}
