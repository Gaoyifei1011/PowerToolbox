using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 计划任务管理页面
    /// </summary>
    public sealed partial class ScheduledTaskManagerPage : Page, INotifyPropertyChanged
    {
        private readonly string ScheduledTaskInformationString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskInformation");
        private readonly string ScheduledTaskEmptyDescriptionString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskEmptyDescription");
        private readonly string ScheduledTaskEmptyWithConditionDescriptionString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskEmptyWithConditionDescription");
        private readonly BitmapImage emptyImage = new();
        private ITaskService taskService;
        private bool isInitialized;

        private string _scheduledTaskDescription;

        public string ScheduledTaskDescription
        {
            get { return _scheduledTaskDescription; }

            set
            {
                if (!string.Equals(_scheduledTaskDescription, value))
                {
                    _scheduledTaskDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScheduledTaskDescription)));
                }
            }
        }

        private ScheduledTaskResultKind _scheduledTaskResultKind;

        public ScheduledTaskResultKind ScheduledTaskResultKind
        {
            get { return _scheduledTaskResultKind; }

            set
            {
                if (!Equals(_scheduledTaskResultKind, value))
                {
                    _scheduledTaskResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScheduledTaskResultKind)));
                }
            }
        }

        private string _scheduledTaskFailedContent;

        public string ScheduledTaskFailedContent
        {
            get { return _scheduledTaskFailedContent; }

            set
            {
                if (!string.Equals(_scheduledTaskFailedContent, value))
                {
                    _scheduledTaskFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScheduledTaskFailedContent)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private List<ScheduledTaskModel> ScheduledTaskList { get; } = [];

        public List<ScheduledTaskModel> ScheduledTaskCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ScheduledTaskManagerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        taskService = (ITaskService)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnNavigatedTo), 1, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
                await GetScheduledTaskAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 点击选中计划任务项
        /// </summary>
        private void OnCheckBoxExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 修改当前计划任务的状态
        /// </summary>
        /// TODO：未完成
        private async void OnScheduledTaskEnableStateExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask)
            {
                (bool result, Exception exception) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask?.RegisteredTask.Enabled = !scheduledTask.RegisteredTask.Enabled;
                        return ValueTuple.Create<bool, Exception>(true, null);
                    }
                    catch (Exception e)
                    {
                        return ValueTuple.Create(false, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                // TODO：显示通知，并修改按钮状态，及相关信息
            }
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        /// TODO：未完成
        private async void OnDeleteScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask)
            {
                List<ScheduledTaskModel> modifiedScheduledTaskList = [];
                (bool result, Exception exception) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask?.TaskFolder.DeleteTask(scheduledTask.RegisteredTask.Name, 0);
                        return ValueTuple.Create<bool, Exception>(true, null);
                    }
                    catch (Exception e)
                    {
                        return ValueTuple.Create(false, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                // TODO：显示通知，并修改按钮状态，及相关信息
            }
        }

        /// <summary>
        /// 打开任务计划程序路径
        /// </summary>
        private void OnOpenProcessPathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string processPath && !string.IsNullOrEmpty(processPath))
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
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnOpenProcessPathExecuteRequested), 1, e);
                    }
                });
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第一部分：计划任务管理页面——挂载的事件

        /// <summary>
        /// 搜索驱动名称
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading && ScheduledTaskList.Count > 0)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
                ScheduledTaskCollection.Clear();
                foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskList)
                {
                    if (scheduledTaskItem.Name.Contains(SearchText) || scheduledTaskItem.Author.Contains(SearchText) || scheduledTaskItem.Description.Contains(SearchText) || scheduledTaskItem.Path.Contains(SearchText) || scheduledTaskItem.ProcessPath.Contains(SearchText))
                    {
                        scheduledTaskItem.IsSelected = false;
                        ScheduledTaskList.Add(scheduledTaskItem);
                    }
                }

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 搜索驱动名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (!string.IsNullOrEmpty(SearchText) && ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading && ScheduledTaskList.Count > 0)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
                ScheduledTaskCollection.Clear();
                foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskList)
                {
                    if (scheduledTaskItem.Name.Contains(SearchText) || scheduledTaskItem.Author.Contains(SearchText) || scheduledTaskItem.Description.Contains(SearchText) || scheduledTaskItem.Path.Contains(SearchText) || scheduledTaskItem.ProcessPath.Contains(SearchText))
                    {
                        scheduledTaskItem.IsSelected = false;
                        ScheduledTaskList.Add(scheduledTaskItem);
                    }
                }

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskCollection)
            {
                scheduledTaskItem.IsSelected = true;
            }

            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskCollection)
            {
                scheduledTaskItem.IsSelected = false;
            }

            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 启用计划任务
        /// </summary>
        /// TODO：未完成
        private async void OnEnableScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [.. ScheduledTaskCollection.Where(item => item.IsSelected)];
            List<ScheduledTaskModel> modifiedScheduledTaskList = [];

            foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
            {
                await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        if (!scheduledTaskItem.IsEnabled)
                        {
                            scheduledTaskItem.IsEnabled = true;
                            modifiedScheduledTaskList.Add(scheduledTaskItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnEnableScheduledTaskClicked), 1, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                // TODO：显示通知，并修改按钮状态，及相关信息
            }
        }

        /// <summary>
        /// 禁用计划任务
        /// </summary>
        /// TODO：未完成
        private async void OnDisableScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [.. ScheduledTaskCollection.Where(item => item.IsSelected)];
            List<ScheduledTaskModel> modifiedScheduledTaskList = [];

            foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
            {
                await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        if (scheduledTaskItem.IsEnabled)
                        {
                            scheduledTaskItem.IsEnabled = false;
                            modifiedScheduledTaskList.Add(scheduledTaskItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnEnableScheduledTaskClicked), 1, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                // TODO：显示通知，并修改按钮状态，及相关信息
            }
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        /// TODO：未完成
        private async void OnDeleteScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [.. ScheduledTaskCollection.Where(item => item.IsSelected)];
            List<ScheduledTaskModel> modifiedScheduledTaskList = [];

            foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
            {
                await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTaskItem.TaskFolder.DeleteTask(scheduledTaskItem.RegisteredTask.Name, 0);
                        modifiedScheduledTaskList.Add(scheduledTaskItem);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnEnableScheduledTaskClicked), 1, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                // TODO：显示通知，并修改按钮状态，及相关信息
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetScheduledTaskAsync();
        }

        /// <summary>
        /// 使用说明
        /// </summary>
        /// TODO：未完成
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 打开计划任务程序
        /// </summary>
        private void OnOpenScheduledTaskProgramClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskschd.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnOpenScheduledTaskProgramClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取是否正在加载中或操作中
        /// </summary>
        private bool GetIsLoadingOrOperating(ScheduledTaskResultKind scheduledTaskResultKind)
        {
            return !(scheduledTaskResultKind is ScheduledTaskResultKind.Loading || scheduledTaskResultKind is ScheduledTaskResultKind.Operating);
        }

        #endregion 第一部分：计划任务管理页面——挂载的事件

        /// <summary>
        /// 获取计划任务
        /// </summary>
        private async Task GetScheduledTaskAsync()
        {
            ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
            ScheduledTaskList.Clear();
            ScheduledTaskCollection.Clear();
            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));

            List<ScheduledTaskModel> scheduledTaskList = await Task.Factory.StartNew((param) =>
            {
                List<ScheduledTaskModel> scheduledTaskList = [];

                try
                {
                    if (!taskService.Connected)
                    {
                        taskService.Connect(null, null, null, null);
                    }

                    ITaskFolder rootFolder = taskService.GetFolder("\\");
                    List<ITaskFolder> taskFolderList = [];
                    taskFolderList.Add(rootFolder);
                    taskFolderList.AddRange(GetSubTaskFolder(rootFolder));

                    List<(ITaskFolder, List<IRegisteredTask>)> registeredTaskDict = [];
                    foreach (ITaskFolder taskFolder in taskFolderList)
                    {
                        List<IRegisteredTask> registeredTaskList = GetRegisteredTask(taskFolder);
                        if (registeredTaskList.Count > 0)
                        {
                            registeredTaskDict.Add(ValueTuple.Create(taskFolder, registeredTaskList));
                        }
                    }
                    foreach ((ITaskFolder taskFolder, List<IRegisteredTask> registeredTaskList) in registeredTaskDict)
                    {
                        foreach (IRegisteredTask registeredTask in registeredTaskList)
                        {
                            ScheduledTaskModel scheduledTask = GetScheduledTasks(registeredTask, taskFolder);
                            if (scheduledTask is not null)
                            {
                                scheduledTaskList.Add(scheduledTask);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskAsync), 1, e);
                }

                return scheduledTaskList;
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

            ScheduledTaskList.AddRange(scheduledTaskList);

            if (ScheduledTaskList.Count is 0)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Failed;
                ScheduledTaskFailedContent = ScheduledTaskEmptyDescriptionString;
            }
            else
            {
                foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskList)
                {
                    MemoryStream memoryStream = await Task.Run(() =>
                    {
                        MemoryStream memoryStream = null;

                        try
                        {
                            if (!string.IsNullOrEmpty(scheduledTaskItem.ProcessPath))
                            {
                                Bitmap thumbnailBitmap = ThumbnailHelper.GetThumbnailBitmap(scheduledTaskItem.ProcessPath);

                                if (thumbnailBitmap is not null)
                                {
                                    memoryStream = new();
                                    thumbnailBitmap.Save(memoryStream, ImageFormat.Png);
                                    memoryStream.Seek(0, SeekOrigin.Begin);
                                    thumbnailBitmap.Dispose();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskAsync), 2, e);
                        }

                        return memoryStream;
                    });

                    if (memoryStream is not null)
                    {
                        try
                        {
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            scheduledTaskItem.TaskIcon = bitmapImage;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskAsync), 3, e);
                            scheduledTaskItem.TaskIcon = emptyImage;
                        }
                        finally
                        {
                            memoryStream?.Dispose();
                        }
                    }

                    if (string.IsNullOrEmpty(SearchText))
                    {
                        ScheduledTaskCollection.Add(scheduledTaskItem);
                        continue;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(scheduledTaskItem.Name) && scheduledTaskItem.Name.Contains(SearchText))
                        {
                            ScheduledTaskCollection.Add(scheduledTaskItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(scheduledTaskItem.Author) && scheduledTaskItem.Author.Contains(SearchText))
                        {
                            ScheduledTaskCollection.Add(scheduledTaskItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(scheduledTaskItem.Description) && scheduledTaskItem.Description.Contains(SearchText))
                        {
                            ScheduledTaskCollection.Add(scheduledTaskItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(scheduledTaskItem.Path) && scheduledTaskItem.Path.Contains(SearchText))
                        {
                            ScheduledTaskCollection.Add(scheduledTaskItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(scheduledTaskItem.ProcessPath) && scheduledTaskItem.ProcessPath.Contains(SearchText))
                        {
                            ScheduledTaskCollection.Add(scheduledTaskItem);
                            continue;
                        }
                    }
                }

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 获取当前任务文件夹的所有子文件夹
        /// </summary>
        private List<ITaskFolder> GetSubTaskFolder(ITaskFolder rootFolder)
        {
            List<ITaskFolder> subTaskFolderList = [];

            try
            {
                ITaskFolderCollection taskFolderCollection = rootFolder.GetFolders(0);

                for (int index = 1; index <= taskFolderCollection.Count; index++)
                {
                    ITaskFolder subFolder = taskFolderCollection[index];
                    if (string.Equals(subFolder.Path, @"\Microsoft\Windows", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    subTaskFolderList.Add(subFolder);
                    subTaskFolderList.AddRange(GetSubTaskFolder(subFolder));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetSubTaskFolder), 1, e);
            }

            return subTaskFolderList;
        }

        /// <summary>
        /// 获取当前任务文件夹的所有计划任务
        /// </summary>
        private List<IRegisteredTask> GetRegisteredTask(ITaskFolder taskFolder)
        {
            List<IRegisteredTask> registeredTaskList = [];

            try
            {
                IRegisteredTaskCollection registeredTaskCollection = taskFolder.GetTasks(1);

                for (int index = 1; index <= registeredTaskCollection.Count; index++)
                {
                    registeredTaskList.Add(registeredTaskCollection[index]);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetRegisteredTask), 1, e);
            }

            return registeredTaskList;
        }

        /// <summary>
        /// 获取计划任务信息
        /// </summary>
        private ScheduledTaskModel GetScheduledTasks(IRegisteredTask registeredTask, ITaskFolder taskFolder)
        {
            try
            {
                ITaskDefinition taskDefinition = registeredTask.Definition;
                ScheduledTaskModel scheduledTask = new()
                {
                    IsSelected = false,
                    Name = registeredTask.Name,
                    Path = registeredTask.Path,
                    LastRunTime = new DateTimeOffset(registeredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    LastTaskResult = string.Format("0x{0:X8}({1})", registeredTask.LastTaskResult, Marshal.GetExceptionForHR(registeredTask.LastTaskResult) is Exception exception ? exception.Message : "运行成功"),
                    NextRunTime = new DateTimeOffset(registeredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    State = Convert.ToString(registeredTask.State),
                    IsEnabled = registeredTask.Enabled,
                    Author = registeredTask.Definition.RegistrationInfo.Author,
                    Description = registeredTask.Definition.RegistrationInfo.Description,
                    Version = registeredTask.Definition.RegistrationInfo.Version,
                    RegisteredTask = registeredTask,
                    TaskFolder = taskFolder
                };

                IActionCollection actionCollection = registeredTask.Definition.Actions;
                for (int index = 1; index <= actionCollection.Count; index++)
                {
                    if (actionCollection[index] is IExecAction2 execAction)
                    {
                        scheduledTask.ProcessPath = execAction.Path;
                        scheduledTask.ProcessArguments = execAction.Arguments;
                        break;
                    }
                }

                return scheduledTask;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取加载计划任务是否成功
        /// </summary>
        private Visibility GetScheduledTaskSuccessfullyState(ScheduledTaskResultKind scheduledTaskResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? scheduledTaskResultKind is ScheduledTaskResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : scheduledTaskResultKind is ScheduledTaskResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索计划任务是否成功
        /// </summary>
        private Visibility CheckScheduledTaskState(ScheduledTaskResultKind scheduledTaskResultKind, ScheduledTaskResultKind comparedScheduledTaskResultKind)
        {
            return Equals(scheduledTaskResultKind, comparedScheduledTaskResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
