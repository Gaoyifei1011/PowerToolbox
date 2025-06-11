using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using SwitchTheme.Helpers.Root;
using SwitchTheme.Services.Controls.Settings;
using SwitchTheme.Services.Root;
using SwitchTheme.Views.Windows;

// ���� CA1806 ����
#pragma warning disable CA1806

namespace SwitchTheme
{
    public static class Program
    {
        private static readonly NameValueCollection configurationCollection = ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection") as NameValueCollection;

        /// <summary>
        /// Windows ������ ���̳���
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                try
                {
                    Process.Start("windowstools:");
                }
                catch (Exception)
                { }
                return;
            }

            bool isExisted = false;
            Process[] processArray = Process.GetProcessesByName("SwitchTheme");

            foreach (Process process in processArray)
            {
                if (process.Id is not 0 && process.MainWindowHandle != IntPtr.Zero)
                {
                    isExisted = true;
                    break;
                }
            }

            if (!isExisted)
            {
                InitializeProgramResources();

                configurationCollection["DpiAwareness"] = "PerMonitorV2";
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += OnThreadException;
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                if (AutoSwitchThemeService.AutoSwitchThemeEnableValue)
                {
                    SystemTrayService.InitializeSystemTray(ResourceService.SystemTrayResource.GetString("Title"), Application.ExecutablePath);
                    new SystemTrayApp();
                    Application.Run(new SystemTrayWindow());
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
        /// ���� Windows ���� UI �߳��쳣
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Windows Forms Xaml Islands UI Exception", args.Exception);
        }

        /// <summary>
        /// ���� Windows ����� UI �߳��쳣
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Background thread Exception", args.ExceptionObject as Exception);
        }

        /// <summary>
        /// ����Ӧ�ó����������Դ
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Key);

            ThemeService.InitializeTheme();
            AutoSwitchThemeService.InitializeAutoSwitchTheme();
        }
    }
}
