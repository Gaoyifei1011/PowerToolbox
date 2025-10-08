using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Threading;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 更新应用对话框
    /// </summary>
    public sealed partial class UpdateAppDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string CancelString = ResourceService.DialogResource.GetString("Cancel");
        private readonly string CloseString = ResourceService.DialogResource.GetString("Close");
        private readonly string CloseAppString = ResourceService.DialogResource.GetString("CloseApp");
        private readonly string UpdateString = ResourceService.DialogResource.GetString("Update");
        private readonly string UpdateDownloadingString = ResourceService.DialogResource.GetString("UpdateDownloading");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private Progress<StorePackageUpdateStatus> storePackageUpdateProgress = null;
        private CancellationTokenSource cancellationTokenSource = null;

        private UpdateAppResultKind _updateAppResultKind = UpdateAppResultKind.Initialize;

        public UpdateAppResultKind UpdateAppResultKind
        {
            get { return _updateAppResultKind; }

            set
            {
                if (!Equals(_updateAppResultKind, value))
                {
                    _updateAppResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateAppResultKind)));
                }
            }
        }

        private string _primaryText;

        public string PrimaryText
        {
            get { return _primaryText; }

            set
            {
                if (!string.Equals(_primaryText, value))
                {
                    _primaryText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrimaryText)));
                }
            }
        }

        private string _closeText;

        public string CloseText
        {
            get { return _closeText; }

            set
            {
                if (!string.Equals(_closeText, value))
                {
                    _closeText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CloseText)));
                }
            }
        }

        private string _updateDownloadString;

        public string UpdateDownloadString
        {
            get { return _updateDownloadString; }

            set
            {
                if (!string.Equals(_updateDownloadString, value))
                {
                    _updateDownloadString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateDownloadString)));
                }
            }
        }

        private bool _isCancelingUpdate;

        public bool IsCancelingUpdate
        {
            get { return _isCancelingUpdate; }

            set
            {
                if (!Equals(_isCancelingUpdate, value))
                {
                    _isCancelingUpdate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCancelingUpdate)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateAppDialog()
        {
            InitializeComponent();
            PrimaryText = UpdateString;
            CloseText = CloseString;
        }

        /// <summary>
        /// 对话框关闭后触发的事件
        /// </summary>
        private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (cancellationTokenSource is not null && (UpdateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Deploying))
            {
                try
                {
                    cancellationTokenSource.Cancel();
                    UpdateAppResultKind = UpdateAppResultKind.Canceling;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpdateAppDialog), nameof(OnCancelOrCloseClicked), 1, e);
                }
            }
        }

        /// <summary>
        /// 更新应用
        /// </summary>
        private async void OnUpdateClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                args.Cancel = true;
                if (UpdateAppResultKind is UpdateAppResultKind.Successfully)
                {
                    (Application.Current as XamlIslandsApp).Dispose();
                }
                else
                {
                    UpdateAppResultKind = UpdateAppResultKind.Pending;
                    UpdateDownloadString = string.Format(UpdateDownloadingString, VolumeSizeHelper.ConvertVolumeSizeToString(0), VolumeSizeHelper.ConvertVolumeSizeToString(0));
                    CloseText = CancelString;
                    if (cancellationTokenSource is null)
                    {
                        StoreContext storeContext = StoreContext.GetDefault();
                        IReadOnlyList<StorePackageUpdate> storePackageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
                        cancellationTokenSource = new();
                        bool updateFailed = false;
                        storePackageUpdateProgress = new();
                        storePackageUpdateProgress.ProgressChanged += (sender, progress) =>
                        {
                            if (progress.PackageUpdateState is StorePackageUpdateState.Pending)
                            {
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Pending;
                                        CloseText = CancelString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.Downloading)
                            {
                                synchronizationContext.Post((_) =>
                                {
                                    string downloadedSize = VolumeSizeHelper.ConvertVolumeSizeToString(progress.PackageDownloadSizeInBytes);
                                    string totalSize = VolumeSizeHelper.ConvertVolumeSizeToString(progress.PackageBytesDownloaded);
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Downloading;
                                        UpdateDownloadString = string.Format(UpdateDownloadingString, downloadedSize, totalSize);
                                        CloseText = CancelString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.Deploying)
                            {
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Deploying;
                                        CloseText = CancelString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.Canceled)
                            {
                                updateFailed = true;
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Canceled;
                                        CloseText = CloseString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.OtherError)
                            {
                                updateFailed = true;
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                                        CloseText = CloseString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.ErrorLowBattery)
                            {
                                updateFailed = true;
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                                        CloseText = CloseString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.ErrorWiFiRecommended)
                            {
                                updateFailed = true;
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                                        CloseText = CloseString;
                                    }
                                }, null);
                            }
                            else if (progress.PackageUpdateState is StorePackageUpdateState.ErrorWiFiRequired)
                            {
                                updateFailed = true;
                                synchronizationContext.Post((_) =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                                        CloseText = CloseString;
                                    }
                                }, null);
                            }
                        };
                        StorePackageUpdateResult storePackageUpdateResult = await storeContext.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdateList).AsTask(cancellationTokenSource.Token, storePackageUpdateProgress);
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = null;
                        CloseText = CloseString;
                        if (storePackageUpdateResult.OverallState is StorePackageUpdateState.Completed)
                        {
                            if (updateFailed)
                            {
                                UpdateAppResultKind = UpdateAppResultKind.Failed;
                            }
                            else
                            {
                                UpdateAppResultKind = UpdateAppResultKind.Successfully;
                                PrimaryText = CloseAppString;
                            }
                            UpdateAppResultKind = updateFailed ? UpdateAppResultKind.Failed : UpdateAppResultKind.Successfully;
                        }
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                UpdateAppResultKind = UpdateAppResultKind.Canceled;
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
                CloseText = CloseString;
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpdateAppDialog), nameof(OnUpdateClicked), 1, e);
            }
            catch (Exception e)
            {
                UpdateAppResultKind = UpdateAppResultKind.Failed;
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
                CloseText = CloseString;
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpdateAppDialog), nameof(OnUpdateClicked), 2, e);
            }
        }

        /// <summary>
        /// 取消更新或关闭更新窗口
        /// </summary>
        private void OnCancelOrCloseClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (UpdateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Deploying)
            {
                args.Cancel = true;

                if (cancellationTokenSource is not null)
                {
                    try
                    {
                        cancellationTokenSource.Cancel();
                        UpdateAppResultKind = UpdateAppResultKind.Canceling;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpdateAppDialog), nameof(OnCancelOrCloseClicked), 1, e);
                    }
                }
                else
                {
                    UpdateAppResultKind = UpdateAppResultKind.Canceled;
                }
            }
        }

        /// <summary>
        /// 检查应用是否正在更新中
        /// </summary>
        private bool GetIsNotUpdating(UpdateAppResultKind updateAppReusltKind)
        {
            return !(updateAppReusltKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Canceling || UpdateAppResultKind is UpdateAppResultKind.Deploying);
        }

        /// <summary>
        /// 检查应用是否正在更新中
        /// </summary>
        private Visibility GetUpdateProgressState(UpdateAppResultKind updateAppReusltKind)
        {
            return (updateAppReusltKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Canceling || UpdateAppResultKind is UpdateAppResultKind.Deploying) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查应用是否正在取消更新中
        /// </summary>
        private bool GetIsNotCanceling(UpdateAppResultKind updateAppReusltKind)
        {
            return updateAppReusltKind is not UpdateAppResultKind.Canceling;
        }
    }
}
