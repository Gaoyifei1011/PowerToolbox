using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using PowerToolbox.WindowsAPI.PInvoke.Shlwapi;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项——个性化页面
    /// </summary>
    public sealed partial class AdvancedSystemOptionsPersonalizationPage : Page, INotifyPropertyChanged
    {
        private readonly string controlPanelPathString = "{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}";
        private readonly string networkPathString = "{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}";
        private readonly string recycleBinPathString = "{645FF040-5081-101B-9F08-00AA002F954E}";
        private readonly string thisPCPathString = "{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        private readonly string userFolderPathString = "{59031A47-3F72-44A7-89C5-5595FE6B30EE}";
        private readonly string EmptyString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Empty");
        private readonly string FullString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Full");
        private readonly string UserFolderString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("UserFolder");
        private AdvancedSystemOptionsPage advancedSystemOptionsPage;

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

        private bool _isIconSelected;

        public bool IsIconSelected
        {
            get { return _isIconSelected; }

            set
            {
                if (!Equals(_isIconSelected, value))
                {
                    _isIconSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIconSelected)));
                }
            }
        }

        private DesktopIconSettingsModel _selectedIconItem;

        public DesktopIconSettingsModel SelectedIconItem
        {
            get { return _selectedIconItem; }

            set
            {
                if (!Equals(_selectedIconItem, value))
                {
                    _selectedIconItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIconItem)));
                }
            }
        }

        private WinRTObservableCollection<DesktopIconSettingsModel> DesktopIconSettingsCollection { get; } = [];

        private WinRTObservableCollection<DesktopIconDisplayModel> DesktopIconDisplayCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPersonalizationPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is AdvancedSystemOptionsPage targetPage && !Equals(advancedSystemOptionsPage, targetPage))
            {
                advancedSystemOptionsPage = targetPage;
            }

            if (RuntimeHelper.IsElevated)
            {
                string thisPCIconPath = string.Format("::{0}", thisPCPathString);
                string userFolderIconPath = string.Format("::{0}", userFolderPathString);
                string networkIconPath = string.Format("::{0}", networkPathString);
                string controlPanelIconPath = string.Format("::{0}", controlPanelPathString);
                string recycleBinPath = string.Format("::{0}", recycleBinPathString);

                // 图标在注册表中存储的键
                string thisPCIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", thisPCPathString);
                string userFolderIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", userFolderPathString);
                string networkIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", networkPathString);
                string recycleBinIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", recycleBinPathString);

                // 图标在注册表中存储的值
                string thisPCIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, thisPCIconRegistryKeyPath, string.Empty);
                string userFolderIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, userFolderIconRegistryKeyPath, string.Empty);
                string networkIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, networkIconRegistryKeyPath, string.Empty);
                string recycleBinFullIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, recycleBinIconRegistryKeyPath, "full");
                string recycleBinEmptyIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, recycleBinIconRegistryKeyPath, "empty");

                // 图标显示名称
                string thisPCDisplayName = await GetShellIconDisplayNameAsync(thisPCIconPath);
                string userFolderDisplayName = await GetShellIconDisplayNameAsync(userFolderIconPath);
                string networkDisplayName = await GetShellIconDisplayNameAsync(networkIconPath);
                string controlPanelDisplayName = await GetShellIconDisplayNameAsync(controlPanelIconPath);
                string recycleBinDisplayName = await GetShellIconDisplayNameAsync(recycleBinPath);

                // 图标的位置和索引
                (string thisPCIconLocationPath, int thisPCIconIndex) = await GetShellIconLocationAsync(thisPCIconRegistryValuePath);
                (string userFolderIconLocationPath, int userFolderIconIndex) = await GetShellIconLocationAsync(userFolderIconRegistryValuePath);
                (string networkIconLocationPath, int networkIconIndex) = await GetShellIconLocationAsync(networkIconRegistryValuePath);
                (string recycleBinFullIconLocationPath, int recycleBinFullIconIndex) = await GetShellIconLocationAsync(recycleBinFullIconRegistryValuePath);
                (string recycleBinEmptyIconLocationPath, int recycleBinEmptyIconIndex) = await GetShellIconLocationAsync(recycleBinEmptyIconRegistryValuePath);

                MemoryStream thisPCIconMemoryStream = await GetShellIconAsync(thisPCIconLocationPath, thisPCIconIndex);
                MemoryStream userFolderIconMemoryStream = await GetShellIconAsync(userFolderIconLocationPath, userFolderIconIndex);
                MemoryStream networkIconMemoryStream = await GetShellIconAsync(networkIconLocationPath, networkIconIndex);
                MemoryStream recycleBinFullMemoryStream = await GetShellIconAsync(recycleBinFullIconLocationPath, recycleBinFullIconIndex);
                MemoryStream recycleBinEmptyMemoryStream = await GetShellIconAsync(recycleBinEmptyIconLocationPath, recycleBinEmptyIconIndex);

                DesktopIconSettingsCollection.Clear();
                if (thisPCIconMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(thisPCIconMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new DesktopIconSettingsModel()
                        {
                            IconTag = "ThisPC",
                            IconRegistryKeyPath = thisPCIconRegistryKeyPath,
                            IconLocationPath = thisPCIconLocationPath,
                            IconIndex = thisPCIconIndex,
                            DisplayName = thisPCDisplayName,
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 1, e);
                    }
                }
                if (userFolderIconMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(userFolderIconMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new DesktopIconSettingsModel()
                        {
                            IconTag = "UserFolder",
                            IconRegistryKeyPath = userFolderIconRegistryKeyPath,
                            IconLocationPath = userFolderIconLocationPath,
                            IconIndex = userFolderIconIndex,
                            DisplayName = userFolderDisplayName,
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 2, e);
                    }
                }
                if (networkIconMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(networkIconMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new DesktopIconSettingsModel()
                        {
                            IconTag = "Network",
                            IconRegistryKeyPath = networkIconRegistryKeyPath,
                            IconLocationPath = networkIconLocationPath,
                            IconIndex = networkIconIndex,
                            DisplayName = networkDisplayName,
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 3, e);
                    }
                }
                if (recycleBinFullMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(recycleBinFullMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new DesktopIconSettingsModel()
                        {
                            IconTag = "RecycleBinFull",
                            IconRegistryKeyPath = recycleBinIconRegistryKeyPath,
                            IconLocationPath = recycleBinFullIconLocationPath,
                            IconIndex = recycleBinFullIconIndex,
                            DisplayName = string.Format("{0}{1}", recycleBinDisplayName, FullString),
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 5, e);
                    }
                }
                if (recycleBinEmptyMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(recycleBinEmptyMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new DesktopIconSettingsModel()
                        {
                            IconTag = "RecycleBinEmpty",
                            IconRegistryKeyPath = recycleBinIconRegistryKeyPath,
                            IconLocationPath = recycleBinEmptyIconLocationPath,
                            IconIndex = recycleBinEmptyIconIndex,
                            DisplayName = string.Format("{0}{1}", recycleBinDisplayName, EmptyString),
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 5, e);
                    }
                }

                bool thisPCIconVisible = GetDesktopIconVisibility(thisPCPathString, "ThisPC");
                bool recycleBinIconVisible = GetDesktopIconVisibility(recycleBinPathString, "RecycleBin");
                bool userFolderIconVisible = GetDesktopIconVisibility(userFolderPathString, "UserFolder");
                bool controlPanelIconVisible = GetDesktopIconVisibility(controlPanelPathString, "ControlPanel");
                bool networkIconVisible = GetDesktopIconVisibility(networkPathString, "Network");

                DesktopIconDisplayCollection.Clear();
                DesktopIconDisplayCollection.Add(new DesktopIconDisplayModel()
                {
                    DisplayName = thisPCDisplayName,
                    IconTag = "ThisPC",
                    IsIconVisible = thisPCIconVisible
                });
                DesktopIconDisplayCollection.Add(new DesktopIconDisplayModel()
                {
                    DisplayName = recycleBinDisplayName,
                    IconTag = "RecycleBin",
                    IsIconVisible = recycleBinIconVisible
                });
                DesktopIconDisplayCollection.Add(new DesktopIconDisplayModel()
                {
                    DisplayName = UserFolderString,
                    IconTag = "UserFolder",
                    IsIconVisible = userFolderIconVisible
                });
                DesktopIconDisplayCollection.Add(new DesktopIconDisplayModel()
                {
                    DisplayName = controlPanelDisplayName,
                    IconTag = "ControlPanel",
                    IsIconVisible = controlPanelIconVisible
                });
                DesktopIconDisplayCollection.Add(new DesktopIconDisplayModel()
                {
                    DisplayName = networkDisplayName,
                    IconTag = "Network",
                    IsIconVisible = networkIconVisible
                });
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 修改桌面图标显示状态
        /// </summary>
        private async void OnDesktopIconDisplayExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DesktopIconDisplayModel desktopIconDisplay)
            {
                desktopIconDisplay.IsIconVisible = !desktopIconDisplay.IsIconVisible;
                desktopIconDisplay.IsIconVisible = await Task.Run(() =>
                {
                    bool isIconVisible = false;
                    switch (desktopIconDisplay.IconTag)
                    {
                        case "ThisPC":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", thisPCPathString, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(thisPCPathString, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "RecycleBin":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", recycleBinPathString, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(recycleBinPathString, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "UserFolder":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", userFolderPathString, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(userFolderPathString, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "ControlPanel":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", controlPanelPathString, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(controlPanelPathString, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "Network":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", networkPathString, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(networkPathString, desktopIconDisplay.IconTag);
                                break;
                            }
                    }
                    Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);
                    return isIconVisible;
                });
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：高级系统选项——个性化页面——挂载的事件

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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnRebuildIconCacheClicked), 1, e);
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnRebuildIconCacheClicked), 2, e);
                        }
                    }
                });
                IsRebuildingIconCache = false;
            }
        }

        /// <summary>
        /// 修改桌面图标
        /// </summary>
        private async void OnChangeDesktopIconClicked(object sender, RoutedEventArgs args)
        {
            if (DesktopIconSettingsGridView.SelectedItem is DesktopIconSettingsModel desktopIconSettings)
            {
                StringBuilder desktopIconStringBuilder = new(desktopIconSettings.IconLocationPath, 260);
                int desktopIconIndex = desktopIconSettings.IconIndex;
                string iconRegistryValuePath = string.Empty;
                if (Shell32Library.PickIconDlg((nint)MainWindow.Current.AppWindow.Id.Value, desktopIconStringBuilder, desktopIconStringBuilder.Capacity, ref desktopIconIndex))
                {
                    await Task.Run(() =>
                    {
                        switch (desktopIconSettings.IconTag)
                        {
                            case "ThisPC":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                    break;
                                }
                            case "UserFolder":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                    break;
                                }
                            case "Network":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                    break;
                                }
                            case "RecycleBinFull":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full", string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full");
                                    break;
                                }
                            case "RecycleBinEmpty":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty", string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty");
                                    break;
                                }
                        }
                        Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);
                    });

                    if (!string.IsNullOrEmpty(iconRegistryValuePath))
                    {
                        (string iconLocationPath, int iconIndex) = await GetShellIconLocationAsync(iconRegistryValuePath);
                        MemoryStream iconMemoryStream = await GetShellIconAsync(iconLocationPath, iconIndex);
                        if (iconMemoryStream is not null)
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(iconMemoryStream.AsRandomAccessStream());
                                desktopIconSettings.IconImage = bitmapImage;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnChangeDesktopIconClicked), 1, e);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 还原默认图标
        /// </summary>
        private async void OnRestoreDefualtIconClicked(object sender, RoutedEventArgs args)
        {
            if (DesktopIconSettingsGridView.SelectedItem is DesktopIconSettingsModel desktopIconSettings)
            {
                string iconRegistryValuePath = string.Empty;
                await Task.Run(() =>
                {
                    switch (desktopIconSettings.IconTag)
                    {
                        case "ThisPC":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, @"%SystemRoot%\System32\imageres.dll,-109");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                break;
                            }
                        case "UserFolder":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, @"%SystemRoot%\System32\imageres.dll,-123");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                break;
                            }
                        case "Network":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, @"%SystemRoot%\System32\imageres.dll,-25");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                break;
                            }
                        case "RecycleBinFull":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full", @"%SystemRoot%\System32\imageres.dll,-54");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full");
                                break;
                            }
                        case "RecycleBinEmpty":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty", @"%SystemRoot%\System32\imageres.dll,-55");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty");
                                break;
                            }
                    }
                    Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);
                });

                if (!string.IsNullOrEmpty(iconRegistryValuePath))
                {
                    (string iconLocationPath, int iconIndex) = await GetShellIconLocationAsync(iconRegistryValuePath);
                    MemoryStream iconMemoryStream = await GetShellIconAsync(iconLocationPath, iconIndex);
                    if (iconMemoryStream is not null)
                    {
                        try
                        {
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(iconMemoryStream.AsRandomAccessStream());
                            desktopIconSettings.IconImage = bitmapImage;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnRestoreDefualtIconClicked), 1, e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 图标选中项发生更改时触发的事件
        /// </summary>
        private void OnIconSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is GridView gridView && !Equals(SelectedIconItem, gridView.SelectedItem))
            {
                SelectedIconItem = gridView.SelectedItem is DesktopIconSettingsModel desktopIconSettingsItem ? desktopIconSettingsItem : null;
            }
            IsIconSelected = SelectedIconItem is not null;
        }

        #endregion 第三部分：高级系统选项——个性化页面——挂载的事件

        /// <summary>
        /// 获取图标存储位置和索引
        /// </summary>
        private async Task<(string, int)> GetShellIconLocationAsync(string iconStoragePath)
        {
            string iconPath = string.Empty;
            int iconIndex = 0;
            await Task.Run(() =>
            {
                try
                {
                    StringBuilder stringBuilder = new(iconStoragePath);
                    iconIndex = ShlwapiLibrary.PathParseIconLocation(stringBuilder);
                    iconPath = stringBuilder.ToString();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetShellIconLocationAsync), 1, e);
                }
            });
            return ValueTuple.Create(iconPath, iconIndex);
        }

        /// <summary>
        /// 获取图标对应的图标资源
        /// </summary>
        private async Task<MemoryStream> GetShellIconAsync(string iconStoragePath, int iconIndex)
        {
            return await Task.Run(() =>
            {
                MemoryStream memoryStream = null;

                try
                {
                    nint[] phicon = new nint[1];
                    int[] piconid = new int[1];
                    User32Library.PrivateExtractIcons(iconStoragePath, iconIndex, 48, 48, phicon, piconid, 1, 0);
                    Icon icon = Icon.FromHandle(phicon[0]);
                    memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetShellIconAsync), 1, e);
                }

                return memoryStream;
            });
        }

        /// <summary>
        /// 获取图标对应的显示名称
        /// </summary>
        private async Task<string> GetShellIconDisplayNameAsync(string iconPathName)
        {
            string displayName = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    if (Shell32Library.SHCreateItemFromParsingName(iconPathName, null, typeof(IShellItem).GUID, out IShellItem shellItem) is 0)
                    {
                        shellItem.GetDisplayName(SIGDN.SIGDN_NORMALDISPLAY, out displayName);
                        Marshal.ReleaseComObject(shellItem);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetShellIconDisplayNameAsync), 1, e);
                }
            });

            return displayName;
        }

        /// <summary>
        /// 获取桌面图标显示状态
        /// </summary>
        private bool GetDesktopIconVisibility(string iconPathName, string iconTag)
        {
            int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", iconPathName);
            bool visible = false;
            switch (iconTag)
            {
                case "ThisPC":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "RecycleBin":
                    {
                        visible = !iconValue.HasValue || iconValue.Value is 0;
                        break;
                    }
                case "UserFolder":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "ControlPanel":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "Network":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
            }
            return visible;
        }
    }
}
