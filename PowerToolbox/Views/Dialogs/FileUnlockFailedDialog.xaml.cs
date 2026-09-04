using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 文件解锁错误信息列表对话框
    /// </summary>
    internal sealed partial class FileUnlockFailedDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ExceptionCodeString = ResourceService.DialogResource.GetString("ExceptionCode");
        private readonly string ExceptionMessageString = ResourceService.DialogResource.GetString("ExceptionMessage");
        private readonly string FileNameCopyString = ResourceService.DialogResource.GetString("FileNameCopy");
        private readonly string FilePathCopyString = ResourceService.DialogResource.GetString("FilePathCopy");
        private readonly string ProcessNameCopyString = ResourceService.DialogResource.GetString("ProcessNameCopy");
        private readonly string ProcessPathCopyString = ResourceService.DialogResource.GetString("ProcessPathCopy");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private WinRTObservableCollection<FileUnlockFailedModel> FileUnlockFailedCollection { get; } = [];

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal FileUnlockFailedDialog(List<FileUnlockFailedModel> fileUnlockFailedList)
        {
            InitializeComponent();

            foreach (FileUnlockFailedModel fileUnlockFailedItem in fileUnlockFailedList)
            {
                FileUnlockFailedCollection.Add(fileUnlockFailedItem);
            }
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 复制异常信息
        /// </summary>
        private async void OnCopyExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is FileUnlockFailedModel fileUnlockFailedItem)
            {
                string fileUnlockFailedString = await GetFileUnlockFailedStringAsync([fileUnlockFailedItem]);

                if (!string.IsNullOrEmpty(fileUnlockFailedString))
                {
                    bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(fileUnlockFailedString));
                    await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
                }
            }
        }

        /// <summary>
        /// 打开文件路径
        /// </summary>
        private void OnOpenFilePathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                OpenFilePath(filePath);
            }
        }

        /// <summary>
        /// 打开进程路径
        /// </summary>
        private void OnOpenProcessPathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string processPath)
            {
                OpenFilePath(processPath);
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
                string fileUnlockFailedString = await GetFileUnlockFailedStringAsync([.. FileUnlockFailedCollection]);

                if (!string.IsNullOrEmpty(fileUnlockFailedString))
                {
                    copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(fileUnlockFailedString));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockFailedDialog), nameof(OnCopyOperationFailedClicked), 1, e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
        }

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OnOpenTaskManagerClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            OpenTaskManager();
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 获取文件解锁失败信息内容
        /// </summary>
        private async Task<string> GetFileUnlockFailedStringAsync(List<FileUnlockFailedModel> fileUnlockFailedList)
        {
            if (fileUnlockFailedList is null || fileUnlockFailedList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    StringBuilder stringBuilder = new();

                    foreach (FileUnlockFailedModel fileUnlockFailedItem in fileUnlockFailedList)
                    {
                        stringBuilder.Append(FileNameCopyString);
                        stringBuilder.AppendLine(fileUnlockFailedItem.FileName);
                        stringBuilder.Append(FilePathCopyString);
                        stringBuilder.AppendLine(fileUnlockFailedItem.FilePath);
                        stringBuilder.Append(ProcessNameCopyString);
                        stringBuilder.AppendLine(fileUnlockFailedItem.ProcessName);
                        stringBuilder.Append(ProcessPathCopyString);
                        stringBuilder.AppendLine(fileUnlockFailedItem.ProcessPath);
                        stringBuilder.Append(ExceptionMessageString);
                        stringBuilder.AppendLine(fileUnlockFailedItem.Exception.Message);
                        stringBuilder.Append(ExceptionCodeString);
                        stringBuilder.AppendLine(string.Format("0x{0:X8}", fileUnlockFailedItem.Exception.HResult));
                        stringBuilder.AppendLine();
                    }

                    return Convert.ToString(stringBuilder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockFailedDialog), nameof(GetFileUnlockFailedStringAsync), 1, e);
                    return default;
                }
            });
        }

        /// <summary>
        /// 打开文件路径
        /// </summary>
        private void OpenFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        if (File.Exists(filePath))
                        {
                            nint pidlList = Shell32Library.ILCreateFromPath(filePath);
                            if (pidlList is not 0)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        else
                        {
                            string directoryPath = Path.GetDirectoryName(filePath);

                            if (Directory.Exists(directoryPath))
                            {
                                Process.Start(directoryPath);
                            }
                            else
                            {
                                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockFailedDialog), nameof(OpenFilePath), 1, e);
                }
            });
        }

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockFailedDialog), nameof(OpenTaskManager), 1, e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
