using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Advapi32;
using PowerToolbox.WindowsAPI.PInvoke.FirewallAPI;
using PowerToolbox.WindowsAPI.PInvoke.Shlwapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 网络回环管理页面
    /// </summary>
    internal sealed partial class LoopbackManagerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string LoopbackCountInfoString = ResourceService.LoopbackManagerResource.GetString("LoopbackCountInfo");
        private readonly string LoopbackEmptyDescriptionString = ResourceService.LoopbackManagerResource.GetString("LoopbackEmptyDescription");
        private readonly string NotAvailableString = ResourceService.LoopbackManagerResource.GetString("NotAvailable");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly BitmapImage emptyImage = new();
        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private LoopbackResultKind _loopbackResultKind;

        private LoopbackResultKind LoopbackResultKind
        {
            get { return _loopbackResultKind; }

            set
            {
                if (!Equals(_loopbackResultKind, value))
                {
                    _loopbackResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(LoopbackResultKind)));
                }
            }
        }

        private int _selectedItemsCount;

        private int SelectedItemsCount
        {
            get { return _selectedItemsCount; }

            set
            {
                if (!Equals(_selectedItemsCount, value))
                {
                    _selectedItemsCount = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItemsCount)));
                }
            }
        }

        private string _loopbackFailedContent;

        private string LoopbackFailedContent
        {
            get { return _loopbackFailedContent; }

            set
            {
                if (!string.Equals(_loopbackFailedContent, value))
                {
                    _loopbackFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(LoopbackFailedContent)));
                }
            }
        }

        private bool _isSaved;

        private bool IsSaved
        {
            get { return _isSaved; }

            set
            {
                if (!Equals(_isSaved, value))
                {
                    _isSaved = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSaved)));
                }
            }
        }

        private WinRTObservableCollection<LoopbackModel> LoopbackCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal LoopbackManagerPage()
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
                await GetLoopbackDataAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 打开应用程序的工作目录
        /// </summary>
        private void OnOpenWorkingDirectoryExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string parameter && !string.IsNullOrEmpty(parameter))
            {
                OpenWorkingDirectory(parameter);
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 了解网络回环
        /// </summary>
        private void OnLearnLoopbackClicked(object sender, RoutedEventArgs args)
        {
            LearnLoopback();
        }

        /// <summary>
        /// 网络回环管理结果列表控件选中项发生变化时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                IsSaved = true;
                SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                IsSaved = true;
                LoopbackManagerListView.SelectAll();
                SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                IsSaved = true;
                LoopbackManagerListView.DeselectRange(new(0, (uint)LoopbackManagerListView.Items.Count));
                SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                IsSaved = true;
                List<Microsoft.UI.Xaml.Data.ItemIndexRange> selectedRangeList = [.. LoopbackManagerListView.SelectedRanges];
                LoopbackManagerListView.SelectAll();
                foreach (Microsoft.UI.Xaml.Data.ItemIndexRange itemIndexRange in selectedRangeList)
                {
                    LoopbackManagerListView.DeselectRange(itemIndexRange);
                }
                SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        private async void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                IsSaved = false;
                List<LoopbackModel> selectedLoopbackList = [];
                foreach (LoopbackModel loopbackItem in LoopbackCollection)
                {
                    loopbackItem.IsOldChecked = false;
                }
                foreach (object loopbackItemObj in LoopbackManagerListView.SelectedItems)
                {
                    if (loopbackItemObj is LoopbackModel loopbackItem)
                    {
                        loopbackItem.IsOldChecked = true;
                        selectedLoopbackList.Add(loopbackItem);
                    }
                }
                SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
                bool result = await SaveLoopbackStateAsync(selectedLoopbackList);
                synchronizationContext.Post(async (_) =>
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.LoopbackSetResult, result));
                }, null);
                await GetLoopbackDataAsync();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void OnResetClicked(object sender, RoutedEventArgs args)
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                IsSaved = false;
                LoopbackResultKind = LoopbackResultKind.Loading;
                LoopbackManagerListView.SelectedItems.Clear();
                for (int index = 0; index < LoopbackCollection.Count; index++)
                {
                    LoopbackModel loopbackItem = LoopbackCollection[index];
                    if (loopbackItem.IsOldChecked)
                    {
                        LoopbackManagerListView.SelectRange(new(index, 1));
                    }
                }
                SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
                LoopbackResultKind = LoopbackCollection.Count is 0 ? LoopbackResultKind.Failed : LoopbackResultKind.Successfully;
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetLoopbackDataAsync();
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 获取应用数据
        /// </summary>
        private async Task GetLoopbackDataAsync()
        {
            if (LoopbackResultKind is not LoopbackResultKind.Loading)
            {
                LoopbackResultKind = LoopbackResultKind.Loading;
                IsSaved = false;
                LoopbackCollection.Clear();
                List<LoopbackModel> loopbackList = await GetLoopbackInfoListAsync();

                if (loopbackList.Count is 0)
                {
                    LoopbackResultKind = LoopbackResultKind.Failed;
                    LoopbackFailedContent = LoopbackEmptyDescriptionString;
                }
                else
                {
                    foreach (LoopbackModel loopbackItem in loopbackList)
                    {
                        try
                        {
                            BitmapImage bitmapImage = new();
                            if (loopbackItem.PackageIconUri is not null)
                            {
                                bitmapImage.UriSource = loopbackItem.PackageIconUri;
                            }
                            loopbackItem.AppIcon = bitmapImage;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(GetLoopbackDataAsync), 1, e);
                            loopbackItem.AppIcon = emptyImage;
                        }
                        LoopbackCollection.Add(loopbackItem);
                    }
                    for (int index = 0; index < LoopbackCollection.Count; index++)
                    {
                        LoopbackModel loopbackItem = LoopbackCollection[index];
                        if (loopbackItem.IsOldChecked)
                        {
                            LoopbackManagerListView.SelectRange(new(index, 1));
                        }
                    }
                    LoopbackResultKind = LoopbackResultKind.Successfully;
                    LoopbackFailedContent = string.Empty;
                    SelectedItemsCount = LoopbackManagerListView.SelectedItems.Count;
                }
            }
        }

        /// <summary>
        /// 获取网络回环信息
        /// </summary>
        private async Task<List<LoopbackModel>> GetLoopbackInfoListAsync()
        {
            return await Task.Run(() =>
            {
                List<LoopbackModel> loopbackList = [];
                List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetLoopbackList();
                List<SID_AND_ATTRIBUTES> inetLoopbackEnabledList = GetLoopbackEnabledList();

                foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList)
                {
                    try
                    {
                        bool isLoopbackEnabled = GetLoopbackEnabled(inetContainerItem.appContainerSid, inetLoopbackEnabledList);

                        StringBuilder displayNameBuilder = new(1024);
                        ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.displayName, displayNameBuilder, displayNameBuilder.Capacity, 0);

                        StringBuilder descriptionBuilder = new(1024);
                        ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.description, descriptionBuilder, descriptionBuilder.Capacity, 0);

                        INET_FIREWALL_AC_BINARIES inetBinaries = inetContainerItem.binaries;
                        string[] stringBinaries = null;
                        if (inetBinaries.count is not 0 && inetBinaries.binaries is not 0)
                        {
                            stringBinaries = new string[inetBinaries.count];
                            long num = inetBinaries.binaries;
                            for (int i = 0; i < inetBinaries.count; i++)
                            {
                                stringBinaries[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr((nint)num));
                                num += IntPtr.Size;
                            }
                        }

                        SecurityIdentifier appContainerSid = null;

                        try
                        {
                            byte revision = Marshal.ReadByte(inetContainerItem.appContainerSid, 0);
                            if (revision is 1 && inetContainerItem.appContainerSid is not 0)
                            {
                                appContainerSid = new(inetContainerItem.appContainerSid);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(GetLoopbackDataAsync), 1, e);
                        }

                        NTAccount userAccountType = null;

                        try
                        {
                            byte revision = Marshal.ReadByte(inetContainerItem.appContainerSid, 0);
                            if (revision is 1 && inetContainerItem.userSid is not 0)
                            {
                                SecurityIdentifier userSid = new(inetContainerItem.userSid);
                                if (userSid.IsValidTargetType(typeof(NTAccount)))
                                {
                                    userAccountType = (NTAccount)userSid.Translate(typeof(NTAccount));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(GetLoopbackDataAsync), 2, e);
                        }

                        string logoFullPath = GetLogoInfo(inetContainerItem.workingDirectory);
                        if (Advapi32Library.ConvertSidToStringSid(inetContainerItem.appContainerSid, out string appContainerStringSid))
                        {
                            loopbackList.Add(new()
                            {
                                AppContainerName = string.IsNullOrEmpty(inetContainerItem.appContainerName) ? NotAvailableString : inetContainerItem.appContainerName,
                                AppContainerSID = appContainerStringSid,
                                DisplayName = string.IsNullOrEmpty(Convert.ToString(displayNameBuilder)) ? NotAvailableString : Convert.ToString(displayNameBuilder),
                                Description = string.IsNullOrEmpty(Convert.ToString(descriptionBuilder)) ? NotAvailableString : Convert.ToString(descriptionBuilder),
                                WorkingDirectory = string.IsNullOrEmpty(inetContainerItem.workingDirectory) ? NotAvailableString : inetContainerItem.workingDirectory,
                                PackageFullName = string.IsNullOrEmpty(inetContainerItem.packageFullName) ? NotAvailableString : inetContainerItem.packageFullName,
                                PackageIconUri = Uri.TryCreate(logoFullPath, UriKind.Absolute, out Uri uri) ? uri : null,
                                AppBinariesPath = stringBinaries is not null ? string.Concat(stringBinaries) : NotAvailableString,
                                AppContainerUserName = userAccountType is not null ? Convert.ToString(userAccountType) : NotAvailableString,
                                IsOldChecked = isLoopbackEnabled
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(GetLoopbackInfoListAsync), 1, e);
                        continue;
                    }
                }
                return loopbackList;
            });
        }

        /// <summary>
        /// 获取网络回环列表数据
        /// </summary>
        private List<INET_FIREWALL_APP_CONTAINER> GetLoopbackList()
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
        /// 获取已开启网络回环的信息列表数据
        /// </summary>
        private List<SID_AND_ATTRIBUTES> GetLoopbackEnabledList()
        {
            nint arrayValue = 0;
            uint size = 0;
            List<SID_AND_ATTRIBUTES> inetContainerConfigList = [];

            GCHandle handle_pdwCntPublicACs = GCHandle.Alloc(size, GCHandleType.Pinned);
            GCHandle handle_ppACs = GCHandle.Alloc(arrayValue, GCHandleType.Pinned);
            FirewallAPILibrary.NetworkIsolationGetAppContainerConfig(out size, out arrayValue);

            int structSize = Marshal.SizeOf<SID_AND_ATTRIBUTES>();
            for (int index = 0; index < size; index++)
            {
                SID_AND_ATTRIBUTES cur = Marshal.PtrToStructure<SID_AND_ATTRIBUTES>(arrayValue);
                inetContainerConfigList.Add(cur);
                arrayValue = new IntPtr((long)arrayValue + structSize);
            }

            handle_pdwCntPublicACs.Free();
            handle_ppACs.Free();
            return inetContainerConfigList;
        }

        /// <summary>
        /// 检查应用网络回环是否已开启
        /// </summary>
        private bool GetLoopbackEnabled(nint appContainerSid, List<SID_AND_ATTRIBUTES> loopbackEnabledList)
        {
            if (loopbackEnabledList is null || loopbackEnabledList.Count is 0)
            {
                return default;
            }

            foreach (SID_AND_ATTRIBUTES sidItem in loopbackEnabledList)
            {
                try
                {
                    byte revision = Marshal.ReadByte(sidItem.Sid, 0);
                    if (revision is not 1)
                    {
                        return false;
                    }

                    revision = Marshal.ReadByte(appContainerSid, 0);
                    if (revision is not 1)
                    {
                        return false;
                    }

                    if (Equals(new SecurityIdentifier(sidItem.Sid), new SecurityIdentifier(appContainerSid)))
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(GetLoopbackEnabled), 1, e);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 保存网络回环状态
        /// </summary>
        private async Task<bool> SaveLoopbackStateAsync(List<LoopbackModel> loopbackList)
        {
            if (loopbackList is null)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                SID_AND_ATTRIBUTES[] sidAndAttributesArray = new SID_AND_ATTRIBUTES[loopbackList.Count];
                for (int index = 0; index < loopbackList.Count; index++)
                {
                    LoopbackModel loopbackItem = loopbackList[index];
                    Advapi32Library.ConvertStringSidToSid(loopbackItem.AppContainerSID, out nint psid);
                    sidAndAttributesArray[index] = new()
                    {
                        Attributes = 0,
                        Sid = psid
                    };
                }
                return FirewallAPILibrary.NetworkIsolationSetAppContainerConfig((uint)loopbackList.Count, sidAndAttributesArray) is 0;
            });
        }

        /// <summary>
        /// 获取应用图标路径
        /// </summary>
        private string GetLogoInfo(string packageInstalledLocation)
        {
            string logoFullPath = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(packageInstalledLocation))
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

                        // 获取应用的显示图片
                        XmlNode logoNode = xmlDocument.SelectSingleNode("//default:Properties/default:Logo", xmlNamespaceManager);
                        string logo = logoNode?.InnerText ?? string.Empty;
                        logoFullPath = Path.Combine(packageInstalledLocation, logo);

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
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(GetLogoInfo), 1, e);
            }

            return logoFullPath;
        }

        /// <summary>
        /// 打开应用程序的工作目录
        /// </summary>
        private void OpenWorkingDirectory(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    Process.Start(folderPath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(OpenWorkingDirectory), 1, e);
                }
            });
        }

        /// <summary>
        /// 了解网络回环
        /// </summary>
        private void LearnLoopback()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/previous-versions/windows/apps/hh780593(v=win.10)");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoopbackManagerPage), nameof(LearnLoopback), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取加载网络回环是否成功
        /// </summary>
        private Visibility GetLoopbackSuccessfullyVisibility(LoopbackResultKind loopbackResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? loopbackResultKind is LoopbackResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : loopbackResultKind is LoopbackResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索网络回环是否成功
        /// </summary>
        private Visibility CheckLoopbackResultKindVisibility(LoopbackResultKind loopbackResultKind, LoopbackResultKind comparedLoopbackResultKind)
        {
            return Equals(loopbackResultKind, comparedLoopbackResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>
        private bool GetIsLoading(LoopbackResultKind loopbackResultKind)
        {
            return loopbackResultKind is not LoopbackResultKind.Loading;
        }

        /// <summary>
        /// 获取是否已经保存
        /// </summary>
        private bool GetIsSaved(LoopbackResultKind loopbackResultKind, bool isSaved)
        {
            return loopbackResultKind is not LoopbackResultKind.Loading && isSaved;
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
