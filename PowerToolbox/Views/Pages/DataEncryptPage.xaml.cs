using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据加密页面
    /// </summary>
    public sealed partial class DataEncryptPage : Page, INotifyPropertyChanged
    {
        private readonly string AESString = ResourceService.DataEncryptResource.GetString("AES");
        private readonly string BlowfishString = ResourceService.DataEncryptResource.GetString("Blowfish");
        private readonly string CaesarCipherString = ResourceService.DataEncryptResource.GetString("CaesarCipher");
        private readonly string ChaCha20String = ResourceService.DataEncryptResource.GetString("ChaCha20");
        private readonly string ContentInitializeString = ResourceService.DataEncryptResource.GetString("ContentInitialize");
        private readonly string ContentEncryptFailedString = ResourceService.DataEncryptResource.GetString("ContentEncryptFailed");
        private readonly string ContentEncryptPartSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptPartSuccessfully");
        private readonly string ContentEncryptWholeSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptWholeSuccessfully");
        private readonly string DESString = ResourceService.DataEncryptResource.GetString("DES");
        private readonly string ECCString = ResourceService.DataEncryptResource.GetString("ECC");
        private readonly string EncryptingString = ResourceService.DataEncryptResource.GetString("Encrypting");
        private readonly string FileInitializeString = ResourceService.DataEncryptResource.GetString("FileInitialize");
        private readonly string FileEncryptFailedString = ResourceService.DataEncryptResource.GetString("FileEncryptFailed");
        private readonly string FileEncryptPartSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptPartSuccessfully");
        private readonly string FileEncryptWholeSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptWholeSuccessfully");
        private readonly string MorseCodeString = ResourceService.DataEncryptResource.GetString("MorseCode");
        private readonly string RabbitString = ResourceService.DataEncryptResource.GetString("Rabbit");
        private readonly string RC2String = ResourceService.DataEncryptResource.GetString("RC2");
        private readonly string RC4String = ResourceService.DataEncryptResource.GetString("RC4");
        private readonly string RC5String = ResourceService.DataEncryptResource.GetString("RC5");
        private readonly string RC6String = ResourceService.DataEncryptResource.GetString("RC6");
        private readonly string RSAString = ResourceService.DataEncryptResource.GetString("RSA");
        private readonly string SM2String = ResourceService.DataEncryptResource.GetString("SM2");
        private readonly string SM4String = ResourceService.DataEncryptResource.GetString("SM4");
        private readonly string TripleDESString = ResourceService.DataEncryptResource.GetString("TripleDES");
        private readonly string XORString = ResourceService.DataEncryptResource.GetString("XOR");

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

        private string _encryptFile;

        public string EncryptFile
        {
            get { return _encryptFile; }

            set
            {
                if (!Equals(_encryptFile, value))
                {
                    _encryptFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptFile)));
                }
            }
        }

        private string _encryptContent;

        public string EncryptContent
        {
            get { return _encryptContent; }

            set
            {
                if (!Equals(_encryptContent, value))
                {
                    _encryptContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptContent)));
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

        private bool _isEncrypting = false;

        public bool IsEncrypting
        {
            get { return _isEncrypting; }

            set
            {
                if (!Equals(_isEncrypting, value))
                {
                    _isEncrypting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEncrypting)));
                }
            }
        }

        private List<DataEncryptTypeModel> DataEncryptTypeList { get; } = [];

        private List<DataEncryptVertifyResultModel> DataEncryptResultCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataEncryptPage()
        {
            InitializeComponent();
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.AES,
                Name = AESString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.Blowfish,
                Name = BlowfishString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.CaesarCipher,
                Name = CaesarCipherString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.ChaCha20,
                Name = ChaCha20String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.DES,
                Name = DESString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.ECC,
                Name = ECCString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.MorseCode,
                Name = MorseCodeString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.Rabbit,
                Name = RabbitString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC2,
                Name = RC2String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC4,
                Name = RC4String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC5,
                Name = RC5String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC6,
                Name = RC6String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RSA,
                Name = RSAString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.SM2,
                Name = SM2String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.SM4,
                Name = SM4String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.TripleDES,
                Name = TripleDESString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.XOR,
                Name = XORString
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
        /// TODO：未完成
        private void OnOpenLocalFileClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 校验内容发生改变时触发的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                EncryptContent = textBox.Text;
            }
        }

        /// <summary>
        /// 开始数据加密
        /// </summary>
        /// TODO：未完成
        private void OnStartEncryptClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataEncryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取校验结果列表显示类型
        /// </summary>
        private Visibility GetEncryptResult(bool isEncrypting, int encryptCount)
        {
            return isEncrypting ? Visibility.Collapsed : encryptCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
