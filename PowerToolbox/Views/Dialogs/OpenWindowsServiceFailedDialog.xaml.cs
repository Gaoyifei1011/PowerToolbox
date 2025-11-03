using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Services.Root;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 打开 Windows 服务失败提示
    /// </summary>
    public sealed partial class OpenWindowsServiceFailedDialog : ContentDialog
    {
        public OpenWindowsServiceFailedDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OnOpenTaskManagerClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OpenWindowsServiceFailedDialog), nameof(OnOpenTaskManagerClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开组策略
        /// </summary>

        private void OnOpenGroupPolicyClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("gpedit.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OpenWindowsServiceFailedDialog), nameof(OnOpenGroupPolicyClicked), 1, e);
                }
            });
        }
    }
}
