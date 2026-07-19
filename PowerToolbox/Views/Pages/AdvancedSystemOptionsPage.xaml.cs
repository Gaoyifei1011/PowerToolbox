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
    public sealed partial class AdvancedSystemOptionsPage : Page, INotifyPropertyChanged
    {
        private readonly string AdvancedSystemOptionsString = ResourceService.AdvancedSystemOptionsResource.GetString("AdvancedSystemOptions");
        private readonly string PersonalizationString = ResourceService.AdvancedSystemOptionsResource.GetString("Personalization");
        private readonly string RestartExplorerString = ResourceService.AdvancedSystemOptionsResource.GetString("RestartExplorer");
        private readonly string RestartingString = ResourceService.AdvancedSystemOptionsResource.GetString("Restarting");
        private readonly string SystemString = ResourceService.AdvancedSystemOptionsResource.GetString("System");

        private bool _isAdvancedSettingsInfoWarning;

        public bool IsAdvancedSettingsInfoWarning
        {
            get { return _isAdvancedSettingsInfoWarning; }

            set
            {
                if (!Equals(_isAdvancedSettingsInfoWarning, value))
                {
                    _isAdvancedSettingsInfoWarning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdvancedSettingsInfoWarning)));
                }
            }
        }

        private bool _isRestartExplorerVisible;

        public bool IsRestartExplorerVisible
        {
            get { return _isRestartExplorerVisible; }

            set
            {
                if (!Equals(_isRestartExplorerVisible, value))
                {
                    _isRestartExplorerVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestartExplorerVisible)));
                }
            }
        }

        private bool _isRestartPCVisible;

        public bool IsRestartPCVisible
        {
            get { return _isRestartPCVisible; }

            set
            {
                if (!Equals(_isRestartPCVisible, value))
                {
                    _isRestartPCVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestartPCVisible)));
                }
            }
        }

        private bool _isRestarting;

        public bool IsRestarting
        {
            get { return _isRestarting; }

            set
            {
                if (!Equals(_isRestarting, value))
                {
                    _isRestarting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestarting)));
                }
            }
        }

        private string _restartExplorerText;

        public string RestartExplorerText
        {
            get { return _restartExplorerText; }

            set
            {
                if (!Equals(_restartExplorerText, value))
                {
                    _restartExplorerText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RestartExplorerText)));
                }
            }
        }

        public List<Type> PageList { get; } = [typeof(AdvancedSystemOptionsListPage), typeof(AdvancedSystemOptionsPersonalizationPage), typeof(AdvancedSystemOptionsSystemPage)];

        public WinRTObservableCollection<DictionaryEntry> BreadCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPage()
        {
            InitializeComponent();
            RestartExplorerText = RestartExplorerString;
        }

        #region 第一部分：重写父类事件

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

        #endregion 第一部分：重写父类事件

        #region 第一部分：高级系统选项页面——挂载的事件

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
                BreadCollection.Add(new DictionaryEntry
                {
                    Key = "AdvancedSystemOptions",
                    Value = AdvancedSystemOptionsString
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                BreadCollection.Add(new DictionaryEntry()
                {
                    Key = "Personalization",
                    Value = PersonalizationString
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[2]))
            {
                BreadCollection.Add(new DictionaryEntry()
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRestartExplorerClicked), 1, e);
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRestartExplorerClicked), 2, e);
                    }
                }
            });
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

        #endregion 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
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
        public Type GetCurrentPageType()
        {
            return AdvancedSystemOptionsFrame.CurrentSourcePageType;
        }
    }
}
