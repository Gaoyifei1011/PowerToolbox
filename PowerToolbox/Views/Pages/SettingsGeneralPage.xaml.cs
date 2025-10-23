using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 设置通用选项页面
    /// </summary>
    public sealed partial class SettingsGeneralPage : Page, INotifyPropertyChanged
    {
        private readonly string BackdropAcrylicString = ResourceService.SettingsGeneralResource.GetString("BackdropAcrylic");
        private readonly string BackdropAcrylicBaseString = ResourceService.SettingsGeneralResource.GetString("BackdropAcrylicBase");
        private readonly string BackdropAcrylicThinString = ResourceService.SettingsGeneralResource.GetString("BackdropAcrylicThin");
        private readonly string BackdropDefaultString = ResourceService.SettingsGeneralResource.GetString("BackdropDefault");
        private readonly string BackdropMicaString = ResourceService.SettingsGeneralResource.GetString("BackdropMica");
        private readonly string BackdropMicaAltString = ResourceService.SettingsGeneralResource.GetString("BackdropMicaAlt");
        private readonly string DesktopAcrylicString = ResourceService.SettingsGeneralResource.GetString("DesktopAcrylic");
        private readonly string MicaString = ResourceService.SettingsGeneralResource.GetString("Mica");
        private readonly string ThemeDarkString = ResourceService.SettingsGeneralResource.GetString("ThemeDark");
        private readonly string ThemeDefaultString = ResourceService.SettingsGeneralResource.GetString("ThemeDefault");
        private readonly string ThemeLightAltString = ResourceService.SettingsGeneralResource.GetString("ThemeLight");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        private KeyValuePair<string, string> _theme;

        public KeyValuePair<string, string> Theme
        {
            get { return _theme; }

            set
            {
                if (!Equals(_theme, value))
                {
                    _theme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
                }
            }
        }

        private KeyValuePair<string, string> _backdrop = default;

        public KeyValuePair<string, string> Backdrop
        {
            get { return _backdrop; }

            set
            {
                if (!Equals(_backdrop, value))
                {
                    _backdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Backdrop)));
                }
            }
        }

        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                if (!Equals(_alwaysShowBackdropValue, value))
                {
                    _alwaysShowBackdropValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropValue)));
                }
            }
        }

        private bool _alwaysShowBackdropEnabled;

        public bool AlwaysShowBackdropEnabled
        {
            get { return _alwaysShowBackdropEnabled; }

            set
            {
                if (!Equals(_alwaysShowBackdropEnabled, value))
                {
                    _alwaysShowBackdropEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropEnabled)));
                }
            }
        }

        private bool _advancedEffectsEnabled;

        public bool AdvancedEffectsEnabled
        {
            get { return _advancedEffectsEnabled; }

            set
            {
                if (!Equals(_advancedEffectsEnabled, value))
                {
                    _advancedEffectsEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedEffectsEnabled)));
                }
            }
        }

        private KeyValuePair<string, string> _appLanguage = LanguageService.AppLanguage;

        public KeyValuePair<string, string> AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                if (!Equals(_appLanguage, value))
                {
                    _appLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLanguage)));
                }
            }
        }

        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                if (!Equals(_topMostValue, value))
                {
                    _topMostValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopMostValue)));
                }
            }
        }

        private List<KeyValuePair<string, string>> ThemeList { get; } = [];

        private List<KeyValuePair<string, string>> BackdropList { get; } = [];

        private WinRTObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsGeneralPage()
        {
            InitializeComponent();

            AdvancedEffectsEnabled = IsAdvancedEffectsEnabled();
            ThemeList.Add(new KeyValuePair<string, string>(ThemeService.ThemeList[0], ThemeDefaultString));
            ThemeList.Add(new KeyValuePair<string, string>(ThemeService.ThemeList[1], ThemeLightAltString));
            ThemeList.Add(new KeyValuePair<string, string>(ThemeService.ThemeList[2], ThemeDarkString));
            Theme = ThemeList.Find(item => string.Equals(item.Key, ThemeService.AppTheme, StringComparison.OrdinalIgnoreCase));

            BackdropList.Add(new KeyValuePair<string, string>(BackdropService.BackdropList[0], BackdropDefaultString));
            BackdropList.Add(new KeyValuePair<string, string>(BackdropService.BackdropList[1], BackdropMicaString));
            BackdropList.Add(new KeyValuePair<string, string>(BackdropService.BackdropList[2], BackdropMicaAltString));
            BackdropList.Add(new KeyValuePair<string, string>(BackdropService.BackdropList[3], BackdropAcrylicString));
            BackdropList.Add(new KeyValuePair<string, string>(BackdropService.BackdropList[4], BackdropAcrylicBaseString));
            BackdropList.Add(new KeyValuePair<string, string>(BackdropService.BackdropList[5], BackdropAcrylicThinString));
            Backdrop = BackdropList.Find(item => string.Equals(item.Key, BackdropService.AppBackdrop, StringComparison.OrdinalIgnoreCase));

            foreach (KeyValuePair<string, string> languageItem in LanguageService.LanguageList)
            {
                if (string.Equals(LanguageService.AppLanguage.Key, languageItem.Key))
                {
                    AppLanguage = languageItem;
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = true
                    });
                }
                else
                {
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = false
                    });
                }
            }

            AlwaysShowBackdropEnabled = IsAdvancedEffectsEnabled() && !string.Equals(Backdrop.Key, BackdropList[0].Key);
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        #region 第一部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 修改应用语言
        /// </summary>
        private async void OnLanguageExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            if (args.Parameter is LanguageModel language)
            {
                foreach (LanguageModel languageItem in LanguageCollection)
                {
                    languageItem.IsChecked = false;
                    if (string.Equals(language.LangaugeInfo.Key, languageItem.LangaugeInfo.Key))
                    {
                        AppLanguage = languageItem.LangaugeInfo;
                        languageItem.IsChecked = true;
                    }
                }

                LanguageService.SetLanguage(AppLanguage);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.LanguageChange));
            }
        }

        #endregion 第一部分：ExecuteCommand 命令调用时挂载的事件

        #region 第二部分：设置通用选项页面——挂载的事件

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:colors");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnSystemThemeSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        private void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> theme)
            {
                Theme = theme;
                ThemeService.SetTheme(Theme.Key);
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        private void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> backdrop)
            {
                Backdrop = backdrop;
                BackdropService.SetBackdrop(Backdrop.Key);
                AlwaysShowBackdropEnabled = IsAdvancedEffectsEnabled() && !string.Equals(Backdrop.Key, BackdropList[0].Key);

                if (Equals(Backdrop, BackdropList[0]))
                {
                    AlwaysShowBackdropService.SetAlwaysShowBackdropValue(false);
                    AlwaysShowBackdropValue = false;
                }
            }
        }

        /// <summary>
        /// 打开系统主题色设置
        /// </summary>
        private void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:easeofaccess-visualeffects");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnSystemBackdropSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开系统语言设置
        /// </summary>
        private void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:regionlanguage-languageoptions");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnSystemLanguageSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 是否开启始终显示背景色
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdropValue(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 语言设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnOpened(object sender, object args)
        {
            foreach (LanguageModel languageItem in LanguageCollection)
            {
                if (languageItem.IsChecked)
                {
                    LanguageListView.ScrollIntoView(languageItem);
                    break;
                }
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostValue = toggleSwitch.IsOn;
            }
        }

        #endregion 第二部分：设置通用选项页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 在用户首选项发生更改时触发的事件
        /// </summary>
        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs args)
        {
            synchronizationContext.Post(_ =>
            {
                bool isAdvancedEffectsEnabled = IsAdvancedEffectsEnabled();
                AdvancedEffectsEnabled = isAdvancedEffectsEnabled;
                AlwaysShowBackdropEnabled = isAdvancedEffectsEnabled && !string.Equals(Backdrop.Key, BackdropList[0].Key);
            }, null);
        }

        /// <summary>
        /// 应用程序即将关闭时发生的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SettingsGeneralPage), nameof(OnApplicationExit), 1, e);
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 检查是否启用系统透明度效果设置
        /// </summary>
        private bool IsAdvancedEffectsEnabled()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency");
        }

        private string LocalizeDisplayNumber(KeyValuePair<string, string> selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => item.Key.Equals(selectedBackdrop.Key));

            if (index is 0)
            {
                return selectedBackdrop.Value;
            }
            else if (index is 1 or 2)
            {
                return string.Join(" ", MicaString, selectedBackdrop.Value);
            }
            else if (index is 3 or 4 or 5)
            {
                return string.Join(" ", DesktopAcrylicString, selectedBackdrop.Value);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
