using PowerToolboxShellExtension.Extensions.DataType.Constant;
using PowerToolboxShellExtension.Services.Root;

namespace PowerToolboxShellExtension.Services.Controls.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class FileShellMenuService
    {
        private static readonly string settingsKey = ConfigKey.FileShellMenuKey;

        private static readonly bool defaultFileShellMenu = false;

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        public static bool GetFileShellMenu()
        {
            bool? fileShellMenu = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!fileShellMenu.HasValue)
            {
                return defaultFileShellMenu;
            }

            return fileShellMenu.Value;
        }
    }
}
