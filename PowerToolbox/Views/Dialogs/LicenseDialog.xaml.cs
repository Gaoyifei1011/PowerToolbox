using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Text;

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    internal sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _licenseText = Encoding.UTF8.GetString(Strings.Resources.LICENSE);

        internal string LicenseText
        {
            get { return _licenseText; }

            set
            {
                if (!Equals(_licenseText, value))
                {
                    _licenseText = value;
                    PropertyChanged?.Invoke(this, new(nameof(LicenseText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal LicenseDialog()
        {
            InitializeComponent();
        }
    }
}
