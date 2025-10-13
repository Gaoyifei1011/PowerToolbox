using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Services.Root;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据加密页面
    /// </summary>
    public sealed partial class DataEncryptPage : Page, INotifyPropertyChanged
    {
        private readonly string ContentInitializeString = ResourceService.DataEncryptResource.GetString("ContentInitialize");
        private readonly string ContentEncryptFailedString = ResourceService.DataEncryptResource.GetString("ContentEncryptFailed");
        private readonly string ContentEncryptPartSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptPartSuccessfully");
        private readonly string ContentEncryptWholeSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptWholeSuccessfully");
        private readonly string FileInitializeString = ResourceService.DataEncryptResource.GetString("FileInitialize");
        private readonly string FileEncryptFailedString = ResourceService.DataEncryptResource.GetString("FileEncryptFailed");
        private readonly string FileEncryptPartSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptPartSuccessfully");
        private readonly string FileEncryptWholeSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptWholeSuccessfully");

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

        public event PropertyChangedEventHandler PropertyChanged;

        public DataEncryptPage()
        {
            InitializeComponent();
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
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataEncryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
