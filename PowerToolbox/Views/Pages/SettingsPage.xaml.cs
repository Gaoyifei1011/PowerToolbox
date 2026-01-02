using IWshRuntimeLibrary;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Shell;
using Windows.UI.StartScreen;

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

        private List<Type> PageList { get; } = [typeof(SettingsGeneralPage), typeof(SettingsAdvancedPage), typeof(SettingsAboutPage)];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            SettingsFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                SelectedItem = SettingsSelctorBar.Items[0];
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectedItem = sender.SelectedItem;
            int index = sender.Items.IndexOf(SelectedItem);
            Type currentPage = GetCurrentPageType();
            int currentIndex = PageList.FindIndex(item => Equals(item, currentPage));

            if (index is 0)
            {
                if (currentPage is null)
                {
                    NavigateTo(PageList[0]);
                }
                else if (!Equals(currentPage, PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
                }
            }
            else if (index is 1 && !Equals(currentPage, PageList[1]))
            {
                NavigateTo(PageList[1], null, index > currentIndex);
            }
            else if (index is 2 && !Equals(GetCurrentPageType(), PageList[2]))
            {
                NavigateTo(PageList[2], null, index > currentIndex);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < SettingsSelctorBar.Items.Count)
            {
                SelectedItem = SettingsSelctorBar.Items[index];
            }

            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(OnNavigationFailed), 1, args.Exception);
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(OnPinToDesktopClicked), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(OnPinToStartScreenClicked), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(OnPinToTaskbarClicked), 1, e);
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
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:appsfeatures-app");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(OnAppSettingsClicked), 1, e);
                }
            });
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(OnTroubleShootClicked), 1, e);
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
                if (slideDirection.HasValue)
                {
                    SettingsFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                SettingsFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsPage), nameof(NavigateTo), 1, e);
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
