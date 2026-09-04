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
    /// 打开自启任务失败提示
    /// </summary>
    internal sealed partial class OpenStartupTaskFailedDialog : ContentDialog
    {
        #region 第一部分：构造函数

        internal OpenStartupTaskFailedDialog()
        {
            InitializeComponent();
        }

        #endregion 第一部分：构造函数

        #region 第二部分：挂载事件处理

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OnOpenTaskManagerClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OpenTaskManager();
        }

        /// <summary>
        /// 打开组策略
        /// </summary>

        private void OnOpenGroupPolicyClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OpenGroupPolicy();
        }

        #endregion 第二部分：挂载事件处理

        #region 第三部分：数据操作与业务逻辑

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OpenTaskManager()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OpenStartupTaskFailedDialog), nameof(OpenTaskManager), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开组策略
        /// </summary>
        private void OpenGroupPolicy()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("gpedit.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OpenStartupTaskFailedDialog), nameof(OpenGroupPolicy), 1, e);
                }
            });
        }

        #endregion 第三部分：数据操作与业务逻辑
    }
}
