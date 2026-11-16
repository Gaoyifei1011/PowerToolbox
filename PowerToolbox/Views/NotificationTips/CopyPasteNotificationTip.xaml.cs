using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace PowerToolbox.Views.NotificationTips
{
    /// <summary>
    /// 复制剪贴应用内通知
    /// </summary>
    internal sealed partial class CopyPasteNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        internal bool IsSuccessfully
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSuccessfully)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal CopyPasteNotificationTip(bool isSuccessfully)
        {
            InitializeComponent();
            IsSuccessfully = isSuccessfully;
        }
    }
}
