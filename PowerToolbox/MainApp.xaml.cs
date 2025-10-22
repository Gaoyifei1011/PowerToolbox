using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Diagnostics.Tracing;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerToolbox
{
    /// <summary>
    /// PowerToolbox 应用程序
    /// </summary>
    public partial class MainApp : Application, IDisposable
    {
        private bool isDisposed;

        public Window MainWindow { get; private set; }

        public MainApp()
        {
            InitializeComponent();
            DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 启动应用程序时调用，初始化应用主窗口
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            MainWindow = new MainWindow();
            MainWindow.Activate();
            SetAppIcon(MainWindow.AppWindow);
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, nameof(PowerToolbox), nameof(MainApp), nameof(OnUnhandledException), 1, args.Exception);
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private void SetAppIcon(AppWindow appWindow)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(System.Windows.Forms.Application.ExecutablePath, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            nint[] hIcons = new nint[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(System.Windows.Forms.Application.ExecutablePath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
            {
                appWindow.SetIcon(new IconId() { Value = (ulong)hIcons[0] });
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MainApp()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                GlobalNotificationService.SendNotification();
                DownloadSchedulerService.TerminateDownload();
                DownloadSchedulerService.CloseDownloadScheduler();
                LogService.CloseLog();
                isDisposed = true;
                Environment.Exit(0);
            }
        }
    }
}
