using PowerToolbox.Extensions.DataType.Constant;
using PowerToolbox.Services.Root;

namespace PowerToolbox.Services.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class FileShellMenuService
    {
        private static readonly string settingsKey = ConfigKey.FileShellMenuKey;

        private static readonly bool defaultFileShellMenu = true;

        public static bool FileShellMenu { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的文件右键菜单显示值
        /// </summary>
        public static void InitializeFileShellMenu()
        {
            FileShellMenu = GetFileShellMenu();
        }

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetFileShellMenu()
        {
            bool? fileShellMenu = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!fileShellMenu.HasValue)
            {
                SetFileShellMenu(defaultFileShellMenu);
                return defaultFileShellMenu;
            }

            return fileShellMenu.Value;
        }

        /// <summary>
        /// 文件右键菜单显示值发生修改时修改设置存储的文件右键菜单显示值
        /// </summary>
        public static void SetFileShellMenu(bool fileShellMenu)
        {
            FileShellMenu = fileShellMenu;
            LocalSettingsService.SaveSetting(settingsKey, fileShellMenu);
        }
    }
}
