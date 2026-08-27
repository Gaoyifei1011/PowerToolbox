using System.Reflection;
using System.Resources;

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    internal static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        internal static ResourceManager AdvancedSystemOptionsListResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptionsList", CurrentAssembly);

        internal static ResourceManager AdvancedSystemOptionsPersonalizationResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptionsPersonalization", CurrentAssembly);

        internal static ResourceManager AdvancedSystemOptionsSystemResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptionsSystem", CurrentAssembly);

        internal static ResourceManager AdvancedSystemOptionsResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptions", CurrentAssembly);

        internal static ResourceManager AllToolsResource { get; } = new("PowerToolbox.Strings.AllTools", CurrentAssembly);

        internal static ResourceManager ContextMenuManagerResource { get; } = new("PowerToolbox.Strings.ContextMenuManager", CurrentAssembly);

        internal static ResourceManager DataDecryptResource { get; } = new("PowerToolbox.Strings.DataDecrypt", CurrentAssembly);

        internal static ResourceManager DataEncryptResource { get; } = new("PowerToolbox.Strings.DataEncrypt", CurrentAssembly);

        internal static ResourceManager DataVerifyResource { get; } = new("PowerToolbox.Strings.DataVerify", CurrentAssembly);

        internal static ResourceManager DataVerifyEncryptResource { get; } = new("PowerToolbox.Strings.DataVerifyEncrypt", CurrentAssembly);

        internal static ResourceManager DialogResource { get; } = new("PowerToolbox.Strings.Dialog", CurrentAssembly);

        internal static ResourceManager DownloadManagerResource { get; } = new("PowerToolbox.Strings.DownloadManager", CurrentAssembly);

        internal static ResourceManager DriverManagerResource { get; } = new("PowerToolbox.Strings.DriverManager", CurrentAssembly);

        internal static ResourceManager ExtensionNameResource { get; } = new("PowerToolbox.Strings.ExtensionName", CurrentAssembly);

        internal static ResourceManager FileCertificateResource { get; } = new("PowerToolbox.Strings.FileCertificate", CurrentAssembly);

        internal static ResourceManager FileManagerResource { get; } = new("PowerToolbox.Strings.FileManager", CurrentAssembly);

        internal static ResourceManager FileNameResource { get; } = new("PowerToolbox.Strings.FileName", CurrentAssembly);

        internal static ResourceManager FilePropertiesResource { get; } = new("PowerToolbox.Strings.FileProperties", CurrentAssembly);

        internal static ResourceManager FileUnlockResource { get; } = new("PowerToolbox.Strings.FileUnlock", CurrentAssembly);

        internal static ResourceManager IconExtractResource { get; } = new("PowerToolbox.Strings.IconExtract", CurrentAssembly);

        internal static ResourceManager LoafResource { get; } = new("PowerToolbox.Strings.Loaf", CurrentAssembly);

        internal static ResourceManager LoopbackManagerResource { get; } = new("PowerToolbox.Strings.LoopbackManager", CurrentAssembly);

        internal static ResourceManager NotificationTipResource { get; } = new("PowerToolbox.Strings.NotificationTip", CurrentAssembly);

        internal static ResourceManager PriExtractResource { get; } = new("PowerToolbox.Strings.PriExtract", CurrentAssembly);

        internal static ResourceManager ScheduledTaskManagerResource { get; } = new("PowerToolbox.Strings.ScheduledTaskManager", CurrentAssembly);

        internal static ResourceManager SettingsResource { get; } = new("PowerToolbox.Strings.Settings", CurrentAssembly);

        internal static ResourceManager SettingsAboutResource { get; } = new("PowerToolbox.Strings.SettingsAbout", CurrentAssembly);

        internal static ResourceManager SettingsAdvancedResource { get; } = new("PowerToolbox.Strings.SettingsAdvanced", CurrentAssembly);

        internal static ResourceManager SettingsGeneralResource { get; } = new("PowerToolbox.Strings.SettingsGeneral", CurrentAssembly);

        internal static ResourceManager ShellMenuResource { get; } = new("PowerToolbox.Strings.ShellMenu", CurrentAssembly);

        internal static ResourceManager ShellMenuEditResource { get; } = new("PowerToolbox.Strings.ShellMenuEdit", CurrentAssembly);

        internal static ResourceManager ShellMenuListResource { get; } = new("PowerToolbox.Strings.ShellMenuList", CurrentAssembly);

        internal static ResourceManager SimulateUpdateResource { get; } = new("PowerToolbox.Strings.SimulateUpdate", CurrentAssembly);

        internal static ResourceManager SystemInformationResource { get; } = new("PowerToolbox.Strings.SystemInformation", CurrentAssembly);

        internal static ResourceManager ThemeSwitchResource { get; } = new("PowerToolbox.Strings.ThemeSwitch", CurrentAssembly);

        internal static ResourceManager UpdateManagerResource { get; } = new("PowerToolbox.Strings.UpdateManager", CurrentAssembly);

        internal static ResourceManager UpperAndLowerCaseResource { get; } = new("PowerToolbox.Strings.UpperAndLowerCase", CurrentAssembly);

        internal static ResourceManager WindowResource { get; } = new("PowerToolbox.Strings.Window", CurrentAssembly);

        internal static ResourceManager WinFRResource { get; } = new("PowerToolbox.Strings.WinFR", CurrentAssembly);

        internal static ResourceManager WinSATResource { get; } = new("PowerToolbox.Strings.WinSAT", CurrentAssembly);
    }
}
