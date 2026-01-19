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
    public sealed partial class UpdateManagerPage : Page, INotifyPropertyChanged
    {
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
        private readonly string InsiderBetaString = ResourceService.UpdateManagerResource.GetString("InsiderBeta");
        private readonly string InsiderCanaryString = ResourceService.UpdateManagerResource.GetString("InsiderCanary");
        private readonly string InsiderDevString = ResourceService.UpdateManagerResource.GetString("InsiderDev");
        private readonly string InsiderReleasePreviewString = ResourceService.UpdateManagerResource.GetString("InsiderReleasePreview");
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
        private readonly string UpdateInstalledString = ResourceService.UpdateManagerResource.GetString("UpdateInstalled");
        private readonly string UpdateNameString = ResourceService.UpdateManagerResource.GetString("UpdateName");
        private readonly string UpdateNotInstalledString = ResourceService.UpdateManagerResource.GetString("UpdateNotInstalled");
        private readonly string UpdatePrepareInstallingString = ResourceService.UpdateManagerResource.GetString("UpdatePrepareInstalling");
        private readonly string UpdatePrepareUninstallingString = ResourceService.UpdateManagerResource.GetString("UpdatePrepareUninstalling");
        private readonly string UpdateSucceedString = ResourceService.UpdateManagerResource.GetString("UpdateSucceed");
        private readonly string UpdateSucceedWithErrorsString = ResourceService.UpdateManagerResource.GetString("UpdateSucceedWithErrors");
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

        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private string _windowsUpdateVersion;

        public string WindowsUpdateVersion
        {
            get { return _windowsUpdateVersion; }

            set
            {
                if (!Equals(_windowsUpdateVersion, value))
                {
                    _windowsUpdateVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsUpdateVersion)));
                }
            }
        }

        private string _wuapiDllVersion;

        public string WuapiDllVersion
        {
            get { return _wuapiDllVersion; }

            set
            {
                if (!Equals(_wuapiDllVersion, value))
                {
                    _wuapiDllVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WuapiDllVersion)));
                }
            }
        }

        private bool _isCheckUpdateEnabled = true;

        public bool IsCheckUpdateEnabled
        {
            get { return _isCheckUpdateEnabled; }

            set
            {
                if (!Equals(_isCheckUpdateEnabled, value))
                {
                    _isCheckUpdateEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCheckUpdateEnabled)));
                }
            }
        }

        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecking)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableHideEnabled)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableInstallEnabled)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableCancelInstallEnabled)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledUninstallEnabled)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledCancelInstallEnabled)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHiddenShowEnabled)));
                }
            }
        }

        private bool _isExcludeDrivers;

        public bool IsExcludeDrivers
        {
            get { return _isExcludeDrivers; }

            set
            {
                if (!Equals(_isExcludeDrivers, value))
                {
                    _isExcludeDrivers = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExcludeDrivers)));
                }
            }
        }

        private bool _isCleaning;

        public bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                if (!Equals(_isCleaning, value))
                {
                    _isCleaning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCleaning)));
                }
            }
        }

        private bool _isUpdateCompletedNeedRebootPrompt;

        public bool IsUpdateCompletedNeedRebootPrompt
        {
            get { return _isUpdateCompletedNeedRebootPrompt; }

            set
            {
                if (!Equals(_isUpdateCompletedNeedRebootPrompt, value))
                {
                    _isUpdateCompletedNeedRebootPrompt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdateCompletedNeedRebootPrompt)));
                }
            }
        }

        private bool _isPreviewChannelChangedNeedRebootPrompt;

        public bool IsPreviewChannelChangedNeedRebootPrompt
        {
            get { return _isPreviewChannelChangedNeedRebootPrompt; }

            set
            {
                if (!Equals(_isPreviewChannelChangedNeedRebootPrompt, value))
                {
                    _isPreviewChannelChangedNeedRebootPrompt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPreviewChannelChangedNeedRebootPrompt)));
                }
            }
        }

        private bool _isIncludePotentiallySupersededUpdate;

        public bool IsIncludePotentiallySupersededUpdate
        {
            get { return _isIncludePotentiallySupersededUpdate; }

            set
            {
                if (!Equals(_isIncludePotentiallySupersededUpdate, value))
                {
                    _isIncludePotentiallySupersededUpdate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncludePotentiallySupersededUpdate)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedUpdateSource;

        public KeyValuePair<string, string> SelectedUpdateSource
        {
            get { return _selectedUpdateSource; }

            set
            {
                if (!Equals(_selectedUpdateSource, value))
                {
                    _selectedUpdateSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedUpdateSource)));
                }
            }
        }

        private List<KeyValuePair<string, string>> UpdateSourceList { get; } = [];

        private WinRTObservableCollection<UpdateModel> AvailableUpdateCollection { get; } = [];

        private WinRTObservableCollection<UpdateModel> InstalledUpdateCollection { get; } = [];

        private WinRTObservableCollection<UpdateModel> HiddenUpdateCollection { get; } = [];

        private WinRTObservableCollection<UpdateModel> UpdateHistoryCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateManagerPage()
        {
            InitializeComponent();
            SelectedItem = UpdateManagerSelctorBar.Items[0];
            UpdateSourceList.Add(new KeyValuePair<string, string>("Microsoft Update", MicrosoftUpdateString));
            UpdateSourceList.Add(new KeyValuePair<string, string>("DCat Flighting Prod", DCatFlightingProdString));
            UpdateSourceList.Add(new KeyValuePair<string, string>("Windows Store(DCat Prod)", WindowsStoreString));
            UpdateSourceList.Add(new KeyValuePair<string, string>("Windows Update", WindowsUpdateString));
            SelectedUpdateSource = UpdateSourceList[0];
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

                if (RuntimeHelper.IsElevated)
                {
                    IsExcludeDrivers = await Task.Run(() =>
                    {
                        try
                        {
                            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true);
                            int returnValue = Convert.ToInt32(registryKey.GetValue("ExcludeWUDriversInQualityUpdate"));
                            return Convert.ToBoolean(returnValue);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnNavigatedTo), 1, e);
                            return false;
                        }
                    });
                }
                else
                {
                    return;
                }

                // 获取更新版本信息
                string windowsUpdateVersion = string.Empty;
                string wuapiDllVersion = string.Empty;

                await Task.Run(() =>
                {
                    try
                    {
                        updateSession.ClientApplicationID = "PowerToolbox:" + Guid.NewGuid().ToString();
                        updateSearcher = updateSession.CreateUpdateSearcher() as UpdateSearcher;
                        updateSearcher.IgnoreDownloadPriority = true;
                        WindowsUpdateAgentInfo windowsUpdateAgentInfo = new();
                        object apiMajorVersion = windowsUpdateAgentInfo.GetInfo("ApiMajorVersion");
                        object apiMinorVersion = windowsUpdateAgentInfo.GetInfo("ApiMinorVersion");
                        object productVersionString = windowsUpdateAgentInfo.GetInfo("ProductVersionString");
                        windowsUpdateVersion = string.Format("{0}.{1}", apiMajorVersion, apiMinorVersion);
                        wuapiDllVersion = Convert.ToString(productVersionString);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnNavigatedTo), 2, e);
                    }
                });

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

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

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
                    IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                    IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                    IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);

                    Task.Run(() =>
                    {
                        try
                        {
                            if (downloadJobDict.TryGetValue(availableUpdate.UpdateID, out IDownloadJob downloadJob))
                            {
                                downloadJob.RequestAbort();
                                downloadJobDict.Remove(availableUpdate.UpdateID);
                            }

                            if (installationJobDict.TryGetValue(availableUpdate.UpdateID, out IInstallationJob installationJob))
                            {
                                installationJob.RequestAbort();
                                installationJobDict.Remove(availableUpdate.UpdateID);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableCancelInstallExecuteRequested), 1, e);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 可用更新：修改可用更新项选中状态
        /// </summary>
        private void OnAvailableCheckClickExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter as UpdateModel is UpdateModel availableUpdate)
            {
                availableUpdate.IsSelected = !availableUpdate.IsSelected;
                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
            }
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel availableUpdate)
            {
                availableUpdate.IsSelected = false;

                bool hideResult = await Task.Run(() =>
                {
                    bool result = false;

                    // 隐藏更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated && !availableUpdate.IsUpdating)
                    {
                        try
                        {
                            availableUpdate.UpdateInformation.IsHidden = true;
                            availableUpdate.UpdateInformation.Update.IsHidden = true;
                            result = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableHideExecuteRequested), 1, e);
                        }
                    }

                    return result;
                });

                // 隐藏成功时更新界面显示内容
                if (hideResult)
                {
                    try
                    {
                        availableUpdate.UpdateProgress = string.Empty;
                        AvailableUpdateCollection.Remove(availableUpdate);
                        HiddenUpdateCollection.Add(availableUpdate);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableHideExecuteRequested), 2, e);
                    }

                    IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                    IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                    IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                    IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
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
                bool isUpdating = false;

                if (!availableUpdate.IsUpdating)
                {
                    isUpdating = true;
                    availableUpdate.IsUpdating = true;
                    availableUpdate.IsUpdateCanceled = false;
                    availableUpdate.UpdatePercentage = 0;
                    availableUpdate.IsUpdatePreparing = true;
                    availableUpdate.UpdateProgress = UpdatePrepareInstallingString;
                }

                if (isUpdating)
                {
                    try
                    {
                        IsCheckUpdateEnabled = false;
                        bool updateResult = false;
                        UpdateCollection updateCollection = new() { availableUpdate.UpdateInformation.Update };

                        // 先下载更新
                        IDownloadResult downloadResult = await Task.Run(() =>
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
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableInstallExecuteRequested), 1, e);
                                return null;
                            }
                        });

                        if (downloadResult is not null)
                        {
                            // 更新下载成功
                            if (downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                            {
                                availableUpdate.UpdatePercentage = 50;
                                availableUpdate.UpdateProgress = DownloadUpdateCompletedString;
                                updateResult = true;
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

                        // 移除更新下载任务
                        downloadJobDict.Remove(availableUpdate.UpdateID);

                        // 保证更新下载成功后再进行安装更新
                        if (updateResult)
                        {
                            // 后面再安装更新
                            IInstallationResult installationResult = await Task.Run(() =>
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
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableInstallExecuteRequested), 2, e);
                                    return null;
                                }
                            });

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

                            // 移除更新安装任务
                            installationJobDict.Remove(availableUpdate.UpdateID);
                        }

                        // 当前更新的下载和安装所有步骤都已完成
                        IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                        IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                        IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);

                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableInstallExecuteRequested), 3, e);
                    }
                }
            }
        }

        /// <summary>
        /// 更新历史记录，复制更新描述信息
        /// </summary>
        private async void OnCopyInformationExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel updateItem)
            {
                string copyString = await Task.Run(() =>
                {
                    StringBuilder copyInformationBuilder = new();
                    copyInformationBuilder.Append(UpdateNameString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.Title) ? NotAvailableString : updateItem.UpdateInformation.Title);
                    copyInformationBuilder.Append(DescriptionString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.Description) ? NotAvailableString : updateItem.UpdateInformation.Description);
                    copyInformationBuilder.Append(IsBetaString);
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.IsBeta ? YesString : NoString);
                    copyInformationBuilder.Append(IsMandatoryString);
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.IsMandatory ? YesString : NoString);
                    copyInformationBuilder.Append(MaxDownloadSizeString);
                    copyInformationBuilder.AppendLine(VolumeSizeHelper.ConvertVolumeSizeToString(Convert.ToDouble(updateItem.UpdateInformation.Update.MaxDownloadSize)));
                    copyInformationBuilder.Append(MinDownloadSizeString);
                    copyInformationBuilder.AppendLine(VolumeSizeHelper.ConvertVolumeSizeToString(Convert.ToDouble(updateItem.UpdateInformation.Update.MinDownloadSize)));
                    copyInformationBuilder.Append(MsrcSeverityString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.MsrcSeverity) ? NotAvailableString : updateItem.UpdateInformation.MsrcSeverity);
                    copyInformationBuilder.Append(RecommendedCpuSpeedString);
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.RecommendedCpuSpeed is 0 ? NotAvailableString : string.Format("{0} MHz", updateItem.UpdateInformation.RecommendedCpuSpeed));
                    copyInformationBuilder.Append(RecommendedHardDiskSpaceString);
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.RecommendedHardDiskSpace is 0 ? NotAvailableString : string.Format("{0} MB", updateItem.UpdateInformation.RecommendedHardDiskSpace));
                    copyInformationBuilder.Append(RecommendedMemoryString);
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.RecommendedMemory is 0 ? NotAvailableString : string.Format("{0} MB", updateItem.UpdateInformation.RecommendedMemory));
                    copyInformationBuilder.Append(ReleaseNotesString);
                    copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.UpdateInformation.ReleaseNotes) ? NotAvailableString : updateItem.UpdateInformation.ReleaseNotes);
                    copyInformationBuilder.Append(SupportedUrlString);
                    copyInformationBuilder.AppendLine(updateItem.UpdateInformation.SupportURL);

                    if (updateItem.UpdateInformation.UpdateType is UpdateType.utDriver)
                    {
                        copyInformationBuilder.Append(DeviceProblemNumberString);
                        copyInformationBuilder.AppendLine(Convert.ToString(updateItem.WindowsDriverInformation.DeviceProblemNumber));
                        copyInformationBuilder.Append(DriverClassString);
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.WindowsDriverInformation.DriverClass) ? NotAvailableString : updateItem.WindowsDriverInformation.DriverClass);
                        copyInformationBuilder.Append(DriverHardwareIDString);
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.WindowsDriverInformation.DriverHardwareID) ? NotAvailableString : updateItem.WindowsDriverInformation.DriverHardwareID);
                        copyInformationBuilder.Append(DriverManufacturerString);
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.WindowsDriverInformation.DriverManufacturer) ? NotAvailableString : updateItem.WindowsDriverInformation.DriverManufacturer);
                        copyInformationBuilder.Append(DriverModelString);
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.WindowsDriverInformation.DriverModel) ? NotAvailableString : updateItem.WindowsDriverInformation.DriverModel);
                        copyInformationBuilder.Append(DriverProviderString);
                        copyInformationBuilder.AppendLine(string.IsNullOrEmpty(updateItem.WindowsDriverInformation.DriverProvider) ? NotAvailableString : updateItem.WindowsDriverInformation.DriverProvider);
                        copyInformationBuilder.Append(DriverVerDateString);
                        copyInformationBuilder.AppendLine(updateItem.WindowsDriverInformation.DriverVerDate.ToString("yyyy/MM/dd"));
                    }

                    return Convert.ToString(copyInformationBuilder);
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(copyString);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 隐藏更新：修改隐藏更新项选中状态
        /// </summary>
        private void OnHiddenCheckBoxClickExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter as UpdateModel is UpdateModel hiddenUpdate)
            {
                hiddenUpdate.IsSelected = !hiddenUpdate.IsSelected;
                IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
            }
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpdateModel hiddenUpdate)
            {
                hiddenUpdate.IsSelected = false;

                bool showResult = await Task.Run(() =>
                {
                    bool result = false;

                    // 显示更新（只有 Power Users 管理组的管理员和成员才能设置此属性的值）
                    if (RuntimeHelper.IsElevated && !hiddenUpdate.IsUpdating)
                    {
                        try
                        {
                            hiddenUpdate.UpdateInformation.IsHidden = false;
                            hiddenUpdate.UpdateInformation.Update.IsHidden = false;
                            result = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnHiddenShowExecuteRequested), 1, e);
                        }
                    }

                    return result;
                });

                // 显示成功时更新界面显示内容
                if (showResult)
                {
                    try
                    {
                        hiddenUpdate.UpdateProgress = UpdateNotInstalledString;
                        HiddenUpdateCollection.Remove(hiddenUpdate);
                        AvailableUpdateCollection.Add(hiddenUpdate);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnHiddenShowExecuteRequested), 2, e);
                    }

                    IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                    IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                    IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                    IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
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
                IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
                IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);

                Task.Run(() =>
                {
                    try
                    {
                        if (uninstallationJobDict.TryGetValue(installedUpdate.UpdateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            uninstallationJobDict.Remove(installedUpdate.UpdateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnInstalledCancelUninstallExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 已安装更新：修改已安装更新项选中状态
        /// </summary>
        private void OnInstalledCheckBoxClickExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter as UpdateModel is UpdateModel installedUpdate)
            {
                installedUpdate.IsSelected = !installedUpdate.IsSelected;
                IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
                IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);
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
                        IInstallationResult installationResult = await Task.Run(() =>
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
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnInstalledUninstallExecuteRequested), 1, e);
                                return null;
                            }
                        });

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

                        // 移除更新卸载任务
                        uninstallationJobDict.Remove(installedUpdate.UpdateID);
                        IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
                        IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);

                        // 所有更新下载、安装和卸载完成，恢复检查更新功能
                        if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                        {
                            IsCheckUpdateEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnInstalledUninstallExecuteRequested), 2, e);
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
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(supportUrl);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnLearnMoreExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 更新历史记录：打开更新对应的受支持的链接
        /// </summary>
        private void OnOpenSupportUrlExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string supportUrl && !string.IsNullOrEmpty(supportUrl))
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(supportUrl);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnOpenSupportUrlExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 已安装更新：使用命令卸载更新
        /// </summary>
        private void OnUninstallWithCmdExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string title && !string.IsNullOrEmpty(title))
            {
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnUninstallWithCmdExecuteRequested), 1, e);
                    }
                });
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：Windows 更新管理页面——挂载的事件

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsCheckUpdateEnabled = false;
            IsChecking = true;
            await CheckUpdate();
        }

        /// <summary>
        /// 打开 Windows 更新
        /// </summary>
        private void OnWindowsUpdateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsupdate");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnWindowsUpdateClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开 Windows 更新历史记录
        /// </summary>
        private void OnWindowsUpdateHistoryClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsupdate-history");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnWindowsUpdateHistoryClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:windowsinsider");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnWIPSettingsClicked), 1, e);
                }
            });
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
        private void OnSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectedItem = sender.SelectedItem;
        }

        /// <summary>
        /// 可用更新：全选
        /// </summary>
        private void OnAvailableSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = true;
            }

            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
        }

        /// <summary>
        /// 可用更新：全部不选
        /// </summary>
        private void OnAvailableSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            IsAvailableHideEnabled = false;
            IsAvailableInstallEnabled = false;
            IsAvailableCancelInstallEnabled = false;
        }

        /// <summary>
        /// 可用更新：安装选定的更新
        /// </summary>
        private void OnAvailableInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> installList = [.. AvailableUpdateCollection.Where(item => item.IsSelected)];

            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            IsCheckUpdateEnabled = false;
            foreach (UpdateModel installItem in installList)
            {
                if (!installItem.IsUpdating)
                {
                    // 更新更新项的状态
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase) && !availableUpdateItem.IsUpdating)
                        {
                            availableUpdateItem.IsUpdating = true;
                            availableUpdateItem.IsUpdateCanceled = false;
                            availableUpdateItem.UpdatePercentage = 0;
                            availableUpdateItem.IsUpdatePreparing = true;
                            availableUpdateItem.UpdateProgress = UpdatePrepareInstallingString;
                            break;
                        }
                    }

                    Task.Run(() =>
                    {
                        AutoResetEvent updateProgressEvent = new(false);
                        bool updateResult = false;
                        UpdateCollection updateCollection = new() { installItem.UpdateInformation.Update };

                        // 先下载更新
                        IDownloadResult downloadResult = null;

                        try
                        {
                            AutoResetEvent downloadEvent = new(false);
                            IDownloadJob downloadJob = null;
                            UpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();
                            updateDownloader.Updates = updateCollection;
                            DownloadProgressChangedCallback downloadProgressChangedCallback = new();
                            DownloadCompletedCallback downloadCompletedCallback = new();
                            downloadProgressChangedCallback.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(sender, args, installItem);
                            downloadCompletedCallback.DownloadCompleted += (sender, args) => downloadEvent.Set();
                            downloadJob = updateDownloader.BeginDownload(downloadProgressChangedCallback, downloadCompletedCallback, null);

                            if (!downloadJobDict.ContainsKey(installItem.UpdateID))
                            {
                                downloadJobDict.Add(installItem.UpdateID, downloadJob);
                            }

                            downloadEvent.WaitOne();
                            downloadEvent.Dispose();
                            downloadResult = updateDownloader.EndDownload(downloadJob);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableInstallClicked), 1, e);
                        }

                        synchronizationContext.Post(_ =>
                        {
                            if (downloadResult is not null)
                            {
                                // 更新下载成功
                                if (downloadResult.ResultCode is OperationResultCode.orcSucceeded)
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.UpdatePercentage = 50;
                                            availableUpdateItem.UpdateProgress = DownloadUpdateCompletedString;
                                            break;
                                        }
                                    }
                                    updateResult = true;
                                }
                                // 更新下载已取消
                                else if (downloadResult.ResultCode is OperationResultCode.orcAborted)
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            availableUpdateItem.UpdateProgress = DownloadUpdateCanceledString;
                                        }
                                    }
                                }
                                // 更新下载失败
                                else
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);
                                            availableUpdateItem.UpdateProgress = exception is not null ? string.Format(DownloadUpdateFailedWithInformationString, exception.Message) : string.Format(DownloadUpdateFailedWithCodeString, downloadResult.HResult);
                                            break;
                                        }
                                    }
                                }
                            }
                            // 更新下载失败
                            else
                            {
                                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                {
                                    if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        availableUpdateItem.IsUpdateCanceled = false;
                                        availableUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(downloadResult.HResult);
                                        availableUpdateItem.UpdateProgress = exception is not null ? string.Format(DownloadUpdateFailedWithInformationString, exception.Message) : string.Format(DownloadUpdateFailedWithCodeString, downloadResult.HResult);
                                        break;
                                    }
                                }
                            }

                            updateProgressEvent.Set();
                        }, null);

                        updateProgressEvent.WaitOne();

                        // 移除更新下载任务
                        downloadJobDict.Remove(installItem.UpdateID);

                        // 保证更新下载成功后再进行安装更新
                        if (updateResult)
                        {
                            IInstallationResult installationResult = null;

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
                                installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnInstallationProgressChanged(sender, args, installItem);
                                installationJob = updateInstaller.BeginInstall(installationProgressChangedCallback, installationCompletedCallback, null);

                                if (!installationJobDict.ContainsKey(installItem.UpdateID))
                                {
                                    installationJobDict.Add(installItem.UpdateID, installationJob);
                                }

                                installEvent.WaitOne();
                                installEvent.Dispose();
                                installationResult = updateInstaller.EndInstall(installationJob);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableInstallClicked), 2, e);
                            }

                            synchronizationContext.Post(_ =>
                            {
                                if (installationResult is not null)
                                {
                                    // 更新安装成功
                                    if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                                    {
                                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                        {
                                            if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                            {
                                                availableUpdateItem.UpdatePercentage = 0;
                                                availableUpdateItem.IsUpdateCanceled = true;
                                                availableUpdateItem.UpdateProgress = installationResult.RebootRequired ? InstallUpdateCompletedNeedRebootString : InstallUpdateCompletedString;

                                                if (installationResult.RebootRequired && !IsUpdateCompletedNeedRebootPrompt)
                                                {
                                                    IsUpdateCompletedNeedRebootPrompt = true;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    // 更新安装已取消
                                    else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                                    {
                                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                        {
                                            if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                            {
                                                availableUpdateItem.IsUpdateCanceled = false;
                                                availableUpdateItem.IsUpdating = false;
                                                availableUpdateItem.UpdateProgress = InstallUpdateCanceledString;
                                            }
                                        }
                                    }
                                    // 更新安装失败
                                    else
                                    {
                                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                        {
                                            if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                            {
                                                availableUpdateItem.IsUpdateCanceled = false;
                                                availableUpdateItem.IsUpdating = false;
                                                Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                                                availableUpdateItem.UpdateProgress = exception is not null ? string.Format(InstallUpdateFailedWithInformationString, exception.Message) : string.Format(InstallUpdateFailedWithCodeString, installationResult.HResult);
                                                break;
                                            }
                                        }
                                    }
                                }
                                // 更新安装失败
                                else
                                {
                                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                                    {
                                        if (string.Equals(availableUpdateItem.UpdateID, installItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            availableUpdateItem.IsUpdateCanceled = false;
                                            availableUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                                            availableUpdateItem.UpdateProgress = exception is not null ? string.Format(InstallUpdateFailedWithInformationString, exception.Message) : string.Format(InstallUpdateFailedWithCodeString, installationResult.HResult);
                                            break;
                                        }
                                    }
                                }

                                updateProgressEvent.Set();
                            }, null);

                            updateProgressEvent.WaitOne();
                            updateProgressEvent.Dispose();

                            // 移除更新安装任务
                            installationJobDict.Remove(installItem.UpdateID);
                        }

                        // 当前更新的下载和安装所有步骤都已完成
                        synchronizationContext.Post(_ =>
                        {
                            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);

                            // 所有更新下载、安装和卸载完成，恢复检查更新功能
                            if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                            {
                                IsCheckUpdateEnabled = true;
                            }
                        }, null);
                    });
                }
            }

            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
        }

        /// <summary>
        /// 可用更新：隐藏
        /// </summary>
        private async void OnAvailableHideClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> hideList = [.. AvailableUpdateCollection.Where(item => item.IsSelected)];

            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            bool hideResult = await Task.Run(() =>
            {
                bool result = false;
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel hideItem in hideList)
                    {
                        foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                        {
                            if (string.Equals(availableUpdateItem.UpdateID, hideItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!availableUpdateItem.IsUpdating)
                                {
                                    try
                                    {
                                        availableUpdateItem.UpdateInformation.IsHidden = true;
                                        availableUpdateItem.UpdateInformation.Update.IsHidden = true;
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableHideClicked), 1, e);
                                        continue;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    result = true;
                }

                return result;
            });

            if (hideResult)
            {
                foreach (UpdateModel hideItem in hideList)
                {
                    try
                    {
                        if (hideItem.UpdateInformation.Update.IsHidden)
                        {
                            for (int index = 0; index < AvailableUpdateCollection.Count; index++)
                            {
                                if (string.Equals(AvailableUpdateCollection[index].UpdateID, hideItem.UpdateID))
                                {
                                    AvailableUpdateCollection[index].UpdateProgress = string.Empty;
                                    AvailableUpdateCollection.RemoveAt(index);
                                    break;
                                }
                            }

                            HiddenUpdateCollection.Add(hideItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableHideClicked), 2, e);
                    }
                }

                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
            }
        }

        /// <summary>
        /// 可用更新：取消安装选定的更新
        /// </summary>
        private void OnAvailableCancelInstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> cancelList = [.. AvailableUpdateCollection.Where(item => item.IsSelected)];

            foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
            {
                availableUpdateItem.IsSelected = false;
            }

            foreach (UpdateModel cancelItem in cancelList)
            {
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (string.Equals(availableUpdateItem.UpdateID, cancelItem.UpdateID, StringComparison.OrdinalIgnoreCase) && availableUpdateItem.IsUpdating)
                    {
                        availableUpdateItem.UpdateProgress = UpdateCancelingString;
                        availableUpdateItem.IsUpdateCanceled = true;
                        break;
                    }
                }

                Task.Run(() =>
                {
                    try
                    {
                        if (downloadJobDict.TryGetValue(cancelItem.UpdateID, out IDownloadJob downloadJob))
                        {
                            downloadJob.RequestAbort();
                            downloadJobDict.Remove(cancelItem.UpdateID);
                        }

                        if (installationJobDict.TryGetValue(cancelItem.UpdateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            installationJobDict.Remove(cancelItem.UpdateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnAvailableCancelInstallClicked), 1, e);
                    }
                });
            }

            IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
            IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
            IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
        }

        /// <summary>
        /// 已安装更新：全选
        /// </summary>
        private void OnInstalledSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = true;
            }

            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);
        }

        /// <summary>
        /// 已安装更新：全部不选
        /// </summary>
        private void OnInstalledSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = false;
            }

            IsInstalledUninstallEnabled = false;
            IsInstalledCancelInstallEnabled = false;
        }

        /// <summary>
        /// 已安装更新：卸载
        /// </summary>
        private void OnInstalledUninstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> uninstallList = [.. InstalledUpdateCollection.Where(item => item.IsSelected)];

            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = false;
            }

            IsCheckUpdateEnabled = false;
            foreach (UpdateModel uninstallItem in uninstallList)
            {
                if (!uninstallItem.IsUpdating)
                {
                    // 更新卸载项的状态
                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                    {
                        if (string.Equals(installedUpdateItem.UpdateID, uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase) && !installedUpdateItem.IsUpdating)
                        {
                            installedUpdateItem.IsUpdating = true;
                            installedUpdateItem.IsUpdateCanceled = false;
                            installedUpdateItem.UpdatePercentage = 0;
                            installedUpdateItem.IsUpdatePreparing = true;
                            installedUpdateItem.UpdateProgress = UpdatePrepareUninstallingString;
                            break;
                        }
                    }

                    Task.Run(() =>
                    {
                        AutoResetEvent updateProgressEvent = new(false);
                        UpdateCollection updateCollection = new() { uninstallItem.UpdateInformation.Update };

                        // 卸载更新
                        IInstallationResult installationResult = null;

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
                            installationProgressChangedCallback.InstallationProgressChanged += (sender, args) => OnUninstallationProgressChanged(sender, args, uninstallItem);
                            installationJob = updateInstaller.BeginUninstall(installationProgressChangedCallback, installationCompletedCallback, null);

                            if (!uninstallationJobDict.ContainsKey(uninstallItem.UpdateID))
                            {
                                uninstallationJobDict.Add(uninstallItem.UpdateID, installationJob);
                            }

                            uninstallEvent.WaitOne();
                            uninstallEvent.Dispose();
                            installationResult = updateInstaller.EndUninstall(installationJob);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnInstalledUninstallClicked), 1, e);
                        }

                        synchronizationContext.Post(_ =>
                        {
                            if (installationResult is not null)
                            {
                                // 更新卸载成功
                                if (installationResult.ResultCode is OperationResultCode.orcSucceeded)
                                {
                                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                    {
                                        if (string.Equals(installedUpdateItem.UpdateID, uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            installedUpdateItem.UpdatePercentage = 0;
                                            installedUpdateItem.IsUpdateCanceled = true;
                                            installedUpdateItem.UpdateProgress = installationResult.RebootRequired ? UninstallUpdateCompletedNeedRebootString : UninstallUpdateCompletedString;

                                            if (installationResult.RebootRequired && !IsUpdateCompletedNeedRebootPrompt)
                                            {
                                                IsUpdateCompletedNeedRebootPrompt = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                                // 更新卸载已取消
                                else if (installationResult.ResultCode is OperationResultCode.orcAborted)
                                {
                                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                    {
                                        if (string.Equals(installedUpdateItem.UpdateID, uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            installedUpdateItem.IsUpdateCanceled = false;
                                            installedUpdateItem.IsUpdating = false;
                                            installedUpdateItem.UpdateProgress = UninstallUpdateCanceledString;
                                        }
                                    }
                                }
                                // 更新安装失败
                                else
                                {
                                    foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                    {
                                        if (string.Equals(installedUpdateItem.UpdateID, uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            installedUpdateItem.IsUpdateCanceled = false;
                                            installedUpdateItem.IsUpdating = false;
                                            Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                                            installedUpdateItem.UpdateProgress = exception is not null ? string.Format(UninstallUpdateFailedWithInformationString, exception.Message) : string.Format(UninstallUpdateFailedWithCodeString, installationResult.HResult);
                                            break;
                                        }
                                    }
                                }
                            }
                            // 更新安装失败
                            else
                            {
                                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                                {
                                    if (string.Equals(installedUpdateItem.UpdateID, uninstallItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        installedUpdateItem.IsUpdateCanceled = false;
                                        installedUpdateItem.IsUpdating = false;
                                        Exception exception = Marshal.GetExceptionForHR(installationResult.HResult);
                                        installedUpdateItem.UpdateProgress = exception is not null ? string.Format(UninstallUpdateFailedWithInformationString, exception.Message) : string.Format(UninstallUpdateFailedWithCodeString, installationResult.HResult);
                                        break;
                                    }
                                }
                            }

                            updateProgressEvent.Set();
                        }, null);

                        updateProgressEvent.WaitOne();
                        updateProgressEvent.Dispose();

                        // 移除更新卸载任务
                        uninstallationJobDict.Remove(uninstallItem.UpdateID);

                        // 当前更新的卸载所有步骤都已完成
                        synchronizationContext.Post(_ =>
                        {
                            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
                            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);

                            // 所有更新下载、安装和卸载完成，恢复检查更新功能
                            if (downloadJobDict.Count is 0 && installationJobDict.Count is 0 && uninstallationJobDict.Count is 0)
                            {
                                IsCheckUpdateEnabled = true;
                            }
                        }, null);
                    });
                }
            }

            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);
        }

        /// <summary>
        /// 已安装更新：取消卸载更新
        /// </summary>
        private void OnInstalledCancelUninstallClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> cancelList = [.. InstalledUpdateCollection.Where(item => item.IsSelected)];

            foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
            {
                installedUpdateItem.IsSelected = false;
            }

            foreach (UpdateModel cancelItem in cancelList)
            {
                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (string.Equals(installedUpdateItem.UpdateID, cancelItem.UpdateID, StringComparison.OrdinalIgnoreCase) && installedUpdateItem.IsUpdating)
                    {
                        installedUpdateItem.UpdateProgress = UpdateCancelingString;
                        installedUpdateItem.IsUpdateCanceled = true;
                        break;
                    }
                }

                Task.Run(() =>
                {
                    try
                    {
                        if (uninstallationJobDict.TryGetValue(cancelItem.UpdateID, out IInstallationJob installationJob))
                        {
                            installationJob.RequestAbort();
                            uninstallationJobDict.Remove(cancelItem.UpdateID);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnInstalledCancelUninstallClicked), 2, e);
                    }
                });
            }

            IsInstalledUninstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && item.UpdateInformation.IsCanUninstall);
            IsInstalledCancelInstallEnabled = InstalledUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled && item.UpdateInformation.IsCanUninstall);
        }

        /// <summary>
        /// 隐藏更新：全选
        /// </summary>
        private void OnHiddenSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
            {
                hiddenUpdateItem.IsSelected = true;
            }

            IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
        }

        /// <summary>
        /// 隐藏更新：全部不选
        /// </summary>
        private void OnHiddenSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
            {
                hiddenUpdateItem.IsSelected = false;
            }

            IsHiddenShowEnabled = false;
        }

        /// <summary>
        /// 隐藏更新：显示
        /// </summary>
        private async void OnHiddenShowClicked(object sender, RoutedEventArgs args)
        {
            List<UpdateModel> showList = [.. HiddenUpdateCollection.Where(item => item.IsSelected)];

            foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
            {
                hiddenUpdateItem.IsSelected = false;
            }

            bool showResult = await Task.Run(() =>
            {
                bool result = false;
                if (RuntimeHelper.IsElevated)
                {
                    foreach (UpdateModel showItem in showList)
                    {
                        foreach (UpdateModel hiddenUpdateItem in HiddenUpdateCollection)
                        {
                            if (string.Equals(hiddenUpdateItem.UpdateID, showItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!showItem.IsUpdating)
                                {
                                    try
                                    {
                                        hiddenUpdateItem.UpdateInformation.IsHidden = false;
                                        hiddenUpdateItem.UpdateInformation.Update.IsHidden = false;
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnHiddenShowClicked), 1, e);
                                        continue;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    result = true;
                }

                return result;
            });

            if (showResult)
            {
                foreach (UpdateModel showItem in showList)
                {
                    try
                    {
                        if (!showItem.UpdateInformation.Update.IsHidden)
                        {
                            for (int index = 0; index < HiddenUpdateCollection.Count; index++)
                            {
                                if (string.Equals(HiddenUpdateCollection[index].UpdateID, showItem.UpdateID))
                                {
                                    HiddenUpdateCollection[index].UpdateProgress = UpdateNotInstalledString;
                                    HiddenUpdateCollection.RemoveAt(index);
                                    break;
                                }
                            }

                            AvailableUpdateCollection.Add(showItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnHiddenShowClicked), 2, e);
                    }
                }

                IsAvailableHideEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating && !item.UpdateInformation.IsHidden);
                IsAvailableInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && !item.IsUpdating);
                IsAvailableCancelInstallEnabled = AvailableUpdateCollection.Any(item => item.IsSelected && item.IsUpdating && !item.IsUpdateCanceled);
                IsHiddenShowEnabled = HiddenUpdateCollection.Any(item => item.IsSelected && item.UpdateInformation.IsHidden);
            }
        }

        /// <summary>
        /// 重启以完成后续更新的安装或卸载
        /// </summary>
        private void OnRebootClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
            });
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
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsIncludePotentiallySupersededUpdate = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置更新源
        /// </summary>
        private void OnUpdateSourceSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is KeyValuePair<string, string> updateSource)
            {
                SelectedUpdateSource = updateSource;
            }
        }

        /// <summary>
        /// Windows 更新不包括驱动程序
        /// </summary>
        private async void OnExcludeDriversToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                bool value = toggleSwitch.IsOn;
                IsExcludeDrivers = await Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", value);
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate");
                });
            }
        }

        /// <summary>
        /// 清除历史更新记录
        /// </summary>
        private async void OnCleanUpdateHistoryClicked(object sender, RoutedEventArgs args)
        {
            IsCleaning = true;

            bool result = await Task.Run(() =>
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(OnCleanUpdateHistoryClicked), 1, e);
                    return false;
                }
            });

            IsCleaning = false;
            synchronizationContext.Send(async (_) =>
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CleanUpdateHistory, result));
            }, null);

            if (true)
            {
                // 获取历史更新记录
                List<UpdateModel> updateHistoryList = await Task.Run(GetUpdateHistoryList);

                UpdateHistoryCollection.Clear();
                foreach (UpdateModel updateHistoryItem in updateHistoryList)
                {
                    UpdateHistoryCollection.Add(updateHistoryItem);
                }
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

        #endregion 第三部分：Windows 更新管理页面——挂载的事件

        #region 第四部分：Windows 更新管理页面——自定义事件

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
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (string.Equals(availableUpdateItem.UpdateID, updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                        {
                            availableUpdateItem.IsUpdatePreparing = false;
                            availableUpdateItem.UpdatePercentage = percentage / 2;
                            availableUpdateItem.UpdateProgress = string.Format(DownloadUpdateProgressInitializingString, percentage);
                            break;
                        }
                    }
                }, null);
            }
            // 下载当前更新
            else if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphDownloading)
            {
                synchronizationContext.Post(_ =>
                {
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (string.Equals(availableUpdateItem.UpdateID, updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                        {
                            availableUpdateItem.IsUpdatePreparing = false;
                            availableUpdateItem.UpdatePercentage = percentage / 2;
                            availableUpdateItem.UpdateProgress = string.Format(DownloadUpdateProgressDownloadingString, percentage);
                            break;
                        }
                    }
                }, null);
            }
            else if (downloadProgress.CurrentUpdateDownloadPhase is DownloadPhase.dphVerifying)
            {
                synchronizationContext.Post(_ =>
                {
                    foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                    {
                        if (string.Equals(availableUpdateItem.UpdateID, updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                        {
                            availableUpdateItem.IsUpdatePreparing = false;
                            availableUpdateItem.UpdatePercentage = percentage / 2;
                            availableUpdateItem.UpdateProgress = string.Format(DownloadUpdateProgressVerifyingString, percentage);
                            break;
                        }
                    }
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
                foreach (UpdateModel availableUpdateItem in AvailableUpdateCollection)
                {
                    if (string.Equals(availableUpdateItem.UpdateID, updateItem.UpdateID, StringComparison.OrdinalIgnoreCase))
                    {
                        availableUpdateItem.UpdatePercentage = 50 + percentage / 2;
                        availableUpdateItem.UpdateProgress = string.Format(InstallUpdateProgressString, percentage);
                        break;
                    }
                }
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
                foreach (UpdateModel installedUpdateItem in InstalledUpdateCollection)
                {
                    if (string.Equals(installedUpdateItem.UpdateID, updateItem.UpdateID))
                    {
                        installedUpdateItem.UpdatePercentage = percentage;
                        installedUpdateItem.UpdateProgress = string.Format(UninstallUpdateProgressString, percentage);
                        break;
                    }
                }
            }, null);
        }

        #endregion 第四部分：Windows 更新管理页面——自定义事件

        /// <summary>
        /// 检查更新
        /// </summary>
        private async Task CheckUpdate()
        {
            List<UpdateModel> availableUpdateList = [];
            List<UpdateModel> installedUpdateList = [];
            List<UpdateModel> hiddenUpdateList = [];
            List<UpdateModel> updateHistoryList = [];

            bool searchResult = await Task.Run(() =>
            {
                bool result = false;

                // 设置搜索结果中是否接收已替代的更新
                updateSearcher.IncludePotentiallySupersededUpdates = IsIncludePotentiallySupersededUpdate;
                updateSearcher.ServerSelection = ServerSelection.ssDefault;
                updateSearcher.IgnoreDownloadPriority = true;

                // 设置更新源
                foreach (IUpdateService2 updateService in updateServiceManager.Services)
                {
                    if (string.Equals(updateService.Name, SelectedUpdateSource.Key, StringComparison.OrdinalIgnoreCase))
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
                                IsSelected = false,
                                IsUpdateCanceled = false,
                                IsUpdatePreparing = false,
                                UpdatePercentage = 0,
                                WindowsDriverInformation = new WindowsDriverInformation(),
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpdateManagerPage), nameof(CheckUpdate), 1, e);
                }

                return result;
            });

            if (searchResult)
            {
                AvailableUpdateCollection.Clear();
                foreach (UpdateModel availableUpdateItem in availableUpdateList)
                {
                    AvailableUpdateCollection.Add(availableUpdateItem);
                }

                InstalledUpdateCollection.Clear();
                foreach (UpdateModel installedUpdateItem in installedUpdateList)
                {
                    InstalledUpdateCollection.Add(installedUpdateItem);
                }

                HiddenUpdateCollection.Clear();
                foreach (UpdateModel hiddenUpdateItem in hiddenUpdateList)
                {
                    HiddenUpdateCollection.Add(hiddenUpdateItem);
                }

                UpdateHistoryCollection.Clear();
                foreach (UpdateModel updateHistoryItem in updateHistoryList)
                {
                    UpdateHistoryCollection.Add(updateHistoryItem);
                }
            }

            IsCheckUpdateEnabled = true;
            IsChecking = false;
        }

        /// <summary>
        /// 获取历史更新记录
        /// </summary>
        private List<UpdateModel> GetUpdateHistoryList()
        {
            List<UpdateModel> updateHistoryList = [];

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
                    RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\PCHC", "UpgradeEligibility");
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
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\PCHC", "UpgradeEligibility", 1);

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
            });

            return default;
        }

        /// <summary>
        /// 暂停服务
        /// </summary>
        private void StopService(string serviceName)
        {
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
        /// 开启服务
        /// </summary>
        private void StartService(string serviceName)
        {
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

        private bool GetUpdateState(bool param1, int param2)
        {
            return param1 && Convert.ToBoolean(param2);
        }
    }
}
