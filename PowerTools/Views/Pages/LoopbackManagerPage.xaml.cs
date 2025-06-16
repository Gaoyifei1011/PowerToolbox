using PowerTools.Extensions.DataType.Enums;
using PowerTools.Models;
using PowerTools.Services.Root;
using PowerTools.Views.TeachingTips;
using PowerTools.Views.Windows;
using PowerTools.WindowsAPI.PInvoke.FirewallAPI;
using PowerTools.WindowsAPI.PInvoke.Shlwapi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerTools.Views.Pages
{
    /// <summary>
    /// 网络回环管理页面
    /// </summary>
    public sealed partial class LoopbackManagerPage : Page, INotifyPropertyChanged
    {
        private readonly string LoopbackInformationString = ResourceService.LoopbackManagerResource.GetString("LoopbackInformation");
        private readonly string LoopbackEmptyDescriptionString = ResourceService.LoopbackManagerResource.GetString("LoopbackEmptyDescription");
        private readonly string LoopbackEmptyWithConditionDescriptionString = ResourceService.LoopbackManagerResource.GetString("LoopbackEmptyWithConditionDescription");
        private readonly BitmapImage emptyImage = new();
        private bool isInitialized;

        private string _loopbackDescription = string.Empty;

        public string LoopbackDescription
        {
            get { return _loopbackDescription; }

            set
            {
                if (!Equals(_loopbackDescription, value))
                {
                    _loopbackDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoopbackDescription)));
                }
            }
        }

        private LoopbackResultKind _loopbackResultKind;

        public LoopbackResultKind LoopbackResultKind
        {
            get { return _loopbackResultKind; }

            set
            {
                if (!Equals(_loopbackResultKind, value))
                {
                    _loopbackResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoopbackResultKind)));
                }
            }
        }

        private string _loopbackFailedContent;

        public string LoopbackFailedContent
        {
            get { return _loopbackFailedContent; }

            set
            {
                if (!string.Equals(_loopbackFailedContent, value))
                {
                    _loopbackFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoopbackFailedContent)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private bool _isSaved = false;

        public bool IsSaved
        {
            get { return _isSaved; }

            set
            {
                if (!Equals(_isSaved, value))
                {
                    _isSaved = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaved)));
                }
            }
        }

        private List<LoopbackModel> LoopbackList { get; } = [];

        private ObservableCollection<LoopbackModel> LoopbackCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public LoopbackManagerPage()
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
                await GetLoopbackDataAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 点击复选框时使保存按钮处于可选状态
        /// </summary>
        private void OnCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (!IsSaved)
            {
                IsSaved = true;
            }

            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 打开应用程序的工作目录
        /// </summary>
        private void OnOpenWorkingDirectoryRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string parameter && !string.IsNullOrEmpty(parameter))
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(parameter);
                    }
                    catch (Exception)
                    {
                    }
                });
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：网络回环管理页面——挂载的事件

        /// <summary>
        /// 了解网络回环
        /// </summary>
        private void OnLearnLoopbackClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/previous-versions/windows/apps/hh780593(v=win.10)");
                }
                catch (Exception)
                {
                }
            });
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && LoopbackList.Count > 0)
            {
                LoopbackResultKind = LoopbackResultKind.Loading;
                LoopbackCollection.Clear();
                foreach (LoopbackModel loopbackItem in LoopbackList)
                {
                    if (!string.IsNullOrEmpty(loopbackItem.AppContainerName) && loopbackItem.AppContainerName.Contains(SearchText))
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                        LoopbackCollection.Add(loopbackItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.DisplayName) && loopbackItem.DisplayName.Contains(SearchText))
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                        LoopbackCollection.Add(loopbackItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.Description) && loopbackItem.Description.Contains(SearchText))
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                        LoopbackCollection.Add(loopbackItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.PackageFullName) && loopbackItem.PackageFullName.Contains(SearchText))
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                        LoopbackCollection.Add(loopbackItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.AppContainerUserName) && loopbackItem.AppContainerUserName.Contains(SearchText))
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                        LoopbackCollection.Add(loopbackItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(loopbackItem.AppContainerSIDName) && loopbackItem.AppContainerSIDName.Contains(SearchText))
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                        LoopbackCollection.Add(loopbackItem);
                        continue;
                    }
                }

                LoopbackResultKind = LoopbackCollection.Count is 0 ? LoopbackResultKind.Failed : LoopbackResultKind.Successfully;
                LoopbackFailedContent = LoopbackCollection.Count is 0 ? LoopbackEmptyWithConditionDescriptionString : string.Empty;
                LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 搜索应用名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && LoopbackList.Count > 0)
            {
                LoopbackResultKind = LoopbackResultKind.Loading;
                LoopbackCollection.Clear();
                foreach (LoopbackModel loopbackItem in LoopbackList)
                {
                    loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                    LoopbackCollection.Add(loopbackItem);
                }

                LoopbackResultKind = LoopbackCollection.Count is 0 ? LoopbackResultKind.Failed : LoopbackResultKind.Successfully;
                LoopbackFailedContent = LoopbackCollection.Count is 0 ? LoopbackEmptyWithConditionDescriptionString : string.Empty;
                LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = true;
            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                loopbackItem.IsSelected = true;
            }

            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = true;
            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                loopbackItem.IsSelected = false;
            }

            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 保存
        /// </summary>
        private async void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = false;
            List<LoopbackModel> selectedLoopbackList = [];
            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                if (loopbackItem.IsSelected)
                {
                    loopbackItem.IsOldChecked = loopbackItem.IsSelected;
                    selectedLoopbackList.Add(loopbackItem);
                }
            }

            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
            await SetLoopbackStateAsync(selectedLoopbackList);
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void OnResetClicked(object sender, RoutedEventArgs args)
        {
            IsSaved = false;
            SearchText = string.Empty;
            LoopbackResultKind = LoopbackResultKind.Loading;
            LoopbackCollection.Clear();

            foreach (LoopbackModel loopbackItem in LoopbackCollection)
            {
                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
            }

            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetLoopbackDataAsync();
        }

        #endregion 第三部分：网络回环管理页面——挂载的事件

        /// <summary>
        /// 获取应用数据
        /// </summary>
        private async Task GetLoopbackDataAsync()
        {
            LoopbackResultKind = LoopbackResultKind.Loading;
            IsSaved = false;
            LoopbackList.Clear();
            LoopbackCollection.Clear();
            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));

            List<LoopbackModel> loopbackList = await Task.Run(() =>
            {
                List<LoopbackModel> loopbackList = [];
                List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetLoopbackList();
                List<SID_AND_ATTRIBUTES> inetLoopbackEnabledList = GetLoopbackEnabledList();

                foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList)
                {
                    try
                    {
                        bool isEnabled = GetLoopbackEnabled(inetContainerItem.appContainerSid, inetLoopbackEnabledList);

                        StringBuilder displayNameBuilder = new(1024);
                        ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.displayName, displayNameBuilder, displayNameBuilder.Capacity, IntPtr.Zero);

                        StringBuilder descriptionBuilder = new(1024);
                        ShlwapiLibrary.SHLoadIndirectString(inetContainerItem.description, descriptionBuilder, descriptionBuilder.Capacity, IntPtr.Zero);

                        INET_FIREWALL_AC_BINARIES inetBinaries = inetContainerItem.binaries;
                        string[] stringBinaries = null;
                        if (inetBinaries.count != 0 && inetBinaries.binaries != IntPtr.Zero)
                        {
                            stringBinaries = new string[inetBinaries.count];
                            long num = inetBinaries.binaries.ToInt64();
                            for (int i = 0; i < inetBinaries.count; i++)
                            {
                                stringBinaries[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr((IntPtr)num));
                                num += IntPtr.Size;
                            }
                        }

                        SecurityIdentifier appContainerSid = null;

                        try
                        {
                            byte revision = Marshal.ReadByte(inetContainerItem.appContainerSid, 0);
                            if (revision is 1 && inetContainerItem.appContainerSid != IntPtr.Zero)
                            {
                                appContainerSid = new SecurityIdentifier(inetContainerItem.appContainerSid);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Parse app container sid failed", e);
                        }

                        NTAccount userAccountType = null;

                        try
                        {
                            byte revision = Marshal.ReadByte(inetContainerItem.appContainerSid, 0);
                            if (revision is 1 && inetContainerItem.userSid != IntPtr.Zero)
                            {
                                SecurityIdentifier userSid = new(inetContainerItem.userSid);
                                userAccountType = (NTAccount)userSid.Translate(typeof(NTAccount));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, "Parse user sid failed", e);
                        }

                        string logoFullPath = GetLogoInfo(inetContainerItem.workingDirectory);

                        LoopbackModel loopbackItem = new()
                        {
                            AppContainerName = string.IsNullOrEmpty(inetContainerItem.appContainerName) ? ResourceService.LoopbackManagerResource.GetString("Unknown") : inetContainerItem.appContainerName,
                            AppContainerSID = inetContainerItem.appContainerSid,
                            AppContainerSIDName = string.IsNullOrEmpty(Convert.ToString(appContainerSid)) ? ResourceService.LoopbackManagerResource.GetString("Unknown") : Convert.ToString(appContainerSid),
                            DisplayName = string.IsNullOrEmpty(Convert.ToString(displayNameBuilder)) ? ResourceService.LoopbackManagerResource.GetString("Unknown") : Convert.ToString(displayNameBuilder),
                            Description = string.IsNullOrEmpty(Convert.ToString(descriptionBuilder)) ? ResourceService.LoopbackManagerResource.GetString("Unknown") : Convert.ToString(descriptionBuilder),
                            WorkingDirectory = string.IsNullOrEmpty(inetContainerItem.workingDirectory) ? ResourceService.LoopbackManagerResource.GetString("Unknown") : inetContainerItem.workingDirectory,
                            PackageFullName = string.IsNullOrEmpty(inetContainerItem.packageFullName) ? ResourceService.LoopbackManagerResource.GetString("Unknown") : inetContainerItem.packageFullName,
                            PackageIconUri = Uri.TryCreate(logoFullPath, UriKind.Absolute, out Uri uri) ? uri : null,
                            AppBinariesPath = stringBinaries is not null ? string.Concat(stringBinaries) : ResourceService.LoopbackManagerResource.GetString("Unknown"),
                            AppContainerUserName = userAccountType is not null ? Convert.ToString(userAccountType) : ResourceService.LoopbackManagerResource.GetString("Unknown"),
                            IsSelected = isEnabled,
                            IsOldChecked = isEnabled
                        };

                        loopbackList.Add(loopbackItem);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                return loopbackList;
            });

            LoopbackList.AddRange(loopbackList);

            if (LoopbackList.Count is 0)
            {
                LoopbackResultKind = LoopbackResultKind.Failed;
                LoopbackFailedContent = LoopbackEmptyDescriptionString;
            }
            else
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    foreach (LoopbackModel loopbackItem in LoopbackList)
                    {
                        loopbackItem.IsSelected = loopbackItem.IsOldChecked;

                        try
                        {
                            BitmapImage bitmapImage = new();
                            if (loopbackItem.PackageIconUri is not null)
                            {
                                bitmapImage.UriSource = loopbackItem.PackageIconUri;
                            }
                            loopbackItem.AppIcon = bitmapImage;
                        }
                        catch (Exception)
                        {
                            loopbackItem.AppIcon = emptyImage;
                        }

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            LoopbackCollection.Add(loopbackItem);
                            continue;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(loopbackItem.AppContainerName) && loopbackItem.AppContainerName.Contains(SearchText))
                            {
                                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                                LoopbackCollection.Add(loopbackItem);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(loopbackItem.DisplayName) && loopbackItem.DisplayName.Contains(SearchText))
                            {
                                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                                LoopbackCollection.Add(loopbackItem);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(loopbackItem.Description) && loopbackItem.Description.Contains(SearchText))
                            {
                                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                                LoopbackCollection.Add(loopbackItem);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(loopbackItem.PackageFullName) && loopbackItem.PackageFullName.Contains(SearchText))
                            {
                                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                                LoopbackCollection.Add(loopbackItem);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(loopbackItem.AppContainerUserName) && loopbackItem.AppContainerUserName.Contains(SearchText))
                            {
                                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                                LoopbackCollection.Add(loopbackItem);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(loopbackItem.AppContainerSIDName) && loopbackItem.AppContainerSIDName.Contains(SearchText))
                            {
                                loopbackItem.IsSelected = loopbackItem.IsOldChecked;
                                LoopbackCollection.Add(loopbackItem);
                                continue;
                            }
                        }
                    }
                }
            }

            LoopbackResultKind = LoopbackCollection.Count is 0 ? LoopbackResultKind.Failed : LoopbackResultKind.Successfully;
            LoopbackFailedContent = LoopbackCollection.Count is 0 ? LoopbackEmptyWithConditionDescriptionString : string.Empty;
            LoopbackDescription = string.Format(LoopbackInformationString, LoopbackCollection.Count, LoopbackCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 获取网络回环列表数据
        /// </summary>
        private List<INET_FIREWALL_APP_CONTAINER> GetLoopbackList()
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
        /// 获取已开启网络回环的信息列表数据
        /// </summary>
        private List<SID_AND_ATTRIBUTES> GetLoopbackEnabledList()
        {
            IntPtr arrayValue = IntPtr.Zero;
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
        private bool GetLoopbackEnabled(IntPtr appContainerSid, List<SID_AND_ATTRIBUTES> loopbackEnabledList)
        {
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
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置网络回环状态
        /// </summary>
        private async Task SetLoopbackStateAsync(List<LoopbackModel> loopbackList)
        {
            if (loopbackList is not null)
            {
                bool result = await Task.Run(() =>
                {
                    SID_AND_ATTRIBUTES[] sidAndAttributesArray = new SID_AND_ATTRIBUTES[loopbackList.Count];
                    int count = 0;

                    foreach (LoopbackModel loopbackItem in loopbackList)
                    {
                        if (loopbackItem.IsSelected)
                        {
                            sidAndAttributesArray[count] = new SID_AND_ATTRIBUTES
                            {
                                Attributes = 0u,
                                Sid = loopbackItem.AppContainerSID
                            };
                            count++;
                        }
                    }

                    return FirewallAPILibrary.NetworkIsolationSetAppContainerConfig(loopbackList.Count, sidAndAttributesArray) is 0;
                });

                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LoopbackSetResult, result));
            }
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
            catch (Exception)
            {
            }

            return logoFullPath;
        }

        /// <summary>
        /// 获取加载网络回环是否成功
        /// </summary>
        private Visibility GetLoopbackSuccessfullyState(LoopbackResultKind loopbackResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? loopbackResultKind is LoopbackResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : loopbackResultKind is LoopbackResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索网络回环是否成功
        /// </summary>
        private Visibility CheckLoopbackState(LoopbackResultKind loopbackResultKind, LoopbackResultKind comparedLoopbackResultKind)
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
    }
}
