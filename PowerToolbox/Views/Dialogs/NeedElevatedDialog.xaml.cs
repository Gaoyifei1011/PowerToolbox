using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 需要提权信息对话框
    /// </summary>
    public sealed partial class NeedElevatedDialog : ContentDialog
    {
        public NeedElevatedDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 提权运行该应用
        /// </summary>
        private void OnRunAsAdministratorClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        Arguments = "--elevated",
                        FileName = Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
            });
        }
    }
}
