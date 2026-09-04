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

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 错误信息列表对话框
    /// </summary>
    internal sealed partial class OperationFailedDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ExceptionCodeString = ResourceService.DialogResource.GetString("ExceptionCode");
        private readonly string ExceptionMessageString = ResourceService.DialogResource.GetString("ExceptionMessage");
        private readonly string FileNameCopyString = ResourceService.DialogResource.GetString("FileNameCopy");
        private readonly string FilePathCopyString = ResourceService.DialogResource.GetString("FilePathCopy");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private WinRTObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal OperationFailedDialog(List<OperationFailedModel> operationFailedList)
        {
            InitializeComponent();

            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedCollection.Add(operationFailedItem);
            }
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 复制异常信息
        /// </summary>
        private async void OnCopyExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OperationFailedModel operationFailedItem)
            {
                string operationFailedString = await GetOperationFailedStringAsync([operationFailedItem]);

                if (!string.IsNullOrEmpty(operationFailedString))
                {
                    bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(operationFailedString));
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
                string operationFailedString = await GetOperationFailedStringAsync([.. OperationFailedCollection]);

                if (!string.IsNullOrEmpty(operationFailedString))
                {
                    copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(operationFailedString));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OperationFailedDialog), nameof(OnCopyOperationFailedClicked), 1, e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 获取文件解锁失败信息内容
        /// </summary>
        private async Task<string> GetOperationFailedStringAsync(List<OperationFailedModel> operationFailedList)
        {
            if (operationFailedList is null || operationFailedList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    StringBuilder stringBuilder = new();

                    foreach (OperationFailedModel operationFailedItem in operationFailedList)
                    {
                        stringBuilder.Append(FileNameCopyString);
                        stringBuilder.AppendLine(operationFailedItem.FileName);
                        stringBuilder.Append(FilePathCopyString);
                        stringBuilder.AppendLine(operationFailedItem.FilePath);
                        stringBuilder.Append(ExceptionMessageString);
                        stringBuilder.AppendLine(operationFailedItem.Exception.Message);
                        stringBuilder.Append(ExceptionCodeString);
                        stringBuilder.AppendLine(string.Format("0x{0:X8}", operationFailedItem.Exception.HResult));
                        stringBuilder.AppendLine();
                    }

                    return Convert.ToString(stringBuilder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OperationFailedDialog), nameof(GetOperationFailedStringAsync), 1, e);
                    return default;
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
