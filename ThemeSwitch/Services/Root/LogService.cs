using Microsoft.VisualBasic.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThemeSwitch.WindowsAPI.PInvoke.Shell32;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace ThemeSwitch.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly string notavailable = "N/A";
        private static SemaphoreSlim logSemaphoreSlim = new(1, 1);
        private static bool isInitialized = false;
        private static FileLogTraceListener fileLogTraceListener = new();
        private static DirectoryInfo logDirectory;

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static void Initialize()
        {
            Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, IntPtr.Zero, out string localAppdataPath);

            if (!string.IsNullOrEmpty(localAppdataPath))
            {
                try
                {
                    if (Directory.Exists(localAppdataPath))
                    {
                        string logFolderPath = Path.Combine(localAppdataPath, "Logs");
                        logDirectory = Directory.CreateDirectory(logFolderPath);
                        isInitialized = true;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

            fileLogTraceListener.Append = false;
            fileLogTraceListener.AutoFlush = true;
            fileLogTraceListener.Encoding = Encoding.UTF8;

            if (logDirectory is not null)
            {
                fileLogTraceListener.Location = LogFileLocation.Custom;
                fileLogTraceListener.CustomLocation = logDirectory.FullName;
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(TraceEventType traceEventType, string nameSpaceName, string className, string methodName, int index, Exception exception)
        {
            Task.Run(() =>
            {
                if (logSemaphoreSlim is not null)
                {
                    logSemaphoreSlim?.Wait();

                    if (fileLogTraceListener is not null)
                    {
                        try
                        {
                            if (logDirectory is not null && !Directory.Exists(logDirectory.FullName))
                            {
                                Directory.CreateDirectory(logDirectory.FullName);
                            }

                            string logFileName = string.Format("Logs-{0}-{1}-{2}-{3:D2}-{4}.xml", nameSpaceName, className, methodName, index, DateTimeOffset.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));

                            StringBuilder stringBuilder = new();
                            stringBuilder.AppendLine("Session");
                            stringBuilder.AppendLine("Exception log session");
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Channel");
                            stringBuilder.AppendLine("Exception log channel");
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("ActivityId");
                            stringBuilder.AppendLine(Guid.NewGuid().ToString("B"));
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Level");
                            stringBuilder.AppendLine(Convert.ToString(traceEventType));
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("NameSpace");
                            stringBuilder.AppendLine(nameSpaceName);
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Class");
                            stringBuilder.AppendLine(className);
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Method");
                            stringBuilder.AppendLine(methodName);
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Index");
                            stringBuilder.AppendLine(Convert.ToString(index));
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("HelpLink");
                            stringBuilder.AppendLine(string.IsNullOrEmpty(exception.HelpLink) ? notavailable : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Message");
                            stringBuilder.AppendLine(string.IsNullOrEmpty(exception.Message) ? notavailable : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("HResult");
                            stringBuilder.AppendLine(Convert.ToString(exception.HResult, 16).ToUpper());
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("Source");
                            stringBuilder.AppendLine(string.IsNullOrEmpty(exception.Source) ? notavailable : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                            stringBuilder.AppendLine("=========================================================");
                            stringBuilder.AppendLine("StackTrace");
                            stringBuilder.AppendLine(string.IsNullOrEmpty(exception.StackTrace) ? notavailable : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));
                            stringBuilder.AppendLine("=========================================================");

                            fileLogTraceListener.BaseFileName = logFileName;
                            fileLogTraceListener.WriteLine(stringBuilder.ToString());
                        }
                        catch (Exception)
                        {
                            return;
                        }
                        finally
                        {
                            logSemaphoreSlim?.Release();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 打开日志记录文件夹
        /// </summary>
        public static void OpenLogFolder()
        {
            if (isInitialized)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(logDirectory.FullName);
                    }
                    catch (Exception e)
                    {
                        WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(LogService), nameof(OpenLogFolder), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 关闭日志记录服务
        /// </summary>
        public static void CloseLog()
        {
            try
            {
                fileLogTraceListener.Dispose();
                fileLogTraceListener = null;
                logSemaphoreSlim.Dispose();
                logSemaphoreSlim = null;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
