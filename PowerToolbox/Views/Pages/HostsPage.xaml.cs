using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.Collections;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Dnsapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// Hosts 文件编辑器页面
    /// </summary>
    public sealed partial class HostsPage : Page, INotifyPropertyChanged
    {
        private readonly string hostsPath = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
        private readonly string AvailableString = ResourceService.HostsResource.GetString("Available");
        private readonly string HostsEmptyDescriptionString = ResourceService.HostsResource.GetString("HostsEmptyDescription");
        private readonly string HostsEmptyWithConditionDescriptionString = ResourceService.HostsResource.GetString("HostsEmptyWithConditionDescription");
        private readonly string NotAvailableString = ResourceService.HostsResource.GetString("NotAvailable");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private bool isAllowClosed = false;
        private FileSystemWatcher fileSystemWatcher;
        private HostsModel oldHostsItem;

        private HostsResultKind _hostsResultKind;

        public HostsResultKind HostsResultKind
        {
            get { return _hostsResultKind; }

            set
            {
                if (!Equals(_hostsResultKind, value))
                {
                    _hostsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HostsResultKind)));
                }
            }
        }

        private string _hostsFailedContent;

        public string HostsFailedContent
        {
            get { return _hostsFailedContent; }

            set
            {
                if (!Equals(_hostsFailedContent, value))
                {
                    _hostsFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HostsFailedContent)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string _addressText;

        public string AddressText
        {
            get { return _addressText; }

            set
            {
                if (!string.Equals(_addressText, value))
                {
                    _addressText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddressText)));
                }
            }
        }

        private string _hostText;

        public string HostText
        {
            get { return _hostText; }

            set
            {
                if (!string.Equals(_hostText, value))
                {
                    _hostText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HostText)));
                }
            }
        }

        /// <summary>
        /// 注释
        /// </summary>
        private string _annotationText;

        public string AnnotationText
        {
            get { return _annotationText; }

            set
            {
                if (!string.Equals(_annotationText, value))
                {
                    _annotationText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnnotationText)));
                }
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        private bool _isAvailable;

        public bool IsAvailable
        {
            get { return _isAvailable; }

            set
            {
                if (!Equals(_isAvailable, value))
                {
                    _isAvailable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
                }
            }
        }

        private bool _isPrimaryButtonEnabled;

        public bool IsPrimaryButtonEnabled
        {
            get { return _isPrimaryButtonEnabled; }

            set
            {
                if (!Equals(_isPrimaryButtonEnabled, value))
                {
                    _isPrimaryButtonEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPrimaryButtonEnabled)));
                }
            }
        }

        private List<HostsModel> HostsList { get; } = [];

        private WinRTObservableCollection<HostsModel> HostsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public HostsPage()
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

            await Task.Run(() =>
            {
                if (fileSystemWatcher is null)
                {
                    fileSystemWatcher = new()
                    {
                        Path = Path.GetDirectoryName(hostsPath),
                        Filter = Path.GetFileName(hostsPath),
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
                    };

                    if (fileSystemWatcher is not null)
                    {
                        fileSystemWatcher.Changed += OnWatcherChanged;
                        fileSystemWatcher.Created += OnWatcherCreated;
                        fileSystemWatcher.Deleted += OnWatcherDeleted;
                        fileSystemWatcher.Error += OnWatcherError;
                        fileSystemWatcher.Renamed += OnWatcherRenamed;
                    }
                }
            });

            await GetHostsAsync();
        }

        /// <summary>
        /// 离开该页面触发的事件
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            Task.Run(() =>
            {
                if (fileSystemWatcher is not null)
                {
                    try
                    {
                        fileSystemWatcher.Changed -= OnWatcherChanged;
                        fileSystemWatcher.Created -= OnWatcherCreated;
                        fileSystemWatcher.Deleted -= OnWatcherDeleted;
                        fileSystemWatcher.Error -= OnWatcherError;
                        fileSystemWatcher.Renamed -= OnWatcherRenamed;

                        fileSystemWatcher.Dispose();
                        fileSystemWatcher = null;
                    }
                    catch (Exception e)
                    {
                        fileSystemWatcher = null;
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(HostsPage), nameof(OnNavigatedFrom), 1, e);
                    }
                }
            });
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 编辑 Hosts 文件条目内容
        /// </summary>
        private void OnEditExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HostsModel hosts)
            {
                AddressText = hosts.Address;
                HostText = hosts.Hosts;
                AnnotationText = hosts.Annotation;
                IsAvailable = hosts.IsAvailable;
                oldHostsItem = hosts;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(AddressText) && !string.IsNullOrEmpty(HostText);

                EditHostFlyout.ShowAt(MainWindow.Current.Content, new()
                {
                    Placement = FlyoutPlacementMode.Full,
                    ShowMode = FlyoutShowMode.Standard
                });
            }
        }

        /// <summary>
        /// 删除 Hosts 文件条目内容
        /// </summary>
        private void OnDeleteExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HostsModel hosts)
            {
                HostsList.Remove(hosts);
                HostsCollection.Remove(hosts);

                // TODO：未完成
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：Hosts 文件编辑器页面——挂载的事件

        /// <summary>
        /// 浮出控件关闭时触发的事件
        /// </summary>
        private void OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (isAllowClosed)
            {
                isAllowClosed = false;
            }
            else
            {
                args.Cancel = true;
            }
        }

        /// <summary>
        /// 浮出控件接受屏幕按键触发的事件
        /// </summary>
        private void OnFlyoutKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Escape && sender is Grid grid && grid.Tag is Flyout flyout)
            {
                isAllowClosed = true;
                flyout.Hide();
            }
        }

        /// <summary>
        /// 地址文本框内容发生变化触发的事件
        /// </summary>
        private void OnAddressTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                AddressText = textBox.Text;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(AddressText) && !string.IsNullOrEmpty(HostText);
            }
        }

        /// <summary>
        /// 主机文本框内容发生变化触发的事件
        /// </summary>
        private void OnHostTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                HostText = textBox.Text;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(AddressText) && !string.IsNullOrEmpty(HostText);
            }
        }

        /// <summary>
        /// 注释文本框内容发生变化触发的事件
        /// </summary>
        private void OnAnnotationTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                AnnotationText = textBox.Text;
            }
        }

        /// <summary>
        /// 添加 Hosts 文件条目内容
        /// </summary>
        private async void OnAddClicked(object sender, RoutedEventArgs args)
        {
            isAllowClosed = true;
            AddNewHostFlyout.Hide();

            bool isExisted = await Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    bool isExisted = false;

                    foreach (HostsModel hostsItem in HostsCollection)
                    {
                        if (string.Equals(AddressText, hostsItem.Address, StringComparison.OrdinalIgnoreCase) && string.Equals(HostText, hostsItem.Hosts, StringComparison.OrdinalIgnoreCase))
                        {
                            isExisted = true;
                            break;
                        }
                    }

                    return isExisted;
                }
                else
                {
                    return false;
                }
            });

            if (isExisted)
            {
                // TODO：显示通知
                return;
            }
            else
            {
                bool isValid = await Task.Run(() =>
                {
                    bool isValid = true;

                    // TODO：添加 IP 地址的格式检查
                    return isValid;
                });

                if (isValid)
                {
                    // 保存信息
                }
            }
        }

        /// <summary>
        /// 更新 Hosts 文件条目内容
        /// </summary>
        private void OnUpdateClicked(object sender, RoutedEventArgs args)
        {
            isAllowClosed = true;
            EditHostFlyout.Hide();

            Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    // TODO：未完成
                }
            });
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (sender is Button button && button.Tag is Flyout flyout)
            {
                isAllowClosed = true;
                flyout.Hide();
            }
        }

        /// <summary>
        /// 查询搜索内容
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && HostsList.Count > 0)
            {
                HostsResultKind = HostsResultKind.Loading;
                HostsCollection.Clear();
                foreach (HostsModel hostsItem in HostsList)
                {
                    if (!string.IsNullOrEmpty(hostsItem.Address) && hostsItem.Address.Contains(SearchText))
                    {
                        HostsList.Add(hostsItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(hostsItem.Hosts) && hostsItem.Hosts.Contains(SearchText))
                    {
                        HostsList.Add(hostsItem);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(hostsItem.Annotation) && hostsItem.Annotation.Contains(SearchText))
                    {
                        HostsList.Add(hostsItem);
                        continue;
                    }
                }

                HostsResultKind = HostsCollection.Count is 0 ? HostsResultKind.Failed : HostsResultKind.Successfully;
                HostsFailedContent = HostsCollection.Count is 0 ? HostsEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 搜索应用名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && HostsList.Count > 0)
            {
                HostsResultKind = HostsResultKind.Loading;
                HostsCollection.Clear();
                foreach (HostsModel hostsItem in HostsList)
                {
                    HostsCollection.Add(hostsItem);
                }

                HostsResultKind = HostsCollection.Count is 0 ? HostsResultKind.Failed : HostsResultKind.Successfully;
                HostsFailedContent = HostsCollection.Count is 0 ? HostsEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 添加条目
        /// </summary>
        private void OnAddNewHostClicked(object sender, RoutedEventArgs args)
        {
            AddressText = string.Empty;
            HostText = string.Empty;
            AnnotationText = string.Empty;
            IsAvailable = true;

            AddNewHostFlyout.ShowAt(MainWindow.Current.Content, new()
            {
                Placement = FlyoutPlacementMode.Full,
                ShowMode = FlyoutShowMode.Standard,
            });
        }

        /// <summary>
        /// 打开 Hosts 文件
        /// </summary>
        private void OnOpenFileClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    if (File.Exists(hostsPath))
                    {
                        ProcessStartInfo processStartInfo = new()
                        {
                            FileName = "notepad.exe",
                            Arguments = hostsPath,
                            UseShellExecute = true,
                            Verb = "runas"
                        };

                        Process.Start(processStartInfo);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(HostsPage), nameof(OnOpenFileClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetHostsAsync();
        }

        /// <summary>
        /// 更新 DNS 缓存
        /// </summary>
        private void OnUpdateDNSCacheClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                DnsapiLibrary.DnsFlushResolverCache();
            });
        }

        #endregion 第三部分：Hosts 文件编辑器页面——挂载的事件

        #region 第四部分：Hosts 文件编辑器页面——自定义事件

        /// <summary>
        /// 当更改指定 Path 中的文件和目录时发生的事件
        /// </summary>
        private void OnWatcherChanged(object sender, FileSystemEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 当在指定 Path 中创建文件和目录时发生的事件
        /// </summary>
        private void OnWatcherCreated(object sender, FileSystemEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 删除指定 Path 中的文件或目录时发生的事件
        /// </summary>
        private void OnWatcherDeleted(object sender, FileSystemEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 当 FileSystemWatcher 的实例无法继续监视更改或内部缓冲区溢出时发生的事件
        /// </summary>
        private void OnWatcherError(object sender, ErrorEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        /// <summary>
        /// 重命名指定 Path 中的文件或目录时发生的事件
        /// </summary>
        private void OnWatcherRenamed(object sender, RenamedEventArgs args)
        {
            synchronizationContext.Post(async (_) =>
            {
                await GetHostsAsync();
            }, null);
        }

        #endregion 第四部分：Hosts 文件编辑器页面——自定义事件

        /// <summary>
        /// 获取 Hosts 文件内容
        /// </summary>
        private async Task GetHostsAsync()
        {
            // TODO：未完成
            HostsResultKind = HostsResultKind.Loading;
            HostsList.Clear();
            HostsCollection.Clear();

            List<HostsModel> hostsList = await Task.Run(() =>
            {
                List<HostsModel> hostsList = [];
                string[] hostsLinesArray = File.ReadAllLines(hostsPath, Encoding.UTF8);

                foreach (string hostsLine in hostsLinesArray)
                {
                    if (string.IsNullOrWhiteSpace(hostsLine))
                    {
                        continue;
                    }

                    string trimmedHostLine = hostsLine.Trim();
                    ParseHostsLine(trimmedHostLine);
                }

                return hostsList;
            });

            HostsList.AddRange(hostsList);

            if (HostsList.Count is 0)
            {
                HostsResultKind = HostsResultKind.Failed;
                HostsFailedContent = HostsEmptyDescriptionString;
            }
            else
            {
                foreach (HostsModel hostsItem in HostsList)
                {
                }
            }

            HostsCollection.Add(new HostsModel()
            {
                Address = "TestAddress",
                Annotation = "TestAnnotation",
                Hosts = "TestHosts",
                IsAvailable = true,
                IsAvailableString = AvailableString
            });

            HostsResultKind = HostsResultKind.Successfully;
        }

        /// <summary>
        /// 获取加载网络回环是否成功
        /// </summary>
        private Visibility GetHostsSuccessfullyState(HostsResultKind hostsResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? hostsResultKind is HostsResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : hostsResultKind is HostsResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索网络回环是否成功
        /// </summary>
        private Visibility CheckHostsState(HostsResultKind hostsResultKind, HostsResultKind comparedHostsResultKind)
        {
            return Equals(hostsResultKind, comparedHostsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(HostsResultKind hostsResultKind)
        {
            return hostsResultKind is not HostsResultKind.Loading;
        }

        /// <summary>
        /// 解析每一行 Hosts 文件内容
        /// </summary>
        private HostsModel ParseHostsLine(string line)
        {
            // TODO：未完成
            string[] hostsLineArray = line.Split(' ');

            return new HostsModel();
        }
    }
}
