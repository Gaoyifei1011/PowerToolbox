using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 提取图标页面
    /// </summary>
    public sealed partial class IconExtractPage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.IconExtractResource.GetString("DragOverContent");
        private readonly string FileAssociatedString = ResourceService.IconExtractResource.GetString("FileAssociated");
        private readonly string FileAssociatedFilterConditionString = ResourceService.IconExtractResource.GetString("FileAssociatedFilterCondition");
        private readonly string FileContainedString = ResourceService.IconExtractResource.GetString("FileContained");
        private readonly string FileContainedFilterConditionString = ResourceService.IconExtractResource.GetString("FileContainedFilterCondition");
        private readonly string GetFileAssociatedResultsFailedString = ResourceService.IconExtractResource.GetString("GetFileAssociatedResultsFailed");
        private readonly string GetFileAssociatedResultsSuccessfullyString = ResourceService.IconExtractResource.GetString("GetFileAssociatedResultsSuccessfully");
        private readonly string GetFileContainedResultsString = ResourceService.IconExtractResource.GetString("GetFileContainedResults");
        private readonly string NoOtherExtensionNameFileString = ResourceService.IconExtractResource.GetString("NoOtherExtensionNameFile");
        private readonly string NoMultiFileString = ResourceService.IconExtractResource.GetString("NoMultiFile");
        private readonly string NoResourcesString = ResourceService.IconExtractResource.GetString("NoResources");
        private readonly string NoSelectedFileString = ResourceService.IconExtractResource.GetString("NoSelectedFile");
        private readonly string NotSupportedString = ResourceService.IconExtractResource.GetString("NotSupported");
        private readonly string ParsingIconString = ResourceService.IconExtractResource.GetString("ParsingIcon");
        private readonly string SelectFileString = ResourceService.IconExtractResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.IconExtractResource.GetString("SelectFolder");
        private readonly string SavingNowString = ResourceService.IconExtractResource.GetString("SavingNow");
        private string filePath;

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        private IconExtractResultKind _iconExtractResultKind = IconExtractResultKind.Welcome;

        public IconExtractResultKind IconExtractResultKind
        {
            get { return _iconExtractResultKind; }

            set
            {
                if (!Equals(_iconExtractResultKind, value))
                {
                    _iconExtractResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconExtractResultKind)));
                }
            }
        }

        private bool _isImageEmpty;

        public bool IsImageEmpty
        {
            get { return _isImageEmpty; }

            set
            {
                if (!Equals(_isImageEmpty, value))
                {
                    _isImageEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsImageEmpty)));
                }
            }
        }

        private bool _isSaving;

        public bool IsSaving
        {
            get { return _isSaving; }

            set
            {
                if (!Equals(_isSaving, value))
                {
                    _isSaving = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaving)));
                }
            }
        }

        private string _getResults;

        public string GetResults
        {
            get { return _getResults; }

            set
            {
                if (!string.Equals(_getResults, value))
                {
                    _getResults = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GetResults)));
                }
            }
        }

        private string _noResources;

        public string NoResources
        {
            get { return _noResources; }

            set
            {
                if (!string.Equals(_noResources, value))
                {
                    _noResources = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoResources)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedGetIconType;

        public KeyValuePair<string, string> SelectedGetIconType
        {
            get { return _selectedGetIconType; }

            set
            {
                if (!Equals(_selectedGetIconType, value))
                {
                    _selectedGetIconType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedGetIconType)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedIconFormat;

        public KeyValuePair<string, string> SelectedIconFormat
        {
            get { return _selectedIconFormat; }

            set
            {
                if (!Equals(_selectedIconFormat, value))
                {
                    _selectedIconFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIconFormat)));
                }
            }
        }

        private KeyValuePair<int, string> _selectedIconSize;

        public KeyValuePair<int, string> SelectedIconSize
        {
            get { return _selectedIconSize; }

            set
            {
                if (!Equals(_selectedIconSize, value))
                {
                    _selectedIconSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIconSize)));
                }
            }
        }

        private bool _is16SizeEnabled;

        public bool Is16SizeEnabled
        {
            get { return _is16SizeEnabled; }

            set
            {
                if (!Equals(_is16SizeEnabled, value))
                {
                    _is16SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is16SizeEnabled)));
                }
            }
        }

        private bool _is24SizeEnabled;

        public bool Is24SizeEnabled
        {
            get { return _is24SizeEnabled; }

            set
            {
                if (!Equals(_is24SizeEnabled, value))
                {
                    _is24SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is24SizeEnabled)));
                }
            }
        }

        private bool _is32SizeEnabled;

        public bool Is32SizeEnabled
        {
            get { return _is32SizeEnabled; }

            set
            {
                if (!Equals(_is32SizeEnabled, value))
                {
                    _is32SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is32SizeEnabled)));
                }
            }
        }

        private bool _is48SizeEnabled;

        public bool Is48SizeEnabled
        {
            get { return _is48SizeEnabled; }

            set
            {
                if (!Equals(_is48SizeEnabled, value))
                {
                    _is48SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is48SizeEnabled)));
                }
            }
        }

        private bool _is64SizeEnabled;

        public bool Is64SizeEnabled
        {
            get { return _is64SizeEnabled; }

            set
            {
                if (!Equals(_is64SizeEnabled, value))
                {
                    _is64SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is64SizeEnabled)));
                }
            }
        }

        private bool _is72SizeEnabled;

        public bool Is72SizeEnabled
        {
            get { return _is72SizeEnabled; }

            set
            {
                if (!Equals(_is72SizeEnabled, value))
                {
                    _is72SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is72SizeEnabled)));
                }
            }
        }

        private bool _is96SizeEnabled;

        public bool Is96SizeEnabled
        {
            get { return _is96SizeEnabled; }

            set
            {
                if (!Equals(_is96SizeEnabled, value))
                {
                    _is96SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is96SizeEnabled)));
                }
            }
        }

        private bool _is128SizeEnabled;

        public bool Is128SizeEnabled
        {
            get { return _is128SizeEnabled; }

            set
            {
                if (!Equals(_is128SizeEnabled, value))
                {
                    _is128SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is128SizeEnabled)));
                }
            }
        }

        private bool _is256SizeEnabled;

        public bool Is256SizeEnabled
        {
            get { return _is256SizeEnabled; }

            set
            {
                if (!Equals(_is256SizeEnabled, value))
                {
                    _is256SizeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is256SizeEnabled)));
                }
            }
        }

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get { return _imageSource; }

            set
            {
                if (!Equals(_imageSource, value))
                {
                    _imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
                }
            }
        }

        public List<KeyValuePair<string, string>> GetIconTypeList { get; } = [];

        private List<KeyValuePair<string, string>> IconFormatList { get; } =
        [
            new KeyValuePair<string,string>(".ico", ".ico" ),
            new KeyValuePair<string,string>(".png", ".png" )
        ];

        private List<KeyValuePair<int, string>> IconSizeList { get; set; } =
        [
            new KeyValuePair<int,string>(16, "16 * 16" ),
            new KeyValuePair<int,string>(24, "24 * 24" ),
            new KeyValuePair<int,string>(32, "32 * 32" ),
            new KeyValuePair<int,string>(48, "48 * 48" ),
            new KeyValuePair<int,string>(64, "64 * 64" ),
            new KeyValuePair<int,string>(72, "72 * 72" ),
            new KeyValuePair<int,string>(96, "96 * 96" ),
            new KeyValuePair<int,string>(128, "128 * 128" ),
            new KeyValuePair<int,string>(256, "256 * 256" )
        ];

        private WinRTObservableCollection<IconModel> IconCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public IconExtractPage()
        {
            InitializeComponent();
            GetIconTypeList.Add(new KeyValuePair<string, string>("FileContained", FileContainedString));
            GetIconTypeList.Add(new KeyValuePair<string, string>("FileAssociated", FileAssociatedString));
            SelectedGetIconType = GetIconTypeList[0];
            SelectedIconFormat = IconFormatList[1];
            SelectedIconSize = IconSizeList[8];
            IsSelected = false;
            IsImageEmpty = true;
            GetResults = NoSelectedFileString;
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override async void OnDragOver(Microsoft.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IconExtractResultKind is IconExtractResultKind.Parsing)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = ParsingIconString;
                }
                else
                {
                    if (IsSaving)
                    {
                        args.AcceptedOperation = DataPackageOperation.None;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = SavingNowString;
                    }
                    else
                    {
                        IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                        if (dragItemsList.Count is 1)
                        {
                            string extensionName = Path.GetExtension(dragItemsList[0].Name);

                            if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                            {
                                if (string.Equals(extensionName, ".exe", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".dll", StringComparison.OrdinalIgnoreCase))
                                {
                                    args.AcceptedOperation = DataPackageOperation.Copy;
                                    args.DragUIOverride.IsCaptionVisible = true;
                                    args.DragUIOverride.IsContentVisible = false;
                                    args.DragUIOverride.IsGlyphVisible = true;
                                    args.DragUIOverride.Caption = DragOverContentString;
                                }
                                else
                                {
                                    args.AcceptedOperation = DataPackageOperation.None;
                                    args.DragUIOverride.IsCaptionVisible = true;
                                    args.DragUIOverride.IsContentVisible = false;
                                    args.DragUIOverride.IsGlyphVisible = true;
                                    args.DragUIOverride.Caption = NoOtherExtensionNameFileString;
                                }
                            }
                            else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                            {
                                args.AcceptedOperation = DataPackageOperation.Copy;
                                args.DragUIOverride.IsCaptionVisible = true;
                                args.DragUIOverride.IsContentVisible = false;
                                args.DragUIOverride.IsGlyphVisible = true;
                                args.DragUIOverride.Caption = DragOverContentString;
                            }
                            else
                            {
                                args.AcceptedOperation = DataPackageOperation.None;
                                args.DragUIOverride.IsCaptionVisible = true;
                                args.DragUIOverride.IsContentVisible = false;
                                args.DragUIOverride.IsGlyphVisible = true;
                                args.DragUIOverride.Caption = NotSupportedString;
                            }
                        }
                        else
                        {
                            args.AcceptedOperation = DataPackageOperation.None;
                            args.DragUIOverride.IsCaptionVisible = true;
                            args.DragUIOverride.IsContentVisible = false;
                            args.DragUIOverride.IsGlyphVisible = true;
                            args.DragUIOverride.Caption = NoMultiFileString;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDragOver), 1, e);
            }
            finally
            {
                args.Handled = true;
                dragOperationDeferral.Complete();
            }
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(Microsoft.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            filePath = string.Empty;

            try
            {
                DataPackageView dataPackageView = args.DataView;
                IReadOnlyList<IStorageItem> filesList = await Task.Run(async () =>
                {
                    try
                    {
                        if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                        {
                            return await dataPackageView.GetStorageItemsAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 1, e);
                    }

                    return null;
                });

                if (filesList is not null && filesList.Count is 1)
                {
                    filePath = filesList[0].Path;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                {
                    await ParseIconFileAsync(filePath);
                }
                else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                {
                    await GetFileAssociatedIconAsync(filePath);
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：提取图标页面——挂载的事件

        /// <summary>
        /// 网格控件选中项发生改变时的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            IList<object> selectedItemsList = (sender as GridView).SelectedItems;
            if (selectedItemsList.Count > 0)
            {
                IsSelected = true;

                if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                {
                    int iconIndex = Convert.ToInt32((selectedItemsList.Last() as IconModel).DisplayIndex);

                    try
                    {
                        Icon icon = GetFixedSizeIcon(iconIndex, Convert.ToInt32(SelectedIconSize.Key));
                        MemoryStream memoryStream = new();
                        icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                        ImageSource = bitmapImage;
                        IsImageEmpty = false;
                        icon.Dispose();
                        memoryStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnSelectionChanged), 1, e);
                    }
                }
                else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                {
                    try
                    {
                        int size = Convert.ToInt32(SelectedIconSize.Key);
                        Icon icon = null;
                        if (size is 16)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_SMALL);
                        }
                        else if (size is 32)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_LARGE);
                        }
                        else if (size is 48)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE);
                        }
                        else if (size is 256)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_JUMBO);
                        }

                        if (icon is not null)
                        {
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            ImageSource = bitmapImage;
                            IsImageEmpty = false;
                            memoryStream.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnSelectionChanged), 2, e);
                    }
                }
            }
            else
            {
                IsSelected = false;
            }
        }

        /// <summary>
        /// 选择获取的图标类型
        /// </summary>
        private void OnGetIconTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> getIconType)
            {
                SelectedGetIconType = getIconType;
                IsSelected = false;
                IconExtractResultKind = IconExtractResultKind.Welcome;
                IsImageEmpty = true;
                GetResults = NoSelectedFileString;
                ImageSource = null;
                NoResources = string.Empty;
                SelectedIconSize = IconSizeList[8];
                IconCollection.Clear();
            }
        }

        /// <summary>
        /// 选择图标格式
        /// </summary>
        private void OnIconFormatClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> iconFormat)
            {
                SelectedIconFormat = iconFormat;
            }
        }

        /// <summary>
        /// 选择图标尺寸
        /// </summary>
        private void OnIconSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<int, string> iconSize)
            {
                SelectedIconSize = iconSize;

                if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                {
                    if (IconsGridView.SelectedItem is not null)
                    {
                        int iconIndex = Convert.ToInt32((IconsGridView.SelectedItem as IconModel).DisplayIndex);

                        try
                        {
                            Icon icon = GetFixedSizeIcon(iconIndex, Convert.ToInt32(SelectedIconSize.Key));
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            ImageSource = bitmapImage;
                            IsImageEmpty = false;
                            icon.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnIconSizeClicked), 1, e);
                        }
                    }
                }
                else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                {
                    try
                    {
                        int size = Convert.ToInt32(SelectedIconSize.Key);
                        Icon icon = null;
                        if (size is 16)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_SMALL);
                        }
                        else if (size is 32)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_LARGE);
                        }
                        else if (size is 48)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE);
                        }
                        else if (size is 256)
                        {
                            icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_JUMBO);
                        }

                        if (icon is not null)
                        {
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            ImageSource = bitmapImage;
                            IsImageEmpty = false;
                            memoryStream.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnIconSizeClicked), 2, e);
                    }
                }
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private async void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            string filterString = string.Empty;
            if (Equals(SelectedGetIconType, GetIconTypeList[0]))
            {
                filterString = FileContainedFilterConditionString;
            }
            else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
            {
                filterString = FileAssociatedFilterConditionString;
            }

            if (!string.IsNullOrEmpty(filterString))
            {
                OpenFileDialog openFileDialog = new()
                {
                    Multiselect = false,
                    Filter = filterString,
                    Title = SelectFileString
                };
                if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                    {
                        await ParseIconFileAsync(openFileDialog.FileName);
                    }
                    else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                    {
                        await GetFileAssociatedIconAsync(openFileDialog.FileName);
                    }
                }
                openFileDialog.Dispose();
            }
        }

        /// <summary>
        /// 导出选中的图标
        /// </summary>
        private async void OnExportSelectedIconsClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                IList<object> selectedItemsList = IconsGridView.SelectedItems;
                if (Equals(SelectedIconFormat, IconFormatList[0]) && !(Is16SizeEnabled || Is24SizeEnabled || Is32SizeEnabled || Is48SizeEnabled || Is64SizeEnabled || Is72SizeEnabled || Is96SizeEnabled || Is128SizeEnabled || Is256SizeEnabled))
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.IcoSizeNotSelected));
                    return;
                }
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    IsSaving = false;
                    int saveFailedCount = 0;

                    if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                    {
                        await Task.Run(() =>
                        {
                            for (int index = 0; index < selectedItemsList.Count; index++)
                            {
                                if (selectedItemsList[index] is object selectedItem)
                                {
                                    int iconIndex = Convert.ToInt32((selectedItem as IconModel).DisplayIndex);

                                    // 提取 Ico 图标文件
                                    if (Equals(SelectedIconFormat, IconFormatList[0]))
                                    {
                                        try
                                        {
                                            List<Bitmap> icoList = [];

                                            if (Is256SizeEnabled && GetFixedSizeIcon(iconIndex, 256) is Icon size256Icon)
                                            {
                                                icoList.Add(size256Icon.ToBitmap());
                                            }

                                            if (Is128SizeEnabled && GetFixedSizeIcon(iconIndex, 128) is Icon size128Icon)
                                            {
                                                icoList.Add(size128Icon.ToBitmap());
                                            }

                                            if (Is96SizeEnabled && GetFixedSizeIcon(iconIndex, 96) is Icon size96Icon)
                                            {
                                                icoList.Add(size96Icon.ToBitmap());
                                            }

                                            if (Is72SizeEnabled && GetFixedSizeIcon(iconIndex, 72) is Icon size72Icon)
                                            {
                                                icoList.Add(size72Icon.ToBitmap());
                                            }

                                            if (Is64SizeEnabled && GetFixedSizeIcon(iconIndex, 64) is Icon size64Icon)
                                            {
                                                icoList.Add(size64Icon.ToBitmap());
                                            }

                                            if (Is48SizeEnabled && GetFixedSizeIcon(iconIndex, 48) is Icon size48Icon)
                                            {
                                                icoList.Add(size48Icon.ToBitmap());
                                            }

                                            if (Is32SizeEnabled && GetFixedSizeIcon(iconIndex, 32) is Icon size32Icon)
                                            {
                                                icoList.Add(size32Icon.ToBitmap());
                                            }

                                            if (Is24SizeEnabled && GetFixedSizeIcon(iconIndex, 24) is Icon size24Icon)
                                            {
                                                icoList.Add(size24Icon.ToBitmap());
                                            }

                                            if (Is16SizeEnabled && GetFixedSizeIcon(iconIndex, 16) is Icon size16Icon)
                                            {
                                                icoList.Add(size16Icon.ToBitmap());
                                            }

                                            if (!(icoList.Count > 0 && SaveIcon(icoList, Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.ico", Path.GetFileName(filePath), iconIndex))) is true))
                                            {
                                                saveFailedCount++;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            saveFailedCount++;
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 1, e);
                                        }
                                    }
                                    else if (SelectedIconFormat.Equals(IconFormatList[1]))
                                    {
                                        try
                                        {
                                            if (GetFixedSizeIcon(iconIndex, Convert.ToInt32(SelectedIconSize.Key)) is Icon icon)
                                            {
                                                icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                                icon.Dispose();
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            saveFailedCount++;
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 2, e);
                                        }
                                    }
                                }
                            }
                        });
                    }
                    else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                    {
                        // 提取 Ico 图标文件
                        if (Equals(SelectedIconFormat, IconFormatList[0]))
                        {
                            try
                            {
                                List<Bitmap> icoList = [];

                                if (Is256SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_JUMBO) is Icon size256Icon)
                                {
                                    icoList.Add(size256Icon.ToBitmap());
                                }

                                if (Is48SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE) is Icon size48Icon)
                                {
                                    icoList.Add(size48Icon.ToBitmap());
                                }

                                if (Is32SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_LARGE) is Icon size32Icon)
                                {
                                    icoList.Add(size32Icon.ToBitmap());
                                }

                                if (Is16SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_SMALL) is Icon size16Icon)
                                {
                                    icoList.Add(size16Icon.ToBitmap());
                                }

                                if (!(icoList.Count > 0 && SaveIcon(icoList, Path.Combine(openFolderDialog.SelectedPath, string.Format("{0}.ico", Path.GetFileName(filePath)))) is true))
                                {
                                    saveFailedCount++;
                                }
                            }
                            catch (Exception e)
                            {
                                saveFailedCount++;
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 3, e);
                            }
                        }
                        else if (SelectedIconFormat.Equals(IconFormatList[1]))
                        {
                            try
                            {
                                if (Convert.ToInt32(SelectedIconSize.Key) is 16)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_SMALL) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                                else if (Convert.ToInt32(SelectedIconSize.Key) is 32)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_LARGE) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                                else if (Convert.ToInt32(SelectedIconSize.Key) is 48)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                                else if (Convert.ToInt32(SelectedIconSize.Key) is 256)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_JUMBO) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                saveFailedCount++;
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 4, e);
                            }
                        }
                    }

                    openFolderDialog.Dispose();
                    IsSaving = false;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.IconExtract, selectedItemsList.Count - saveFailedCount, saveFailedCount));
                }
                else
                {
                    openFolderDialog.Dispose();
                }
            }
        }

        /// <summary>
        /// 导出所有图标
        /// </summary>
        private async void OnExportAllIconsClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (Equals(SelectedIconFormat, IconFormatList[0]) && !(Is16SizeEnabled || Is24SizeEnabled || Is32SizeEnabled || Is48SizeEnabled || Is64SizeEnabled || Is72SizeEnabled || Is96SizeEnabled || Is128SizeEnabled || Is256SizeEnabled))
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.IcoSizeNotSelected));
                    return;
                }
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    List<IconModel> iconList = [.. IconCollection];
                    IsSaving = false;
                    int saveFailedCount = 0;

                    if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                    {
                        await Task.Run(() =>
                        {
                            for (int index = 0; index < iconList.Count; index++)
                            {
                                if (IconCollection[index] is object selectedItem)
                                {
                                    int iconIndex = Convert.ToInt32((selectedItem as IconModel).DisplayIndex);

                                    // 提取 Ico 图标文件
                                    if (Equals(SelectedIconFormat, IconFormatList[0]))
                                    {
                                        try
                                        {
                                            List<Bitmap> icoList = [];

                                            if (Is256SizeEnabled && GetFixedSizeIcon(iconIndex, 256) is Icon size256Icon)
                                            {
                                                icoList.Add(size256Icon.ToBitmap());
                                            }

                                            if (Is128SizeEnabled && GetFixedSizeIcon(iconIndex, 128) is Icon size128Icon)
                                            {
                                                icoList.Add(size128Icon.ToBitmap());
                                            }

                                            if (Is96SizeEnabled && GetFixedSizeIcon(iconIndex, 96) is Icon size96Icon)
                                            {
                                                icoList.Add(size96Icon.ToBitmap());
                                            }

                                            if (Is72SizeEnabled && GetFixedSizeIcon(iconIndex, 72) is Icon size72Icon)
                                            {
                                                icoList.Add(size72Icon.ToBitmap());
                                            }

                                            if (Is64SizeEnabled && GetFixedSizeIcon(iconIndex, 64) is Icon size64Icon)
                                            {
                                                icoList.Add(size64Icon.ToBitmap());
                                            }

                                            if (Is48SizeEnabled && GetFixedSizeIcon(iconIndex, 48) is Icon size48Icon)
                                            {
                                                icoList.Add(size48Icon.ToBitmap());
                                            }

                                            if (Is32SizeEnabled && GetFixedSizeIcon(iconIndex, 32) is Icon size32Icon)
                                            {
                                                icoList.Add(size32Icon.ToBitmap());
                                            }

                                            if (Is24SizeEnabled && GetFixedSizeIcon(iconIndex, 24) is Icon size24Icon)
                                            {
                                                icoList.Add(size24Icon.ToBitmap());
                                            }

                                            if (Is16SizeEnabled && GetFixedSizeIcon(iconIndex, 16) is Icon size16Icon)
                                            {
                                                icoList.Add(size16Icon.ToBitmap());
                                            }

                                            if (!(icoList.Count > 0 && SaveIcon(icoList, Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.ico", Path.GetFileName(filePath), iconIndex))) is true))
                                            {
                                                saveFailedCount++;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            saveFailedCount++;
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 1, e);
                                        }
                                    }
                                    else if (SelectedIconFormat.Equals(IconFormatList[1]))
                                    {
                                        try
                                        {
                                            if (GetFixedSizeIcon(iconIndex, Convert.ToInt32(SelectedIconSize.Key)) is Icon icon)
                                            {
                                                icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                                icon.Dispose();
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            saveFailedCount++;
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 2, e);
                                        }
                                    }
                                }
                            }
                        });
                    }
                    else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                    {
                        // 提取 Ico 图标文件
                        if (Equals(SelectedIconFormat, IconFormatList[0]))
                        {
                            try
                            {
                                List<Bitmap> icoList = [];

                                if (Is256SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_JUMBO) is Icon size256Icon)
                                {
                                    icoList.Add(size256Icon.ToBitmap());
                                }

                                if (Is48SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE) is Icon size48Icon)
                                {
                                    icoList.Add(size48Icon.ToBitmap());
                                }

                                if (Is32SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_LARGE) is Icon size32Icon)
                                {
                                    icoList.Add(size32Icon.ToBitmap());
                                }

                                if (Is16SizeEnabled && GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_SMALL) is Icon size16Icon)
                                {
                                    icoList.Add(size16Icon.ToBitmap());
                                }

                                if (!(icoList.Count > 0 && SaveIcon(icoList, Path.Combine(openFolderDialog.SelectedPath, string.Format("{0}.ico", Path.GetFileName(filePath)))) is true))
                                {
                                    saveFailedCount++;
                                }
                            }
                            catch (Exception e)
                            {
                                saveFailedCount++;
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 1, e);
                            }
                        }
                        else if (SelectedIconFormat.Equals(IconFormatList[1]))
                        {
                            try
                            {
                                if (Convert.ToInt32(SelectedIconSize.Key) is 16)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_SMALL) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                                else if (Convert.ToInt32(SelectedIconSize.Key) is 32)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_LARGE) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                                else if (Convert.ToInt32(SelectedIconSize.Key) is 48)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                                else if (Convert.ToInt32(SelectedIconSize.Key) is 256)
                                {
                                    if (GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_JUMBO) is Icon icon)
                                    {
                                        icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1}.png", Path.GetFileName(filePath), Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                        icon.Dispose();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                saveFailedCount++;
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 2, e);
                            }
                        }
                    }

                    openFolderDialog.Dispose();
                    IsSaving = false;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.IconExtract, IconCollection.Count - saveFailedCount, saveFailedCount));
                }
                else
                {
                    openFolderDialog.Dispose();
                }
            }
        }

        #endregion 第二部分：提取图标页面——挂载的事件

        /// <summary>
        /// 解析带有图标的二进制文件
        /// </summary>
        public async Task ParseIconFileAsync(string iconFilePath)
        {
            IconCollection.Clear();
            int iconsNum = 0;
            IconExtractResultKind = IconExtractResultKind.Parsing;

            List<IconModel> iconsList = await Task.Run(() =>
            {
                List<IconModel> iconsList = [];

                try
                {
                    filePath = iconFilePath;
                    // 图标个数
                    iconsNum = User32Library.PrivateExtractIcons(filePath, 0, 0, 0, null, null, 0, 0);

                    // 显示图标
                    nint[] phicon = new nint[iconsNum];
                    int[] piconid = new int[iconsNum];

                    int nIcons = User32Library.PrivateExtractIcons(filePath, 0, 48, 48, phicon, piconid, iconsNum, 0);
                    for (int index = 0; index < iconsNum; index++)
                    {
                        Icon icon = Icon.FromHandle(phicon[index]);
                        MemoryStream memoryStream = new();
                        icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        iconsList.Add(new IconModel()
                        {
                            DisplayIndex = Convert.ToString(index),
                            IconMemoryStream = memoryStream,
                        });

                        icon.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseIconFileAsync), 1, e);
                }

                return iconsList;
            });

            if (iconsList.Count > 0)
            {
                try
                {
                    GetResults = string.Format(GetFileContainedResultsString, Path.GetFileName(filePath), iconsNum);
                    NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                    ImageSource = null;
                    IsImageEmpty = true;

                    foreach (IconModel iconItem in iconsList)
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(iconItem.IconMemoryStream.AsRandomAccessStream());
                        IconCollection.Add(new IconModel()
                        {
                            DisplayIndex = iconItem.DisplayIndex,
                            IconImage = bitmapImage
                        });

                        iconItem.IconMemoryStream.Dispose();
                    }

                    IconExtractResultKind = IconExtractResultKind.Successfully;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseIconFileAsync), 2, e);
                    GetResults = string.Format(GetFileContainedResultsString, Path.GetFileName(filePath), 0);
                    NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                    ImageSource = null;
                    IsImageEmpty = true;
                    IconExtractResultKind = IconExtractResultKind.Failed;
                }
            }
            else
            {
                GetResults = string.Format(GetFileContainedResultsString, Path.GetFileName(filePath), 0);
                NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                ImageSource = null;
                IsImageEmpty = true;
                IconExtractResultKind = IconExtractResultKind.Failed;
            }
        }

        /// <summary>
        /// 获取文件关联的图标
        /// </summary>
        public async Task GetFileAssociatedIconAsync(string iconFilePath)
        {
            IconCollection.Clear();
            IconExtractResultKind = IconExtractResultKind.Parsing;

            List<IconModel> iconsList = [];

            try
            {
                filePath = iconFilePath;
                Icon icon = GetFixedSizeAssociatedIcon(filePath, SHIL.SHIL_EXTRALARGE);
                if (icon is not null)
                {
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    iconsList.Add(new IconModel()
                    {
                        DisplayIndex = "0",
                        IconMemoryStream = memoryStream,
                    });

                    icon.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFileAssociatedIconAsync), 1, e);
            }

            if (iconsList.Count > 0)
            {
                try
                {
                    GetResults = string.Format(GetFileAssociatedResultsSuccessfullyString, Path.GetFileName(filePath));
                    NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                    ImageSource = null;
                    IsImageEmpty = true;

                    foreach (IconModel iconItem in iconsList)
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(iconItem.IconMemoryStream.AsRandomAccessStream());
                        IconCollection.Add(new IconModel()
                        {
                            DisplayIndex = iconItem.DisplayIndex,
                            IconImage = bitmapImage
                        });

                        iconItem.IconMemoryStream.Dispose();
                    }

                    IconExtractResultKind = IconExtractResultKind.Successfully;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFileAssociatedIconAsync), 2, e);
                    GetResults = string.Format(GetFileAssociatedResultsFailedString, Path.GetFileName(filePath), 0);
                    NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                    ImageSource = null;
                    IsImageEmpty = true;
                    IconExtractResultKind = IconExtractResultKind.Failed;
                }
            }
            else
            {
                GetResults = string.Format(GetFileAssociatedResultsFailedString, Path.GetFileName(filePath), 0);
                NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                ImageSource = null;
                IsImageEmpty = true;
                IconExtractResultKind = IconExtractResultKind.Failed;
            }
        }

        /// <summary>
        /// 检查是否正在解析中或保存中
        /// </summary>
        public bool GetIsNotParsingOrSaving(IconExtractResultKind iconExtractResultKind, bool isSaving)
        {
            return iconExtractResultKind is not IconExtractResultKind.Parsing && !isSaving;
        }

        /// <summary>
        /// 保存获取到的 ico 图片到 Icon 文件
        /// </summary>
        public static bool SaveIcon(IEnumerable<Bitmap> imageList, string outputPath)
        {
            try
            {
                List<Bitmap> bitmapList = [.. imageList];
                if (bitmapList.Count is 0)
                {
                    throw new ArgumentException("can not be null", nameof(imageList));
                }

                using FileStream stream = new(outputPath, FileMode.Create);
                using BinaryWriter binaryWriter = new(stream);

                // 写入ICO文件头
                binaryWriter.Write((ushort)0); // 保留字
                binaryWriter.Write((ushort)1); // 类型：ICO
                binaryWriter.Write((ushort)bitmapList.Count); // 图像数量

                int currentOffset = 6 + 16 * bitmapList.Count; // 第一个图像数据偏移
                List<byte[]> entryList = [];
                List<byte[]> imageDataList = [];

                foreach (Bitmap bitmap in bitmapList)
                {
                    // 转换为32位ARGB格式确保透明度
                    using Bitmap convertedBmp = ConvertTo32bppArgb(bitmap);
                    using MemoryStream memoryStream = new();
                    convertedBmp.Save(memoryStream, ImageFormat.Png);
                    byte[] pngData = memoryStream.ToArray();
                    imageDataList.Add(pngData);

                    // 创建目录项
                    byte width = GetIconDimension(convertedBmp.Width);
                    byte height = GetIconDimension(convertedBmp.Height);
                    uint size = (uint)pngData.Length;
                    uint offset = (uint)currentOffset;

                    using (MemoryStream emptyStream = new())
                    using (BinaryWriter entryWriter = new(emptyStream))
                    {
                        entryWriter.Write(width);
                        entryWriter.Write(height);
                        entryWriter.Write((byte)0); // 调色板数量（无调色板）
                        entryWriter.Write((byte)0); // 保留
                        entryWriter.Write((ushort)1); // 颜色平面数
                        entryWriter.Write((ushort)32); // 每像素位数（32位ARGB）
                        entryWriter.Write(size);
                        entryWriter.Write(offset);
                        entryList.Add(emptyStream.ToArray());
                    }

                    currentOffset += (int)size;
                }

                // 写入所有目录项
                foreach (byte[] entry in entryList)
                {
                    binaryWriter.Write(entry);
                }

                // 写入所有PNG图像数据
                foreach (byte[] data in imageDataList)
                {
                    binaryWriter.Write(data);
                }

                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(SaveIcon), 1, e);
                return false;
            }
        }

        private static byte GetIconDimension(int dimension)
        {
            return (byte)(dimension >= 256 ? 0 : dimension);
        }

        private static Bitmap ConvertTo32bppArgb(Bitmap image)
        {
            Bitmap clone = new(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(clone))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            return clone;
        }

        /// <summary>
        /// 获取固定大小图标资源
        /// </summary>
        private Icon GetFixedSizeIcon(int iconIndex, int size)
        {
            try
            {
                nint[] phicon = new nint[1];
                int[] piconid = new int[1];
                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, size, size, phicon, piconid, 1, 0);
                return nIcons is not 0 ? Icon.FromHandle(phicon[0]) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFixedSizeIcon), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 获取固定大小文件关联图标资源
        /// </summary>
        private Icon GetFixedSizeAssociatedIcon(string filePath, SHIL shil)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    Shell32Library.SHGetFileInfo(filePath, 0, out SHFILEINFO shellInfo, (uint)Marshal.SizeOf<SHFILEINFO>(), SHGFI.SHGFI_SYSICONINDEX);
                    return Shell32Library.SHGetImageList(shil, typeof(IImageList).GUID, out IImageList imageList) is 0 && imageList.GetIcon(shellInfo.iIcon, 0, out nint hIcon) is 0 ? Icon.FromHandle(hIcon) : null;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFileAssociatedIconAsync), 1, e);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 检查图标提取是否成功
        /// </summary>
        private Visibility CheckIconExtractState(IconExtractResultKind iconResultResultKind, IconExtractResultKind comparedIconExtractResultKind)
        {
            return Equals(iconResultResultKind, comparedIconExtractResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility GetSelectedFileAssociatedIconSize(KeyValuePair<string, string> selectedGetIconType, KeyValuePair<string, string> comparedGetIconType)
        {
            return Equals(selectedGetIconType, comparedGetIconType) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
