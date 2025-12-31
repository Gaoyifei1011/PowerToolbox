using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.Encrypt;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1806，CA1822，CA2022，IDE0060 警告
#pragma warning disable CA1806,CA1822,CA2022,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据解密页面
    /// </summary>
    public sealed partial class DataDecryptPage : Page, INotifyPropertyChanged
    {
        private readonly string AllFilesString = ResourceService.DataDecryptResource.GetString("AllFiles");
        private readonly string ASCIIString = ResourceService.DataDecryptResource.GetString("ASCII");
        private readonly string AESString = ResourceService.DataDecryptResource.GetString("AES");
        private readonly string ANSIX923String = ResourceService.DataDecryptResource.GetString("ANSIX923");
        private readonly string Base64String = ResourceService.DataDecryptResource.GetString("Base64");
        private readonly string BigEndianUnicodeString = ResourceService.DataDecryptResource.GetString("BigEndianUnicode");
        private readonly string BinaryFileString = ResourceService.DataDecryptResource.GetString("BinaryFile");
        private readonly string CaesarCipherString = ResourceService.DataDecryptResource.GetString("CaesarCipher");
        private readonly string CBCString = ResourceService.DataDecryptResource.GetString("CBC");
        private readonly string CFBString = ResourceService.DataDecryptResource.GetString("CFB");
        private readonly string ChaCha20String = ResourceService.DataDecryptResource.GetString("ChaCha20");
        private readonly string ContentDecryptedDataSaveFailedString = ResourceService.DataDecryptResource.GetString("ContentDecryptedDataSaveFailed");
        private readonly string ContentDecryptedDataSaveFailedToTempFileString = ResourceService.DataDecryptResource.GetString("ContentDecryptedDataSaveFailedToTempFile");
        private readonly string ContentDecryptedDataSaveSuccessfullyString = ResourceService.DataDecryptResource.GetString("ContentDecryptedDataSaveSuccessfully");
        private readonly string ContentDecryptFailedString = ResourceService.DataDecryptResource.GetString("ContentDecryptFailed");
        private readonly string ContentDecryptSuccessfullyString = ResourceService.DataDecryptResource.GetString("ContentDecryptSuccessfully");
        private readonly string ContentEmptyString = ResourceService.DataDecryptResource.GetString("ContentEmpty");
        private readonly string ContentInitializeString = ResourceService.DataDecryptResource.GetString("ContentInitialize");
        private readonly string CTSString = ResourceService.DataDecryptResource.GetString("CTS");
        private readonly string CustomString = ResourceService.DataDecryptResource.GetString("Custom");
        private readonly string DecryptingString = ResourceService.DataDecryptResource.GetString("Decrypting");
        private readonly string DecryptKey162432SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey162432Size");
        private readonly string DecryptKey1624SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey1624Size");
        private readonly string DecryptKey16SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey16Size");
        private readonly string DecryptKey32SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey32Size");
        private readonly string DecryptKey8SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey8Size");
        private readonly string DecryptKeyEmptyString = ResourceService.DataDecryptResource.GetString("DecryptKeyEmpty");
        private readonly string DecryptKeyInitializeString = ResourceService.DataDecryptResource.GetString("DecryptKeyInitialize");
        private readonly string DecryptPrivateKeyEmptyString = ResourceService.DataDecryptResource.GetString("DecryptPrivateKeyEmpty");
        private readonly string DecryptTypeMustContentString = ResourceService.DataDecryptResource.GetString("DecryptTypeMustContent");
        private readonly string DecryptTypeNotSelectedString = ResourceService.DataDecryptResource.GetString("DecryptTypeNotSelected");
        private readonly string DESString = ResourceService.DataDecryptResource.GetString("DES");
        private readonly string DragOverContentString = ResourceService.DataDecryptResource.GetString("DragOverContent");
        private readonly string ECBString = ResourceService.DataDecryptResource.GetString("ECB");
        private readonly string FileDecryptedDataSaveFailedString = ResourceService.DataDecryptResource.GetString("FileDecryptedDataSaveFailed");
        private readonly string FileDecryptedDataSaveFailedToTempFileString = ResourceService.DataDecryptResource.GetString("FileDecryptedDataSaveFailedToTempFile");
        private readonly string FileDecryptedDataSaveSuccessfullyString = ResourceService.DataDecryptResource.GetString("FileDecryptedDataSaveSuccessfully");
        private readonly string FileDecryptFailedString = ResourceService.DataDecryptResource.GetString("FileDecryptFailed");
        private readonly string FileDecryptSuccessfullyString = ResourceService.DataDecryptResource.GetString("FileDecryptSuccessfully");
        private readonly string FileInitializeString = ResourceService.DataDecryptResource.GetString("FileInitialize");
        private readonly string FileNotExistedString = ResourceService.DataDecryptResource.GetString("FileNotExisted");
        private readonly string FileNotSelectedString = ResourceService.DataDecryptResource.GetString("FileNotSelected");
        private readonly string FileNotTextContentString = ResourceService.DataDecryptResource.GetString("FileNotTextContent");
        private readonly string GB18030String = ResourceService.DataDecryptResource.GetString("GB18030");
        private readonly string GB2312String = ResourceService.DataDecryptResource.GetString("GB2312");
        private readonly string GBKString = ResourceService.DataDecryptResource.GetString("GBK");
        private readonly string InitializationVector12SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector12Size");
        private readonly string InitializationVector16SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector16Size");
        private readonly string InitializationVector8SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector8Size");
        private readonly string InitializationVectorEmptyString = ResourceService.DataDecryptResource.GetString("InitializationVectorEmpty");
        private readonly string ISO10126String = ResourceService.DataDecryptResource.GetString("ISO10126");
        private readonly string ISO88591String = ResourceService.DataDecryptResource.GetString("ISO88591");
        private readonly string LargeContentString = ResourceService.DataDecryptResource.GetString("LargeContent");
        private readonly string MorseCodeString = ResourceService.DataDecryptResource.GetString("MorseCode");
        private readonly string NoMultiFileString = ResourceService.DataDecryptResource.GetString("NoMultiFile");
        private readonly string NonePaddingString = ResourceService.DataDecryptResource.GetString("NonePadding");
        private readonly string OaepString = ResourceService.DataDecryptResource.GetString("Oaep");
        private readonly string OFBString = ResourceService.DataDecryptResource.GetString("OFB");
        private readonly string Pkcs1String = ResourceService.DataDecryptResource.GetString("Pkcs1");
        private readonly string Pkcs7String = ResourceService.DataDecryptResource.GetString("Pkcs7");
        private readonly string RabbitString = ResourceService.DataDecryptResource.GetString("Rabbit");
        private readonly string RC2String = ResourceService.DataDecryptResource.GetString("RC2");
        private readonly string RC4String = ResourceService.DataDecryptResource.GetString("RC4");
        private readonly string RC5String = ResourceService.DataDecryptResource.GetString("RC5");
        private readonly string RC6String = ResourceService.DataDecryptResource.GetString("RC6");
        private readonly string RijndaelString = ResourceService.DataDecryptResource.GetString("Rijndael");
        private readonly string RSAString = ResourceService.DataDecryptResource.GetString("RSA");
        private readonly string SelectedDataDecryptInputContentFormatErrorString = ResourceService.DataDecryptResource.GetString("SelectedDataDecryptInputContentFormatError");
        private readonly string SelectFileString = ResourceService.DataDecryptResource.GetString("SelectFile");
        private readonly string SM4String = ResourceService.DataDecryptResource.GetString("SM4");
        private readonly string StringLengthString = ResourceService.DataDecryptResource.GetString("StringLength");
        private readonly string TextEncodingInvalidString = ResourceService.DataDecryptResource.GetString("TextEncodingInvalid");
        private readonly string TripleDESString = ResourceService.DataDecryptResource.GetString("TripleDES");
        private readonly string UnicodeString = ResourceService.DataDecryptResource.GetString("Unicode");
        private readonly string UnknownErrorString = ResourceService.DataDecryptResource.GetString("UnknownError");
        private readonly string UTF32String = ResourceService.DataDecryptResource.GetString("UTF32");
        private readonly string UTF7String = ResourceService.DataDecryptResource.GetString("UTF7");
        private readonly string UTF8String = ResourceService.DataDecryptResource.GetString("UTF8");
        private readonly string XORString = ResourceService.DataDecryptResource.GetString("XOR");
        private readonly string ZerosString = ResourceService.DataDecryptResource.GetString("Zeros");
        private int selectDecryptIndex = -1;
        private string selectedDecryptFile = string.Empty;
        private string inputtedDecryptContent = string.Empty;
        private string decryptedLocalFile = string.Empty;

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

        private bool _hasDecryptKeyStringType;

        public bool HasDecryptKeyStringType
        {
            get { return _hasDecryptKeyStringType; }

            set
            {
                if (!Equals(_hasDecryptKeyStringType, value))
                {
                    _hasDecryptKeyStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptKeyStringType)));
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

        private bool _hasDecryptedBlockCipherMode;

        public bool HasDecryptedBlockCipherMode
        {
            get { return _hasDecryptedBlockCipherMode; }

            set
            {
                if (!Equals(_hasDecryptedBlockCipherMode, value))
                {
                    _hasDecryptedBlockCipherMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptedBlockCipherMode)));
                }
            }
        }

        private KeyValuePair<CipherMode, string> _selectedDecryptedBlockCipherMode;

        public KeyValuePair<CipherMode, string> SelectedDecryptedBlockCipherMode
        {
            get { return _selectedDecryptedBlockCipherMode; }

            set
            {
                if (!Equals(_selectedDecryptedBlockCipherMode, value))
                {
                    _selectedDecryptedBlockCipherMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDecryptedBlockCipherMode)));
                }
            }
        }

        private bool _hasPaddingMode;

        public bool HasPaddingMode
        {
            get { return _hasPaddingMode; }

            set
            {
                if (!Equals(_hasPaddingMode, value))
                {
                    _hasPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasPaddingMode)));
                }
            }
        }

        private KeyValuePair<PaddingMode, string> _selectedPaddingMode;

        public KeyValuePair<PaddingMode, string> SelectedPaddingMode
        {
            get { return _selectedPaddingMode; }

            set
            {
                if (!Equals(_selectedPaddingMode, value))
                {
                    _selectedPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPaddingMode)));
                }
            }
        }

        private bool _hasOffset;

        public bool HasOffset
        {
            get { return _hasOffset; }

            set
            {
                if (!Equals(_hasOffset, value))
                {
                    _hasOffset = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasOffset)));
                }
            }
        }

        private int _offset;

        public int Offset
        {
            get { return _offset; }

            set
            {
                if (!Equals(_offset, value))
                {
                    _offset = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offset)));
                }
            }
        }

        private bool _hasDecryptPrivateKey;

        public bool HasDecryptPrivateKey
        {
            get { return _hasDecryptPrivateKey; }

            set
            {
                if (!Equals(_hasDecryptPrivateKey, value))
                {
                    _hasDecryptPrivateKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptPrivateKey)));
                }
            }
        }

        private string _decryptPrivateKeyText;

        public string DecryptPrivateKeyText
        {
            get { return _decryptPrivateKeyText; }

            set
            {
                if (!Equals(_decryptPrivateKeyText, value))
                {
                    _decryptPrivateKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptPrivateKeyText)));
                }
            }
        }

        private bool _hasRSAEncryptionPaddingMode;

        public bool HasRSAEncryptionPaddingMode
        {
            get { return _hasRSAEncryptionPaddingMode; }

            set
            {
                if (!Equals(_hasRSAEncryptionPaddingMode, value))
                {
                    _hasRSAEncryptionPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasRSAEncryptionPaddingMode)));
                }
            }
        }

        private KeyValuePair<RSAEncryptionPaddingMode, string> _selectedRSAEncryptionPaddingMode;

        public KeyValuePair<RSAEncryptionPaddingMode, string> SelectedRSAEncryptionPaddingMode
        {
            get { return _selectedRSAEncryptionPaddingMode; }

            set
            {
                if (!Equals(_selectedRSAEncryptionPaddingMode, value))
                {
                    _selectedRSAEncryptionPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRSAEncryptionPaddingMode)));
                }
            }
        }

        private bool _hasParseAsTextData;

        public bool HasParseAsTextData
        {
            get { return _hasParseAsTextData; }

            set
            {
                if (!Equals(_hasParseAsTextData, value))
                {
                    _hasParseAsTextData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasParseAsTextData)));
                }
            }
        }

        private bool _parseAsTextData;

        public bool ParseAsTextData
        {
            get { return _parseAsTextData; }

            set
            {
                if (!Equals(_parseAsTextData, value))
                {
                    _parseAsTextData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParseAsTextData)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedTextEncodingType;

        public KeyValuePair<string, string> SelectedTextEncodingType
        {
            get { return _selectedTextEncodingType; }

            set
            {
                if (!Equals(_selectedTextEncodingType, value))
                {
                    _selectedTextEncodingType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTextEncodingType)));
                }
            }
        }

        private string _textDecodingCustomTypeText;

        public string TextEncodingCustomTypeText
        {
            get { return _textDecodingCustomTypeText; }

            set
            {
                if (!Equals(_textDecodingCustomTypeText, value))
                {
                    _textDecodingCustomTypeText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextEncodingCustomTypeText)));
                }
            }
        }

        private bool _saveDecryptedDataToLocalFile;

        public bool SaveDecryptedDataToLocalFile
        {
            get { return _saveDecryptedDataToLocalFile; }

            set
            {
                if (!Equals(_saveDecryptedDataToLocalFile, value))
                {
                    _saveDecryptedDataToLocalFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveDecryptedDataToLocalFile)));
                }
            }
        }

        private string _saveDecryptedFilePath = string.Empty;

        public string SaveDecryptedFilePath
        {
            get { return _saveDecryptedFilePath; }

            set
            {
                if (!Equals(_saveDecryptedFilePath, value))
                {
                    _saveDecryptedFilePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveDecryptedFilePath)));
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

        private bool _isLargeContent;

        public bool IsLargeContent
        {
            get { return _isLargeContent; }

            set
            {
                if (!Equals(_isLargeContent, value))
                {
                    _isLargeContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLargeContent)));
                }
            }
        }

        private bool _isBinaryFile;

        public bool IsBinaryFile
        {
            get { return _isBinaryFile; }

            set
            {
                if (!Equals(_isBinaryFile, value))
                {
                    _isBinaryFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBinaryFile)));
                }
            }
        }

        private List<DataDecryptTypeModel> DataDecryptTypeList { get; } = [];

        private List<KeyValuePair<string, string>> DecryptKeyStringTypeList { get; } = [];

        private List<KeyValuePair<string, string>> InitializationVectorStringTypeList { get; } = [];

        private List<KeyValuePair<CipherMode, string>> DecryptedBlockCipherModeList { get; } = [];

        private List<KeyValuePair<PaddingMode, string>> PaddingModeList { get; } = [];

        private List<KeyValuePair<RSAEncryptionPaddingMode, string>> RSAEncryptionPaddingModeList { get; } = [];

        private List<KeyValuePair<string, string>> TextEncodingTypeList { get; } = [];

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
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CBC, CBCString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.ECB, ECBString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.OFB, OFBString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CFB, CFBString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CTS, CTSString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.None, NonePaddingString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.PKCS7, Pkcs7String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.Zeros, ZerosString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ANSIX923, ANSIX923String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ISO10126, ISO10126String));
            RSAEncryptionPaddingModeList.Add(new KeyValuePair<RSAEncryptionPaddingMode, string>(RSAEncryptionPaddingMode.Pkcs1, Pkcs1String));
            RSAEncryptionPaddingModeList.Add(new KeyValuePair<RSAEncryptionPaddingMode, string>(RSAEncryptionPaddingMode.Oaep, OaepString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.ASCII), ASCIIString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.BigEndianUnicode), BigEndianUnicodeString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("ISO-8859-1", ISO88591String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("GB18030", GB18030String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("GB2312", GB2312String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("GBK", GBKString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.Unicode), UnicodeString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF32), UTF32String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF7), UTF7String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("Custom", CustomString));
            SelectedTextEncodingType = TextEncodingTypeList[9];
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
                InitializationVectorText = string.Empty;

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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
                            break;
                        }
                    case DataDecryptType.CaesarCipher:
                        {
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = false;
                            HasDecryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = true;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = false;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = true;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = false;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
                            break;
                        }
                    case DataDecryptType.MorseCode:
                        {
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = false;
                            HasDecryptKey = false;
                            HasDecryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = false;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = true;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = false;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = false;
                            HasDecryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPrivateKey = true;
                            HasRSAEncryptionPaddingMode = true;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = true;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
                            break;
                        }
                    case DataDecryptType.XOR:
                        {
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = DecryptKeyInitializeString;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasParseAsTextData = false;
                            ParseAsTextData = false;
                            SelectedTextEncodingType = TextEncodingTypeList[0];
                            TextEncodingCustomTypeText = string.Empty;
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
        /// 解密密钥字符串解码模式发生变化时触发的事件
        /// </summary>
        private void OnDecryptKeyStringTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> decryptKeyStringType)
            {
                SelectedDecryptKeyStringType = decryptKeyStringType;
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
        /// 初始化向量字符串解码模式发生变化时触发的事件
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
        /// 解密块密码模式发生变化时触发的事件
        /// </summary>
        private void OnDecryptedBlockCipherModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<CipherMode, string> decryptedBlockCipherMode)
            {
                SelectedDecryptedBlockCipherMode = decryptedBlockCipherMode;
                if (SelectedDataDecryptType.DataDecryptType is DataDecryptType.AES || SelectedDataDecryptType.DataDecryptType is DataDecryptType.DES || SelectedDataDecryptType.DataDecryptType is DataDecryptType.RC2 ||
                    SelectedDataDecryptType.DataDecryptType is DataDecryptType.RC5 || SelectedDataDecryptType.DataDecryptType is DataDecryptType.RC6 || SelectedDataDecryptType.DataDecryptType is DataDecryptType.Rijndael ||
                    SelectedDataDecryptType.DataDecryptType is DataDecryptType.SM4 || SelectedDataDecryptType.DataDecryptType is DataDecryptType.TripleDES)
                {
                    HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                }
            }
        }

        /// <summary>
        /// 填充模式发生变化时触发的事件
        /// </summary>
        private void OnPaddingModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<PaddingMode, string> decryptedPaddingMode)
            {
                SelectedPaddingMode = decryptedPaddingMode;
            }
        }

        /// <summary>
        /// 偏移量值发生变化时触发的事件
        /// </summary>
        private void OnOffsetValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN)
            {
                Offset = Convert.ToInt32(args.NewValue);
            }
        }

        /// <summary>
        /// 解密私钥内容发生变化时触发的事件
        /// </summary>
        private void OnDecryptPrivateKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptPrivateKeyText = textBox.Text;
            }
        }

        /// <summary>
        /// RSA 非对称加密填充模式发生变化时触发的事件
        /// </summary>
        private void OnRSAEncryptionPaddingModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<RSAEncryptionPaddingMode, string> encryptedRSAEncryptionPaddingMode)
            {
                SelectedRSAEncryptionPaddingMode = encryptedRSAEncryptionPaddingMode;
            }
        }

        /// <summary>
        /// 以文本方式解析解密后的数据
        /// </summary>
        private void OnParseAsTextDataToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                ParseAsTextData = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 选择文字解码类型
        /// </summary>
        private void OnTextEncodingTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> textDecodingType)
            {
                SelectedTextEncodingType = textDecodingType;
            }
        }

        /// <summary>
        /// 文字自定义解码类型内容发生变化时触发的事件
        /// </summary>
        private void OnTextEncodingCustomTypeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                TextEncodingCustomTypeText = textBox.Text;
            }
        }

        /// <summary>
        /// 保存解密的数据到本地文件中
        /// </summary>
        private void OnSaveDecryptedDataToLocalFileToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SaveDecryptedDataToLocalFile = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 修改文件存储路径
        /// </summary>
        private void OnChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = string.Format(".*|{0}", AllFilesString),
                DefaultExt = ".*",
                Title = SelectFileString
            };
            if (saveFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                SaveDecryptedFilePath = saveFileDialog.FileName;
            }
            saveFileDialog.Dispose();
        }

        /// <summary>
        /// 打开文件所对应的文件夹
        /// </summary>
        private void OnSaveDecryptedFilePathClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(SaveDecryptedFilePath))
                    {
                        if (File.Exists(SaveDecryptedFilePath))
                        {
                            nint pidlList = Shell32Library.ILCreateFromPath(SaveDecryptedFilePath);
                            if (pidlList is not 0)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        else
                        {
                            string directoryPath = Path.GetDirectoryName(SaveDecryptedFilePath);

                            if (Directory.Exists(directoryPath))
                            {
                                Process.Start(directoryPath);
                            }
                            else
                            {
                                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnSaveDecryptedFilePathClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 开始数据解密
        /// </summary>
        private async void OnStartDecryptClicked(object sender, RoutedEventArgs args)
        {
            selectDecryptIndex = SelectedIndex;
            selectedDecryptFile = DecryptFile;
            inputtedDecryptContent = DecryptContent;
            DecryptFailedInformation = string.Empty;
            DecryptResult = string.Empty;
            decryptedLocalFile = string.Empty;
            IsLargeContent = false;

            // 检查要读取解密的文件内容
            if (selectDecryptIndex is 0)
            {
                if (string.IsNullOrEmpty(selectedDecryptFile))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = FileNotSelectedString;
                    return;
                }
                else if (!File.Exists(selectedDecryptFile))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = FileNotExistedString;
                    return;
                }
                else if (!CheckIsTextFile(selectedDecryptFile))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = FileNotTextContentString;
                    return;
                }
            }
            // 检查要解密的字符串
            else if (selectDecryptIndex is 1)
            {
                if (string.IsNullOrEmpty(inputtedDecryptContent))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = ContentEmptyString;
                    return;
                }
            }
            else
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = SelectedDataDecryptInputContentFormatErrorString;
                return;
            }

            // 选择解密的算法为空时的提示
            if (SelectedDataDecryptType is null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = DecryptTypeNotSelectedString;
                return;
            }
            else
            {
                // 检查对应的加密算法所填充的解密密钥或初始化向量是否为空
                switch (SelectedDataDecryptType.DataDecryptType)
                {
                    case DataDecryptType.AES:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.CaesarCipher:
                        {
                            if (selectDecryptIndex is not 1)
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptTypeMustContentString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.ChaCha20:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.DES:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.MorseCode:
                        {
                            if (selectDecryptIndex is not 1)
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptTypeMustContentString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.Rabbit:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.RC2:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.RC4:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.RC5:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.RC6:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.Rijndael:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.RSA:
                        {
                            if (string.IsNullOrEmpty(DecryptPrivateKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptPrivateKeyEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.SM4:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.TripleDES:
                        {
                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }

                            if (SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataDecryptType.XOR:
                        {
                            if (selectDecryptIndex is not 1)
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptTypeMustContentString;
                                return;
                            }

                            if (string.IsNullOrEmpty(DecryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = DecryptKeyEmptyString;
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            // 获取具体的文字编码选项
            Encoding textEncoding = await Task.Run(() =>
            {
                Encoding textEncoding = null;
                if (Equals(SelectedTextEncodingType, TextEncodingTypeList[0]))
                {
                    textEncoding = Encoding.ASCII;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[1]))
                {
                    textEncoding = Encoding.BigEndianUnicode;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[6]))
                {
                    textEncoding = Encoding.Unicode;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[7]))
                {
                    textEncoding = Encoding.UTF32;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[8]))
                {
                    textEncoding = Encoding.UTF7;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[9]))
                {
                    textEncoding = Encoding.UTF8;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[10]))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(TextEncodingCustomTypeText))
                        {
                            textEncoding = int.TryParse(TextEncodingCustomTypeText, out int textEncodingCustomType) ? Encoding.GetEncoding(textEncodingCustomType) : Encoding.GetEncoding(TextEncodingCustomTypeText);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 1, e);
                    }
                }
                else
                {
                    textEncoding = Encoding.GetEncoding(SelectedTextEncodingType.Key);
                }
                return textEncoding;
            });

            // 文字编码异常时的提示
            if (textEncoding is null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = TextEncodingInvalidString;
                return;
            }

            // 解密内容
            IsDecrypting = true;
            ResultSeverity = InfoBarSeverity.Informational;
            ResultMessage = DecryptingString;
            (object decryptedData, Exception decryptException) = await Task.Run(() =>
            {
                return GetDecryptedData(SelectedDataDecryptType.DataDecryptType, selectDecryptIndex, inputtedDecryptContent, textEncoding, selectedDecryptFile);
            });

            // 解密失败
            if (decryptException is not null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                if (selectDecryptIndex is 0)
                {
                    ResultMessage = FileDecryptFailedString;
                }
                else if (selectDecryptIndex is 1)
                {
                    ResultMessage = ContentDecryptFailedString;
                }

                DecryptFailedInformation = !string.IsNullOrEmpty(decryptException.Message) ? decryptException.Message : UnknownErrorString;
            }
            // 解密成功
            else
            {
                bool isSavedToSelectedFile = false;
                bool isSavedToTempFile = false;
                Exception exception = null;
                if (SaveDecryptedDataToLocalFile)
                {
                    await Task.Run(() =>
                    {
                        // 保存到选择的本地文件中
                        try
                        {
                            decryptedLocalFile = SaveDecryptedFilePath;
                            if (decryptedData is string decryptedDataString)
                            {
                                File.WriteAllText(decryptedLocalFile, decryptedDataString);
                                isSavedToSelectedFile = true;
                            }
                            else if (decryptedData is byte[] decryptedDataBytes)
                            {
                                File.WriteAllBytes(decryptedLocalFile, decryptedDataBytes);
                                isSavedToSelectedFile = true;
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 2, e);
                            exception = e;
                        }

                        // 保存到选定的本地文件失败，自动保存到临时文件目录中
                        if (!isSavedToSelectedFile)
                        {
                            try
                            {
                                decryptedLocalFile = Path.GetTempFileName();
                                // 解密后的数据是字符串
                                if (decryptedData is string decryptedDataString)
                                {
                                    File.WriteAllText(decryptedLocalFile, decryptedDataString);
                                    isSavedToTempFile = true;
                                }
                                // 解密后的数据是字节数组
                                else if (decryptedData is byte[] decryptedDataBytes)
                                {
                                    // 以文本方式解析数据
                                    if (ParseAsTextData)
                                    {
                                        string decryptedString = textEncoding.GetString(decryptedDataBytes).TrimEnd();
                                        File.WriteAllText(decryptedLocalFile, Convert.ToString(decryptedString));
                                        isSavedToSelectedFile = true;
                                    }
                                    // 以二进制方式解析数据
                                    else
                                    {
                                        File.WriteAllBytes(decryptedLocalFile, decryptedDataBytes);
                                        isSavedToSelectedFile = true;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 3, e);
                                exception = e;
                            }
                        }
                    });
                }

                // 解密后的数据是字符串
                if (decryptedData is string decryptedDataString)
                {
                    string decryptedString = Convert.ToString(decryptedData).TrimEnd();

                    // 解密后的字符串数据太长
                    if (decryptedString.Length > 1024)
                    {
                        DecryptResult = LargeContentString;
                        IsLargeContent = true;
                        IsBinaryFile = false;

                        if (!SaveDecryptedDataToLocalFile)
                        {
                            await Task.Run(() =>
                            {
                                try
                                {
                                    decryptedLocalFile = Path.GetTempFileName();
                                    File.WriteAllText(decryptedLocalFile, decryptedDataString);
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 4, e);
                                }
                            });
                        }
                    }
                    else
                    {
                        DecryptResult = decryptedString;
                        IsLargeContent = false;
                        IsBinaryFile = false;
                    }
                }
                // 解密后的数据是字节数组
                else if (decryptedData is byte[] decryptedDataBytes)
                {
                    // 以文本方式解析数据
                    if (ParseAsTextData)
                    {
                        try
                        {
                            string decryptedString = textEncoding.GetString(decryptedDataBytes).TrimEnd();

                            // 解密后的字符串数据太长
                            if (decryptedString.Length > 1024)
                            {
                                DecryptResult = LargeContentString;
                                IsLargeContent = true;
                                IsBinaryFile = false;

                                if (!SaveDecryptedDataToLocalFile)
                                {
                                    await Task.Run(() =>
                                    {
                                        try
                                        {
                                            decryptedLocalFile = Path.GetTempFileName();
                                            File.WriteAllText(decryptedLocalFile, decryptedString);
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 5, e);
                                        }
                                    });
                                }
                            }
                            else
                            {
                                DecryptResult = decryptedString;
                                IsLargeContent = false;
                                IsBinaryFile = false;
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 6, e);
                        }
                    }
                    // 以二进制方式解析数据
                    else
                    {
                        DecryptResult = BinaryFileString;
                        IsLargeContent = false;
                        IsBinaryFile = true;

                        if (!SaveDecryptedDataToLocalFile)
                        {
                            await Task.Run(() =>
                            {
                                try
                                {
                                    decryptedLocalFile = Path.GetTempFileName();
                                    File.WriteAllBytes(decryptedLocalFile, decryptedDataBytes);
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnStartDecryptClicked), 7, e);
                                }
                            });
                        }
                    }
                }

                // 显示解密结果
                if (selectDecryptIndex is 0)
                {
                    if (SaveDecryptedDataToLocalFile)
                    {
                        if (isSavedToSelectedFile)
                        {
                            ResultSeverity = InfoBarSeverity.Success;
                            ResultMessage = FileDecryptedDataSaveSuccessfullyString;
                            DecryptFailedInformation = string.Empty;
                        }
                        else
                        {
                            ResultSeverity = isSavedToTempFile ? InfoBarSeverity.Warning : InfoBarSeverity.Error;
                            ResultMessage = isSavedToTempFile ? FileDecryptedDataSaveFailedToTempFileString : FileDecryptedDataSaveFailedString;
                            DecryptFailedInformation = exception is not null && !string.IsNullOrEmpty(exception.Message) ? exception.Message : UnknownErrorString;
                        }
                    }
                    else
                    {
                        ResultSeverity = InfoBarSeverity.Success;
                        ResultMessage = FileDecryptSuccessfullyString;
                        DecryptFailedInformation = string.Empty;
                    }
                }
                else if (selectDecryptIndex is 1)
                {
                    if (SaveDecryptedDataToLocalFile)
                    {
                        if (isSavedToSelectedFile)
                        {
                            ResultSeverity = InfoBarSeverity.Success;
                            ResultMessage = ContentDecryptedDataSaveSuccessfullyString;
                            DecryptFailedInformation = string.Empty;
                        }
                        else
                        {
                            ResultSeverity = isSavedToTempFile ? InfoBarSeverity.Warning : InfoBarSeverity.Error;
                            DecryptFailedInformation = exception is not null && !string.IsNullOrEmpty(exception.Message) ? exception.Message : UnknownErrorString;
                            ResultMessage = isSavedToTempFile ? ContentDecryptedDataSaveFailedToTempFileString : ContentDecryptedDataSaveFailedString;
                        }
                    }
                    else
                    {
                        ResultSeverity = InfoBarSeverity.Success;
                        ResultMessage = ContentDecryptSuccessfullyString;
                        DecryptFailedInformation = string.Empty;
                    }
                }
            }
            IsDecrypting = false;
        }

        /// <summary>
        /// 查看本地文件
        /// </summary>
        private async void OnViewLocalFileClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(decryptedLocalFile))
                    {
                        Process.Start(decryptedLocalFile);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnViewLocalFileClicked), 1, e);
                }
            });
        }

        #endregion 第一部分：数据解密页面——挂载的事件

        /// <summary>
        /// 获取解密后的数据
        /// </summary>
        private (object, Exception) GetDecryptedData(DataDecryptType dataDecryptType, int selectedDecryptIndex, string contentData, Encoding textEncoding, string selectedDecryptFile)
        {
            object decryptedData = null;
            Exception decryptException = null;

            switch (dataDecryptType)
            {
                case DataDecryptType.AES:
                    {
                        try
                        {
                            Aes aes = Aes.Create();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    aes.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    aes.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    aes.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    aes.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            aes.Mode = SelectedDecryptedBlockCipherMode.Key;
                            aes.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = Convert.FromBase64String(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = Convert.FromBase64String(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.AES) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.CaesarCipher:
                    {
                        if (selectedDecryptIndex is 0)
                        {
                            contentData = File.ReadAllText(selectedDecryptFile);
                        }

                        try
                        {
                            decryptedData = CaesarCipher.CaesarDecrypt(contentData, Offset);
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.CaesarCipher) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.ChaCha20:
                    {
                        try
                        {
                            byte[] decryptedKey = null;
                            byte[] decryptedIV = null;
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    decryptedKey = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    decryptedKey = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    decryptedIV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    decryptedIV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                decryptedData = Convert.ToBase64String(ChaCha20.Decrypt(decryptedKey, decryptedIV, contentDataBytes));
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.ChaCha20) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.DES:
                    {
                        try
                        {
                            DES des = DES.Create();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    des.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    des.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    des.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    des.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            des.Mode = SelectedDecryptedBlockCipherMode.Key;
                            des.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = des.CreateDecryptor(des.Key, des.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.DES) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.MorseCode:
                    {
                        if (selectedDecryptIndex is 0)
                        {
                            contentData = File.ReadAllText(selectedDecryptFile);
                        }

                        try
                        {
                            decryptedData = MorseCode.MorseDecode(contentData);
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.MorseCode) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.Rabbit:
                    {
                        try
                        {
                            Rabbit rabbit = new();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    rabbit.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    rabbit.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rabbit.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rabbit.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rabbit.Mode = CipherMode.CBC;
                            rabbit.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = rabbit.CreateDecryptor(rabbit.Key, rabbit.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.Rabbit) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.RC2:
                    {
                        try
                        {
                            RC2 rc2 = RC2.Create();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    rc2.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    rc2.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc2.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc2.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rc2.Mode = SelectedDecryptedBlockCipherMode.Key;
                            rc2.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = rc2.CreateDecryptor(rc2.Key, rc2.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.RC2) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.RC4:
                    {
                        try
                        {
                            RC4 rc4 = RC4.Create();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    rc4.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    rc4.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            rc4.Mode = SelectedDecryptedBlockCipherMode.Key;
                            rc4.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = rc4.CreateDecryptor(rc4.Key, rc4.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.RC4) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.RC5:
                    {
                        try
                        {
                            RC5 rc5 = new();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    rc5.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    rc5.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc5.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc5.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rc5.Mode = SelectedDecryptedBlockCipherMode.Key;
                            rc5.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = rc5.CreateDecryptor(rc5.Key, rc5.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.RC5) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.RC6:
                    {
                        try
                        {
                            RC6 rc6 = new();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    rc6.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    rc6.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc6.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc6.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rc6.Mode = SelectedDecryptedBlockCipherMode.Key;
                            rc6.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = rc6.CreateDecryptor(rc6.Key, rc6.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.RC6) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.Rijndael:
                    {
                        try
                        {
                            Rijndael rijndael = Rijndael.Create();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    rijndael.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    rijndael.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rijndael.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rijndael.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rijndael.Mode = SelectedDecryptedBlockCipherMode.Key;
                            rijndael.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.Rijndael) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.RSA:
                    break;

                case DataDecryptType.SM4:
                    {
                        try
                        {
                            SM4 sm4 = new();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    sm4.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    sm4.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    sm4.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    sm4.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            sm4.Mode = SelectedDecryptedBlockCipherMode.Key;
                            sm4.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = sm4.CreateDecryptor(sm4.Key, sm4.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.SM4) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.TripleDES:
                    {
                        try
                        {
                            TripleDES tripleDes = TripleDES.Create();
                            if (!string.IsNullOrEmpty(DecryptKeyText))
                            {
                                if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[0]))
                                {
                                    tripleDes.Key = Encoding.UTF8.GetBytes(DecryptKeyText);
                                }
                                else if (Equals(SelectedDecryptKeyStringType, DecryptKeyStringTypeList[1]))
                                {
                                    tripleDes.Key = Convert.FromBase64String(DecryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    tripleDes.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    tripleDes.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            tripleDes.Mode = SelectedDecryptedBlockCipherMode.Key;
                            tripleDes.Padding = SelectedPaddingMode.Key;
                            byte[] contentDataBytes = null;
                            if (selectedDecryptIndex is 0 && File.Exists(selectedDecryptFile))
                            {
                                contentDataBytes = textEncoding.GetBytes(File.ReadAllText(selectedDecryptFile));
                            }
                            else if (selectedDecryptIndex is 1)
                            {
                                contentDataBytes = textEncoding.GetBytes(contentData);
                            }

                            if (contentDataBytes is not null)
                            {
                                ICryptoTransform cryptoTransform = tripleDes.CreateDecryptor(tripleDes.Key, tripleDes.IV);
                                MemoryStream memoryStream = new();
                                CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                                cryptoStream.Write(contentDataBytes, 0, contentDataBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = memoryStream.ToArray();
                                cryptoStream.Dispose();
                                memoryStream.Dispose();
                                cryptoTransform.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.TripleDES) + 1, e);
                        }
                        break;
                    }
                case DataDecryptType.XOR:
                    {
                        if (selectedDecryptIndex is 0)
                        {
                            contentData = File.ReadAllText(selectedDecryptFile);
                        }

                        try
                        {
                            decryptedData = XOR.XORDecrypt(contentData, DecryptKeyText);
                        }
                        catch (Exception e)
                        {
                            decryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(GetDecryptedData), Convert.ToInt32(DataDecryptType.MorseCode) + 1, e);
                        }
                        break;
                    }
            }
            return ValueTuple.Create(decryptedData, decryptException);
        }

        /// <summary>
        /// 检查读取的文件是否是文本文件
        /// </summary>
        public static bool CheckIsTextFile(string fileName)
        {
            bool isTextFile = true;
            try
            {
                using FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read);
                int i = 0;
                int length = (int)fileStream.Length;
                byte data;
                while (i < length && isTextFile)
                {
                    data = (byte)fileStream.ReadByte();
                    isTextFile = data is not 0;
                    i++;
                }
                return isTextFile;
            }
            catch (Exception e)
            {
                isTextFile = false;
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(CheckIsTextFile), 1, e);
            }
            return isTextFile;
        }

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

        private Visibility GetHasParseAsTextData(bool hasParseAsTextData, bool parseAsTextData)
        {
            return hasParseAsTextData && parseAsTextData ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility GetCustomTextEncodingType(bool hasParseAsTextData, bool parseAsTextData, string selectedTextEncodingType, string comparedTextEncodingType)
        {
            return hasParseAsTextData && parseAsTextData && string.Equals(selectedTextEncodingType, comparedTextEncodingType) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取解密结果
        /// </summary>
        private Visibility GetDecryptResult(InfoBarSeverity resultSeverity)
        {
            return resultSeverity is InfoBarSeverity.Success || resultSeverity is InfoBarSeverity.Warning ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetIsNotDecryptedResultString(bool isLargeContent, bool isBinaryFile)
        {
            return !(isLargeContent || isBinaryFile);
        }

        private Visibility GetIsDecryptedResultString(bool isLargeContent, bool isBinaryFile)
        {
            return isLargeContent || isBinaryFile ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
