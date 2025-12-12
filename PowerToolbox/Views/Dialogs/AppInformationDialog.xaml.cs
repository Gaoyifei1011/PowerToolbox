using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.KernelAppCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string WindowsAppSDKVersionString = ResourceService.DialogResource.GetString("WindowsAppSDKVersion");
        private readonly string WinUIVersionString = ResourceService.DialogResource.GetString("WinUIVersion");
        private readonly string DoNetVersionString = ResourceService.DialogResource.GetString("DoNetVersion");

        private bool _isLoadCompleted = false;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private WinRTObservableCollection<DictionaryEntry> AppInformationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用信息初始化触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            List<KeyValuePair<string, Version>> dependencyInformationList = [];
            await Task.Run(() =>
            {
                uint bufferLength = 0;

                KernelAppCoreLibrary.GetCurrentPackageInfo(PACKAGE_FLAGS.PACKAGE_PROPERTY_STATIC, ref bufferLength, null, out uint count);

                if (count > 0)
                {
                    List<PACKAGE_INFO> packageInfoList = [];
                    byte[] buffer = new byte[bufferLength];
                    KernelAppCoreLibrary.GetCurrentPackageInfo(PACKAGE_FLAGS.PACKAGE_PROPERTY_STATIC, ref bufferLength, buffer, out count);

                    for (int index = 0; index < count; index++)
                    {
                        int packageInfoSize = Marshal.SizeOf<PACKAGE_INFO>();
                        nint packageInfoPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, index * packageInfoSize);
                        Marshal.Copy(buffer, index * packageInfoSize, packageInfoPtr, packageInfoSize);
                        PACKAGE_INFO packageInfo = Marshal.PtrToStructure<PACKAGE_INFO>(packageInfoPtr);
                        packageInfoList.Add(packageInfo);
                    }

                    foreach (PACKAGE_INFO packageInfo in packageInfoList)
                    {
                        // WinUI 3 版本信息
                        if (packageInfo.packageFullName.Contains("Microsoft.WindowsAppRuntime"))
                        {
                            dependencyInformationList.Add(new KeyValuePair<string, Version>(WindowsAppSDKVersionString, new Version(packageInfo.packageId.version.Parts.Major, packageInfo.packageId.version.Parts.Minor, packageInfo.packageId.version.Parts.Build, packageInfo.packageId.version.Parts.Revision)));

                            FileVersionInfo winUI3File = FileVersionInfo.GetVersionInfo(Path.Combine(packageInfo.path, "Microsoft.UI.Xaml.Controls.dll"));
                            dependencyInformationList.Add(new KeyValuePair<string, Version>(WinUIVersionString, new Version(winUI3File.FileMajorPart, winUI3File.FileMinorPart, winUI3File.FileBuildPart, winUI3File.FilePrivatePart)));
                            break;
                        }
                    }

                    // .NET 版本信息
                    dependencyInformationList.Add(new KeyValuePair<string, Version>(DoNetVersionString, new Version(RuntimeInformation.FrameworkDescription.Remove(0, 15))));
                }
            });

            foreach (KeyValuePair<string, Version> dependencyInformation in dependencyInformationList)
            {
                AppInformationCollection.Add(new DictionaryEntry(dependencyInformation.Key, dependencyInformation.Value));
            }

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool copyResult = false;
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();
                    foreach (DictionaryEntry appInformationItem in AppInformationCollection)
                    {
                        stringBuilder.Append(appInformationItem.Key);
                        stringBuilder.Append(appInformationItem.Value);
                        stringBuilder.Append(Environment.NewLine);
                    }

                    return stringBuilder;
                });

                copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AppInformationDialog), nameof(OnCopyAppInformationClicked), 1, e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
        }
    }
}
