using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using PowerToolbox.Extensions.Threading;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Services.Shell;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox
{
    /// <summary>
    /// PowerToolbox 桌面程序
    /// </summary>
    public class Program
    {
        private static readonly Guid CLSID_ApplicationActivationManager = new("45BA127D-10A8-46EA-8AB7-56EA9078943C");
        private static readonly IApplicationActivationManager applicationActivationManager = (IApplicationActivationManager)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ApplicationActivationManager));

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (RuntimeHelper.IsMSIX)
            {
                if (RuntimeHelper.IsElevated && args.Length is 1 && args[0] is "--elevated")
                {
                    uint aumidLength = 260;
                    StringBuilder aumidBuilder = new((int)aumidLength);
                    Kernel32Library.GetCurrentApplicationUserModelId(ref aumidLength, aumidBuilder);
                    applicationActivationManager.ActivateApplication(Convert.ToString(aumidBuilder), string.Empty, ACTIVATEOPTIONS.AO_NONE, out uint _);
                    return;
                }
            }
            else
            {
                try
                {
                    Process.Start("powertoolbox:");
                }
                catch (Exception)
                { }
                return;
            }

            InitializeProgramResources();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                new MainApp();
            });
        }

        /// <summary>
        /// 处理应用非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(TraceEventType.Warning, nameof(PowerToolbox), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Key);

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            TopMostService.InitializeTopMost();

            DownloadOptionsService.InitializeDownloadOptions();
            FileShellMenuService.InitializeFileShellMenu();
            AutoThemeSwitchService.InitializeAutoThemeSwitch();
            ShellMenuService.InitializeShellMenu();

            DownloadSchedulerService.InitializeDownloadScheduler();
        }
    }
}
