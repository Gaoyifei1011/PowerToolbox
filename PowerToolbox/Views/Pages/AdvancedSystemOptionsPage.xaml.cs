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
using PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr;
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
    /// 高级系统选项页面
    /// </summary>
    public sealed partial class AdvancedSystemOptionsPage : Page, INotifyPropertyChanged
    {
        private readonly string AlwaysNotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("AlwaysNotify");
        private readonly string HibernationFileTypeReducedString = ResourceService.AdvancedSystemOptionsResource.GetString("HibernationFileTypeReduced");
        private readonly string HibernationFileTypeFullString = ResourceService.AdvancedSystemOptionsResource.GetString("HibernationFileTypeFull");
        private readonly string NeedRestartString = ResourceService.AdvancedSystemOptionsResource.GetString("NeedRestart");
        private readonly string NeverNotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("NeverNotify");
        private readonly string NotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("Notify");
        private readonly string NotifyWithoutDimmingString = ResourceService.AdvancedSystemOptionsResource.GetString("NotifyWithoutDimming");
        private readonly string OperationString = ResourceService.AdvancedSystemOptionsResource.GetString("Operation");
        private readonly string RestartExplorerString = ResourceService.AdvancedSystemOptionsResource.GetString("RestartExplorer");
        private readonly string RestartingString = ResourceService.AdvancedSystemOptionsResource.GetString("Restarting");
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private readonly Guid Balance = new("381B4222-F694-41F0-9685-FF5BB260DF2E");
        private readonly Guid EnergySaving = new("A1841308-3541-4FAB-BC81-F71556F20B4A");
        private readonly Guid HighPerformance = new("8C5E7FDA-E8BF-4A96-9A85-A6E23A8C635C");
        private readonly Guid OutstandingPerformance = new("E9A42B02-D5DF-448D-AA00-03F14749EB61");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        private InfoBarSeverity _advancedSettingsInfoSeverity;

        public InfoBarSeverity AdvancedSettingsInfoSeverity
        {
            get { return _advancedSettingsInfoSeverity; }

            set
            {
                if (!Equals(_advancedSettingsInfoSeverity, value))
                {
                    _advancedSettingsInfoSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedSettingsInfoSeverity)));
                }
            }
        }

        private string _AdvancedSettingsOperationText;

        public string AdvancedSettingsOperationText
        {
            get { return _AdvancedSettingsOperationText; }

            set
            {
                if (!Equals(_AdvancedSettingsOperationText, value))
                {
                    _AdvancedSettingsOperationText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedSettingsOperationText)));
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

        private ComboBoxItemModel _selectedNotifyMode;

        public ComboBoxItemModel SelectedNotifyMode
        {
            get { return _selectedNotifyMode; }

            set
            {
                if (!Equals(_selectedNotifyMode, value))
                {
                    _selectedNotifyMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNotifyMode)));
                }
            }
        }

        private bool _isBackgroundAppsTaskEnabled;

        public bool IsBackgroundAppsTaskEnabled
        {
            get { return _isBackgroundAppsTaskEnabled; }

            set
            {
                if (!Equals(_isBackgroundAppsTaskEnabled, value))
                {
                    _isBackgroundAppsTaskEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBackgroundAppsTaskEnabled)));
                }
            }
        }

        private bool _isHibernationEnabled;

        public bool IsHibernationEnabled
        {
            get { return _isHibernationEnabled; }

            set
            {
                if (!Equals(_isHibernationEnabled, value))
                {
                    _isHibernationEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHibernationEnabled)));
                }
            }
        }

        private bool _isHibernationOpened;

        public bool IsHibernationOpened
        {
            get { return _isHibernationOpened; }

            set
            {
                if (!Equals(_isHibernationOpened, value))
                {
                    _isHibernationOpened = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHibernationOpened)));
                }
            }
        }

        private ComboBoxItemModel _selectedHibernationFileType;

        public ComboBoxItemModel SelectedHibernationFileType
        {
            get { return _selectedHibernationFileType; }

            set
            {
                if (!Equals(_selectedHibernationFileType, value))
                {
                    _selectedHibernationFileType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedHibernationFileType)));
                }
            }
        }

        private string _hibernationFileSize;

        public string HibernationFileSize
        {
            get { return _hibernationFileSize; }

            set
            {
                if (!Equals(_hibernationFileSize, value))
                {
                    _hibernationFileSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HibernationFileSize)));
                }
            }
        }

        private int _hibernationFilePercent;

        public int HibernationFilePercent
        {
            get { return _hibernationFilePercent; }

            set
            {
                if (!Equals(_hibernationFilePercent, value))
                {
                    _hibernationFilePercent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HibernationFilePercent)));
                }
            }
        }

        private bool _isFastStartupEnabled;

        public bool IsFastStartupEnabled
        {
            get { return _isFastStartupEnabled; }

            set
            {
                if (!Equals(_isFastStartupEnabled, value))
                {
                    _isFastStartupEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFastStartupEnabled)));
                }
            }
        }

        private bool _isSystemReservedStorageLoadingOrUpdating;

        public bool IsSystemReservedStorageLoadingOrUpdating
        {
            get { return _isSystemReservedStorageLoadingOrUpdating; }

            set
            {
                if (!Equals(_isSystemReservedStorageLoadingOrUpdating, value))
                {
                    _isSystemReservedStorageLoadingOrUpdating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemReservedStorageLoadingOrUpdating)));
                }
            }
        }

        private bool _isGeneratingBatteryReport;

        public bool IsGeneratingBatteryReport
        {
            get { return _isGeneratingBatteryReport; }

            set
            {
                if (!Equals(_isGeneratingBatteryReport, value))
                {
                    _isGeneratingBatteryReport = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGeneratingBatteryReport)));
                }
            }
        }

        private bool _isSystemReservedStorageEnabled;

        public bool IsSystemReservedStorageEnabled
        {
            get { return _isSystemReservedStorageEnabled; }

            set
            {
                if (!Equals(_isSystemReservedStorageEnabled, value))
                {
                    _isSystemReservedStorageEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemReservedStorageEnabled)));
                }
            }
        }

        private bool _isRebuildingIconCache;

        public bool IsRebuildingIconCache
        {
            get { return _isRebuildingIconCache; }

            set
            {
                if (!Equals(_isRebuildingIconCache, value))
                {
                    _isRebuildingIconCache = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRebuildingIconCache)));
                }
            }
        }

        private bool _isVirtualizationBasedSecurityEnabled;

        public bool IsVirtualizationBasedSecurityEnabled
        {
            get { return _isVirtualizationBasedSecurityEnabled; }

            set
            {
                if (!Equals(_isVirtualizationBasedSecurityEnabled, value))
                {
                    _isVirtualizationBasedSecurityEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVirtualizationBasedSecurityEnabled)));
                }
            }
        }

        private bool _isNICOffloadSettingsEnabled;

        public bool IsNICOffloadSettingsEnabled
        {
            get { return _isNICOffloadSettingsEnabled; }

            set
            {
                if (!Equals(_isNICOffloadSettingsEnabled, value))
                {
                    _isNICOffloadSettingsEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNICOffloadSettingsEnabled)));
                }
            }
        }

        private bool _isClosingWakeUpTask;

        public bool IsClosingWakeUpTask
        {
            get { return _isClosingWakeUpTask; }

            set
            {
                if (!Equals(_isClosingWakeUpTask, value))
                {
                    _isClosingWakeUpTask = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsClosingWakeUpTask)));
                }
            }
        }

        private bool _isRestartingGraphicsDriver;

        public bool IsRestartingGraphicsDriver
        {
            get { return _isRestartingGraphicsDriver; }

            set
            {
                if (!Equals(_isRestartingGraphicsDriver, value))
                {
                    _isRestartingGraphicsDriver = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestartingGraphicsDriver)));
                }
            }
        }

        private List<ComboBoxItemModel> NotifyModeList { get; } = [];

        private List<ComboBoxItemModel> HibernationFileTypeList { get; } = [];

        public WinRTObservableCollection<string> WakeUpTaskCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPage()
        {
            InitializeComponent();
            AdvancedSettingsInfoSeverity = InfoBarSeverity.Informational;
            AdvancedSettingsOperationText = OperationString;
            RestartExplorerText = RestartExplorerString;
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = AlwaysNotifyString, SelectedValue = UacLevel.AlwaysNotify });
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = NotifyString, SelectedValue = UacLevel.Notify });
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = NotifyWithoutDimmingString, SelectedValue = UacLevel.NotifyWithoutDimming });
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = NeverNotifyString, SelectedValue = UacLevel.NeverNotify });
            HibernationFileTypeList.Add(new ComboBoxItemModel() { DisplayMember = HibernationFileTypeReducedString, SelectedValue = "HibernationFileTypeReduced" });
            HibernationFileTypeList.Add(new ComboBoxItemModel() { DisplayMember = HibernationFileTypeFullString, SelectedValue = "HibernationFileTypeFull" });
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (RuntimeHelper.IsElevated)
            {
                UacLevel uacLevel = await Task.Run(UACHelper.GetUacLevel);
                SelectedNotifyMode = NotifyModeList.Find((item) => Equals(item.SelectedValue, uacLevel));
                IsBackgroundAppsTaskEnabled = await Task.Run(() =>
                {
                    return RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled") is 0;
                });
                SYSTEM_POWER_CAPABILITIES systemPowerCapabilities = await Task.Run(() =>
                {
                    PowrProfLibrary.GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilities);
                    return systemPowerCapabilities;
                });
                IsHibernationEnabled = systemPowerCapabilities.SystemS4;
                IsHibernationOpened = systemPowerCapabilities.HiberFilePresent;

                (int hiberFileType, int hiberFileSizePercent) = await Task.Run(() =>
                {
                    int hiberFileType = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileType");
                    int hiberFileSizePercent = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileSizePercent");
                    return ValueTuple.Create(hiberFileType, hiberFileSizePercent);
                });
                if (hiberFileType is 1)
                {
                    SelectedHibernationFileType = HibernationFileTypeList[0];
                    HibernationFilePercent = 20;
                }
                else if (hiberFileType is 2)
                {
                    SelectedHibernationFileType = HibernationFileTypeList[1];
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
                }
                HibernationFileSize = await Task.Run(() =>
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnNavigatedTo), 1, e);
                        }
                    }

                    return hibernationFileSize;
                });
                IsFastStartupEnabled = await Task.Run(() =>
                {
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled");
                });
                if (!IsSystemReservedStorageLoadingOrUpdating)
                {
                    IsSystemReservedStorageLoadingOrUpdating = true;
                    _ = Task.Run(() =>
                    {
                        bool isSystemReservedStorageEnabled = false;

                        try
                        {
                            Process powerShellProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "powershell.exe",
                                Arguments = "-NoProfile -ExecutionPolicy Bypass -Command Get-WindowsReservedStorageState",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
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
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnNavigatedTo), 4, e);
                        }

                        synchronizationContext.Post((_) =>
                        {
                            IsSystemReservedStorageEnabled = isSystemReservedStorageEnabled;
                            IsSystemReservedStorageLoadingOrUpdating = false;
                        }, null);
                    });
                }
                _ = Task.Run(() =>
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnNavigatedTo), 5, e);
                    }

                    synchronizationContext.Post((_) =>
                    {
                        IsVirtualizationBasedSecurityEnabled = isVirtualizationBasedSecurityEnabled;
                    }, null);
                });

                IsNICOffloadSettingsEnabled = await Task.Run(() =>
                {
                    return RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload") is 0;
                });
                IsClosingWakeUpTask = true;
                _ = Task.Run(() =>
                {
                    List<string> wakeUpTaskList = GetWaskUpTaskList();
                    synchronizationContext.Post((_) =>
                    {
                        WakeUpTaskCollection.Clear();
                        foreach (string wakeUpTask in wakeUpTaskList)
                        {
                            WakeUpTaskCollection.Add(wakeUpTask);
                        }
                        IsClosingWakeUpTask = false;
                    }, null);
                });
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 重启资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            RestartExplorerText = RestartingString;
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
                            System.Runtime.InteropServices.ComTypes.FILETIME fileTime = new();
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRestartExplorerClicked), 1, e);
                }
            });
            IsRestarting = false;
            RestartExplorerText = RestartExplorerString;
            IsRestartExplorerVisible = false;
            if (!IsRestartExplorerVisible && !IsRestartPCVisible)
            {
                AdvancedSettingsInfoSeverity = InfoBarSeverity.Informational;
                AdvancedSettingsOperationText = OperationString;
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

        /// <summary>
        /// 打开系统电源设置
        /// </summary>
        private void OnOpenSystemPowerSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:powersleep");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnOpenSystemPowerSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开控制面板电源选项
        /// </summary>
        private void OnOpenControlPanelClicked(object sender, RoutedEventArgs args)
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
        private void OnChangePowerModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                string tag = Convert.ToString(menuFlyoutItem.Tag);
                Task.Run(() =>
                {
                    switch (tag)
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
        }

        /// <summary>
        /// 通知模式发生更改时触发的事件
        /// </summary>
        private async void OnNotifyModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel notifyMode && !Equals(SelectedNotifyMode, notifyMode))
            {
                SelectedNotifyMode = notifyMode;
                UacLevel uacLevel = await Task.Run(() =>
                {
                    UACHelper.SetUacLevel((UacLevel)SelectedNotifyMode.SelectedValue);
                    return UACHelper.GetUacLevel();
                });
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
                IsBackgroundAppsTaskEnabled = await Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", IsBackgroundAppsTaskEnabled ? 0 : 1);
                    return RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled") is 0;
                });
            }
        }

        /// <summary>
        /// 了解电源设置
        /// </summary>
        private void OnLearnPowerSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows/win32/power/system-power-states");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnLearnPowerSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 开启睡眠
        /// </summary>
        private async void OnEnableHibernationToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsHibernationEnabled, toggleSwitch.IsOn))
            {
                IsHibernationOpened = toggleSwitch.IsOn;

                SYSTEM_POWER_CAPABILITIES systemPowerCapabilities = await Task.Run(() =>
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "powercfg.exe",
                            Arguments = string.Format("/hibernate {0}", IsHibernationOpened ? "on" : "off"),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        powerCfgProcess.WaitForExit();
                        powerCfgProcess.Dispose();
                    }

                    PowrProfLibrary.GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilities);
                    return systemPowerCapabilities;
                });
                IsHibernationEnabled = systemPowerCapabilities.SystemS4;
                IsHibernationOpened = systemPowerCapabilities.HiberFilePresent;
                (int hiberFileType, int hiberFileSizePercent) = await Task.Run(() =>
                {
                    int hiberFileType = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileType");
                    int hiberFileSizePercent = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileSizePercent");
                    return ValueTuple.Create(hiberFileType, hiberFileSizePercent);
                });
                if (hiberFileType is 1)
                {
                    SelectedHibernationFileType = HibernationFileTypeList[0];
                    HibernationFilePercent = 20;
                }
                else if (hiberFileType is 2)
                {
                    SelectedHibernationFileType = HibernationFileTypeList[1];
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
                }
                HibernationFileSize = await Task.Run(() =>
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnNavigatedTo), 1, e);
                        }
                    }

                    return hibernationFileSize;
                });
                IsFastStartupEnabled = await Task.Run(() =>
                {
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled");
                });
            }
        }

        /// <summary>
        /// 修改休眠文件类型
        /// </summary>
        private async void OnHibernationFileTypeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel hibernationFileType && !Equals(SelectedHibernationFileType, hibernationFileType))
            {
                SelectedHibernationFileType = hibernationFileType;

                if (RuntimeHelper.IsElevated)
                {
                    await Task.Run(() =>
                    {
                        if (hibernationFileType.SelectedValue is "HibernationFileTypeReduced")
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
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnHibernationFileTypeSelectionChanged), 1, e);
                            }
                        }
                        else if (hibernationFileType.SelectedValue is "HibernationFileTypeFull")
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
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnHibernationFileTypeSelectionChanged), 2, e);
                            }
                        }
                    });
                }

                (int hiberFileType, int hiberFileSizePercent) = await Task.Run(() =>
                {
                    int hiberFileType = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileType");
                    int hiberFileSizePercent = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileSizePercent");
                    return ValueTuple.Create(hiberFileType, hiberFileSizePercent);
                });
                if (hiberFileType is 1)
                {
                    SelectedHibernationFileType = HibernationFileTypeList[0];
                    HibernationFilePercent = 20;
                }
                else if (hiberFileType is 2)
                {
                    SelectedHibernationFileType = HibernationFileTypeList[1];
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
                }
                HibernationFileSize = await Task.Run(() =>
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnNavigatedTo), 1, e);
                        }
                    }

                    return hibernationFileSize;
                });
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
                    HibernationFilePercent = 20;
                }
                else if (Equals(SelectedHibernationFileType, HibernationFileTypeList[1]))
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
            await Task.Run(() =>
            {
                try
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "powercfg.exe",
                            Arguments = string.Format("/h /size {0}", HibernationFilePercent),
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnSaveHibernationFilePercentClicked), 1, e);
                }
            });

            (int hiberFileType, int hiberFileSizePercent) = await Task.Run(() =>
            {
                int hiberFileType = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileType");
                int hiberFileSizePercent = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HiberFileSizePercent");
                return ValueTuple.Create(hiberFileType, hiberFileSizePercent);
            });
            if (hiberFileType is 1)
            {
                SelectedHibernationFileType = HibernationFileTypeList[0];
                HibernationFilePercent = 20;
            }
            else if (hiberFileType is 2)
            {
                SelectedHibernationFileType = HibernationFileTypeList[1];
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
            }
            HibernationFileSize = await Task.Run(() =>
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnNavigatedTo), 1, e);
                    }
                }

                return hibernationFileSize;
            });
        }

        /// <summary>
        /// 修改快速启动状态
        /// </summary>
        private async void OnFastStartupToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsFastStartupEnabled, toggleSwitch.IsOn))
            {
                IsFastStartupEnabled = toggleSwitch.IsOn;
                IsFastStartupEnabled = await Task.Run(() =>
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", Convert.ToInt32(IsFastStartupEnabled));
                    }
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled");
                });
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnGenerateBatteryReportClicked), 1, e);
                    }
                });
                IsGeneratingBatteryReport = false;
            }
        }

        /// <summary>
        /// 打开系统存储设置
        /// </summary>
        private void OnOpenSystemStorageSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:storagesense");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnOpenSystemStorageSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 了解系统保留存储
        /// </summary>
        private void OnLearnSystemReservedStorageClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://support.microsoft.com/windows/storage-settings-in-windows-5bc98443-0711-8038-4621-6a18ddc904f2");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnLearnSystemReservedStorageClicked), 1, e);
                }
            });
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
                IsSystemReservedStorageEnabled = await Task.Run(() =>
                {
                    bool isSystemReservedStorageEnabled = false;

                    try
                    {
                        if (RuntimeHelper.IsElevated)
                        {
                            Process dismProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "dism.exe",
                                Arguments = string.Format("/Online /Set-ReservedStorageState /State:{0}", IsSystemReservedStorageEnabled ? "Enabled" : "Disabled"),
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            });
                            dismProcess.WaitForExit();
                            dismProcess.Dispose();
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnSystemReservedStorageToggled), 1, e);
                    }

                    return isSystemReservedStorageEnabled;
                });
                IsSystemReservedStorageLoadingOrUpdating = false;
            }
        }

        /// <summary>
        /// 重建图标缓存
        /// </summary>
        private async void OnRebuildIconCacheClicked(object sender, RoutedEventArgs args)
        {
            if (!IsRebuildingIconCache)
            {
                IsRebuildingIconCache = true;
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

                        string iconCacheDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IconCache.db");
                        if (File.Exists(iconCacheDbFile))
                        {
                            File.Delete(iconCacheDbFile);
                        }
                        string explorerFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Explorer"));
                        foreach (FileInfo fileInfo in from file in new DirectoryInfo(explorerFolder).EnumerateFiles() where file.Name.Contains("iconcache") || file.Name.Contains("thumbcache") select file)
                        {
                            fileInfo.Delete();
                        }
                    }
                    catch (Win32Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRebuildIconCacheClicked), 1, e);
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRebuildIconCacheClicked), 2, e);
                        }
                    }
                });
                IsRebuildingIconCache = false;
            }
        }

        /// <summary>
        /// 了解基于虚拟化的安全
        /// </summary>
        private void OnLearnVBSClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows-hardware/design/device-experiences/oem-vbs");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnLearnVBSClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 修改基于虚拟化的安全状态
        /// </summary>
        private async void OnVirtualizationBasedSecurityToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsVirtualizationBasedSecurityEnabled, toggleSwitch.IsOn))
            {
                IsVirtualizationBasedSecurityEnabled = toggleSwitch.IsOn;
                IsVirtualizationBasedSecurityEnabled = await Task.Run(() =>
                {
                    bool isVirtualizationBasedSecurityEnabled = false;

                    try
                    {
                        if (RuntimeHelper.IsElevated)
                        {
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", IsVirtualizationBasedSecurityEnabled);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard", "EnableVirtualizationBasedSecurity", IsVirtualizationBasedSecurityEnabled);
                            Process bcdeditSetProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "bcdedit.exe",
                                Arguments = "/set hypervisorlaunchtype" + " " + (IsVirtualizationBasedSecurityEnabled ? "auto" : "off"),
                                Verb = "open",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            });
                            bcdeditSetProcess.WaitForExit();
                            bcdeditSetProcess.Dispose();
                        }

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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnVirtualizationBasedSecurityToggled), 1, e);
                    }

                    return isVirtualizationBasedSecurityEnabled;
                });
                AdvancedSettingsInfoSeverity = InfoBarSeverity.Warning;
                AdvancedSettingsOperationText = NeedRestartString;
                IsRestartPCVisible = true;
            }
        }

        /// <summary>
        /// 了解网卡负载
        /// </summary>
        private void OnLearnNICOffloadClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/windows-hardware/drivers/network/using-registry-values-to-enable-and-disable-task-offloading");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnLearnVBSClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 修改网卡负载状态
        /// </summary>
        private async void OnNICOffloadSettingsToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsNICOffloadSettingsEnabled, toggleSwitch.IsOn))
            {
                IsNICOffloadSettingsEnabled = toggleSwitch.IsOn;
                IsNICOffloadSettingsEnabled = await Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload", IsNICOffloadSettingsEnabled ? 0 : 1);
                    return RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload") is 0;
                });
                AdvancedSettingsInfoSeverity = InfoBarSeverity.Warning;
                AdvancedSettingsOperationText = NeedRestartString;
                IsRestartPCVisible = true;
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
                List<string> wakeUpTaskList = await Task.Run(() =>
                {
                    DisableAllWakeUpRunTask();
                    return GetWaskUpTaskList();
                });
                WakeUpTaskCollection.Clear();
                foreach (string wakeUpTask in wakeUpTaskList)
                {
                    WakeUpTaskCollection.Add(wakeUpTask);
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
                                string[] deviceArray = GetDevices(new Guid("1CA05180-A699-450A-9A0C-DE4FBE3DDD89"), CM_GET_DEVICE_INTERFACE_LIST_FLAGS.CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
                                DEVPROPKEY devPropKey = new()
                                {
                                    fmtid = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"),
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
                                StartInfo = new ProcessStartInfo
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRestartGraphicsDriverClicked), 1, e);
                    }
                });
                IsRestartingGraphicsDriver = false;
            }
        }

        #endregion 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 获取选中的通知模式
        /// </summary>
        private Visibility GetSelectedNotifyMode(object selectedValue, object comparedValue)
        {
            return Equals(selectedValue, comparedValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取持续唤醒任务列表
        /// </summary>
        public static List<string> GetWaskUpTaskList()
        {
            List<string> wakeUpTaskList = [];
            try
            {
                ITaskService service = (ITaskService)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
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
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(GetWaskUpTaskList), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(GetWaskUpTaskList), 2, e);
            }

            return wakeUpTaskList;
        }

        /// <summary>
        /// 禁用所有持续唤醒任务
        /// </summary>
        private static void DisableAllWakeUpRunTask()
        {
            try
            {
                ITaskService taskService = (ITaskService)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(DisableAllWakeUpRunTask), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(DisableAllWakeUpRunTask), 2, e);
            }
        }

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
                identifier = text.Substring(4, text.Length - 4);
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
    }
}
