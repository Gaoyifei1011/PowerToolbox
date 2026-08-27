using Microsoft.UI.Xaml.Controls;
using System.Windows.Forms;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 应用重启对话框
    /// </summary>
    internal sealed partial class RestartAppsDialog : ContentDialog
    {
        internal RestartAppsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        private void OnRestartAppsClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Application.Restart();
            (Microsoft.UI.Xaml.Application.Current as MainApp).Dispose();
        }
    }
}
