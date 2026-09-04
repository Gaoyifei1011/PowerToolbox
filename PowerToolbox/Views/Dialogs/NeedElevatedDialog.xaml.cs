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
    internal sealed partial class NeedElevatedDialog : ContentDialog
    {
        #region 第一部分：构造函数

        internal NeedElevatedDialog()
        {
            InitializeComponent();
        }

        #endregion 第一部分：构造函数

        #region 第二部分：挂载事件处理

        /// <summary>
        /// 提权运行该应用
        /// </summary>
        private void OnRunAsAdministratorClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RunAsAdministrator();
        }

        #endregion 第二部分：挂载事件处理

        /// <summary>
        /// 提权运行该应用
        /// </summary>
        private void RunAsAdministrator()
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
