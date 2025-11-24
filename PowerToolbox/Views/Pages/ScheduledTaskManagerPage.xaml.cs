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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
    public sealed partial class ScheduledTaskManagerPage : Page, INotifyPropertyChanged
    {
        private readonly string NotAvailableString = ResourceService.ScheduledTaskManagerResource.GetString("NotAvailable");
        private readonly string ScheduledTaskInformationString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskInformation");
        private readonly string ScheduledTaskEmptyDescriptionString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskEmptyDescription");
        private readonly string ScheduledTaskEmptyWithConditionDescriptionString = ResourceService.ScheduledTaskManagerResource.GetString("ScheduledTaskEmptyWithConditionDescription");
        private readonly string SelectFolderString = ResourceService.ScheduledTaskManagerResource.GetString("SelectFolder");
        private readonly string TaskStateDisabledString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateDisabled");
        private readonly string TaskStateQueuedString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateQueued");
        private readonly string TaskStateReadyString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateReady");
        private readonly string TaskStateRunningString = ResourceService.ScheduledTaskManagerResource.GetString("TaskStateRunning");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
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

        private bool _isModifiedFailed;

        public bool IsModifiedFailed
        {
            get { return _isModifiedFailed; }

            set
            {
                if (!Equals(_isModifiedFailed, value))
                {
                    _isModifiedFailed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifiedFailed)));
                }
            }
        }

        private List<ScheduledTaskModel> ScheduledTaskList { get; } = [];

        private List<ScheduledTaskFailedModel> ScheduledTaskFailedList { get; } = [];

        public WinRTObservableCollection<ScheduledTaskModel> ScheduledTaskCollection { get; } = [];

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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnNavigatedTo), 1, e);
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
            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
        }

        /// <summary>
        /// 禁用计划任务
        /// </summary>
        private async void OnDisableScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask is not null)
            {
                scheduledTask.IsProcessing = true;
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();

                (bool result, Exception exception, ScheduledTaskModel newScheduledTask) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask.RegisteredTask.Enabled = false;
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(true, null, new ScheduledTaskModel()
                        {
                            LastRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            LastTaskResult = string.Format("0x{0:X8}({1})", scheduledTask.RegisteredTask.LastTaskResult, new Win32Exception(scheduledTask.RegisteredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                            NextRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            State = GetTaskState(scheduledTask.RegisteredTask.State),
                            IsEnabled = scheduledTask.RegisteredTask.Enabled
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnDisableScheduledTaskExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(false, e, null);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                scheduledTask.IsProcessing = false;

                if (result)
                {
                    scheduledTask.LastRunTime = newScheduledTask.LastRunTime;
                    scheduledTask.LastTaskResult = newScheduledTask.LastTaskResult;
                    scheduledTask.NextRunTime = newScheduledTask.NextRunTime;
                    scheduledTask.State = newScheduledTask.State;
                    scheduledTask.IsEnabled = newScheduledTask.IsEnabled;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskDisable, 1, 0));
                }
                else
                {
                    IsModifiedFailed = true;
                    ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                    {
                        Name = scheduledTask.Name,
                        Path = scheduledTask.Path,
                        Exception = exception
                    });
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskDisable, 0, 1));
                }
            }
        }

        /// <summary>
        /// 启用计划任务
        /// </summary>
        private async void OnEnableScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask is not null)
            {
                scheduledTask.IsProcessing = true;
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();

                (bool result, Exception exception, ScheduledTaskModel newScheduledTask) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask.RegisteredTask.Enabled = true;
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(true, null, new ScheduledTaskModel()
                        {
                            LastRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            LastTaskResult = string.Format("0x{0:X8}({1})", scheduledTask.RegisteredTask.LastTaskResult, new Win32Exception(scheduledTask.RegisteredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                            NextRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            State = GetTaskState(scheduledTask.RegisteredTask.State),
                            IsEnabled = scheduledTask.RegisteredTask.Enabled
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnEnableScheduledTaskExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(false, e, null);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                scheduledTask.IsProcessing = false;

                if (result)
                {
                    scheduledTask.LastRunTime = newScheduledTask.LastRunTime;
                    scheduledTask.LastTaskResult = newScheduledTask.LastTaskResult;
                    scheduledTask.NextRunTime = newScheduledTask.NextRunTime;
                    scheduledTask.State = newScheduledTask.State;
                    scheduledTask.IsEnabled = newScheduledTask.IsEnabled;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskEnable, 1, 0));
                }
                else
                {
                    IsModifiedFailed = true;
                    ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                    {
                        Name = scheduledTask.Name,
                        Path = scheduledTask.Path,
                        Exception = exception
                    });
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskEnable, 0, 1));
                }
            }
        }

        /// <summary>
        /// 运行计划任务
        /// </summary>
        private async void OnRunScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask is not null)
            {
                scheduledTask.IsProcessing = true;
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();

                (bool result, Exception exception, ScheduledTaskModel newScheduledTask) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask.RegisteredTask.Run(null);
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(true, null, new ScheduledTaskModel()
                        {
                            LastRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            LastTaskResult = string.Format("0x{0:X8}({1})", scheduledTask.RegisteredTask.LastTaskResult, new Win32Exception(scheduledTask.RegisteredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                            NextRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            State = GetTaskState(scheduledTask.RegisteredTask.State),
                            IsEnabled = scheduledTask.RegisteredTask.Enabled
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnRunScheduledTaskExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(false, e, null);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                scheduledTask.IsProcessing = false;

                if (result)
                {
                    scheduledTask.LastRunTime = newScheduledTask.LastRunTime;
                    scheduledTask.LastTaskResult = newScheduledTask.LastTaskResult;
                    scheduledTask.NextRunTime = newScheduledTask.NextRunTime;
                    scheduledTask.State = newScheduledTask.State;
                    scheduledTask.IsEnabled = newScheduledTask.IsEnabled;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskRun, 1, 0));
                }
                else
                {
                    IsModifiedFailed = true;
                    ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                    {
                        Name = scheduledTask.Name,
                        Path = scheduledTask.Path,
                        Exception = exception
                    });
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskRun, 0, 1));
                }
            }
        }

        /// <summary>
        /// 结束计划任务
        /// </summary>
        private async void OnStopScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask is not null)
            {
                scheduledTask.IsProcessing = true;
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();

                (bool result, Exception exception, ScheduledTaskModel newScheduledTask) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask.RegisteredTask.Stop(0);
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(true, null, new ScheduledTaskModel()
                        {
                            LastRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            LastTaskResult = string.Format("0x{0:X8}({1})", scheduledTask.RegisteredTask.LastTaskResult, new Win32Exception(scheduledTask.RegisteredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                            NextRunTime = new DateTimeOffset(scheduledTask.RegisteredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            State = GetTaskState(scheduledTask.RegisteredTask.State),
                            IsEnabled = scheduledTask.RegisteredTask.Enabled
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnStopScheduledTaskExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, Exception, ScheduledTaskModel>(false, e, null);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                scheduledTask.IsProcessing = false;

                if (result)
                {
                    scheduledTask.LastRunTime = newScheduledTask.LastRunTime;
                    scheduledTask.LastTaskResult = newScheduledTask.LastTaskResult;
                    scheduledTask.NextRunTime = newScheduledTask.NextRunTime;
                    scheduledTask.State = newScheduledTask.State;
                    scheduledTask.IsEnabled = newScheduledTask.IsEnabled;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskStop, 1, 0));
                }
                else
                {
                    IsModifiedFailed = true;
                    ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                    {
                        Name = scheduledTask.Name,
                        Path = scheduledTask.Path,
                        Exception = exception
                    });
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskStop, 0, 1));
                }
            }
        }

        /// <summary>
        /// 导出计划任务
        /// </summary>
        private async void OnExportScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask is not null)
            {
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    bool result = await Task.Factory.StartNew((param) =>
                    {
                        try
                        {
                            File.WriteAllText(Path.Combine(openFolderDialog.SelectedPath, scheduledTask.Name + ".xml"), scheduledTask.RegisteredTask.Xml);
                            return true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnExportScheduledTaskExecuteRequested), 1, e);
                            return false;
                        }
                    }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                    if (result)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskExport, 1, 0));
                    }
                    else
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskExport, 0, 1));
                    }
                }
                openFolderDialog.Dispose();
            }
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        private async void OnDeleteScheduledTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ScheduledTaskModel scheduledTask && scheduledTask is not null)
            {
                scheduledTask.IsProcessing = true;
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();

                (bool result, Exception exception) = await Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        scheduledTask.TaskFolder.DeleteTask(scheduledTask.RegisteredTask.Name, 0);
                        return ValueTuple.Create<bool, Exception>(true, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnStopScheduledTaskExecuteRequested), 1, e);
                        return ValueTuple.Create(false, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);

                scheduledTask.IsProcessing = false;

                if (result)
                {
                    ScheduledTaskCollection.Remove(scheduledTask);
                    ScheduledTaskList.Remove(scheduledTask);
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskDelete, 1, 0));

                    if (ScheduledTaskList.Count is 0)
                    {
                        ScheduledTaskResultKind = ScheduledTaskResultKind.Failed;
                        ScheduledTaskFailedContent = ScheduledTaskEmptyDescriptionString;
                    }
                    else
                    {
                        ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                        ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                        ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
                    }
                }
                else
                {
                    IsModifiedFailed = true;
                    ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                    {
                        Name = scheduledTask.Name,
                        Path = scheduledTask.Path,
                        Exception = exception
                    });
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskDelete, 0, 1));
                }
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnOpenProcessPathExecuteRequested), 1, e);
                    }
                });
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：计划任务管理页面——挂载的事件

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
                catch
                {
                    return;
                }
            });
        }

        /// <summary>
        /// 搜索计划任务名称
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
                        ScheduledTaskCollection.Add(scheduledTaskItem);
                    }
                }

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 计划任务名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading && ScheduledTaskList.Count > 0)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
                ScheduledTaskCollection.Clear();
                foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskList)
                {
                    if (scheduledTaskItem.Name.Contains(SearchText) || scheduledTaskItem.Author.Contains(SearchText) || scheduledTaskItem.Description.Contains(SearchText) || scheduledTaskItem.Path.Contains(SearchText) || scheduledTaskItem.ProcessPath.Contains(SearchText))
                    {
                        scheduledTaskItem.IsSelected = false;
                        ScheduledTaskCollection.Add(scheduledTaskItem);
                    }
                }

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
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

            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
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

            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
        }

        /// <summary>
        /// 运行计划任务
        /// </summary>
        private async void OnRunScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [.. (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Where(item => item.IsSelected)];
            if (selectedScheduledTaskList.Count > 0)
            {
                List<ScheduledTaskModel> newScheduledTaskList = [];
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();
                foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
                {
                    scheduledTaskItem.IsSelected = false;
                    scheduledTaskItem.IsProcessing = true;
                }
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));

                await Task.Factory.StartNew((param) =>
                {
                    foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
                    {
                        bool result = false;

                        if (scheduledTaskItem.IsEnabled)
                        {
                            try
                            {
                                scheduledTaskItem.RegisteredTask.Run(null);
                                result = true;
                            }
                            catch (Exception e)
                            {
                                ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                                {
                                    Name = scheduledTaskItem.Name,
                                    Path = scheduledTaskItem.Path,
                                    Exception = e
                                });
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnRunScheduledTaskClicked), 1, e);
                            }
                        }

                        if (result)
                        {
                            newScheduledTaskList.Add(new()
                            {
                                Name = scheduledTaskItem.Name,
                                Path = scheduledTaskItem.Path,
                                LastRunTime = new DateTimeOffset(scheduledTaskItem.RegisteredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                                LastTaskResult = string.Format("0x{0:X8}({1})", scheduledTaskItem.RegisteredTask.LastTaskResult, new Win32Exception(scheduledTaskItem.RegisteredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                                NextRunTime = new DateTimeOffset(scheduledTaskItem.RegisteredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                                State = GetTaskState(scheduledTaskItem.RegisteredTask.State),
                                IsEnabled = scheduledTaskItem.RegisteredTask.Enabled,
                            });
                        }
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
                IsModifiedFailed = ScheduledTaskFailedList.Count > 0;
                synchronizationContext.Post(async (_) =>
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskRun, selectedScheduledTaskList.Count - ScheduledTaskFailedList.Count, ScheduledTaskFailedList.Count));
                }, null);

                foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskList)
                {
                    scheduledTaskItem.IsProcessing = false;
                    foreach (ScheduledTaskModel newScheduledTaskItem in newScheduledTaskList)
                    {
                        if (string.Equals(scheduledTaskItem.Name, newScheduledTaskItem.Name) && string.Equals(scheduledTaskItem.Path, newScheduledTaskItem.Path))
                        {
                            scheduledTaskItem.LastRunTime = newScheduledTaskItem.LastRunTime;
                            scheduledTaskItem.LastTaskResult = newScheduledTaskItem.LastTaskResult;
                            scheduledTaskItem.NextRunTime = newScheduledTaskItem.NextRunTime;
                            scheduledTaskItem.State = newScheduledTaskItem.State;
                            scheduledTaskItem.IsEnabled = newScheduledTaskItem.IsEnabled;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 结束计划任务
        /// </summary>
        private async void OnStopScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [.. (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Where(item => item.IsSelected)];
            if (selectedScheduledTaskList.Count > 0)
            {
                List<ScheduledTaskModel> newScheduledTaskList = [];
                IsModifiedFailed = false;
                ScheduledTaskFailedList.Clear();
                foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
                {
                    scheduledTaskItem.IsSelected = false;
                    scheduledTaskItem.IsProcessing = true;
                }
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));

                await Task.Factory.StartNew((param) =>
                {
                    foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
                    {
                        bool result = false;

                        if (scheduledTaskItem.IsEnabled)
                        {
                            try
                            {
                                scheduledTaskItem.RegisteredTask.Stop(0);
                                result = true;
                            }
                            catch (Exception e)
                            {
                                ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                                {
                                    Name = scheduledTaskItem.Name,
                                    Path = scheduledTaskItem.Path,
                                    Exception = e
                                });
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnStopScheduledTaskClicked), 1, e);
                            }
                        }

                        if (result)
                        {
                            newScheduledTaskList.Add(new()
                            {
                                Name = scheduledTaskItem.Name,
                                Path = scheduledTaskItem.Path,
                                LastRunTime = new DateTimeOffset(scheduledTaskItem.RegisteredTask.LastRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                                LastTaskResult = string.Format("0x{0:X8}({1})", scheduledTaskItem.RegisteredTask.LastTaskResult, new Win32Exception(scheduledTaskItem.RegisteredTask.LastTaskResult) is Exception exception ? exception.Message : NotAvailableString),
                                NextRunTime = new DateTimeOffset(scheduledTaskItem.RegisteredTask.NextRunTime).ToString("yyyy-MM-dd HH:mm:ss"),
                                State = GetTaskState(scheduledTaskItem.RegisteredTask.State),
                                IsEnabled = scheduledTaskItem.RegisteredTask.Enabled,
                            });
                        }
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
                IsModifiedFailed = ScheduledTaskFailedList.Count > 0;
                synchronizationContext.Post(async (_) =>
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskStop, selectedScheduledTaskList.Count - ScheduledTaskFailedList.Count, ScheduledTaskFailedList.Count));
                }, null);

                foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskList)
                {
                    scheduledTaskItem.IsProcessing = false;
                    foreach (ScheduledTaskModel newScheduledTaskItem in newScheduledTaskList)
                    {
                        if (string.Equals(scheduledTaskItem.Name, newScheduledTaskItem.Name) && string.Equals(scheduledTaskItem.Path, newScheduledTaskItem.Path))
                        {
                            scheduledTaskItem.LastRunTime = newScheduledTaskItem.LastRunTime;
                            scheduledTaskItem.LastTaskResult = newScheduledTaskItem.LastTaskResult;
                            scheduledTaskItem.NextRunTime = newScheduledTaskItem.NextRunTime;
                            scheduledTaskItem.State = newScheduledTaskItem.State;
                            scheduledTaskItem.IsEnabled = newScheduledTaskItem.IsEnabled;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出计划任务
        /// </summary>
        private async void OnExportScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
            List<ScheduledTaskModel> selectedScheduledTaskList = [.. (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Where(item => item.IsSelected)];
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
                    await Task.Factory.StartNew((param) =>
                    {
                        foreach (ScheduledTaskModel scheduledTaskItem in selectedScheduledTaskList)
                        {
                            try
                            {
                                File.WriteAllText(Path.Combine(openFolderDialog.SelectedPath, scheduledTaskItem.Name + ".xml"), scheduledTaskItem.RegisteredTask.Xml);
                            }
                            catch (Exception e)
                            {
                                ScheduledTaskFailedList.Add(new ScheduledTaskFailedModel()
                                {
                                    Name = scheduledTaskItem.Name,
                                    Path = scheduledTaskItem.Path,
                                    Exception = e
                                });
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnExportScheduledTaskClicked), 1, e);
                            }
                        }
                    }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
                }
                openFolderDialog.Dispose();
                IsModifiedFailed = ScheduledTaskFailedList.Count > 0;
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ScheduledTaskExport, selectedScheduledTaskList.Count - ScheduledTaskFailedList.Count, ScheduledTaskFailedList.Count));
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
        /// 查看错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new ScheduledTaskFailedDialog(ScheduledTaskFailedList));
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
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskschd.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnOpenScheduledTaskProgramClicked), 1, e);
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

        #endregion 第三部分：计划任务管理页面——挂载的事件

        /// <summary>
        /// 获取计划任务
        /// </summary>
        private async Task GetScheduledTaskAsync()
        {
            ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
            ScheduledTaskList.Clear();
            ScheduledTaskCollection.Clear();
            ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskAsync), 1, e);
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
                            if (!string.IsNullOrEmpty(scheduledTaskItem.ProcessPath) && !string.Equals(scheduledTaskItem.ProcessPath, NotAvailableString, StringComparison.OrdinalIgnoreCase))
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskAsync), 2, e);
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTaskAsync), 3, e);
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
                ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, (ScheduledTaskCollection as ObservableCollection<ScheduledTaskModel>).Count(item => item.IsSelected));
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
        /// 获取计划任务信息
        /// </summary>
        private ScheduledTaskModel GetScheduledTasks(IRegisteredTask registeredTask, ITaskFolder taskFolder)
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTasks), 1, sube);
                }

                ScheduledTaskModel scheduledTask = new()
                {
                    IsSelected = false,
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

                return scheduledTask;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(GetScheduledTasks), 2, e);
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
    }
}
