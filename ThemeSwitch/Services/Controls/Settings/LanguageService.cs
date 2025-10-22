using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ThemeSwitch.Extensions.DataType.Constant;
using ThemeSwitch.Services.Root;
using ThemeSwitch.WindowsAPI.ComTypes;
using ThemeSwitch.WindowsAPI.PInvoke.Shlwapi;
using ThemeSwitch.WindowsAPI.PInvoke.User32;
using Windows.Globalization;

namespace ThemeSwitch.Services.Controls.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static readonly string settingsKey = ConfigKey.LanguageKey;
        private static readonly Guid CLSID_AppxFactory = new("5842A140-FF9F-4166-8F5C-62F5B7B0C781");
        private static readonly IAppxFactory appxFactory = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_AppxFactory)) as IAppxFactory;
        private static KeyValuePair<string, string> defaultAppLanguage;

        public static KeyValuePair<string, string> AppLanguage { get; private set; }

        public static FlowDirection FlowDirection { get; private set; }

        private static readonly List<string> AppLanguagesList = [];

        public static List<KeyValuePair<string, string>> LanguageList { get; } = [];

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static void InitializeLanguage()
        {
            InitializeLanguageList();

            defaultAppLanguage = LanguageList.Find(item => string.Equals(item.Key, "en-US", StringComparison.OrdinalIgnoreCase));

            AppLanguage = GetLanguage();
        }

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            try
            {
                if (ShlwapiLibrary.SHCreateStreamOnFileEx(Path.Combine(AppContext.BaseDirectory, "AppxManifest.xml"), STGM.STGM_SHARE_DENY_READ | STGM.STGM_SHARE_EXCLUSIVE, 0, false, null, out IStream stream) is 0)
                {
                    appxFactory.CreateManifestReader(stream, out IAppxManifestReader2 appxManifestReader);

                    if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
                    {
                        while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                        {
                            if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                            {
                                AppLanguagesList.Add(language);
                            }

                            appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                        }
                    }

                    Marshal.Release(Marshal.GetIUnknownForObject(stream));
                }

                if (AppLanguagesList.Count is 0)
                {
                    AppLanguagesList.Add("en-us");
                }

                AppLanguagesList.Sort();
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(ThemeSwitch), nameof(LanguageService), nameof(InitializeLanguageList), 1, e);
                AppLanguagesList.Clear();
                AppLanguagesList.Add("en-us");
            }

            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);

                LanguageList.Add(new KeyValuePair<string, string>(culture.Name, culture.NativeName));
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(CultureInfo currentCulture, out KeyValuePair<string, string> language)
        {
            foreach (KeyValuePair<string, string> languageItem in LanguageList)
            {
                if (string.Equals(languageItem.Key, currentCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    language = languageItem;
                    return true;
                }
            }

            language = new();
            return false;
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetLanguage()
        {
            string language = LocalSettingsService.ReadSetting<string>(settingsKey);

            // 当前系统语言和当前系统语言的父区域性的语言
            CultureInfo currentCultureInfo = CultureInfo.CurrentCulture.Parent.Parent;
            CultureInfo currentParentCultureInfo = CultureInfo.CurrentCulture.Parent.Parent.Parent;
            bool existResult = false;

            if (string.IsNullOrEmpty(language))
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                existResult = IsExistsInLanguageList(currentCultureInfo, out KeyValuePair<string, string> currentLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (existResult)
                {
                    SetLanguage(currentLanguage);
                    FlowDirection = currentCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    User32Library.SetProcessDefaultLayout(Convert.ToUInt32(currentCultureInfo.TextInfo.IsRightToLeft));
                    return currentLanguage;
                }
                else
                {
                    existResult = IsExistsInLanguageList(currentParentCultureInfo, out KeyValuePair<string, string> currentParentLanguage);

                    // 如果存在，设置存储值和应用初次设置的语言为当前系统语言的父区域性的语言
                    if (existResult)
                    {
                        SetLanguage(currentParentLanguage);
                        FlowDirection = currentParentCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                        User32Library.SetProcessDefaultLayout(Convert.ToUInt32(currentParentCultureInfo.TextInfo.IsRightToLeft));
                        return currentParentLanguage;
                    }

                    // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                    else
                    {
                        SetLanguage(defaultAppLanguage);
                        CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(defaultAppLanguage.Key);
                        FlowDirection = defaultCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                        User32Library.SetProcessDefaultLayout(Convert.ToUInt32(defaultCultureInfo.TextInfo.IsRightToLeft));
                        return defaultAppLanguage;
                    }
                }
            }

            CultureInfo savedCultureInfo = CultureInfo.GetCultureInfo(language);
            FlowDirection = savedCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            return LanguageList.Find(item => string.Equals(language, item.Key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static void SetLanguage(KeyValuePair<string, string> language)
        {
            AppLanguage = language;
            ApplicationLanguages.PrimaryLanguageOverride = language.Key;
        }
    }
}
