using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 添加下载任务对话框
    /// </summary>
    public sealed partial class AddDownloadTaskDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string SelectFolderString = ResourceService.DialogResource.GetString("SelectFolder");
        private bool isAllowClosed = false;

        private bool _isPrimaryEnabled;

        public bool IsPrimaryEnabled
        {
            get { return _isPrimaryEnabled; }

            set
            {
                if (!Equals(_isPrimaryEnabled, value))
                {
                    _isPrimaryEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPrimaryEnabled)));
                }
            }
        }

        private string _downloadLinkText;

        public string DownloadLinkText
        {
            get { return _downloadLinkText; }

            set
            {
                if (!string.Equals(_downloadLinkText, value))
                {
                    _downloadLinkText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadLinkText)));
                }
            }
        }

        private string _downloadFileNameText;

        public string DownloadFileNameText
        {
            get { return _downloadFileNameText; }

            set
            {
                if (!string.Equals(_downloadFileNameText, value))
                {
                    _downloadFileNameText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFileNameText)));
                }
            }
        }

        private string _downloadFolderText = DownloadOptionsService.DownloadFolder;

        public string DownloadFolderText
        {
            get { return _downloadFolderText; }

            set
            {
                if (!string.Equals(_downloadFolderText, value))
                {
                    _downloadFolderText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolderText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddDownloadTaskDialog()
        {
            InitializeComponent();
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
        }

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
                    string createFileName = await Task.Run(() =>
                    {
                        try
                        {
                            bool createSucceeded = Uri.TryCreate(DownloadLinkText, UriKind.Absolute, out Uri uri);
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(AddDownloadTaskDialog), nameof(OnDownloadLinkTextChanged), 1, e);
                            return string.Empty;
                        }
                    });

                    if (!string.IsNullOrEmpty(createFileName))
                    {
                        DownloadFileNameText = createFileName;
                        IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
                    }
                }
                else
                {
                    DownloadFileNameText = string.Empty;
                    IsPrimaryButtonEnabled = !string.IsNullOrEmpty(DownloadLinkText) && !string.IsNullOrEmpty(DownloadFolderText);
                }
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
            OpenFolderDialog openFolderDialog = new()
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
                    bool result = await Task.Run(() =>
                    {
                        try
                        {
                            File.Delete(filePath);
                            DownloadSchedulerService.CreateDownload(DownloadLinkText, filePath);
                            return true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(AddDownloadTaskDialog), nameof(OnDownloadClicked), 1, e);
                            return false;
                        }
                    });

                    if (!result)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteFileFailed));
                    }
                }
                // 打开本地目录
                else if (contentDialogResult is ContentDialogResult.Secondary)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            Process.Start(Path.GetDirectoryName(filePath));
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(AddDownloadTaskDialog), nameof(OnDownloadClicked), 2, e);
                        }
                    });
                }
            }
            else
            {
                DownloadSchedulerService.CreateDownload(DownloadLinkText, filePath);
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
    }
}
