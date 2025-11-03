using Microsoft.Win32;
using ThemeSwitchService.Helpers.Root;

namespace ThemeSwitchService.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
    /// </summary>
    public static class LocalSettingsService
    {
        private static readonly string settingsKey = @"S-1-5-21-2219739030-47996506-626211670-1001\Software\PowerToolbox\Settings";

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            return RegistryHelper.ReadRegistryKey<T>(Registry.Users, settingsKey, key);
        }
    }
}
