using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using ThemeSwitch.Helpers.Root;
using ThemeSwitch.Services.Controls.Settings;
using ThemeSwitch.Services.Root;
using ThemeSwitch.Views.Windows;

// ���� CA1806 ����
#pragma warning disable CA1806

namespace ThemeSwitch
{
    public static class Program
    {
        private static readonly NameValueCollection configurationCollection = ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection") as NameValueCollection;
        private static string ThemeSwitchString;

        /// <summary>
        /// PowerTools ���̳���
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                try
                {
                    Process.Start("powertools:");
                }
                catch (Exception)
                { }
                return;
            }

            bool isExisted = false;
            Process[] processArray = Process.GetProcessesByName("ThemeSwitch");

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
                ThemeSwitchString = ResourceService.ThemeSwitchResource.GetString("ThemeSwitch");
                configurationCollection["DpiAwareness"] = "PerMonitorV2";
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += OnThreadException;
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
                {
                    SystemTrayService.InitializeSystemTray(ThemeSwitchString, Application.ExecutablePath);
                    new ThemeSwitchApp();
                    Application.Run(new ThemeSwitchWindow());
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
            AutoThemeSwitchService.InitializeAutoThemeSwitch();
        }
    }
}
