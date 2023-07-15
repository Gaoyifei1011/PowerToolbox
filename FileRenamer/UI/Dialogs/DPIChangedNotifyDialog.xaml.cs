using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// ��Ļ����֪ͨ�Ի���
    /// </summary>
    public sealed partial class DPIChangedNotifyDialog : ExtendedContentDialog
    {
        public DPIChangedNotifyDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ����Ӧ��
        /// </summary>
        public void OnRestartClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RestartApps();
        }

        /// <summary>
        /// ����Ӧ�ã����ر���������
        /// </summary>
        private void RestartApps()
        {
            Program.ApplicationRoot.CloseApp();
            Application.Restart();
        }
    }
}
