using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic.FileIO;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.System;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 添加下载任务对话框
    /// </summary>
    internal sealed partial class AddDownloadTaskDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string SelectFolderString = ResourceService.DialogResource.GetString("SelectFolder");
        private bool isAllowClosed = false;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isPrimaryEnabled;

        private bool IsPrimaryEnabled
        {
            get { return _isPrimaryEnabled; }

            set
            {
                if (!Equals(_isPrimaryEnabled, value))
                {
                    _isPrimaryEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPrimaryEnabled)));
                }
            }
        }

        private string _downloadLinkText;

        private string DownloadLinkText
        {
            get { return _downloadLinkText; }

            set
            {
                if (!string.Equals(_downloadLinkText, value))
                {
                    _downloadLinkText = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadLinkText)));
                }
            }
        }

        private string _downloadFileNameText;

        private string DownloadFileNameText
        {
            get { return _downloadFileNameText; }

            set
            {
                if (!string.Equals(_downloadFileNameText, value))
                {
                    _downloadFileNameText = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadFileNameText)));
                }
            }
        }

        private string _downloadFolderText = DownloadOptionsService.DownloadFolder;

        private string DownloadFolderText
        {
            get { return _downloadFolderText; }

            set
            {
                if (!string.Equals(_downloadFolderText, value))
                {
                    _downloadFolderText = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadFolderText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal AddDownloadTaskDialog()
        {
            InitializeComponent();
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 对话框接受屏幕按键触发的事件
        /// </summary>
        protected override void OnKeyDown(Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Escape)
            {
                isAllowClosed = true;
                Hide();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 对话框打开后触发的事件
        /// </summary>
        private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            DownloadLinkText = string.Empty;
            DownloadFileNameText = string.Empty;
            DownloadFolderText = DownloadOptionsService.DownloadFolder;
            IsPrimaryEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

        /// <summary>
        /// 对话框关闭时触发的事件
        /// </summary>
        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
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
        /// 获取输入的下载链接
        /// </summary>
        private async void OnDownloadLinkTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DownloadLinkText = textBox.Text;

                if (!string.IsNullOrEmpty(DownloadLinkText))
                {
                    string createFileName = await GetLinkFileNameAsync(DownloadLinkText);
                    DownloadFileNameText = !string.IsNullOrEmpty(createFileName) ? createFileName : string.Empty;
                }
                else
                {
                    DownloadFileNameText = string.Empty;
                }

                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
            }
        }

        /// <summary>
        /// 获取输入的下载链接
        /// </summary>
        private void OnDownloadFileNameTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DownloadFileNameText = textBox.Text;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFileNameText) && !string.IsNullOrEmpty(DownloadFolderText);
            }
        }

        /// <summary>
        /// 获取输入的下载目录
        /// </summary>
        private void OnDownloadFolderTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DownloadLinkText = textBox.Text;
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                DownloadFolderText = openFolderDialog.SelectedPath;
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private async void OnDownloadClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            isAllowClosed = true;
            Hide();

            // 检查文件路径
            if (DownloadFileNameText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || DownloadFolderText.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return;
            }

            string filePath = Path.Combine(DownloadFolderText, DownloadFileNameText);

            // 检查本地文件是否存在
            if (File.Exists(filePath))
            {
                ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new FileCheckDialog());

                // 删除本地文件并下载文件
                if (contentDialogResult is ContentDialogResult.Primary)
                {
                    bool result = await DownloadFileAsync(filePath, DownloadLinkText, true);

                    if (!result)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteFileFailed));
                    }
                }
                // 打开本地目录
                else if (contentDialogResult is ContentDialogResult.Secondary)
                {
                    await OpenLocalFolderAsync(filePath);
                }
            }
            else
            {
                await DownloadFileAsync(filePath, DownloadLinkText, false);
            }
        }

        /// <summary>
        /// 关闭对话框或使用说明
        /// </summary>
        private void OnCloseClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            isAllowClosed = true;
            Hide();
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 获取链接对应的文件名称
        /// </summary>
        private async Task<string> GetLinkFileNameAsync(string downloadLinkText)
        {
            if (string.IsNullOrEmpty(downloadLinkText))
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    bool createSucceeded = Uri.TryCreate(downloadLinkText, UriKind.Absolute, out Uri uri);
                    if (createSucceeded && uri.Segments.Length >= 1)
                    {
                        string fileName = uri.Segments[uri.Segments.Length - 1];
                        if (fileName is not "/")
                        {
                            return fileName;
                        }
                    }

                    return string.Empty;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AddDownloadTaskDialog), nameof(GetLinkFileNameAsync), 1, e);
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private async Task<bool> DownloadFileAsync(string filePath, string downloadLinkText, bool needToDeleteFile)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(downloadLinkText))
            {
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    if (needToDeleteFile)
                    {
                        FileSystem.DeleteFile(filePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }
                    DownloadSchedulerService.CreateDownload(downloadLinkText, filePath);
                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AddDownloadTaskDialog), nameof(DownloadFileAsync), 1, e);
                    return false;
                }
            });
        }

        /// <summary>
        /// 定位文件
        /// </summary>
        private async Task OpenLocalFolderAsync(string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        nint pidlList = Shell32Library.ILCreateFromPath(filePath);
                        if (pidlList is not 0)
                        {
                            Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                            Shell32Library.ILFree(pidlList);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AddDownloadTaskDialog), nameof(OpenLocalFolderAsync), 1, e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
