using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TaskScheduler;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 计划任务管理页面
    /// </summary>
    internal sealed partial class ScheduledTaskManagerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string NotAvailableString = ResourceService.ScheduledTaskManagerResource.GetString("NotAvailable");
        private readonly string ScheduledTaskCountInfoString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskCountInfo");
        private readonly string ScheduledTaskEmptyDescriptionString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskEmptyDescription");
        private readonly string SelectFolderString = ResourceService.ScheduledTaskManagerResource.GetString("SelectFolder");
        private readonly string TaskStateDisabledString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateDisabled");
        private readonly string TaskStateQueuedString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateQueued");
        private readonly string TaskStateReadyString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateReady");
        private readonly string TaskStateRunningString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateRunning");
        private readonly BitmapImage emptyImage = new();
        private ITaskService taskService;
        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private ScheduledTaskResultKind _scheduledTaskResultKind;

        private ScheduledTaskResultKind ScheduledTaskResultKind
        {
            get { return _scheduledTaskResultKind; }

            set
            {
                if (!Equals(_scheduledTaskResultKind, value))
                {
                    _scheduledTaskResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(ScheduledTaskResultKind)));
                }
            }
        }

        private int _selectedItemsCount;

        private int SelectedItemsCount
        {
            get { return _selectedItemsCount; }

            set
            {
                if (!Equals(_selectedItemsCount, value))
                {
                    _selectedItemsCount = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItemsCount)));
                }
            }
        }

        private string _scheduledTaskFailedContent;

        private string ScheduledTaskFailedContent
        {
            get { return _scheduledTaskFailedContent; }

            set
            {
                if (!string.Equals(_scheduledTaskFailedContent, value))
                {
                    _scheduledTaskFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(ScheduledTaskFailedContent)));
                }
            }
        }

        private bool _isModifiedFailed;

        private bool IsModifiedFailed
        {
            get { return _isModifiedFailed; }

            set
            {
                if (!Equals(_isModifiedFailed, value))
                {
                    _isModifiedFailed = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifiedFailed)));
                }
            }
        }

        private WinRTObservableCollection<ScheduledTaskFailedModel> ScheduledTaskFailedCollection { get; } = [];

        private WinRTObservableCollection<ScheduledTaskModel> ScheduledTaskCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal ScheduledTaskManagerPage()
        {
            InitializeComponent();
            InitializeSchedulerTaskService();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await GetScheduledTaskAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 禁用计划任务
        /// </summary>
        private async void OnDisableScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask.RegisteredTask is not null)
            {
                List<ScheduledTaskModel> scheduledTaskList = [scheduledTask];
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind.Disable, scheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Disable, scheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Disable, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 启用计划任务
        /// </summary>
        private async void OnEnableScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask.RegisteredTask is not null)
            {
                List<ScheduledTaskModel> scheduledTaskList = [scheduledTask];
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind.Enable, scheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Enable, scheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Enable, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 运行计划任务
        /// </summary>
        private async void OnRunScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask.RegisteredTask is not null)
            {
                List<ScheduledTaskModel> scheduledTaskList = [scheduledTask];
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind.Run, scheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Run, scheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Run, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 结束计划任务
        /// </summary>
        private async void OnStopScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask.RegisteredTask is not null)
            {
                List<ScheduledTaskModel> scheduledTaskList = [scheduledTask];
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind.Stop, scheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Stop, scheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Stop, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 导出计划任务
        /// </summary>
        private async void OnExportScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask.RegisteredTask is not null)
            {
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    List<ScheduledTaskModel> scheduledTaskList = [scheduledTask];
                    (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await ExportScheduledTaskListAsync(openFolderDialog.SelectedPath, scheduledTaskList);
                    await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Export, scheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                    await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Export, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
                }
                openFolderDialog.Dispose();
            }
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        private async void OnDeleteScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask.RegisteredTask is not null)
            {
                List<ScheduledTaskModel> scheduledTaskList = [scheduledTask];
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await DeleteScheduledTaskListAsync(scheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Delete, scheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Delete, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 打开任务计划程序路径
        /// </summary>
        private void OnOpenProcessPathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string processPath && !string.IsNullOrEmpty(processPath))
            {
                OpenProcessPath(processPath);
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 点击关闭按钮关闭使用说明
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (ScheduledTaskSplitView.IsPaneOpen)
            {
                ScheduledTaskSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        private void OnRunAsAdministratorClicked(object sender, RoutedEventArgs args)
        {
            RunAsAdministrator();
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            if (ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading)
            {
                ScheduledTaskManagerListView.SelectAll();
                SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            if (ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading)
            {
                ScheduledTaskManagerListView.DeselectRange(new(0, (uint)ScheduledTaskManagerListView.Items.Count));
                SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            if (ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading)
            {
                List<Microsoft.UI.Xaml.Data.ItemIndexRange> selectedRangeList = [.. ScheduledTaskManagerListView.SelectedRanges];
                ScheduledTaskManagerListView.SelectAll();
                foreach (Microsoft.UI.Xaml.Data.ItemIndexRange itemIndexRange in selectedRangeList)
                {
                    ScheduledTaskManagerListView.DeselectRange(itemIndexRange);
                }
                SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
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
        /// 运行计划任务
        /// </summary>
        private async void OnRunScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [];
            foreach (object item in ScheduledTaskManagerListView.SelectedItems)
            {
                if (item is ScheduledTaskModel scheduledTaskItem)
                {
                    selectedScheduledTaskList.Add(scheduledTaskItem);
                }
            }

            if (selectedScheduledTaskList.Count > 0)
            {
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind.Run, selectedScheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Run, selectedScheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Run, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 结束计划任务
        /// </summary>
        private async void OnStopScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [];
            foreach (object item in ScheduledTaskManagerListView.SelectedItems)
            {
                if (item is ScheduledTaskModel scheduledTaskItem)
                {
                    selectedScheduledTaskList.Add(scheduledTaskItem);
                }
            }

            if (selectedScheduledTaskList.Count > 0)
            {
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind.Stop, selectedScheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Stop, selectedScheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Stop, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 导出计划任务
        /// </summary>
        private async void OnExportScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [];
            foreach (object item in ScheduledTaskManagerListView.SelectedItems)
            {
                if (item is ScheduledTaskModel scheduledTaskItem)
                {
                    selectedScheduledTaskList.Add(scheduledTaskItem);
                }
            }

            if (selectedScheduledTaskList.Count > 0)
            {
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await ExportScheduledTaskListAsync(openFolderDialog.SelectedPath, selectedScheduledTaskList);
                    await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Export, selectedScheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                    await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Export, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
                }
                openFolderDialog.Dispose();
            }
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        private async void OnDeleteScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [];
            foreach (object item in ScheduledTaskManagerListView.SelectedItems)
            {
                if (item is ScheduledTaskModel scheduledTaskItem)
                {
                    selectedScheduledTaskList.Add(scheduledTaskItem);
                }
            }

            if (selectedScheduledTaskList.Count > 0)
            {
                (List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList) = await DeleteScheduledTaskListAsync(selectedScheduledTaskList);
                await UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind.Delete, selectedScheduledTaskList, successfullyScheduledTaskList, failedScheduledTaskList);
                await ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind.Delete, successfullyScheduledTaskList.Count, failedScheduledTaskList.Count);
            }
        }

        /// <summary>
        /// 查看错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new ScheduledTaskFailedDialog(new(ScheduledTaskFailedCollection)));
        }

        /// <summary>
        /// 使用说明
        /// </summary>

        private async void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            await Task.Delay(300);
            if (!ScheduledTaskSplitView.IsPaneOpen)
            {
                ScheduledTaskSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 打开计划任务程序
        /// </summary>
        private void OnOpenScheduledTaskProgramClicked(object sender, RoutedEventArgs args)
        {
            OpenScheduledTaskProgram();
        }

        /// <summary>
        /// 计划任务管理结果列表控件选中项发生变化时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading)
            {
                SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 获取是否正在加载中或操作中
        /// </summary>
        private bool GetIsLoadingOrOperating(ScheduledTaskResultKind scheduledTaskResultKind)
        {
            return !(scheduledTaskResultKind is ScheduledTaskResultKind.Loading || scheduledTaskResultKind is ScheduledTaskResultKind.Operating);
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化任务服务
        /// </summary>
        private void InitializeSchedulerTaskService()
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    taskService = (ITaskService)Activator.CreateInstance(Type.GetTypeFromCLSID(new("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(InitializeSchedulerTaskService), 1, e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 获取计划任务
        /// </summary>
        private async Task GetScheduledTaskAsync()
        {
            if (ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
                ScheduledTaskCollection.Clear();
                List<ScheduledTaskModel> scheduledTaskList = await GetScheduledTaskInfoListAsync();

                if (scheduledTaskList.Count is 0)
                {
                    ScheduledTaskResultKind = ScheduledTaskResultKind.Failed;
                    ScheduledTaskFailedContent = ScheduledTaskEmptyDescriptionString;
                    SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
                }
                else
                {
                    foreach (ScheduledTaskModel scheduledTaskItem in scheduledTaskList)
                    {
                        scheduledTaskItem.TaskIcon = await GetScheduledTaskProgramThumbnailImageAsync(scheduledTaskItem.ProcessPath);
                        ScheduledTaskCollection.Add(scheduledTaskItem);
                    }

                    ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                    ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyDescriptionString : string.Empty;
                    SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
                }
            }
        }

        /// <summary>
        /// 获取计划任务信息
        /// </summary>
        private async Task<List<ScheduledTaskModel>> GetScheduledTaskInfoListAsync()
        {
            return await Task.Factory.StartNew((param) =>
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
                            if (GetScheduledTask(registeredTask, taskFolder) is ScheduledTaskModel scheduledTask)
                            {
                                scheduledTaskList.Add(scheduledTask);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskInfoListAsync), 1, e);
                }

                scheduledTaskList.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));
                return scheduledTaskList;
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 获取计划任务信息
        /// </summary>
        private ScheduledTaskModel GetScheduledTask(IRegisteredTask registeredTask, ITaskFolder taskFolder)
        {
            try
            {
                ITaskDefinition taskDefinition = registeredTask.Definition;
                string version = string.Empty;
                try
                {
                    XDocument doc = XDocument.Parse(registeredTask.Xml);
                    XNamespace ns = "http://schemas.microsoft.com/windows/2004/02/mit/task";
                    XElement taskElement = doc.Root;
                    version = taskElement.Attribute("version")?.Value;
                }
                catch (Exception sube)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTask), 1, sube);
                }

                ScheduledTaskModel scheduledTask = new()
                {
                    Name = string.IsNullOrEmpty(registeredTask.Name) ? NotAvailableString : registeredTask.Name,
                    Path = string.IsNullOrEmpty(registeredTask.Path) ? NotAvailableString : registeredTask.Path,
                    LastRunTime = new DateTimeOffset(registeredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    LastTaskResult = string.Format("0x{0:X8}({1})", registeredTask.LastTaskResult, new Win32Exception(registeredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                    NextRunTime = new DateTimeOffset(registeredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    State = GetTaskState(registeredTask.State),
                    IsEnabled = registeredTask.Enabled,
                    Author = string.IsNullOrEmpty(registeredTask.Definition.RegistrationInfo.Author) ? NotAvailableString : registeredTask.Definition.RegistrationInfo.Author,
                    Description = string.IsNullOrEmpty(registeredTask.Definition.RegistrationInfo.Description) ? NotAvailableString : registeredTask.Definition.RegistrationInfo.Description,
                    Version = string.IsNullOrEmpty(version) ? NotAvailableString : version,
                    RegisteredTask = registeredTask,
                    TaskFolder = taskFolder
                };

                IActionCollection actionCollection = registeredTask.Definition.Actions;
                for (int index = 1; index <= actionCollection.Count; index++)
                {
                    if (actionCollection[index] is IExecAction2 execAction)
                    {
                        scheduledTask.ProcessPath = string.IsNullOrEmpty(execAction.Path) ? NotAvailableString : Environment.ExpandEnvironmentVariables(execAction.Path.Trim('"'));
                        scheduledTask.ProcessArguments = string.IsNullOrEmpty(execAction.Arguments) ? NotAvailableString : execAction.Arguments;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(scheduledTask.ProcessPath))
                {
                    scheduledTask.ProcessPath = string.Empty;
                }

                if (string.IsNullOrEmpty(scheduledTask.ProcessArguments))
                {
                    scheduledTask.ProcessArguments = string.Empty;
                }

                return scheduledTask;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTask), 2, e);
                return null;
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetSubTaskFolder), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetRegisteredTask), 1, e);
            }

            return registeredTaskList;
        }

        /// <summary>
        /// 获取计划任务的状态
        /// </summary>
        private async Task<(bool result, ScheduledTaskModel newScheduledTask)> GetTaskSchedulerStateAsync(IRegisteredTask registeredTask)
        {
            if (registeredTask is null)
            {
                return default;
            }

            return await Task.Factory.StartNew((param) =>
            {
                try
                {
                    return ValueTuple.Create<bool, ScheduledTaskModel>(true, new()
                    {
                        LastRunTime = new DateTimeOffset(registeredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                        LastTaskResult = string.Format("0x{0:X8}({1})", registeredTask.LastTaskResult, new Win32Exception(registeredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                        NextRunTime = new DateTimeOffset(registeredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                        State = GetTaskState(registeredTask.State),
                        IsEnabled = registeredTask.Enabled
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetTaskSchedulerStateAsync), 1, e);
                    return ValueTuple.Create<bool, ScheduledTaskModel>(false, null);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 获取计划任务程序缩略图
        /// </summary>
        private async Task<BitmapImage> GetScheduledTaskProgramThumbnailImageAsync(string programPath)
        {
            if (string.IsNullOrEmpty(programPath))
            {
                return default;
            }

            MemoryStream memoryStream = await Task.Run(() =>
            {
                MemoryStream memoryStream = null;

                try
                {
                    if (File.Exists(programPath) && ThumbnailHelper.GetThumbnailBitmap(programPath, 256) is Bitmap thumbnailBitmap)
                    {
                        memoryStream = new();
                        thumbnailBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        thumbnailBitmap.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskProgramThumbnailImageAsync), 1, e);
                }

                return memoryStream;
            });

            if (memoryStream is not null)
            {
                try
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    return bitmapImage;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskProgramThumbnailImageAsync), 2, e);
                    return emptyImage;
                }
                finally
                {
                    memoryStream?.Dispose();
                }
            }
            else
            {
                return emptyImage;
            }
        }

        /// <summary>
        /// 更新计划任务的状态
        /// </summary>
        private async Task<(List<ScheduledTaskModel>, List<(ScheduledTaskModel, Exception)>)> SetTaskSchedulerListStateAsync(ScheduledTaskOperationKind schedulerTaskOperationKind, List<ScheduledTaskModel> scheduledTaskList)
        {
            if (scheduledTaskList is null || scheduledTaskList.Count is 0)
            {
                return ValueTuple.Create<List<ScheduledTaskModel>, List<(ScheduledTaskModel, Exception)>>([], []);
            }

            foreach (ScheduledTaskModel scheduledTaskItem in scheduledTaskList)
            {
                scheduledTaskItem.IsProcessing = true;
            }

            List<ScheduledTaskModel> successfullyScheduledTaskList = [];
            List<(ScheduledTaskModel, Exception)> failedScheduledTaskList = [];
            foreach (ScheduledTaskModel scheduledTaskItem in scheduledTaskList)
            {
                (bool result, Exception exception) = await SetScheduledTaskStateAsync(schedulerTaskOperationKind, scheduledTaskItem.RegisteredTask);
                scheduledTaskItem.IsProcessing = false;
                if (result)
                {
                    successfullyScheduledTaskList.Add(scheduledTaskItem);
                }
                else
                {
                    failedScheduledTaskList.Add(ValueTuple.Create(scheduledTaskItem, exception is null ? new Exception() : exception));
                }
            }
            return ValueTuple.Create(successfullyScheduledTaskList, failedScheduledTaskList);
        }

        /// <summary>
        /// 设置计划任务的状态
        /// </summary>
        private async Task<(bool, Exception)> SetScheduledTaskStateAsync(ScheduledTaskOperationKind schedulerTaskOperationKind, IRegisteredTask registeredTask)
        {
            if (registeredTask is null)
            {
                return default;
            }

            return await Task.Factory.StartNew((param) =>
            {
                try
                {
                    if (schedulerTaskOperationKind is ScheduledTaskOperationKind.Disable)
                    {
                        registeredTask.Enabled = false;
                    }
                    else if (schedulerTaskOperationKind is ScheduledTaskOperationKind.Enable)
                    {
                        registeredTask.Enabled = true;
                    }
                    else if (schedulerTaskOperationKind is ScheduledTaskOperationKind.Run)
                    {
                        if (registeredTask.Enabled)
                        {
                            registeredTask.Run(null);
                        }
                    }
                    else if (schedulerTaskOperationKind is ScheduledTaskOperationKind.Stop)
                    {
                        if (registeredTask.Enabled)
                        {
                            registeredTask.Stop(0);
                        }
                    }
                    return ValueTuple.Create<bool, Exception>(true, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(SetScheduledTaskStateAsync), 1, e);
                    return ValueTuple.Create(false, e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 导出计划任务
        /// </summary>
        private async Task<(List<ScheduledTaskModel>, List<(ScheduledTaskModel, Exception)>)> ExportScheduledTaskListAsync(string folderPath, List<ScheduledTaskModel> scheduledTaskList)
        {
            if (string.IsNullOrEmpty(folderPath) || scheduledTaskList is null || scheduledTaskList.Count is 0)
            {
                return ValueTuple.Create<List<ScheduledTaskModel>, List<(ScheduledTaskModel, Exception)>>([], []);
            }

            List<ScheduledTaskModel> successfullyScheduledTaskList = [];
            List<(ScheduledTaskModel, Exception)> failedScheduledTaskList = [];
            foreach (ScheduledTaskModel scheduledTaskItem in scheduledTaskList)
            {
                (bool result, Exception exception) = await ExportScheduledTaskAsync(folderPath, scheduledTaskItem.RegisteredTask);
                if (result)
                {
                    successfullyScheduledTaskList.Add(scheduledTaskItem);
                }
                else
                {
                    failedScheduledTaskList.Add(ValueTuple.Create(scheduledTaskItem, exception is null ? new Exception() : exception));
                }
            }
            return ValueTuple.Create(successfullyScheduledTaskList, failedScheduledTaskList);
        }

        /// <summary>
        /// 导出计划任务
        /// </summary>
        private async Task<(bool, Exception)> ExportScheduledTaskAsync(string folderPath, IRegisteredTask registeredTask)
        {
            if (string.IsNullOrEmpty(folderPath) || registeredTask is null)
            {
                return default;
            }

            return await Task.Factory.StartNew((param) =>
            {
                try
                {
                    File.WriteAllText(Path.Combine(folderPath, registeredTask.Name + ".xml"), registeredTask.Xml);
                    return ValueTuple.Create<bool, Exception>(true, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(ExportScheduledTaskAsync), 1, e);
                    return ValueTuple.Create(false, e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        private async Task<(List<ScheduledTaskModel>, List<(ScheduledTaskModel, Exception)>)> DeleteScheduledTaskListAsync(List<ScheduledTaskModel> scheduledTaskList)
        {
            if (scheduledTaskList is null || scheduledTaskList.Count is 0)
            {
                return ValueTuple.Create<List<ScheduledTaskModel>, List<(ScheduledTaskModel, Exception)>>([], []);
            }

            List<ScheduledTaskModel> successfullyScheduledTaskList = [];
            List<(ScheduledTaskModel, Exception)> failedScheduledTaskList = [];
            foreach (ScheduledTaskModel scheduledTaskItem in scheduledTaskList)
            {
                (bool result, Exception exception) = await DeleteScheduledTaskAsync(scheduledTaskItem.TaskFolder, scheduledTaskItem.RegisteredTask.Name);
                if (result)
                {
                    successfullyScheduledTaskList.Add(scheduledTaskItem);
                }
                else
                {
                    failedScheduledTaskList.Add(ValueTuple.Create(scheduledTaskItem, exception is null ? new Exception() : exception));
                }
            }
            return ValueTuple.Create(successfullyScheduledTaskList, failedScheduledTaskList);
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        private async Task<(bool, Exception)> DeleteScheduledTaskAsync(ITaskFolder taskFolder, string taskName)
        {
            if (taskFolder is null || string.IsNullOrEmpty(taskName))
            {
                return default;
            }

            return await Task.Factory.StartNew((param) =>
            {
                try
                {
                    taskFolder.DeleteTask(taskName, 0);
                    return ValueTuple.Create<bool, Exception>(true, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(DeleteScheduledTaskAsync), 1, e);
                    return ValueTuple.Create(false, e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 更新计划任务状态
        /// </summary>
        private async Task UpdateTaskSchedulerStateAsync(ScheduledTaskOperationKind scheduledTaskOperationKind, List<ScheduledTaskModel> scheduledTaskList, List<ScheduledTaskModel> successfullyScheduledTaskList, List<(ScheduledTaskModel, Exception)> failedScheduledTaskList)
        {
            if (scheduledTaskList is null || scheduledTaskList.Count is 0 || successfullyScheduledTaskList is null || failedScheduledTaskList is null)
            {
                return;
            }

            if (successfullyScheduledTaskList.Count > 0)
            {
                if (scheduledTaskOperationKind is ScheduledTaskOperationKind.Disable || scheduledTaskOperationKind is ScheduledTaskOperationKind.Enable || scheduledTaskOperationKind is ScheduledTaskOperationKind.Stop || scheduledTaskOperationKind is ScheduledTaskOperationKind.Run)
                {
                    foreach (ScheduledTaskModel scheduledTask in successfullyScheduledTaskList)
                    {
                        (bool result, ScheduledTaskModel updatedScheduledTask) = await GetTaskSchedulerStateAsync(scheduledTask.RegisteredTask);
                        if (result)
                        {
                            scheduledTask.LastRunTime = updatedScheduledTask.LastRunTime;
                            scheduledTask.LastTaskResult = updatedScheduledTask.LastTaskResult;
                            scheduledTask.NextRunTime = updatedScheduledTask.NextRunTime;
                            scheduledTask.State = updatedScheduledTask.State;
                            scheduledTask.IsEnabled = updatedScheduledTask.IsEnabled;
                        }
                    }
                }

                if (scheduledTaskOperationKind is ScheduledTaskOperationKind.Delete)
                {
                    foreach (ScheduledTaskModel scheduledTask in successfullyScheduledTaskList)
                    {
                        ScheduledTaskCollection.Remove(scheduledTask);
                    }
                    SelectedItemsCount = ScheduledTaskManagerListView.SelectedItems.Count;
                }
            }

            ScheduledTaskFailedCollection.Clear();
            if (failedScheduledTaskList.Count > 0)
            {
                foreach ((ScheduledTaskModel failedScheduledTask, Exception exception) in failedScheduledTaskList)
                {
                    ScheduledTaskFailedCollection.Add(new()
                    {
                        Name = failedScheduledTask.Name,
                        Path = failedScheduledTask.Path,
                        Exception = exception
                    });
                }
            }
        }

        /// <summary>
        /// 显示计划任务操作结果通知
        /// </summary>
        private async Task ShowTaskSchedulerOperationResultNotificationAsync(ScheduledTaskOperationKind scheduledTaskOperationKind, int successItems, int failedItems)
        {
            OperationKind operationKind = scheduledTaskOperationKind switch
            {
                ScheduledTaskOperationKind.Delete => OperationKind.ScheduledTaskDelete,
                ScheduledTaskOperationKind.Export => OperationKind.ScheduledTaskExport,
                ScheduledTaskOperationKind.Disable => OperationKind.ScheduledTaskDisable,
                ScheduledTaskOperationKind.Enable => OperationKind.ScheduledTaskEnable,
                ScheduledTaskOperationKind.Stop => OperationKind.ScheduledTaskStop,
                ScheduledTaskOperationKind.Run => OperationKind.ScheduledTaskRun,
                _ => OperationKind.NoOperation
            };
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(operationKind, successItems, failedItems));
        }

        /// <summary>
        /// 打开文件路径
        /// </summary>
        private void OpenProcessPath(string processPath)
        {
            if (string.IsNullOrEmpty(processPath))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(processPath))
                    {
                        if (File.Exists(processPath))
                        {
                            nint pidlList = Shell32Library.ILCreateFromPath(processPath);
                            if (pidlList is not 0)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OpenProcessPath), 1, e);
                }
            });
        }

        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        private void RunAsAdministrator()
        {
            Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        Arguments = "--elevated",
                        FileName = System.Windows.Forms.Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(RunAsAdministrator), 1, e);
                    return;
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OpenScheduledTaskProgram), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取加载计划任务是否成功
        /// </summary>
        private Visibility GetScheduledTaskSuccessfullyVisibility(ScheduledTaskResultKind scheduledTaskResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? scheduledTaskResultKind is ScheduledTaskResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : scheduledTaskResultKind is ScheduledTaskResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索计划任务是否成功
        /// </summary>
        private Visibility CheckScheduledTaskResultKindVisibility(ScheduledTaskResultKind scheduledTaskResultKind, ScheduledTaskResultKind comparedScheduledTaskResultKind)
        {
            return Equals(scheduledTaskResultKind, comparedScheduledTaskResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取任务的运行状态
        /// </summary>
        private string GetTaskState(_TASK_STATE taskState) => taskState switch
        {
            _TASK_STATE.TASK_STATE_UNKNOWN => NotAvailableString,
            _TASK_STATE.TASK_STATE_DISABLED => TaskStateDisabledString,
            _TASK_STATE.TASK_STATE_QUEUED => TaskStateQueuedString,
            _TASK_STATE.TASK_STATE_READY => TaskStateReadyString,
            _TASK_STATE.TASK_STATE_RUNNING => TaskStateRunningString,
            _ => NotAvailableString,
        };

        #endregion 第七部分：数据操作与业务逻辑
    }
}
