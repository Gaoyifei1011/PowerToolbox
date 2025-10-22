using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Threading;
using ThemeSwitch.Extensions.Threading;
using ThemeSwitch.Helpers.Root;
using ThemeSwitch.Services.Controls.Settings;
using ThemeSwitch.Services.Root;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace ThemeSwitch
{
    public static class Program
    {
        private static string ThemeSwitchString;

        /// <summary>
        /// PowerToolbox 托盘程序
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                try
                {
                    Process.Start("powertoolbox:");
                }
                catch (Exception)
                { }
                return;
            }

            bool isExisted = false;
            Process[] processArray = Process.GetProcessesByName("ThemeSwitch");

            foreach (Process process in processArray)
            {
                if (process.Id is not 0 && !process.MainWindowHandle.Equals(IntPtr.Zero))
                {
                    isExisted = true;
                    break;
                }
            }

            if (!isExisted)
            {
                InitializeProgramResources();
                ThemeSwitchString = ResourceService.ThemeSwitchTrayResource.GetString("ThemeSwitch");
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
                {
                    SystemTrayService.InitializeSystemTray(ThemeSwitchString, System.Windows.Forms.Application.ExecutablePath);
                    Application.Start((param) =>
                    {
                        SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                        new ThemeSwitchApp();
                    });
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 处理应用非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, nameof(ThemeSwitch), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Key);

            ThemeService.InitializeTheme();
            AutoThemeSwitchService.InitializeAutoThemeSwitch();
        }
    }
}
