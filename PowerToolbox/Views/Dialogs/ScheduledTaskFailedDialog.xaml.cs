using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 计划任务操作错误信息列表对话框
    /// </summary>
    internal sealed partial class ScheduledTaskFailedDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ExceptionCodeString = ResourceService.DialogResource.GetString("ExceptionCode");
        private readonly string ExceptionMessageString = ResourceService.DialogResource.GetString("ExceptionMessage");
        private readonly string ScheduledTaskNameCopyString = ResourceService.DialogResource.GetString("ScheduledTaskNameCopy");
        private readonly string ScheduledTaskPathCopyString = ResourceService.DialogResource.GetString("ScheduledTaskPathCopy");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private WinRTObservableCollection<ScheduledTaskFailedModel> ScheduledTaskFailedCollection { get; } = [];

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal ScheduledTaskFailedDialog(List<ScheduledTaskFailedModel> scheduledTaskFailedList)
        {
            InitializeComponent();

            foreach (ScheduledTaskFailedModel scheduledTaskFailedItem in scheduledTaskFailedList)
            {
                ScheduledTaskFailedCollection.Add(scheduledTaskFailedItem);
            }
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 复制异常信息
        /// </summary>
        private async void OnCopyExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskFailedModel scheduledTaskFailedItem)
            {
                string scheduledTaskFailedString = await GetScheduledTaskFailedStringAsync([scheduledTaskFailedItem]);

                if (!string.IsNullOrEmpty(scheduledTaskFailedString))
                {
                    bool copyResult = CopyPasteHelper.CopyToClipboard(scheduledTaskFailedString);
                    await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
                }
            }
        }

        #endregion 第四部分：命令调用处理

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 复制所有的错误内容到剪贴板
        /// </summary>
        private async void OnCopyOperationFailedClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool copyResult = false;
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                string scheduledTaskFailedString = await GetScheduledTaskFailedStringAsync([.. ScheduledTaskFailedCollection]);

                if (!string.IsNullOrEmpty(scheduledTaskFailedString))
                {
                    copyResult = CopyPasteHelper.CopyToClipboard(scheduledTaskFailedString);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskFailedDialog), nameof(OnCopyOperationFailedClicked), 1, e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
        }

        /// <summary>
        /// 打开计划任务程序
        /// </summary>
        private void OnOpenScheduledTaskProgramClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OpenScheduledTaskProgram();
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 获取计划任务失败信息内容
        /// </summary>
        private async Task<string> GetScheduledTaskFailedStringAsync(List<ScheduledTaskFailedModel> scheduledTaskFailedList)
        {
            if (scheduledTaskFailedList is null || scheduledTaskFailedList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    StringBuilder stringBuilder = new();

                    foreach (ScheduledTaskFailedModel scheduledTaskFailedItem in scheduledTaskFailedList)
                    {
                        stringBuilder.Append(ScheduledTaskNameCopyString);
                        stringBuilder.AppendLine(scheduledTaskFailedItem.Name);
                        stringBuilder.Append(ScheduledTaskPathCopyString);
                        stringBuilder.AppendLine(scheduledTaskFailedItem.Path);
                        stringBuilder.Append(ExceptionMessageString);
                        stringBuilder.AppendLine(scheduledTaskFailedItem.Exception.Message);
                        stringBuilder.Append(ExceptionCodeString);
                        stringBuilder.AppendLine(string.Format("0x{0:X8}", scheduledTaskFailedItem.Exception.HResult));
                        stringBuilder.AppendLine();
                    }

                    return Convert.ToString(stringBuilder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskFailedDialog), nameof(GetScheduledTaskFailedStringAsync), 1, e);
                    return default;
                }
            });
        }

        /// <summary>
        /// 打开计划任务程序
        /// </summary>
        private void OpenScheduledTaskProgram()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskschd.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskFailedDialog), nameof(OpenScheduledTaskProgram), 1, e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
