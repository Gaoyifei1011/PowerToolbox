using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Cfgmgr32;
using PowerToolbox.WindowsAPI.PInvoke.Dxgi;
using PowerToolbox.WindowsAPI.PInvoke.PowrProf;
using PowerToolbox.WindowsAPI.PInvoke.Setupapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项——系统页面
    /// </summary>
    internal sealed partial class AdvancedSystemOptionsSystemPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AlwaysNotifyString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("AlwaysNotify");
        private readonly string NeverNotifyString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("NeverNotify");
        private readonly string NotifyString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("Notify");
        private readonly string NotifyWithoutDimmingString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("NotifyWithoutDimming");
        private readonly string HibernationFileTypeFullString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("HibernationFileTypeFull");
        private readonly string HibernationFileTypeReducedString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("HibernationFileTypeReduced");
        private readonly string HibernationFileTypeUnknownString = ResourceService.AdvancedSystemOptionsSystemResource.GetString("HibernationFileTypeUnknown");
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private readonly Guid Balance = new("381B4222-F694-41F0-9685-FF5BB260DF2E");
        private readonly Guid EnergySaving = new("A1841308-3541-4FAB-BC81-F71556F20B4A");
        private readonly Guid HighPerformance = new("8C5E7FDA-E8BF-4A96-9A85-A6E23A8C635C");
        private readonly Guid OutstandingPerformance = new("E9A42B02-D5DF-448D-AA00-03F14749EB61");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly string[] extensionsArray = [".avif", ".bmp", ".dib", ".gif", ".heic", ".heif", ".hif", ".ico", ".jfif", ".jpe", ".jpeg", ".jpg", ".jxl", ".jxr", ".png", ".tga", ".thumb", ".tif", ".tiff", ".webp"];
        private bool isInitialized;
        private AdvancedSystemOptionsPage advancedSystemOptionsPage;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isHibernationEnabled;

        private bool IsHibernationEnabled
        {
            get { return _isHibernationEnabled; }

            set
            {
                if (!Equals(_isHibernationEnabled, value))
                {
                    _isHibernationEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsHibernationEnabled)));
                }
            }
        }

        private bool _isHibernationOpened;

        private bool IsHibernationOpened
        {
            get { return _isHibernationOpened; }

            set
            {
                if (!Equals(_isHibernationOpened, value))
                {
                    _isHibernationOpened = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsHibernationOpened)));
                }
            }
        }

        private ComboBoxItemModel _selectedHibernationFileType;

        private ComboBoxItemModel SelectedHibernationFileType
        {
            get { return _selectedHibernationFileType; }

            set
            {
                if (!Equals(_selectedHibernationFileType, value))
                {
                    _selectedHibernationFileType = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedHibernationFileType)));
                }
            }
        }

        private string _hibernationFileSize;

        private string HibernationFileSize
        {
            get { return _hibernationFileSize; }

            set
            {
                if (!Equals(_hibernationFileSize, value))
                {
                    _hibernationFileSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(HibernationFileSize)));
                }
            }
        }

        private int _hibernationFilePercent;

        private int HibernationFilePercent
        {
            get { return _hibernationFilePercent; }

            set
            {
                if (!Equals(_hibernationFilePercent, value))
                {
                    _hibernationFilePercent = value;
                    PropertyChanged?.Invoke(this, new(nameof(HibernationFilePercent)));
                }
            }
        }

        private bool _isFastStartupEnabled;

        private bool IsFastStartupEnabled
        {
            get { return _isFastStartupEnabled; }

            set
            {
                if (!Equals(_isFastStartupEnabled, value))
                {
                    _isFastStartupEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsFastStartupEnabled)));
                }
            }
        }

        private bool _isGeneratingBatteryReport;

        private bool IsGeneratingBatteryReport
        {
            get { return _isGeneratingBatteryReport; }

            set
            {
                if (!Equals(_isGeneratingBatteryReport, value))
                {
                    _isGeneratingBatteryReport = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsGeneratingBatteryReport)));
                }
            }
        }

        private ComboBoxItemModel _selectedNotifyMode;

        private ComboBoxItemModel SelectedNotifyMode
        {
            get { return _selectedNotifyMode; }

            set
            {
                if (!Equals(_selectedNotifyMode, value))
                {
                    _selectedNotifyMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedNotifyMode)));
                }
            }
        }

        private bool _isBackgroundAppsTaskEnabled;

        private bool IsBackgroundAppsTaskEnabled
        {
            get { return _isBackgroundAppsTaskEnabled; }

            set
            {
                if (!Equals(_isBackgroundAppsTaskEnabled, value))
                {
                    _isBackgroundAppsTaskEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsBackgroundAppsTaskEnabled)));
                }
            }
        }

        private bool _isSystemReservedStorageLoadingOrUpdating;

        private bool IsSystemReservedStorageLoadingOrUpdating
        {
            get { return _isSystemReservedStorageLoadingOrUpdating; }

            set
            {
                if (!Equals(_isSystemReservedStorageLoadingOrUpdating, value))
                {
                    _isSystemReservedStorageLoadingOrUpdating = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSystemReservedStorageLoadingOrUpdating)));
                }
            }
        }

        private bool _isSystemReservedStorageEnabled;

        private bool IsSystemReservedStorageEnabled
        {
            get { return _isSystemReservedStorageEnabled; }

            set
            {
                if (!Equals(_isSystemReservedStorageEnabled, value))
                {
                    _isSystemReservedStorageEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSystemReservedStorageEnabled)));
                }
            }
        }

        private bool _isVirtualizationBasedSecurityEnabled;

        private bool IsVirtualizationBasedSecurityEnabled
        {
            get { return _isVirtualizationBasedSecurityEnabled; }

            set
            {
                if (!Equals(_isVirtualizationBasedSecurityEnabled, value))
                {
                    _isVirtualizationBasedSecurityEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsVirtualizationBasedSecurityEnabled)));
                }
            }
        }

        private bool _isNICOffloadSettingsEnabled;

        private bool IsNICOffloadSettingsEnabled
        {
            get { return _isNICOffloadSettingsEnabled; }

            set
            {
                if (!Equals(_isNICOffloadSettingsEnabled, value))
                {
                    _isNICOffloadSettingsEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsNICOffloadSettingsEnabled)));
                }
            }
        }

        private bool _isClosingWakeUpTask;

        private bool IsClosingWakeUpTask
        {
            get { return _isClosingWakeUpTask; }

            set
            {
                if (!Equals(_isClosingWakeUpTask, value))
                {
                    _isClosingWakeUpTask = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsClosingWakeUpTask)));
                }
            }
        }

        private bool _isRestartingGraphicsDriver;

        private bool IsRestartingGraphicsDriver
        {
            get { return _isRestartingGraphicsDriver; }

            set
            {
                if (!Equals(_isRestartingGraphicsDriver, value))
                {
                    _isRestartingGraphicsDriver = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRestartingGraphicsDriver)));
                }
            }
        }

        private bool _isWindowsPhotoViewerEnabled;

        private bool IsWindowsPhotoViewerEnabled
        {
            get { return _isWindowsPhotoViewerEnabled; }

            set
            {
                if (!Equals(_isWindowsPhotoViewerEnabled, value))
                {
                    _isWindowsPhotoViewerEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsWindowsPhotoViewerEnabled)));
                }
            }
        }

        private bool _isApprovalModeForBuiltinAdministratorAccountEnabled;

        private bool IsApprovalModeForBuiltinAdministratorAccountEnabled
        {
            get { return _isApprovalModeForBuiltinAdministratorAccountEnabled; }

            set
            {
                if (!Equals(_isApprovalModeForBuiltinAdministratorAccountEnabled, value))
                {
                    _isApprovalModeForBuiltinAdministratorAccountEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsApprovalModeForBuiltinAdministratorAccountEnabled)));
                }
            }
        }

        private List<ComboBoxItemModel> HibernationFileTypeList { get; } = [];

        private List<ComboBoxItemModel> NotifyModeList { get; } = [];

        private WinRTObservableCollection<string> WakeUpTaskCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal AdvancedSystemOptionsSystemPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is AdvancedSystemOptionsPage targetPage && !Equals(advancedSystemOptionsPage, targetPage))
            {
                advancedSystemOptionsPage = targetPage;
            }

            await InitializeDataAsync();
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 了解电源设置
        /// </summary>
        private void OnLearnPowerSettingsClicked(object sender, RoutedEventArgs args)
        {
            LearnPowerSettings();
        }

        /// <summary>
        /// 开启睡眠
        /// </summary>
        private async void OnEnableHibernationToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsHibernationOpened, toggleSwitch.IsOn))
            {
                IsHibernationOpened = toggleSwitch.IsOn;
                await SetHibernationAsync(IsHibernationOpened);
                await UpdateHibernationAsync();
            }
        }

        /// <summary>
        /// 修改休眠文件类型
        /// </summary>
        private async void OnHibernationFileTypeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedHibernationFileType, comboBox.SelectedItem))
            {
                SelectedHibernationFileType = comboBox.SelectedItem is ComboBoxItemModel hibernationFileType ? hibernationFileType : null;

                if (SelectedHibernationFileType is not null)
                {
                    await SetHibernationFileTypeAsync(SelectedHibernationFileType.SelectedValue);
                }
                await UpdateHibernationAsync();
            }
        }

        /// <summary>
        /// 休眠文件百分比值发生变化时触发的事件
        /// </summary>
        private void OnHibernationFilePercentValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN && args.OldValue is not double.NaN)
            {
                int newValue = Convert.ToInt32(args.NewValue);
                HibernationFilePercent = int.MaxValue;
                HibernationFilePercent = Convert.ToInt32(args.OldValue);

                if (Equals(SelectedHibernationFileType, HibernationFileTypeList[0]))
                {
                    HibernationFilePercent = 0;
                }
                else if (Equals(SelectedHibernationFileType, HibernationFileTypeList[1]))
                {
                    HibernationFilePercent = 20;
                }
                else if (Equals(SelectedHibernationFileType, HibernationFileTypeList[2]))
                {
                    if (newValue < 40)
                    {
                        HibernationFilePercent = 40;
                    }
                    else if (newValue > 100)
                    {
                        HibernationFilePercent = 100;
                    }
                    else
                    {
                        HibernationFilePercent = newValue;
                    }
                }
                else
                {
                    HibernationFilePercent = newValue;
                }
            }
        }

        /// <summary>
        /// 修改休眠文件大小
        /// </summary>
        private async void OnSaveHibernationFilePercentClicked(object sender, RoutedEventArgs args)
        {
            await SaveHibernationFilePercentAsync(HibernationFilePercent);
            await UpdateHibernationAsync();
        }

        /// <summary>
        /// 修改快速启动状态
        /// </summary>
        private async void OnFastStartupToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsFastStartupEnabled, toggleSwitch.IsOn))
            {
                IsFastStartupEnabled = toggleSwitch.IsOn;
                await SetIsFastStartupEnabledAsync(IsFastStartupEnabled);
                IsFastStartupEnabled = await GetIsFastStartupEnabledAsync();
            }
        }

        /// <summary>
        /// 打开系统电源设置
        /// </summary>
        private void OnOpenSystemPowerSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSystemPowerSettings();
        }

        /// <summary>
        /// 打开控制面板电源选项
        /// </summary>
        private void OnOpenControlPanelClicked(object sender, RoutedEventArgs args)
        {
            OpenControlPanel();
        }

        /// <summary>
        /// 修改电源模式
        /// </summary>
        private void OnChangePowerModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                ChangePowerMode(Convert.ToString(menuFlyoutItem.Tag));
            }
        }

        /// <summary>
        /// 生成电池报告
        /// </summary>
        private async void OnGenerateBatteryReportClicked(object sender, RoutedEventArgs args)
        {
            if (!IsGeneratingBatteryReport)
            {
                IsGeneratingBatteryReport = true;
                await GenerateBatteryReportAsync();
                IsGeneratingBatteryReport = false;
            }
        }

        /// <summary>
        /// 通知模式发生更改时触发的事件
        /// </summary>
        private async void OnNotifyModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedNotifyMode, comboBox.SelectedItem))
            {
                SelectedNotifyMode = comboBox.SelectedItem is ComboBoxItemModel notifyMode ? notifyMode : null;

                if (SelectedNotifyMode is not null)
                {
                    await SetUacLevelAsync(SelectedNotifyMode.SelectedValue);
                }

                UacLevel uacLevel = await GetUacLevelAsync();
                SelectedNotifyMode = NotifyModeList.Find((item) => Equals(item.SelectedValue, uacLevel));
            }
        }

        /// <summary>
        /// 后台应用任务可运行状态发生更改时触发的事件
        /// </summary>
        private async void OnBackgroundAppsTaskToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsBackgroundAppsTaskEnabled, toggleSwitch.IsOn))
            {
                IsBackgroundAppsTaskEnabled = toggleSwitch.IsOn;
                await SetIsBackgroundAppsTaskEnabledAsync(IsBackgroundAppsTaskEnabled);
                IsBackgroundAppsTaskEnabled = await GetIsBackgroundAppsTaskEnabledAsync();
            }
        }

        /// <summary>
        /// 打开系统存储设置
        /// </summary>
        private void OnOpenSystemStorageSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSystemStorageSettings();
        }

        /// <summary>
        /// 了解系统保留存储
        /// </summary>
        private void OnLearnSystemReservedStorageClicked(object sender, RoutedEventArgs args)
        {
            LearnSystemReservedStorage();
        }

        /// <summary>
        /// 启用 / 关闭系统保留存储
        /// </summary>
        private async void OnSystemReservedStorageToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsSystemReservedStorageEnabled, toggleSwitch.IsOn) && !IsSystemReservedStorageLoadingOrUpdating)
            {
                IsSystemReservedStorageLoadingOrUpdating = true;
                IsSystemReservedStorageEnabled = toggleSwitch.IsOn;
                await SetSystemReservedStorageAsync(IsSystemReservedStorageEnabled);
                IsSystemReservedStorageEnabled = await GetSystemReservedStorageAsync();
                IsSystemReservedStorageLoadingOrUpdating = false;
            }
        }

        /// <summary>
        /// 了解基于虚拟化的安全
        /// </summary>
        private void OnLearnVBSClicked(object sender, RoutedEventArgs args)
        {
            LearnVBS();
        }

        /// <summary>
        /// 修改基于虚拟化的安全状态
        /// </summary>
        private async void OnVirtualizationBasedSecurityToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsVirtualizationBasedSecurityEnabled, toggleSwitch.IsOn))
            {
                IsVirtualizationBasedSecurityEnabled = toggleSwitch.IsOn;
                await SetIsVirtualizationBasedSecurityEnabledAsync(IsVirtualizationBasedSecurityEnabled);
                IsVirtualizationBasedSecurityEnabled = await GetIsVirtualizationBasedSecurityEnabledAsync();
                ShowNotification(false, true);
            }
        }

        /// <summary>
        /// 了解网卡负载
        /// </summary>
        private void OnLearnNICOffloadClicked(object sender, RoutedEventArgs args)
        {
            LearnNICOffload();
        }

        /// <summary>
        /// 修改网卡负载状态
        /// </summary>
        private async void OnNICOffloadSettingsToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsNICOffloadSettingsEnabled, toggleSwitch.IsOn))
            {
                IsNICOffloadSettingsEnabled = toggleSwitch.IsOn;
                await SetNICOffloadSettingsEnabledAsync(IsNICOffloadSettingsEnabled);
                IsNICOffloadSettingsEnabled = await GetNICOffloadSettingsEnabledAsync();
                ShowNotification(false, true);
            }
        }

        /// <summary>
        /// 关闭唤醒任务
        /// </summary>
        private async void OnCloseWakeUpTaskClicked(object sender, RoutedEventArgs args)
        {
            if (!IsClosingWakeUpTask)
            {
                IsClosingWakeUpTask = true;
                await DisableAllWakeUpRunTaskAsync();
                List<string> wakeUpTaskList = await GetWaskUpTaskListAsync();
                WakeUpTaskCollection.Clear();
                if (wakeUpTaskList.Count > 0)
                {
                    foreach (string wakeUpTask in wakeUpTaskList)
                    {
                        WakeUpTaskCollection.Add(wakeUpTask);
                    }
                }
                IsClosingWakeUpTask = false;
            }
        }

        /// <summary>
        /// 重启显卡驱动
        /// </summary>
        private async void OnRestartGraphicsDriverClicked(object sender, RoutedEventArgs args)
        {
            if (!IsRestartingGraphicsDriver)
            {
                IsRestartingGraphicsDriver = true;
                await RestartGraphicsDriverAsync();
                IsRestartingGraphicsDriver = false;
            }
        }

        /// <summary>
        /// 创建上帝模式文件夹
        /// </summary>
        private void OnCreateGodModeClicked(object sender, RoutedEventArgs args)
        {
            CreateGodMode();
        }

        /// <summary>
        /// 启用 / 禁用 Windows 照片查看器
        /// </summary>
        private async void OnWindowsPhotoViewerSettingsToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsWindowsPhotoViewerEnabled, toggleSwitch.IsOn))
            {
                IsWindowsPhotoViewerEnabled = toggleSwitch.IsOn;
                await SetIsWindowsPhotoViewerEnabledAsync(IsWindowsPhotoViewerEnabled);
                IsWindowsPhotoViewerEnabled = await GetIsWindowsPhotoViewerEnabledAsync();
            }
        }

        /// <summary>
        /// 启用 / 禁用用于内置管理员帐户的管理员批准模式
        /// </summary>
        private async void OnApprovalModeForBuiltinAdministratorAccountSettingsToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsApprovalModeForBuiltinAdministratorAccountEnabled, toggleSwitch.IsOn))
            {
                IsApprovalModeForBuiltinAdministratorAccountEnabled = toggleSwitch.IsOn;
                await SetIsApprovalModeForBuiltinAdministratorAccountSettingsAsync(IsApprovalModeForBuiltinAdministratorAccountEnabled);
                IsApprovalModeForBuiltinAdministratorAccountEnabled = await GetIsApprovalModeForBuiltinAdministratorAccountSettingsAsync();
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async Task InitializeDataAsync()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                HibernationFileTypeList.Add(new() { DisplayMember = HibernationFileTypeUnknownString, SelectedValue = "HibernationFileTypeUnknown" });
                HibernationFileTypeList.Add(new() { DisplayMember = HibernationFileTypeReducedString, SelectedValue = "HibernationFileTypeReduced" });
                HibernationFileTypeList.Add(new() { DisplayMember = HibernationFileTypeFullString, SelectedValue = "HibernationFileTypeFull" });
                NotifyModeList.Add(new() { DisplayMember = AlwaysNotifyString, SelectedValue = UacLevel.AlwaysNotify });
                NotifyModeList.Add(new() { DisplayMember = NotifyString, SelectedValue = UacLevel.Notify });
                NotifyModeList.Add(new() { DisplayMember = NotifyWithoutDimmingString, SelectedValue = UacLevel.NotifyWithoutDimming });
                NotifyModeList.Add(new() { DisplayMember = NeverNotifyString, SelectedValue = UacLevel.NeverNotify });
            }

            if (RuntimeHelper.IsElevated)
            {
                await UpdateHibernationAsync();
                UacLevel uacLevel = await GetUacLevelAsync();
                SelectedNotifyMode = NotifyModeList.Find((item) => Equals(item.SelectedValue, uacLevel));
                IsBackgroundAppsTaskEnabled = await GetIsBackgroundAppsTaskEnabledAsync();

                synchronizationContext.Post(async (_) =>
                {
                    if (!IsSystemReservedStorageLoadingOrUpdating)
                    {
                        IsSystemReservedStorageLoadingOrUpdating = true;
                        IsSystemReservedStorageEnabled = await GetSystemReservedStorageAsync();
                        IsSystemReservedStorageLoadingOrUpdating = false;
                    }
                }, null);

                synchronizationContext.Post(async (_) =>
                {
                    IsVirtualizationBasedSecurityEnabled = await GetIsVirtualizationBasedSecurityEnabledAsync();
                }, null);

                IsNICOffloadSettingsEnabled = await GetNICOffloadSettingsEnabledAsync();

                synchronizationContext.Post(async (_) =>
                {
                    IsClosingWakeUpTask = true;
                    List<string> wakeUpTaskList = await GetWaskUpTaskListAsync();
                    WakeUpTaskCollection.Clear();
                    if (wakeUpTaskList.Count > 0)
                    {
                        foreach (string wakeUpTask in wakeUpTaskList)
                        {
                            WakeUpTaskCollection.Add(wakeUpTask);
                        }
                    }
                    IsClosingWakeUpTask = false;
                }, null);

                IsWindowsPhotoViewerEnabled = await GetIsWindowsPhotoViewerEnabledAsync();
                IsApprovalModeForBuiltinAdministratorAccountEnabled = await GetIsApprovalModeForBuiltinAdministratorAccountSettingsAsync();
            }
        }

        /// <summary>
        /// 了解电源设置
        /// </summary>
        private void LearnPowerSettings()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows/win32/power/system-power-states");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(LearnPowerSettings), 1, e);
                }
            });
        }

        /// <summary>
        /// 更新系统休眠状态
        /// </summary>
        private async Task UpdateHibernationAsync()
        {
            SYSTEM_POWER_CAPABILITIES systemPowerCapabilities = await GetSystemPowerCapabilitiesAsync();
            IsHibernationEnabled = systemPowerCapabilities.SystemS4;
            IsHibernationOpened = systemPowerCapabilities.HiberFilePresent;
            (int hiberFileType, int hiberFileSizePercent) = await GetHibernateFileInformationAsync();
            if (hiberFileType is 0)
            {
                SelectedHibernationFileType = HibernationFileTypeList[0];
                HibernationFilePercent = 0;
                HibernationFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0);
            }
            else if (hiberFileType is 1)
            {
                SelectedHibernationFileType = HibernationFileTypeList[1];
                HibernationFilePercent = 20;
                HibernationFileSize = await GetHibernationFileSizeAsync();
            }
            else if (hiberFileType is 2)
            {
                SelectedHibernationFileType = HibernationFileTypeList[2];
                if (hiberFileSizePercent < 40)
                {
                    HibernationFilePercent = 40;
                }
                else if (hiberFileSizePercent > 100)
                {
                    HibernationFilePercent = 100;
                }
                else
                {
                    HibernationFilePercent = hiberFileSizePercent;
                }
                HibernationFileSize = await GetHibernationFileSizeAsync();
            }
            else
            {
                SelectedHibernationFileType = HibernationFileTypeList[0];
                HibernationFilePercent = 0;
                HibernationFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0);
            }

            IsFastStartupEnabled = await GetIsFastStartupEnabledAsync();
        }

        /// <summary>
        /// 设置系统休眠状态
        /// </summary>
        private async Task SetHibernationAsync(bool isHibernationOpened)
        {
            await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    try
                    {
                        Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "powercfg.exe",
                            Arguments = string.Format("/hibernate {0}", isHibernationOpened ? "on" : "off"),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        powerCfgProcess.WaitForExit();
                        powerCfgProcess.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(SetHibernationAsync), 1, e);
                    }
                }
            });
        }

        /// <summary>
        /// 修改休眠文件类型
        /// </summary>
        private async Task SetHibernationFileTypeAsync(object hibernationFileType)
        {
            if (hibernationFileType is null)
            {
                return;
            }

            await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    if (hibernationFileType is "HibernationFileTypeUnknown")
                    {
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileType", 0);
                    }
                    else if (hibernationFileType is "HibernationFileTypeReduced")
                    {
                        try
                        {
                            Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "powercfg.exe",
                                Arguments = "/h /size 0",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            });
                            powerCfgProcess.WaitForExit();
                            powerCfgProcess.Dispose();
                            powerCfgProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "powercfg.exe",
                                Arguments = "/h /type reduced",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            });
                            powerCfgProcess.WaitForExit();
                            powerCfgProcess.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(SetHibernationFileTypeAsync), 1, e);
                        }
                    }
                    else if (hibernationFileType is "HibernationFileTypeFull")
                    {
                        try
                        {
                            Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "powercfg.exe",
                                Arguments = "/h /type full",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            });
                            powerCfgProcess.WaitForExit();
                            powerCfgProcess.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(SetHibernationFileTypeAsync), 2, e);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 获取系统电源功能的信息
        /// </summary>
        private async Task<SYSTEM_POWER_CAPABILITIES> GetSystemPowerCapabilitiesAsync()
        {
            return await Task.Run(() =>
            {
                PowrProfLibrary.GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilities);
                return systemPowerCapabilities;
            });
        }

        /// <summary>
        /// 获取休眠文件信息
        /// </summary>
        private async Task<(int, int)> GetHibernateFileInformationAsync()
        {
            return await Task.Run(() =>
            {
                int hiberFileType = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileType");
                int hiberFileSizePercent = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileSizePercent");
                return ValueTuple.Create(hiberFileType, hiberFileSizePercent);
            });
        }

        /// <summary>
        /// 获取休眠文件大小
        /// </summary>
        private async Task<string> GetHibernationFileSizeAsync()
        {
            return await Task.Run(() =>
            {
                string hibernationFile = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "hiberfil.sys");
                string hibernationFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0);

                if (File.Exists(hibernationFile))
                {
                    try
                    {
                        FileInfo hibernationFileInfo = new(hibernationFile);
                        hibernationFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(hibernationFileInfo.Length);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(GetHibernationFileSizeAsync), 1, e);
                    }
                }

                return hibernationFileSize;
            });
        }

        /// <summary>
        /// 获取系统快速启动启用状态
        /// </summary>
        private async Task<bool> GetIsFastStartupEnabledAsync()
        {
            return await Task.Run(() =>
            {
                return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled");
            });
        }

        /// <summary>
        /// 设置系统快速启动启用状态
        /// </summary>
        private async Task SetIsFastStartupEnabledAsync(bool isFastStartupEnabled)
        {
            await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", Convert.ToInt32(isFastStartupEnabled));
                }
            });
        }

        /// <summary>
        /// 修改休眠文件大小
        /// </summary>
        private async Task SaveHibernationFilePercentAsync(int hibernationFilePercent)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "powercfg.exe",
                            Arguments = string.Format("/h /size {0}", hibernationFilePercent),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        powerCfgProcess.WaitForExit();
                        powerCfgProcess.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(SaveHibernationFilePercentAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开系统电源设置
        /// </summary>
        private void OpenSystemPowerSettings()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:powersleep");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(OpenSystemPowerSettings), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开控制面板电源选项
        /// </summary>
        private void OpenControlPanel()
        {
            Task.Run(() =>
            {
                IOpenControlPanel openControlPanel = (IOpenControlPanel)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_OpenControlPanel));
                openControlPanel?.Open("Microsoft.PowerOptions", null, 0);
            });
        }

        /// <summary>
        /// 修改电源模式
        /// </summary>
        private void ChangePowerMode(string powerMode)
        {
            if (string.IsNullOrEmpty(powerMode))
            {
                return;
            }

            Task.Run(() =>
            {
                switch (powerMode)
                {
                    case "EnergySaving":
                        {
                            uint result = PowrProfLibrary.PowerSetActiveScheme(0, EnergySaving);
                            break;
                        }
                    case "Balance":
                        {
                            uint result = PowrProfLibrary.PowerSetActiveScheme(0, Balance);
                            break;
                        }
                    case "HighPerformance":
                        {
                            uint result = PowrProfLibrary.PowerSetActiveScheme(0, HighPerformance);
                            break;
                        }
                    case "OutstandingPerformance":
                        {
                            uint result = PowrProfLibrary.PowerSetActiveScheme(0, OutstandingPerformance);
                            break;
                        }
                }
            });
        }

        /// <summary>
        /// 生成电池报告
        /// </summary>
        private async Task GenerateBatteryReportAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "powercfg.exe",
                        Arguments = string.Format("/batteryreport /output {0}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "battery-report.html")),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    powerCfgProcess.WaitForExit();
                    powerCfgProcess.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(GenerateBatteryReportAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取通知模式
        /// </summary>
        private async Task<UacLevel> GetUacLevelAsync()
        {
            return await Task.Run(UACHelper.GetUacLevel);
        }

        /// <summary>
        /// 设置通知模式
        /// </summary>
        private async Task SetUacLevelAsync(object uacLevelObj)
        {
            await Task.Run(() =>
            {
                if (uacLevelObj is UacLevel uacLevel)
                {
                    UACHelper.SetUacLevel(uacLevel);
                }
            });
        }

        /// <summary>
        /// 获取后台应用任务可运行状态
        /// </summary>
        private async Task<bool> GetIsBackgroundAppsTaskEnabledAsync()
        {
            return await Task.Run(() =>
            {
                return RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled") is 0;
            });
        }

        /// <summary>
        /// 设置后台应用任务可运行状态
        /// </summary>
        private async Task SetIsBackgroundAppsTaskEnabledAsync(bool isBackgroundAppsTaskEnabled)
        {
            await Task.Run(() =>
            {
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", isBackgroundAppsTaskEnabled ? 0 : 1);
            });
        }

        /// <summary>
        /// 打开系统存储设置
        /// </summary>
        private void OpenSystemStorageSettings()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:storagesense");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(OpenSystemStorageSettings), 1, e);
                }
            });
        }

        /// <summary>
        /// 了解系统保留存储
        /// </summary>
        private void LearnSystemReservedStorage()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://support.microsoft.com/windows/storage-settings-in-windows-5bc98443-0711-8038-4621-6a18ddc904f2");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(LearnSystemReservedStorage), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取系统保留存储
        /// </summary>
        private async Task<bool> GetSystemReservedStorageAsync()
        {
            return await Task.Run(() =>
            {
                bool isSystemReservedStorageEnabled = false;

                try
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        Process powerShellProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "powershell.exe",
                            Arguments = "-NoProfile -ExecutionPolicy Bypass -Command Get-WindowsReservedStorageState",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                        string output = powerShellProcess.StandardOutput.ReadToEnd();
                        string error = powerShellProcess.StandardError.ReadToEnd();
                        powerShellProcess.WaitForExit();
                        powerShellProcess.Dispose();
                        if (output.Contains("Enabled"))
                        {
                            isSystemReservedStorageEnabled = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(GetSystemReservedStorageAsync), 1, e);
                }

                return isSystemReservedStorageEnabled;
            });
        }

        /// <summary>
        /// 设置系统保留存储
        /// </summary>
        private async Task SetSystemReservedStorageAsync(bool isSystemReservedStorageEnabled)
        {
            await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    try
                    {
                        Process dismProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "dism.exe",
                            Arguments = string.Format("/Online /Set-ReservedStorageState /State:{0}", isSystemReservedStorageEnabled ? "Enabled" : "Disabled"),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        dismProcess.WaitForExit();
                        dismProcess.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(SetSystemReservedStorageAsync), 1, e);
                    }
                }
            });
        }

        /// <summary>
        /// 了解基于虚拟化的安全
        /// </summary>
        private void LearnVBS()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows-hardware/design/device-experiences/oem-vbs");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(LearnVBS), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取基于虚拟化的安全状态
        /// </summary>
        private async Task<bool> GetIsVirtualizationBasedSecurityEnabledAsync()
        {
            return await Task.Run(() =>
            {
                bool isVirtualizationBasedSecurityEnabled = false;

                try
                {
                    bool hypervisorEnforcedCodeIntegrityEnabled = RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled");
                    bool enableVirtualizationBasedSecurity = RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard", "EnableVirtualizationBasedSecurity");
                    string hypervisorLaunchType = string.Empty;

                    Process bcdeditProcess = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "bcdedit.exe",
                        Arguments = "/enum",
                        Verb = "open",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    string output = bcdeditProcess.StandardOutput.ReadToEnd();
                    string error = bcdeditProcess.StandardError.ReadToEnd();
                    bcdeditProcess.WaitForExit();
                    bcdeditProcess.Dispose();

                    if (string.IsNullOrEmpty(output))
                    {
                        string[] lines = output.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
                        string hypervisorLaunchTypeLine = lines.FirstOrDefault(line => line.Trim().StartsWith("hypervisorlaunchtype", StringComparison.OrdinalIgnoreCase));

                        if (hypervisorLaunchTypeLine is not null)
                        {
                            string[] hypervisorLaunchTypeState = hypervisorLaunchTypeLine.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                            if (hypervisorLaunchTypeState.Length >= 2)
                            {
                                hypervisorLaunchType = hypervisorLaunchTypeState[1];
                            }
                        }
                    }
                    isVirtualizationBasedSecurityEnabled = (hypervisorEnforcedCodeIntegrityEnabled && enableVirtualizationBasedSecurity) || hypervisorLaunchType.Contains("Auto");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(OnVirtualizationBasedSecurityToggled), 1, e);
                }

                return isVirtualizationBasedSecurityEnabled;
            });
        }

        /// <summary>
        /// 修改基于虚拟化的安全状态
        /// </summary>
        private async Task SetIsVirtualizationBasedSecurityEnabledAsync(bool isVirtualizationBasedSecurityEnabled)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", isVirtualizationBasedSecurityEnabled);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard", "EnableVirtualizationBasedSecurity", isVirtualizationBasedSecurityEnabled);
                        Process bcdeditSetProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "bcdedit.exe",
                            Arguments = "/set hypervisorlaunchtype" + " " + (isVirtualizationBasedSecurityEnabled ? "auto" : "off"),
                            Verb = "open",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        bcdeditSetProcess.WaitForExit();
                        bcdeditSetProcess.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(SetIsVirtualizationBasedSecurityEnabledAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 了解网卡负载
        /// </summary>
        private void LearnNICOffload()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows-hardware/drivers/network/using-registry-values-to-enable-and-disable-task-offloading");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(LearnNICOffload), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取网卡负载状态
        /// </summary>
        private async Task<bool> GetNICOffloadSettingsEnabledAsync()
        {
            return await Task.Run(() =>
            {
                return RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload") is 0;
            });
        }

        /// <summary>
        /// 修改网卡负载状态
        /// </summary>
        private async Task SetNICOffloadSettingsEnabledAsync(bool isNICOffloadSettingsEnabled)
        {
            await Task.Run(() =>
            {
                RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload", isNICOffloadSettingsEnabled ? 0 : 1);
            });
        }

        /// <summary>
        /// 获取持续唤醒任务列表
        /// </summary>
        internal static async Task<List<string>> GetWaskUpTaskListAsync()
        {
            return await Task.Run(() =>
            {
                List<string> wakeUpTaskList = [];
                try
                {
                    ITaskService service = (ITaskService)Activator.CreateInstance(Type.GetTypeFromCLSID(new("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
                    service.Connect();
                    if (service.Connected)
                    {
                        Stack<ITaskFolder> folders = new();
                        folders.Push(service.GetFolder("\\"));

                        while (folders.Count > 0)
                        {
                            ITaskFolder taskFolder = folders.Pop();
                            foreach (IRegisteredTask registeredTask in taskFolder.GetTasks((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN))
                            {
                                try
                                {
                                    ITaskDefinition taskDefinition = registeredTask.Definition;
                                    bool enabled = registeredTask.Enabled;
                                    bool wakeToRun = taskDefinition.Settings.WakeToRun;
                                    string userId = taskDefinition.Principal.UserId ?? string.Empty;
                                    _TASK_LOGON_TYPE logonType = taskDefinition.Principal.LogonType;
                                    bool requiresPassword = logonType is _TASK_LOGON_TYPE.TASK_LOGON_PASSWORD || logonType is _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN_OR_PASSWORD;
                                    bool isSystem = userId.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase) || userId.Equals(@"NT AUTHORITY\SYSTEM", StringComparison.OrdinalIgnoreCase);
                                    if (!isSystem && !requiresPassword && enabled && wakeToRun)
                                    {
                                        wakeUpTaskList.Add(registeredTask.Name);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(GetWaskUpTaskListAsync), 1, e);
                                }
                            }

                            foreach (ITaskFolder subFolder in taskFolder.GetFolders(0))
                            {
                                folders.Push(subFolder);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(GetWaskUpTaskListAsync), 2, e);
                }
                return wakeUpTaskList;
            });
        }

        /// <summary>
        /// 禁用所有持续唤醒任务
        /// </summary>
        private async Task DisableAllWakeUpRunTaskAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    ITaskService taskService = (ITaskService)Activator.CreateInstance(Type.GetTypeFromCLSID(new("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
                    taskService.Connect();
                    Queue<ITaskFolder> taskFolderQueue = new();
                    taskFolderQueue.Enqueue(taskService.GetFolder("\\"));

                    while (taskFolderQueue.Count > 0)
                    {
                        ITaskFolder taskFolder = taskFolderQueue.Dequeue();
                        IRegisteredTaskCollection registeredTaskCollection = taskFolder.GetTasks((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN);

                        foreach (IRegisteredTask registeredTask in registeredTaskCollection)
                        {
                            try
                            {
                                ITaskDefinition definition = registeredTask.Definition;

                                if (definition.Settings.WakeToRun)
                                {
                                    definition.Settings.WakeToRun = false;
                                    taskFolder.RegisterTaskDefinition(registeredTask.Name, definition, (int)_TASK_CREATION.TASK_UPDATE, Type.Missing, Type.Missing, definition.Principal.LogonType, Type.Missing);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(DisableAllWakeUpRunTaskAsync), 1, e);
                            }
                        }

                        ITaskFolderCollection subTaskFolderCollection = taskFolder.GetFolders(0);
                        foreach (ITaskFolder subTaskFolder in subTaskFolderCollection)
                        {
                            taskFolderQueue.Enqueue(subTaskFolder);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(DisableAllWakeUpRunTaskAsync), 2, e);
                }
            });
        }

        /// <summary>
        /// 重启显卡驱动
        /// </summary>
        private async Task RestartGraphicsDriverAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    string name = string.Empty;
                    string id = string.Empty;
                    uint deviceId = 0;
                    string deviceName = string.Empty;
                    uint vendor = 0;

                    Guid dxgiFactoryGuid = typeof(IDXGIFactory6).GUID;
                    if (DxgiLibrary.CreateDXGIFactory(ref dxgiFactoryGuid, out nint ppFactory) is 0)
                    {
                        object factory = Marshal.GetObjectForIUnknown(ppFactory);
                        Guid dxgiAdapterGuid = typeof(IDXGIAdapter).GUID;
                        if (((IDXGIFactory6)factory).EnumAdapterByGpuPreference(0, DXGI_GPU_PREFERENCE.DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, in dxgiAdapterGuid, out IDXGIAdapter dxgiAdapter) is 0 && dxgiAdapter.GetDesc(out DXGI_ADAPTER_DESC dxgiAdapterDesc) is 0)
                        {
                            name = dxgiAdapterDesc.Description;
                            id = string.Empty;
                            deviceId = dxgiAdapterDesc.DeviceId;
                            deviceName = string.Empty;
                            vendor = dxgiAdapterDesc.VendorId;
                            string[] deviceArray = GetDevices(new("1CA05180-A699-450A-9A0C-DE4FBE3DDD89"), CM_GET_DEVICE_INTERFACE_LIST_FLAGS.CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
                            DEVPROPKEY devPropKey = new()
                            {
                                fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"),
                                pid = 2
                            };
                            foreach (string dev in deviceArray)
                            {
                                string pnp = FormatIdentifier(dev);
                                uint size = 0;
                                Cfgmgr32Library.CM_Locate_DevNode(out uint node, pnp, CM_LOCATE_DEVNODE_FLAGS.CM_LOCATE_DEVNODE_NORMAL);
                                Cfgmgr32Library.CM_Get_DevNode_Property(node, ref devPropKey, out _, IntPtr.Zero, ref size, 0);
                                nint buffer = Marshal.AllocHGlobal((int)size);
                                Cfgmgr32Library.CM_Get_DevNode_Property(node, ref devPropKey, out _, buffer, ref size, 0);
                                string description = Marshal.PtrToStringUni(buffer);
                                Marshal.FreeHGlobal(buffer);
                                if (string.Equals(name, description))
                                {
                                    id = pnp;
                                    deviceName = dev;
                                    break;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(id))
                    {
                        Process pnputilProcess = new()
                        {
                            StartInfo = new()
                            {
                                FileName = "pnputil.exe",
                                Arguments = "/restart-device \"" + id + "\"",
                                Verb = "open",
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            }
                        };
                        pnputilProcess.Start();
                        pnputilProcess.WaitForExit();
                        pnputilProcess.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(RestartGraphicsDriverAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 创建上帝模式文件夹
        /// </summary>
        private void CreateGodMode()
        {
            Task.Run(() =>
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GodMode.{ED7BA470-8E54-465E-825C-99712043E01C}"));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsSystemPage), nameof(CreateGodMode), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取 Windows 照片查看器启用状态
        /// </summary>
        private async Task<bool> GetIsWindowsPhotoViewerEnabledAsync()
        {
            return await Task.Run(() =>
            {
                bool isWindowsPhotoViewerEnabled = true;
                foreach (string extension in extensionsArray)
                {
                    isWindowsPhotoViewerEnabled = string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows Photo Viewer\Capabilities\FileAssociations", extension), "PhotoViewer.FileAssoc.Tiff");
                    if (!isWindowsPhotoViewerEnabled)
                    {
                        break;
                    }
                }
                return isWindowsPhotoViewerEnabled;
            });
        }

        /// <summary>
        /// 修改 Windows 照片查看器启用状态
        /// </summary>
        private async Task SetIsWindowsPhotoViewerEnabledAsync(bool isWindowsPhotoViewerEnabled)
        {
            await Task.Run(() =>
            {
                if (isWindowsPhotoViewerEnabled)
                {
                    RegistryHelper.SaveRegistryKey(Registry.ClassesRoot, @"Applications\photoviewer.dll\shell\open", "MuiVerb", "@photoviewer.dll,-3043", true);
                    RegistryHelper.SaveRegistryKey(Registry.ClassesRoot, @"Applications\photoviewer.dll\shell\open\command", string.Empty, "\"%SystemRoot%\\System32\\rundll32.exe\" \"%ProgramFiles%\\Windows Photo Viewer\\PhotoViewer.dll\", ImageView_Fullscreen %1", true);
                    RegistryHelper.SaveRegistryKey(Registry.ClassesRoot, @"Applications\photoviewer.dll\shell\open\DropTarget", "Clsid", "{FFE2A43C-56B9-4BF5-9A79-CC6D4285608A}");
                    foreach (string extension in extensionsArray)
                    {
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows Photo Viewer\Capabilities\FileAssociations", extension, "PhotoViewer.FileAssoc.Tiff");
                    }
                }
                else
                {
                    RegistryHelper.DeleteRegistryKey(Registry.ClassesRoot, @"Applications\photoviewer.dll\shell\open", true);
                    foreach (string extension in extensionsArray)
                    {
                        RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows Photo Viewer\Capabilities\FileAssociations", extension);
                    }
                }
            });
        }

        /// <summary>
        /// 获取用于内置管理员帐户的管理员批准模式
        /// </summary>
        private async Task<bool> GetIsApprovalModeForBuiltinAdministratorAccountSettingsAsync()
        {
            return await Task.Run(() =>
            {
                int? FilterAdministratorTokenValue = RegistryHelper.ReadRegistryKey<int?>(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "FilterAdministratorToken");
                return FilterAdministratorTokenValue.HasValue && FilterAdministratorTokenValue.Value is 1;
            });
        }

        /// <summary>
        /// 设置用于内置管理员帐户的管理员批准模式
        /// </summary>
        private async Task SetIsApprovalModeForBuiltinAdministratorAccountSettingsAsync(bool isApprovalModeForBuiltinAdministratorAccountEnabled)
        {
            await Task.Run(() =>
            {
                if (isApprovalModeForBuiltinAdministratorAccountEnabled)
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "FilterAdministratorToken", 1);
                }
                else
                {
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "FilterAdministratorToken");
                }
            });
        }

        /// <summary>
        /// 获取所有设备
        /// </summary>
        private static string[] GetDevices(Guid interfaceClassGuid, CM_GET_DEVICE_INTERFACE_LIST_FLAGS flags)
        {
            Cfgmgr32Library.CM_Get_Device_Interface_List_Size(out uint size, in interfaceClassGuid, null, flags);
            byte[] buffer = new byte[size * sizeof(char)];
            Cfgmgr32Library.CM_Get_Device_Interface_List(in interfaceClassGuid, null, buffer, size, flags);
            string device = Encoding.Unicode.GetString(buffer);
            return device.Split(['\0'], StringSplitOptions.RemoveEmptyEntries);
        }

        private static string FormatIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }
            if (identifier.StartsWith("\\\\?\\"))
            {
                string text = identifier;
                identifier = text.Substring(4);
            }
            if (identifier.Length > 0 && identifier.Contains('}') && identifier.Contains('{'))
            {
                int brace = identifier.IndexOf('{');
                if (brace > 0)
                {
                    identifier = identifier.Substring(0, brace - 1);
                }
            }
            return identifier.Replace('#', '\\');
        }

        /// <summary>
        /// 获取选中的通知模式
        /// </summary>
        private Visibility GetNotifyModeVisibility(object selectedValue, object comparedValue)
        {
            return Equals(selectedValue, comparedValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        private void ShowNotification(bool isRestartExplorer, bool isRestartPC)
        {
            if (advancedSystemOptionsPage is not null)
            {
                if (isRestartExplorer)
                {
                    advancedSystemOptionsPage.IsRestartExplorerVisible = true;
                }

                if (isRestartPC)
                {
                    advancedSystemOptionsPage.IsRestartPCVisible = true;
                }

                advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
            }
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
