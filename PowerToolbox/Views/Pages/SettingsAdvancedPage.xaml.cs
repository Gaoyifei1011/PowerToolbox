using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
    public sealed partial class SettingsAdvancedPage : Page, INotifyPropertyChanged
    {
        private bool _isRestarting = false;

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

        private bool _fileShellMenuValue = FileShellMenuService.FileShellMenuValue;

        public bool FileShellMenuValue
        {
            get { return _fileShellMenuValue; }

            set
            {
                if (!Equals(_fileShellMenuValue, value))
                {
                    _fileShellMenuValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileShellMenuValue)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsAdvancedPage()
        {
            InitializeComponent();
        }

        #region 设置高级选项页面——挂载的事件

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            IsRestarting = true;

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAdvancedPage), nameof(OnRestartExplorerClicked), 1, e);
                }
            });

            IsRestarting = false;
        }

        /// <summary>
        /// 是否开启显示文件右键菜单
        /// </summary>
        private void OnFileShellMenuToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FileShellMenuService.SetFileShellMenuValue(toggleSwitch.IsOn);
                FileShellMenuValue = toggleSwitch.IsOn;
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

        #endregion 设置高级选项页面——挂载的事件
    }
}
