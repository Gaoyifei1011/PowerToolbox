using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// 数据解密页面
    /// </summary>
    public sealed partial class DataDecryptPage : Page, INotifyPropertyChanged
    {
        private readonly string AESString = ResourceService.DataDecryptResource.GetString("AES");
        private readonly string Base64String = ResourceService.DataDecryptResource.GetString("Base64");
        private readonly string CaesarCipherString = ResourceService.DataDecryptResource.GetString("CaesarCipher");
        private readonly string ChaCha20String = ResourceService.DataDecryptResource.GetString("ChaCha20");
        private readonly string ContentInitializeString = ResourceService.DataDecryptResource.GetString("ContentInitialize");
        private readonly string DecryptingString = ResourceService.DataDecryptResource.GetString("Decrypting");
        private readonly string DecryptKey162432SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey162432Size");
        private readonly string DecryptKey1624SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey1624Size");
        private readonly string DecryptKey16SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey16Size");
        private readonly string DecryptKey32SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey32Size");
        private readonly string DecryptKey8SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey8Size");
        private readonly string DecryptKeyEmptyString = ResourceService.DataDecryptResource.GetString("DecryptKeyEmpty");
        private readonly string DESString = ResourceService.DataDecryptResource.GetString("DES");
        private readonly string DragOverContentString = ResourceService.DataDecryptResource.GetString("DragOverContent");
        private readonly string FileInitializeString = ResourceService.DataDecryptResource.GetString("FileInitialize");
        private readonly string InitializationVector12SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector12Size");
        private readonly string InitializationVector16SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector16Size");
        private readonly string InitializationVector8SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector8Size");
        private readonly string InitializationVectorEmptyString = ResourceService.DataDecryptResource.GetString("InitializationVectorEmpty");
        private readonly string MorseCodeString = ResourceService.DataDecryptResource.GetString("MorseCode");
        private readonly string NoMultiFileString = ResourceService.DataDecryptResource.GetString("NoMultiFile");
        private readonly string RabbitString = ResourceService.DataDecryptResource.GetString("Rabbit");
        private readonly string RC2String = ResourceService.DataDecryptResource.GetString("RC2");
        private readonly string RC4String = ResourceService.DataDecryptResource.GetString("RC4");
        private readonly string RC5String = ResourceService.DataDecryptResource.GetString("RC5");
        private readonly string RC6String = ResourceService.DataDecryptResource.GetString("RC6");
        private readonly string RijndaelString = ResourceService.DataDecryptResource.GetString("Rijndael");
        private readonly string RSAString = ResourceService.DataDecryptResource.GetString("RSA");
        private readonly string SelectFileString = ResourceService.DataDecryptResource.GetString("SelectFile");
        private readonly string SM4String = ResourceService.DataDecryptResource.GetString("SM4");
        private readonly string StringLengthString = ResourceService.DataDecryptResource.GetString("StringLength");
        private readonly string TripleDESString = ResourceService.DataDecryptResource.GetString("TripleDES");
        private readonly string UTF8String = ResourceService.DataDecryptResource.GetString("UTF8");
        private readonly string XORString = ResourceService.DataDecryptResource.GetString("XOR");

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

        private string _decryptFailedInformation;

        public string DecryptFailedInformation
        {
            get { return _decryptFailedInformation; }

            set
            {
                if (!string.Equals(_decryptFailedInformation, value))
                {
                    _decryptFailedInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptFailedInformation)));
                }
            }
        }

        private DataDecryptTypeModel _selectedDataDecryptType;

        public DataDecryptTypeModel SelectedDataDecryptType
        {
            get { return _selectedDataDecryptType; }

            set
            {
                if (!Equals(_selectedDataDecryptType, value))
                {
                    _selectedDataDecryptType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDataDecryptType)));
                }
            }
        }

        private bool _hasDecryptOptions;

        public bool HasDecryptOptions
        {
            get { return _hasDecryptOptions; }

            set
            {
                if (!Equals(_hasDecryptOptions, value))
                {
                    _hasDecryptOptions = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptOptions)));
                }
            }
        }

        private bool _hasDecryptKey;

        public bool HasDecryptKey
        {
            get { return _hasDecryptKey; }

            set
            {
                if (!Equals(_hasDecryptKey, value))
                {
                    _hasDecryptKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptKey)));
                }
            }
        }

        private string _decryptKeyPHText = string.Empty;

        public string DecryptKeyPHText
        {
            get { return _decryptKeyPHText; }

            set
            {
                if (!string.Equals(_decryptKeyPHText, value))
                {
                    _decryptKeyPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptKeyPHText)));
                }
            }
        }

        private string _decryptKeyText = string.Empty;

        public string DecryptKeyText
        {
            get { return _decryptKeyText; }

            set
            {
                if (!string.Equals(_decryptKeyText, value))
                {
                    _decryptKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptKeyText)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedDecryptKeyStringType;

        public KeyValuePair<string, string> SelectedDecryptKeyStringType
        {
            get { return _selectedDecryptKeyStringType; }

            set
            {
                if (!Equals(_selectedDecryptKeyStringType, value))
                {
                    _selectedDecryptKeyStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDecryptKeyStringType)));
                }
            }
        }

        private bool _hasInitializationVector;

        public bool HasInitializationVector
        {
            get { return _hasInitializationVector; }

            set
            {
                if (!Equals(_hasInitializationVector, value))
                {
                    _hasInitializationVector = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasInitializationVector)));
                }
            }
        }

        private string _initializationVectorPHText = string.Empty;

        public string InitializationVectorPHText
        {
            get { return _initializationVectorPHText; }

            set
            {
                if (!string.Equals(_initializationVectorPHText, value))
                {
                    _initializationVectorPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InitializationVectorPHText)));
                }
            }
        }

        private string _initializationVectorText = string.Empty;

        public string InitializationVectorText
        {
            get { return _initializationVectorText; }

            set
            {
                if (!string.Equals(_initializationVectorText, value))
                {
                    _initializationVectorText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InitializationVectorText)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedInitializationVectorStringType;

        public KeyValuePair<string, string> SelectedInitializationVectorStringType
        {
            get { return _selectedInitializationVectorStringType; }

            set
            {
                if (!Equals(_selectedInitializationVectorStringType, value))
                {
                    _selectedInitializationVectorStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedInitializationVectorStringType)));
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

        private string _decryptResult;

        public string DecryptResult
        {
            get { return _decryptResult; }

            set
            {
                if (!string.Equals(_decryptResult, value))
                {
                    _decryptResult = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptResult)));
                }
            }
        }

        private List<DataDecryptTypeModel> DataDecryptTypeList { get; } = [];

        private List<KeyValuePair<string, string>> DecryptKeyStringTypeList { get; } = [];

        private List<KeyValuePair<string, string>> InitializationVectorStringTypeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataDecryptPage()
        {
            InitializeComponent();
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.AES,
                Name = AESString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.CaesarCipher,
                Name = CaesarCipherString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.ChaCha20,
                Name = ChaCha20String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.DES,
                Name = DESString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.MorseCode,
                Name = MorseCodeString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.Rabbit,
                Name = RabbitString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC2,
                Name = RC2String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC4,
                Name = RC4String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC5,
                Name = RC5String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC6,
                Name = RC6String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.Rijndael,
                Name = RijndaelString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RSA,
                Name = RSAString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.SM4,
                Name = SM4String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.TripleDES,
                Name = TripleDESString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.XOR,
                Name = XORString
            });
            SelectedDataDecryptType = DataDecryptTypeList[0];
            DecryptKeyStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            DecryptKeyStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
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

        /// <summary>
        /// 复制错误原因
        /// </summary>
        private async void OnCopyErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            if (ViewErrorInformationFlyout.IsOpen)
            {
                ViewErrorInformationFlyout.Hide();
            }

            if (!string.IsNullOrEmpty(DecryptFailedInformation))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(DecryptFailedInformation);

                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if (ViewErrorInformationFlyout.IsOpen)
            {
                ViewErrorInformationFlyout.Hide();
            }
        }

        /// <summary>
        /// 数据解密类型选中项发生改变时触发的事件
        /// </summary>
        private void OnDecryptSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is DataDecryptTypeModel dataDecryptType)
            {
                SelectedDataDecryptType = dataDecryptType;
                DecryptKeyText = string.Empty;
                //InitializationVectorText = string.Empty;

                switch (SelectedDataDecryptType.DataDecryptType)
                {
                    case DataDecryptType.AES:
                        {
                            DecryptKeyPHText = DecryptKey162432SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.CaesarCipher:
                        {
                            SelectedIndex = 1;
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = false;
                            //HasInitializationVector = false;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = false;
                            //HasOffset = true;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.ChaCha20:
                        {
                            DecryptKeyPHText = DecryptKey32SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector12SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = true;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = false;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.DES:
                        {
                            DecryptKeyPHText = DecryptKey8SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.MorseCode:
                        {
                            SelectedIndex = 1;
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = false;
                            HasDecryptKey = false;
                            //HasInitializationVector = false;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = false;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.Rabbit:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = true;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC2:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC4:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = false;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = false;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC5:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC6:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.Rijndael:
                        {
                            DecryptKeyPHText = DecryptKey162432SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RSA:
                        {
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = false;
                            //HasInitializationVector = false;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = false;
                            //HasOffset = false;
                            //HasDecryptPublicKey = true;
                            //HasDecryptPrivateKey = true;
                            //HasRSADecryptionPaddingMode = true;
                            //HasRSADecryptionOtherOptions = true;
                            break;
                        }
                    case DataDecryptType.SM4:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.TripleDES:
                        {
                            DecryptKeyPHText = DecryptKey1624SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            //HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            //HasDecryptedBlockCipherMode = true;
                            //HasPaddingMode = true;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.XOR:
                        {
                            SelectedIndex = 1;
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            //SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            //SelectedPaddingMode = PaddingModeList[0];
                            //SelectedRSADecryptionPaddingMode = RSADecryptionPaddingModeList[0];
                            //Offset = 0;
                            //DecryptPublicKeyText = string.Empty;
                            //DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = false;
                            //HasDecryptedBlockCipherMode = false;
                            //HasPaddingMode = false;
                            //HasOffset = false;
                            //HasDecryptPublicKey = false;
                            //HasDecryptPrivateKey = false;
                            //HasRSADecryptionPaddingMode = false;
                            //HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 解密密钥字符串编码模式发生变化时触发的事件
        /// </summary>
        private void OnDecryptKeyStringTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> encryptKeyStringType)
            {
                SelectedDecryptKeyStringType = encryptKeyStringType;
            }
        }

        /// <summary>
        /// 解密密钥内容发生改变时触发的事件
        /// </summary>
        private void OnDecryptKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptKeyText = textBox.Text;
            }
        }

        /// <summary>
        /// 初始化向量字符串编码模式发生变化时触发的事件
        /// </summary>
        private void OnInitializationVectorStringTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> initializationVectorStringType)
            {
                SelectedInitializationVectorStringType = initializationVectorStringType;
            }
        }

        /// <summary>
        /// 初始化向量内容改变时触发的事件
        /// </summary>
        private void OnInitializationVectorTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                InitializationVectorText = textBox.Text;
            }
        }

        /// <summary>
        /// 输出格式为大写字母
        /// </summary>
        private void OnUseUpperCaseChecked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(DecryptResult))
            {
                DecryptResult = DecryptResult.ToUpperInvariant();
            }
        }

        /// <summary>
        /// 输出格式为小写字母
        /// </summary>
        private void OnUseUpperCaseUnchecked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(DecryptResult))
            {
                DecryptResult = DecryptResult.ToLowerInvariant();
            }
        }

        /// <summary>
        /// 开始数据解密
        /// </summary>
        private async void OnStartDecryptClicked(object sender, RoutedEventArgs args)
        {
            // TODO：未完成
        }

        #endregion 第一部分：数据解密页面——挂载的事件

        /// <summary>
        /// 获取要解密的类型
        /// </summary>
        private Visibility GetDataDecryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetDecryptInfoButtonHitTestState(InfoBarSeverity resultSeverity, bool isDecrypting)
        {
            return resultSeverity is InfoBarSeverity.Error && !isDecrypting;
        }

        private Visibility GetDecryptInfoButtonVisibility(InfoBarSeverity resultSeverity)
        {
            return resultSeverity is InfoBarSeverity.Informational || resultSeverity is InfoBarSeverity.Error ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetDecryptInfoProgressRingActiveState(InfoBarSeverity resultSeverity, bool isDecrypting)
        {
            return resultSeverity is InfoBarSeverity.Informational && isDecrypting;
        }

        private Visibility GetDecryptFailedInfoButtonState(InfoBarSeverity resultSeverity, bool isDecrypting, string decryptFailedInformation)
        {
            return resultSeverity is InfoBarSeverity.Error && !isDecrypting && !string.IsNullOrEmpty(decryptFailedInformation) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
