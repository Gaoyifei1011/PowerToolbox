using System.Reflection;
using System.Resources;

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        public static ResourceManager AllToolsResource { get; } = new("PowerToolbox.Strings.AllTools", CurrentAssembly);

        public static ResourceManager ContextMenuManagerResource { get; } = new("PowerToolbox.Strings.ContextMenuManager", CurrentAssembly);

        public static ResourceManager DataEncryptResource { get; } = new("PowerToolbox.Strings.DataEncrypt", CurrentAssembly);

        public static ResourceManager DataVerifyResource { get; } = new("PowerToolbox.Strings.DataVerify", CurrentAssembly);

        public static ResourceManager DataVerifyEncryptResource { get; } = new("PowerToolbox.Strings.DataVerifyEncrypt", CurrentAssembly);

        public static ResourceManager DialogResource { get; } = new("PowerToolbox.Strings.Dialog", CurrentAssembly);

        public static ResourceManager DownloadManagerResource { get; } = new("PowerToolbox.Strings.DownloadManager", CurrentAssembly);

        public static ResourceManager DriverManagerResource { get; } = new("PowerToolbox.Strings.DriverManager", CurrentAssembly);

        public static ResourceManager ExperimentalFeatureManagerResource { get; } = new("PowerToolbox.Strings.ExperimentalFeatureManager", CurrentAssembly);

        public static ResourceManager ExtensionNameResource { get; } = new("PowerToolbox.Strings.ExtensionName", CurrentAssembly);

        public static ResourceManager FileCertificateResource { get; } = new("PowerToolbox.Strings.FileCertificate", CurrentAssembly);

        public static ResourceManager FileManagerResource { get; } = new("PowerToolbox.Strings.FileManager", CurrentAssembly);

        public static ResourceManager FileNameResource { get; } = new("PowerToolbox.Strings.FileName", CurrentAssembly);

        public static ResourceManager FilePropertiesResource { get; } = new("PowerToolbox.Strings.FileProperties", CurrentAssembly);

        public static ResourceManager FileUnlockResource { get; } = new("PowerToolbox.Strings.FileUnlock", CurrentAssembly);

        public static ResourceManager HostsResource { get; } = new("PowerToolbox.Strings.Hosts", CurrentAssembly);

        public static ResourceManager IconExtractResource { get; } = new("PowerToolbox.Strings.IconExtract", CurrentAssembly);

        public static ResourceManager LoafResource { get; } = new("PowerToolbox.Strings.Loaf", CurrentAssembly);

        public static ResourceManager LoopbackManagerResource { get; } = new("PowerToolbox.Strings.LoopbackManager", CurrentAssembly);

        public static ResourceManager NotificationTipResource { get; } = new("PowerToolbox.Strings.NotificationTip", CurrentAssembly);

        public static ResourceManager PriExtractResource { get; } = new("PowerToolbox.Strings.PriExtract", CurrentAssembly);

        public static ResourceManager ScheduledTaskManagerResource { get; } = new("PowerToolbox.Strings.ScheduledTaskManager", CurrentAssembly);

        public static ResourceManager SettingsResource { get; } = new("PowerToolbox.Strings.Settings", CurrentAssembly);

        public static ResourceManager SettingsAboutResource { get; } = new("PowerToolbox.Strings.SettingsAbout", CurrentAssembly);

        public static ResourceManager SettingsAdvancedResource { get; } = new("PowerToolbox.Strings.SettingsAdvanced", CurrentAssembly);

        public static ResourceManager SettingsGeneralResource { get; } = new("PowerToolbox.Strings.SettingsGeneral", CurrentAssembly);

        public static ResourceManager ShellMenuResource { get; } = new("PowerToolbox.Strings.ShellMenu", CurrentAssembly);

        public static ResourceManager ShellMenuEditResource { get; } = new("PowerToolbox.Strings.ShellMenuEdit", CurrentAssembly);

        public static ResourceManager ShellMenuListResource { get; } = new("PowerToolbox.Strings.ShellMenuList", CurrentAssembly);

        public static ResourceManager SimulateUpdateResource { get; } = new("PowerToolbox.Strings.SimulateUpdate", CurrentAssembly);

        public static ResourceManager ThemeSwitchResource { get; } = new("PowerToolbox.Strings.ThemeSwitch", CurrentAssembly);

        public static ResourceManager UpdateManagerResource { get; } = new("PowerToolbox.Strings.UpdateManager", CurrentAssembly);

        public static ResourceManager UpperAndLowerCaseResource { get; } = new("PowerToolbox.Strings.UpperAndLowerCase", CurrentAssembly);

        public static ResourceManager WindowResource { get; } = new("PowerToolbox.Strings.Window", CurrentAssembly);

        public static ResourceManager WinFRResource { get; } = new("PowerToolbox.Strings.WinFR", CurrentAssembly);

        public static ResourceManager WinSATResource { get; } = new("PowerToolbox.Strings.WinSAT", CurrentAssembly);
    }
}
