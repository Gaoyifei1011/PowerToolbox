using PowerToolbox.Extensions.Resources;
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

        internal static ResourceManager AdvancedSystemOptionsListResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.AdvancedSystemOptionsList", CurrentAssembly);

        internal static ResourceManager AdvancedSystemOptionsPersonalizationResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.AdvancedSystemOptionsPersonalization", CurrentAssembly);

        internal static ResourceManager AdvancedSystemOptionsSystemResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.AdvancedSystemOptionsSystem", CurrentAssembly);

        internal static ResourceManager AdvancedSystemOptionsResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.AdvancedSystemOptions", CurrentAssembly);

        internal static ResourceManager AllToolsResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.AllTools", CurrentAssembly);

        internal static ResourceManager ContextMenuManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ContextMenuManager", CurrentAssembly);

        internal static ResourceManager DataDecryptResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.DataDecrypt", CurrentAssembly);

        internal static ResourceManager DataEncryptResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.DataEncrypt", CurrentAssembly);

        internal static ResourceManager DataVerifyResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.DataVerify", CurrentAssembly);

        internal static ResourceManager DataVerifyEncryptResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.DataVerifyEncrypt", CurrentAssembly);

        internal static ResourceManager DialogResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.Dialog", CurrentAssembly);

        internal static ResourceManager DownloadManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.DownloadManager", CurrentAssembly);

        internal static ResourceManager DriverManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.DriverManager", CurrentAssembly);

        internal static ResourceManager ExtensionNameResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ExtensionName", CurrentAssembly);

        internal static ResourceManager FileCertificateResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.FileCertificate", CurrentAssembly);

        internal static ResourceManager FileManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.FileManager", CurrentAssembly);

        internal static ResourceManager FileNameResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.FileName", CurrentAssembly);

        internal static ResourceManager FilePropertiesResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.FileProperties", CurrentAssembly);

        internal static ResourceManager FileUnlockResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.FileUnlock", CurrentAssembly);

        internal static ResourceManager IconExtractResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.IconExtract", CurrentAssembly);

        internal static ResourceManager LoafResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.Loaf", CurrentAssembly);

        internal static ResourceManager LoopbackManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.LoopbackManager", CurrentAssembly);

        internal static ResourceManager NotificationTipResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.NotificationTip", CurrentAssembly);

        internal static ResourceManager PriExtractResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.PriExtract", CurrentAssembly);

        internal static ResourceManager ScheduledTaskManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ScheduledTaskManager", CurrentAssembly);

        internal static ResourceManager SettingsResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.Settings", CurrentAssembly);

        internal static ResourceManager SettingsAboutResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.SettingsAbout", CurrentAssembly);

        internal static ResourceManager SettingsAdvancedResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.SettingsAdvanced", CurrentAssembly);

        internal static ResourceManager SettingsGeneralResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.SettingsGeneral", CurrentAssembly);

        internal static ResourceManager ShellMenuResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ShellMenu", CurrentAssembly);

        internal static ResourceManager ShellMenuEditResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ShellMenuEdit", CurrentAssembly);

        internal static ResourceManager ShellMenuListResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ShellMenuList", CurrentAssembly);

        internal static ResourceManager SimulateUpdateResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.SimulateUpdate", CurrentAssembly);

        internal static ResourceManager SystemInformationResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.SystemInformation", CurrentAssembly);

        internal static ResourceManager ThemeSwitchResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.ThemeSwitch", CurrentAssembly);

        internal static ResourceManager UpdateManagerResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.UpdateManager", CurrentAssembly);

        internal static ResourceManager UpperAndLowerCaseResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.UpperAndLowerCase", CurrentAssembly);

        internal static ResourceManager WindowResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.Window", CurrentAssembly);

        internal static ResourceManager WinFRResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.WinFR", CurrentAssembly);

        internal static ResourceManager WinSATResource { get; } = new EmbeddedResourceManager("PowerToolbox.Strings.WinSAT", CurrentAssembly);
    }
}
