using PowerToolbox.Extensions.Resources;
using System.Reflection;

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    internal static class ResourceService
    {
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        internal static EmbeddedResourceManager AdvancedSystemOptionsListResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptionsList", CurrentAssembly);

        internal static EmbeddedResourceManager AdvancedSystemOptionsPersonalizationResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptionsPersonalization", CurrentAssembly);

        internal static EmbeddedResourceManager AdvancedSystemOptionsSystemResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptionsSystem", CurrentAssembly);

        internal static EmbeddedResourceManager AdvancedSystemOptionsResource { get; } = new("PowerToolbox.Strings.AdvancedSystemOptions", CurrentAssembly);

        internal static EmbeddedResourceManager AllToolsResource { get; } = new("PowerToolbox.Strings.AllTools", CurrentAssembly);

        internal static EmbeddedResourceManager ContextMenuManagerResource { get; } = new("PowerToolbox.Strings.ContextMenuManager", CurrentAssembly);

        internal static EmbeddedResourceManager DataDecryptResource { get; } = new("PowerToolbox.Strings.DataDecrypt", CurrentAssembly);

        internal static EmbeddedResourceManager DataEncryptResource { get; } = new("PowerToolbox.Strings.DataEncrypt", CurrentAssembly);

        internal static EmbeddedResourceManager DataVerifyResource { get; } = new("PowerToolbox.Strings.DataVerify", CurrentAssembly);

        internal static EmbeddedResourceManager DataVerifyEncryptResource { get; } = new("PowerToolbox.Strings.DataVerifyEncrypt", CurrentAssembly);

        internal static EmbeddedResourceManager DialogResource { get; } = new("PowerToolbox.Strings.Dialog", CurrentAssembly);

        internal static EmbeddedResourceManager DownloadManagerResource { get; } = new("PowerToolbox.Strings.DownloadManager", CurrentAssembly);

        internal static EmbeddedResourceManager DriverManagerResource { get; } = new("PowerToolbox.Strings.DriverManager", CurrentAssembly);

        internal static EmbeddedResourceManager ExtensionNameResource { get; } = new("PowerToolbox.Strings.ExtensionName", CurrentAssembly);

        internal static EmbeddedResourceManager FileCertificateResource { get; } = new("PowerToolbox.Strings.FileCertificate", CurrentAssembly);

        internal static EmbeddedResourceManager FileManagerResource { get; } = new("PowerToolbox.Strings.FileManager", CurrentAssembly);

        internal static EmbeddedResourceManager FileNameResource { get; } = new("PowerToolbox.Strings.FileName", CurrentAssembly);

        internal static EmbeddedResourceManager FilePropertiesResource { get; } = new("PowerToolbox.Strings.FileProperties", CurrentAssembly);

        internal static EmbeddedResourceManager FileUnlockResource { get; } = new("PowerToolbox.Strings.FileUnlock", CurrentAssembly);

        internal static EmbeddedResourceManager IconExtractResource { get; } = new("PowerToolbox.Strings.IconExtract", CurrentAssembly);

        internal static EmbeddedResourceManager LoafResource { get; } = new("PowerToolbox.Strings.Loaf", CurrentAssembly);

        internal static EmbeddedResourceManager LoopbackManagerResource { get; } = new("PowerToolbox.Strings.LoopbackManager", CurrentAssembly);

        internal static EmbeddedResourceManager NotificationTipResource { get; } = new("PowerToolbox.Strings.NotificationTip", CurrentAssembly);

        internal static EmbeddedResourceManager PriExtractResource { get; } = new("PowerToolbox.Strings.PriExtract", CurrentAssembly);

        internal static EmbeddedResourceManager ScheduledTaskManagerResource { get; } = new("PowerToolbox.Strings.ScheduledTaskManager", CurrentAssembly);

        internal static EmbeddedResourceManager SettingsResource { get; } = new("PowerToolbox.Strings.Settings", CurrentAssembly);

        internal static EmbeddedResourceManager SettingsAboutResource { get; } = new("PowerToolbox.Strings.SettingsAbout", CurrentAssembly);

        internal static EmbeddedResourceManager SettingsAdvancedResource { get; } = new("PowerToolbox.Strings.SettingsAdvanced", CurrentAssembly);

        internal static EmbeddedResourceManager SettingsGeneralResource { get; } = new("PowerToolbox.Strings.SettingsGeneral", CurrentAssembly);

        internal static EmbeddedResourceManager ShellMenuResource { get; } = new("PowerToolbox.Strings.ShellMenu", CurrentAssembly);

        internal static EmbeddedResourceManager ShellMenuEditResource { get; } = new("PowerToolbox.Strings.ShellMenuEdit", CurrentAssembly);

        internal static EmbeddedResourceManager ShellMenuListResource { get; } = new("PowerToolbox.Strings.ShellMenuList", CurrentAssembly);

        internal static EmbeddedResourceManager SimulateUpdateResource { get; } = new("PowerToolbox.Strings.SimulateUpdate", CurrentAssembly);

        internal static EmbeddedResourceManager SystemInformationResource { get; } = new("PowerToolbox.Strings.SystemInformation", CurrentAssembly);

        internal static EmbeddedResourceManager ThemeSwitchResource { get; } = new("PowerToolbox.Strings.ThemeSwitch", CurrentAssembly);

        internal static EmbeddedResourceManager UpdateManagerResource { get; } = new("PowerToolbox.Strings.UpdateManager", CurrentAssembly);

        internal static EmbeddedResourceManager UpperAndLowerCaseResource { get; } = new("PowerToolbox.Strings.UpperAndLowerCase", CurrentAssembly);

        internal static EmbeddedResourceManager WindowResource { get; } = new("PowerToolbox.Strings.Window", CurrentAssembly);

        internal static EmbeddedResourceManager WinFRResource { get; } = new("PowerToolbox.Strings.WinFR", CurrentAssembly);

        internal static EmbeddedResourceManager WinSATResource { get; } = new("PowerToolbox.Strings.WinSAT", CurrentAssembly);
    }
}
