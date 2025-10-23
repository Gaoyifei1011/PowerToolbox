using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.FirewallAPI;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件恢复页面
    /// </summary>
    public sealed partial class WinFRPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;
        private readonly string DriveImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "Imageres.dll");
        private readonly string AfterFormatDiskString = ResourceService.WinFRResource.GetString("AfterFormatDisk");
        private readonly string AnyString = ResourceService.WinFRResource.GetString("Any");
        private readonly string DamagedDiskString = ResourceService.WinFRResource.GetString("DamagedDisk");
        private readonly string DeleteSometimeAgoString = ResourceService.WinFRResource.GetString("DeleteSometimeAgo");
        private readonly string DiskSpaceString = ResourceService.WinFRResource.GetString("DiskSpace");
        private readonly string ExtensiveModeString = ResourceService.WinFRResource.GetString("ExtensiveMode");
        private readonly string FATString = ResourceService.WinFRResource.GetString("FAT");
        private readonly string KeepBothString = ResourceService.WinFRResource.GetString("KeepBoth");
        private readonly string NeverOverrideString = ResourceService.WinFRResource.GetString("NeverOverride");
        private readonly string NTFSString = ResourceService.WinFRResource.GetString("NTFS");
        private readonly string NTFSModeString = ResourceService.WinFRResource.GetString("NTFSMode");
        private readonly string OverrideString = ResourceService.WinFRResource.GetString("Override");
        private readonly string PrepareScanString = ResourceService.WinFRResource.GetString("PrepareScan");
        private readonly string RecentDeleteString = ResourceService.WinFRResource.GetString("RecentDelete");
        private readonly string RecoverDeletedFileString = ResourceService.WinFRResource.GetString("RecoverDeletedFile");
        private readonly string RecoverFileString = ResourceService.WinFRResource.GetString("RecoverFile");
        private readonly string RecoveringDeletedFileString = ResourceService.WinFRResource.GetString("RecoveringDeletedFile");
        private readonly string RegularModeString = ResourceService.WinFRResource.GetString("RegularMode");
        private readonly string ScanDeletedFileString = ResourceService.WinFRResource.GetString("ScanDeletedFile");
        private readonly string ScanningDeletedFileString = ResourceService.WinFRResource.GetString("ScanningDeletedFile");
        private readonly string SegmentModeString = ResourceService.WinFRResource.GetString("SegmentMode");
        private readonly string SelectFolderString = ResourceService.WinFRResource.GetString("SelectFolder");
        private readonly string SignatureModeString = ResourceService.WinFRResource.GetString("SignatureMode");
        private readonly Guid CLSID_ProgressDialog = new("F8383852-FCD3-11d1-A6B9-006097DF5BD4");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly char[] trimCharsArray = ['\b', '\r', '\n'];
        private readonly Regex scanRegex = new(@"(\d{2})%");
        private readonly Regex recoverRegex = new(@"Files recovered: (\d+), total files: (\d+), current filename: ([\w\W]+)");
        private System.Timers.Timer winFRTimer = new();
        private ImageSource SystemDriveSource;
        private ImageSource StandardDriveSource;
        private IProgressDialog progressDialog;
        private Process winFRProcess;
        private bool isRecovering;

        private bool _isDriveLoadCompleted;

        public bool IsDriveLoadCompleted
        {
            get { return _isDriveLoadCompleted; }

            set
            {
                if (!Equals(_isDriveLoadCompleted, value))
                {
                    _isDriveLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDriveLoadCompleted)));
                }
            }
        }

        private DriveModel _selectedItem;

        public DriveModel SelectedItem
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

        private bool _isManualCloseInfoBar;

        public bool IsManualCloseInfoBar
        {
            get { return _isManualCloseInfoBar; }

            set
            {
                if (!Equals(_isManualCloseInfoBar, value))
                {
                    _isManualCloseInfoBar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsManualCloseInfoBar)));
                }
            }
        }

        private bool _isWinFRInstalled;

        public bool IsWinFRInstalled
        {
            get { return _isWinFRInstalled; }

            set
            {
                if (!Equals(_isWinFRInstalled, value))
                {
                    _isWinFRInstalled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWinFRInstalled)));
                }
            }
        }

        private string _saveFolder;

        public string SaveFolder
        {
            get { return _saveFolder; }

            set
            {
                if (!string.Equals(_saveFolder, value))
                {
                    _saveFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedRecoveryMode;

        public KeyValuePair<string, string> SelectedRecoveryMode
        {
            get { return _selectedRecoveryMode; }

            set
            {
                if (!Equals(_saveFolder, value))
                {
                    _selectedRecoveryMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRecoveryMode)));
                }
            }
        }

        private bool _useCustomLogFolder;

        public bool UseCustomLogFolder
        {
            get { return _useCustomLogFolder; }

            set
            {
                if (!Equals(_useCustomLogFolder, value))
                {
                    _useCustomLogFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseCustomLogFolder)));
                }
            }
        }

        private string _logSaveFolder;

        public string LogSaveFolder
        {
            get { return _logSaveFolder; }

            set
            {
                if (!string.Equals(_logSaveFolder, value))
                {
                    _logSaveFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogSaveFolder)));
                }
            }
        }

        private string _regularRestoreContent;

        public string RegularRestoreContent
        {
            get { return _regularRestoreContent; }

            set
            {
                if (!string.Equals(_regularRestoreContent, value))
                {
                    _regularRestoreContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegularRestoreContent)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedRegularDuplicatedFileOption;

        public KeyValuePair<string, string> SelectedRegularDuplicatedFileOption
        {
            get { return _selectedRegularDuplicatedFileOption; }

            set
            {
                if (!Equals(_selectedRegularDuplicatedFileOption, value))
                {
                    _selectedRegularDuplicatedFileOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRegularDuplicatedFileOption)));
                }
            }
        }

        private string _extensiveRestoreContent;

        public string ExtensiveRestoreContent
        {
            get { return _extensiveRestoreContent; }

            set
            {
                if (!string.Equals(_extensiveRestoreContent, value))
                {
                    _extensiveRestoreContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtensiveRestoreContent)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedExtensiveDuplicatedFileOption;

        public KeyValuePair<string, string> SelectedExtensiveDuplicatedFileOption
        {
            get { return _selectedExtensiveDuplicatedFileOption; }

            set
            {
                if (!Equals(_selectedExtensiveDuplicatedFileOption, value))
                {
                    _selectedExtensiveDuplicatedFileOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedExtensiveDuplicatedFileOption)));
                }
            }
        }

        private string _ntfsRestoreContent;

        public string NTFSRestoreContent
        {
            get { return _ntfsRestoreContent; }

            set
            {
                if (!string.Equals(_ntfsRestoreContent, value))
                {
                    _ntfsRestoreContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NTFSRestoreContent)));
                }
            }
        }

        private bool _ntfsRestoreFromRecyclebin;

        public bool NTFSRestoreFromRecyclebin
        {
            get { return _ntfsRestoreFromRecyclebin; }

            set
            {
                if (!Equals(_ntfsRestoreFromRecyclebin, value))
                {
                    _ntfsRestoreFromRecyclebin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NTFSRestoreFromRecyclebin)));
                }
            }
        }

        private bool _ntfsRestoreSystemFile;

        public bool NTFSRestoreSystemFile
        {
            get { return _ntfsRestoreSystemFile; }

            set
            {
                if (!Equals(_ntfsRestoreSystemFile, value))
                {
                    _ntfsRestoreSystemFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NTFSRestoreSystemFile)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedNTFSDuplicatedFileOption;

        public KeyValuePair<string, string> SelectedNTFSDuplicatedFileOption
        {
            get { return _selectedNTFSDuplicatedFileOption; }

            set
            {
                if (!Equals(_selectedNTFSDuplicatedFileOption, value))
                {
                    _selectedNTFSDuplicatedFileOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNTFSDuplicatedFileOption)));
                }
            }
        }

        private bool _ntfsRestoreNonMainDataStream;

        public bool NTFSRestoreNonMainDataStream
        {
            get { return _ntfsRestoreNonMainDataStream; }

            set
            {
                if (!Equals(_ntfsRestoreNonMainDataStream, value))
                {
                    _ntfsRestoreNonMainDataStream = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NTFSRestoreNonMainDataStream)));
                }
            }
        }

        private bool _ntfsUseCustomFileFilterType;

        public bool NTFSUseCustomFileFilterType
        {
            get { return _ntfsUseCustomFileFilterType; }

            set
            {
                if (!Equals(_ntfsUseCustomFileFilterType, value))
                {
                    _ntfsUseCustomFileFilterType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NTFSUseCustomFileFilterType)));
                }
            }
        }

        private string _ntfsCustomFileFilterType;

        public string NTFSCustomFileFilterType
        {
            get { return _ntfsCustomFileFilterType; }

            set
            {
                if (!string.Equals(_ntfsCustomFileFilterType, value))
                {
                    _ntfsCustomFileFilterType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NTFSCustomFileFilterType)));
                }
            }
        }

        private string _segmentRestoreContent;

        public string SegmentRestoreContent
        {
            get { return _segmentRestoreContent; }

            set
            {
                if (!string.Equals(_segmentRestoreContent, value))
                {
                    _segmentRestoreContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentRestoreContent)));
                }
            }
        }

        private bool _segmentRestoreFromRecyclebin;

        public bool SegmentRestoreFromRecyclebin
        {
            get { return _segmentRestoreFromRecyclebin; }

            set
            {
                if (!Equals(_segmentRestoreFromRecyclebin, value))
                {
                    _segmentRestoreFromRecyclebin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentRestoreFromRecyclebin)));
                }
            }
        }

        private bool _segmentRestoreSystemFile;

        public bool SegmentRestoreSystemFile
        {
            get { return _segmentRestoreSystemFile; }

            set
            {
                if (!Equals(_segmentRestoreSystemFile, value))
                {
                    _segmentRestoreSystemFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentRestoreSystemFile)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedSegmentDuplicatedFileOption;

        public KeyValuePair<string, string> SelectedSegmentDuplicatedFileOption
        {
            get { return _selectedSegmentDuplicatedFileOption; }

            set
            {
                if (!Equals(_selectedSegmentDuplicatedFileOption, value))
                {
                    _selectedSegmentDuplicatedFileOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSegmentDuplicatedFileOption)));
                }
            }
        }

        private bool _segmentRestoreNonMainDataStream;

        public bool SegmentRestoreNonMainDataStream
        {
            get { return _segmentRestoreNonMainDataStream; }

            set
            {
                if (!Equals(_segmentRestoreNonMainDataStream, value))
                {
                    _segmentRestoreNonMainDataStream = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentRestoreNonMainDataStream)));
                }
            }
        }

        private bool _segmentUseCustomFileFilterType;

        public bool SegmentUseCustomFileFilterType
        {
            get { return _segmentUseCustomFileFilterType; }

            set
            {
                if (!Equals(_segmentUseCustomFileFilterType, value))
                {
                    _segmentUseCustomFileFilterType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentUseCustomFileFilterType)));
                }
            }
        }

        private string _segmentCustomFileFilterType;

        public string SegmentCustomFileFilterType
        {
            get { return _segmentCustomFileFilterType; }

            set
            {
                if (!string.Equals(_segmentCustomFileFilterType, value))
                {
                    _segmentCustomFileFilterType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentCustomFileFilterType)));
                }
            }
        }

        private int _segmentSourceDeviceNumberSectors;

        public int SegmentSourceDeviceNumberSectors
        {
            get { return _segmentSourceDeviceNumberSectors; }

            set
            {
                if (!Equals(_segmentSourceDeviceNumberSectors, value))
                {
                    _segmentSourceDeviceNumberSectors = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentSourceDeviceNumberSectors)));
                }
            }
        }

        private int _segmentSourceDeviceClusterSize;

        public int SegmentSourceDeviceClusterSize
        {
            get { return _segmentSourceDeviceClusterSize; }

            set
            {
                if (!Equals(_segmentSourceDeviceClusterSize, value))
                {
                    _segmentSourceDeviceClusterSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SegmentSourceDeviceClusterSize)));
                }
            }
        }

        private bool _signatureUseRestoreSpecificExtensionGroups;

        public bool SignatureUseRestoreSpecificExtensionGroups
        {
            get { return _signatureUseRestoreSpecificExtensionGroups; }

            set
            {
                if (!Equals(_signatureUseRestoreSpecificExtensionGroups, value))
                {
                    _signatureUseRestoreSpecificExtensionGroups = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignatureUseRestoreSpecificExtensionGroups)));
                }
            }
        }

        private string _signatureRestoreSpecificExtensionGroupsType;

        public string SignatureRestoreSpecificExtensionGroupsType
        {
            get { return _signatureRestoreSpecificExtensionGroupsType; }

            set
            {
                if (!string.Equals(_signatureRestoreSpecificExtensionGroupsType, value))
                {
                    _signatureRestoreSpecificExtensionGroupsType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignatureRestoreSpecificExtensionGroupsType)));
                }
            }
        }

        private int _signatureSourceDeviceNumberSectors;

        public int SignatureSourceDeviceNumberSectors
        {
            get { return _signatureSourceDeviceNumberSectors; }

            set
            {
                if (!Equals(_signatureSourceDeviceNumberSectors, value))
                {
                    _signatureSourceDeviceNumberSectors = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignatureSourceDeviceNumberSectors)));
                }
            }
        }

        private int _signatureSourceDeviceClusterSize;

        public int SignatureSourceDeviceClusterSize
        {
            get { return _signatureSourceDeviceClusterSize; }

            set
            {
                if (!Equals(_signatureSourceDeviceClusterSize, value))
                {
                    _signatureSourceDeviceClusterSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignatureSourceDeviceClusterSize)));
                }
            }
        }

        private List<KeyValuePair<string, string>> RecoveryModeList { get; } = [];

        private List<KeyValuePair<string, string>> RegularDuplicatedFileOptionList { get; } = [];

        private List<KeyValuePair<string, string>> ExtensiveDuplicatedFileOptionList { get; } = [];

        private List<KeyValuePair<string, string>> NTFSDuplicatedFileOptionList { get; } = [];

        private List<KeyValuePair<string, string>> SegmentDuplicatedFileOptionList { get; } = [];

        public List<RecoveryModeSuggestionModel> RecoveryModeSuggestionList { get; } = [];

        public WinRTObservableCollection<DriveModel> DriveCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinFRPage()
        {
            InitializeComponent();

            RecoveryModeList.Add(new KeyValuePair<string, string>("RegularMode", RegularModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("ExtensiveMode", ExtensiveModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("NTFSModeMode", NTFSModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("SegmentMode", SegmentModeString));
            RecoveryModeList.Add(new KeyValuePair<string, string>("Signature", SignatureModeString));
            SelectedRecoveryMode = RecoveryModeList[0];

            RegularDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("Override", OverrideString));
            RegularDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("NeverOverride", NeverOverrideString));
            RegularDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("KeepBoth", KeepBothString));
            SelectedRegularDuplicatedFileOption = RegularDuplicatedFileOptionList[0];

            ExtensiveDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("Override", OverrideString));
            ExtensiveDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("NeverOverride", NeverOverrideString));
            ExtensiveDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("KeepBoth", KeepBothString));
            SelectedExtensiveDuplicatedFileOption = ExtensiveDuplicatedFileOptionList[0];

            NTFSDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("Override", OverrideString));
            NTFSDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("NeverOverride", NeverOverrideString));
            NTFSDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("KeepBoth", KeepBothString));
            SelectedNTFSDuplicatedFileOption = NTFSDuplicatedFileOptionList[0];

            SegmentDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("Override", OverrideString));
            SegmentDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("NeverOverride", NeverOverrideString));
            SegmentDuplicatedFileOptionList.Add(new KeyValuePair<string, string>("KeepBoth", KeepBothString));
            SelectedSegmentDuplicatedFileOption = SegmentDuplicatedFileOptionList[0];

            RecoveryModeSuggestionList.Add(new RecoveryModeSuggestionModel()
            {
                FileSystem = NTFSString,
                Circumstances = RecentDeleteString,
                RecommendedMode = RegularModeString
            });

            RecoveryModeSuggestionList.Add(new RecoveryModeSuggestionModel()
            {
                FileSystem = NTFSString,
                Circumstances = DeleteSometimeAgoString,
                RecommendedMode = ExtensiveModeString
            });

            RecoveryModeSuggestionList.Add(new RecoveryModeSuggestionModel()
            {
                FileSystem = NTFSString,
                Circumstances = AfterFormatDiskString,
                RecommendedMode = ExtensiveModeString
            });

            RecoveryModeSuggestionList.Add(new RecoveryModeSuggestionModel()
            {
                FileSystem = NTFSString,
                Circumstances = DamagedDiskString,
                RecommendedMode = ExtensiveModeString
            });

            RecoveryModeSuggestionList.Add(new RecoveryModeSuggestionModel()
            {
                FileSystem = FATString,
                Circumstances = AnyString,
                RecommendedMode = ExtensiveModeString
            });
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            IsWinFRInstalled = await GetWinFRInstalledAsync();

            if (!isInitialized)
            {
                isInitialized = true;

                try
                {
                    int iconsNum = User32Library.PrivateExtractIcons(DriveImagePath, 0, 0, 0, null, null, 0, 0);
                    nint[] phicon = new nint[iconsNum];
                    int[] piconid = new int[iconsNum];
                    int nIcons = User32Library.PrivateExtractIcons(DriveImagePath, 31, 256, 256, phicon, piconid, 1, 0);

                    Icon icon = Icon.FromHandle(phicon[0]);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    SystemDriveSource = bitmapImage;
                    icon.Dispose();
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnNavigatedTo), 1, e);
                }

                try
                {
                    int iconsNum = User32Library.PrivateExtractIcons(DriveImagePath, 0, 0, 0, null, null, 0, 0);
                    nint[] phicon = new nint[iconsNum];
                    int[] piconid = new int[iconsNum];
                    int nIcons = User32Library.PrivateExtractIcons(DriveImagePath, 30, 256, 256, phicon, piconid, 1, 0);

                    Icon icon = Icon.FromHandle(phicon[0]);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    StandardDriveSource = bitmapImage;
                    icon.Dispose();
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnNavigatedTo), 2, e);
                }

                GlobalNotificationService.ApplicationExit += OnApplicationExit;
                winFRTimer.Elapsed += OnElapsed;
                await GetDriverInfoAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：文件恢复页面——挂载的事件

        /// <summary>
        /// 刷新磁盘数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetDriverInfoAsync();
        }

        /// <summary>
        /// 驱动器信息网格列表选中项发生改变时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is GridView gridView && gridView.SelectedItem is DriveModel driveItem)
            {
                SelectedItem = driveItem;
            }
        }

        /// <summary>
        /// 手动关闭信息栏提示
        /// </summary>
        private void OnCloseInfoBarClicked(InfoBar sender, object args)
        {
            IsManualCloseInfoBar = true;
        }

        /// <summary>
        /// 安装 Windows 文件恢复
        /// </summary>
        private void OnInstallWinFRClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://apps.microsoft.com/detail/9N26S50LN705");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnInstallWinFRClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 常规模式恢复文件内容
        /// </summary>
        private void OnRegularRestoreContentTextChanged(object sender, RoutedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                RegularRestoreContent = textBox.Text;
            }
        }

        /// <summary>
        /// 广泛模式恢复文件内容
        /// </summary>
        private void OnExtensiveRestoreContentTextChanged(object sender, RoutedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                ExtensiveRestoreContent = textBox.Text;
            }
        }

        /// <summary>
        /// NTFS 模式恢复文件内容
        /// </summary>
        private void OnNTFSRestoreContentTextChanged(object sender, RoutedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                NTFSRestoreContent = textBox.Text;
            }
        }

        /// <summary>
        /// 段模式恢复文件内容
        /// </summary>
        private void OnSegmentRestoreContentTextChanged(object sender, RoutedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                SegmentRestoreContent = textBox.Text;
            }
        }

        /// <summary>
        /// 开始恢复
        /// </summary>
        private async void OnRecoveryClicked(SplitButton sender, SplitButtonClickEventArgs args)
        {
            bool checkState = await CheckWinFRStateAsync(true);

            if (checkState)
            {
                string winFRCommand = await GetWinFRCommandAsync(true);

                try
                {
                    isRecovering = true;
                    progressDialog = (IProgressDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ProgressDialog));

                    if (progressDialog is not null)
                    {
                        // 准备扫描
                        progressDialog.SetTitle(PrepareScanString);
                        progressDialog.SetLine(1, PrepareScanString, false, IntPtr.Zero);
                        progressDialog.StartProgressDialog((IntPtr)MainWindow.Current.AppWindow.Id.Value, null, PROGDLG.PROGDLG_MODAL | PROGDLG.PROGDLG_NOMINIMIZE, IntPtr.Zero);

                        await Task.Run(() =>
                        {
                            try
                            {
                                if (!winFRTimer.Enabled)
                                {
                                    winFRTimer.Start();
                                }

                                if (winFRProcess is null)
                                {
                                    winFRProcess = new()
                                    {
                                        StartInfo =
                                        {
                                            FileName = "cmd.exe",
                                            Arguments = string.Format(@"/C chcp 65001>nul && {0}",winFRCommand),
                                            RedirectStandardInput = true,
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true,
                                            UseShellExecute = false,
                                            CreateNoWindow = true,
                                            StandardErrorEncoding = Encoding.Unicode,
                                            StandardOutputEncoding = Encoding.Unicode
                                        }
                                    };

                                    winFRProcess.Start();
                                    StreamReader outputReader = winFRProcess.StandardOutput;
                                    char[] buffer = new char[1024];
                                    bool scanningSection = false;
                                    bool recoverSection = false;

                                    while (!winFRProcess.HasExited)
                                    {
                                        int charLength = outputReader.Read(buffer, 0, buffer.Length);
                                        if (charLength > 0)
                                        {
                                            string content = new(buffer, 0, charLength);

                                            if (!string.IsNullOrEmpty(content))
                                            {
                                                content = content.Trim(trimCharsArray);

                                                // 第一阶段
                                                if (content.Contains("Pass 1"))
                                                {
                                                    scanningSection = true;
                                                    recoverSection = false;
                                                }

                                                // 进入第一阶段（扫描阶段）
                                                if (scanningSection)
                                                {
                                                    if (scanRegex.Matches(content) is MatchCollection scanCollection && scanCollection.Count > 0 && scanCollection[scanCollection.Count - 1].Groups is GroupCollection groupCollection && groupCollection.Count is 2)
                                                    {
                                                        uint scanPercentage = Convert.ToUInt32(groupCollection[1].Value);

                                                        synchronizationContext.Post((_) =>
                                                        {
                                                            if (progressDialog is not null && !progressDialog.HasUserCanceled())
                                                            {
                                                                progressDialog.SetTitle(ScanDeletedFileString);
                                                                progressDialog.SetLine(1, ScanDeletedFileString, false, IntPtr.Zero);
                                                                progressDialog.SetLine(2, string.Format(ScanningDeletedFileString, scanPercentage), false, IntPtr.Zero);
                                                                progressDialog.SetProgress(scanPercentage, 200);
                                                            }
                                                        }, null);
                                                    }
                                                }

                                                if (content.Contains("Pass 2"))
                                                {
                                                    scanningSection = false;
                                                    recoverSection = true;
                                                }

                                                // 进入第二阶段（恢复阶段）
                                                if (recoverSection)
                                                {
                                                    if (recoverRegex.Matches(content) is MatchCollection recoverCollection && recoverCollection.Count > 0 && recoverCollection[recoverCollection.Count - 1].Groups is GroupCollection groupCollection && groupCollection.Count is 4)
                                                    {
                                                        uint currentItemIndex = Convert.ToUInt32(groupCollection[1].Value);
                                                        uint totalItemIndex = Convert.ToUInt32(groupCollection[2].Value);
                                                        string fileName = groupCollection[3].Value;
                                                        uint finishedPercentage = Convert.ToUInt32(currentItemIndex * 100 / totalItemIndex);

                                                        synchronizationContext.Post((_) =>
                                                        {
                                                            if (progressDialog is not null && !progressDialog.HasUserCanceled())
                                                            {
                                                                progressDialog.SetTitle(RecoverDeletedFileString);
                                                                progressDialog.SetLine(1, RecoverDeletedFileString, false, IntPtr.Zero);
                                                                progressDialog.SetLine(2, string.Format(RecoveringDeletedFileString, finishedPercentage), false, IntPtr.Zero);
                                                                progressDialog.SetLine(3, string.Format(RecoverFileString, currentItemIndex, totalItemIndex, fileName), false, IntPtr.Zero);
                                                                progressDialog.SetProgress(finishedPercentage + 100, 200);
                                                            }
                                                        }, null);
                                                    }
                                                }

                                                // 恢复完成，查看文件目录
                                                if (content.Contains("Progress: 100%"))
                                                {
                                                    synchronizationContext.Post((_) =>
                                                    {
                                                        if (progressDialog is not null && !progressDialog.HasUserCanceled())
                                                        {
                                                            progressDialog.StopProgressDialog();
                                                            Marshal.ReleaseComObject(progressDialog);
                                                            progressDialog = null;
                                                        }
                                                    }, null);

                                                    if (winFRTimer.Enabled)
                                                    {
                                                        winFRTimer.Stop();
                                                    }
                                                    Process.Start(SaveFolder);
                                                }
                                            }
                                        }
                                    }

                                    // 防止意外发生
                                    synchronizationContext.Post((_) =>
                                    {
                                        try
                                        {
                                            if (progressDialog is not null)
                                            {
                                                progressDialog.StopProgressDialog();
                                                Marshal.ReleaseComObject(progressDialog);
                                                progressDialog = null;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnRecoveryClicked), 1, e);
                                        }
                                    }, null);

                                    if (winFRTimer.Enabled)
                                    {
                                        winFRTimer.Stop();
                                    }

                                    if (winFRProcess is not null)
                                    {
                                        winFRProcess.Dispose();
                                        winFRProcess = null;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnRecoveryClicked), 2, e);

                                synchronizationContext.Post((_) =>
                                {
                                    try
                                    {
                                        if (progressDialog is not null)
                                        {
                                            progressDialog.StopProgressDialog();
                                            Marshal.ReleaseComObject(progressDialog);
                                            progressDialog = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }, null);
                                if (winFRTimer.Enabled)
                                {
                                    winFRTimer.Stop();
                                }
                                winFRProcess = null;
                            }
                        });

                        isRecovering = false;
                    }
                }
                catch (Exception e)
                {
                    isRecovering = false;
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnRecoveryClicked), 3, e);
                    try
                    {
                        if (progressDialog is not null)
                        {
                            progressDialog.StopProgressDialog();
                            Marshal.ReleaseComObject(progressDialog);
                            progressDialog = null;
                        }
                    }
                    catch (Exception)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnRecoveryClicked), 4, e);
                    }
                }
            }
        }

        /// <summary>
        /// 复制恢复命令
        /// </summary>
        private async void OnCopyWinFRCommandClicked(object sender, RoutedEventArgs args)
        {
            bool checkState = await CheckWinFRStateAsync(false);

            if (checkState)
            {
                string winFRCommand = await GetWinFRCommandAsync(false);
                bool copyResult = CopyPasteHelper.CopyToClipboard(winFRCommand);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 了解文件恢复
        /// </summary>
        private void OnLearnWinFRClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://aka.ms/winfrhelp");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnLearnWinFRClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 选择保存目录
        /// </summary>
        private void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                SaveFolder = openFolderDialog.SelectedPath;
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        private void OnOpenSaveFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(LogSaveFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnOpenSaveFolderClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 选择日志目录
        /// </summary>
        private void OnLogSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                LogSaveFolder = openFolderDialog.SelectedPath;
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 修改恢复模式
        /// </summary>
        private void OnRecoveryModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> recoveryMode)
            {
                SelectedRecoveryMode = recoveryMode;
            }
        }

        /// <summary>
        /// 使用自定义日志文件目录
        /// </summary>
        private void OnUseCustomLogFolderToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                UseCustomLogFolder = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 重复文件选项
        /// </summary>
        private void OnRegularDuplicatedFileOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> ntfsDuplicatedFileOption)
            {
                SelectedRegularDuplicatedFileOption = ntfsDuplicatedFileOption;
            }
        }

        /// <summary>
        /// 重复文件选项
        /// </summary>
        private void OnExtensiveDuplicatedFileOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> ntfsDuplicatedFileOption)
            {
                SelectedExtensiveDuplicatedFileOption = ntfsDuplicatedFileOption;
            }
        }

        /// <summary>
        /// 是否从回收站中恢复未删除的文件
        /// </summary>
        private void OnNTFSRestoreFromRecyclebinToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                NTFSRestoreFromRecyclebin = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 恢复系统文件
        /// </summary>
        private void OnNTFSRestoreSystemFileToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                NTFSRestoreSystemFile = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 重复文件选项
        /// </summary>
        private void OnNTFSDuplicatedFileOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> regularDuplicatedFileOption)
            {
                SelectedNTFSDuplicatedFileOption = regularDuplicatedFileOption;
            }
        }

        /// <summary>
        /// 恢复没有主数据流的文件
        /// </summary>
        private void OnNTFSRestoreNonMainDataStreamToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                NTFSRestoreNonMainDataStream = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 自定义文件筛选类型
        /// </summary>
        private void OnNTFSUseCustomFileFilterTypeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                NTFSUseCustomFileFilterType = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 文件筛选类型
        /// </summary>
        private void OnNTFSCustomFileFilterTypeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                NTFSCustomFileFilterType = textBox.Text;
            }
        }

        /// <summary>
        /// 是否从回收站中恢复未删除的文件
        /// </summary>
        private void OnSegmentRestoreFromRecyclebinToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SegmentRestoreFromRecyclebin = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 恢复系统文件
        /// </summary>
        private void OnSegmentRestoreSystemFileToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SegmentRestoreSystemFile = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 重复文件选项
        /// </summary>
        private void OnSegmentDuplicatedFileOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> ntfsDuplicatedFileOption)
            {
                SelectedSegmentDuplicatedFileOption = ntfsDuplicatedFileOption;
            }
        }

        /// <summary>
        /// 恢复没有主数据流的文件
        /// </summary>
        private void OnSegmentRestoreNonMainDataStreamToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SegmentRestoreNonMainDataStream = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 自定义文件筛选类型
        /// </summary>
        private void OnSegmentUseCustomFileFilterTypeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SegmentUseCustomFileFilterType = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 文件筛选类型
        /// </summary>
        private void OnSegmentCustomFileFilterTypeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                SegmentCustomFileFilterType = textBox.Text;
            }
        }

        /// <summary>
        /// 修改源设备扇区数
        /// </summary>
        private void OnSegmentSourceDeviceNumberSectorsValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN)
            {
                try
                {
                    SegmentSourceDeviceNumberSectors = Math.Abs(Convert.ToInt32(args.NewValue));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSegmentSourceDeviceNumberSectorsValueChanged), 1, e);
                    SegmentSourceDeviceNumberSectors = 0;
                }
            }
            else
            {
                SegmentSourceDeviceNumberSectors = 0;
            }
        }

        /// <summary>
        /// 修改源设备群集大小
        /// </summary>
        private void OnSegmentSourceDeviceClusterSizeValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN)
            {
                try
                {
                    SegmentSourceDeviceClusterSize = Math.Abs(Convert.ToInt32(args.NewValue));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSegmentSourceDeviceClusterSizeValueChanged), 1, e);
                    SegmentSourceDeviceClusterSize = 0;
                }
            }
            else
            {
                SegmentSourceDeviceClusterSize = 0;
            }
        }

        /// <summary>
        /// 恢复特定扩展组
        /// </summary>
        private void OnSignatureUseRestoreSpecificExtensionGroupsToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SignatureUseRestoreSpecificExtensionGroups = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 修改特定扩展组类型
        /// </summary>
        private void OnSignatureRestoreSpecificExtensionGroupsTypeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                SignatureRestoreSpecificExtensionGroupsType = textBox.Text;
            }
        }

        /// <summary>
        /// 修改源设备扇区数
        /// </summary>
        private void OnSignatureSourceDeviceNumberSectorsValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN)
            {
                try
                {
                    SignatureSourceDeviceNumberSectors = Math.Abs(Convert.ToInt32(args.NewValue));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSignatureSourceDeviceNumberSectorsValueChanged), 1, e);
                    SignatureSourceDeviceNumberSectors = 0;
                }
            }
            else
            {
                SignatureSourceDeviceNumberSectors = 0;
            }
        }

        /// <summary>
        /// 修改源设备群集大小
        /// </summary>
        private void OnSignatureSourceDeviceClusterSizeValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN)
            {
                try
                {
                    SignatureSourceDeviceClusterSize = Math.Abs(Convert.ToInt32(args.NewValue));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSignatureSourceDeviceClusterSizeValueChanged), 1, e);
                    SignatureSourceDeviceClusterSize = 0;
                }
            }
            else
            {
                SignatureSourceDeviceClusterSize = 0;
            }
        }

        #endregion 第二部分：文件恢复页面——挂载的事件

        #region 第三部分：文件恢复页面——自定义事件

        /// <summary>
        /// 应用程序即将关闭时发生的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                winFRTimer.Elapsed -= OnElapsed;
                winFRTimer.Dispose();
                winFRTimer = null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 达到时间间隔触发的事件
        /// </summary>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            synchronizationContext.Post((_) =>
            {
                try
                {
                    // 用户手动取消操作
                    if (progressDialog is not null && progressDialog.HasUserCanceled())
                    {
                        progressDialog.StopProgressDialog();
                        Marshal.ReleaseComObject(progressDialog);
                        progressDialog = null;
                        if (winFRTimer.Enabled)
                        {
                            winFRTimer.Stop();
                        }

                        if (winFRProcess is not null)
                        {
                            if (!winFRProcess.HasExited)
                            {
                                KillProcessAndChildren(winFRProcess.Id);
                            }

                            winFRProcess.Dispose();
                            winFRProcess = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnElapsed), 1, e);
                }
            }, null);
        }

        #endregion 第三部分：文件恢复页面——自定义事件

        /// <summary>
        /// 获取驱动器信息
        /// </summary>
        private async Task GetDriverInfoAsync()
        {
            IsDriveLoadCompleted = false;

            List<DriveModel> driveList = await Task.Run(() =>
            {
                List<DriveModel> driveList = [];
                DriveInfo[] driverInfoArray = DriveInfo.GetDrives();

                foreach (DriveInfo driveInfo in driverInfoArray)
                {
                    double driveUsedPercentage = ((driveInfo.TotalSize - driveInfo.TotalFreeSpace) * 100 / driveInfo.TotalSize);

                    DriveModel driveItem = new()
                    {
                        IsSelected = false,
                        Name = string.Format("{0} ({1})", driveInfo.VolumeLabel, driveInfo.Name.TrimEnd('\\')),
                        Space = string.Format(DiskSpaceString, VolumeSizeHelper.ConvertVolumeSizeToString(driveInfo.TotalFreeSpace), VolumeSizeHelper.ConvertVolumeSizeToString(driveInfo.TotalSize)),
                        DriveUsedPercentage = driveUsedPercentage,
                        IsAvailableSpaceWarning = driveUsedPercentage > 90,
                        IsAvailableSpaceError = driveUsedPercentage > 95,
                        DriveInfo = driveInfo
                    };

                    bool isSystemDrive = string.Equals(driveInfo.RootDirectory.FullName, Path.GetPathRoot(Environment.SystemDirectory));

                    if (isSystemDrive)
                    {
                        driveItem.DiskImage = SystemDriveSource;
                        driveItem.IsSytemDrive = true;
                    }
                    else
                    {
                        driveItem.DiskImage = StandardDriveSource;
                        driveItem.IsSytemDrive = false;
                    }

                    driveList.Add(driveItem);
                }

                return driveList;
            });

            SelectedItem = null;
            DriveCollection.Clear();
            foreach (DriveModel driveItem in driveList)
            {
                DriveCollection.Add(driveItem);
            }

            IsDriveLoadCompleted = true;
        }

        /// <summary>
        /// 检查 WinFR 状态
        /// </summary>
        private async Task<bool> CheckWinFRStateAsync(bool isExecute)
        {
            if (isExecute)
            {
                if (isRecovering)
                {
                    return false;
                }

                IsWinFRInstalled = await GetWinFRInstalledAsync();

                if (isExecute && !IsWinFRInstalled)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinFRNotInstalled));
                    return false;
                }
            }

            if (SelectedItem is null)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DriveEmpty));
                return false;
            }

            if (string.IsNullOrEmpty(SaveFolder))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectFolderEmpty));
                return false;
            }

            if (string.Equals(Path.GetPathRoot(SaveFolder), SelectedItem.DriveInfo.RootDirectory.FullName, StringComparison.OrdinalIgnoreCase))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SameDriveAndSelectFolder));
                return false;
            }

            if (UseCustomLogFolder && string.IsNullOrEmpty(LogSaveFolder))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectLogFolderEmpty));
                return false;
            }

            if (Equals(SelectedRecoveryMode, RecoveryModeList[0]) && string.IsNullOrEmpty(RegularRestoreContent))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.RestoreContentEmpty));
                return false;
            }

            if (Equals(SelectedRecoveryMode, RecoveryModeList[1]) && string.IsNullOrEmpty(ExtensiveRestoreContent))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.RestoreContentEmpty));
                return false;
            }

            if (Equals(SelectedRecoveryMode, RecoveryModeList[2]) && string.IsNullOrEmpty(NTFSRestoreContent))
            {
                if (string.IsNullOrEmpty(NTFSRestoreContent))
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.RestoreContentEmpty));
                    return false;
                }

                if (NTFSUseCustomFileFilterType && string.IsNullOrEmpty(NTFSCustomFileFilterType))
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CustomFileFilterTypeEmpty));
                    return false;
                }
            }

            if (Equals(SelectedRecoveryMode, RecoveryModeList[3]))
            {
                if (string.IsNullOrEmpty(SegmentRestoreContent))
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.RestoreContentEmpty));
                    return false;
                }

                if (SegmentUseCustomFileFilterType && string.IsNullOrEmpty(SegmentCustomFileFilterType))
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CustomFileFilterTypeEmpty));
                    return false;
                }
            }

            if (Equals(SelectedRecoveryMode, RecoveryModeList[4]) && SignatureUseRestoreSpecificExtensionGroups && string.IsNullOrEmpty(SignatureRestoreSpecificExtensionGroupsType))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.RestoreSpecificExtensionGroupsTypeEmpty));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取 Windows 文件恢复安装状态
        /// </summary>
        private async Task<bool> GetWinFRInstalledAsync()
        {
            return await Task.Run(() =>
            {
                List<INET_FIREWALL_APP_CONTAINER> inetLoopbackList = GetAppContainerList();

                foreach (INET_FIREWALL_APP_CONTAINER inetContainerItem in inetLoopbackList)
                {
                    if (!string.IsNullOrEmpty(inetContainerItem.packageFullName) && string.Equals(inetContainerItem.packageFullName, "Microsoft.WindowsFileRecovery_8wekyb3d8bbwe", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// 获取 WinFR 命令
        /// </summary>
        private async Task<string> GetWinFRCommandAsync(bool isExecute)
        {
            return await Task.Run(() =>
            {
                StringBuilder winFRCommandBuilder = new("WinFR.exe");
                winFRCommandBuilder.Append(' ');

                // 常规模式
                if (Equals(SelectedRecoveryMode, RecoveryModeList[0]))
                {
                    winFRCommandBuilder.Append("/regular");
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SelectedItem.DriveInfo.Name.TrimEnd('\\'));
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SaveFolder);
                    winFRCommandBuilder.Append(' ');

                    if (UseCustomLogFolder)
                    {
                        winFRCommandBuilder.Append("/p:");
                        winFRCommandBuilder.Append(LogSaveFolder);
                        winFRCommandBuilder.Append(' ');
                    }

                    string[] restoreContentArray = RegularRestoreContent.Split(';');
                    foreach (string restoreContent in restoreContentArray)
                    {
                        winFRCommandBuilder.Append("/n ");
                        winFRCommandBuilder.Append(restoreContent);
                        winFRCommandBuilder.Append(' ');
                    }

                    if (Equals(SelectedRegularDuplicatedFileOption, RegularDuplicatedFileOptionList[0]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('a');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedRegularDuplicatedFileOption, RegularDuplicatedFileOptionList[1]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('n');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedRegularDuplicatedFileOption, RegularDuplicatedFileOptionList[2]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('b');
                        winFRCommandBuilder.Append(' ');
                    }
                }
                // 广泛模式
                else if (Equals(SelectedRecoveryMode, RecoveryModeList[1]))
                {
                    winFRCommandBuilder.Append("/extensive");
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SelectedItem.DriveInfo.Name.TrimEnd('\\'));
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SaveFolder);
                    winFRCommandBuilder.Append(' ');

                    if (UseCustomLogFolder)
                    {
                        winFRCommandBuilder.Append("/p:");
                        winFRCommandBuilder.Append(LogSaveFolder);
                        winFRCommandBuilder.Append(' ');
                    }

                    string[] restoreContentArray = ExtensiveRestoreContent.Split(';');
                    foreach (string restoreContent in restoreContentArray)
                    {
                        winFRCommandBuilder.Append("/n ");
                        winFRCommandBuilder.Append(restoreContent);
                        winFRCommandBuilder.Append(' ');
                    }

                    if (Equals(SelectedExtensiveDuplicatedFileOption, ExtensiveDuplicatedFileOptionList[0]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('a');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedExtensiveDuplicatedFileOption, ExtensiveDuplicatedFileOptionList[1]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('n');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedExtensiveDuplicatedFileOption, ExtensiveDuplicatedFileOptionList[2]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('b');
                        winFRCommandBuilder.Append(' ');
                    }
                }
                // NTFS 模式
                else if (Equals(SelectedRecoveryMode, RecoveryModeList[2]))
                {
                    winFRCommandBuilder.Append("/ntfs");
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SelectedItem.DriveInfo.Name.TrimEnd('\\'));
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SaveFolder);
                    winFRCommandBuilder.Append(' ');

                    if (UseCustomLogFolder)
                    {
                        winFRCommandBuilder.Append("/p:");
                        winFRCommandBuilder.Append(LogSaveFolder);
                        winFRCommandBuilder.Append(' ');
                    }

                    string[] restoreContentArray = NTFSRestoreContent.Split(';');
                    foreach (string restoreContent in restoreContentArray)
                    {
                        winFRCommandBuilder.Append("/n ");
                        winFRCommandBuilder.Append(restoreContent);
                        winFRCommandBuilder.Append(' ');
                    }

                    if (NTFSRestoreFromRecyclebin)
                    {
                        winFRCommandBuilder.Append("/u");
                        winFRCommandBuilder.Append(' ');
                    }

                    if (NTFSRestoreSystemFile)
                    {
                        winFRCommandBuilder.Append("/k");
                        winFRCommandBuilder.Append(' ');
                    }

                    if (Equals(SelectedNTFSDuplicatedFileOption, NTFSDuplicatedFileOptionList[0]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('a');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedNTFSDuplicatedFileOption, NTFSDuplicatedFileOptionList[1]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('n');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedNTFSDuplicatedFileOption, NTFSDuplicatedFileOptionList[2]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('b');
                        winFRCommandBuilder.Append(' ');
                    }

                    if (NTFSRestoreNonMainDataStream)
                    {
                        winFRCommandBuilder.Append("/g");
                        winFRCommandBuilder.Append(' ');
                    }

                    if (NTFSUseCustomFileFilterType)
                    {
                        winFRCommandBuilder.Append("/e:");
                        string[] ntfsCustomFileFilterTypeArray = NTFSCustomFileFilterType.Split(';');
                        for (int index = 0; index < ntfsCustomFileFilterTypeArray.Length; index++)
                        {
                            string ntfsCustomFileFilterType = ntfsCustomFileFilterTypeArray[index];
                            winFRCommandBuilder.Append(ntfsCustomFileFilterType);
                            if (index < ntfsCustomFileFilterTypeArray.Length - 1)
                            {
                                winFRCommandBuilder.Append(';');
                            }
                        }
                    }
                }
                // 段模式
                else if (Equals(SelectedRecoveryMode, RecoveryModeList[3]))
                {
                    winFRCommandBuilder.Append("/segment");
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SelectedItem.DriveInfo.Name.TrimEnd('\\'));
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SaveFolder);
                    winFRCommandBuilder.Append(' ');

                    if (UseCustomLogFolder)
                    {
                        winFRCommandBuilder.Append("/p:");
                        winFRCommandBuilder.Append(LogSaveFolder);
                        winFRCommandBuilder.Append(' ');
                    }

                    string[] restoreContentArray = SegmentRestoreContent.Split(';');
                    foreach (string restoreContent in restoreContentArray)
                    {
                        winFRCommandBuilder.Append("/n ");
                        winFRCommandBuilder.Append(restoreContent);
                        winFRCommandBuilder.Append(' ');
                    }

                    if (NTFSRestoreFromRecyclebin)
                    {
                        winFRCommandBuilder.Append("/u");
                        winFRCommandBuilder.Append(' ');
                    }

                    if (NTFSRestoreSystemFile)
                    {
                        winFRCommandBuilder.Append("/k");
                        winFRCommandBuilder.Append(' ');
                    }

                    if (Equals(SelectedSegmentDuplicatedFileOption, SegmentDuplicatedFileOptionList[0]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('a');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedSegmentDuplicatedFileOption, SegmentDuplicatedFileOptionList[1]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('n');
                        winFRCommandBuilder.Append(' ');
                    }
                    else if (Equals(SelectedSegmentDuplicatedFileOption, SegmentDuplicatedFileOptionList[2]))
                    {
                        winFRCommandBuilder.Append("/o:");
                        winFRCommandBuilder.Append('b');
                        winFRCommandBuilder.Append(' ');
                    }

                    if (SegmentRestoreNonMainDataStream)
                    {
                        winFRCommandBuilder.Append("/g");
                        winFRCommandBuilder.Append(' ');
                    }

                    if (SegmentUseCustomFileFilterType)
                    {
                        winFRCommandBuilder.Append("/e:");
                        string[] segmentCustomFileFilterTypeArray = SegmentCustomFileFilterType.Split(';');
                        for (int index = 0; index < segmentCustomFileFilterTypeArray.Length; index++)
                        {
                            string segmentCustomFileFilterType = segmentCustomFileFilterTypeArray[index];
                            winFRCommandBuilder.Append(segmentCustomFileFilterType);
                            if (index < segmentCustomFileFilterTypeArray.Length - 1)
                            {
                                winFRCommandBuilder.Append(';');
                            }
                        }
                        winFRCommandBuilder.Append(' ');
                    }

                    if (!Equals(SegmentSourceDeviceNumberSectors, 0))
                    {
                        winFRCommandBuilder.Append(string.Format("/s:{0}", SegmentSourceDeviceNumberSectors));
                        winFRCommandBuilder.Append(' ');
                    }

                    if (!Equals(SegmentSourceDeviceClusterSize, 0))
                    {
                        winFRCommandBuilder.Append(string.Format("/b:{0}", SegmentSourceDeviceClusterSize));
                        winFRCommandBuilder.Append(' ');
                    }
                }
                // 签名模式
                else if (Equals(SelectedRecoveryMode, RecoveryModeList[4]))
                {
                    winFRCommandBuilder.Append("/signature");
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SelectedItem.DriveInfo.Name.TrimEnd('\\'));
                    winFRCommandBuilder.Append(' ');
                    winFRCommandBuilder.Append(SaveFolder);
                    winFRCommandBuilder.Append(' ');

                    if (UseCustomLogFolder)
                    {
                        winFRCommandBuilder.Append("/p:");
                        winFRCommandBuilder.Append(LogSaveFolder);
                        winFRCommandBuilder.Append(' ');
                    }

                    if (SignatureUseRestoreSpecificExtensionGroups)
                    {
                        winFRCommandBuilder.Append("/y:");
                        string[] signatureRestoreSpecificExtensionGroupsTypArray = SignatureRestoreSpecificExtensionGroupsType.Split(';');
                        for (int index = 0; index < signatureRestoreSpecificExtensionGroupsTypArray.Length; index++)
                        {
                            string signatureRestoreSpecificExtensionGroupsType = signatureRestoreSpecificExtensionGroupsTypArray[index];
                            winFRCommandBuilder.Append(signatureRestoreSpecificExtensionGroupsType);
                            if (index < signatureRestoreSpecificExtensionGroupsTypArray.Length - 1)
                            {
                                winFRCommandBuilder.Append(',');
                            }
                        }
                        winFRCommandBuilder.Append(' ');
                    }

                    if (!Equals(SignatureSourceDeviceNumberSectors, 0))
                    {
                        winFRCommandBuilder.Append(string.Format("/s:{0}", SignatureSourceDeviceNumberSectors));
                        winFRCommandBuilder.Append(' ');
                    }

                    if (!Equals(SignatureSourceDeviceClusterSize, 0))
                    {
                        winFRCommandBuilder.Append(string.Format("/b:{0}", SignatureSourceDeviceClusterSize));
                        winFRCommandBuilder.Append(' ');
                    }
                }

                if (isExecute)
                {
                    winFRCommandBuilder.Append("/a");
                }

                return winFRCommandBuilder.ToString();
            });
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
        /// 终止进程树
        /// </summary>
        private static void KillProcessAndChildren(int pid)
        {
            if (pid is not 0)
            {
                ManagementObjectSearcher managementObjectSearcher = new("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                foreach (ManagementObject managementObject in managementObjectCollection.Cast<ManagementObject>())
                {
                    KillProcessAndChildren(Convert.ToInt32(managementObject["ProcessID"]));
                }
                try
                {
                    Process process = Process.GetProcessById(pid);
                    process.Kill();
                    process.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(KillProcessAndChildren), 1, e);
                }
            }
        }

        /// <summary>
        /// 获取信息栏显示状态
        /// </summary>
        private Visibility GetInfoBarState(bool isManualCloseInfoBar, bool isWinFRInstalled)
        {
            return !isManualCloseInfoBar && !isWinFRInstalled ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取恢复模式
        /// </summary>
        private Visibility GetRecoveryMode(string recoveryMode, string comparedRecoveryMode)
        {
            return string.Equals(recoveryMode, comparedRecoveryMode) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
