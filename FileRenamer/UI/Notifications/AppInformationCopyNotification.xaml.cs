using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// Ӧ����Ϣ����Ӧ����֪ͨ
    /// </summary>
    public sealed partial class AppInformationCopyNotification : InAppNotification
    {
        public AppInformationCopyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}
