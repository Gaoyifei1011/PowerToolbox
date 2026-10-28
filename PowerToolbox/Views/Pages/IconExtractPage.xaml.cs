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
    internal sealed partial class IconExtractPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

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

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private IconExtractResultKind _iconExtractResultKind;

        internal IconExtractResultKind IconExtractResultKind
        {
            get { return _iconExtractResultKind; }

            private set
            {
                if (!Equals(_iconExtractResultKind, value))
                {
                    _iconExtractResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(IconExtractResultKind)));
                }
            }
        }

        private bool _isItemSelected;

        private bool IsItemSelected
        {
            get { return _isItemSelected; }

            set
            {
                if (!Equals(_isItemSelected, value))
                {
                    _isItemSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsItemSelected)));
                }
            }
        }

        private bool _isImageEmpty;

        private bool IsImageEmpty
        {
            get { return _isImageEmpty; }

            set
            {
                if (!Equals(_isImageEmpty, value))
                {
                    _isImageEmpty = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsImageEmpty)));
                }
            }
        }

        private bool _isSaving;

        internal bool IsSaving
        {
            get { return _isSaving; }

            private set
            {
                if (!Equals(_isSaving, value))
                {
                    _isSaving = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSaving)));
                }
            }
        }

        private string _getResults;

        private string GetResults
        {
            get { return _getResults; }

            set
            {
                if (!string.Equals(_getResults, value))
                {
                    _getResults = value;
                    PropertyChanged?.Invoke(this, new(nameof(GetResults)));
                }
            }
        }

        private string _noResources;

        private string NoResources
        {
            get { return _noResources; }

            set
            {
                if (!string.Equals(_noResources, value))
                {
                    _noResources = value;
                    PropertyChanged?.Invoke(this, new(nameof(NoResources)));
                }
            }
        }

        private ComboBoxItemModel _selectedGetIconType;

        internal ComboBoxItemModel SelectedGetIconType
        {
            get { return _selectedGetIconType; }

            private set
            {
                if (!Equals(_selectedGetIconType, value))
                {
                    _selectedGetIconType = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedGetIconType)));
                }
            }
        }

        private ComboBoxItemModel _selectedIconFormat;

        private ComboBoxItemModel SelectedIconFormat
        {
            get { return _selectedIconFormat; }

            set
            {
                if (!Equals(_selectedIconFormat, value))
                {
                    _selectedIconFormat = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedIconFormat)));
                }
            }
        }

        private IconSizeModel _selectedFileContainSize;

        public IconSizeModel SelectedFileContainSize
        {
            get { return _selectedFileContainSize; }

            set
            {
                if (!Equals(_selectedFileContainSize, value))
                {
                    _selectedFileContainSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedFileContainSize)));
                }
            }
        }

        private IconSizeModel _selectedFileAssociateSize;

        public IconSizeModel SelectedFileAssociateSize
        {
            get { return _selectedFileAssociateSize; }

            set
            {
                if (!Equals(_selectedFileAssociateSize, value))
                {
                    _selectedFileAssociateSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedFileAssociateSize)));
                }
            }
        }

        private ImageSource _imageSource;

        private ImageSource ImageSource
        {
            get { return _imageSource; }

            set
            {
                if (!Equals(_imageSource, value))
                {
                    _imageSource = value;
                    PropertyChanged?.Invoke(this, new(nameof(ImageSource)));
                }
            }
        }

        internal List<ComboBoxItemModel> GetIconTypeList { get; } = [];

        private List<ComboBoxItemModel> IconFormatList { get; } =
        [
            new() { SelectedValue = ".ico", DisplayMember = ".ico" },
            new() { SelectedValue = ".png", DisplayMember = ".png" }
        ];

        private List<IconSizeModel> FileContainIconSizeList { get; } =
        [
            new() { Size = 16, ExtractSize = "16 * 16" },
            new() { Size = 24, ExtractSize = "24 * 24" },
            new() { Size = 32, ExtractSize = "32 * 32" },
            new() { Size = 48, ExtractSize = "48 * 48" },
            new() { Size = 64, ExtractSize = "64 * 64" },
            new() { Size = 72, ExtractSize = "72 * 72" },
            new() { Size = 96, ExtractSize = "96 * 96" },
            new() { Size = 128, ExtractSize = "128 * 128" },
            new() { Size = 256, ExtractSize = "256 * 256" }
        ];

        private List<IconSizeModel> FileAssociateIconSizeList { get; } =
        [
            new() { Size = 16, ExtractSize = "16 * 16" },
            new() { Size = 32, ExtractSize = "32 * 32" },
            new() { Size = 48, ExtractSize = "48 * 48" },
            new() { Size = 256, ExtractSize = "256 * 256" }
        ];

        private WinRTObservableCollection<IconModel> IconCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal IconExtractPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

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
                        if (await GetDragDropSelectedFilesAsync(args.DataView) is List<string> fileList && fileList.Count is 1)
                        {
                            // 文件包含的图标
                            if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                            {
                                string extensionName = Path.GetExtension(fileList[0]);

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
                            // 文件关联的图标
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
                if (await GetDragDropSelectedFilesAsync(args.DataView) is List<string> fileList && fileList.Count is 1)
                {
                    filePath = fileList[0];
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                {
                    await ParseFileContainIconAsync(filePath);
                }
                else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                {
                    await ParseFileAssociateIconAsync(filePath);
                }
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 网格控件选中项发生改变时的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            IsItemSelected = IconExtractGridView.SelectedItems.Count > 0;
            if (IconExtractGridView.SelectedItem is IconModel iconItem)
            {
                // 文件包含的图标
                if (Equals(SelectedGetIconType, GetIconTypeList[0]) && SelectedFileContainSize is not null)
                {
                    try
                    {
                        if (GetFileContainFixSizeIcon(filePath, Convert.ToInt32(iconItem.DisplayIndex), SelectedFileContainSize.Size) is Icon icon)
                        {
                            UpdateDisplayIcon(icon);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnSelectionChanged), 1, e);
                    }
                }
                // 文件关联的图标
                else if (Equals(SelectedGetIconType, GetIconTypeList[1]) && SelectedFileAssociateSize is not null)
                {
                    try
                    {
                        if (GetFileAssociateFixSizeIcon(filePath, SelectedFileAssociateSize.Size) is Icon icon)
                        {
                            UpdateDisplayIcon(icon);
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
                ImageSource = null;
                IsImageEmpty = true;
            }
        }

        /// <summary>
        /// 选择获取的图标类型
        /// </summary>
        private void OnGetIconTypeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedGetIconType, comboBox.SelectedItem))
            {
                SelectedGetIconType = comboBox.SelectedItem is ComboBoxItemModel getIconType ? getIconType : null;
                IconExtractResultKind = IconExtractResultKind.Welcome;
                IsImageEmpty = true;
                GetResults = NoSelectedFileString;
                ImageSource = null;
                NoResources = string.Empty;
                IsItemSelected = false;
                SelectedFileContainSize = FileContainIconSizeList[8];
                SelectedFileAssociateSize = FileAssociateIconSizeList[3];
                IconCollection.Clear();
            }
        }

        /// <summary>
        /// 选择图标格式
        /// </summary>
        private void OnIconFormatSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedIconFormat, comboBox.SelectedItem))
            {
                SelectedIconFormat = comboBox.SelectedItem is ComboBoxItemModel iconFormat ? iconFormat : null;
            }
        }

        /// <summary>
        /// 文件包含图标尺寸选项选中项发生变化后触发的事件
        /// </summary>
        private void OnFileContainSizeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedFileContainSize, comboBox.SelectedItem))
            {
                SelectedFileContainSize = comboBox.SelectedItem is IconSizeModel iconSize ? iconSize : null;

                if (SelectedFileContainSize is not null && IconExtractGridView.SelectedItem is IconModel iconItem)
                {
                    try
                    {
                        if (GetFileContainFixSizeIcon(filePath, Convert.ToInt32(iconItem.DisplayIndex), SelectedFileContainSize.Size) is Icon icon)
                        {
                            UpdateDisplayIcon(icon);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnFileContainSizeSelectionChanged), 1, e);
                    }
                }
            }
        }

        /// <summary>
        /// 文件关联图标尺寸选项选中项发生变化后触发的事件
        /// </summary>
        private void OnFileAssociateSizeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedFileAssociateSize, comboBox.SelectedItem))
            {
                SelectedFileAssociateSize = comboBox.SelectedItem is IconSizeModel iconSize ? iconSize : null;

                if (SelectedFileAssociateSize is not null)
                {
                    try
                    {
                        if (GetFileAssociateFixSizeIcon(filePath, SelectedFileAssociateSize.Size) is Icon icon)
                        {
                            UpdateDisplayIcon(icon);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnFileAssociateSizeSelectionChanged), 2, e);
                    }
                }
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            IconExtractGridView.SelectAll();
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            IconExtractGridView.DeselectRange(new(0, (uint)IconExtractGridView.Items.Count));
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            List<object> selectedItemList = [.. IconExtractGridView.SelectedItems];

            foreach (object item in IconExtractGridView.Items)
            {
                if (selectedItemList.Contains(item))
                {
                    IconExtractGridView.SelectedItems.Remove(item);
                }
                else
                {
                    IconExtractGridView.SelectedItems.Add(item);
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
                        await ParseFileContainIconAsync(openFileDialog.FileName);
                    }
                    else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                    {
                        await ParseFileAssociateIconAsync(openFileDialog.FileName);
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
                IList<object> selectedItemsList = IconExtractGridView.SelectedItems;
                if (Equals(SelectedIconFormat, IconFormatList[0]) && ((Equals(SelectedGetIconType, GetIconTypeList[0]) && FileContainIconGridView.SelectedItems.Count is 0) || Equals(SelectedGetIconType, GetIconTypeList[1]) && FileAssociateIconGridView.SelectedItems.Count is 0))
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

                    // 文件包含的图标
                    if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                    {
                        // 提取 Ico 图标文件
                        if (Equals(SelectedIconFormat, IconFormatList[0]))
                        {
                            saveFailedCount = await SaveFileContainIcoImageAsync(openFolderDialog.SelectedPath, [.. FileContainIconGridView.SelectedItems], [.. selectedItemsList]);
                        }
                        // 提取 Png 图标文件
                        else if (SelectedIconFormat.Equals(IconFormatList[1]))
                        {
                            if (SelectedFileContainSize is not null)
                            {
                                saveFailedCount = await SaveFileContainPngImageAsync(openFolderDialog.SelectedPath, SelectedFileContainSize.Size, [.. selectedItemsList]);
                            }
                        }
                    }
                    // 文件关联的图标
                    else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                    {
                        // 提取 Ico 图标文件
                        if (Equals(SelectedIconFormat, IconFormatList[0]))
                        {
                            saveFailedCount = SaveFileAssociateIcoImage(openFolderDialog.SelectedPath, [.. FileAssociateIconGridView.SelectedItems], [.. selectedItemsList]);
                        }
                        // 提取 Png 图标文件
                        else if (SelectedIconFormat.Equals(IconFormatList[1]))
                        {
                            if (SelectedFileAssociateSize is not null)
                            {
                                saveFailedCount = SaveFileAssociatePngImage(openFolderDialog.SelectedPath, SelectedFileAssociateSize.Size, [.. selectedItemsList]);
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
                if (Equals(SelectedIconFormat, IconFormatList[0]) && ((Equals(SelectedGetIconType, GetIconTypeList[0]) && FileContainIconGridView.SelectedItems.Count is 0) || Equals(SelectedGetIconType, GetIconTypeList[1]) && FileAssociateIconGridView.SelectedItems.Count is 0))
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
                    List<object> iconList = [.. IconCollection];
                    IsSaving = false;
                    int saveFailedCount = 0;

                    // 文件包含的图标
                    if (Equals(SelectedGetIconType, GetIconTypeList[0]))
                    {
                        // 提取 Ico 图标文件
                        if (Equals(SelectedIconFormat, IconFormatList[0]))
                        {
                            saveFailedCount = await SaveFileContainIcoImageAsync(openFolderDialog.SelectedPath, [.. FileContainIconGridView.SelectedItems], iconList);
                        }
                        // 提取 Png 图标文件
                        else if (SelectedIconFormat.Equals(IconFormatList[1]))
                        {
                            if (SelectedFileContainSize is not null)
                            {
                                saveFailedCount = await SaveFileContainPngImageAsync(openFolderDialog.SelectedPath, SelectedFileContainSize.Size, iconList);
                            }
                        }
                    }
                    // 文件关联的图标
                    else if (Equals(SelectedGetIconType, GetIconTypeList[1]))
                    {
                        // 提取 Ico 图标文件
                        if (Equals(SelectedIconFormat, IconFormatList[0]))
                        {
                            saveFailedCount = SaveFileAssociateIcoImage(openFolderDialog.SelectedPath, [.. FileAssociateIconGridView.SelectedItems], iconList);
                        }
                        // 提取 Png 图标文件
                        else if (SelectedIconFormat.Equals(IconFormatList[1]))
                        {
                            if (SelectedFileAssociateSize is not null)
                            {
                                saveFailedCount = SaveFileAssociatePngImage(openFolderDialog.SelectedPath, SelectedFileAssociateSize.Size, iconList);
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

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            GetIconTypeList.Add(new() { SelectedValue = "FileContained", DisplayMember = FileContainedString });
            GetIconTypeList.Add(new() { SelectedValue = "FileAssociated", DisplayMember = FileAssociatedString });
            IconExtractResultKind = IconExtractResultKind.Welcome;
            SelectedGetIconType = GetIconTypeList[0];
            SelectedIconFormat = IconFormatList[1];
            SelectedFileContainSize = FileContainIconSizeList[8];
            SelectedFileAssociateSize = FileAssociateIconSizeList[3];
            IsImageEmpty = true;
            IsItemSelected = false;
            GetResults = NoSelectedFileString;
        }

        /// <summary>
        /// 解析文件包含的所有图标文件
        /// </summary>
        internal async Task ParseFileContainIconAsync(string iconFilePath)
        {
            if (string.IsNullOrEmpty(iconFilePath))
            {
                return;
            }

            if (IconExtractResultKind is not IconExtractResultKind.Parsing)
            {
                IconCollection.Clear();
                IconExtractResultKind = IconExtractResultKind.Parsing;
                filePath = iconFilePath;
                (int iconTotalNum, List<IconModel> iconList) = await GetFileContainIconListAsync(filePath);

                if (iconList.Count > 0)
                {
                    try
                    {
                        GetResults = string.Format(GetFileContainedResultsString, Path.GetFileName(filePath), iconTotalNum);
                        NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                        ImageSource = null;
                        IsImageEmpty = true;

                        foreach (IconModel iconItem in iconList)
                        {
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(iconItem.IconMemoryStream.AsRandomAccessStream());
                            IconCollection.Add(new() { DisplayIndex = iconItem.DisplayIndex, IconImage = bitmapImage });
                            iconItem.IconMemoryStream.Dispose();
                        }

                        IconExtractResultKind = IconExtractResultKind.Successfully;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseFileContainIconAsync), 1, e);
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
        }

        /// <summary>
        /// 解析文件关联的所有图标文件
        /// </summary>
        internal async Task ParseFileAssociateIconAsync(string iconFilePath)
        {
            if (string.IsNullOrEmpty(iconFilePath))
            {
                return;
            }

            if (IconExtractResultKind is not IconExtractResultKind.Parsing)
            {
                IconCollection.Clear();
                IconExtractResultKind = IconExtractResultKind.Parsing;
                filePath = iconFilePath;
                (int iconTotalNum, List<IconModel> iconList) = GetFileAssociateIconList(filePath);

                if (iconList.Count > 0)
                {
                    try
                    {
                        GetResults = string.Format(GetFileAssociatedResultsSuccessfullyString, Path.GetFileName(filePath));
                        NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                        ImageSource = null;
                        IsImageEmpty = true;

                        foreach (IconModel iconItem in iconList)
                        {
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(iconItem.IconMemoryStream.AsRandomAccessStream());
                            IconCollection.Add(new() { DisplayIndex = iconItem.DisplayIndex, IconImage = bitmapImage });
                            iconItem.IconMemoryStream.Dispose();
                        }

                        IconExtractResultKind = IconExtractResultKind.Successfully;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseFileAssociateIconAsync), 2, e);
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
        }

        /// <summary>
        /// 获取文件包含的所有图标列表
        /// </summary>
        private async Task<(int iconNum, List<IconModel>)> GetFileContainIconListAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<IconModel> fileContainIconList = [];
                int iconTotalNum = 0;

                try
                {
                    // 图标个数
                    iconTotalNum = User32Library.PrivateExtractIcons(filePath, 0, 0, 0, null, null, 0, 0);

                    // 显示图标
                    nint[] phicon = new nint[iconTotalNum];
                    int[] piconid = new int[iconTotalNum];

                    int nIcons = User32Library.PrivateExtractIcons(filePath, 0, 48, 48, phicon, piconid, iconTotalNum, 0);
                    for (int index = 0; index < iconTotalNum; index++)
                    {
                        Icon icon = Icon.FromHandle(phicon[index]);
                        MemoryStream memoryStream = new();
                        icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        fileContainIconList.Add(new() { DisplayIndex = Convert.ToString(index), IconMemoryStream = memoryStream });
                        icon.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFileContainIconListAsync), 1, e);
                }
                return ValueTuple.Create(iconTotalNum, fileContainIconList);
            });
        }

        /// <summary>
        /// 获取文件关联的所有图标列表
        /// </summary>
        private (int iconNum, List<IconModel>) GetFileAssociateIconList(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return default;
            }

            List<IconModel> fileAssociateIconList = [];
            int iconTotalNum = 0;

            try
            {
                Icon icon = GetFileAssociateFixSizeIcon(filePath, 48);
                if (icon is not null)
                {
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    fileAssociateIconList.Add(new() { DisplayIndex = "0", IconMemoryStream = memoryStream, });
                    icon.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFileAssociateIconList), 1, e);
            }
            return ValueTuple.Create(iconTotalNum, fileAssociateIconList);
        }

        /// <summary>
        /// 获取文件包含的图标列表
        /// </summary>
        private List<Bitmap> GetFileContainIcoList(List<object> iconSizeList, string filePath, int iconIndex)
        {
            List<Bitmap> icoList = [];

            foreach (object iconSizeItem in iconSizeList)
            {
                if (iconSizeItem is IconSizeModel iconSize && GetFileContainFixSizeIcon(filePath, iconIndex, iconSize.Size) is Icon icon)
                {
                    icoList.Add(icon.ToBitmap());
                }
            }

            return icoList;
        }

        /// <summary>
        /// 获取文件关联的图标列表
        /// </summary>
        private List<Bitmap> GetFileAssociateIcoList(List<object> iconSizeList, string filePath)
        {
            List<Bitmap> icoList = [];

            foreach (object iconSizeItem in iconSizeList)
            {
                if (iconSizeItem is IconSizeModel iconSize && GetFileAssociateFixSizeIcon(filePath, iconSize.Size) is Icon icon)
                {
                    icoList.Add(icon.ToBitmap());
                }
            }

            return icoList;
        }

        /// <summary>
        /// 获取文件包含图标固定大小资源
        /// </summary>
        private Icon GetFileContainFixSizeIcon(string filePath, int iconIndex, int size)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return default;
            }

            try
            {
                nint[] phicon = new nint[1];
                int[] piconid = new int[1];
                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, size, size, phicon, piconid, 1, 0);
                return nIcons is not 0 ? Icon.FromHandle(phicon[0]) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetFileContainFixSizeIcon), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 获取文件关联图标固定大小资源
        /// </summary>
        private Icon GetFileAssociateFixSizeIcon(string filePath, int size)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return default;
            }

            try
            {
                SHIL shil = SHIL.SHIL_LARGE;
                if (size is 16)
                {
                    shil = SHIL.SHIL_SMALL;
                }
                else if (size is 32)
                {
                    shil = SHIL.SHIL_LARGE;
                }
                else if (size is 48)
                {
                    shil = SHIL.SHIL_EXTRALARGE;
                }
                else if (size is 256)
                {
                    shil = SHIL.SHIL_JUMBO;
                }

                Shell32Library.SHGetFileInfo(filePath, 0, out SHFILEINFO shellInfo, (uint)Marshal.SizeOf<SHFILEINFO>(), SHGFI.SHGFI_SYSICONINDEX);
                return Shell32Library.SHGetImageList(shil, typeof(IImageList).GUID, out IImageList imageList) is 0 && imageList.GetIcon(shellInfo.iIcon, 0, out nint hIcon) is 0 ? Icon.FromHandle(hIcon) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseFileAssociateIconAsync), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 获取拖拽支持选中的文件
        /// </summary>
        private async Task<List<string>> GetDragDropSelectedFilesAsync(DataPackageView dataPackageView)
        {
            if (dataPackageView is null)
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                List<string> fileList = [];

                try
                {
                    if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                    {
                        foreach (IStorageItem storageItem in await dataPackageView.GetStorageItemsAsync())
                        {
                            fileList.Add(storageItem.Path);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(GetDragDropSelectedFilesAsync), 1, e);
                }

                return fileList;
            });
        }

        /// <summary>
        /// 更新要显示的图标
        /// </summary>
        private void UpdateDisplayIcon(Icon icon)
        {
            if (icon is null)
            {
                return;
            }

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

        /// <summary>
        /// 保存文件包含的 ICO 图片格式
        /// </summary>
        private async Task<int> SaveFileContainIcoImageAsync(string folderPath, List<object> selectedIconSizeList, List<object> selectedItemList)
        {
            if (string.IsNullOrEmpty(folderPath) || selectedIconSizeList is null || selectedIconSizeList.Count is 0 || selectedItemList is null || selectedItemList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                int saveFailedCount = 0;

                foreach (object selectedItem in selectedItemList)
                {
                    if (selectedItem is IconModel iconItem)
                    {
                        int iconIndex = Convert.ToInt32(iconItem.DisplayIndex);
                        List<Bitmap> fileContainIcoList = GetFileContainIcoList(selectedIconSizeList, filePath, iconIndex);
                        if (!(fileContainIcoList.Count > 0 && SaveIcon(Path.Combine(folderPath, string.Format("{0} - {1}.ico", Path.GetFileName(filePath), iconIndex)), fileContainIcoList) is true))
                        {
                            saveFailedCount++;
                        }
                    }
                }
                return saveFailedCount;
            });
        }

        /// <summary>
        /// 保存文件包含的 PNG 图片格式
        /// </summary>
        private async Task<int> SaveFileContainPngImageAsync(string folderPath, int fileContainSize, List<object> selectedItemList)
        {
            if (string.IsNullOrEmpty(folderPath) || selectedItemList is null || selectedItemList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                int saveFailedCount = 0;

                foreach (object selectedItem in selectedItemList)
                {
                    if (selectedItem is IconModel iconItem)
                    {
                        try
                        {
                            int iconIndex = Convert.ToInt32(iconItem.DisplayIndex);
                            if (GetFileContainFixSizeIcon(filePath, iconIndex, fileContainSize) is Icon icon)
                            {
                                icon.ToBitmap().Save(Path.Combine(folderPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, fileContainSize)), ImageFormat.Png);
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

                return saveFailedCount;
            });
        }

        /// <summary>
        /// 保存文件关联的 ICO 图片格式
        /// </summary>
        private int SaveFileAssociateIcoImage(string folderPath, List<object> selectedIconSizeList, List<object> selectedItemList)
        {
            if (string.IsNullOrEmpty(folderPath) || selectedIconSizeList is null || selectedIconSizeList.Count is 0 || selectedItemList is null || selectedItemList.Count is 0)
            {
                return default;
            }

            int saveFailedCount = 0;

            foreach (object selectedItem in selectedItemList)
            {
                if (selectedItem is IconModel iconItem)
                {
                    List<Bitmap> fileAssociateIcoList = GetFileAssociateIcoList(selectedIconSizeList, filePath);
                    if (!(fileAssociateIcoList.Count > 0 && SaveIcon(Path.Combine(folderPath, string.Format("{0} - {1}.ico", Path.GetFileName(filePath), Convert.ToInt32(iconItem.DisplayIndex))), fileAssociateIcoList) is true))
                    {
                        saveFailedCount++;
                    }
                }
            }
            return saveFailedCount;
        }

        /// <summary>
        /// 保存文件关联的 PNG 图片格式
        /// </summary>
        private int SaveFileAssociatePngImage(string folderPath, int fileAssociateSize, List<object> selectedItemList)
        {
            if (string.IsNullOrEmpty(folderPath) || selectedItemList is null || selectedItemList.Count is 0)
            {
                return default;
            }

            int saveFailedCount = 0;

            foreach (object selectedItem in selectedItemList)
            {
                if (selectedItem is IconModel iconItem)
                {
                    try
                    {
                        // 必须运行在 UI 线程上
                        if (GetFileAssociateFixSizeIcon(filePath, fileAssociateSize) is Icon icon)
                        {
                            icon.ToBitmap().Save(Path.Combine(folderPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), Convert.ToInt32(iconItem.DisplayIndex), fileAssociateSize)), ImageFormat.Png);
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

            return saveFailedCount;
        }

        /// <summary>
        /// 保存图标文件
        /// </summary>
        private static bool SaveIcon(string savePath, List<Bitmap> imageList)
        {
            if (string.IsNullOrEmpty(savePath) || imageList is null || imageList.Count is 0)
            {
                return default;
            }

            // 保存图标
            try
            {
                using FileStream stream = new(savePath, FileMode.Create);
                using BinaryWriter binaryWriter = new(stream);

                // 写入ICO文件头
                binaryWriter.Write((ushort)0); // 保留字
                binaryWriter.Write((ushort)1); // 类型：ICO
                binaryWriter.Write((ushort)imageList.Count); // 图像数量

                int currentOffset = 6 + 16 * imageList.Count; // 第一个图像数据偏移
                List<byte[]> entryList = [];
                List<byte[]> imageDataList = [];

                foreach (Bitmap bitmap in imageList)
                {
                    // 转换为32位ARGB格式确保透明度
                    if (ConvertTo32bppArgb(bitmap) is Bitmap convertedBmp)
                    {
                        using MemoryStream memoryStream = new();
                        convertedBmp.Save(memoryStream, ImageFormat.Png);
                        byte[] pngData = memoryStream.ToArray();
                        imageDataList.Add(pngData);

                        // 创建目录项
                        byte width = (byte)(convertedBmp.Width >= 256 ? 0 : convertedBmp.Width);
                        byte height = (byte)(convertedBmp.Height >= 256 ? 0 : convertedBmp.Height);
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
                        convertedBmp.Dispose();
                    }
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

        /// <summary>
        /// 将任意像素格式的 Bitmap 转换为 Format32bppArgb(32 位 ARGB)格式的 Bitmap
        /// </summary>
        private static Bitmap ConvertTo32bppArgb(Bitmap bitmap)
        {
            if (bitmap is null)
            {
                return default;
            }

            Bitmap format32Bitmap = new(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(format32Bitmap))
            {
                graphics.DrawImage(bitmap, new Rectangle(0, 0, format32Bitmap.Width, format32Bitmap.Height));
            }
            return format32Bitmap;
        }

        /// <summary>
        /// 检查是否正在解析中或保存中
        /// </summary>
        internal bool GetIsNotParsingOrSaving(IconExtractResultKind iconExtractResultKind, bool isSaving)
        {
            return iconExtractResultKind is not IconExtractResultKind.Parsing && !isSaving;
        }

        /// <summary>
        /// 检查图标提取是否成功
        /// </summary>
        private Visibility CheckIconExtractResultKindVisibility(IconExtractResultKind iconResultResultKind, IconExtractResultKind comparedIconExtractResultKind)
        {
            return Equals(iconResultResultKind, comparedIconExtractResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility GetIconTypeVisibility(ComboBoxItemModel selectedGetIconType, ComboBoxItemModel comparedGetIconType)
        {
            return Equals(selectedGetIconType, comparedGetIconType) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
