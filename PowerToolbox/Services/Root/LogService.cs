using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly string notAvailable = "N/A";
        private static SemaphoreSlim logSemaphoreSlim = new(1, 1);
        private static bool isInitialized = false;
        private static DirectoryInfo logDirectory;

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static void Initialize()
        {
            Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, 0, out string localAppDataPath);

            if (!string.IsNullOrEmpty(localAppDataPath))
            {
                try
                {
                    if (Directory.Exists(localAppDataPath))
                    {
                        string logFolderPath = Path.Combine(localAppDataPath, "Logs");
                        logDirectory = Directory.CreateDirectory(logFolderPath);
                        isInitialized = true;
                    }
                }
                catch (Exception)
                {
                    return;
                }
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

                    try
                    {
                        if (!Directory.Exists(logDirectory.FullName))
                        {
                            Directory.CreateDirectory(logDirectory.FullName);
                        }

                        string logFileName = string.Format("Logs-{0}-{1}-{2}-{3:D2}-{4}.xml", nameSpaceName, className, methodName, index, DateTimeOffset.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));

                        /*
                        <LogRecord>
                         <Session>Exception log session</Session>
                         <Channel>Exception log channel</Channel>
                         <ActivityId>Guid.NewGuid().ToString("B")</ActivityId>
                         <Data>
                          <Level>Convert.ToString(eventLevel)</Level>
                          <NameSpace>nameSpaceName</NameSpace>
                          <Class>className</Class>
                          <Method>methodName</Method>
                          <Index>index</Index>
                          <HelpLink>string.IsNullOrEmpty(exception.HelpLink) ? unknown : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' ')</HelpLink>
                          <Message>string.IsNullOrEmpty(exception.Message) ? unknown : exception.Message.Replace('\r', ' ').Replace('\n', ' ')</Message>
                          <HResult>Convert.ToString(exception.HResult, 16).ToUpper()</HResult>
                          <Source>string.IsNullOrEmpty(exception.Source) ? unknown : exception.Source.Replace('\r', ' ').Replace('\n', ' ')</Source>
                          <StackTrace>string.IsNullOrEmpty(exception.StackTrace) ? unknown : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' ')</StackTrace>
                         </Data>
                        </LogRecord>
                        */
                        XmlDocument xmlDocument = new();
                        XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                        xmlDocument.AppendChild(xmlDeclaration);
                        XmlElement logRecordElement = xmlDocument.CreateElement("LogRecord");
                        xmlDocument.AppendChild(logRecordElement);
                        XmlElement sessionElement = xmlDocument.CreateElement("Session");
                        sessionElement.InnerText = "Exception log session";
                        logRecordElement.AppendChild(sessionElement);
                        XmlElement channelElement = xmlDocument.CreateElement("Channel");
                        channelElement.InnerText = "Exception log channel";
                        logRecordElement.AppendChild(channelElement);
                        XmlElement activityIdElement = xmlDocument.CreateElement("ActivityId");
                        activityIdElement.InnerText = Guid.NewGuid().ToString("B");
                        logRecordElement.AppendChild(activityIdElement);
                        XmlElement dataElement = xmlDocument.CreateElement("Data");
                        logRecordElement.AppendChild(dataElement);
                        XmlElement levelElement = xmlDocument.CreateElement("Level");
                        levelElement.InnerText = Convert.ToString(traceEventType);
                        dataElement.AppendChild(levelElement);
                        XmlElement nameSpaceElement = xmlDocument.CreateElement("NameSpace");
                        nameSpaceElement.InnerText = nameSpaceName;
                        dataElement.AppendChild(nameSpaceElement);
                        XmlElement classElement = xmlDocument.CreateElement("Class");
                        classElement.InnerText = className;
                        dataElement.AppendChild(classElement);
                        XmlElement methodElement = xmlDocument.CreateElement("Method");
                        methodElement.InnerText = methodName;
                        dataElement.AppendChild(methodElement);
                        XmlElement indexElement = xmlDocument.CreateElement("Index");
                        indexElement.InnerText = Convert.ToString(index);
                        dataElement.AppendChild(indexElement);
                        XmlElement helpLinkElement = xmlDocument.CreateElement("HelpLink");
                        helpLinkElement.InnerText = string.IsNullOrEmpty(exception.HelpLink) ? notAvailable : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' ');
                        dataElement.AppendChild(helpLinkElement);
                        XmlElement messageElement = xmlDocument.CreateElement("Message");
                        messageElement.InnerText = string.IsNullOrEmpty(exception.Message) ? notAvailable : exception.Message.Replace('\r', ' ').Replace('\n', ' ');
                        dataElement.AppendChild(messageElement);
                        XmlElement hResultElement = xmlDocument.CreateElement("HResult");
                        hResultElement.InnerText = Convert.ToString(exception.HResult, 16).ToUpper();
                        dataElement.AppendChild(hResultElement);
                        XmlElement sourceElement = xmlDocument.CreateElement("Source");
                        sourceElement.InnerText = string.IsNullOrEmpty(exception.Source) ? notAvailable : exception.Source.Replace('\r', ' ').Replace('\n', ' ');
                        dataElement.AppendChild(sourceElement);

                        XmlElement stackTraceElement = xmlDocument.CreateElement("StackTrace");
                        stackTraceElement.InnerText = string.IsNullOrEmpty(exception.StackTrace) ? notAvailable : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' ');
                        dataElement.AppendChild(stackTraceElement);
                        xmlDocument.Save(Path.Combine(logDirectory.FullName, logFileName));
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
                        WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LogService), nameof(OpenLogFolder), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 清除所有的日志文件
        /// </summary>
        public static async Task<bool> ClearLogAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    string[] logFiles = Directory.GetFiles(logDirectory.FullName);
                    foreach (string logFile in logFiles)
                    {
                        File.Delete(logFile);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LogService), nameof(ClearLogAsync), 1, e);
                    return false;
                }
            });
        }

        /// <summary>
        /// 关闭日志记录服务
        /// </summary>
        public static void CloseLog()
        {
            logSemaphoreSlim.Dispose();
            logSemaphoreSlim = null;
        }
    }
}
