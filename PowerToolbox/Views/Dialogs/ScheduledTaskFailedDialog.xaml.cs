using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.Collections;
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
    public sealed partial class ScheduledTaskFailedDialog : ContentDialog
    {
        private readonly string ExceptionCodeString = ResourceService.DialogResource.GetString("ExceptionCode");
        private readonly string ExceptionMessageString = ResourceService.DialogResource.GetString("ExceptionMessage");
        private readonly string ScheduledTaskNameCopyString = ResourceService.DialogResource.GetString("ScheduledTaskNameCopy");
        private readonly string ScheduledTaskPathCopyString = ResourceService.DialogResource.GetString("ScheduledTaskPathCopy");

        private WinRTObservableCollection<ScheduledTaskFailedModel> ScheduledTaskFailedCollection { get; } = [];

        public ScheduledTaskFailedDialog(List<ScheduledTaskFailedModel> scheduledTaskFailedList)
        {
            InitializeComponent();

            foreach (ScheduledTaskFailedModel scheduledTaskFailedItem in scheduledTaskFailedList)
            {
                ScheduledTaskFailedCollection.Add(scheduledTaskFailedItem);
            }
        }

        /// <summary>
        /// 复制异常信息
        /// </summary>
        private async void OnCopyExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskFailedModel scheduledTaskFailedItem)
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();
                    stringBuilder.Append(ScheduledTaskNameCopyString);
                    stringBuilder.AppendLine(scheduledTaskFailedItem.Name);
                    stringBuilder.Append(ScheduledTaskPathCopyString);
                    stringBuilder.AppendLine(scheduledTaskFailedItem.Path);
                    stringBuilder.Append(ExceptionMessageString);
                    stringBuilder.AppendLine(scheduledTaskFailedItem.Exception.Message);
                    stringBuilder.Append(ExceptionCodeString);
                    stringBuilder.AppendLine(string.Format("0x{0:X8}", scheduledTaskFailedItem.Exception.HResult));
                    return stringBuilder;
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制所有的错误内容到剪贴板
        /// </summary>
        private async void OnCopyOperationFailedClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool copyResult = false;
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();

                    foreach (ScheduledTaskFailedModel scheduledTaskFailedItem in ScheduledTaskFailedCollection)
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

                    return stringBuilder;
                });

                copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
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
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskschd.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskFailedDialog), nameof(OnOpenScheduledTaskProgramClicked), 1, e);
                }
            });
        }
    }
}
