using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using System;
using System.Diagnostics;
using System.Drawing;

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
            LogService.WriteLog(TraceEventType.Warning, nameof(PowerToolbox), nameof(MainApp), nameof(OnUnhandledException), 1, args.Exception);
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private void SetAppIcon(AppWindow appWindow)
        {
            try
            {
                Icon icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

                if (icon is not null)
                {
                    appWindow.SetIcon(new IconId() { Value = (ulong)icon.Handle });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainApp), nameof(SetAppIcon), 1, e);
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
