using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 系统信息页面
    /// </summary>
    public sealed partial class SystemInformationPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;
        private readonly string AvailablePhysicalMemoryString = ResourceService.SystemInformationResource.GetString("AvailablePhysicalMemory");
        private readonly string AvailableVirtualMemoryString = ResourceService.SystemInformationResource.GetString("AvailableVirtualMemory");
        private readonly string BIOSModeString = ResourceService.SystemInformationResource.GetString("BIOSMode");
        private readonly string BIOSVersionString = ResourceService.SystemInformationResource.GetString("BIOSVersion");
        private readonly string BootDeviceString = ResourceService.SystemInformationResource.GetString("BootDevice");
        private readonly string ErrorInformationString = ResourceService.SystemInformationResource.GetString("ErrorInformation");
        private readonly string EmbeddedControllerVersionString = ResourceService.SystemInformationResource.GetString("EmbeddedControllerVersion");
        private readonly string FirstInstalledDateString = ResourceService.SystemInformationResource.GetString("FirstInstalledDate");
        private readonly string InstalledPhysicalMemoryString = ResourceService.SystemInformationResource.GetString("InstalledPhysicalMemory");
        private readonly string HostNameString = ResourceService.SystemInformationResource.GetString("HostName");
        private readonly string MainboardManufacturerString = ResourceService.SystemInformationResource.GetString("MainboardManufacturer");
        private readonly string MainboardProductString = ResourceService.SystemInformationResource.GetString("MainboardProduct");
        private readonly string MainboardVersionString = ResourceService.SystemInformationResource.GetString("MainboardVersion");
        private readonly string OperatingSystemManufacturerString = ResourceService.SystemInformationResource.GetString("OperatingSystemManufacturer");
        private readonly string OperatingSystemNameString = ResourceService.SystemInformationResource.GetString("OperatingSystemName");
        private readonly string OperatingSystemVersionString = ResourceService.SystemInformationResource.GetString("OperatingSystemVersion");
        private readonly string ProductIDString = ResourceService.SystemInformationResource.GetString("ProductID");
        private readonly string PageFilePositionString = ResourceService.SystemInformationResource.GetString("PageFilePosition");
        private readonly string PageFileSpaceString = ResourceService.SystemInformationResource.GetString("PageFileSpace");
        private readonly string ProcessorString = ResourceService.SystemInformationResource.GetString("Processor");
        private readonly string RegionSettingsString = ResourceService.SystemInformationResource.GetString("RegionSettings");
        private readonly string SMBIOSVersionString = ResourceService.SystemInformationResource.GetString("SMBIOSVersion");
        private readonly string SystemArchitectureString = ResourceService.SystemInformationResource.GetString("SystemArchitecture");
        private readonly string SystemBootTimeString = ResourceService.SystemInformationResource.GetString("SystemBootTime");
        private readonly string SystemManufacturerString = ResourceService.SystemInformationResource.GetString("SystemManufacturer");
        private readonly string SystemDirectoryString = ResourceService.SystemInformationResource.GetString("SystemDirectory");
        private readonly string SystemModelString = ResourceService.SystemInformationResource.GetString("SystemModel");
        private readonly string SystemSKUString = ResourceService.SystemInformationResource.GetString("SystemSKU");
        private readonly string TimeZoneString = ResourceService.SystemInformationResource.GetString("TimeZone");
        private readonly string TotalPhysicalMemoryString = ResourceService.SystemInformationResource.GetString("TotalPhysicalMemory");
        private readonly string TotalVirtualMemoryString = ResourceService.SystemInformationResource.GetString("TotalVirtualMemory");
        private readonly string WindowsDirectoryString = ResourceService.SystemInformationResource.GetString("WindowsDirectory");
        private readonly string UnknownString = ResourceService.SystemInformationResource.GetString("Unknown");
        private SystemInformation systemInformation;

        private SystemInformationResultKind _systemInformationResultKind;

        public SystemInformationResultKind SystemInformationResultKind
        {
            get { return _systemInformationResultKind; }

            set
            {
                if (!Equals(_systemInformationResultKind, value))
                {
                    _systemInformationResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemInformationResultKind)));
                }
            }
        }

        private string _systemInformationFailedContent;

        public string SystemInformationFailedContent
        {
            get { return _systemInformationFailedContent; }

            set
            {
                if (!Equals(_systemInformationFailedContent, value))
                {
                    _systemInformationFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemInformationFailedContent)));
                }
            }
        }

        public WinRTObservableCollection<SystemInformationModel> SystemInformationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SystemInformationPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                SystemInformationResultKind = SystemInformationResultKind.Loading;
                (bool result, SystemInformation gettedSystemInformation, Exception exception) = await GetSystemInformationAsync();

                if (result)
                {
                    systemInformation = gettedSystemInformation;
                    SystemInformationCollection.Clear();
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = HostNameString, Content = systemInformation.HostName });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = OperatingSystemNameString, Content = systemInformation.OperatingSystemName });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = OperatingSystemVersionString, Content = systemInformation.OperatingSystemVersion });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = OperatingSystemManufacturerString, Content = systemInformation.OperatingSystemManufacturer });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = ProductIDString, Content = systemInformation.ProductID });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = FirstInstalledDateString, Content = systemInformation.FirstInstalledDate });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemBootTimeString, Content = systemInformation.SystemBootTime });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemManufacturerString, Content = systemInformation.SystemManufacturer });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemModelString, Content = systemInformation.SystemModel });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemArchitectureString, Content = systemInformation.SystemArchitecture });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemSKUString, Content = systemInformation.SystemSKU });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = ProcessorString, Content = systemInformation.Processor });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = BIOSVersionString, Content = systemInformation.BIOSVersion });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SMBIOSVersionString, Content = systemInformation.SMBIOSVersion });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = EmbeddedControllerVersionString, Content = systemInformation.EmbeddedControllerVersion });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = BIOSModeString, Content = systemInformation.BIOSMode });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = MainboardManufacturerString, Content = systemInformation.MainboardManufacturer });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = MainboardProductString, Content = systemInformation.MainboardProduct });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = MainboardVersionString, Content = systemInformation.MainboardVersion });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = WindowsDirectoryString, Content = systemInformation.WindowsDirectory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemDirectoryString, Content = systemInformation.SystemDirectory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = BootDeviceString, Content = systemInformation.BootDevice });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = RegionSettingsString, Content = systemInformation.RegionSettings });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = TimeZoneString, Content = systemInformation.TimeZone });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = InstalledPhysicalMemoryString, Content = systemInformation.InstalledPhysicalMemory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = TotalPhysicalMemoryString, Content = systemInformation.TotalPhysicalMemory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = AvailablePhysicalMemoryString, Content = systemInformation.AvailablePhysicalMemory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = TotalVirtualMemoryString, Content = systemInformation.TotalVirtualMemory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = AvailableVirtualMemoryString, Content = systemInformation.AvailableVirtualMemory });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = PageFilePositionString, Content = systemInformation.PageFilePosition });
                    SystemInformationCollection.Add(new SystemInformationModel() { Item = PageFileSpaceString, Content = systemInformation.PageFileSpace });
                    SystemInformationResultKind = SystemInformationResultKind.Successfully;
                }
                else
                {
                    systemInformation = null;
                    SystemInformationFailedContent = string.Format(ErrorInformationString, string.Format("0x{0:X8}", exception.HResult), exception.Message);
                    SystemInformationResultKind = SystemInformationResultKind.Failed;
                }
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第二部分：系统信息页面——挂载的事件

        /// <summary>
        /// 复制系统信息
        /// </summary>
        private async void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            if (SystemInformationResultKind is SystemInformationResultKind.Successfully)
            {
                string copySystemInformationContent = await Task.Run(() =>
                {
                    StringBuilder systemInformationBuilder = new();

                    foreach (SystemInformationModel systemInformationItem in SystemInformationCollection)
                    {
                        systemInformationBuilder.AppendLine(string.Format("{0}\t{1}", systemInformationItem.Item, systemInformationItem.Content));
                    }

                    return systemInformationBuilder.ToString();
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(copySystemInformationContent);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 刷新系统信息
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            SystemInformationResultKind = SystemInformationResultKind.Loading;
            (bool result, SystemInformation gettedSystemInformation, Exception exception) = await GetSystemInformationAsync();

            if (result)
            {
                systemInformation = gettedSystemInformation;
                SystemInformationCollection.Clear();
                SystemInformationCollection.Add(new SystemInformationModel() { Item = HostNameString, Content = systemInformation.HostName });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = OperatingSystemNameString, Content = systemInformation.OperatingSystemName });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = OperatingSystemVersionString, Content = systemInformation.OperatingSystemVersion });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = OperatingSystemManufacturerString, Content = systemInformation.OperatingSystemManufacturer });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = ProductIDString, Content = systemInformation.ProductID });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = FirstInstalledDateString, Content = systemInformation.FirstInstalledDate });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemBootTimeString, Content = systemInformation.SystemBootTime });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemManufacturerString, Content = systemInformation.SystemManufacturer });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemModelString, Content = systemInformation.SystemModel });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemArchitectureString, Content = systemInformation.SystemArchitecture });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemSKUString, Content = systemInformation.SystemSKU });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = ProcessorString, Content = systemInformation.Processor });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = BIOSVersionString, Content = systemInformation.BIOSVersion });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SMBIOSVersionString, Content = systemInformation.SMBIOSVersion });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = EmbeddedControllerVersionString, Content = systemInformation.EmbeddedControllerVersion });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = BIOSModeString, Content = systemInformation.BIOSMode });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = MainboardManufacturerString, Content = systemInformation.MainboardManufacturer });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = MainboardProductString, Content = systemInformation.MainboardProduct });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = MainboardVersionString, Content = systemInformation.MainboardVersion });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = WindowsDirectoryString, Content = systemInformation.WindowsDirectory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = SystemDirectoryString, Content = systemInformation.SystemDirectory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = BootDeviceString, Content = systemInformation.BootDevice });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = RegionSettingsString, Content = systemInformation.RegionSettings });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = TimeZoneString, Content = systemInformation.TimeZone });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = InstalledPhysicalMemoryString, Content = systemInformation.InstalledPhysicalMemory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = TotalPhysicalMemoryString, Content = systemInformation.TotalPhysicalMemory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = AvailablePhysicalMemoryString, Content = systemInformation.AvailablePhysicalMemory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = TotalVirtualMemoryString, Content = systemInformation.TotalVirtualMemory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = AvailableVirtualMemoryString, Content = systemInformation.AvailableVirtualMemory });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = PageFilePositionString, Content = systemInformation.PageFilePosition });
                SystemInformationCollection.Add(new SystemInformationModel() { Item = PageFileSpaceString, Content = systemInformation.PageFileSpace });
                SystemInformationResultKind = SystemInformationResultKind.Successfully;
            }
            else
            {
                systemInformation = null;
                SystemInformationFailedContent = string.Format(ErrorInformationString, string.Format("0x{0:X8}", exception.HResult), exception.Message);
                SystemInformationResultKind = SystemInformationResultKind.Failed;
            }
        }

        /// <summary>
        /// 打开 Windows 信息
        /// </summary>
        private void OnWindowsInformationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:about");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SystemInformationPage), nameof(OnWindowsInformationClicked), 2, e);
                }
            });
        }

        /// <summary>
        /// 打开 Windows 版本
        /// </summary>
        private void OnWindowsVersionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("winver.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SystemInformationPage), nameof(OnWindowsVersionClicked), 2, e);
                }
            });
        }

        #endregion 第二部分：系统信息页面——挂载的事件

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        private async Task<(bool, SystemInformation, Exception)> GetSystemInformationAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    SystemInformation systemInformation = new()
                    {
                        HostName = Environment.MachineName
                    };

                    ManagementObjectSearcher osNameSearcher = new("SELECT Caption FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in osNameSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.OperatingSystemName = managementObject["Caption"]?.ToString() ?? string.Empty;
                    }
                    osNameSearcher.Dispose();

                    systemInformation.OperatingSystemVersion = RuntimeInformation.OSDescription;

                    ManagementObjectSearcher osManufacturerSearcher = new("SELECT Manufacturer FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in osManufacturerSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.OperatingSystemManufacturer = managementObject["Manufacturer"]?.ToString() ?? string.Empty;
                    }
                    osManufacturerSearcher.Dispose();

                    ManagementObjectSearcher productIDSearcher = new("SELECT SerialNumber FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in productIDSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.ProductID = managementObject["SerialNumber"]?.ToString() ?? string.Empty;
                    }
                    productIDSearcher.Dispose();

                    ManagementObjectSearcher firstInstalledDateSearcher = new("SELECT InstallDate FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in firstInstalledDateSearcher.Get().Cast<ManagementObject>())
                    {
                        string date = managementObject["InstallDate"]?.ToString();
                        systemInformation.FirstInstalledDate = !string.IsNullOrEmpty(date) ? ManagementDateTimeConverter.ToDateTime(date).ToString() : string.Empty;
                    }
                    firstInstalledDateSearcher.Dispose();

                    ManagementObjectSearcher systemBootTimeSearcher = new("SELECT LastBootUpTime FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in systemBootTimeSearcher.Get().Cast<ManagementObject>())
                    {
                        string lastBootupTime = managementObject["LastBootUpTime"]?.ToString();
                        systemInformation.SystemBootTime = !string.IsNullOrEmpty(lastBootupTime) ? ManagementDateTimeConverter.ToDateTime(lastBootupTime).ToString() : string.Empty;
                    }
                    systemBootTimeSearcher.Dispose();

                    ManagementObjectSearcher systemManufacturerSearcher = new("SELECT Manufacturer FROM Win32_ComputerSystem");
                    foreach (ManagementObject managementObject in systemManufacturerSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.SystemManufacturer = managementObject["Manufacturer"]?.ToString() ?? string.Empty;
                    }
                    systemManufacturerSearcher.Dispose();

                    ManagementObjectSearcher systemModelSearcher = new("SELECT Model FROM Win32_ComputerSystem");
                    foreach (ManagementObject managementObject in systemModelSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.SystemModel = managementObject["Model"]?.ToString() ?? string.Empty;
                    }
                    systemModelSearcher.Dispose();

                    systemInformation.SystemArchitecture = RuntimeInformation.OSArchitecture.ToString();

                    ManagementObjectSearcher systemSKUSearcher = new("SELECT SystemSKUNumber FROM Win32_ComputerSystem");
                    foreach (ManagementObject managementObject in systemSKUSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.SystemSKU = managementObject["SystemSKUNumber"]?.ToString() ?? string.Empty;
                    }
                    systemSKUSearcher.Dispose();

                    ManagementObjectSearcher processorSearcher = new("SELECT Name FROM Win32_Processor");
                    {
                        foreach (ManagementObject managementObject in processorSearcher.Get().Cast<ManagementObject>())
                        {
                            systemInformation.Processor = managementObject["Name"]?.ToString() ?? string.Empty;
                        }
                    }

                    ManagementObjectSearcher biosVersionSearcher = new("SELECT Manufacturer, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");
                    foreach (ManagementObject managementObject in biosVersionSearcher.Get().Cast<ManagementObject>())
                    {
                        string manufacturer = managementObject["Manufacturer"]?.ToString() ?? string.Empty;
                        string version = managementObject["SMBIOSBIOSVersion"]?.ToString() ?? string.Empty;
                        string date = string.Empty;
                        if (managementObject["ReleaseDate"] is not null)
                        {
                            date = ManagementDateTimeConverter.ToDateTime(managementObject["ReleaseDate"].ToString()).ToString("yyyy/MM/dd");
                        }
                        systemInformation.BIOSVersion = string.Format("{0} {1}, {2}", manufacturer, version, date);
                    }
                    biosVersionSearcher.Dispose();

                    ManagementObjectSearcher smbiosVersionSearcher = new("SELECT SMBIOSMajorVersion, SMBIOSMinorVersion FROM Win32_BIOS");
                    foreach (ManagementObject managementObject in smbiosVersionSearcher.Get().Cast<ManagementObject>())
                    {
                        ushort major = (ushort)(managementObject["SMBIOSMajorVersion"] ?? 0);
                        ushort minor = (ushort)(managementObject["SMBIOSMinorVersion"] ?? 0);
                        systemInformation.SMBIOSVersion = string.Format("{0}.{1}", major, minor);
                    }
                    smbiosVersionSearcher.Dispose();

                    ManagementObjectSearcher embeddedControllerSearcher = new("SELECT EmbeddedControllerMajorVersion, EmbeddedControllerMinorVersion FROM Win32_BIOS");
                    foreach (ManagementObject managementObject in embeddedControllerSearcher.Get().Cast<ManagementObject>())
                    {
                        ushort major = Convert.ToUInt16(managementObject["EmbeddedControllerMajorVersion"]);
                        ushort minor = Convert.ToUInt16(managementObject["EmbeddedControllerMinorVersion"]);
                        systemInformation.EmbeddedControllerVersion = string.Format("{0}.{1}", major, minor);
                    }
                    embeddedControllerSearcher.Dispose();

                    if (Kernel32Library.GetFirmwareType(out FIRMWARE_TYPE firmwareType))
                    {
                        systemInformation.BIOSMode = firmwareType switch
                        {
                            FIRMWARE_TYPE.FirmwareTypeUnknown => string.Empty,
                            FIRMWARE_TYPE.FirmwareTypeBios => "Legacy",
                            FIRMWARE_TYPE.FirmwareTypeUefi => "UEFI",
                            _ => string.Empty
                        };
                    }

                    ManagementObjectSearcher mainboardManufacturerSearcher = new("SELECT Manufacturer FROM Win32_BaseBoard");
                    foreach (ManagementObject managementObject in mainboardManufacturerSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.MainboardManufacturer = managementObject["Manufacturer"]?.ToString() ?? string.Empty;
                    }
                    mainboardManufacturerSearcher.Dispose();

                    ManagementObjectSearcher mainboardProductSearcher = new("SELECT Product FROM Win32_BaseBoard");
                    foreach (ManagementObject managementObject in mainboardProductSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.MainboardProduct = managementObject["Product"]?.ToString() ?? string.Empty;
                    }
                    mainboardProductSearcher.Dispose();

                    ManagementObjectSearcher mainboardVersionSearcher = new("SELECT Version FROM Win32_BaseBoard");
                    foreach (ManagementObject managementObject in mainboardVersionSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.MainboardVersion = managementObject["Version"]?.ToString() ?? string.Empty;
                    }
                    mainboardVersionSearcher.Dispose();

                    systemInformation.WindowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                    systemInformation.SystemDirectory = Environment.SystemDirectory;

                    ManagementObjectSearcher bootDeviceSearcher = new("SELECT BootDevice FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in bootDeviceSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.BootDevice = managementObject["BootDevice"]?.ToString() ?? string.Empty;
                    }
                    bootDeviceSearcher.Dispose();

                    systemInformation.RegionSettings = string.Format("{0};{1}", CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.NativeName);

                    ManagementObjectSearcher timeZoneSearcher = new("SELECT StandardName, Bias FROM Win32_TimeZone");
                    foreach (ManagementObject managementObject in timeZoneSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.TimeZone = managementObject["StandardName"]?.ToString() ?? string.Empty;
                    }
                    timeZoneSearcher.Dispose();

                    systemInformation.InstalledPhysicalMemory = Kernel32Library.GetPhysicallyInstalledSystemMemory(out ulong totalMemorySize) ? VolumeSizeHelper.ConvertVolumeSizeToString(totalMemorySize * 1024) : string.Empty;

                    ManagementObjectSearcher totalMemorySearcher = new("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                    foreach (ManagementObject managementObject in totalMemorySearcher.Get().Cast<ManagementObject>())
                    {
                        ulong totalPhysicalMemorySize = (ulong)managementObject["TotalPhysicalMemory"];
                        systemInformation.TotalPhysicalMemory = VolumeSizeHelper.ConvertVolumeSizeToString(totalPhysicalMemorySize);
                    }
                    totalMemorySearcher.Dispose();

                    MEMORYSTATUSEX memoryStatusEx = new()
                    {
                        dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>()
                    };

                    if (Kernel32Library.GlobalMemoryStatusEx(ref memoryStatusEx))
                    {
                        systemInformation.AvailablePhysicalMemory = VolumeSizeHelper.ConvertVolumeSizeToString(memoryStatusEx.ullAvailPhys);
                        systemInformation.TotalVirtualMemory = VolumeSizeHelper.ConvertVolumeSizeToString(memoryStatusEx.ullTotalPageFile);
                        systemInformation.AvailableVirtualMemory = VolumeSizeHelper.ConvertVolumeSizeToString(memoryStatusEx.ullAvailPageFile);
                    }

                    ManagementObjectSearcher pageFilePositionSearcher = new("SELECT * FROM Win32_PageFileUsage");
                    foreach (ManagementObject managementObject in pageFilePositionSearcher.Get().Cast<ManagementObject>())
                    {
                        systemInformation.PageFilePosition = (string)managementObject["Name"];
                    }
                    pageFilePositionSearcher.Dispose();

                    ManagementObjectSearcher pageFileSpaceSearcher = new("SELECT SizeStoredInPagingFiles FROM Win32_OperatingSystem");
                    foreach (ManagementObject managementObject in pageFileSpaceSearcher.Get().Cast<ManagementObject>())
                    {
                        ulong pageFileSpace = (ulong)managementObject["SizeStoredInPagingFiles"];
                        systemInformation.PageFileSpace = VolumeSizeHelper.ConvertVolumeSizeToString(pageFileSpace * 1024);
                    }
                    pageFileSpaceSearcher.Dispose();

                    // 为未赋值的属性设置未知
                    PropertyInfo[] systemInformationPropertyInfo = systemInformation.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var systemInformationProperty in systemInformationPropertyInfo)
                    {
                        if (systemInformationProperty.PropertyType == typeof(string) && systemInformationProperty.CanRead && systemInformationProperty.CanWrite)
                        {
                            string currentValue = (string)systemInformationProperty.GetValue(systemInformation);
                            if (string.IsNullOrEmpty(currentValue))
                            {
                                systemInformationProperty.SetValue(systemInformation, UnknownString);
                            }
                        }
                    }

                    return ValueTuple.Create<bool, SystemInformation, Exception>(true, systemInformation, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SystemInformationPage), nameof(GetSystemInformationAsync), 1, e);
                    return ValueTuple.Create<bool, SystemInformation, Exception>(true, null, e);
                }
            });
        }

        /// <summary>
        /// 获取加载系统信息是否成功
        /// </summary>
        private Visibility GetSystemInformationSuccessfullyState(SystemInformationResultKind systemInformationResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? systemInformationResultKind is SystemInformationResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : systemInformationResultKind is SystemInformationResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查系统信息是否成功
        /// </summary>
        private Visibility CheckSystemInformationState(SystemInformationResultKind systemInformationResultKind, SystemInformationResultKind comparedSystemInformationResultKind)
        {
            return Equals(systemInformationResultKind, comparedSystemInformationResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
