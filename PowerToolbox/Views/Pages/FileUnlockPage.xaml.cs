using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件解锁页面
    /// </summary>
    public sealed partial class FileUnlockPage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.FileUnlockResource.GetString("DragOverContent");
        private readonly string FileNotUseString = ResourceService.FileUnlockResource.GetString("FileNotUse");
        private readonly string NoMultiFileString = ResourceService.FileUnlockResource.GetString("NoMultiFile");
        private readonly string ParseFileFailedString = ResourceService.FileUnlockResource.GetString("ParseFileFailed");
        private readonly string ParseFileSuccessfullyString = ResourceService.FileUnlockResource.GetString("ParseFileSuccessfully");
        private readonly string ParsingFileString = ResourceService.FileUnlockResource.GetString("ParsingFile");
        private readonly string SelectFileString = ResourceService.FileUnlockResource.GetString("SelectFile");
        private readonly string WelcomeString = ResourceService.FileUnlockResource.GetString("Welcome");

        private InfoBarSeverity _resultSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity ResultSeverity
        {
            get { return _resultSeverity; }

            set
            {
                if (!Equals(_resultSeverity, value))
                {
                    _resultSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultSeverity)));
                }
            }
        }

        private string _stateInfoText;

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set
            {
                if (!string.Equals(_stateInfoText, value))
                {
                    _stateInfoText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StateInfoText)));
                }
            }
        }

        private bool _isRingActive = false;

        public bool IsRingActive
        {
            get { return _isRingActive; }

            set
            {
                if (!Equals(_isRingActive, value))
                {
                    _isRingActive = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRingActive)));
                }
            }
        }

        private bool _resultCotnrolVisable = false;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set
            {
                if (!Equals(_resultCotnrolVisable, value))
                {
                    _resultCotnrolVisable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultControlVisable)));
                }
            }
        }

        private string _fileName = string.Empty;

        public string FileName
        {
            get { return _fileName; }

            set
            {
                if (!string.Equals(_fileName, value))
                {
                    _fileName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
                }
            }
        }

        private ObservableCollection<ProcessInfoModel> ProcessInfoCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileUnlockPage()
        {
            InitializeComponent();
            StateInfoText = WelcomeString;
        }

        #region 第一部分：第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override async void OnDragOver(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                if (dragItemsList.Count is 1)
                {
                    args.AcceptedOperation = DataPackageOperation.Copy;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = DragOverContentString;
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = NoMultiFileString;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDragOver), 1, e);
            }
            finally
            {
                args.Handled = true;
                dragOperationDeferral.Complete();
            }
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            string filePath = string.Empty;
            try
            {
                DataPackageView dataPackageView = args.DataView;
                IReadOnlyList<IStorageItem> filesList = await Task.Run(async () =>
                {
                    try
                    {
                        if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                        {
                            return await dataPackageView.GetStorageItemsAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Warning, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDrop), 1, e);
                    }

                    return null;
                });

                if (filesList is not null && filesList.Count is 1)
                {
                    filePath = filesList[0].Path;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                await ParseFileAsync(filePath);
            }
        }

        #endregion 第一部分：第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 终止进程
        /// </summary>
        private async void OnTerminateProcessExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is int processid && processid is not 0)
            {
                bool result = await Task.Run(() =>
                {
                    try
                    {
                        Process process = Process.GetProcessById(processid);
                        process?.Kill();
                        return true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnTerminateProcessExecuteRequested), 1, e);
                        return false;
                    }
                });

                if (result)
                {
                    foreach (ProcessInfoModel processInfoItem in ProcessInfoCollection)
                    {
                        if (processInfoItem.ProcessId.Equals(processid))
                        {
                            ProcessInfoCollection.Remove(processInfoItem);
                            break;
                        }
                    }
                }

                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.TerminateProcess, result));
            }
        }

        /// <summary>
        /// 打开本地目录
        /// </summary>

        private void OnOpenProcessPathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && !string.IsNullOrEmpty(filePath))
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
                                if (pidlList != IntPtr.Zero)
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
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnOpenProcessPathExecuteRequested), 1, e);
                    }
                });
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：文件解锁页面——挂载的事件

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OnOpenTaskManagerClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnOpenTaskManagerClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开本地文件
        /// </summary>
        private async void OnOpenLocalFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Title = SelectFileString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                await ParseFileAsync(openFileDialog.FileName);
            }
            openFileDialog.Dispose();
        }

        #endregion 第三部分：文件解锁页面——挂载的事件

        /// <summary>
        /// 解析文件
        /// </summary>
        public async Task ParseFileAsync(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            ResultSeverity = InfoBarSeverity.Informational;
            IsRingActive = true;
            ResultControlVisable = false;
            StateInfoText = string.Format(ParsingFileString, FileName);
            ProcessInfoCollection.Clear();

            (bool parseSuccessfully, List<(string userName, Process process)> processList) = await Task.Run(() =>
            {
                bool parseSuccessfully = false;

                List<(string userName, Process process)> processList = [];
                uint handle = 0;

                try
                {
                    Guid keyGuid = Guid.NewGuid();
                    int result = RstrtmgrLibrary.RmStartSession(out handle, 0, Convert.ToString(keyGuid));

                    if (result is 0)
                    {
                        uint pnProcInfo = 0;
                        string[] resources = [filePath];

                        result = RstrtmgrLibrary.RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                        if (result is 0)
                        {
                            result = RstrtmgrLibrary.RmGetList(handle, out uint pnProcInfoNeeded, ref pnProcInfo, null, out uint lpdwRebootReasons);
                            parseSuccessfully = true;

                            if (result is 234)
                            {
                                RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                                pnProcInfo = pnProcInfoNeeded;
                                result = RstrtmgrLibrary.RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, out lpdwRebootReasons);

                                if (result is 0)
                                {
                                    for (int index = 0; index < pnProcInfo; index++)
                                    {
                                        try
                                        {
                                            Process process = Process.GetProcessById(processInfo[index].Process.dwProcessId);
                                            processList.Add(ValueTuple.Create(GetProcessUserName(process.Id), process));
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(ParseFileAsync), 1, e);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(ParseFileAsync), 2, e);
                }
                finally
                {
                    RstrtmgrLibrary.RmEndSession(handle);
                }

                return ValueTuple.Create(parseSuccessfully, processList);
            });

            IsRingActive = false;
            if (parseSuccessfully)
            {
                ResultControlVisable = true;
                ResultSeverity = InfoBarSeverity.Success;
                StateInfoText = string.Format(ParseFileSuccessfullyString, FileName);

                if (processList.Count > 0)
                {
                    for (int index = 0; index < processList.Count; index++)
                    {
                        try
                        {
                            Process process = processList[index].process;
                            Icon icon = Icon.ExtractAssociatedIcon(Path.Combine(process.MainModule.FileName));
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());

                            ProcessInfoCollection.Add(new ProcessInfoModel()
                            {
                                ProcessName = Path.GetFileName(process.MainModule.FileVersionInfo.FileName),
                                ProcessId = process.Id,
                                ProcessUser = processList[index].userName,
                                ProcessPath = process.MainModule.FileName,
                                ProcessIcon = bitmapImage
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(ParseFileAsync), 3, e);
                            continue;
                        }
                    }
                }
            }
            else
            {
                ResultControlVisable = false;
                ResultSeverity = InfoBarSeverity.Error;
                StateInfoText = string.Format(ParseFileFailedString, FileName);
            }
        }

        /// <summary>
        /// 通过进程 Id 来获取进程的用户名
        /// </summary>
        private static string GetProcessUserName(int processId)
        {
            string name = string.Empty;

            SelectQuery selectQuery = new("select * from Win32_Process where processID=" + processId);
            ManagementObjectSearcher managementObjectSearcher = new(selectQuery);
            try
            {
                foreach (ManagementObject managementObject in managementObjectSearcher.Get().Cast<ManagementObject>())
                {
                    ManagementBaseObject inManagementBaseObject = null;
                    ManagementBaseObject outManagementBaseObject = null;

                    inManagementBaseObject = managementObject.GetMethodParameters("GetOwner");
                    outManagementBaseObject = managementObject.InvokeMethod("GetOwner", inManagementBaseObject, null);

                    name = Convert.ToString(outManagementBaseObject["User"]);
                    break;
                }
            }
            catch
            {
                name = "SYSTEM";
            }

            return name;
        }
    }
}
