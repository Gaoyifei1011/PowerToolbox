using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.Collections;
using PowerToolbox.Extensions.DataType.Enums;
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

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据校验页面
    /// </summary>
    public sealed partial class DataVertifyPage : Page, INotifyPropertyChanged
    {
        private readonly string BLAKE2spString = ResourceService.DataVertifyResource.GetString("BLAKE2sp");
        private readonly string BLAKE3String = ResourceService.DataVertifyResource.GetString("BLAKE3");
        private readonly string BTIHString = ResourceService.DataVertifyResource.GetString("BTIH");
        private readonly string ContentEmptyString = ResourceService.DataVertifyResource.GetString("ContentEmpty");
        private readonly string ContentInitializeString = ResourceService.DataVertifyResource.GetString("ContentInitialize");
        private readonly string ContentVertifyFailedString = ResourceService.DataVertifyResource.GetString("ContentVertifyFailed");
        private readonly string ContentVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyPartSuccessfully");
        private readonly string ContentVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyWholeSuccessfully");
        private readonly string CRC32String = ResourceService.DataVertifyResource.GetString("CRC32");
        private readonly string CRC64String = ResourceService.DataVertifyResource.GetString("CRC64");
        private readonly string ED2KString = ResourceService.DataVertifyResource.GetString("ED2K");
        private readonly string EDONR224String = ResourceService.DataVertifyResource.GetString("EDONR224");
        private readonly string EDONR256String = ResourceService.DataVertifyResource.GetString("EDONR256");
        private readonly string EDONR384String = ResourceService.DataVertifyResource.GetString("EDONR384");
        private readonly string EDONR512String = ResourceService.DataVertifyResource.GetString("EDONR512");
        private readonly string FileInitializeString = ResourceService.DataVertifyResource.GetString("FileInitialize");
        private readonly string FileNotExistedString = ResourceService.DataVertifyResource.GetString("FileNotExisted");
        private readonly string FileNotSelectedString = ResourceService.DataVertifyResource.GetString("FileNotSelected");
        private readonly string FileVertifyFailedString = ResourceService.DataVertifyResource.GetString("FileVertifyFailed");
        private readonly string FileVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyPartSuccessfully");
        private readonly string FileVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyWholeSuccessfully");
        private readonly string GOST12256String = ResourceService.DataVertifyResource.GetString("GOST12256");
        private readonly string GOST12512String = ResourceService.DataVertifyResource.GetString("GOST12512");
        private readonly string GOST94String = ResourceService.DataVertifyResource.GetString("GOST94");
        private readonly string GOST94CryptoProString = ResourceService.DataVertifyResource.GetString("GOST94CryptoPro");
        private readonly string HAS160String = ResourceService.DataVertifyResource.GetString("HAS160");
        private readonly string MD2String = ResourceService.DataVertifyResource.GetString("MD2");
        private readonly string MD4String = ResourceService.DataVertifyResource.GetString("MD4");
        private readonly string MD5String = ResourceService.DataVertifyResource.GetString("MD5");
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
        private readonly string SNEFRU128String = ResourceService.DataVertifyResource.GetString("SNEFRU128");
        private readonly string SNEFRU256String = ResourceService.DataVertifyResource.GetString("SNEFRU256");
        private readonly string TIGERString = ResourceService.DataVertifyResource.GetString("TIGER");
        private readonly string TIGER2String = ResourceService.DataVertifyResource.GetString("TIGER2");
        private readonly string TTHSString = ResourceService.DataVertifyResource.GetString("TTHS");
        private readonly string VertifyingString = ResourceService.DataVertifyResource.GetString("Vertifying");
        private readonly string VertifyTypeNotSelectedString = ResourceService.DataVertifyResource.GetString("VertifyTypeNotSelected");
        private readonly string WHIRLPOOLString = ResourceService.DataVertifyResource.GetString("WHIRLPOOL");
        private readonly string XXH128String = ResourceService.DataVertifyResource.GetString("XXH128");
        private readonly string XXH3String = ResourceService.DataVertifyResource.GetString("XXH3");
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
                Name = BLAKE2spString,
                DataVertifyType = DataVertifyType.BLAKE2sp
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = BLAKE3String,
                DataVertifyType = DataVertifyType.BLAKE3
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = BTIHString,
                DataVertifyType = DataVertifyType.BTIH
            });
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
                Name = EDONR224String,
                DataVertifyType = DataVertifyType.EDON_R_224
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR256String,
                DataVertifyType = DataVertifyType.EDON_R_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR384String,
                DataVertifyType = DataVertifyType.EDON_R_384
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR512String,
                DataVertifyType = DataVertifyType.EDON_R_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST12256String,
                DataVertifyType = DataVertifyType.GOST12_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST12512String,
                DataVertifyType = DataVertifyType.GOST12_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST94String,
                DataVertifyType = DataVertifyType.GOST94
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST94CryptoProString,
                DataVertifyType = DataVertifyType.GOST94CryptoPro
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = HAS160String,
                DataVertifyType = DataVertifyType.HAS_160
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
                Name = SNEFRU128String,
                DataVertifyType = DataVertifyType.SNEFRU_128
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SNEFRU256String,
                DataVertifyType = DataVertifyType.SNEFRU_256
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
                Name = TTHSString,
                DataVertifyType = DataVertifyType.TTH
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
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH3String,
                DataVertifyType = DataVertifyType.XXH3
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH128String,
                DataVertifyType = DataVertifyType.XXH128
            });
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
                FileStream fileStream = null;
                List<DataEncryptVertifyResultModel> dataVertifyResultList = [];

                try
                {
                    if (selectVertifyIndex is 0)
                    {
                        fileStream = File.OpenRead(selectedVertifyFile);
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

                if ((selectVertifyIndex is 0 && fileStream is not null) || (selectVertifyIndex is 1 && contentData is not null))
                {
                    List<Task> vertifyingTaskList = [];
                    object vertifyingLock = new();
                    foreach (DataVertifyTypeModel dataVertifyTypeItem in selectedDataVertifyTpyeList)
                    {
                        vertifyingTaskList.Add(Task.Run(() =>
                        {
                            string vertifyResultContent = GetVertifiedData(dataVertifyTypeItem.DataVertifyType, selectVertifyIndex, contentData, fileStream);
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

                if (fileStream is not null)
                {
                    try
                    {
                        fileStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnStartVertifyClicked), 2, e);
                    }
                }

                dataVertifyResultList.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));
                return dataVertifyResultList;
            });

            foreach (DataEncryptVertifyResultModel dataEncryptVertifyResultItem in dataVertifyResultList)
            {
                DataVertifyResultCollection.Add(dataEncryptVertifyResultItem);
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

        /// <summary>
        /// 获取校验后的数据
        /// </summary>
        /// TODO：未完成
        private string GetVertifiedData(DataVertifyType dataVertifyType, int selectedVertifyIndex, byte[] contentData, FileStream fileStream)
        {
            string vertifiedData = string.Empty;

            switch (dataVertifyType)
            {
                case DataVertifyType.BLAKE2sp:
                    {
                        break;
                    }
                case DataVertifyType.BLAKE3:
                    {
                        break;
                    }
                case DataVertifyType.BTIH:
                    {
                        break;
                    }
                case DataVertifyType.CRC_32:
                    {
                        break;
                    }
                case DataVertifyType.CRC_64:
                    {
                        break;
                    }
                case DataVertifyType.ED2K:
                    {
                        break;
                    }
                case DataVertifyType.EDON_R_224:
                    {
                        break;
                    }
                case DataVertifyType.EDON_R_256:
                    {
                        break;
                    }
                case DataVertifyType.EDON_R_384:
                    {
                        break;
                    }
                case DataVertifyType.EDON_R_512:
                    {
                        break;
                    }
                case DataVertifyType.GOST12_256:
                    {
                        break;
                    }
                case DataVertifyType.GOST12_512:
                    {
                        break;
                    }
                case DataVertifyType.GOST94:
                    {
                        break;
                    }
                case DataVertifyType.GOST94CryptoPro:
                    {
                        break;
                    }
                case DataVertifyType.HAS_160:
                    {
                        break;
                    }
                case DataVertifyType.MD2:
                    {
                        break;
                    }
                case DataVertifyType.MD4:
                    {
                        break;
                    }
                case DataVertifyType.MD5:
                    {
                        try
                        {
                            MD5 md5 = MD5.Create();
                            byte[] hashBytes = null;
                            if (selectVertifyIndex is 0 && fileStream is not null)
                            {
                                hashBytes = md5.ComputeHash(fileStream);
                            }
                            else if (selectVertifyIndex is 1 && contentData is not null)
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
                            if (selectVertifyIndex is 0 && fileStream is not null)
                            {
                                hashBytes = ripemd160.ComputeHash(fileStream);
                            }
                            else if (selectVertifyIndex is 1 && contentData is not null)
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
                            if (selectVertifyIndex is 0 && fileStream is not null)
                            {
                                hashBytes = sha1.ComputeHash(fileStream);
                            }
                            else if (selectVertifyIndex is 1 && contentData is not null)
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
                        break;
                    }
                case DataVertifyType.SHA_256:
                    {
                        try
                        {
                            SHA256 sha256 = SHA256.Create();
                            byte[] hashBytes = null;
                            if (selectVertifyIndex is 0 && fileStream is not null)
                            {
                                hashBytes = sha256.ComputeHash(fileStream);
                            }
                            else if (selectVertifyIndex is 1 && contentData is not null)
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
                            if (selectVertifyIndex is 0 && fileStream is not null)
                            {
                                hashBytes = sha384.ComputeHash(fileStream);
                            }
                            else if (selectVertifyIndex is 1 && contentData is not null)
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
                            if (selectVertifyIndex is 0 && fileStream is not null)
                            {
                                hashBytes = sha512.ComputeHash(fileStream);
                            }
                            else if (selectVertifyIndex is 1 && contentData is not null)
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
                        break;
                    }
                case DataVertifyType.SHA3_256:
                    {
                        break;
                    }
                case DataVertifyType.SHA3_384:
                    {
                        break;
                    }
                case DataVertifyType.SHA3_512:
                    {
                        break;
                    }
                case DataVertifyType.SM3:
                    {
                        break;
                    }
                case DataVertifyType.SNEFRU_128:
                    {
                        break;
                    }
                case DataVertifyType.SNEFRU_256:
                    {
                        break;
                    }
                case DataVertifyType.TIGER:
                    {
                        break;
                    }
                case DataVertifyType.TIGER2:
                    {
                        break;
                    }
                case DataVertifyType.TTH:
                    {
                        break;
                    }
                case DataVertifyType.WHIRLPOOL:
                    {
                        break;
                    }
                case DataVertifyType.XXH32:
                    {
                        break;
                    }
                case DataVertifyType.XXH64:
                    {
                        break;
                    }
                case DataVertifyType.XXH3:
                    {
                        break;
                    }
                case DataVertifyType.XXH128:
                    {
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
