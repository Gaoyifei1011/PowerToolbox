using System;
using System.Reflection;

namespace PowerToolbox.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    internal static class InfoHelper
    {
        internal static Version AppVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        internal static Version SystemVersion { get; } = Environment.OSVersion.Version;
    }
}
