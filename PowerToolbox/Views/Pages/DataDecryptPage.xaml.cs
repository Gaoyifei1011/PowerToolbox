using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1822，CA2022，IDE0060 警告
#pragma warning disable CA1822,CA2022,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据解密页面
    /// </summary>
    public sealed partial class DataDecryptPage : Page, INotifyPropertyChanged
    {
        private readonly string ContentInitializeString = ResourceService.DataDecryptResource.GetString("ContentInitialize");
        private readonly string DecryptingString = ResourceService.DataDecryptResource.GetString("Decrypting");
        private readonly string DragOverContentString = ResourceService.DataDecryptResource.GetString("DragOverContent");
        private readonly string FileInitializeString = ResourceService.DataDecryptResource.GetString("FileInitialize");
        private readonly string NoMultiFileString = ResourceService.DataDecryptResource.GetString("NoMultiFile");
        private readonly string SelectFileString = ResourceService.DataDecryptResource.GetString("SelectFile");

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        private string _decryptFile;

        public string DecryptFile
        {
            get { return _decryptFile; }

            set
            {
                if (!Equals(_decryptFile, value))
                {
                    _decryptFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptFile)));
                }
            }
        }

        private string _decryptContent;

        public string DecryptContent
        {
            get { return _decryptContent; }

            set
            {
                if (!Equals(_decryptContent, value))
                {
                    _decryptContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptContent)));
                }
            }
        }

        private InfoBarSeverity _resultSeverity;

        private InfoBarSeverity ResultSeverity
        {
            get { return _resultSeverity; }

            set
            {
                if (!Equals(_resultSeverity, value))
                {
                    _resultSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultSeverity)));
                }
            }
        }

        private string _resultMessage;

        public string ResultMessage
        {
            get { return _resultMessage; }

            set
            {
                if (!string.Equals(_resultMessage, value))
                {
                    _resultMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultMessage)));
                }
            }
        }

        private bool _isDecrypting;

        public bool IsDecrypting
        {
            get { return _isDecrypting; }

            set
            {
                if (!Equals(_isDecrypting, value))
                {
                    _isDecrypting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDecrypting)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DataDecryptPage()
        {
            InitializeComponent();
        }

        #region 第一部分：数据解密页面——挂载的事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        private async void OnDataDragOver(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IsDecrypting)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = DecryptingString;
                }
                else
                {
                    IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                    if (dragItemsList.Count is 1)
                    {
                        string extensionName = Path.GetExtension(dragItemsList[0].Name);

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
                        args.DragUIOverride.Caption = NoMultiFileString;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnDragOver), 1, e);
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
        private async void OnDataDrop(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnDrop), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                DecryptFile = filePath;
            }
        }

        /// <summary>
        /// 选中项发生改变时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && radioButtons.SelectedIndex >= 0)
            {
                SelectedIndex = radioButtons.SelectedIndex;
                if (!IsDecrypting && SelectedIndex is 0 && ResultSeverity is InfoBarSeverity.Informational)
                {
                    ResultMessage = FileInitializeString;
                }
                else if (!IsDecrypting && SelectedIndex is 1 && ResultSeverity is InfoBarSeverity.Informational)
                {
                    ResultMessage = ContentInitializeString;
                }
            }
        }

        /// <summary>
        /// 打开本地文件
        /// </summary>
        private void OnOpenLocalFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Title = SelectFileString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                DecryptFile = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 解密内容发生改变时触发的事件
        /// </summary>
        private void OnDecryptContentTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptContent = textBox.Text;
            }
        }

        #endregion 第一部分：数据解密页面——挂载的事件

        /// <summary>
        /// 获取要解密的类型
        /// </summary>
        private Visibility GetDataDecryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
