using System;
using System.ServiceProcess;

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
            ServiceBase.Run(new ThemeSwitchRootService());
        }
    }
}
