using PowerToolbox.Extensions.DataType.Constant;
using PowerToolbox.Services.Root;
using System.ComponentModel;

namespace PowerToolbox.Services.Settings
{
    /// <summary>
    /// 始终显示背景色设置服务
    /// </summary>
    public static class AlwaysShowBackdropService
    {
        private static readonly string settingsKey = ConfigKey.AlwaysShowBackdropKey;

        private static readonly bool defaultAlwaysShowBackdrop = false;

        private static bool _alwaysShowBackdrop;

        public static bool AlwaysShowBackdrop
        {
            get { return _alwaysShowBackdrop; }

            private set
            {
                if (!Equals(_alwaysShowBackdrop, value))
                {
                    _alwaysShowBackdrop = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AlwaysShowBackdrop)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的始终显示背景色值
        /// </summary>
        public static void InitializeAlwaysShowBackdrop()
        {
            AlwaysShowBackdrop = GetAlwaysShowBackdrop();
        }

        /// <summary>
        /// 获取设置存储的始终显示背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAlwaysShowBackdrop()
        {
            bool? alwaysShowBackdrop = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!alwaysShowBackdrop.HasValue)
            {
                SetAlwaysShowBackdrop(defaultAlwaysShowBackdrop);
                return defaultAlwaysShowBackdrop;
            }

            return alwaysShowBackdrop.Value;
        }

        /// <summary>
        /// 始终显示背景色发生修改时修改设置存储的始终显示背景色值
        /// </summary>
        public static void SetAlwaysShowBackdrop(bool alwaysShowBackdrop)
        {
            AlwaysShowBackdrop = alwaysShowBackdrop;
            LocalSettingsService.SaveSetting(settingsKey, alwaysShowBackdrop);
        }
    }
}
