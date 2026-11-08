using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.FirewallAPI;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using PowerToolbox.WindowsAPI.PInvoke.Shlwapi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    internal sealed partial class ContextMenuManagerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private const string packageComPackageKey = @"SOFTWARE\Classes\PackagedCom\Package";
        private const string blockedKey = @"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        private readonly string ContextMenuCountInfoString = ResourceService.ContextMenuManagerResource.GetString("ContextMenuCountInfo");
        private readonly string MenuEmptyDescriptionString = ResourceService.ContextMenuManagerResource.GetString("MenuEmptyDescription");
        private readonly string MenuEmptyWithConditionDescriptionString = ResourceService.ContextMenuManagerResource.GetString("MenuEmptyWithConditionDescription");
        private readonly BitmapImage emptyImage = new();
        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private string _searchText;

        private string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(SearchText)));
                }
            }
        }

        private ContextMenuResultKind _contextMenuResultKind;

        private ContextMenuResultKind ContextMenuResultKind
        {
            get { return _contextMenuResultKind; }

            set
            {
                if (!Equals(_contextMenuResultKind, value))
                {
                    _contextMenuResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(ContextMenuResultKind)));
                }
            }
        }

        private string _contextMenuFailedContent;

        private string ContextMenuFailedContent
        {
            get { return _contextMenuFailedContent; }

            set
            {
                if (!string.Equals(_contextMenuFailedContent, value))
                {
                    _contextMenuFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(ContextMenuFailedContent)));
                }
            }
        }

        private List<ContextMenuModel> ContextMenuList { get; } = [];

        private WinRTObservableCollection<ContextMenuModel> ContextMenuCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal ContextMenuManagerPage()
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

            if (!isInitialized)
            {
                isInitialized = true;
                await GetContextMenuAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 打开应用包路径
        /// </summary>
        private void OnOpenPackagePathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string path)
            {
                OpenPackagePath(path);
            }
        }

        /// <summary>
        /// 保存右键菜单显示状态结果
        /// </summary>
        private async void OnCheckBoxClickExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ContextMenuItemModel contextMenuItem)
            {
                string blockedClsid = contextMenuItem.Clsid.ToString("B").ToUpperInvariant();
                if (contextMenuItem.IsEnabled)
                {
                    contextMenuItem.IsEnabled = !contextMenuItem.IsEnabled;
                    contextMenuItem.BlockedClsidType = BlockedClsidType.CurrentUser;
                    await UpdateContextMenuResultAsync(contextMenuItem.BlockedClsidType, true, blockedClsid);
                }
                else
                {
                    contextMenuItem.IsEnabled = !contextMenuItem.IsEnabled;
                    await UpdateContextMenuResultAsync(contextMenuItem.BlockedClsidType, false, blockedClsid);
                    if (contextMenuItem.BlockedClsidType is BlockedClsidType.LocalMachine && RuntimeHelper.IsElevated)
                    {
                        if (string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.LocalMachine, blockedKey, blockedClsid), string.Format("Blocked by {0}", nameof(PowerToolbox))))
                        {
                            RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, blockedKey, blockedClsid);
                        }
                    }
                    else if (contextMenuItem.BlockedClsidType is BlockedClsidType.CurrentUser)
                    {
                        if (string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, blockedKey, blockedClsid), string.Format("Blocked by {0}", nameof(PowerToolbox))))
                        {
                            RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, blockedKey, blockedClsid);
                        }
                    }
                }

                bool isLocalMachineBlocked = RegistryHelper.ReadRegistryKey<object>(Registry.LocalMachine, blockedKey, blockedClsid) is not null;
                bool isCurrentUserBlocked = RegistryHelper.ReadRegistryKey<object>(Registry.CurrentUser, blockedKey, blockedClsid) is not null;

                if (isLocalMachineBlocked || isCurrentUserBlocked)
                {
                    contextMenuItem.IsEnabled = false;
                    if (isLocalMachineBlocked)
                    {
                        contextMenuItem.BlockedClsidType = BlockedClsidType.LocalMachine;
                    }
                    else if (isCurrentUserBlocked)
                    {
                        contextMenuItem.BlockedClsidType = BlockedClsidType.CurrentUser;
                    }
                    else
                    {
                        contextMenuItem.BlockedClsidType = BlockedClsidType.Unknown;
                    }
                }
                else
                {
                    contextMenuItem.IsEnabled = true;
                    contextMenuItem.BlockedClsidType = BlockedClsidType.Unknown;
                }

                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ContextMenuUpdate, true));
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ContextMenuUpdate, false));
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

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
                    if ((!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchText)) || (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchText)))
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
            await RestoreDefaultAsync();
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

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 获取右键菜单数据
        /// </summary>
        private async Task GetContextMenuAsync()
        {
            if (ContextMenuResultKind is not ContextMenuResultKind.Loading)
            {
                ContextMenuResultKind = ContextMenuResultKind.Loading;
                ContextMenuList.Clear();
                ContextMenuCollection.Clear();
                List<ContextMenuModel> contextMenuList = await GetContextMenuInfoListAsync();

                if (contextMenuList.Count is 0)
                {
                    ContextMenuResultKind = ContextMenuResultKind.Failed;
                    ContextMenuFailedContent = MenuEmptyDescriptionString;
                }
                else
                {
                    ContextMenuList.AddRange(contextMenuList);
                    foreach (ContextMenuModel contextMenuItem in contextMenuList)
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetContextMenuAsync), 1, e);
                            contextMenuItem.PackageIcon = emptyImage;
                        }
                    }

                    foreach (ContextMenuModel contextMenuItem in ContextMenuList)
                    {
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            ContextMenuCollection.Add(contextMenuItem);
                        }
                        else
                        {
                            if ((!string.IsNullOrEmpty(contextMenuItem.PackageDisplayName) && contextMenuItem.PackageDisplayName.Contains(SearchText)) || (!string.IsNullOrEmpty(contextMenuItem.PackageFullName) && contextMenuItem.PackageFullName.Contains(SearchText)))
                            {
                                ContextMenuCollection.Add(contextMenuItem);
                            }
                        }
                    }
                    ContextMenuResultKind = ContextMenuCollection.Count is 0 ? ContextMenuResultKind.Failed : ContextMenuResultKind.Successfully;
                    ContextMenuFailedContent = ContextMenuCollection.Count is 0 ? MenuEmptyWithConditionDescriptionString : string.Empty;
                }
            }
        }

        /// <summary>
        /// 获取右键菜单信息
        /// </summary>
        private async Task<List<ContextMenuModel>> GetContextMenuInfoListAsync()
        {
            return await Task.Run(() =>
            {
                List<ContextMenuModel> contextMenuList = [];

                try
                {
                    List<KeyValuePair<Guid, string>> blockedList = GetBlockedClsidList();
                    List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetAppContainerList();
                    ReadOnlyCollection<string> packageFullNameCollection = RegistryHelper.GetSubKeyCollection(Registry.LocalMachine, packageComPackageKey);
                    string currentPackageFullName = GetCurrentPackageFullName();

                    foreach (string packageFullName in packageFullNameCollection)
                    {
                        // 排除当前应用（当前应用需要在设置中修改右键菜单显示状态）
                        if (string.Equals(packageFullName, currentPackageFullName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        ReadOnlyCollection<string> classCollection = RegistryHelper.GetSubKeyCollection(Registry.LocalMachine, string.Format(@"{0}\{1}\Class", packageComPackageKey, packageFullName));

                        List<ContextMenuItemModel> contextMenuItemList = [];

                        foreach (string clsidString in classCollection)
                        {
                            if (Guid.TryParse(clsidString, out Guid clsid))
                            {
                                string dllPath = RegistryHelper.ReadRegistryKey<string>(Registry.LocalMachine, string.Format(@"{0}\{1}\Class\{2}", packageComPackageKey, packageFullName, clsidString), "DllPath");

                                if (!string.IsNullOrEmpty(dllPath) && dllPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                                {
                                    int serverId = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, string.Format(@"{0}\{1}\Class\{2}", packageComPackageKey, packageFullName, clsidString), "ServerId");
                                    int threading = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, string.Format(@"{0}\{1}\Class\{2}", packageComPackageKey, packageFullName, clsidString), "Threading");
                                    int index = blockedList.FindIndex(item => Equals(item.Key, clsid));

                                    contextMenuItemList.Add(new()
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
                        }

                        if (GetPackagePath(packageFullName) is string packagePath)
                        {
                            (string displayName, string logoFullPath, List<Guid> clsidList) = GetAppInfo(packagePath);

                            if (clsidList is not null && clsidList.Count > 0)
                            {
                                StringBuilder displayNameBuilder = new(1024);
                                foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in from INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList where inetContainerItem.displayName.Contains(packageFullName) select inetContainerItem)
                                {
                                    ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.displayName, displayNameBuilder, displayNameBuilder.Capacity, 0);
                                    break;
                                }

                                ContextMenuModel contextMenu = new()
                                {
                                    PackageDisplayName = string.IsNullOrEmpty(Convert.ToString(displayNameBuilder)) ? displayName : Convert.ToString(displayNameBuilder),
                                    PackageFullName = packageFullName,
                                    PackageIconUri = Uri.TryCreate(logoFullPath, UriKind.Absolute, out Uri uri) ? uri : null,
                                    PackagePath = packagePath,
                                    ContextMenuItemCollection = [.. contextMenuItemList]
                                };

                                contextMenuList.Add(contextMenu);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetContextMenuAsync), 1, e);
                }

                return contextMenuList;
            });
        }

        /// <summary>
        /// 获取已禁用菜单 CLSID 列表
        /// </summary>
        private List<KeyValuePair<Guid, string>> GetBlockedClsidList()
        {
            List<KeyValuePair<Guid, string>> blockClsidList = [];

            try
            {
                ReadOnlyCollection<string> blcokedClsidCollection = RegistryHelper.GetValueNameCollection(Registry.LocalMachine, blockedKey);

                foreach (string blockedClsid in blcokedClsidCollection)
                {
                    if (Guid.TryParse(blockedClsid, out Guid clsid))
                    {
                        blockClsidList.Add(new(clsid, Convert.ToString(Registry.LocalMachine)));
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetBlockedClsidList), 1, e);
            }

            try
            {
                ReadOnlyCollection<string> blcokedClsidCollection = RegistryHelper.GetValueNameCollection(Registry.CurrentUser, blockedKey);

                foreach (string blockedClsid in blcokedClsidCollection)
                {
                    if (Guid.TryParse(blockedClsid, out Guid clsid))
                    {
                        blockClsidList.Add(new(clsid, Convert.ToString(Registry.LocalMachine)));
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetBlockedClsidList), 2, e);
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
            if (string.IsNullOrEmpty(packageInstalledLocation))
            {
                return default;
            }

            try
            {
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(GetAppInfo), 1, e);
                return ValueTuple.Create(string.Empty, string.Empty, new List<Guid>());
            }
        }

        /// <summary>
        /// 获取设备所有应用容器数据列表
        /// </summary>
        private List<INET_FIREWALL_APP_CONTAINER> GetAppContainerList()
        {
            nint arrayValue = 0;
            uint size = 0;
            List<INET_FIREWALL_APP_CONTAINER> inetContainerList = [];

            GCHandle handle_pdwCntPublicACs = GCHandle.Alloc(size, GCHandleType.Pinned);
            GCHandle handle_ppACs = GCHandle.Alloc(arrayValue, GCHandleType.Pinned);
            FirewallAPILibrary.NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out size, out arrayValue);

            nint pACs = arrayValue;
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
        /// 恢复默认
        /// </summary>
        private async Task RestoreDefaultAsync()
        {
            await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    try
                    {
                        ReadOnlyCollection<string> blcokedClsidCollection = RegistryHelper.GetValueNameCollection(Registry.LocalMachine, blockedKey);
                        foreach (string blockedClsid in blcokedClsidCollection)
                        {
                            if (string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.LocalMachine, blockedKey, blockedClsid), string.Format("Blocked by {0}", nameof(PowerToolbox))))
                            {
                                RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, blockedKey, blockedClsid);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(RestoreDefaultAsync), 1, e);
                    }
                }

                try
                {
                    ReadOnlyCollection<string> blcokedClsidCollection = RegistryHelper.GetValueNameCollection(Registry.CurrentUser, blockedKey);
                    foreach (string blockedClsid in blcokedClsidCollection)
                    {
                        if (string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, blockedKey, blockedClsid), string.Format("Blocked by {0}", nameof(PowerToolbox))))
                        {
                            RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, blockedKey, blockedClsid);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(RestoreDefaultAsync), 2, e);
                }
            });
        }

        /// <summary>
        /// 更新右键菜单显示效果
        /// </summary>
        private async Task UpdateContextMenuResultAsync(BlockedClsidType blockedClsidType, bool isBlocked, string blockedClsid)
        {
            if (isBlocked)
            {
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, blockedKey, blockedClsid, string.Format("Blocked by {0}", nameof(PowerToolbox)));
            }
            else
            {
                if (blockedClsidType is BlockedClsidType.LocalMachine && RuntimeHelper.IsElevated)
                {
                    if (string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.LocalMachine, blockedKey, blockedClsid), string.Format("Blocked by {0}", nameof(PowerToolbox))))
                    {
                        RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, blockedKey, blockedClsid);
                    }
                }
                else if (blockedClsidType is BlockedClsidType.CurrentUser)
                {
                    if (string.Equals(RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, blockedKey, blockedClsid), string.Format("Blocked by {0}", nameof(PowerToolbox))))
                    {
                        RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, blockedKey, blockedClsid);
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前应用包完整名称
        /// </summary>
        private string GetCurrentPackageFullName()
        {
            int length = 0;
            string currentPackageFullName = string.Empty;

            if (Kernel32Library.GetCurrentPackageFullName(ref length, null) is 122)
            {
                StringBuilder packageFullNameBuilder = new(length + 1);
                Kernel32Library.GetCurrentPackageFullName(ref length, packageFullNameBuilder);
                currentPackageFullName = Convert.ToString(packageFullNameBuilder);
            }

            return currentPackageFullName;
        }

        /// <summary>
        /// 获取当前应用包完整名称
        /// </summary>
        private string GetPackagePath(string packageFullName)
        {
            if (string.IsNullOrEmpty(packageFullName))
            {
                return default;
            }

            int length = 0;
            string packagePath = string.Empty;

            if (Kernel32Library.GetPackagePathByFullName(packageFullName, ref length, null) is 122)
            {
                StringBuilder packagePathBuilder = new(length + 1);
                Kernel32Library.GetPackagePathByFullName(packageFullName, ref length, packagePathBuilder);
                packagePath = Convert.ToString(packagePathBuilder);
            }

            return packagePath;
        }

        /// <summary>
        /// 打开应用包路径
        /// </summary>
        private void OpenPackagePath(string filePath)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(filePath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ContextMenuManagerPage), nameof(OpenPackagePath), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取加载右键菜单是否成功
        /// </summary>
        private Visibility GetContextMenuSuccessfullyVisibility(ContextMenuResultKind contextMenuResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? contextMenuResultKind is ContextMenuResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : contextMenuResultKind is ContextMenuResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索右键菜单是否成功
        /// </summary>
        private Visibility CheckContextMenuResultKindVisibility(ContextMenuResultKind contextMenuResultKind, ContextMenuResultKind comparedContextMenuResultKind)
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

        #endregion 第七部分：数据操作与业务逻辑
    }
}
