using System;
using System.Diagnostics;
using System.ServiceProcess;
using ThemeSwitchService.Helpers.Root;
using ThemeSwitchService.Services.Root;

namespace ThemeSwitchService
{
    /// <summary>
    /// 应用程序的主入口点
    /// </summary>
    public class Program
    {
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

            LogService.Initialize();
            ServiceBase.Run(new ThemeSwitchRootService());
        }
    }
}
