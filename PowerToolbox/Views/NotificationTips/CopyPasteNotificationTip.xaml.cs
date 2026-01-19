using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace PowerToolbox.Views.NotificationTips
{
    /// <summary>
    /// 复制剪贴应用内通知
    /// </summary>
    public sealed partial class CopyPasteNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        private bool _isSuccessfully;

        public bool IsSuccessfully
        {
            get { return _isSuccessfully; }

            set
            {
                if (!Equals(_isSuccessfully, value))
                {
                    _isSuccessfully = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessfully)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CopyPasteNotificationTip(bool isSuccessfully)
        {
            InitializeComponent();
            IsSuccessfully = isSuccessfully;
        }
    }
}
