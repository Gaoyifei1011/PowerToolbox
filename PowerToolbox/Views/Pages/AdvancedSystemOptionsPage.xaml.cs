using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.PowrProf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private readonly int DISM_RESERVED_STORAGE_DISABLED = 0x00000000;
        private readonly int DISM_RESERVED_STORAGE_ENABLED = 0x00000001;
        private readonly string AlwaysNotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("AlwaysNotify");
        private readonly string HibernationFileTypeReducedString = ResourceService.AdvancedSystemOptionsResource.GetString("HibernationFileTypeReduced");
        private readonly string HibernationFileTypeFullString = ResourceService.AdvancedSystemOptionsResource.GetString("HibernationFileTypeFull");
        private readonly string NeverNotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("NeverNotify");
        private readonly string NotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("Notify");
        private readonly string NotifyWithoutDimmingString = ResourceService.AdvancedSystemOptionsResource.GetString("NotifyWithoutDimming");
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private readonly Guid Balance = new("381B4222-F694-41F0-9685-FF5BB260DF2E");
        private readonly Guid EnergySaving = new("A1841308-3541-4FAB-BC81-F71556F20B4A");
        private readonly Guid HighPerformance = new("8C5E7FDA-E8BF-4A96-9A85-A6E23A8C635C");
        private readonly Guid OutstandingPerformance = new("E9A42B02-D5DF-448D-AA00-03F14749EB61");

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

        private List<ComboBoxItemModel> NotifyModeList { get; } = [];

        private List<ComboBoxItemModel> HibernationFileTypeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPage()
        {
            InitializeComponent();
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
        }

        #endregion 第一部分：重载父类事件

        #region 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 打开系统设置
        /// </summary>
        private void OnOpenSystemSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:powersleep");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnOpenSystemSettingsClicked), 1, e);
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
            if (sender is ToggleSwitch toggleSwitch)
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
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsHibernationOpened = toggleSwitch.IsOn;

                SYSTEM_POWER_CAPABILITIES systemPowerCapabilities = await Task.Run(() =>
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
                            powerCfgProcess = Process.Start(new ProcessStartInfo()
                            {
                                FileName = "powercfg.exe",
                                Arguments = "/h /type reduced",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            });
                            powerCfgProcess.WaitForExit();
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
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnHibernationFileTypeSelectionChanged), 2, e);
                        }
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
                    Process powerCfgProcess = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "powercfg.exe",
                        Arguments = string.Format("/h /size {0}", HibernationFilePercent),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    powerCfgProcess.WaitForExit();
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
            if (sender is ToggleSwitch toggleSwitch)
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
        private void OnGenerateBatteryReportClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "powercfg.exe",
                        Arguments = string.Format("/batteryreport /output {0}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "battery-report.html")),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnGenerateBatteryReportClicked), 1, e);
                }
            });
        }

        #endregion 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 获取选中的通知模式
        /// </summary>
        private Visibility GetSelectedNotifyMode(object selectedValue, object comparedValue)
        {
            return string.Equals(selectedValue, comparedValue) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
