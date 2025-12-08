using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox.Services.Download
{
    /// <summary>
    /// Aria2 下载服务
    /// </summary>
    public static class Aria2Service
    {
        private static readonly string aria2FilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Mile.Aria2.exe");
        private static readonly string defaultAria2Arguments = "-c --enable-rpc=true --rpc-allow-origin-all=true --rpc-listen-all=true --rpc-listen-port=6600 --stop-with-process={0} -D";
        private static readonly string rpcServerLink = "http://127.0.0.1:6600/rpc";
        private static string aria2Arguments;
        private static System.Timers.Timer aria2Timer;

        private static SemaphoreSlim Aria2SemaphoreSlim { get; set; } = new(1, 1);

        private static Dictionary<string, string> Aria2DownloadDict { get; } = [];

        public static string Aria2ConfPath { get; private set; }

        public static event Action<DownloadProgress> DownloadProgress;

        /// 初始化Aria2配置文件
        /// </summary>
        public static void InitializeAria2Conf()
        {
            try
            {
                Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, 0, out string localAppDataPath);
                Aria2ConfPath = Path.Combine(localAppDataPath, "Aria2.conf");

                // 原配置文件存在且新的配置文件不存在，拷贝到指定目录
                if (!File.Exists(Aria2ConfPath))
                {
                    byte[] mileAria2 = Strings.Resources.Aria2Conf;
                    FileStream fileStream = new(Aria2ConfPath, FileMode.Create);
                    fileStream.Write(mileAria2, 0, mileAria2.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }

                // 使用自定义的配置文件目录
                aria2Arguments = string.Format("--conf-path=\"{0}\" --stop-with-process={1} -D", Aria2ConfPath, Process.GetCurrentProcess().Id);
            }
            //  发生异常时，使用默认的参数
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(InitializeAria2Conf), 1, e);
                aria2Arguments = string.Format(defaultAria2Arguments, Process.GetCurrentProcess().Id);
            }
        }

        /// <summary>
        /// 初始化运行 Aria2 下载进程和下载监控服务
        /// </summary>
        public static void Initialize()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = aria2FilePath,
                        Arguments = aria2Arguments,
                        UseShellExecute = true,
                        Verb = "open",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    aria2Timer = new System.Timers.Timer(1000);
                    aria2Timer.Elapsed += OnTimerElapsed;
                    aria2Timer.Start();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(Initialize), 1, e);
                }
            });
        }

        /// <summary>
        /// 关闭 Aria2 下载监控服务
        /// </summary>
        public static void Release()
        {
            Task.Run(() =>
            {
                try
                {
                    aria2Timer?.Stop();
                    aria2Timer?.Dispose();
                    aria2Timer = null;
                    Aria2SemaphoreSlim?.Dispose();
                    Aria2SemaphoreSlim = null;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(Release), 1, e);
                }
            });
        }

        /// <summary>
        /// 判断Aria2 rpc 端口是否存在
        /// </summary>
        public static async Task<bool> IsAria2ExistedAsync()
        {
            try
            {
                /*
                <methodCall>
                  <methodName>aria2.getVersion</methodName>
                </methodCall>
                 */
                XmlDocument versionDocument = new();
                XmlDeclaration xmlDeclaration = versionDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                versionDocument.AppendChild(xmlDeclaration);
                XmlElement methodCallElement = versionDocument.CreateElement("methodCall");
                versionDocument.AppendChild(methodCallElement);
                XmlElement methodNameElement = versionDocument.CreateElement("methodName");
                methodNameElement.InnerText = "aria2.getVersion";
                methodCallElement.AppendChild(methodNameElement);

                string versionString = versionDocument.OuterXml;
                byte[] contentBytes = Encoding.UTF8.GetBytes(versionString);
                StringContent stringContent = new(versionString);
                stringContent.Headers.ContentLength = contentBytes.Length;
                stringContent.Headers.ContentType.CharSet = "utf-8";
                HttpClient httpClient = new();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(IsAria2ExistedAsync), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        /*
                        <methodCall>
                         <methodName>aria2.addUri</methodName>
                          <params>
                            <param>
                              <value>
                                <array>
                                  <data>
                                    <value><string>url</string></value>
                                  </data>
                                </array>
                              </value>
                            </param>
                            <param>
                              <value>
                                <struct>
                                  <member>
                                    <name>out</name>
                                    <value><string>Path.GetFileName(saveFilePath)</string></value>
                                  </member>
                                  <member>
                                    <name>dir</name>
                                    <value><string>Path.GetDirectoryName(saveFilePath)</string></value>
                                  </member>
                                </struct>
                              </value>
                            </param>
                          </params>
                        </methodCall>
                        */
                        XmlDocument createDownloadElement = new();
                        XmlDeclaration xmlDeclaration = createDownloadElement.CreateXmlDeclaration("1.0", "UTF-8", null);
                        createDownloadElement.AppendChild(xmlDeclaration);
                        XmlElement methodCallElement = createDownloadElement.CreateElement("methodCall");
                        createDownloadElement.AppendChild(methodCallElement);
                        XmlElement methodNameElement = createDownloadElement.CreateElement("methodName");
                        methodNameElement.InnerText = "aria2.addUri";
                        methodCallElement.AppendChild(methodNameElement);
                        XmlElement paramsElement = createDownloadElement.CreateElement("params");
                        methodCallElement.AppendChild(paramsElement);
                        XmlElement param1Element = createDownloadElement.CreateElement("param");
                        paramsElement.AppendChild(param1Element);
                        XmlElement value1Element = createDownloadElement.CreateElement("value");
                        param1Element.AppendChild(value1Element);
                        XmlElement arrayElement = createDownloadElement.CreateElement("array");
                        value1Element.AppendChild(arrayElement);
                        XmlElement dataElement = createDownloadElement.CreateElement("data");
                        arrayElement.AppendChild(dataElement);
                        XmlElement value2Element = createDownloadElement.CreateElement("value");
                        dataElement.AppendChild(value2Element);
                        XmlElement string1Element = createDownloadElement.CreateElement("string");
                        string1Element.InnerText = url;
                        value2Element.AppendChild(string1Element);
                        XmlElement param2Element = createDownloadElement.CreateElement("param");
                        paramsElement.AppendChild(param2Element);
                        XmlElement value3Element = createDownloadElement.CreateElement("value");
                        param2Element.AppendChild(value3Element);
                        XmlElement structElement = createDownloadElement.CreateElement("struct");
                        value3Element.AppendChild(structElement);
                        XmlElement member1Element = createDownloadElement.CreateElement("member");
                        structElement.AppendChild(member1Element);
                        XmlElement name1Element = createDownloadElement.CreateElement("name");
                        name1Element.InnerText = "out";
                        member1Element.AppendChild(name1Element);
                        XmlElement value4Element = createDownloadElement.CreateElement("value");
                        member1Element.AppendChild(value4Element);
                        XmlElement string2Element = createDownloadElement.CreateElement("string");
                        string2Element.InnerText = Path.GetFileName(saveFilePath);
                        value4Element.AppendChild(string2Element);
                        XmlElement member2Element = createDownloadElement.CreateElement("member");
                        structElement.AppendChild(member2Element);
                        XmlElement name2Element = createDownloadElement.CreateElement("name");
                        name2Element.InnerText = "dir";
                        member2Element.AppendChild(name2Element);
                        XmlElement value5Element = createDownloadElement.CreateElement("value");
                        member2Element.AppendChild(value5Element);
                        XmlElement string3Element = createDownloadElement.CreateElement("string");
                        string3Element.InnerText = Path.GetDirectoryName(saveFilePath);
                        value5Element.AppendChild(string3Element);

                        string createDownloadString = createDownloadElement.OuterXml;
                        byte[] contentBytes = Encoding.UTF8.GetBytes(createDownloadString);
                        StringContent stringContent = new(createDownloadString);
                        stringContent.Headers.ContentLength = contentBytes.Length;
                        stringContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            /*
                            <methodResponse>
                             <params>
                              <param>
                               <value>
                                <string>downloadID</string>
                               </value>
                              </param>
                             </params>
                            </methodResponse>
                            */
                            string responseContent = await response.Content.ReadAsStringAsync();
                            XDocument responseDocument = XDocument.Parse(responseContent);
                            XElement resultStringElement = responseDocument.Descendants("string").FirstOrDefault();

                            if (resultStringElement is not null)
                            {
                                string gid = resultStringElement.Value;

                                Aria2SemaphoreSlim?.Wait();

                                try
                                {
                                    if (!Aria2DownloadDict.ContainsKey(gid))
                                    {
                                        Aria2DownloadDict.Add(gid, saveFilePath);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(CreateDownload), 1, e);
                                }
                                finally
                                {
                                    Aria2SemaphoreSlim?.Release();
                                }

                                DownloadProgress?.Invoke(new DownloadProgress()
                                {
                                    DownloadID = gid,
                                    DownloadProgressState = DownloadProgressState.Queued,
                                    FileName = Path.GetFileName(saveFilePath),
                                    FilePath = saveFilePath,
                                    DownloadSpeed = 0,
                                    CompletedSize = 0,
                                    TotalSize = 0,
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(CreateDownload), 2, e);
                }
            });
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(string downloadID)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        /*
                         <methodCall>
                           <methodName>aria2.unpause</methodName>
                           <params>
                             <param>
                               <value>
                                 <string>downloadID</string>
                               </value>
                             </param>
                           </params>
                         </methodCall>
                         */
                        XmlDocument continueDownloadElement = new();
                        XmlDeclaration xmlDeclaration = continueDownloadElement.CreateXmlDeclaration("1.0", "UTF-8", null);
                        continueDownloadElement.AppendChild(xmlDeclaration);
                        XmlElement methodCallElement = continueDownloadElement.CreateElement("methodCall");
                        continueDownloadElement.AppendChild(methodCallElement);
                        XmlElement methodNameElement = continueDownloadElement.CreateElement("methodName");
                        methodNameElement.InnerText = "aria2.unpause";
                        methodCallElement.AppendChild(methodNameElement);
                        XmlElement paramsElement = continueDownloadElement.CreateElement("params");
                        methodCallElement.AppendChild(paramsElement);
                        XmlElement paramElement = continueDownloadElement.CreateElement("param");
                        paramsElement.AppendChild(paramElement);
                        XmlElement valueElement = continueDownloadElement.CreateElement("value");
                        paramElement.AppendChild(valueElement);
                        XmlElement stringElement = continueDownloadElement.CreateElement("string");
                        stringElement.InnerText = downloadID;
                        valueElement.AppendChild(stringElement);

                        string continueString = continueDownloadElement.OuterXml;
                        byte[] contentBytes = Encoding.UTF8.GetBytes(continueString);
                        StringContent stringContent = new(continueString);
                        stringContent.Headers.ContentLength = contentBytes.Length;
                        stringContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            /*
                            <methodResponse>
                             <params>
                              <param>
                               <value>
                                <string>downloadID</string>
                               </value>
                              </param>
                             </params>
                            </methodResponse>
                            */
                            string responseContent = await response.Content.ReadAsStringAsync();
                            XDocument responseDocument = XDocument.Parse(responseContent);
                            XElement resultStringElement = responseDocument.Descendants("string").FirstOrDefault();

                            if (resultStringElement is not null)
                            {
                                string gid = resultStringElement.Value;

                                Aria2SemaphoreSlim?.Wait();

                                try
                                {
                                    if (Aria2DownloadDict.TryGetValue(gid, out string saveFilePath))
                                    {
                                        DownloadProgress?.Invoke(new DownloadProgress()
                                        {
                                            DownloadID = gid,
                                            DownloadProgressState = DownloadProgressState.Queued,
                                            FileName = Path.GetFileName(saveFilePath),
                                            FilePath = saveFilePath,
                                            DownloadSpeed = 0,
                                            CompletedSize = 0,
                                            TotalSize = 0,
                                        });
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(ContinueDownload), 1, e);
                                }
                                finally
                                {
                                    Aria2SemaphoreSlim?.Release();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(ContinueDownload), 2, e);
                }
            });
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public static void PauseDownload(string downloadID)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        /*
                        <methodCall>
                          <methodName>aria2.forcePause</methodName>
                          <params>
                            <param>
                              <value>
                                <string>downloadID</string>
                              </value>
                            </param>
                          </params>
                        </methodCall>
                        */
                        XmlDocument pauseDownloadElement = new();
                        XmlDeclaration xmlDeclaration = pauseDownloadElement.CreateXmlDeclaration("1.0", "UTF-8", null);
                        pauseDownloadElement.AppendChild(xmlDeclaration);
                        XmlElement methodCallElement = pauseDownloadElement.CreateElement("methodCall");
                        pauseDownloadElement.AppendChild(methodCallElement);
                        XmlElement methodNameElement = pauseDownloadElement.CreateElement("methodName");
                        methodNameElement.InnerText = "aria2.forcePause";
                        methodCallElement.AppendChild(methodNameElement);
                        XmlElement paramsElement = pauseDownloadElement.CreateElement("params");
                        methodCallElement.AppendChild(paramsElement);
                        XmlElement paramElement = pauseDownloadElement.CreateElement("param");
                        paramsElement.AppendChild(paramElement);
                        XmlElement valueElement = pauseDownloadElement.CreateElement("value");
                        paramElement.AppendChild(valueElement);
                        XmlElement stringElement = pauseDownloadElement.CreateElement("string");
                        stringElement.InnerText = downloadID;
                        valueElement.AppendChild(stringElement);

                        string pauseDownloadString = pauseDownloadElement.OuterXml;
                        byte[] contentBytes = Encoding.UTF8.GetBytes(pauseDownloadString);
                        StringContent stringContent = new(pauseDownloadString);
                        stringContent.Headers.ContentLength = contentBytes.Length;
                        stringContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            /*
                            <methodResponse>
                              <params>
                               <param>
                                <value>
                                 <string>downloadID</string>
                                </value>
                               </param>
                              </params>
                             </methodResponse>
                             */
                            string responseContent = await response.Content.ReadAsStringAsync();
                            XDocument responseDocument = XDocument.Parse(responseContent);
                            XElement resultStringElement = responseDocument.Descendants("string").FirstOrDefault();

                            if (resultStringElement is not null)
                            {
                                string gid = resultStringElement.Value;

                                Aria2SemaphoreSlim?.Wait();

                                try
                                {
                                    if (Aria2DownloadDict.TryGetValue(gid, out string saveFilePath))
                                    {
                                        DownloadProgress?.Invoke(new DownloadProgress()
                                        {
                                            DownloadID = gid,
                                            DownloadProgressState = DownloadProgressState.Paused,
                                            FileName = Path.GetFileName(saveFilePath),
                                            FilePath = saveFilePath,
                                            DownloadSpeed = 0,
                                            CompletedSize = 0,
                                            TotalSize = 0,
                                        });
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(PauseDownload), 1, e);
                                }
                                finally
                                {
                                    Aria2SemaphoreSlim?.Release();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(PauseDownload), 2, e);
                }
            });
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(string downloadID)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        /*
                         <methodCall>
                           <methodName>aria2.forceRemove</methodName>
                           <params>
                             <param>
                               <value>
                                 <string>downloadID</string>
                               </value>
                             </param>
                           </params>
                         </methodCall>
                         */
                        XmlDocument deleteDownloadElement = new();
                        XmlDeclaration xmlDeclaration = deleteDownloadElement.CreateXmlDeclaration("1.0", "UTF-8", null);
                        deleteDownloadElement.AppendChild(xmlDeclaration);
                        XmlElement methodCallElement = deleteDownloadElement.CreateElement("methodCall");
                        deleteDownloadElement.AppendChild(methodCallElement);
                        XmlElement methodNameElement = deleteDownloadElement.CreateElement("methodName");
                        methodNameElement.InnerText = "aria2.forceRemove";
                        methodCallElement.AppendChild(methodNameElement);
                        XmlElement paramsElement = deleteDownloadElement.CreateElement("params");
                        methodCallElement.AppendChild(paramsElement);
                        XmlElement paramElement = deleteDownloadElement.CreateElement("param");
                        paramsElement.AppendChild(paramElement);
                        XmlElement valueElement = deleteDownloadElement.CreateElement("value");
                        paramElement.AppendChild(valueElement);
                        XmlElement stringElement = deleteDownloadElement.CreateElement("string");
                        stringElement.InnerText = downloadID;
                        valueElement.AppendChild(stringElement);

                        string deleteDownloadString = deleteDownloadElement.OuterXml;
                        byte[] contentBytes = Encoding.UTF8.GetBytes(deleteDownloadString);
                        StringContent stringContent = new(deleteDownloadString);
                        stringContent.Headers.ContentLength = contentBytes.Length;
                        stringContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            /*
                            <methodResponse>
                             <params>
                              <param>
                               <value>
                                <string>downloadID</string>
                               </value>
                              </param>
                             </params>
                            </methodResponse>
                            */
                            string responseContent = await response.Content.ReadAsStringAsync();
                            XDocument responseDocument = XDocument.Parse(responseContent);
                            XElement resultStringElement = responseDocument.Descendants("string").FirstOrDefault();

                            if (resultStringElement is not null)
                            {
                                string gid = resultStringElement.Value;

                                Aria2SemaphoreSlim?.Wait();

                                try
                                {
                                    if (Aria2DownloadDict.TryGetValue(gid, out string saveFilePath))
                                    {
                                        Aria2DownloadDict.Remove(gid);
                                        DownloadProgress?.Invoke(new DownloadProgress()
                                        {
                                            DownloadID = gid,
                                            DownloadProgressState = DownloadProgressState.Deleted,
                                            FileName = Path.GetFileName(saveFilePath),
                                            FilePath = saveFilePath,
                                            DownloadSpeed = 0,
                                            CompletedSize = 0,
                                            TotalSize = 0,
                                        });
                                    }

                                    await RemoveResultAsync();
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(DeleteDownload), 1, e);
                                }
                                finally
                                {
                                    Aria2SemaphoreSlim?.Release();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(DeleteDownload), 2, e);
                }
            });
        }

        /// <summary>
        /// 汇报下载任务状态信息
        /// </summary>
        private static async Task<(bool, DownloadProgressState, double, double, double)> TellStatusAsync(string downloadID)
        {
            bool isTellStatusSuccessfully = false;
            DownloadProgressState downloadProgressState = DownloadProgressState.Failed;
            double completedSize = 0;
            double totalSize = 0;
            double downloadSpeed = 0;

            try
            {
                if (await IsAria2ExistedAsync())
                {
                    XmlDocument tellStatusDocument = new();
                    XmlDeclaration xmlDecl = tellStatusDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                    tellStatusDocument.AppendChild(xmlDecl);
                    XmlElement methodCallElement = tellStatusDocument.CreateElement("methodCall");
                    tellStatusDocument.AppendChild(methodCallElement);
                    XmlElement methodNameElement = tellStatusDocument.CreateElement("methodName");
                    methodNameElement.InnerText = "aria2.tellStatus";
                    methodCallElement.AppendChild(methodNameElement);
                    XmlElement paramsElement = tellStatusDocument.CreateElement("params");
                    methodCallElement.AppendChild(paramsElement);
                    XmlElement param1Element = tellStatusDocument.CreateElement("param");
                    paramsElement.AppendChild(param1Element);
                    XmlElement value1Element = tellStatusDocument.CreateElement("value");
                    param1Element.AppendChild(value1Element);
                    XmlElement string1Element = tellStatusDocument.CreateElement("string");
                    string1Element.InnerText = downloadID;
                    value1Element.AppendChild(string1Element);
                    XmlElement param2Element = tellStatusDocument.CreateElement("param");
                    paramsElement.AppendChild(param2Element);
                    XmlElement value2Element = tellStatusDocument.CreateElement("value");
                    param2Element.AppendChild(value2Element);
                    XmlElement arrayElement = tellStatusDocument.CreateElement("array");
                    value2Element.AppendChild(arrayElement);
                    XmlElement dataElement = tellStatusDocument.CreateElement("data");
                    arrayElement.AppendChild(dataElement);
                    string[] fieldsArray = ["gid", "status", "totalLength", "completedLength", "downloadSpeed"];
                    foreach (string field in fieldsArray)
                    {
                        XmlElement value3Element = tellStatusDocument.CreateElement("value");
                        dataElement.AppendChild(value3Element);

                        XmlElement string2Element = tellStatusDocument.CreateElement("string");
                        string2Element.InnerText = field;
                        value3Element.AppendChild(string2Element);
                    }

                    string tellStatusString = tellStatusDocument.OuterXml;
                    byte[] contentBytes = Encoding.UTF8.GetBytes(tellStatusString);
                    StringContent stringContent = new(tellStatusString);
                    stringContent.Headers.ContentLength = contentBytes.Length;
                    stringContent.Headers.ContentType.CharSet = "utf-8";
                    HttpClient httpClient = new();
                    HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);

                    // 请求成功
                    if (response.IsSuccessStatusCode)
                    {
                        isTellStatusSuccessfully = true;
                        /*
                        <methodResponse>
                         <params>
                          <param>
                           <value>
                            <struct>
                             <member>
                              <name>completedLength</name>
                              <value>
                               <string>61849600</string>
                              </value>
                             </member>
                             <member>
                              <name>downloadSpeed</name>
                              <value>
                               <string>504017</string>
                              </value>
                             </member>
                             <member>
                              <name>gid</name>
                              <value>
                               <string>3a499d980f961675</string>
                              </value>
                             </member>
                             <member>
                              <name>status</name>
                              <value>
                               <string>active</string>
                              </value>
                             </member>
                             <member>
                              <name>totalLength</name>
                              <value>
                               <string>272726240</string>
                              </value>
                             </member>
                            </struct>
                           </value>
                          </param>
                         </params>
                        </methodResponse>
                        */
                        string responseContent = await response.Content.ReadAsStringAsync();
                        XDocument responseDocument = XDocument.Parse(responseContent);
                        string status = GetValue(responseDocument, "status", "[Unknown]");
                        completedSize = Convert.ToDouble(GetValue(responseDocument, "completedLength", "0"));
                        totalSize = Convert.ToDouble(GetValue(responseDocument, "totalLength", "0"));
                        downloadSpeed = Convert.ToDouble(GetValue(responseDocument, "downloadSpeed", "0"));

                        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Downloading;
                        }
                        else if (string.Equals(status, "waiting", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Queued;
                        }
                        else if (string.Equals(status, "paused", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Paused;
                        }
                        else if (string.Equals(status, "error", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Failed;
                        }
                        else if (string.Equals(status, "complete", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Finished;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(TellStatusAsync), 1, e);
            }

            return ValueTuple.Create(isTellStatusSuccessfully, downloadProgressState, completedSize, totalSize, downloadSpeed);
        }

        /// <summary>
        /// 获取下载进度状态
        /// </summary>
        private static async void OnTimerElapsed(object sender, ElapsedEventArgs args)
        {
            Aria2SemaphoreSlim?.Wait();

            try
            {
                List<string> finishedDownloadKeyList = [];

                foreach (KeyValuePair<string, string> aria2DownloadItem in Aria2DownloadDict)
                {
                    (bool isTellStatusSuccessfully, DownloadProgressState downloadProgressState, double completedSize, double totalSize, double downloadSpeed) = await TellStatusAsync(aria2DownloadItem.Key);

                    if (isTellStatusSuccessfully)
                    {
                        DownloadProgress?.Invoke(new DownloadProgress()
                        {
                            DownloadID = aria2DownloadItem.Key,
                            DownloadProgressState = downloadProgressState,
                            FileName = Path.GetFileName(aria2DownloadItem.Value),
                            FilePath = aria2DownloadItem.Value,
                            DownloadSpeed = downloadSpeed,
                            CompletedSize = completedSize,
                            TotalSize = totalSize,
                        });

                        // 任务下载失败或完成时从 Aria2 进程中删除队列任务
                        if (downloadProgressState is DownloadProgressState.Failed || downloadProgressState is DownloadProgressState.Finished)
                        {
                            finishedDownloadKeyList.Add(aria2DownloadItem.Key);
                        }
                    }
                }

                foreach (string finishedDownloadKey in finishedDownloadKeyList)
                {
                    Aria2DownloadDict.Remove(finishedDownloadKey);
                }

                await RemoveResultAsync();
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(OnTimerElapsed), 1, e);
            }
            finally
            {
                Aria2SemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 移除下载完成结果
        /// </summary>
        private static async Task RemoveResultAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        /*
                        <methodCall>
                         <methodName>aria2.purgeDownloadResult</methodName>
                        </methodCall>
                        */
                        XmlDocument removeResultDocument = new();
                        XmlDeclaration xmlDeclaration = removeResultDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                        removeResultDocument.AppendChild(xmlDeclaration);
                        XmlElement methodCallElement = removeResultDocument.CreateElement("methodCall");
                        removeResultDocument.AppendChild(methodCallElement);
                        XmlElement methodNameElement = removeResultDocument.CreateElement("methodName");
                        methodNameElement.InnerText = "aria2.purgeDownloadResult";
                        methodCallElement.AppendChild(methodNameElement);

                        string removeResultString = removeResultDocument.OuterXml;
                        byte[] contentBytes = Encoding.UTF8.GetBytes(removeResultString);
                        StringContent stringContent = new(removeResultString);
                        stringContent.Headers.ContentLength = contentBytes.Length;
                        stringContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        await httpClient.PostAsync(new Uri(rpcServerLink), stringContent);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(Aria2Service), nameof(RemoveResultAsync), 1, e);
                }
            });
        }

        private static string GetValue(XDocument xDocument, string name, string defaultValue)
        {
            return xDocument.Descendants("member").FirstOrDefault(item => string.Equals(item.Element("name")?.Value, name))?.Element("value")?.Element("string")?.Value ?? defaultValue;
        }
    }
}
