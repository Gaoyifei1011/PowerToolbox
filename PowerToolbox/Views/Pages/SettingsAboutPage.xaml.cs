using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Windows.Services.Store;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class SettingsAboutPage : Page, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecking)));
                }
            }
        }

        //项目引用信息
        private ListDictionary ReferenceList { get; } = new()
        {
            { "Microsoft.Windows.SDK.BuildTools", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.Windows.SDK.BuildTools.MSIX", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.Windows.SDK.NET.Ref", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.WindowsAppSDK", new Uri("https://github.com/microsoft/windowsappsdk") },
            { "Mile.Aria2", new Uri("https://github.com/ProjectMile/Mile.Aria2") },
            { "System.Runtime.WindowsRuntime", new Uri("https://github.com/dotnet/corefx") }
        };

        //项目感谢者信息
        private ListDictionary ThanksList { get; } = new()
        {
            { "AndromedaMelody", new Uri("https://github.com/AndromedaMelody") },
            { "Blinue", new Uri("https://github.com/Blinue") },
            { "cnbluefire", new Uri("https://github.com/cnbluefire") },
            { "MouriNaruto" , new Uri("https://github.com/MouriNaruto") },
            { "Osirisoo0O" , new Uri("https://github.com/Osirisoo0O") }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsAboutPage()
        {
            InitializeComponent();
        }

        #region 第一部分：关于页面——挂载的事件

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://apps.microsoft.com/detail/9MV67V21H386");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnShowReleaseNotesClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new AppInformationDialog());
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:about");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnSystemInformationClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new LicenseDialog());
        }

        /// <summary>
        /// 帮助翻译应用
        /// </summary>
        private void OnHelpTranslateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/PowerToolbox/issues");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnHelpTranslateClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        private void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/PowerToolbox");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnProjectDescriptionClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/PowerToolbox/issues");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnSendFeedbackClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            if (!IsChecking)
            {
                IsChecking = true;

                if (IsNetworkConnected())
                {
                    bool isNewest = false;

                    try
                    {
                        StoreContext storeContext = StoreContext.GetDefault();
                        IReadOnlyList<StorePackageUpdate> packageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
                        isNewest = packageUpdateList.Count is 0;
                        IsChecking = false;
                        synchronizationContext.Post(async (_) =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, Convert.ToInt32(isNewest)));
                        }, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnCheckUpdateClicked), 1, e);
                        IsChecking = false;
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                    }

                    if (!isNewest)
                    {
                        await MainWindow.Current.ShowDialogAsync(new UpdateAppDialog());
                    }
                }
                else
                {
                    IsChecking = false;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                }
            }
        }

        #endregion 第一部分：关于页面——挂载的事件

        /// <summary>
        /// 检测网络是否已经连接
        /// </summary>
        private static bool IsNetworkConnected()
        {
            try
            {
                using Ping ping = new();
                PingReply pingReply = ping.Send("8.8.8.8", 1000);
                return pingReply.Status is IPStatus.Success;
            }
            catch (PingException e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(IsNetworkConnected), 1, e);
                return false;
            }
        }
    }
}
