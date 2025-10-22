using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Services.Root;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 打开自启任务失败提示
    /// </summary>
    public sealed partial class OpenStartupTaskFailedDialog : ContentDialog
    {
        public OpenStartupTaskFailedDialog()
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(OpenStartupTaskFailedDialog), nameof(OnOpenTaskManagerClicked), 1, e);
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
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(OpenStartupTaskFailedDialog), nameof(OnOpenGroupPolicyClicked), 1, e);
                }
            });
        }
    }
}
