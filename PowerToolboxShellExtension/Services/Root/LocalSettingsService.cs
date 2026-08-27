using PowerToolboxShellExtension.Helpers.Root;

namespace PowerToolboxShellExtension.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
    /// </summary>
    internal static class LocalSettingsService
    {
        private static readonly string settingsKey = @"Software\PowerToolbox\Settings";

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        internal static T ReadSetting<T>(string key)
        {
            return RegistryHelper.ReadRegistryKey<T>(settingsKey, key);
        }
    }
}
