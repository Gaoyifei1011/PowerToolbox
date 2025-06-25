﻿using System.IO;
using System.Reflection;
using System.Resources;

namespace ThemeSwitch.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "PowerToolbox.exe"));

        public static ResourceManager ThemeSwitchTrayResource { get; } = new("PowerToolbox.Strings.ThemeSwitchTray", CurrentAssembly);
    }
}