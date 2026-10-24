using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
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
    internal sealed partial class SettingsAboutPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AppInformationString = ResourceService.SettingsAboutResource.GetString("AppInformation");
        private readonly string HelpTranslateString = ResourceService.SettingsAboutResource.GetString("HelpTranslate");
        private readonly string ProjectHomePageString = ResourceService.SettingsAboutResource.GetString("ProjectHomePage");
        private readonly string SendFeedbackString = ResourceService.SettingsAboutResource.GetString("SendFeedback");
        private readonly string ShowLicenseString = ResourceService.SettingsAboutResource.GetString("ShowLicense");
        private readonly string ShowReleaseNotesString = ResourceService.SettingsAboutResource.GetString("ShowReleaseNotes");
        private readonly string SystemInformationString = ResourceService.SettingsAboutResource.GetString("SystemInformation");

        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private List<OperationBarItemModel> OperationBarItemList { get; } = [];

        // 项目引用信息
        private ListDictionary ReferenceList { get; } = new()
        {
            { "Microsoft.Windows.SDK.BuildTools", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.Windows.SDK.BuildTools.MSIX", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.Windows.SDK.NET.Ref", new Uri("https://aka.ms/WinSDKProjectURL") },
            { "Microsoft.WindowsAppSDK", new Uri("https://github.com/microsoft/windowsappsdk") },
            { "Mile.Aria2", new Uri("https://github.com/ProjectMile/Mile.Aria2") },
            { "System.Numerics.Vectors", new Uri("https://github.com/dotnet/maintenance-packages") },
            { "System.Runtime.WindowsRuntime", new Uri("https://github.com/dotnet/corefx") }
        };

        // 项目感谢者信息
        private ListDictionary ThanksList { get; } = new()
        {
            { "AndromedaMelody", new Uri("https://github.com/AndromedaMelody") },
            { "Blinue", new Uri("https://github.com/Blinue") },
            { "cnbluefire", new Uri("https://github.com/cnbluefire") },
            { "MouriNaruto" , new Uri("https://github.com/MouriNaruto") },
            { "Osirisoo0O" , new Uri("https://github.com/Osirisoo0O") }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal SettingsAboutPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 点击操作栏项目时触发的事件
        /// </summary>
        private async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is OperationBarItemModel operationBarItem)
            {
                switch (operationBarItem.Tag)
                {
                    case "ProjectHomePage":
                        {
                            OpenProjectHomePage();
                            break;
                        }
                    case "SendFeedback":
                        {
                            SendFeedback();
                            break;
                        }
                    case "CheckUpdate":
                        {
                            if (!operationBarItem.IsCheckingUpdate)
                            {
                                operationBarItem.IsCheckingUpdate = true;
                                if (IsNetworkConnected())
                                {
                                    bool isNewest = false;

                                    try
                                    {
                                        operationBarItem.IsCheckingUpdate = true;
                                        isNewest = await GetIsNewestAsync();
                                        operationBarItem.IsCheckingUpdate = false;
                                        synchronizationContext.Post(async (_) =>
                                        {
                                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, Convert.ToInt32(isNewest)));
                                        }, null);
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OnItemClicked), 1, e);
                                        operationBarItem.IsCheckingUpdate = false;
                                        synchronizationContext.Post(async (_) =>
                                        {
                                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                                        }, null);
                                    }

                                    if (!isNewest)
                                    {
                                        await MainWindow.Current.ShowDialogAsync(new UpdateAppDialog());
                                    }
                                }
                                else
                                {
                                    operationBarItem.IsCheckingUpdate = false;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                                }
                            }
                            break;
                        }
                    case "HelpTranslate":
                        {
                            HelpTranslate();
                            break;
                        }
                    case "ShowLicense":
                        {
                            await MainWindow.Current.ShowDialogAsync(new LicenseDialog());
                            break;
                        }
                    case "ShowReleaseNotes":
                        {
                            ShowReleaseNotes();
                            break;
                        }
                    case "AppInformation":
                        {
                            await MainWindow.Current.ShowDialogAsync(new AppInformationDialog());
                            break;
                        }
                    case "SystemInformation":
                        {
                            ShowSystemInformation();
                            break;
                        }
                }
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            OperationBarItemList.Add(new()
            {
                Title = ProjectHomePageString,
                Tag = "ProjectHomePage",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE80F"
            });
            OperationBarItemList.Add(new()
            {
                Title = SendFeedbackString,
                Tag = "SendFeedback",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uED15"
            });
            OperationBarItemList.Add(new()
            {
                Tag = "CheckUpdate",
                OperationBarKind = OperationBarKind.CheckUpdate,
                IconGlyph = "\uE895"
            });
            OperationBarItemList.Add(new()
            {
                Title = HelpTranslateString,
                Tag = "HelpTranslate",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uF2B7"
            });
            OperationBarItemList.Add(new()
            {
                Title = ShowLicenseString,
                Tag = "ShowLicense",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE779"
            });
            OperationBarItemList.Add(new()
            {
                Title = ShowReleaseNotesString,
                Tag = "ShowReleaseNotes",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE70B"
            });
            OperationBarItemList.Add(new()
            {
                Title = AppInformationString,
                Tag = "AppInformation",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uECAA"
            });
            OperationBarItemList.Add(new()
            {
                Title = SystemInformationString,
                Tag = "SystemInformation",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE770"
            });
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void ShowReleaseNotes()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://apps.microsoft.com/detail/9MV67V21H386");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(ShowReleaseNotes), 1, e);
                }
            });
        }

        /// <summary>
        /// 查看系统信息
        /// </summary>
        private void ShowSystemInformation()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:about");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(ShowSystemInformation), 1, e);
                }
            });
        }

        /// <summary>
        /// 帮助翻译
        /// </summary>
        private void HelpTranslate()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/PowerToolbox/issues");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(HelpTranslate), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开项目主页
        /// </summary>
        private void OpenProjectHomePage()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/PowerToolbox");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(OpenProjectHomePage), 1, e);
                }
            });
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private void SendFeedback()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://github.com/Gaoyifei1011/PowerToolbox/issues");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsAboutPage), nameof(SendFeedback), 1, e);
                }
            });
        }

        /// <summary>
        /// 检查应用是否是最新版本
        /// </summary>
        private async Task<bool> GetIsNewestAsync()
        {
            StoreContext storeContext = StoreContext.GetDefault();
            IReadOnlyList<StorePackageUpdate> packageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
            return packageUpdateList.Count is 0;
        }

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

        #endregion 第五部分：数据操作与业务逻辑
    }
}
