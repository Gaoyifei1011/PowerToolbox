using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.Collections;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.FirewallAPI;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using PowerToolbox.WindowsAPI.PInvoke.Shlwapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

// 抑制 CA18062，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 右键菜单管理页面
    /// </summary>
    public sealed partial class ContextMenuManagerPage : Page, INotifyPropertyChanged
    {
        private const string packageComPackageKey = @"SOFTWARE\Classes\PackagedCom\Package";
        private const string blockedKey = @"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        private readonly string MenuEmptyDescriptionString = ResourceService.ContextMenuManagerResource.GetString("MenuEmptyDescription");
        private readonly string MenuEmptyWithConditionDescriptionString = ResourceService.ContextMenuManagerResource.GetString("MenuEmptyWithConditionDescription");
        private readonly BitmapImage emptyImage = new();
        private bool isInitialized;

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private ContextMenuResultKind _contextMenuResultKind;

        public ContextMenuResultKind ContextMenuResultKind
        {
            get { return _contextMenuResultKind; }

            set
            {
                if (!Equals(_contextMenuResultKind, value))
                {
                    _contextMenuResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContextMenuResultKind)));
                }
            }
        }

        private string _contextMenuFailedContent;

        public string ContextMenuFailedContent
        {
            get { return _contextMenuFailedContent; }

            set
            {
                if (!string.Equals(_contextMenuFailedContent, value))
                {
                    _contextMenuFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContextMenuFailedContent)));
                }
            }
        }

        private List<ContextMenuModel> ContextMenuList { get; } = [];

        private WinRTObservableCollection<ContextMenuModel> ContextMenuCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ContextMenuManagerPage()
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

            if (!isInitialized)
            {
                isInitialized = true;
                await GetContextMenuAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 打开应用包路径
        /// </summary>
        private void OnOpenPackagePathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string path)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(path);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OnOpenPackagePathExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 点击复选框时使保存按钮处于可选状态
        /// </summary>
        private async void OnCheckBoxClickExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ContextMenuItemModel contextMenu)
            {
                (BlockedClsidType blockedClsidType, bool operationResult) result = await Task.Run(() =>
                {
                    RegistryKey registryKey = null;
                    BlockedClsidType blockedClsidType = BlockedClsidType.Unknown;
                    if (contextMenu.BlockedClsidType is BlockedClsidType.LocalMachine)
                    {
                        try
                        {
                            registryKey = Registry.LocalMachine.CreateSubKey(blockedKey, true);
                            blockedClsidType = BlockedClsidType.LocalMachine;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OnCheckBoxClickExecuteRequested), 1, e);
                        }
                    }
                    else
                    {
                        try
                        {
                            registryKey = Registry.CurrentUser.CreateSubKey(blockedKey, true);
                            blockedClsidType = BlockedClsidType.CurrentUser;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OnCheckBoxClickExecuteRequested), 2, e);
                        }
                    }

                    if (registryKey is not null)
                    {
                        try
                        {
                            string name = contextMenu.Clsid.ToString("B").ToUpperInvariant();
                            if (contextMenu.IsEnabled)
                            {
                                object oldValue = registryKey.GetValue(name);
                                if (oldValue is null)
                                {
                                    registryKey.SetValue(name, "Blocked by PowerToolbox");
                                    registryKey.Close();
                                    registryKey.Dispose();
                                }
                                return ValueTuple.Create(blockedClsidType, true);
                            }
                            else
                            {
                                registryKey.DeleteValue(name);
                                registryKey.Close();
                                registryKey.Dispose();
                                return ValueTuple.Create(blockedClsidType, true);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OnCheckBoxClickExecuteRequested), 3, e);
                            registryKey.Close();
                            registryKey.Dispose();
                            return ValueTuple.Create(blockedClsidType, false);
                        }
                    }
                    else
                    {
                        registryKey.Close();
                        registryKey.Dispose();
                        return ValueTuple.Create(blockedClsidType, true);
                    }
                });

                if (result.operationResult)
                {
                    contextMenu.IsEnabled = !contextMenu.IsEnabled;
                    contextMenu.BlockedClsidType = IsEnabled ? BlockedClsidType.Unknown : result.blockedClsidType;
                }
                else
                {
                    contextMenu.IsEnabled = contextMenu.IsEnabled;
                }

                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ContextMenuUpdate, result.operationResult));
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：右键菜单管理页面——挂载的事件

        /// <summary>
        /// 点击关闭按钮关闭使用说明
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (ContextMenuSplitView.IsPaneOpen)
            {
                ContextMenuSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnOpenSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && ContextMenuResultKind is not ContextMenuResultKind.Loading && ContextMenuList.Count > 0)
            {
                ContextMenuResultKind = ContextMenuResultKind.Loading;
                ContextMenuCollection.Clear();
                foreach (ContextMenuModel contextMenuItem in ContextMenuList)
                {
                    if (!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }
                }

                ContextMenuResultKind = ContextMenuCollection.Count is 0 ? ContextMenuResultKind.Failed : ContextMenuResultKind.Successfully;
                ContextMenuFailedContent = ContextMenuCollection.Count is 0 ? MenuEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 搜索应用名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && ContextMenuResultKind is not ContextMenuResultKind.Loading && ContextMenuList.Count > 0)
            {
                ContextMenuResultKind = ContextMenuResultKind.Loading;
                ContextMenuCollection.Clear();
                foreach (ContextMenuModel contextMenuItem in ContextMenuList)
                {
                    ContextMenuCollection.Add(contextMenuItem);
                }

                ContextMenuResultKind = ContextMenuCollection.Count is 0 ? ContextMenuResultKind.Failed : ContextMenuResultKind.Successfully;
                ContextMenuFailedContent = ContextMenuCollection.Count is 0 ? MenuEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetContextMenuAsync();
        }

        /// <summary>
        /// 恢复默认
        /// </summary>
        private async void OnRestoreDefaultClicked(object sender, RoutedEventArgs args)
        {
            ContextMenuResultKind = ContextMenuResultKind.Loading;
            SearchText = string.Empty;

            await Task.Run(() =>
            {
                List<ContextMenuModel> contextMenuList = [.. ContextMenuList];

                try
                {
                    RegistryKey blockKey = Registry.LocalMachine.OpenSubKey(blockedKey, true);
                    string[] blcokedClsidArray = blockKey.GetValueNames();
                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (Convert.ToString(blockKey.GetValue(blockedClsid)) is "Blocked by PowerToolbox")
                        {
                            blockKey.DeleteValue(blockedClsid);
                        }
                    }

                    blockKey.Close();
                    blockKey.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OnRestoreDefaultClicked), 1, e);
                }

                try
                {
                    RegistryKey blockKey = Registry.CurrentUser.OpenSubKey(blockedKey, true);
                    string[] blcokedClsidArray = blockKey.GetValueNames();
                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (Convert.ToString(blockKey.GetValue(blockedClsid)) is "Blocked by PowerToolbox")
                        {
                            blockKey.DeleteValue(blockedClsid);
                        }
                    }

                    blockKey.Close();
                    blockKey.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OnRestoreDefaultClicked), 2, e);
                }
            });

            await GetContextMenuAsync();
        }

        /// <summary>
        /// 打开使用说明
        /// </summary>
        private async void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            await Task.Delay(300);
            if (!ContextMenuSplitView.IsPaneOpen)
            {
                ContextMenuSplitView.IsPaneOpen = true;
            }
        }

        #endregion 第三部分：右键菜单管理页面——挂载的事件

        /// <summary>
        /// 获取右键菜单数据
        /// </summary>
        private async Task GetContextMenuAsync()
        {
            ContextMenuResultKind = ContextMenuResultKind.Loading;
            ContextMenuList.Clear();
            ContextMenuCollection.Clear();

            List<ContextMenuModel> queryedContextMenuList = await Task.Run(() =>
            {
                List<ContextMenuModel> queryedContextMenuList = [];
                List<KeyValuePair<Guid, string>> blockedList = GetBlockedClsidList();
                List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetAppContainerList();

                try
                {
                    RegistryKey packageListKey = Registry.LocalMachine.OpenSubKey(packageComPackageKey, false);

                    if (packageListKey is not null)
                    {
                        string[] packageFullNameArray = packageListKey.GetSubKeyNames();
                        int length = 0;
                        string currentPackageFullName = string.Empty;

                        if (Kernel32Library.GetCurrentPackageFullName(ref length, null) is 122)
                        {
                            StringBuilder packageFullNameBuilder = new(length + 1);
                            Kernel32Library.GetCurrentPackageFullName(ref length, packageFullNameBuilder);
                            currentPackageFullName = Convert.ToString(packageFullNameBuilder);
                        }

                        foreach (string packageFullName in packageFullNameArray)
                        {
                            if (string.Equals(packageFullName, currentPackageFullName, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            RegistryKey classKey = packageListKey.OpenSubKey(string.Join(@"\", packageFullName, "Class"));

                            if (classKey is not null)
                            {
                                string[] clsidArray = classKey.GetSubKeyNames();
                                List<ContextMenuItemModel> contextMenuItemList = [];

                                foreach (string clsidString in clsidArray)
                                {
                                    if (Guid.TryParse(clsidString, out Guid clsid))
                                    {
                                        RegistryKey clsidKey = classKey.OpenSubKey(clsidString, false);

                                        if (clsidKey is not null)
                                        {
                                            string dllPath = Convert.ToString(clsidKey.GetValue("DllPath", string.Empty));

                                            if (!string.IsNullOrEmpty(dllPath) && dllPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                                            {
                                                int serverId = Convert.ToInt32(clsidKey.GetValue("ServerId", 0));
                                                int threading = Convert.ToInt32(clsidKey.GetValue("Threading", 0));
                                                int index = blockedList.FindIndex(item => Equals(item.Key, clsid));

                                                contextMenuItemList.Add(new ContextMenuItemModel()
                                                {
                                                    BlockedClsidType = index >= 0 && index < blockedList.Count ? Enum.TryParse(blockedList[index].Value, out BlockedClsidType blockedClsidType) ? blockedClsidType : BlockedClsidType.Unknown : BlockedClsidType.Unknown,
                                                    Clsid = clsid,
                                                    ClsidString = Convert.ToString(clsid).ToUpperInvariant(),
                                                    DllPath = dllPath,
                                                    IsEnabled = index is -1,
                                                    ThreadingMode = threading switch
                                                    {
                                                        0 => ApartmentState.STA,
                                                        1 => ApartmentState.MTA,
                                                        _ => ApartmentState.Unknown,
                                                    }
                                                });
                                            }
                                        }

                                        clsidKey.Close();
                                        clsidKey.Dispose();
                                    }
                                }

                                length = 0;
                                string packagePath = string.Empty;

                                if (Kernel32Library.GetPackagePathByFullName(packageFullName, ref length, null) is 122)
                                {
                                    StringBuilder packagePathBuilder = new(length + 1);
                                    int result = Kernel32Library.GetPackagePathByFullName(packageFullName, ref length, packagePathBuilder);
                                    packagePath = Convert.ToString(packagePathBuilder);
                                }

                                (string displayName, string logoFullPath, List<Guid> clsidList) = GetAppInfo(packagePath);

                                if (clsidList.Count > 0)
                                {
                                    StringBuilder displayNameBuilder = new(1024);
                                    foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList)
                                    {
                                        if (inetContainerItem.displayName.Contains(packageFullName))
                                        {
                                            ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.displayName, displayNameBuilder, displayNameBuilder.Capacity, IntPtr.Zero);
                                        }
                                    }

                                    ContextMenuModel contextMenu = new()
                                    {
                                        PackageDisplayName = string.IsNullOrEmpty(Convert.ToString(displayNameBuilder)) ? displayName : Convert.ToString(displayNameBuilder),
                                        PackageFullName = packageFullName,
                                        PackageIconUri = Uri.TryCreate(logoFullPath, UriKind.Absolute, out Uri uri) ? uri : null,
                                        PackagePath = packagePath,
                                        ContextMenuItemCollection = [.. contextMenuItemList]
                                    };

                                    queryedContextMenuList.Add(contextMenu);
                                }

                                classKey.Close();
                                classKey.Dispose();
                            }
                        }

                        packageListKey.Close();
                        packageListKey.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetContextMenuAsync), 1, e);
                }

                return queryedContextMenuList;
            });

            ContextMenuList.AddRange(queryedContextMenuList);

            if (ContextMenuList.Count is 0)
            {
                ContextMenuResultKind = ContextMenuResultKind.Failed;
                ContextMenuFailedContent = MenuEmptyDescriptionString;
            }
            else
            {
                foreach (ContextMenuModel contextMenuItem in ContextMenuList)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        if (contextMenuItem.PackageIconUri is not null)
                        {
                            bitmapImage.UriSource = contextMenuItem.PackageIconUri;
                        }
                        contextMenuItem.PackageIcon = bitmapImage;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetContextMenuAsync), 2, e);
                        contextMenuItem.PackageIcon = emptyImage;
                    }

                    if (string.IsNullOrEmpty(SearchText))
                    {
                        ContextMenuCollection.Add(contextMenuItem);
                        continue;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchText))
                        {
                            ContextMenuCollection.Add(contextMenuItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchText))
                        {
                            ContextMenuCollection.Add(contextMenuItem);
                            continue;
                        }
                    }
                }

                ContextMenuResultKind = ContextMenuCollection.Count is 0 ? ContextMenuResultKind.Failed : ContextMenuResultKind.Successfully;
                ContextMenuFailedContent = ContextMenuCollection.Count is 0 ? MenuEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 获取已禁用菜单 CLSID 列表
        /// </summary>
        private List<KeyValuePair<Guid, string>> GetBlockedClsidList()
        {
            List<KeyValuePair<Guid, string>> blockClsidList = [];

            try
            {
                RegistryKey blockKey = Registry.LocalMachine.OpenSubKey(blockedKey, false);

                if (blockKey is not null)
                {
                    string[] blcokedClsidArray = blockKey.GetValueNames();

                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (Guid.TryParse(blockedClsid, out Guid clsid))
                        {
                            blockClsidList.Add(new KeyValuePair<Guid, string>(clsid, Convert.ToString(Registry.LocalMachine)));
                        }
                    }

                    blockKey.Close();
                    blockKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetBlockedClsidList), 1, e);
            }

            try
            {
                RegistryKey blockKey = Registry.CurrentUser.OpenSubKey(blockedKey, false);

                if (blockKey is not null)
                {
                    string[] blcokedClsidArray = blockKey.GetValueNames();

                    foreach (string blockedClsid in blcokedClsidArray)
                    {
                        if (Guid.TryParse(blockedClsid, out Guid clsid))
                        {
                            blockClsidList.Add(new KeyValuePair<Guid, string>(clsid, Convert.ToString(Registry.CurrentUser)));
                        }
                    }

                    blockKey.Close();
                    blockKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetBlockedClsidList), 2, e);
            }

            return blockClsidList;
        }

        /// <summary>
        /// 检索打包应用信息
        /// </summary>
        /// <returns>
        /// 参数 1：返回应用的显示名称
        /// 参数 2：返回应用的显示图片位置
        /// 参数 3：返回应用的菜单 clsid 项列表
        /// </returns>
        private static (string displayName, string logoFullPath, List<Guid> clsidList) GetAppInfo(string packageInstalledLocation)
        {
            try
            {
                if (string.IsNullOrEmpty(packageInstalledLocation))
                {
                    return ValueTuple.Create(string.Empty, string.Empty, new List<Guid>());
                }

                string manifestFilePath = Path.Combine(packageInstalledLocation, "AppxManifest.xml");

                if (File.Exists(manifestFilePath))
                {
                    string contents = File.ReadAllText(manifestFilePath);
                    XmlDocument xmlDocument = new();
                    xmlDocument.LoadXml(contents);

                    XmlNamespaceManager xmlNamespaceManager = new(xmlDocument.NameTable);
                    xmlNamespaceManager.AddNamespace("default", "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
                    xmlNamespaceManager.AddNamespace("desktop4", "http://schemas.microsoft.com/appx/manifest/desktop/windows10/4");
                    xmlNamespaceManager.AddNamespace("desktop5", "http://schemas.microsoft.com/appx/manifest/desktop/windows10/5");
                    xmlNamespaceManager.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");

                    XmlNodeList desktop4VerbNodesList = xmlDocument.SelectNodes("//desktop4:FileExplorerContextMenus//desktop4:Verb", xmlNamespaceManager);
                    XmlNodeList desktop5VerbNodesList = xmlDocument.SelectNodes("//desktop4:FileExplorerContextMenus//desktop5:Verb", xmlNamespaceManager);

                    // 获取右键菜单项 CLSID 列表
                    List<Guid> clsidList = [];
                    if ((desktop4VerbNodesList?.Count ?? 0) + (desktop5VerbNodesList?.Count ?? 0) > 0)
                    {
                        if (desktop4VerbNodesList is not null)
                        {
                            for (int i = 0; i < desktop4VerbNodesList.Count; i++)
                            {
                                string clsid = desktop4VerbNodesList[i]?.Attributes?["Clsid"]?.Value;
                                if (Guid.TryParse(clsid, out Guid guid))
                                {
                                    clsidList.Add(guid);
                                }
                            }
                        }

                        if (desktop5VerbNodesList is not null)
                        {
                            for (int i = 0; i < desktop5VerbNodesList.Count; i++)
                            {
                                string clsid = desktop5VerbNodesList[i]?.Attributes?["Clsid"]?.Value;
                                if (Guid.TryParse(clsid, out Guid guid))
                                {
                                    clsidList.Add(guid);
                                }
                            }
                        }

                        clsidList = [.. clsidList.Distinct()];
                    }

                    // 获取应用的显示图片
                    XmlNode logoNode = xmlDocument.SelectSingleNode("//default:Properties/default:Logo", xmlNamespaceManager);
                    string logo = logoNode?.InnerText ?? string.Empty;
                    string logoFullPath = Path.Combine(packageInstalledLocation, logo);

                    if (!File.Exists(logoFullPath))
                    {
                        string logoDirectory = Path.GetDirectoryName(logoFullPath);
                        logoFullPath = string.Empty;
                        string logoKey = Path.GetFileNameWithoutExtension(logo);
                        string extension = Path.GetExtension(logo);
                        if (Directory.Exists(logoDirectory))
                        {
                            string[] files = Directory.GetFiles(logoDirectory, $"{logoKey}*{extension}");
                            logoFullPath = files?.FirstOrDefault(c => !c.Contains("contrast"));
                            if (string.IsNullOrEmpty(logoFullPath))
                            {
                                logoFullPath = files?.FirstOrDefault() ?? string.Empty;
                            }
                        }
                    }

                    // 获取应用的显示名称
                    string displayName = string.Empty;
                    XmlNodeList appNodesList = xmlDocument.SelectNodes("//uap:VisualElements", xmlNamespaceManager);

                    if (appNodesList is not null && appNodesList.Count > 0)
                    {
                        foreach (XmlNode appNode in appNodesList.OfType<XmlNode>().OrderBy(c => string.Equals(c.Attributes?["AppListEntry"]?.Value, "none") ? 1 : 0))
                        {
                            displayName = appNode.Attributes?["DisplayName"]?.Value ?? string.Empty;
                        }
                    }

                    return ValueTuple.Create(displayName, logoFullPath, clsidList);
                }

                return ValueTuple.Create(string.Empty, string.Empty, new List<Guid>());
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetAppInfo), 1, e);
                return ValueTuple.Create(string.Empty, string.Empty, new List<Guid>());
            }
        }

        /// <summary>
        /// 获取设备所有应用容器数据列表
        /// </summary>
        private List<INET_FIREWALL_APP_CONTAINER> GetAppContainerList()
        {
            IntPtr arrayValue = IntPtr.Zero;
            uint size = 0;
            List<INET_FIREWALL_APP_CONTAINER> inetContainerList = [];

            GCHandle handle_pdwCntPublicACs = GCHandle.Alloc(size, GCHandleType.Pinned);
            GCHandle handle_ppACs = GCHandle.Alloc(arrayValue, GCHandleType.Pinned);
            FirewallAPILibrary.NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out size, out arrayValue);

            IntPtr pACs = arrayValue;

            int structSize = Marshal.SizeOf<INET_FIREWALL_APP_CONTAINER>();

            for (int index = 0; index < size; index++)
            {
                INET_FIREWALL_APP_CONTAINER container = Marshal.PtrToStructure<INET_FIREWALL_APP_CONTAINER>(arrayValue);

                inetContainerList.Add(container);
                arrayValue = new IntPtr((long)arrayValue + structSize);
            }

            handle_pdwCntPublicACs.Free();
            handle_ppACs.Free();
            FirewallAPILibrary.NetworkIsolationFreeAppContainers(pACs);
            return inetContainerList;
        }

        /// <summary>
        /// 获取加载右键菜单是否成功
        /// </summary>
        private Visibility GetContextMenuSuccessfullyState(ContextMenuResultKind contextMenuResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? contextMenuResultKind is ContextMenuResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : contextMenuResultKind is ContextMenuResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索右键菜单是否成功
        /// </summary>
        private Visibility CheckContextMenuState(ContextMenuResultKind contextMenuResultKind, ContextMenuResultKind comparedContextMenuResultKind)
        {
            return Equals(contextMenuResultKind, comparedContextMenuResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(ContextMenuResultKind contextMenuResultKind)
        {
            return contextMenuResultKind is not ContextMenuResultKind.Loading;
        }
    }
}
