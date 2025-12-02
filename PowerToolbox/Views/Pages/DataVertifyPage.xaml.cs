using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.Hashing;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1822，CA2022，IDE0060 警告
#pragma warning disable CA1822,CA2022,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据校验页面
    /// </summary>
    public sealed partial class DataVertifyPage : Page, INotifyPropertyChanged
    {
        private readonly string ContentEmptyString = ResourceService.DataVertifyResource.GetString("ContentEmpty");
        private readonly string ContentInitializeString = ResourceService.DataVertifyResource.GetString("ContentInitialize");
        private readonly string ContentVertifyFailedString = ResourceService.DataVertifyResource.GetString("ContentVertifyFailed");
        private readonly string ContentVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyPartSuccessfully");
        private readonly string ContentVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyWholeSuccessfully");
        private readonly string CRC32String = ResourceService.DataVertifyResource.GetString("CRC32");
        private readonly string CRC64String = ResourceService.DataVertifyResource.GetString("CRC64");
        private readonly string DragOverContentString = ResourceService.DataVertifyResource.GetString("DragOverContent");
        private readonly string ED2KString = ResourceService.DataVertifyResource.GetString("ED2K");
        private readonly string FileInitializeString = ResourceService.DataVertifyResource.GetString("FileInitialize");
        private readonly string FileNotExistedString = ResourceService.DataVertifyResource.GetString("FileNotExisted");
        private readonly string FileNotSelectedString = ResourceService.DataVertifyResource.GetString("FileNotSelected");
        private readonly string FileVertifyFailedString = ResourceService.DataVertifyResource.GetString("FileVertifyFailed");
        private readonly string FileVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyPartSuccessfully");
        private readonly string FileVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyWholeSuccessfully");
        private readonly string MD2String = ResourceService.DataVertifyResource.GetString("MD2");
        private readonly string MD4String = ResourceService.DataVertifyResource.GetString("MD4");
        private readonly string MD5String = ResourceService.DataVertifyResource.GetString("MD5");
        private readonly string NoMultiFileString = ResourceService.DataVertifyResource.GetString("NoMultiFile");
        private readonly string RIPEMD160String = ResourceService.DataVertifyResource.GetString("RIPEMD160");
        private readonly string SelectFileString = ResourceService.DataVertifyResource.GetString("SelectFile");
        private readonly string SHA1String = ResourceService.DataVertifyResource.GetString("SHA1");
        private readonly string SHA224String = ResourceService.DataVertifyResource.GetString("SHA224");
        private readonly string SHA256String = ResourceService.DataVertifyResource.GetString("SHA256");
        private readonly string SHA3224String = ResourceService.DataVertifyResource.GetString("SHA3224");
        private readonly string SHA3256String = ResourceService.DataVertifyResource.GetString("SHA3256");
        private readonly string SHA3384String = ResourceService.DataVertifyResource.GetString("SHA3384");
        private readonly string SHA3512String = ResourceService.DataVertifyResource.GetString("SHA3512");
        private readonly string SHA384String = ResourceService.DataVertifyResource.GetString("SHA384");
        private readonly string SHA512String = ResourceService.DataVertifyResource.GetString("SHA512");
        private readonly string SM3String = ResourceService.DataVertifyResource.GetString("SM3");
        private readonly string TIGERString = ResourceService.DataVertifyResource.GetString("TIGER");
        private readonly string TIGER2String = ResourceService.DataVertifyResource.GetString("TIGER2");
        private readonly string VertifyingString = ResourceService.DataVertifyResource.GetString("Vertifying");
        private readonly string VertifyTypeNotSelectedString = ResourceService.DataVertifyResource.GetString("VertifyTypeNotSelected");
        private readonly string WHIRLPOOLString = ResourceService.DataVertifyResource.GetString("WHIRLPOOL");
        private readonly string XXH32String = ResourceService.DataVertifyResource.GetString("XXH32");
        private readonly string XXH64String = ResourceService.DataVertifyResource.GetString("XXH64");
        private int selectVertifyIndex = -1;
        private string selectedVertifyFile = string.Empty;
        private string selectedVertifyContent = string.Empty;

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

        private string _vertifyFile;

        public string VertifyFile
        {
            get { return _vertifyFile; }

            set
            {
                if (!Equals(_vertifyFile, value))
                {
                    _vertifyFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VertifyFile)));
                }
            }
        }

        private string _vertifyContent;

        public string VertifyContent
        {
            get { return _vertifyContent; }

            set
            {
                if (!Equals(_vertifyContent, value))
                {
                    _vertifyContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VertifyContent)));
                }
            }
        }

        private InfoBarSeverity _resultSeverity;

        private InfoBarSeverity ResultServerity
        {
            get { return _resultSeverity; }

            set
            {
                if (!Equals(_resultSeverity, value))
                {
                    _resultSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultServerity)));
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

        private bool _isAllSelected;

        public bool IsAllSelected
        {
            get { return _isAllSelected; }

            set
            {
                if (!Equals(_isAllSelected, value))
                {
                    _isAllSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAllSelected)));
                }
            }
        }

        private bool _useUpperCase;

        public bool UseUpperCase
        {
            get { return _useUpperCase; }

            set
            {
                if (!Equals(_useUpperCase, value))
                {
                    _useUpperCase = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseUpperCase)));
                }
            }
        }

        private bool _isVertifying;

        public bool IsVertifying
        {
            get { return _isVertifying; }

            set
            {
                if (!Equals(_isVertifying, value))
                {
                    _isVertifying = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVertifying)));
                }
            }
        }

        private List<DataVertifyTypeModel> DataVertifyTypeList { get; } = [];

        private WinRTObservableCollection<DataEncryptVertifyResultModel> DataVertifyResultCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataVertifyPage()
        {
            InitializeComponent();
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = CRC32String,
                DataVertifyType = DataVertifyType.CRC_32
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = CRC64String,
                DataVertifyType = DataVertifyType.CRC_64
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = ED2KString,
                DataVertifyType = DataVertifyType.ED2K
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = MD2String,
                DataVertifyType = DataVertifyType.MD2
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = MD4String,
                DataVertifyType = DataVertifyType.MD4
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = MD5String,
                DataVertifyType = DataVertifyType.MD5
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = RIPEMD160String,
                DataVertifyType = DataVertifyType.RIPEMD_160
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA1String,
                DataVertifyType = DataVertifyType.SHA_1
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA224String,
                DataVertifyType = DataVertifyType.SHA_224
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA256String,
                DataVertifyType = DataVertifyType.SHA_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA384String,
                DataVertifyType = DataVertifyType.SHA_384
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA512String,
                DataVertifyType = DataVertifyType.SHA_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3224String,
                DataVertifyType = DataVertifyType.SHA3_224
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3256String,
                DataVertifyType = DataVertifyType.SHA3_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3384String,
                DataVertifyType = DataVertifyType.SHA3_384
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3512String,
                DataVertifyType = DataVertifyType.SHA3_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SM3String,
                DataVertifyType = DataVertifyType.SM3
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = TIGERString,
                DataVertifyType = DataVertifyType.TIGER
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = TIGER2String,
                DataVertifyType = DataVertifyType.TIGER2
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = WHIRLPOOLString,
                DataVertifyType = DataVertifyType.WHIRLPOOL
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH32String,
                DataVertifyType = DataVertifyType.XXH32
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH64String,
                DataVertifyType = DataVertifyType.XXH64
            });
        }

        #region 第一部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 数据校验类型选中项发生改变时触发的事件
        /// </summary>
        private void OnDataVertifyCheckExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            IsAllSelected = DataVertifyTypeList.All(item => item.IsSelected);
        }

        #endregion 第一部分：ExecuteCommand 命令调用时挂载的事件

        #region 第二部分：数据校验页面——挂载的事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        private async void OnDataDragOver(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IsVertifying)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = VertifyingString;
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnDragOver), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                VertifyFile = filePath;
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
                if (SelectedIndex is 0 && ResultServerity is InfoBarSeverity.Informational)
                {
                    ResultMessage = FileInitializeString;
                }
                else if (SelectedIndex is 1 && ResultServerity is InfoBarSeverity.Informational)
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
                VertifyFile = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 校验内容发生改变时触发的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                VertifyContent = textBox.Text;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DataVertifyTypeModel dataVertifyTypeItem in DataVertifyTypeList)
            {
                dataVertifyTypeItem.IsSelected = true;
            }

            IsAllSelected = true;
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (DataVertifyTypeModel dataVertifyTypeItem in DataVertifyTypeList)
            {
                dataVertifyTypeItem.IsSelected = false;
            }

            IsAllSelected = false;
        }

        /// <summary>
        /// 输出格式为大写字母
        /// </summary>
        private void OnUseUpperCaseChecked(object sender, RoutedEventArgs args)
        {
            foreach (DataEncryptVertifyResultModel dataEncryptVertifyResultItem in DataVertifyResultCollection)
            {
                dataEncryptVertifyResultItem.Result = dataEncryptVertifyResultItem.Result.ToUpperInvariant();
            }
        }

        /// <summary>
        /// 输出格式为小写字母
        /// </summary>
        private void OnUseUpperCaseUnchecked(object sender, RoutedEventArgs args)
        {
            foreach (DataEncryptVertifyResultModel dataEncryptVertifyResultItem in DataVertifyResultCollection)
            {
                dataEncryptVertifyResultItem.Result = dataEncryptVertifyResultItem.Result.ToLowerInvariant();
            }
        }

        /// <summary>
        /// 开始数据校验
        /// </summary>
        private async void OnStartVertifyClicked(object sender, RoutedEventArgs args)
        {
            selectVertifyIndex = SelectedIndex;
            selectedVertifyFile = VertifyFile;
            selectedVertifyContent = VertifyContent;
            if (selectVertifyIndex is 0 && string.IsNullOrEmpty(selectedVertifyFile))
            {
                ResultServerity = InfoBarSeverity.Error;
                if (string.IsNullOrEmpty(selectedVertifyFile))
                {
                    ResultMessage = FileNotSelectedString;
                    return;
                }
                else if (!File.Exists(selectedVertifyFile))
                {
                    ResultMessage = FileNotExistedString;
                    return;
                }
                return;
            }
            else if (selectVertifyIndex is 1 && string.IsNullOrEmpty(selectedVertifyContent))
            {
                ResultServerity = InfoBarSeverity.Error;
                ResultMessage = ContentEmptyString;
            }

            List<DataVertifyTypeModel> selectedDataVertifyTpyeList = [.. DataVertifyTypeList.Where(item => item.IsSelected)];
            if (selectedDataVertifyTpyeList.Count is 0)
            {
                ResultServerity = InfoBarSeverity.Error;
                ResultMessage = VertifyTypeNotSelectedString;
                return;
            }
            IsVertifying = true;
            ResultServerity = InfoBarSeverity.Informational;
            ResultMessage = VertifyingString;
            DataVertifyResultCollection.Clear();
            List<DataEncryptVertifyResultModel> dataVertifyResultList = await Task.Run(async () =>
            {
                byte[] contentData = null;
                List<DataEncryptVertifyResultModel> dataVertifyResultList = [];

                try
                {
                    if (selectVertifyIndex is 0)
                    {
                        FileStream fileStream = File.OpenRead(selectedVertifyFile);
                        contentData = new byte[(int)fileStream.Length];
                        fileStream.Read(contentData, 0, contentData.Length);
                        fileStream.Dispose();
                    }
                    else if (selectVertifyIndex is 1)
                    {
                        contentData = Encoding.UTF8.GetBytes(selectedVertifyContent);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnStartVertifyClicked), 1, e);
                }

                if (contentData is not null && (selectVertifyIndex is 0 || selectVertifyIndex is 1))
                {
                    List<Task> vertifyingTaskList = [];
                    object vertifyingLock = new();
                    foreach (DataVertifyTypeModel dataVertifyTypeItem in selectedDataVertifyTpyeList)
                    {
                        vertifyingTaskList.Add(Task.Run(() =>
                        {
                            string vertifyResultContent = GetVertifiedData(dataVertifyTypeItem.DataVertifyType, selectVertifyIndex, contentData);
                            if (!string.IsNullOrEmpty(vertifyResultContent))
                            {
                                lock (vertifyingLock)
                                {
                                    dataVertifyResultList.Add(new DataEncryptVertifyResultModel()
                                    {
                                        Name = dataVertifyTypeItem.Name,
                                        Result = vertifyResultContent
                                    });
                                }
                            }
                        }));
                    }
                    await Task.WhenAll(vertifyingTaskList);
                }

                dataVertifyResultList.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));
                return dataVertifyResultList;
            });

            if (UseUpperCase)
            {
                foreach (DataEncryptVertifyResultModel dataEncryptVertifyResultItem in dataVertifyResultList)
                {
                    dataEncryptVertifyResultItem.Result = dataEncryptVertifyResultItem.Result.ToUpperInvariant();
                    DataVertifyResultCollection.Add(dataEncryptVertifyResultItem);
                }
            }
            else
            {
                foreach (DataEncryptVertifyResultModel dataEncryptVertifyResultItem in dataVertifyResultList)
                {
                    dataEncryptVertifyResultItem.Result = dataEncryptVertifyResultItem.Result.ToLowerInvariant();
                    DataVertifyResultCollection.Add(dataEncryptVertifyResultItem);
                }
            }

            if (DataVertifyResultCollection.Count > 0)
            {
                ResultServerity = InfoBarSeverity.Success;
                if (Equals(selectedDataVertifyTpyeList.Count, DataVertifyResultCollection.Count))
                {
                    if (selectVertifyIndex is 0)
                    {
                        ResultMessage = string.Format(FileVertifyWholeSuccessfullyString, DataVertifyResultCollection.Count);
                    }
                    else if (selectVertifyIndex is 1)
                    {
                        ResultMessage = string.Format(ContentVertifyWholeSuccessfullyString, DataVertifyResultCollection.Count);
                    }
                }
                else
                {
                    if (selectVertifyIndex is 0)
                    {
                        ResultMessage = string.Format(FileVertifyPartSuccessfullyString, DataVertifyResultCollection.Count, selectedDataVertifyTpyeList.Count - DataVertifyResultCollection.Count);
                    }
                    else if (selectVertifyIndex is 1)
                    {
                        ResultMessage = string.Format(ContentVertifyPartSuccessfullyString, DataVertifyResultCollection.Count, selectedDataVertifyTpyeList.Count - DataVertifyResultCollection.Count);
                    }
                }
            }
            else
            {
                ResultServerity = InfoBarSeverity.Error;
                if (selectVertifyIndex is 0)
                {
                    ResultMessage = FileVertifyFailedString;
                }
                else if (selectVertifyIndex is 1)
                {
                    ResultMessage = ContentVertifyFailedString;
                }
            }
            IsVertifying = false;
        }

        #endregion 第二部分：数据校验页面——挂载的事件

        /// <summary>
        /// 获取校验后的数据
        /// </summary>
        /// TODO：未完成
        private string GetVertifiedData(DataVertifyType dataVertifyType, int selectedVertifyIndex, byte[] contentData)
        {
            string vertifiedData = string.Empty;

            switch (dataVertifyType)
            {
                case DataVertifyType.CRC_32:
                    {
                        try
                        {
                            Crc32 crc32 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = crc32.ComputeHash(contentData);
                            }
                            crc32.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.CRC_32) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.CRC_64:
                    {
                        try
                        {
                            Crc64 crc64 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = crc64.ComputeHash(contentData);
                            }
                            crc64.Dispose();

                            if (hashBytes is not null)
                            {
                                vertifiedData = string.Format("{0:x}", BitConverter.ToUInt64(hashBytes, 0));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.CRC_64) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.ED2K:
                    {
                        try
                        {
                            ED2K ed2k = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = ed2k.ComputeHash(contentData);
                            }
                            ed2k.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.ED2K) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.MD2:
                    {
                        try
                        {
                            MD2 md2 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = md2.ComputeHash(contentData);
                            }
                            md2.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.MD2) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.MD4:
                    {
                        try
                        {
                            MD4 md4 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = md4.ComputeHash(contentData);
                            }
                            md4.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.MD4) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.MD5:
                    {
                        try
                        {
                            MD5 md5 = MD5.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = md5.ComputeHash(contentData);
                            }
                            md5.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.MD5) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.RIPEMD_160:
                    {
                        try
                        {
                            RIPEMD160 ripemd160 = RIPEMD160.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = ripemd160.ComputeHash(contentData);
                            }
                            ripemd160.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.RIPEMD_160) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA_1:
                    {
                        try
                        {
                            SHA1 sha1 = SHA1.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha1.ComputeHash(contentData);
                            }
                            sha1.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA_1) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA_224:
                    {
                        try
                        {
                            Sha224 sha224 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha224.ComputeHash(contentData);
                            }
                            sha224.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA_224) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA_256:
                    {
                        try
                        {
                            SHA256 sha256 = SHA256.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha256.ComputeHash(contentData);
                            }
                            sha256.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA_256) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA_384:
                    {
                        try
                        {
                            SHA384 sha384 = SHA384.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha384.ComputeHash(contentData);
                            }
                            sha384.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA_384) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA_512:
                    {
                        try
                        {
                            SHA512 sha512 = SHA512.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha512.ComputeHash(contentData);
                            }
                            sha512.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA_512) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA3_224:
                    {
                        try
                        {
                            Sha3_224 sha3_224 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_224.ComputeHash(contentData);
                            }
                            sha3_224.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA3_224) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA3_256:
                    {
                        try
                        {
                            Sha3_256 sha3_256 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_256.ComputeHash(contentData);
                            }
                            sha3_256.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA3_256) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA3_384:
                    {
                        try
                        {
                            Sha3_384 sha3_384 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_384.ComputeHash(contentData);
                            }
                            sha3_384.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA3_384) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SHA3_512:
                    {
                        try
                        {
                            Sha3_512 sha3_512 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_512.ComputeHash(contentData);
                            }
                            sha3_512.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SHA3_512) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.SM3:
                    {
                        try
                        {
                            SM3 sm3 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sm3.ComputeHash(contentData);
                            }
                            sm3.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.SM3) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.TIGER:
                    {
                        try
                        {
                            Tiger tiger = new(1);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = tiger.ComputeHash(contentData);
                            }
                            tiger.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.TIGER) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.TIGER2:
                    {
                        try
                        {
                            Tiger tiger = new(128);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = tiger.ComputeHash(contentData);
                            }
                            tiger.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.TIGER2) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.WHIRLPOOL:
                    {
                        try
                        {
                            Whirlpool whirlpool = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = whirlpool.ComputeHash(contentData);
                            }
                            whirlpool.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.WHIRLPOOL) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.XXH32:
                    {
                        try
                        {
                            XxHash32 xxHash32 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = xxHash32.ComputeHash(contentData);
                            }
                            xxHash32.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.XXH32) + 1, e);
                        }
                        break;
                    }
                case DataVertifyType.XXH64:
                    {
                        try
                        {
                            XXHash64 xxhash64 = new(0);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = xxhash64.ComputeHash(contentData);
                            }
                            xxhash64.Dispose();

                            if (hashBytes is not null)
                            {
                                StringBuilder stringBuilder = new();
                                foreach (byte b in hashBytes)
                                {
                                    stringBuilder.Append(b.ToString("x2"));
                                }
                                vertifiedData = Convert.ToString(stringBuilder);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(GetVertifiedData), Convert.ToInt32(DataVertifyType.XXH64) + 1, e);
                        }
                        break;
                    }
            }

            return vertifiedData;
        }

        /// <summary>
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataVertifyType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取校验结果列表显示类型
        /// </summary>
        private Visibility GetVertifyResult(bool isVertifying, int vertifyCount)
        {
            return isVertifying ? Visibility.Collapsed : vertifyCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
