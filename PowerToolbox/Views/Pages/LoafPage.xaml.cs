using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 摸鱼页面
    /// </summary>
    public sealed partial class LoafPage : Page, INotifyPropertyChanged
    {
        private readonly string LoafTimeString = ResourceService.LoafResource.GetString("LoafTime");
        private readonly string LockScreenString = ResourceService.LoafResource.GetString("LockScreen");
        private readonly string LogOffString = ResourceService.LoafResource.GetString("LogOff");
        private readonly string NoneString = ResourceService.LoafResource.GetString("None");
        private readonly string RestartString = ResourceService.LoafResource.GetString("Restart");
        private readonly string ShutdownString = ResourceService.LoafResource.GetString("Shutdown");
        private readonly string SleepString = ResourceService.LoafResource.GetString("Sleep");
        private readonly string Windows10StyleString = ResourceService.LoafResource.GetString("Windows10Style");
        private readonly string Windows11StyleString = ResourceService.LoafResource.GetString("Windows11Style");
        private bool isInitialized;
        private bool isLoadWallpaperFailed;

        private bool _loadImageCompleted;

        public bool LoadImageCompleted
        {
            get { return _loadImageCompleted; }

            set
            {
                if (!Equals(_loadImageCompleted, value))
                {
                    _loadImageCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadImageCompleted)));
                }
            }
        }

        private BitmapImage _loafImage;

        public BitmapImage LoafImage
        {
            get { return _loafImage; }

            set
            {
                if (!Equals(_loafImage, value))
                {
                    _loafImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoafImage)));
                }
            }
        }

        private bool _isLoafing;

        public bool IsLoafing
        {
            get { return _isLoafing; }

            set
            {
                if (!Equals(_isLoafing, value))
                {
                    _isLoafing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoafing)));
                }
            }
        }

        private bool _blockAllKeys = true;

        public bool BlockAllKeys
        {
            get { return _blockAllKeys; }

            set
            {
                if (!Equals(_blockAllKeys, value))
                {
                    _blockAllKeys = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockAllKeys)));
                }
            }
        }

        private ComboBoxItemModel _selectedSimulateUpdateStyle;

        public ComboBoxItemModel SelectedSimulateUpdateStyle
        {
            get { return _selectedSimulateUpdateStyle; }

            set
            {
                if (!Equals(_selectedSimulateUpdateStyle, value))
                {
                    _selectedSimulateUpdateStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSimulateUpdateStyle)));
                }
            }
        }

        private TimeSpan _durationTime = new(0, 30, 0);

        public TimeSpan DurationTime
        {
            get { return _durationTime; }

            set
            {
                if (!Equals(_durationTime, value))
                {
                    _durationTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DurationTime)));
                }
            }
        }

        private ComboBoxItemModel _selectedAfterSimulatedOperation;

        public ComboBoxItemModel SelectedAfterSimulateOperation
        {
            get { return _selectedAfterSimulatedOperation; }

            set
            {
                if (!Equals(_selectedAfterSimulatedOperation, value))
                {
                    _selectedAfterSimulatedOperation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAfterSimulateOperation)));
                }
            }
        }

        private List<ComboBoxItemModel> SimulateUpdateStyleList { get; } = [];

        private List<ComboBoxItemModel> AfterSimulateOperationList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public LoafPage()
        {
            InitializeComponent();
            SimulateUpdateStyleList.Add(new ComboBoxItemModel() { SelectedValue = SimulateUpdateKind.Windows11, DisplayMember = Windows11StyleString });
            SimulateUpdateStyleList.Add(new ComboBoxItemModel() { SelectedValue = SimulateUpdateKind.Windows10, DisplayMember = Windows10StyleString });
            AfterSimulateOperationList.Add(new ComboBoxItemModel() { SelectedValue = "None", DisplayMember = NoneString });
            AfterSimulateOperationList.Add(new ComboBoxItemModel() { SelectedValue = "LockScreen", DisplayMember = LockScreenString });
            AfterSimulateOperationList.Add(new ComboBoxItemModel() { SelectedValue = "LogOff", DisplayMember = LogOffString });
            AfterSimulateOperationList.Add(new ComboBoxItemModel() { SelectedValue = "Sleep", DisplayMember = SleepString });
            AfterSimulateOperationList.Add(new ComboBoxItemModel() { SelectedValue = "Restart", DisplayMember = RestartString });
            AfterSimulateOperationList.Add(new ComboBoxItemModel() { SelectedValue = "Shutdown", DisplayMember = ShutdownString });
            SelectedSimulateUpdateStyle = SimulateUpdateStyleList[0];
            SelectedAfterSimulateOperation = AfterSimulateOperationList[0];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                MemoryStream memoryStream = new();

                await Task.Run(async () =>
                {
                    try
                    {
                        HttpClient httpClient = new()
                        {
                            Timeout = TimeSpan.FromSeconds(5)
                        };

                        HttpResponseMessage responseMessage = await httpClient.GetAsync("https://bing.biturl.top/?resolution=1920&format=image");

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                            await stream.CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                        }

                        responseMessage.Dispose();
                        httpClient.Dispose();
                    }
                    catch (Exception e)
                    {
                        isLoadWallpaperFailed = true;
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoafPage), nameof(OnNavigatedTo), 1, e);
                    }
                });

                try
                {
                    BitmapImage bitmapImage = new();
                    await bitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
                    LoafImage = bitmapImage;
                    LoadImageCompleted = true;
                    memoryStream.Dispose();
                }
                catch (Exception e)
                {
                    isLoadWallpaperFailed = true;
                    LoafImage = ActualTheme is ElementTheme.Light ? new(new Uri("ms-appx:///Assets/Images/LoafLightWallpaper.jpg")) : new(new Uri("ms-appx:///Assets/Images/LoafDarkWallpaper.jpg"));
                    LoadImageCompleted = true;
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoafPage), nameof(OnNavigatedTo), 2, e);
                }
            }

            ActualThemeChanged += OnActualThemeChanged;
        }

        /// <summary>
        /// 离开该页面时触发的事件
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            try
            {
                ActualThemeChanged -= OnActualThemeChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(LoafPage), nameof(OnNavigatedFrom), 1, e);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：摸鱼页面——挂载的事件

        /// <summary>
        /// 当前应用主题发生变化时对应的事件
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (isLoadWallpaperFailed)
            {
                LoafImage = ActualTheme is ElementTheme.Light ? new(new Uri("ms-appx:///Assets/Images/LoafLightWallpaper.jpg")) : new(new Uri("ms-appx:///Assets/Images/LoafDarkWallpaper.jpg"));
            }
        }

        /// <summary>
        /// 开始摸鱼
        /// </summary>
        private void OnStartLoafClicked(object sender, RoutedEventArgs args)
        {
            new SimulateUpdateWindow((SimulateUpdateKind)SelectedSimulateUpdateStyle.SelectedValue, DurationTime, BlockAllKeys, Convert.ToString(SelectedAfterSimulateOperation.SelectedValue)).Activate();
            SimulateUpdateWindow.Current.Closed += OnClosed;
            IsLoafing = true;
        }

        /// <summary>
        /// 停止模拟更新后触发的事件
        /// </summary>
        private void OnClosed(object sender, WindowEventArgs args)
        {
            SimulateUpdateWindow.Current?.Closed -= OnClosed;
            IsLoafing = false;
        }

        /// <summary>
        /// 选择模拟更新的界面风格
        /// </summary>
        private void OnUpdateStyleSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel simulateUpdateStyle && !Equals(SelectedSimulateUpdateStyle, simulateUpdateStyle))
            {
                SelectedSimulateUpdateStyle = simulateUpdateStyle;
            }
        }

        /// <summary>
        /// 是否在模拟更新的时候屏蔽所有键盘按键
        /// </summary>
        private void OnBlockAllKeysToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                BlockAllKeys = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 时间更改时触发的事件
        /// </summary>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                DurationTime = args.NewTime;
            }
        }

        /// <summary>
        /// 模拟更新结束后是否自动锁屏
        /// </summary>
        private void OnAfterSimulateOperationSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel afterSimulateOperation && !Equals(SelectedAfterSimulateOperation, afterSimulateOperation))
            {
                SelectedAfterSimulateOperation = afterSimulateOperation;
            }
        }

        #endregion 第二部分：摸鱼页面——挂载的事件
    }
}
