using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using PowerToolbox.Extensions.Collections;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        private readonly string FilterConditionString = ResourceService.IconExtractResource.GetString("FilterCondition");
        private readonly string GetResultsString = ResourceService.IconExtractResource.GetString("GetResults");
        private readonly string NoOtherExtensionNameFileString = ResourceService.IconExtractResource.GetString("NoOtherExtensionNameFile");
        private readonly string NoMultiFileString = ResourceService.IconExtractResource.GetString("NoMultiFile");
        private readonly string NoResourcesString = ResourceService.IconExtractResource.GetString("NoResources");
        private readonly string NoSelectedFileString = ResourceService.IconExtractResource.GetString("NoSelectedFile");
        private readonly string ParsingIconString = ResourceService.IconExtractResource.GetString("ParsingIcon");
        private readonly string SelectFileString = ResourceService.IconExtractResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.IconExtractResource.GetString("SelectFolder");
        private readonly string SavingNowString = ResourceService.IconExtractResource.GetString("SavingNow");
        private readonly object iconExtractLock = new();

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
            new KeyValuePair<int,string>(96, "96 * 96" ),
            new KeyValuePair<int,string>(128, "128 * 128" ),
            new KeyValuePair<int,string>(256, "256 * 256" )
        ];

        private WinRTObservableCollection<IconModel> IconCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public IconExtractPage()
        {
            InitializeComponent();
            SelectedIconFormat = IconFormatList[1];
            SelectedIconSize = IconSizeList[7];
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDragOver), 1, e);
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
            string filePath = string.Empty;
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
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 1, e);
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                await ParseIconFileAsync(filePath);
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

                IntPtr[] phicon = new IntPtr[1];
                int[] piconid = new int[1];
                int iconIndex = Convert.ToInt32((selectedItemsList.Last() as IconModel).DisplayIndex);
                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Key), Convert.ToInt32(SelectedIconSize.Key), phicon, piconid, 1, 0);

                if (nIcons is 0)
                {
                    ImageSource = null;
                    IsImageEmpty = true;
                }
                else
                {
                    try
                    {
                        Icon icon = Icon.FromHandle(phicon[0]);
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
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnSelectionChanged), 1, e);
                    }
                }
            }
            else
            {
                IsSelected = false;
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

                if (IconsGridView.SelectedItem is not null)
                {
                    IntPtr[] phicon = new IntPtr[1];
                    int[] piconid = new int[1];
                    int iconIndex = Convert.ToInt32((IconsGridView.SelectedItem as IconModel).DisplayIndex);
                    int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Key), Convert.ToInt32(SelectedIconSize.Key), phicon, piconid, 1, 0);

                    if (nIcons is 0)
                    {
                        ImageSource = null;
                        IsImageEmpty = true;
                    }
                    else
                    {
                        try
                        {
                            Icon icon = Icon.FromHandle(phicon[0]);
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnIconSizeClicked), 1, e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private async void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = FilterConditionString,
                Title = SelectFileString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                await ParseIconFileAsync(openFileDialog.FileName);
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 导出选中的图标
        /// </summary>
        private async void OnExportSelectedIconsClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                IList<object> selectedItemsList = IconsGridView.SelectedItems;

                OpenFolderDialog openFolderDialog = new()
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    IsSaving = false;
                    int saveFailedCount = 0;

                    await Task.Run(() =>
                    {
                        for (int index = 0; index < selectedItemsList.Count; index++)
                        {
                            if (selectedItemsList[index] is object selectedItem)
                            {
                                IntPtr[] phicon = new IntPtr[1];
                                int[] piconid = new int[1];
                                int iconIndex = Convert.ToInt32((selectedItem as IconModel).DisplayIndex);

                                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Key), Convert.ToInt32(SelectedIconSize.Key), phicon, piconid, 1, 0);

                                if (nIcons is not 0)
                                {
                                    try
                                    {
                                        if (Icon.FromHandle(phicon[0]) is Icon icon)
                                        {
                                            if (Equals(SelectedIconFormat, IconFormatList[0]))
                                            {
                                                bool result = SaveIcon(icon, Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1} - {2}.ico", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Key))));
                                                if (!result)
                                                {
                                                    saveFailedCount++;
                                                }
                                            }
                                            else
                                            {
                                                icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        saveFailedCount++;
                                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportSelectedIconsClicked), 1, e);
                                    }
                                }
                            }
                        }
                    });

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
                OpenFolderDialog openFolderDialog = new()
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    IsSaving = false;
                    int saveFailedCount = 0;

                    await Task.Run(() =>
                    {
                        lock (iconExtractLock)
                        {
                            for (int index = 0; index < IconCollection.Count; index++)
                            {
                                IntPtr[] phicon = new IntPtr[1];
                                int[] piconid = new int[1];
                                int iconIndex = Convert.ToInt32(IconCollection[index].DisplayIndex);
                                int nIcons = User32Library.PrivateExtractIcons(filePath, iconIndex, Convert.ToInt32(SelectedIconSize.Key), Convert.ToInt32(SelectedIconSize.Key), phicon, piconid, 1, 0);

                                if (nIcons is not 0)
                                {
                                    try
                                    {
                                        if (Icon.FromHandle(phicon[0]) is Icon icon)
                                        {
                                            if (Equals(SelectedIconFormat, IconFormatList[0]))
                                            {
                                                bool result = SaveIcon(icon, Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1} - {2}.ico", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Key))));
                                                if (!result)
                                                {
                                                    saveFailedCount++;
                                                }
                                            }
                                            else
                                            {
                                                icon.ToBitmap().Save(Path.Combine(openFolderDialog.SelectedPath, string.Format("{0} - {1} - {2}.png", Path.GetFileName(filePath), iconIndex, Convert.ToInt32(SelectedIconSize.Key))), ImageFormat.Png);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        saveFailedCount++;
                                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnExportAllIconsClicked), 1, e);
                                    }
                                }
                            }
                        }
                    });

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
            lock (iconExtractLock)
            {
                IconCollection.Clear();
            }

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
                    IntPtr[] phicon = new IntPtr[iconsNum];
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseIconFileAsync), 1, e);
                }

                return iconsList;
            });

            if (iconsList.Count > 0)
            {
                try
                {
                    GetResults = string.Format(GetResultsString, Path.GetFileName(filePath), iconsNum);
                    NoResources = string.Format(NoResourcesString, Path.GetFileName(filePath));
                    ImageSource = null;
                    IsImageEmpty = true;

                    foreach (IconModel iconItem in iconsList)
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(iconItem.IconMemoryStream.AsRandomAccessStream());

                        lock (iconExtractLock)
                        {
                            IconCollection.Add(new IconModel()
                            {
                                DisplayIndex = iconItem.DisplayIndex,
                                IconImage = bitmapImage
                            });
                        }

                        iconItem.IconMemoryStream.Dispose();
                    }

                    IconExtractResultKind = IconExtractResultKind.Successfully;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(ParseIconFileAsync), 2, e);
                }
            }
            else
            {
                GetResults = string.Format(GetResultsString, Path.GetFileName(filePath), 0);
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
        private bool SaveIcon(Icon rawIcon, string destination)
        {
            try
            {
                MemoryStream bitMapStream = new(); //存原图的内存流
                MemoryStream iconStream = new(); //存图标的内存流
                rawIcon.ToBitmap().Save(bitMapStream, ImageFormat.Png); //将原图读取为png格式并存入原图内存流
                BinaryWriter iconWriter = new(iconStream); //新建二进制写入器以写入目标图标内存流

                // 下面是根据原图信息，进行文件头写入
                iconWriter.Write((short)0);
                iconWriter.Write((short)1);
                iconWriter.Write((short)1);
                iconWriter.Write((byte)0); // 图片宽度，由于 bytes 最大数值为 255，图片大小为 256 时，数值异常
                iconWriter.Write((byte)0); // 图片高度，由于 bytes 最大数值为 255，图片大小为 256 时，数值异常
                iconWriter.Write((short)0);
                iconWriter.Write((short)0);
                iconWriter.Write((short)32);
                iconWriter.Write((int)bitMapStream.Length);
                iconWriter.Write(22);
                //写入图像体至目标图标内存流
                iconWriter.Write(bitMapStream.ToArray());
                //保存流，并将流指针定位至头部以Icon对象进行读取输出为文件
                iconWriter.Flush();
                iconWriter.Seek(0, SeekOrigin.Begin);
                FileStream iconFileStream = new(destination, FileMode.Create);
                Icon icon = new(iconStream);
                icon.Save(iconFileStream); //储存图像

                // 释放资源
                iconFileStream.Close();
                iconWriter.Close();
                iconStream.Close();
                bitMapStream.Close();
                icon.Dispose();
                return File.Exists(destination);
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(SaveIcon), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 检查图标提取是否成功
        /// </summary>
        private Visibility CheckIconExtractState(IconExtractResultKind iconResultResultKind, IconExtractResultKind comparedIconExtractResultKind)
        {
            return Equals(iconResultResultKind, comparedIconExtractResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
