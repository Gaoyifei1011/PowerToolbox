using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.PowrProf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项页面
    /// </summary>
    public sealed partial class AdvancedSystemOptionsPage : Page, INotifyPropertyChanged
    {
        private readonly string AlwaysNotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("AlwaysNotify");
        private readonly string NeverNotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("NeverNotify");
        private readonly string NotifyString = ResourceService.AdvancedSystemOptionsResource.GetString("Notify");
        private readonly string NotifyWithoutDimmingString = ResourceService.AdvancedSystemOptionsResource.GetString("NotifyWithoutDimming");
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private readonly Guid Balance = new("381B4222-F694-41F0-9685-FF5BB260DF2E");
        private readonly Guid EnergySaving = new("A1841308-3541-4FAB-BC81-F71556F20B4A");
        private readonly Guid HighPerformance = new("8C5E7FDA-E8BF-4A96-9A85-A6E23A8C635C");
        private readonly Guid OutstandingPerformance = new("E9A42B02-D5DF-448D-AA00-03F14749EB61");
        private bool isUACSettingsNeedClose = false;
        private bool isInitialized = false;

        private ComboBoxItemModel _selectedNotifyMode;

        public ComboBoxItemModel SelectedNotifyMode
        {
            get { return _selectedNotifyMode; }

            set
            {
                if (!Equals(_selectedNotifyMode, value))
                {
                    _selectedNotifyMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNotifyMode)));
                }
            }
        }

        private List<ComboBoxItemModel> NotifyModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPage()
        {
            InitializeComponent();
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = AlwaysNotifyString, SelectedValue = UacLevel.AlwaysNotify });
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = NotifyString, SelectedValue = UacLevel.Notify });
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = NotifyWithoutDimmingString, SelectedValue = UacLevel.NotifyWithoutDimming });
            NotifyModeList.Add(new ComboBoxItemModel() { DisplayMember = NeverNotifyString, SelectedValue = UacLevel.NeverNotify });
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 打开系统设置
        /// </summary>
        private void OnOpenSystemSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:powersleep");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnOpenSystemSettingsClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开控制面板电源选项
        /// </summary>
        private void OnOpenControlPanelClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                IOpenControlPanel openControlPanel = (IOpenControlPanel)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_OpenControlPanel));
                openControlPanel?.Open("Microsoft.PowerOptions", null, 0);
            });
        }

        /// <summary>
        /// 修改电源模式
        /// </summary>
        private void OnChangePowerModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                string tag = Convert.ToString(menuFlyoutItem.Tag);
                Task.Run(() =>
                {
                    switch (tag)
                    {
                        case "EnergySaving":
                            {
                                uint result = PowrProfLibrary.PowerSetActiveScheme(0, EnergySaving);
                                break;
                            }
                        case "Balance":
                            {
                                uint result = PowrProfLibrary.PowerSetActiveScheme(0, Balance);
                                break;
                            }
                        case "HighPerformance":
                            {
                                uint result = PowrProfLibrary.PowerSetActiveScheme(0, HighPerformance);
                                break;
                            }
                        case "OutstandingPerformance":
                            {
                                uint result = PowrProfLibrary.PowerSetActiveScheme(0, OutstandingPerformance);
                                break;
                            }
                    }
                });
            }
        }

        /// <summary>
        /// 关闭用户账户控制浮出控件时触发的事件
        /// </summary>
        private void OnUACSettingsFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (isUACSettingsNeedClose)
            {
                isUACSettingsNeedClose = false;
            }
            else
            {
                args.Cancel = true;
            }
        }

        /// <summary>
        /// 打开用户账户控制浮出控件后发生的事件
        /// </summary>
        private async void OnUACSettingsFlyoutOpened(object sender, object args)
        {
            UacLevel uacLevel = await Task.Run(UACHelper.GetUacLevel);
            SelectedNotifyMode = NotifyModeList.Find((item) => Equals(item.SelectedValue, uacLevel));
        }

        /// <summary>
        /// 关闭用户账户控制设置
        /// </summary>
        private void OnCloseUACSettingsClicked(object sender, RoutedEventArgs args)
        {
            isUACSettingsNeedClose = true;
            UACSettingsFlyout.Hide();
        }

        /// <summary>
        /// 通知模式发生更改时触发的事件
        /// </summary>
        private void OnNotifyModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel notifyMode && !Equals(SelectedNotifyMode, notifyMode))
            {
                SelectedNotifyMode = notifyMode;
            }
        }

        /// <summary>
        /// 修改用户账户控制
        /// </summary>
        private void OnChangeNotifyModeClicked(object sender, RoutedEventArgs args)
        {
            isUACSettingsNeedClose = true;
            UACSettingsFlyout.Hide();
            Task.Run(() =>
            {
                UACHelper.SetUacLevel((UacLevel)SelectedNotifyMode.SelectedValue);
            });
        }

        /// <summary>
        /// 取消修改用户账户控制
        /// </summary>
        private void OnCancelNotifyModeChangeClicked(object sender, RoutedEventArgs args)
        {
            isUACSettingsNeedClose = true;
            UACSettingsFlyout.Hide();
        }

        #endregion 第一部分：高级系统选项页面——挂载的事件

        /// <summary>
        /// 获取选中的通知模式
        /// </summary>
        private Visibility GetSelectedNotifyMode(object selectedValue, object comparedValue)
        {
            return string.Equals(selectedValue, comparedValue) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
