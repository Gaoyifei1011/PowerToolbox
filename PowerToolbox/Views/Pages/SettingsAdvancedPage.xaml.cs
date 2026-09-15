using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 设置高级选项页面
    /// </summary>
    internal sealed partial class SettingsAdvancedPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：属性、列表与事件

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

        private bool _fileShellMenu;

        private bool FileShellMenu
        {
            get { return _fileShellMenu; }

            set
            {
                if (!Equals(_fileShellMenu, value))
                {
                    _fileShellMenu = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileShellMenu)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、列表与事件

        #region 第二部分：构造函数

        internal SettingsAdvancedPage()
        {
            InitializeComponent();
        }

        #endregion 第二部分：构造函数

        #region 第三部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面后触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            FileShellMenu = FileShellMenuService.FileShellMenu;
        }

        #endregion 第三部分：父类虚方法重写

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            IsRestarting = true;
            await RestartExplorerAsync();
            IsRestarting = false;
        }

        /// <summary>
        /// 是否开启显示文件右键菜单
        /// </summary>
        private void OnFileShellMenuToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(FileShellMenu, toggleSwitch.IsOn))
            {
                FileShellMenu = toggleSwitch.IsOn;
                FileShellMenuService.SetFileShellMenu(toggleSwitch.IsOn);
                FileShellMenu = FileShellMenuService.FileShellMenu;
            }
        }

        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        private void OnOpenLogFolderClicked(object sender, RoutedEventArgs args)
        {
            LogService.OpenLogFolder();
        }

        /// <summary>
        /// 清除所有日志记录
        /// </summary>
        private async void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = await LogService.ClearLogAsync();
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.LogClean, result));
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 重启资源管理器
        /// </summary>
        private async Task RestartExplorerAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    int dwRmStatus = RstrtmgrLibrary.RmStartSession(out uint dwSessionHandle, 0, Guid.Empty.ToString());

                    if (dwRmStatus is 0)
                    {
                        Process[] processList = Process.GetProcessesByName("explorer");
                        RM_UNIQUE_PROCESS[] lpRmProcList = new RM_UNIQUE_PROCESS[processList.Length];

                        for (int index = 0; index < processList.Length; index++)
                        {
                            lpRmProcList[index].dwProcessId = processList[index].Id;
                            FILETIME fileTime = new();
                            long time = processList[index].StartTime.ToFileTime();
                            fileTime.dwLowDateTime = (int)(time & 0xFFFFFFFF);
                            fileTime.dwHighDateTime = (int)(time >> 32);
                            lpRmProcList[index].ProcessStartTime = fileTime;
                        }

                        dwRmStatus = RstrtmgrLibrary.RmRegisterResources(dwSessionHandle, 0, null, (uint)processList.Length, lpRmProcList, 0, null);

                        if (dwRmStatus is 0)
                        {
                            dwRmStatus = RstrtmgrLibrary.RmShutdown(dwSessionHandle, RM_SHUTDOWN_TYPE.RmForceShutdown, null);

                            if (dwRmStatus is 0)
                            {
                                dwRmStatus = RstrtmgrLibrary.RmRestart(dwSessionHandle, 0, null);

                                if (dwRmStatus is 0)
                                {
                                    dwRmStatus = RstrtmgrLibrary.RmEndSession(dwSessionHandle);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAdvancedPage), nameof(RestartExplorerAsync), 1, e);
                }
            });
        }

        #endregion 第五部分：挂载事件处理
    }
}
