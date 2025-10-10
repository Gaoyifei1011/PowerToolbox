using IWshRuntimeLibrary;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private readonly string AppNameString = ResourceService.SettingsResource.GetString("AppName");
        private Guid IID_ITaskbarManagerDesktopAppSupportStatics = new("CDFEFD63-E879-4134-B9A7-8283F05F9480");
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

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            new KeyValuePair<string, Type>("General",typeof(SettingsGeneralPage)),
            new KeyValuePair<string, Type>("Advanced", typeof(SettingsAdvancedPage)),
            new KeyValuePair<string, Type>("About", typeof(SettingsAboutPage)),
        ];

        private List<NavigationModel> NavigationItemList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            SettingsFrame.ContentTransitions = SuppressNavigationTransitionCollection;
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：设置页面——挂载的事件

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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnNavigated), 1, e);
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnNavigationFailed), 1, e);
            }
        }

        /// <summary>
        /// 关闭使用说明浮出栏
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (SettingsSplitView.IsPaneOpen)
            {
                SettingsSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        private async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new RestartAppsDialog());
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        private void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            ShowSettingsInstruction();
        }

        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        private void OnRunAsAdministratorClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        Arguments = "--elevated",
                        FileName = System.Windows.Forms.Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
            });
        }

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private async void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            bool isCreatedSuccessfully = await Task.Run(() =>
            {
                try
                {
                    WshShell wshShell = new();
                    WshShortcut wshShortcut = (WshShortcut)wshShell.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Format(@"{0}.lnk", AppNameString)));
                    uint aumidLength = 260;
                    StringBuilder aumidBuilder = new((int)aumidLength);
                    Kernel32Library.GetCurrentApplicationUserModelId(ref aumidLength, aumidBuilder);
                    wshShortcut.TargetPath = string.Format(@"shell:AppsFolder\{0}", Convert.ToString(aumidBuilder));
                    wshShortcut.Save();
                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnPinToDesktopClicked), 1, e);
                    return false;
                }
            });

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Desktop, isCreatedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            try
            {
                IReadOnlyList<AppListEntry> appEntriesList = await Package.Current.GetAppListEntriesAsync();

                if (appEntriesList[0] is AppListEntry defaultEntry)
                {
                    StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                    bool containsEntry = await startScreenManager.ContainsAppListEntryAsync(defaultEntry);

                    if (!containsEntry)
                    {
                        await startScreenManager.RequestAddAppListEntryAsync(defaultEntry);
                    }

                    isPinnedSuccessfully = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnPinToStartScreenClicked), 1, e);
            }
            finally
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.StartScreen, isPinnedSuccessfully));
            }
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            try
            {
                if (Marshal.QueryInterface(Marshal.GetIUnknownForObject(WindowsRuntimeMarshal.GetActivationFactory(typeof(TaskbarManager))), ref IID_ITaskbarManagerDesktopAppSupportStatics, out _) is 0)
                {
                    string featureId = "com.microsoft.windows.taskbar.pin";
                    string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                    string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                    LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                    if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                    {
                        isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnPinToTaskbarClicked), 1, e);
            }
            finally
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, isPinnedSuccessfully));
            }
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private void OnAppSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start("ms-settings:appsfeatures-app");
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnAppSettingsClicked), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 了解传递优化
        /// </summary>
        private void OnLearnDeliveryOptimizationClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 了解后台智能传输服务
        /// </summary>
        private void OnLearnBitsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:troubleshoot");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnTroubleShootClicked), 1, e);
                }
            });
        }

        #endregion 第二部分：设置页面——挂载的事件

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
                        SettingsFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                    }

                    // 导航到该项目对应的页面
                    SettingsFrame.Navigate(navigationItem.NavigationPage, parameter);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        private Type GetCurrentPageType()
        {
            return SettingsFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 显示设置说明
        /// </summary>
        public void ShowSettingsInstruction()
        {
            if (!SettingsSplitView.IsPaneOpen)
            {
                SettingsSplitView.IsPaneOpen = true;
            }
        }
    }
}
