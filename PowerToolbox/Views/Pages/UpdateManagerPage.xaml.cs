using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WUApiLib;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// Windows 更新管理页面
    /// </summary>
    internal sealed partial class UpdateManagerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AvailableUpdateString = ResourceService.UpdateManagerResource.GetString("AvailableUpdate");
        private readonly string CurrentChannelString = ResourceService.UpdateManagerResource.GetString("CurrentChannel");
        private readonly string CurrentConfigString = ResourceService.UpdateManagerResource.GetString("CurrentConfig");
        private readonly string DCatFlightingProdString = ResourceService.UpdateManagerResource.GetString("DCatFlightingProd");
        private readonly string DescriptionString = ResourceService.UpdateManagerResource.GetString("Description");
        private readonly string DeviceEnteredWindowsInsiderString = ResourceService.UpdateManagerResource.GetString("DeviceEnteredWindowsInsider");
        private readonly string DeviceProblemNumberString = ResourceService.UpdateManagerResource.GetString("DeviceProblemNumber");
        private readonly string DownloadUpdateCanceledString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateCanceled");
        private readonly string DownloadUpdateCompletedString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateCompleted");
        private readonly string DownloadUpdateFailedWithCodeString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithCode");
        private readonly string DownloadUpdateFailedWithInformationString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateFailedWithInformation");
        private readonly string DownloadUpdateProgressDownloadingString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateProgressDownloading");
        private readonly string DownloadUpdateProgressInitializingString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateProgressInitializing");
        private readonly string DownloadUpdateProgressVerifyingString = ResourceService.UpdateManagerResource.GetString("DownloadUpdateProgressVerifying");
        private readonly string DriverClassString = ResourceService.UpdateManagerResource.GetString("DriverClass");
        private readonly string DriverHardwareIDString = ResourceService.UpdateManagerResource.GetString("DriverHardwareID");
        private readonly string DriverManufacturerString = ResourceService.UpdateManagerResource.GetString("DriverManufacturer");
        private readonly string DriverModelString = ResourceService.UpdateManagerResource.GetString("DriverModel");
        private readonly string DriverProviderString = ResourceService.UpdateManagerResource.GetString("DriverProvider");
        private readonly string DriverVerDateString = ResourceService.UpdateManagerResource.GetString("DriverVerDate");
        private readonly string HiddenUpdateString = ResourceService.UpdateManagerResource.GetString("HiddenUpdate");
        private readonly string InsiderBetaString = ResourceService.UpdateManagerResource.GetString("InsiderBeta");
        private readonly string InsiderCanaryString = ResourceService.UpdateManagerResource.GetString("InsiderCanary");
        private readonly string InsiderDevString = ResourceService.UpdateManagerResource.GetString("InsiderDev");
        private readonly string InsiderReleasePreviewString = ResourceService.UpdateManagerResource.GetString("InsiderReleasePreview");
        private readonly string InstalledUpdateString = ResourceService.UpdateManagerResource.GetString("InstalledUpdate");
        private readonly string InstallUpdateCanceledString = ResourceService.UpdateManagerResource.GetString("InstallUpdateCanceled");
        private readonly string InstallUpdateCompletedNeedRebootString = ResourceService.UpdateManagerResource.GetString("InstallUpdateCompletedNeedReboot");
        private readonly string InstallUpdateCompletedString = ResourceService.UpdateManagerResource.GetString("InstallUpdateCompleted");
        private readonly string InstallUpdateFailedWithCodeString = ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithCode");
        private readonly string InstallUpdateFailedWithInformationString = ResourceService.UpdateManagerResource.GetString("InstallUpdateFailedWithInformation");
        private readonly string InstallUpdateProgressString = ResourceService.UpdateManagerResource.GetString("InstallUpdateProgress");
        private readonly string IsBetaString = ResourceService.UpdateManagerResource.GetString("IsBeta");
        private readonly string IsMandatoryString = ResourceService.UpdateManagerResource.GetString("IsMandatory");
        private readonly string LearnWindowsInsiderPreviewString = ResourceService.UpdateManagerResource.GetString("LearnWindowsInsiderPreview");
        private readonly string MaxDownloadSizeString = ResourceService.UpdateManagerResource.GetString("MaxDownloadSize");
        private readonly string MicrosoftUpdateString = ResourceService.UpdateManagerResource.GetString("MicrosoftUpdate");
        private readonly string MinDownloadSizeString = ResourceService.UpdateManagerResource.GetString("MinDownloadSize");
        private readonly string ModifyWindowsInsiderPreviewString = ResourceService.UpdateManagerResource.GetString("ModifyWindowsInsiderPreview");
        private readonly string MsrcSeverityString = ResourceService.UpdateManagerResource.GetString("MsrcSeverity");
        private readonly string NoString = ResourceService.UpdateManagerResource.GetString("No");
        private readonly string NotAvailableString = ResourceService.UpdateManagerResource.GetString("NotAvailable");
        private readonly string OpenPowerToolboxString = ResourceService.UpdateManagerResource.GetString("OpenPowerToolbox");
        private readonly string RecommendedCpuSpeedString = ResourceService.UpdateManagerResource.GetString("RecommendedCpuSpeed");
        private readonly string RecommendedHardDiskSpaceString = ResourceService.UpdateManagerResource.GetString("RecommendedHardDiskSpace");
        private readonly string RecommendedMemoryString = ResourceService.UpdateManagerResource.GetString("RecommendedMemory");
        private readonly string RestartPCString = ResourceService.UpdateManagerResource.GetString("RestartPC");
        private readonly string ReleaseNotesString = ResourceService.UpdateManagerResource.GetString("ReleaseNotes");
        private readonly string SupportedUrlString = ResourceService.UpdateManagerResource.GetString("SupportedUrl");
        private readonly string UninstallUpdateCanceledString = ResourceService.UpdateManagerResource.GetString("UninstallUpdateCanceled");
        private readonly string UninstallUpdateCompletedNeedRebootString = ResourceService.UpdateManagerResource.GetString("UninstallUpdateCompletedNeedReboot");
        private readonly string UninstallUpdateCompletedString = ResourceService.UpdateManagerResource.GetString("UninstallUpdateCompleted");
        private readonly string UninstallUpdateFailedWithCodeString = ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithCode");
        private readonly string UninstallUpdateFailedWithInformationString = ResourceService.UpdateManagerResource.GetString("UninstallUpdateFailedWithInformation");
        private readonly string UninstallUpdateProgressString = ResourceService.UpdateManagerResource.GetString("UninstallUpdateProgress");
        private readonly string UpdateAbortedString = ResourceService.UpdateManagerResource.GetString("UpdateAborted");
        private readonly string UpdateCancelingString = ResourceService.UpdateManagerResource.GetString("UpdateCanceling");
        private readonly string UpdateFailedString = ResourceService.UpdateManagerResource.GetString("UpdateFailed");
        private readonly string UpdateHistoryString = ResourceService.UpdateManagerResource.GetString("UpdateHistory");
        private readonly string UpdateInstalledString = ResourceService.UpdateManagerResource.GetString("UpdateInstalled");
        private readonly string UpdateNameString = ResourceService.UpdateManagerResource.GetString("UpdateName");
        private readonly string UpdateNotInstalledString = ResourceService.UpdateManagerResource.GetString("UpdateNotInstalled");
        private readonly string UpdatePrepareInstallingString = ResourceService.UpdateManagerResource.GetString("UpdatePrepareInstalling");
        private readonly string UpdatePrepareUninstallingString = ResourceService.UpdateManagerResource.GetString("UpdatePrepareUninstalling");
        private readonly string UpdateSucceedString = ResourceService.UpdateManagerResource.GetString("UpdateSucceed");
        private readonly string UpdateTypeDriverString = ResourceService.UpdateManagerResource.GetString("UpdateTypeDriver");
        private readonly string UpdateTypeSoftwareString = ResourceService.UpdateManagerResource.GetString("UpdateTypeSoftware");
        private readonly string WindowsInsiderConfigStatusString = ResourceService.UpdateManagerResource.GetString("WindowsInsiderConfigStatus");
        private readonly string WindowsStoreString = ResourceService.UpdateManagerResource.GetString("WindowsStore");
        private readonly string WindowsUpdateString = ResourceService.UpdateManagerResource.GetString("WindowsUpdate");
        private readonly string YesString = ResourceService.UpdateManagerResource.GetString("Yes");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly UpdateSession updateSession = new();
        private readonly UpdateServiceManager updateServiceManager = new();
        private readonly Dictionary<string, IDownloadJob> downloadJobDict = [];
        private readonly Dictionary<string, IInstallationJob> installationJobDict = [];
        private readonly Dictionary<string, IInstallationJob> uninstallationJobDict = [];
        private readonly Regex updateNumberRegex = new("""KB(\d)*""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private bool isInitialized;
        private UpdateSearcher updateSearcher;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private int _selectedIndex;

        private int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedIndex)));
                }
            }
        }

        private string _windowsUpdateVersion;

        private string WindowsUpdateVersion
        {
            get { return _windowsUpdateVersion; }

            set
            {
                if (!Equals(_windowsUpdateVersion, value))
                {
                    _windowsUpdateVersion = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowsUpdateVersion)));
                }
            }
        }

        private string _wuapiDllVersion;

        private string WuapiDllVersion
        {
            get { return _wuapiDllVersion; }

            set
            {
                if (!Equals(_wuapiDllVersion, value))
                {
                    _wuapiDllVersion = value;
                    PropertyChanged?.Invoke(this, new(nameof(WuapiDllVersion)));
                }
            }
        }

        private bool _isCheckUpdateEnabled;

        private bool IsCheckUpdateEnabled
        {
            get { return _isCheckUpdateEnabled; }

            set
            {
                if (!Equals(_isCheckUpdateEnabled, value))
                {
                    _isCheckUpdateEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCheckUpdateEnabled)));
                }
            }
        }

        private bool _isChecking;

        private bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsChecking)));
                }
            }
        }

        private bool _isAvailableHideEnabled;

        private bool IsAvailableHideEnabled
        {
            get { return _isAvailableHideEnabled; }

            set
            {
                if (!Equals(_isAvailableHideEnabled, value))
                {
                    _isAvailableHideEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsAvailableHideEnabled)));
                }
            }
        }

        private bool _isAvailableInstallEnabled;

        private bool IsAvailableInstallEnabled
        {
            get { return _isAvailableInstallEnabled; }

            set
            {
                if (!Equals(_isAvailableInstallEnabled, value))
                {
                    _isAvailableInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsAvailableInstallEnabled)));
                }
            }
        }

        private bool _isAvailableCancelInstallEnabled;

        private bool IsAvailableCancelInstallEnabled
        {
            get { return _isAvailableCancelInstallEnabled; }

            set
            {
                if (!Equals(_isAvailableCancelInstallEnabled, value))
                {
                    _isAvailableCancelInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsAvailableCancelInstallEnabled)));
                }
            }
        }

        private bool _isInstalledUninstallEnabled;

        private bool IsInstalledUninstallEnabled
        {
            get { return _isInstalledUninstallEnabled; }

            set
            {
                if (!Equals(_isInstalledUninstallEnabled, value))
                {
                    _isInstalledUninstallEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalledUninstallEnabled)));
                }
            }
        }

        private bool _isInstalledCancelInstallEnabled;

        private bool IsInstalledCancelInstallEnabled
        {
            get { return _isInstalledCancelInstallEnabled; }

            set
            {
                if (!Equals(_isInstalledCancelInstallEnabled, value))
                {
                    _isInstalledCancelInstallEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalledCancelInstallEnabled)));
                }
            }
        }

        private bool _isHiddenShowEnabled;

        private bool IsHiddenShowEnabled
        {
            get { return _isHiddenShowEnabled; }

            set
            {
                if (!Equals(_isHiddenShowEnabled, value))
                {
                    _isHiddenShowEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsHiddenShowEnabled)));
                }
            }
        }

        private bool _isExcludeDrivers;

        private bool IsExcludeDrivers
        {
            get { return _isExcludeDrivers; }

            set
            {
                if (!Equals(_isExcludeDrivers, value))
                {
                    _isExcludeDrivers = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsExcludeDrivers)));
                }
            }
        }

        private bool _isCleaning;

        private bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                if (!Equals(_isCleaning, value))
                {
                    _isCleaning = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCleaning)));
                }
            }
        }

        private bool _isUpdateCompletedNeedRebootPrompt;

        private bool IsUpdateCompletedNeedRebootPrompt
        {
            get { return _isUpdateCompletedNeedRebootPrompt; }

            set
            {
                if (!Equals(_isUpdateCompletedNeedRebootPrompt, value))
                {
                    _isUpdateCompletedNeedRebootPrompt = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdateCompletedNeedRebootPrompt)));
                }
            }
        }

        private bool _isPreviewChannelChangedNeedRebootPrompt;

        private bool IsPreviewChannelChangedNeedRebootPrompt
        {
            get { return _isPreviewChannelChangedNeedRebootPrompt; }

            set
            {
                if (!Equals(_isPreviewChannelChangedNeedRebootPrompt, value))
                {
                    _isPreviewChannelChangedNeedRebootPrompt = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPreviewChannelChangedNeedRebootPrompt)));
                }
            }
        }

        private bool _isIncludePotentiallySupersededUpdate;

        private bool IsIncludePotentiallySupersededUpdate
        {
            get { return _isIncludePotentiallySupersededUpdate; }

            set
            {
                if (!Equals(_isIncludePotentiallySupersededUpdate, value))
                {
                    _isIncludePotentiallySupersededUpdate = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsIncludePotentiallySupersededUpdate)));
                }
            }
        }

        private ComboBoxItemModel _selectedUpdateSource;

        private ComboBoxItemModel SelectedUpdateSource
        {
            get { return _selectedUpdateSource; }

            set
            {
                if (!Equals(_selectedUpdateSource, value))
                {
                    _selectedUpdateSource = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedUpdateSource)));
                }
            }
        }

        private List<RadioButtonItemModel> UpdateManagerSelectorRadioList { get; } = [];

        private List<ComboBoxItemModel> UpdateSourceList { get; } = [];

        private WinRTObservableCollection<UpdateModel> AvailableUpdateCollection { get; } = [];

        private WinRTObservableCollection<UpdateModel> InstalledUpdateCollection { get; } = [];

        private WinRTObservableCollection<UpdateModel> HiddenUpdateCollection { get; } = [];

        private WinRTObservableCollection<UpdateModel> UpdateHistoryCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal UpdateManagerPage()
        {
            InitializeComponent();
            InitializeData();
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
                IsExcludeDrivers = await GetIsExcludeDriversAsync();

                // 获取更新版本信息
                (string windowsUpdateVersion, string wuapiDllVersion) = await GetUpdateVersionInformationAsync();
                WindowsUpdateVersion = windowsUpdateVersion;
                WuapiDllVersion = wuapiDllVersion;

                // 获取历史更新记录
                List<UpdateModel> updateHistoryList = await Task.Run(GetUpdateHistoryList);
                UpdateHistoryCollection.Clear();
                foreach (UpdateModel updateHistoryItem in updateHistoryList)
                {
                    UpdateHistoryCollection.Add(updateHistoryItem);
                }
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 可用更新：取消更新
        /// </summary>
        private void OnAvailableCancelInstallExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel availableUpdate)
            {
                if (availableUpdate.IsUpdating)
                {
                    availableUpdate.UpdateProgress = UpdateCancelingString;
                    availableUpdate.IsUpdateCanceled = true;
                    UpdateAvailableSelectedItemsState();
                    CancelUpdate(availableUpdate.UpdateID);
                }
            }
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel availableUpdate)
            {
                UpdateManagerAvailableListView.SelectedItems.Remove(availableUpdate);

                if (!availableUpdate.IsUpdating && await HideUpdateAsync(availableUpdate.UpdateInformation) && availableUpdate.UpdateInformation.Update.IsHidden)
                {
                    UpdateHideResult(availableUpdate);
                    UpdateAvailableSelectedItemsState();
                    IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
                }
            }
        }

        /// <summary>
        /// 可用更新：安装
        /// </summary>
        private async void OnAvailableInstallExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel availableUpdate)
            {
                if (!availableUpdate.IsUpdating)
                {
                    availableUpdate.IsUpdating = true;
                    availableUpdate.IsUpdateCanceled = false;
                    availableUpdate.UpdatePercentage = 0;
                    availableUpdate.IsUpdatePreparing = true;
                    availableUpdate.UpdateProgress = UpdatePrepareInstallingString;

                    try
                    {
                        IsCheckUpdateEnabled = false;
                        bool updateResult = false;
                        UpdateCollection updateCollection = new() { availableUpdate.UpdateInformation.Update };

                        // 先下载更新
                        IDownloadResult downloadResult = await DownloadUpdateAsync(availableUpdate, updateCollection);
                        updateResult = downloadResult is not null && downloadResult.ResultCode is OperationResultCode.orcSucceeded;
                        UpdateDownloadResult(availableUpdate, downloadResult);

                        // 移除更新下载任务
                        downloadJobDict.Remove(availableUpdate.UpdateID);

                        // 保证更新下载成功后再进行安装更新
                        if (updateResult)
                        {
                            // 后面再安装更新
                            IInstallationResult installationResult = await InstallUpdateAsync(availableUpdate, updateCollection);
                            UpdateInstallResult(availableUpdate, installationResult);

                            // 移除更新安装任务
                            installationJobDict.Remove(availableUpdate.UpdateID);
                        }

                        // 当前更新的下载和安装所有步骤都已完成
                        UpdateAvailableSelectedItemsState();
                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableInstallExecuteRequested), 1, e);
                    }
                }
            }
        }

        /// <summary>
        /// 更新历史记录：复制更新描述信息
        /// </summary>
        private async void OnCopyInformationExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                if (await GetUpdateHistoryInformationStringAsync(updateItem.UpdateInformation, updateItem.WindowsDriverInformation) is string updateHistoryString)
                {
                    bool copyResult = CopyPasteHelper.CopyToClipboard(updateHistoryString);
                    await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
                }
            }
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel hiddenUpdate)
            {
                UpdateManagerHiddenListView.SelectedItems.Remove(hiddenUpdate);

                // 显示成功时更新界面显示内容
                if (await ShowUpdateAsync(hiddenUpdate.UpdateInformation) && !hiddenUpdate.UpdateInformation.Update.IsHidden)
                {
                    UpdateShowResult(hiddenUpdate);
                    UpdateAvailableSelectedItemsState();
                    IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
                }
            }
        }

        /// <summary>
        /// 已安装更新：取消卸载更新
        /// </summary>
        private void OnInstalledCancelUninstallExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel installedUpdate)
            {
                installedUpdate.UpdateProgress = UpdateCancelingString;
                installedUpdate.IsUpdateCanceled = true;
                UpdateInstalledSelectedItemState();
                CancelUninstallUpdate(installedUpdate.UpdateID);
            }
        }

        /// <summary>
        /// 已安装更新：卸载更新
        /// </summary>
        private async void OnInstalledUninstallExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel installedUpdate)
            {
                bool isUpdating = false;

                if (!installedUpdate.IsUpdating)
                {
                    isUpdating = true;
                    installedUpdate.IsUpdating = true;
                    installedUpdate.IsUpdateCanceled = false;
                    installedUpdate.UpdatePercentage = 0;
                    installedUpdate.IsUpdatePreparing = true;
                    installedUpdate.UpdateProgress = UpdatePrepareUninstallingString;
                }

                if (isUpdating)
                {
                    try
                    {
                        IsCheckUpdateEnabled = false;
                        UpdateCollection updateCollection = new() { installedUpdate.UpdateInformation.Update };

                        // 卸载更新
                        IInstallationResult installationResult = await UninstallUpdateAsync(installedUpdate, updateCollection);
                        UpdateUninstallResult(installedUpdate, installationResult);

                        // 移除更新卸载任务
                        uninstallationJobDict.Remove(installedUpdate.UpdateID);
                        UpdateInstalledSelectedItemState();

                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnInstalledUninstallExecuteRequested), 1, e);
                    }
                }
            }
        }

        /// <summary>
        /// 了解详细信息
        /// </summary>
        private void OnLearnMoreExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string supportUrl && !string.IsNullOrEmpty(supportUrl))
            {
                LearnMore(supportUrl);
            }
        }

        /// <summary>
        /// 更新历史记录：打开更新对应的受支持的链接
        /// </summary>
        private void OnOpenSupportUrlExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string supportUrl && !string.IsNullOrEmpty(supportUrl))
            {
                OpenSupportUrl(supportUrl);
            }
        }

        /// <summary>
        /// 已安装更新：使用命令卸载更新
        /// </summary>
        private void OnUninstallWithCmdExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string title && !string.IsNullOrEmpty(title))
            {
                UninstallWithCmd(title);
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsCheckUpdateEnabled = false;
            IsChecking = true;
            if (SelectedUpdateSource is not null)
            {
                (bool searchResult, List<UpdateModel> availableUpdateList, List<UpdateModel> installedUpdateList, List<UpdateModel> hiddenUpdateList, List<UpdateModel> updateHistoryList) = await CheckUpdateAsync(IsIncludePotentiallySupersededUpdate, Convert.ToString(SelectedUpdateSource.SelectedValue));

                if (searchResult)
                {
                    AvailableUpdateCollection.Clear();
                    if (availableUpdateList is not null && availableUpdateList.Count > 0)
                    {
                        foreach (UpdateModel availableUpdateItem in availableUpdateList)
                        {
                            AvailableUpdateCollection.Add(availableUpdateItem);
                        }
                    }

                    InstalledUpdateCollection.Clear();
                    if (installedUpdateList is not null && installedUpdateList.Count > 0)
                    {
                        foreach (UpdateModel installedUpdateItem in installedUpdateList)
                        {
                            InstalledUpdateCollection.Add(installedUpdateItem);
                        }
                    }

                    HiddenUpdateCollection.Clear();
                    if (hiddenUpdateList is not null && hiddenUpdateList.Count > 0)
                    {
                        foreach (UpdateModel hiddenUpdateItem in hiddenUpdateList)
                        {
                            HiddenUpdateCollection.Add(hiddenUpdateItem);
                        }
                    }

                    UpdateHistoryCollection.Clear();
                    if (updateHistoryList is not null && updateHistoryList.Count > 0)
                    {
                        foreach (UpdateModel updateHistoryItem in updateHistoryList)
                        {
                            UpdateHistoryCollection.Add(updateHistoryItem);
                        }
                    }
                }
            }
            IsCheckUpdateEnabled = true;
            IsChecking = false;
        }

        /// <summary>
        /// 打开 Windows 更新
        /// </summary>
        private void OnWindowsUpdateClicked(object sender, RoutedEventArgs args)
        {
            OpenWindowsUpdate();
        }

        /// <summary>
        /// 打开 Windows 更新历史记录
        /// </summary>
        private void OnWindowsUpdateHistoryClicked(object sender, RoutedEventArgs args)
        {
            OpenWindowsUpdateHistory();
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenWIPSettings();
        }

        /// <summary>
        /// 查看 Windows 更新的版本信息
        /// </summary>
        private void OnWindowsUpdateInformationClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(ViewMoreButton);
        }

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && !Equals(SelectedIndex, radioButtons.SelectedIndex))
            {
                SelectedIndex = radioButtons.SelectedIndex;
            }
        }

        /// <summary>
        /// 可用更新：全选
        /// </summary>
        private void OnAvailableSelectAllClicked(object sender, RoutedEventArgs args)
        {
            UpdateManagerAvailableListView.SelectAll();
            UpdateAvailableSelectedItemsState();
        }

        /// <summary>
        /// 可用更新：全部不选
        /// </summary>
        private void OnAvailableSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            UpdateManagerAvailableListView.DeselectRange(new(0, (uint)UpdateManagerAvailableListView.Items.Count));
            IsAvailableHideEnabled = false;
            IsAvailableInstallEnabled = false;
            IsAvailableCancelInstallEnabled = false;
        }

        /// <summary>
        /// 可用更新：全部反选
        /// </summary>
        private void OnAvailableSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            List<object> selectedItemList = [.. UpdateManagerAvailableListView.SelectedItems];

            foreach (object item in UpdateManagerAvailableListView.Items)
            {
                if (selectedItemList.Contains(item))
                {
                    UpdateManagerAvailableListView.SelectedItems.Remove(item);
                }
                else
                {
                    UpdateManagerAvailableListView.SelectedItems.Add(item);
                }
            }

            UpdateAvailableSelectedItemsState();
        }

        /// <summary>
        /// 可用更新：安装选定的更新
        /// </summary>
        private void OnAvailableInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> installList = [];

            foreach (object availableUpdateItemObj in UpdateManagerAvailableListView.SelectedItems)
            {
                if (availableUpdateItemObj is UpdateModel availableUpdateItem)
                {
                    installList.Add(availableUpdateItem);
                }
            }

            UpdateManagerAvailableListView.SelectedItems.Clear();
            IsCheckUpdateEnabled = false;
            foreach (UpdateModel installItem in installList)
            {
                if (!installItem.IsUpdating)
                {
                    // 更新更新项的状态
                    installItem.IsUpdating = true;
                    installItem.IsUpdateCanceled = false;
                    installItem.UpdatePercentage = 0;
                    installItem.IsUpdatePreparing = true;
                    installItem.UpdateProgress = UpdatePrepareInstallingString;

                    synchronizationContext.Post(async (_) =>
                    {
                        bool updateResult = false;
                        UpdateCollection updateCollection = new() { installItem.UpdateInformation.Update };

                        // 先下载更新
                        IDownloadResult downloadResult = await DownloadUpdateAsync(installItem, updateCollection);
                        updateResult = downloadResult is not null && downloadResult.ResultCode is OperationResultCode.orcSucceeded;
                        UpdateDownloadResult(installItem, downloadResult);

                        // 移除更新下载任务
                        downloadJobDict.Remove(installItem.UpdateID);

                        // 保证更新下载成功后再进行安装更新
                        if (updateResult)
                        {
                            IInstallationResult installationResult = await InstallUpdateAsync(installItem, updateCollection);
                            UpdateInstallResult(installItem, installationResult);

                            // 移除更新安装任务
                            installationJobDict.Remove(installItem.UpdateID);
                        }

                        UpdateAvailableSelectedItemsState();
                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }, null);
                }
            }

            IsAvailableHideEnabled = false;
            IsAvailableInstallEnabled = false;
            IsAvailableCancelInstallEnabled = false;
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = [];

            foreach (object availableUpdateItemObj in UpdateManagerAvailableListView.SelectedItems)
            {
                if (availableUpdateItemObj is UpdateModel availableUpdateItem)
                {
                    hideList.Add(availableUpdateItem);
                }
            }

            UpdateManagerAvailableListView.SelectedItems.Clear();
            foreach (UpdateModel hideItem in hideList)
            {
                if (!hideItem.IsUpdating && await HideUpdateAsync(hideItem.UpdateInformation) && hideItem.UpdateInformation.Update.IsHidden)
                {
                    UpdateHideResult(hideItem);
                }
            }

            IsAvailableHideEnabled = false;
            IsAvailableInstallEnabled = false;
            IsAvailableCancelInstallEnabled = false;
            IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 可用更新：取消安装选定的更新
        /// </summary>
        private void OnAvailableCancelInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> cancelList = [];

            foreach (object availableUpdateItemObj in UpdateManagerAvailableListView.SelectedItems)
            {
                if (availableUpdateItemObj is UpdateModel availableUpdateItem)
                {
                    cancelList.Add(availableUpdateItem);
                }
            }

            foreach (UpdateModel cancelItem in cancelList)
            {
                cancelItem.UpdateProgress = UpdateCancelingString;
                cancelItem.IsUpdateCanceled = true;
                CancelUpdate(cancelItem.UpdateID);
            }

            UpdateAvailableSelectedItemsState();
        }

        /// <summary>
        /// 可用更新：选中项发生变化时触发的事件
        /// </summary>
        private void OnAvailableSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            UpdateAvailableSelectedItemsState();
        }

        /// <summary>
        /// 已安装更新：全选
        /// </summary>
        private void OnInstalledSelectAllClicked(object sender, RoutedEventArgs args)
        {
            UpdateManagerInstalledListView.SelectAll();
            UpdateInstalledSelectedItemState();
        }

        /// <summary>
        /// 已安装更新：全部不选
        /// </summary>
        private void OnInstalledSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            UpdateManagerInstalledListView.DeselectRange(new(0, (uint)UpdateManagerInstalledListView.Items.Count));
            IsInstalledUninstallEnabled = false;
            IsInstalledCancelInstallEnabled = false;
        }

        /// <summary>
        /// 已安装更新：全部反选
        /// </summary>
        private void OnInstalledSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            List<object> selectedItemList = [.. UpdateManagerInstalledListView.SelectedItems];

            foreach (object item in UpdateManagerInstalledListView.Items)
            {
                if (selectedItemList.Contains(item))
                {
                    UpdateManagerInstalledListView.SelectedItems.Remove(item);
                }
                else
                {
                    UpdateManagerInstalledListView.SelectedItems.Add(item);
                }
            }

            UpdateInstalledSelectedItemState();
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUninstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> uninstallList = [];

            foreach (object installedUpdateItemObj in UpdateManagerInstalledListView.SelectedItems)
            {
                if (installedUpdateItemObj is UpdateModel installedUpdateItem)
                {
                    uninstallList.Add(installedUpdateItem);
                }
            }

            UpdateManagerInstalledListView.SelectedItems.Clear();
            IsCheckUpdateEnabled = false;
            foreach (UpdateModel uninstallItem in uninstallList)
            {
                if (!uninstallItem.IsUpdating)
                {
                    // 更新卸载项的状态
                    uninstallItem.IsUpdating = true;
                    uninstallItem.IsUpdateCanceled = false;
                    uninstallItem.UpdatePercentage = 0;
                    uninstallItem.IsUpdatePreparing = true;
                    uninstallItem.UpdateProgress = UpdatePrepareUninstallingString;

                    synchronizationContext.Post(async (_) =>
                    {
                        UpdateCollection updateCollection = new() { uninstallItem.UpdateInformation.Update };

                        // 卸载更新
                        IInstallationResult installationResult = await UninstallUpdateAsync(uninstallItem, updateCollection);
                        UpdateUninstallResult(uninstallItem, installationResult);

                        // 移除更新卸载任务
                        uninstallationJobDict.Remove(uninstallItem.UpdateID);

                        // 当前更新的卸载所有步骤都已完成
                        UpdateInstalledSelectedItemState();

                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }, null);
                }
            }
            UpdateInstalledSelectedItemState();
        }

        /// <summary>
        /// 已安装更新：取消卸载更新
        /// </summary>
        private void OnInstalledCancelUninstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> cancelList = [];

            foreach (object installedUpdateItemObj in UpdateManagerInstalledListView.SelectedItems)
            {
                if (installedUpdateItemObj is UpdateModel installedUpdateItem)
                {
                    cancelList.Add(installedUpdateItem);
                }
            }

            foreach (UpdateModel cancelItem in cancelList)
            {
                cancelItem.UpdateProgress = UpdateCancelingString;
                cancelItem.IsUpdateCanceled = true;
                CancelUninstallUpdate(cancelItem.UpdateID);
            }

            UpdateInstalledSelectedItemState();
        }

        /// <summary>
        /// 已安装更新：选中项发生变化时触发的事件
        /// </summary>
        private void OnInstalledSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            UpdateInstalledSelectedItemState();
        }

        /// <summary>
        /// 隐藏更新：全选
        /// </summary>
        private void OnHiddenSelectAllClicked(object sender, RoutedEventArgs args)
        {
            UpdateManagerHiddenListView.SelectAll();
            IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 隐藏更新：全部不选
        /// </summary>
        private void OnHiddenSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            UpdateManagerHiddenListView.DeselectRange(new(0, (uint)UpdateManagerHiddenListView.Items.Count));
            IsHiddenShowEnabled = false;
        }

        private void OnHiddenSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            List<object> selectedItemList = [.. UpdateManagerHiddenListView.SelectedItems];

            foreach (object item in UpdateManagerHiddenListView.Items)
            {
                if (selectedItemList.Contains(item))
                {
                    UpdateManagerHiddenListView.SelectedItems.Remove(item);
                }
                else
                {
                    UpdateManagerHiddenListView.SelectedItems.Add(item);
                }
            }

            IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> showList = [];

            foreach (object hiddenUpdateItemObj in UpdateManagerHiddenListView.SelectedItems)
            {
                if (hiddenUpdateItemObj is UpdateModel hiddenUpdateItem)
                {
                    showList.Add(hiddenUpdateItem);
                }
            }

            UpdateManagerHiddenListView.SelectedItems.Clear();
            foreach (UpdateModel showItem in showList)
            {
                if (await ShowUpdateAsync(showItem.UpdateInformation) && !showItem.UpdateInformation.Update.IsHidden)
                {
                    UpdateShowResult(showItem);
                }
            }

            UpdateAvailableSelectedItemsState();
            IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 隐藏更新：选中项发生变化时触发的事件
        /// </summary>
        private void OnHiddenSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            IsHiddenShowEnabled = UpdateManagerHiddenListView.SelectedItems.Count > 0 && HiddenUpdateCollection.Any(item => item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 重启以完成后续更新的安装或卸载
        /// </summary>
        private void OnRebootClicked(object sender, RoutedEventArgs args)
        {
            RebootPC();
        }

        /// <summary>
        /// 关闭按钮关闭通知显示
        /// </summary>
        private void OnHideNeedRebootPromptClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string needRebootPrompt && !string.IsNullOrEmpty(needRebootPrompt))
            {
                if (string.Equals(needRebootPrompt, "UpdateCompletedNeedReboot", StringComparison.OrdinalIgnoreCase))
                {
                    IsUpdateCompletedNeedRebootPrompt = false;
                }
                else if (string.Equals(needRebootPrompt, "PreviewChannelChangedNeedReboot", StringComparison.OrdinalIgnoreCase))
                {
                    IsPreviewChannelChangedNeedRebootPrompt = false;
                }
            }
        }

        /// <summary>
        /// 搜索结果中是否包含被取代的更新
        /// </summary>
        private void OnIncludePotentiallySupersededUpdateToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsIncludePotentiallySupersededUpdate, toggleSwitch.IsOn))
            {
                IsIncludePotentiallySupersededUpdate = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置更新源
        /// </summary>
        private void OnUpdateSourceSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedUpdateSource, comboBox.SelectedItem))
            {
                SelectedUpdateSource = comboBox.SelectedItem is ComboBoxItemModel updateSource ? updateSource : null;
            }
        }

        /// <summary>
        /// Windows 更新不包括驱动程序
        /// </summary>
        private async void OnExcludeDriversToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsExcludeDrivers, toggleSwitch.IsOn))
            {
                IsExcludeDrivers = toggleSwitch.IsOn;
                await SetIsExcludeDriversAsync(IsExcludeDrivers);
                IsExcludeDrivers = await GetIsExcludeDriversAsync();
            }
        }

        /// <summary>
        /// 清除历史更新记录
        /// </summary>
        private async void OnCleanUpdateHistoryClicked(object sender, RoutedEventArgs args)
        {
            IsCleaning = true;
            bool cleanResult = await CleanUpdateHistoryAsync();
            IsCleaning = false;
            synchronizationContext.Send(async (_) =>
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CleanUpdateHistory, cleanResult));
            }, null);
            // 获取历史更新记录
            List<UpdateModel> updateHistoryList = await Task.Run(GetUpdateHistoryList);
            UpdateHistoryCollection.Clear();
            foreach (UpdateModel updateHistoryItem in updateHistoryList)
            {
                UpdateHistoryCollection.Add(updateHistoryItem);
            }
        }

        /// <summary>
        /// 更改设备的预览计划频道
        /// </summary>
        private async void OnPreviewChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string previewChannel && !string.IsNullOrEmpty(previewChannel))
            {
                ExitCustomPreviewChannel();

                if (!string.Equals(previewChannel, "ExitPreviewChannel", StringComparison.OrdinalIgnoreCase))
                {
                    EnterCustomPreviewChannel(previewChannel);
                }

                IsPreviewChannelChangedNeedRebootPrompt = true;
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.InsiderPreviewSettings));
            }
        }

        /// <summary>
        /// 下载更新中，进度变化时更新 UI 下载进度
        /// </summary>
        private void OnDownloadProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            IDownloadProgress downloadProgress = (sender as DownloadProgressChangedCallback).CallbackArgs.Progress;
            double percentage = downloadProgress.CurrentUpdatePercentComplete;

            // 初始化当前更新的下载
            if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphInitializing)
            {
                synchronizationContext.Post(_ =>
                {
                    updateItem.IsUpdatePreparing = false;
                    updateItem.UpdatePercentage = percentage / 2;
                    updateItem.UpdateProgress = string.Format(DownloadUpdateProgressInitializingString, percentage);
                }, null);
            }
            // 下载当前更新
            else if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphDownloading)
            {
                synchronizationContext.Post(_ =>
                {
                    updateItem.IsUpdatePreparing = false;
                    updateItem.UpdatePercentage = percentage / 2;
                    updateItem.UpdateProgress = string.Format(DownloadUpdateProgressDownloadingString, percentage);
                }, null);
            }
            else if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphVerifying)
            {
                synchronizationContext.Post(_ =>
                {
                    updateItem.IsUpdatePreparing = false;
                    updateItem.UpdatePercentage = percentage / 2;
                    updateItem.UpdateProgress = string.Format(DownloadUpdateProgressVerifyingString, percentage);
                }, null);
            }
        }

        /// <summary>
        /// 安装更新中，进度变化时更改 UI 卸载进度
        /// </summary>
        private void OnInstallationProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            IInstallationProgress installationProgress = (sender as InstallationProgressChangedCallback).CallbackArgs.Progress;
            double percentage = installationProgress.CurrentUpdatePercentComplete;

            synchronizationContext.Post(_ =>
            {
                updateItem.UpdatePercentage = 50 + percentage / 2;
                updateItem.UpdateProgress = string.Format(InstallUpdateProgressString, percentage);
            }, null);
        }

        /// <summary>
        /// 卸载更新中，进度变化时更改 UI 卸载进度
        /// </summary>
        private void OnUninstallationProgressChanged(object sender, EventArgs args, UpdateModel updateItem)
        {
            IInstallationProgress installationProgress = (sender as InstallationProgressChangedCallback).CallbackArgs.Progress;
            double percentage = installationProgress.CurrentUpdatePercentComplete;

            synchronizationContext.Post(_ =>
            {
                updateItem.UpdatePercentage = percentage;
                updateItem.UpdateProgress = string.Format(UninstallUpdateProgressString, percentage);
            }, null);
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            UpdateManagerSelectorRadioList.Add(new() { SelectorName = AvailableUpdateString });
            UpdateManagerSelectorRadioList.Add(new() { SelectorName = InstalledUpdateString });
            UpdateManagerSelectorRadioList.Add(new() { SelectorName = HiddenUpdateString });
            UpdateManagerSelectorRadioList.Add(new() { SelectorName = UpdateHistoryString });
            UpdateSourceList.Add(new() { SelectedValue = "Microsoft Update", DisplayMember = MicrosoftUpdateString });
            UpdateSourceList.Add(new() { SelectedValue = "DCat Flighting Prod", DisplayMember = DCatFlightingProdString });
            UpdateSourceList.Add(new() { SelectedValue = "Windows Store(DCat Prod)", DisplayMember = WindowsStoreString });
            UpdateSourceList.Add(new() { SelectedValue = "Windows Update", DisplayMember = WindowsUpdateString });
            SelectedIndex = 0;
            SelectedUpdateSource = UpdateSourceList[0];
            IsCheckUpdateEnabled = true;
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async Task<(bool, List<UpdateModel>, List<UpdateModel>, List<UpdateModel>, List<UpdateModel>)> CheckUpdateAsync(bool isIncludePotentiallySupersededUpdate, string updateSource)
        {
            if (string.IsNullOrEmpty(updateSource))
            {
                return default;
            }

            return await Task.Run(() =>
            {
                bool result = false;
                List<UpdateModel> availableUpdateList = [];
                List<UpdateModel> installedUpdateList = [];
                List<UpdateModel> hiddenUpdateList = [];
                List<UpdateModel> updateHistoryList = [];

                // 设置搜索结果中是否接收已替代的更新
                updateSearcher.IncludePotentiallySupersededUpdates = isIncludePotentiallySupersededUpdate;
                updateSearcher.ServerSelection = ServerSelection.ssDefault;
                updateSearcher.IgnoreDownloadPriority = true;

                // 设置更新源
                foreach (IUpdateService2 updateService in updateServiceManager.Services)
                {
                    if (string.Equals(updateService.Name, updateSource, StringComparison.OrdinalIgnoreCase))
                    {
                        updateSearcher.ServiceID = updateService.ServiceID;
                        break;
                    }
                }

                try
                {
                    AutoResetEvent searchEvent = new(false);
                    string query = "(IsInstalled = 0 and IsHidden = 0 and DeploymentAction=*) or (IsInstalled = 1 and IsHidden = 0 and DeploymentAction=*) or (IsHidden = 1 and DeploymentAction=*)";
                    SearchCompletedCallback searchCompletedCallback = new();
                    searchCompletedCallback.SearchCompleted += (sender, args) => searchEvent.Set();
                    ISearchJob searchJob = updateSearcher.BeginSearch(query, searchCompletedCallback, null);
                    searchEvent.WaitOne();
                    searchEvent.Dispose();
                    ISearchResult searchResult = updateSearcher.EndSearch(searchJob);

                    // 搜索更新内容
                    if (searchResult is not null)
                    {
                        // 读取已搜索到的更新
                        foreach (IUpdate5 update in searchResult.Updates)
                        {
                            UpdateInformation updateInformation = new()
                            {
                                Update = update,
                                Description = update.Description,
                                EulaText = update.EulaText,
                                IsBeta = update.IsBeta,
                                IsHidden = update.IsHidden,
                                IsInstalled = update.IsInstalled,
                                IsMandatory = update.IsMandatory,
                                IsCanUninstall = update.IsUninstallable,
                                MaxDownloadSize = update.MaxDownloadSize,
                                MinDownloadSize = update.MinDownloadSize,
                                MsrcSeverity = update.MsrcSeverity,
                                RecommendedCpuSpeed = update.RecommendedCpuSpeed,
                                RecommendedHardDiskSpace = update.RecommendedHardDiskSpace,
                                RecommendedMemory = update.RecommendedMemory,
                                RebootRequired = update.RebootRequired,
                                ReleaseNotes = update.ReleaseNotes,
                                SupportURL = update.SupportUrl,
                                Title = update.Title,
                                UpdateType = update.Type,
                                UpdateID = string.IsNullOrEmpty(update.Identity.UpdateID) ? Guid.NewGuid().ToString() : update.Identity.UpdateID,
                            };

                            foreach (object cveID in update.CveIDs)
                            {
                                updateInformation.CveIDList.Add(Convert.ToString(cveID));
                            }

                            foreach (object moreInfoUrl in update.MoreInfoUrls)
                            {
                                updateInformation.KBArticleIDList.Add(Convert.ToString(moreInfoUrl));
                            }

                            foreach (object moreInfoUrl in update.MoreInfoUrls)
                            {
                                updateInformation.MoreInfoList.Add(Convert.ToString(moreInfoUrl));
                            }

                            foreach (object language in update.Languages)
                            {
                                updateInformation.SupportedLanguageList.Add(Convert.ToString(language));
                            }

                            UpdateModel updateItem = new()
                            {
                                UpdateInformation = updateInformation,
                                Description = string.IsNullOrEmpty(updateInformation.Description) ? NotAvailableString : updateInformation.Description,
                                EulaText = string.IsNullOrEmpty(updateInformation.EulaText) ? NotAvailableString : updateInformation.EulaText,
                                IsBeta = updateInformation.IsBeta ? YesString : NoString,
                                IsMandatory = updateInformation.IsMandatory ? YesString : NoString,
                                MaxDownloadSize = VolumeSizeHelper.ConvertVolumeSizeToString(Convert.ToDouble(updateInformation.Update.MaxDownloadSize)),
                                MinDownloadSize = VolumeSizeHelper.ConvertVolumeSizeToString(Convert.ToDouble(updateInformation.Update.MinDownloadSize)),
                                MsrcSeverity = string.IsNullOrEmpty(updateInformation.MsrcSeverity) ? NotAvailableString : updateInformation.MsrcSeverity,
                                RecommendedCpuSpeed = updateInformation.RecommendedCpuSpeed is 0 ? NotAvailableString : string.Format("{0} MHz", updateInformation.RecommendedCpuSpeed),
                                RecommendedHardDiskSpace = updateInformation.RecommendedHardDiskSpace is 0 ? NotAvailableString : string.Format("{0} MB", updateInformation.RecommendedHardDiskSpace),
                                RecommendedMemory = updateInformation.RecommendedMemory is 0 ? NotAvailableString : string.Format("{0} MB", updateInformation.RecommendedMemory),
                                ReleaseNotes = string.IsNullOrEmpty(updateInformation.ReleaseNotes) ? NotAvailableString : updateInformation.ReleaseNotes,
                                SupportURL = updateInformation.SupportURL,
                                Title = string.IsNullOrEmpty(updateInformation.Title) ? NotAvailableString : updateInformation.Title,
                                UpdateID = updateInformation.UpdateID,
                                IsUpdating = false,
                                UpdateProgress = string.Empty,
                                IsUpdateCanceled = false,
                                IsUpdatePreparing = false,
                                UpdatePercentage = 0,
                                WindowsDriverInformation = new(),
                                DeviceProblemNumber = string.Empty,
                                DriverClass = string.Empty,
                                DriverHardwareID = string.Empty,
                                DriverManufacturer = string.Empty,
                                DriverModel = string.Empty,
                                DriverProvider = string.Empty,
                                DriverVerDate = string.Empty,
                            };

                            if (updateInformation.UpdateType is UpdateType.utSoftware)
                            {
                                updateItem.UpdateType = UpdateTypeSoftwareString;
                            }
                            else if (updateInformation.UpdateType is UpdateType.utDriver)
                            {
                                updateItem.UpdateType = UpdateTypeDriverString;
                                IWindowsDriverUpdate5 windowsDriverUpdate = update as IWindowsDriverUpdate5;

                                if (windowsDriverUpdate is not null)
                                {
                                    WindowsDriverInformation windowsDriverInformation = new()
                                    {
                                        DeviceProblemNumber = windowsDriverUpdate.DeviceProblemNumber,
                                        DriverClass = windowsDriverUpdate.DriverClass,
                                        DriverHardwareID = windowsDriverUpdate.DriverHardwareID,
                                        DriverManufacturer = windowsDriverUpdate.DriverManufacturer,
                                        DriverModel = windowsDriverUpdate.DriverModel,
                                        DriverProvider = windowsDriverUpdate.DriverProvider,
                                        DriverVerDate = windowsDriverUpdate.DriverVerDate,
                                        WindowsDriverUpdate = windowsDriverUpdate
                                    };

                                    updateItem.WindowsDriverInformation = windowsDriverInformation;
                                    updateItem.DeviceProblemNumber = Convert.ToString(windowsDriverInformation.DeviceProblemNumber);
                                    updateItem.DriverClass = string.IsNullOrEmpty(windowsDriverInformation.DriverClass) ? NotAvailableString : windowsDriverInformation.DriverClass;
                                    updateItem.DriverHardwareID = string.IsNullOrEmpty(windowsDriverInformation.DriverHardwareID) ? NotAvailableString : windowsDriverInformation.DriverHardwareID;
                                    updateItem.DriverManufacturer = string.IsNullOrEmpty(windowsDriverInformation.DriverManufacturer) ? NotAvailableString : windowsDriverInformation.DriverManufacturer;
                                    updateItem.DriverModel = string.IsNullOrEmpty(windowsDriverInformation.DriverModel) ? NotAvailableString : windowsDriverInformation.DriverModel;
                                    updateItem.DriverProvider = string.IsNullOrEmpty(windowsDriverInformation.DriverProvider) ? NotAvailableString : windowsDriverInformation.DriverProvider;
                                    updateItem.DriverVerDate = windowsDriverInformation.DriverVerDate.ToString("yyyy/MM/dd");
                                }
                            }
                            else
                            {
                                updateItem.UpdateType = NotAvailableString;
                            }

                            updateItem.CveIDList.AddRange(updateInformation.CveIDList);
                            updateItem.KBArticleIDList.AddRange(updateInformation.KBArticleIDList);
                            updateItem.SupportedLanguageList.AddRange(updateInformation.SupportedLanguageList);
                            updateItem.MoreInfoList.AddRange(updateInformation.MoreInfoList);

                            // 隐藏的更新
                            if (updateInformation.Update.IsHidden)
                            {
                                updateItem.UpdateProgress = string.Empty;
                                hiddenUpdateList.Add(updateItem);
                            }
                            // 已安装的更新
                            else if (update.IsInstalled)
                            {
                                updateItem.UpdateProgress = UpdateInstalledString;
                                installedUpdateList.Add(updateItem);
                            }
                            // 可用更新
                            else
                            {
                                updateItem.UpdateProgress = UpdateNotInstalledString;
                                availableUpdateList.Add(updateItem);
                            }
                        }
                        result = true;
                    }

                    updateHistoryList.AddRange(GetUpdateHistoryList());
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(CheckUpdateAsync), 1, e);
                }

                return (result, availableUpdateList, installedUpdateList, hiddenUpdateList, updateHistoryList);
            });
        }

        /// <summary>
        /// 获取历史更新记录
        /// </summary>
        private List<UpdateModel> GetUpdateHistoryList()
        {
            List<UpdateModel> updateHistoryList = [];

            try
            {
                int updateHistoryCount = updateSearcher.GetTotalHistoryCount();

                if (updateHistoryCount > 0)
                {
                    foreach (IUpdateHistoryEntry2 updateHistoryEntry in updateSearcher.QueryHistory(0, updateHistoryCount))
                    {
                        if (!string.IsNullOrEmpty(updateHistoryEntry.Title))
                        {
                            UpdateHistoryInformation updateHistoryInformation = new()
                            {
                                ClientApplicationID = updateHistoryEntry.ClientApplicationID,
                                Date = updateHistoryEntry.Date,
                                HResult = updateHistoryEntry.HResult,
                                OperationResultCode = updateHistoryEntry.ResultCode,
                                SupportUrl = updateHistoryEntry.SupportUrl,
                                Title = updateHistoryEntry.Title,
                                UpdateHistoryEntry = updateHistoryEntry,
                                UpdateID = !string.IsNullOrEmpty(updateHistoryEntry.UpdateIdentity.UpdateID) ? Guid.NewGuid().ToString() : updateHistoryEntry.UpdateIdentity.UpdateID
                            };

                            UpdateModel update = new()
                            {
                                UpdateHistoryInformation = updateHistoryInformation,
                                Date = updateHistoryInformation.Date.ToString("yyyy/MM/dd"),
                                HistoryUpdateResult = GetUpdateResult(updateHistoryInformation.OperationResultCode, updateHistoryInformation.Date, updateHistoryInformation.HResult),
                                SupportURL = updateHistoryInformation.SupportUrl,
                                Title = updateHistoryInformation.Title,
                                UpdateID = updateHistoryInformation.UpdateID
                            };

                            updateHistoryList.Add(update);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(GetUpdateHistoryList), 1, e);
            }

            return updateHistoryList;
        }

        /// <summary>
        /// 获取更新的安装状态
        /// </summary>
        private string GetUpdateResult(OperationResultCode operationResultCode, DateTimeOffset date, int hResult)
        {
            switch (operationResultCode)
            {
                case OperationResultCode.orcAborted:
                    {
                        return string.Format(UpdateAbortedString, date.ToString("yyyy/MM/dd"), hResult);
                    }
                case OperationResultCode.orcSucceeded:
                    {
                        return string.Format(UpdateSucceedString, date.ToString("yyyy/MM/dd"));
                    }
                case OperationResultCode.orcFailed:
                    {
                        return string.Format(UpdateFailedString, date.ToString("yyyy/MM/dd"), hResult);
                    }
                default:
                    {
                        return NotAvailableString;
                    }
            }
        }

        /// <summary>
        /// 获取 Windows 更新是否不包括驱动程序
        /// </summary>
        private async Task<bool> GetIsExcludeDriversAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(GetIsExcludeDriversAsync), 1, e);
                    return false;
                }
            });
        }

        /// <summary>
        /// 设置 Windows 更新是否不包括驱动程序
        /// </summary>
        private async Task SetIsExcludeDriversAsync(bool isExcludeDrivers)
        {
            await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", isExcludeDrivers);
                }
            });
        }

        /// <summary>
        /// 获取更新版本
        /// </summary>
        private async Task<(string, string)> GetUpdateVersionInformationAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    updateSession.ClientApplicationID = nameof(PowerToolbox) + ":" + Guid.NewGuid().ToString();
                    updateSearcher = updateSession.CreateUpdateSearcher() as UpdateSearcher;
                    updateSearcher.IgnoreDownloadPriority = true;
                    WindowsUpdateAgentInfo windowsUpdateAgentInfo = new();
                    object apiMajorVersion = windowsUpdateAgentInfo.GetInfo("ApiMajorVersion");
                    object apiMinorVersion = windowsUpdateAgentInfo.GetInfo("ApiMinorVersion");
                    object productVersionString = windowsUpdateAgentInfo.GetInfo("ProductVersionString");
                    return (string.Format("{0}.{1}", apiMajorVersion, apiMinorVersion), Convert.ToString(productVersionString));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(GetUpdateVersionInformationAsync), 1, e);
                    return default;
                }
            });
        }

        /// <summary>
        /// 取消更新
        /// </summary>
        private void CancelUpdate(string updateId)
        {
            if (string.IsNullOrEmpty(updateId))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (downloadJobDict.TryGetValue(updateId, out IDownloadJob downloadJob))
                    {
                        downloadJob.RequestAbort();
                        downloadJobDict.Remove(updateId);
                    }

                    if (installationJobDict.TryGetValue(updateId, out IInstallationJob installationJob))
                    {
                        installationJob.RequestAbort();
                        installationJobDict.Remove(updateId);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(CancelUpdate), 1, e);
                }
            });
        }

        /// <summary>
        /// 隐藏更新
        /// </summary>
        private async Task<bool> HideUpdateAsync(UpdateInformation updateInformation)
        {
            if (updateInformation is null)
            {
                return false;
            }

            return await Task.Run(() =>
            {
                bool result = false;

                // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                if (RuntimeHelper.IsElevated)
                {
                    try
                    {
                        updateInformation.IsHidden = true;
                        updateInformation.Update.IsHidden = true;
                        result = true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(HideUpdateAsync), 1, e);
                    }
                }

                return result;
            });
        }

        /// <summary>
        /// 下载更新
        /// </summary>
        private async Task<IDownloadResult> DownloadUpdateAsync(UpdateModel availableUpdate, UpdateCollection updateCollection)
        {
            if (availableUpdate is null && updateCollection is null && updateCollection.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    AutoResetEvent downloadEvent = new(false);

                    IDownloadJob downloadJob = null;
                    UpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();
                    updateDownloader.Updates = updateCollection;

                    DownloadProgressChangedCallback downloadProgressChangedCallback = new();
                    DownloadCompletedCallback downloadCompletedCallback = new();
                    downloadProgressChangedCallback.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(sender, args, availableUpdate);
                    downloadCompletedCallback.DownloadCompleted += (sender, args) => downloadEvent.Set();
                    downloadJob = updateDownloader.BeginDownload(downloadProgressChangedCallback, downloadCompletedCallback, null);

                    if (!downloadJobDict.ContainsKey(availableUpdate.UpdateID))
                    {
                        downloadJobDict.Add(availableUpdate.UpdateID, downloadJob);
                    }

                    downloadEvent.WaitOne();
                    downloadEvent.Dispose();
                    return updateDownloader.EndDownload(downloadJob);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(DownloadUpdateAsync), 1, e);
                    return null;
                }
            });
        }

        /// <summary>
        /// 安装更新
        /// </summary>
        private async Task<IInstallationResult> InstallUpdateAsync(UpdateModel availableUpdate, UpdateCollection updateCollection)
        {
            if (availableUpdate is null && updateCollection is null && updateCollection.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    AutoResetEvent installEvent = new(false);

                    IInstallationJob installationJob = null;
                    UpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;
                    updateInstaller.Updates = updateCollection;
                    updateInstaller.ForceQuiet = true;
                    InstallationCompletedCallback installationCompletedCallback = new();
                    InstallationProgressChangedCallback installationProgressChangedCallback = new();
                    installationCompletedCallback.InstallationCompleted += (sender, args) => installEvent.Set();
                    installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnInstallationProgressChanged(sender, args, availableUpdate);
                    installationJob = updateInstaller.BeginInstall(installationProgressChangedCallback, installationCompletedCallback, null);

                    if (!installationJobDict.ContainsKey(availableUpdate.UpdateID))
                    {
                        installationJobDict.Add(availableUpdate.UpdateID, installationJob);
                    }

                    installEvent.WaitOne();
                    installEvent.Dispose();
                    return updateInstaller.EndInstall(installationJob);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(InstallUpdateAsync), 1, e);
                    return null;
                }
            });
        }

        /// <summary>
        /// 显示更新
        /// </summary>
        private async Task<bool> ShowUpdateAsync(UpdateInformation updateInformation)
        {
            if (updateInformation is null)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                bool result = false;

                // 显示更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                if (RuntimeHelper.IsElevated)
                {
                    try
                    {
                        updateInformation.IsHidden = false;
                        updateInformation.Update.IsHidden = false;
                        result = true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(ShowUpdateAsync), 1, e);
                    }
                }

                return result;
            });
        }

        /// <summary>
        /// 取消卸载更新
        /// </summary>
        private void CancelUninstallUpdate(string updateId)
        {
            if (string.IsNullOrEmpty(updateId))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (uninstallationJobDict.TryGetValue(updateId, out IInstallationJob installationJob))
                    {
                        installationJob.RequestAbort();
                        uninstallationJobDict.Remove(updateId);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(CancelUninstallUpdate), 1, e);
                }
            });
        }

        /// <summary>
        /// 卸载更新
        /// </summary>
        private async Task<IInstallationResult> UninstallUpdateAsync(UpdateModel installedUpdate, UpdateCollection updateCollection)
        {
            if (installedUpdate is null && updateCollection is null && updateCollection.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    AutoResetEvent uninstallEvent = new(false);

                    IInstallationJob installationJob = null;
                    UpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;
                    updateInstaller.Updates = updateCollection;
                    updateInstaller.ForceQuiet = true;
                    InstallationCompletedCallback installationCompletedCallback = new();
                    InstallationProgressChangedCallback installationProgressChangedCallback = new();
                    installationCompletedCallback.InstallationCompleted += (sender, args) => uninstallEvent.Set();
                    installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnUninstallationProgressChanged(sender, args, installedUpdate);
                    installationJob = updateInstaller.BeginUninstall(installationProgressChangedCallback, installationCompletedCallback, null);

                    if (!uninstallationJobDict.ContainsKey(installedUpdate.UpdateID))
                    {
                        uninstallationJobDict.Add(installedUpdate.UpdateID, installationJob);
                    }

                    uninstallEvent.WaitOne();
                    uninstallEvent.Dispose();
                    return updateInstaller.EndUninstall(installationJob);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(UninstallUpdateAsync), 1, e);
                    return null;
                }
            });
        }

        /// <summary>
        /// 获取更新描述信息内容
        /// </summary>
        private async Task<string> GetUpdateHistoryInformationStringAsync(UpdateInformation updateInformation, WindowsDriverInformation windowsDriverInformation)
        {
            if (updateInformation is null && windowsDriverInformation is null)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                StringBuilder copyInformationBuilder = new();
                copyInformationBuilder.Append(UpdateNameString);
                copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateInformation.Title) ? NotAvailableString : updateInformation.Title);
                copyInformationBuilder.Append(DescriptionString);
                copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateInformation.Description) ? NotAvailableString : updateInformation.Description);
                copyInformationBuilder.Append(IsBetaString);
                copyInformationBuilder.AppendLine(updateInformation.IsBeta ? YesString : NoString);
                copyInformationBuilder.Append(IsMandatoryString);
                copyInformationBuilder.AppendLine(updateInformation.IsMandatory ? YesString : NoString);
                copyInformationBuilder.Append(MaxDownloadSizeString);
                copyInformationBuilder.AppendLine(VolumeSizeHelper.ConvertVolumeSizeToString(Convert.ToDouble(updateInformation.Update.MaxDownloadSize)));
                copyInformationBuilder.Append(MinDownloadSizeString);
                copyInformationBuilder.AppendLine(VolumeSizeHelper.ConvertVolumeSizeToString(Convert.ToDouble(updateInformation.Update.MinDownloadSize)));
                copyInformationBuilder.Append(MsrcSeverityString);
                copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateInformation.MsrcSeverity) ? NotAvailableString : updateInformation.MsrcSeverity);
                copyInformationBuilder.Append(RecommendedCpuSpeedString);
                copyInformationBuilder.AppendLine(updateInformation.RecommendedCpuSpeed is 0 ? NotAvailableString : string.Format("{0} MHz", updateInformation.RecommendedCpuSpeed));
                copyInformationBuilder.Append(RecommendedHardDiskSpaceString);
                copyInformationBuilder.AppendLine(updateInformation.RecommendedHardDiskSpace is 0 ? NotAvailableString : string.Format("{0} MB", updateInformation.RecommendedHardDiskSpace));
                copyInformationBuilder.Append(RecommendedMemoryString);
                copyInformationBuilder.AppendLine(updateInformation.RecommendedMemory is 0 ? NotAvailableString : string.Format("{0} MB", updateInformation.RecommendedMemory));
                copyInformationBuilder.Append(ReleaseNotesString);
                copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateInformation.ReleaseNotes) ? NotAvailableString : updateInformation.ReleaseNotes);
                copyInformationBuilder.Append(SupportedUrlString);
                copyInformationBuilder.AppendLine(updateInformation.SupportURL);

                if (updateInformation.UpdateType is UpdateType.utDriver)
                {
                    copyInformationBuilder.Append(DeviceProblemNumberString);
                    copyInformationBuilder.AppendLine(Convert.ToString(windowsDriverInformation.DeviceProblemNumber));
                    copyInformationBuilder.Append(DriverClassString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(windowsDriverInformation.DriverClass) ? NotAvailableString : windowsDriverInformation.DriverClass);
                    copyInformationBuilder.Append(DriverHardwareIDString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(windowsDriverInformation.DriverHardwareID) ? NotAvailableString : windowsDriverInformation.DriverHardwareID);
                    copyInformationBuilder.Append(DriverManufacturerString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(windowsDriverInformation.DriverManufacturer) ? NotAvailableString : windowsDriverInformation.DriverManufacturer);
                    copyInformationBuilder.Append(DriverModelString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(windowsDriverInformation.DriverModel) ? NotAvailableString : windowsDriverInformation.DriverModel);
                    copyInformationBuilder.Append(DriverProviderString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(windowsDriverInformation.DriverProvider) ? NotAvailableString : windowsDriverInformation.DriverProvider);
                    copyInformationBuilder.Append(DriverVerDateString);
                    copyInformationBuilder.AppendLine(windowsDriverInformation.DriverVerDate.ToString("yyyy/MM/dd"));
                }

                return Convert.ToString(copyInformationBuilder);
            });
        }

        /// <summary>
        /// 使用命令卸载更新
        /// </summary>
        private void UninstallWithCmd(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    Match matchResult = updateNumberRegex.Match(title);

                    if (matchResult is not null && matchResult.Success && matchResult.Value.Length > 2)
                    {
                        string kbNumber = matchResult.Value.Substring(2);
                        string executeFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "wusa.exe");
                        ProcessStartInfo processStartInfo = new()
                        {
                            FileName = "wusa.exe",
                            Arguments = string.Format("/uninstall /kb:{0}", kbNumber),
                            UseShellExecute = true
                        };

                        Process.Start(processStartInfo);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(UninstallWithCmd), 1, e);
                }
            });
        }

        /// <summary>
        /// 清除历史更新记录
        /// </summary>
        private async Task<bool> CleanUpdateHistoryAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    StopService("wuauserv");
                    StopService("usosvc");
                    string logFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"SoftwareDistribution\DataStore\Logs\edb.log");
                    string databaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"USOPrivate\UpdateStore");

                    if (File.Exists(logFileName))
                    {
                        FileSystem.DeleteFile(logFileName, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }

                    if (Directory.Exists(databaseDirectory))
                    {
                        foreach (string databaseFile in Directory.GetFiles(databaseDirectory))
                        {
                            FileSystem.DeleteFile(databaseFile, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        }
                    }

                    StartService("wuauserv");
                    StartService("usosvc");
                    Process.Start("UsoClient.exe", "RefreshSettings");
                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(CleanUpdateHistoryAsync), 1, e);
                    return false;
                }
            });
        }

        /// <summary>
        /// 更新可用更新选中项状态
        /// </summary>
        private void UpdateAvailableSelectedItemsState()
        {
            if (UpdateManagerAvailableListView.SelectedItems.Count > 0)
            {
                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsUpdating && !item.IsUpdateCanceled);
            }
            else
            {
                IsAvailableHideEnabled = false;
                IsAvailableInstallEnabled = false;
                IsAvailableCancelInstallEnabled = false;
            }
        }

        /// <summary>
        /// 更新已安装更新选中项状态
        /// </summary>
        private void UpdateInstalledSelectedItemState()
        {
            if (UpdateManagerInstalledListView.SelectedItems.Count > 0)
            {
                IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
                IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);
            }
            else
            {
                IsInstalledUninstallEnabled = false;
                IsInstalledCancelInstallEnabled = false;
            }
        }

        /// <summary>
        /// 更新下载更新结果
        /// </summary>
        private void UpdateDownloadResult(UpdateModel availableUpdate, IDownloadResult downloadResult)
        {
            if (availableUpdate is not null)
            {
                if (downloadResult is not null)
                {
                    // 更新下载成功
                    if (downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                    {
                        availableUpdate.UpdatePercentage = 50;
                        availableUpdate.UpdateProgress = DownloadUpdateCompletedString;
                    }
                    // 更新下载已取消
                    else if (downloadResult.ResultCode is OperationResultCode.orcAborted)
                    {
                        availableUpdate.IsUpdateCanceled = false;
                        availableUpdate.IsUpdating = false;
                        availableUpdate.UpdateProgress = DownloadUpdateCanceledString;
                    }
                    // 更新下载失败
                    else
                    {
                        availableUpdate.IsUpdateCanceled = false;
                        availableUpdate.IsUpdating = false;
                        Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);
                        availableUpdate.UpdateProgress = exception is not null ? string.Format(DownloadUpdateFailedWithInformationString, exception.Message) : string.Format(DownloadUpdateFailedWithCodeString, downloadResult.HResult);
                    }
                }
                // 更新下载失败
                else
                {
                    availableUpdate.IsUpdateCanceled = false;
                    availableUpdate.IsUpdating = false;
                    Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);
                    availableUpdate.UpdateProgress = exception is not null ? string.Format(DownloadUpdateFailedWithInformationString, exception.Message) : string.Format(DownloadUpdateFailedWithCodeString, downloadResult.HResult);
                }
            }
        }

        /// <summary>
        /// 更新安装更新结果
        /// </summary>
        private void UpdateInstallResult(UpdateModel availableUpdate, IInstallationResult installationResult)
        {
            if (availableUpdate is not null)
            {
                if (installationResult is not null)
                {
                    // 更新安装成功
                    if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                    {
                        availableUpdate.UpdatePercentage = 0;
                        availableUpdate.IsUpdateCanceled = true;
                        availableUpdate.UpdateProgress = installationResult.RebootRequired ? InstallUpdateCompletedNeedRebootString : InstallUpdateCompletedString;

                        if (installationResult.RebootRequired && !IsUpdateCompletedNeedRebootPrompt)
                        {
                            IsUpdateCompletedNeedRebootPrompt = true;
                        }
                    }
                    // 更新安装已取消
                    else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                    {
                        availableUpdate.IsUpdateCanceled = false;
                        availableUpdate.IsUpdating = false;
                        availableUpdate.UpdateProgress = InstallUpdateCanceledString;
                    }
                    // 更新安装失败
                    else
                    {
                        availableUpdate.IsUpdateCanceled = false;
                        availableUpdate.IsUpdating = false;
                        Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                        availableUpdate.UpdateProgress = exception is not null ? string.Format(InstallUpdateFailedWithInformationString, exception.Message) : string.Format(InstallUpdateFailedWithCodeString, installationResult.HResult);
                    }
                }
                // 更新安装失败
                else
                {
                    availableUpdate.IsUpdateCanceled = false;
                    availableUpdate.IsUpdating = false;
                    Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                    availableUpdate.UpdateProgress = exception is not null ? string.Format(InstallUpdateFailedWithInformationString, exception.Message) : string.Format(InstallUpdateFailedWithCodeString, installationResult.HResult);
                }
            }
        }

        /// <summary>
        /// 更新卸载更新结果
        /// </summary>
        private void UpdateUninstallResult(UpdateModel installedUpdate, IInstallationResult installationResult)
        {
            if (installedUpdate is not null)
            {
                if (installationResult is not null)
                {
                    // 更新卸载成功
                    if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                    {
                        installedUpdate.UpdatePercentage = 0;
                        installedUpdate.IsUpdateCanceled = true;
                        installedUpdate.UpdateProgress = installationResult.RebootRequired ? UninstallUpdateCompletedNeedRebootString : UninstallUpdateCompletedString;

                        if (installationResult.RebootRequired && !IsUpdateCompletedNeedRebootPrompt)
                        {
                            IsUpdateCompletedNeedRebootPrompt = true;
                        }
                    }
                    // 更新卸载已取消
                    else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                    {
                        installedUpdate.IsUpdateCanceled = false;
                        installedUpdate.IsUpdating = false;
                        installedUpdate.UpdateProgress = UninstallUpdateCanceledString;
                    }
                    // 更新卸载失败
                    else
                    {
                        installedUpdate.IsUpdateCanceled = false;
                        installedUpdate.IsUpdating = false;
                        Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                        installedUpdate.UpdateProgress = exception is not null ? string.Format(UninstallUpdateFailedWithInformationString, exception.Message) : string.Format(UninstallUpdateFailedWithCodeString, installationResult.HResult);
                    }
                }
                // 更新卸载失败
                else
                {
                    installedUpdate.IsUpdateCanceled = false;
                    installedUpdate.IsUpdating = false;
                    Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                    installedUpdate.UpdateProgress = exception is not null ? string.Format(UninstallUpdateFailedWithInformationString, exception.Message) : string.Format(UninstallUpdateFailedWithCodeString, installationResult.HResult);
                }
            }
        }

        /// <summary>
        /// 更新隐藏更新结果
        /// </summary>
        private void UpdateHideResult(UpdateModel availableUpdate)
        {
            if (availableUpdate is null)
            {
                return;
            }

            try
            {
                availableUpdate.UpdateProgress = string.Empty;
                AvailableUpdateCollection.Remove(availableUpdate);
                HiddenUpdateCollection.Add(availableUpdate);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(UpdateHideResult), 1, e);
            }
        }

        /// <summary>
        /// 更新显示更新结果
        /// </summary>
        private void UpdateShowResult(UpdateModel hiddenUpdate)
        {
            if (hiddenUpdate is null)
            {
                return;
            }

            try
            {
                hiddenUpdate.UpdateProgress = UpdateNotInstalledString;
                HiddenUpdateCollection.Remove(hiddenUpdate);
                AvailableUpdateCollection.Add(hiddenUpdate);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(UpdateShowResult), 1, e);
            }
        }

        /// <summary>
        /// 移除自定义预览体验计划设置
        /// </summary>
        private bool ExitCustomPreviewChannel()
        {
            Task.Run(() =>
            {
                try
                {
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Account");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Cache");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Restricted");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ToastNotification");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\WUMUDCat");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingExternal");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingPreview");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingInsiderSlow");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingInsiderFast");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "BranchReadinessLevel");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\WindowsUpdate", "AllowWindowsUpdate");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\MoSetup", "AllowUpgradesWithUnsupportedTPMOrCPU");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassRAMCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassSecureBootCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassStorageCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassTPMCheck");
                    RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\PCHC", "UpgradeEligibility");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(ExitCustomPreviewChannel), 1, e);
                }
            });

            return default;
        }

        /// <summary>
        /// 添加自定义预览体验计划设置
        /// </summary>
        private bool EnterCustomPreviewChannel(string previewChannel)
        {
            if (string.IsNullOrEmpty(previewChannel))
            {
                return default;
            }

            Task.Run(() =>
            {
                // 注册表存储的内容
                // {"Message":"Windows 预览体验计划配置情况","LinkTitle":"","LinkUrl":"","DynamicXaml":"<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Spacing=\"3\"><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"设备已经使用 PowerToolbox 强行进入 Windows 预览体验计划\" /><TextBlock FontSize=\"15\" Text=\"如果您想更改 Windows 预览体验计划的设置，请在 PowerToolbox 中进行设置\" /><Button Margin=\"0,0,0,10\" Command=\"{StaticResource ActivateUriCommand}\" CommandParameter=\"powertoolbox://\" Content=\"打开 PowerToolbox\" /><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"当前配置\" /><TextBlock FontSize=\"15\"><Run Text=\"当前通道：\" /><Run Text=\"开发版（Dev）\" /></TextBlock><Button Margin=\"0,0,0,10\" Command=\"{StaticResource ActivateUriCommand}\" CommandParameter=\"https://www.microsoft.com/windowsinsider\" Content=\"了解 Windows 预览体验计划\" /></StackPanel>","Severity":0}
                string stickyMessage = """{{"Message":"{0}","LinkTitle":"","LinkUrl":"","DynamicXaml":"<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Spacing=\"3\"><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"{1}\" /><TextBlock FontSize=\"15\" Text=\"{2}\" /><Button Margin=\"0,0,0,10\" Command=\"{{StaticResource ActivateUriCommand}}\" CommandParameter=\"powertoolbox://\" Content=\"{3}\" /><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"{4}\" /><TextBlock FontSize=\"15\"><Run Text=\"{5}\" /><Run Text=\"{6}\" /></TextBlock><Button Margin=\"0,0,0,10\" Command=\"{{StaticResource ActivateUriCommand}}\" CommandParameter=\"https://www.microsoft.com/windowsinsider\" Content=\"{7}\" /></StackPanel>","Severity":0}}""";
                // "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Spacing=\"3\"><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"设备已经使用 PowerToolbox强行进入 Windows 预览体验计划\" /><TextBlock FontSize=\"15\" Text=\"如果您想更改 Windows 预览体验计划的设置，请在 PowerToolbox 中进行设置\" /><Button Margin=\"0,0,0,10\" Command=\"{StaticResource ActivateUriCommand}\" CommandParameter=\"powertoolbox://\" Content=\"打开 PowerToolbox\" /><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"当前配置\" /><TextBlock FontSize=\"15\"><Run Text=\"当前通道：\" /><Run Text=\"开发版（Dev）\" /></TextBlock><Button Margin=\"0,0,0,10\" Command=\"{StaticResource ActivateUriCommand}\" CommandParameter=\"https://www.microsoft.com/windowsinsider\" Content=\"了解 Windows 预览体验计划\" /></StackPanel>"
                string stickyXaml = """<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Spacing=\"3\"><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"{0}\" /><TextBlock FontSize=\"15\" Text=\"{1}\" /><Button Margin=\"0,0,0,10\" Command=\"{{StaticResource ActivateUriCommand}}\" CommandParameter=\"powertoolbox://\" Content=\"{2}\" /><TextBlock FontSize=\"18\" FontWeight=\"SemiBold\" Text=\"{3}\" /><TextBlock FontSize=\"15\"><Run Text=\"{4}\" /><Run Text=\"{5}\" /></TextBlock><Button Margin=\"0,0,0,10\" Command=\"{{StaticResource ActivateUriCommand}}\" CommandParameter=\"https://www.microsoft.com/windowsinsider\" Content=\"{6}\" /></StackPanel>""";
                string channelName = string.Empty;
                string channelLocalizedName = string.Empty;
                int branchReadinessLevel = 0;

                if (string.Equals(previewChannel, "ReleasePreview", StringComparison.OrdinalIgnoreCase))
                {
                    channelName = "ReleasePreview";
                    channelLocalizedName = InsiderReleasePreviewString;
                    branchReadinessLevel = 8;
                }
                else if (string.Equals(previewChannel, "Beta", StringComparison.OrdinalIgnoreCase))
                {
                    channelName = "Beta";
                    channelLocalizedName = InsiderBetaString;
                    branchReadinessLevel = 4;
                }
                else if (string.Equals(previewChannel, "Dev", StringComparison.OrdinalIgnoreCase))
                {
                    channelName = "Dev";
                    channelLocalizedName = InsiderDevString;
                    branchReadinessLevel = 2;
                }
                else if (string.Equals(previewChannel, "Canary", StringComparison.OrdinalIgnoreCase))
                {
                    channelName = "CanaryChannel";
                    channelLocalizedName = InsiderCanaryString;
                }

                if (RuntimeHelper.IsElevated)
                {
                    try
                    {
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Orchestrator", "EnableUUPScan", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\RingExternal", "Enabled", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\SLS\Programs\WUMUDCat", "WUMUDCATEnabled", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "EnablePreviewBuilds", 2);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "IsBuildFlightingEnabled", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "IsConfigSettingsFlightingEnabled", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "IsConfigExpFlightingEnabled", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "TestFlags", 32);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RingId", 11);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "Ring", "External");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "ContentType", "Mainline");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "BranchName", channelName);

                        try
                        {
                            string xaml = string.Format(stickyXaml, DeviceEnteredWindowsInsiderString, ModifyWindowsInsiderPreviewString, OpenPowerToolboxString, CurrentConfigString, CurrentChannelString, channelLocalizedName, LearnWindowsInsiderPreviewString);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Strings", "StickyXaml", xaml);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(ExitCustomPreviewChannel), 2, e);
                        }

                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIHiddenElements", 65535);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIDisabledElements", 65535);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIServiceDrivenElementVisibility", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIErrorMessageVisibility", 192);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 3);

                        if (branchReadinessLevel is not 0)
                        {
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "BranchReadinessLevel", branchReadinessLevel);
                        }

                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIHiddenElements_Rejuv", 65534);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Visibility", "UIDisabledElements_Rejuv", 65535);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIRing", "External");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIContentType", "Mainline");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIBranch", channelName);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIOptin", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RingBackup", "External");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "RingBackupV2", "External");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "BranchBackup", channelName);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Cache", "PropertyIgnoreList", "AccountsBlob;;CTACBlob;FlightIDBlob;ServiceDrivenActionResults");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Cache", "RequestedCTACAppIds", "WU;FSS");
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Account", "SupportedTypes", 3);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Account", "Status", 8);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "UseSettingsExperience", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "AllowFSSCommunications", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "UICapabilities", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "IgnoreConsolidation", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "MsaUserTicketHr", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "MsaDeviceTicketHr", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "ValidateOnlineHr", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "LastHR", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "ErrorState", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "PilotInfoRing", 3);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "RegistryAllowlistVersion", 4);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\ClientState", "FileAllowlistVersion", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI", "UIControllableState", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIDialogConsent", 0);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "UIUsage", 26);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "OptOutState", 25);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", "AdvancedToggleState", 24);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\WindowsUpdate", "AllowWindowsUpdate", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\MoSetup", "AllowUpgradesWithUnsupportedTPMOrCPU", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassRAMCheck", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassSecureBootCheck", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassStorageCheck", 1);
                        RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SYSTEM\Setup\LabConfig", "BypassTPMCheck", 1);
                        RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\PCHC", "UpgradeEligibility", 1);

                        try
                        {
                            string message = string.Format(stickyMessage, WindowsInsiderConfigStatusString, DeviceEnteredWindowsInsiderString, ModifyWindowsInsiderPreviewString, OpenPowerToolboxString, CurrentConfigString, CurrentChannelString, channelLocalizedName, LearnWindowsInsiderPreviewString);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\WindowsSelfHost\UI\Strings", "StickyMessage", message);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(ExitCustomPreviewChannel), 3, e);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(ExitCustomPreviewChannel), 4, e);
                    }
                }
            });

            return default;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        private void StartService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                return;
            }

            try
            {
                ServiceController serviceController = new(serviceName);
                if (serviceController.Status is not ServiceControllerStatus.Running && serviceController.Status is not ServiceControllerStatus.StartPending)
                {
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }
                serviceController.Dispose();
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(StartService), 1, e);
            }
        }

        /// <summary>
        /// 暂停服务
        /// </summary>
        private void StopService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                return;
            }

            try
            {
                ServiceController serviceController = new(serviceName);
                if (serviceController.Status is not ServiceControllerStatus.Stopped && serviceController.Status is not ServiceControllerStatus.StopPending)
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
                serviceController.Dispose();
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(StopService), 1, e);
            }
        }

        /// <summary>
        /// 了解详细信息
        /// </summary>
        private void LearnMore(string supportUrl)
        {
            if (string.IsNullOrEmpty(supportUrl))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    Process.Start(supportUrl);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(LearnMore), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开更新对应的受支持的链接
        /// </summary>
        private void OpenSupportUrl(string supportUrl)
        {
            if (string.IsNullOrEmpty(supportUrl))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    Process.Start(supportUrl);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OpenSupportUrl), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开 Windows 更新
        /// </summary>
        private void OpenWindowsUpdate()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsupdate");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OpenWindowsUpdate), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开 Windows 更新历史记录
        /// </summary>
        private void OpenWindowsUpdateHistory()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsupdate-history");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OpenWindowsUpdateHistory), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OpenWIPSettings()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsinsider");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OpenWIPSettings), 1, e);
                }
            });
        }

        /// <summary>
        /// 重启设备
        /// </summary>
        private void RebootPC()
        {
            Task.Run(() =>
            {
                ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
            });
        }

        private bool GetUpdateState(bool param1, int param2)
        {
            return param1 && Convert.ToBoolean(param2);
        }

        private Visibility GetSelectedIndexVisibility(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
