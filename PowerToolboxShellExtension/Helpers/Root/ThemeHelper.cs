namespace PowerToolboxShellExtension.Helpers.Root
{
    /// <summary>
    /// 系统主题辅助类
    /// </summary>
    internal static class ThemeHelper
    {
        internal static bool AppsUseLightTheme { get; } = false;

        static ThemeHelper()
        {
            bool? appsUseLightTheme = RegistryHelper.ReadRegistryKey<bool?>(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme");
            AppsUseLightTheme = appsUseLightTheme.HasValue && appsUseLightTheme.Value;
        }
    }
}
