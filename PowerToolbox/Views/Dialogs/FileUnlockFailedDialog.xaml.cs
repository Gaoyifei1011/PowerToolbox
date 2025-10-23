using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Pages;
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
    public sealed partial class FileUnlockFailedDialog : ContentDialog
    {
        private readonly string ExceptionCodeString = ResourceService.DialogResource.GetString("ExceptionCode");
        private readonly string ExceptionMessageString = ResourceService.DialogResource.GetString("ExceptionMessage");
        private readonly string FileNameCopyString = ResourceService.DialogResource.GetString("FileNameCopy");
        private readonly string FilePathCopyString = ResourceService.DialogResource.GetString("FilePathCopy");
        private readonly string ProcessNameCopyString = ResourceService.DialogResource.GetString("ProcessNameCopy");
        private readonly string ProcessPathCopyString = ResourceService.DialogResource.GetString("ProcessPathCopy");

        private WinRTObservableCollection<FileUnlockFailedModel> FileUnlockFailedCollection { get; } = [];

        public FileUnlockFailedDialog(List<FileUnlockFailedModel> fileUnlockFailedList)
        {
            InitializeComponent();

            foreach (FileUnlockFailedModel fileUnlockFailedItem in fileUnlockFailedList)
            {
                FileUnlockFailedCollection.Add(fileUnlockFailedItem);
            }
        }

        /// <summary>
        /// 复制异常信息
        /// </summary>
        private async void OnCopyExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is FileUnlockFailedModel fileUnlockFailedItem)
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();
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
                    return stringBuilder;
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 打开文件路径
        /// </summary>
        private void OnOpenFilePathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            if (File.Exists(filePath))
                            {
                                IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                                if (!pidlList.Equals(IntPtr.Zero))
                                {
                                    Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnOpenFilePathExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开进程路径
        /// </summary>
        private void OnOpenProcessPathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string processPath)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(processPath))
                        {
                            if (File.Exists(processPath))
                            {
                                IntPtr pidlList = Shell32Library.ILCreateFromPath(processPath);
                                if (!pidlList.Equals(IntPtr.Zero))
                                {
                                    Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                                    Shell32Library.ILFree(pidlList);
                                }
                            }
                            else
                            {
                                string directoryPath = Path.GetDirectoryName(processPath);

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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnOpenProcessPathExecuteRequested), 1, e);
                    }
                });
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

                    foreach (FileUnlockFailedModel fileUnlockFailedItem in FileUnlockFailedCollection)
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

                    return stringBuilder;
                });

                copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
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

            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockFailedDialog), nameof(OnOpenTaskManagerClicked), 1, e);
                }
            });
        }
    }
}
