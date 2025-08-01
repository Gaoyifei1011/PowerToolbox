﻿using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

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
        private readonly string RecentDeleteString = ResourceService.WinFRResource.GetString("RecentDelete");
        private readonly string RegularModeString = ResourceService.WinFRResource.GetString("RegularMode");
        private readonly string SegmentModeString = ResourceService.WinFRResource.GetString("SegmentMode");
        private readonly string SelectFolderString = ResourceService.WinFRResource.GetString("SelectFolder");
        private readonly string SignatureModeString = ResourceService.WinFRResource.GetString("SignatureMode");

        private ImageSource SystemDriveSource;
        private ImageSource StandardDriveSource;

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

        private string _restoreContent;

        public string RestoreContent
        {
            get { return _restoreContent; }

            set
            {
                if (!string.Equals(_restoreContent, value))
                {
                    _restoreContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RestoreContent)));
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

        private List<KeyValuePair<string, string>> NTFSDuplicatedFileOptionList { get; } = [];

        private List<KeyValuePair<string, string>> SegmentDuplicatedFileOptionList { get; } = [];

        public List<RecoveryModeSuggestionModel> RecoveryModeSuggestionList { get; } = [];

        public ObservableCollection<DriveModel> DriveCollection { get; } = [];

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

            if (!isInitialized)
            {
                isInitialized = true;

                try
                {
                    int iconsNum = User32Library.PrivateExtractIcons(DriveImagePath, 0, 0, 0, null, null, 0, 0);
                    IntPtr[] phicon = new IntPtr[iconsNum];
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnNavigatedTo), 1, e);
                }

                try
                {
                    int iconsNum = User32Library.PrivateExtractIcons(DriveImagePath, 0, 0, 0, null, null, 0, 0);
                    IntPtr[] phicon = new IntPtr[iconsNum];
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnNavigatedTo), 1, e);
                }

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
        /// 恢复文件内容
        /// </summary>
        private void OnRestoreContentTextChanged(object sender, RoutedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
            {
                RestoreContent = textBox.Text;
            }
        }

        /// <summary>
        /// 开始恢复
        /// </summary>
        /// TODO：未完成
        private void OnRecoveryClicked(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
        }

        /// <summary>
        /// 复制恢复命令
        /// </summary>
        /// TODO：未完成
        private void OnCopyWinFRCommandClicked(object sender, RoutedEventArgs args)
        {
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnLearnWinFRClicked), 1, e);
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
                    Process.Start(SaveFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnLearnWinFRClicked), 1, e);
                }
            });
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
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> ntfsDuplicatedFileOption)
            {
                SelectedNTFSDuplicatedFileOption = ntfsDuplicatedFileOption;
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
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
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
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSegmentSourceDeviceNumberSectorsValueChanged), 1, e);
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSegmentSourceDeviceClusterSizeValueChanged), 1, e);
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
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox)
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSignatureSourceDeviceNumberSectorsValueChanged), 1, e);
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(WinFRPage), nameof(OnSignatureSourceDeviceClusterSizeValueChanged), 1, e);
                    SignatureSourceDeviceClusterSize = 0;
                }
            }
            else
            {
                SignatureSourceDeviceClusterSize = 0;
            }
        }

        #endregion 第二部分：文件恢复页面——挂载的事件

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
                    double driveUsedPercentage = (driveInfo.TotalFreeSpace * 100 / driveInfo.TotalSize);

                    DriveModel driveItem = new()
                    {
                        IsSelected = false,
                        Name = string.Format("{0} ({1})", driveInfo.VolumeLabel, driveInfo.Name.TrimEnd('\\')),
                        Space = string.Format(DiskSpaceString, VolumeSizeHelper.ConvertVolumeSizeToString(driveInfo.TotalFreeSpace), VolumeSizeHelper.ConvertVolumeSizeToString(driveInfo.TotalSize)),
                        DriveUsedPercentage = driveUsedPercentage,
                        IsAvailableSpaceWarning = driveUsedPercentage > 90,
                        IsAvailableSpaceError = driveUsedPercentage > 95,
                        DriverInfo = driveInfo
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
        /// 获取恢复模式
        /// </summary>
        private Visibility GetRecoveryMode(string recoveryMode, string comparedRecoveryMode)
        {
            return string.Equals(recoveryMode, comparedRecoveryMode) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
