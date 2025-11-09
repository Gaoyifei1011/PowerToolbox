using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Windows
{
    /// <summary>
    /// 模拟更新窗口
    /// </summary>
    public sealed partial class SimulateUpdateWindow : Window, INotifyPropertyChanged
    {
        private readonly string Windows10UpdateText1String = ResourceService.SimulateUpdateResource.GetString("Windows10UpdateText1");
        private readonly string Windows11UpdateText1String = ResourceService.SimulateUpdateResource.GetString("Windows11UpdateText1");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private System.Timers.Timer simulateUpdateTimer = new();
        private readonly int simulateTotalTime = 0;
        private int simulatePassedTime = 0;
        private readonly bool _blockAllKeys = false;
        private readonly bool _lockScreenAutomaticly = false;
        private nint hHook = 0;
        private HOOKPROC keyBoardHookProc;

        public new static SimulateUpdateWindow Current { get; private set; }

        private SimulateUpdateKind _simulateUpdateKind;

        public SimulateUpdateKind SimulateUpdateKind
        {
            get { return _simulateUpdateKind; }

            set
            {
                if (!Equals(_simulateUpdateKind, value))
                {
                    _simulateUpdateKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimulateUpdateKind)));
                }
            }
        }

        private string _windows11UpdateText;

        public string Windows11UpdateText
        {
            get { return _windows11UpdateText; }

            set
            {
                if (!Equals(_windows11UpdateText, value))
                {
                    _windows11UpdateText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Windows11UpdateText)));
                }
            }
        }

        private string _windows10UpdateText;

        public string Windows10UpdateText
        {
            get { return _windows10UpdateText; }

            set
            {
                if (!Equals(_windows10UpdateText, value))
                {
                    _windows10UpdateText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Windows10UpdateText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SimulateUpdateWindow(SimulateUpdateKind simulateUpdateKind, TimeSpan duration, bool blockAllKeys, bool lockScreenAutomaticly)
        {
            Current = this;
            InitializeComponent();

            AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            SimulateUpdateKind = simulateUpdateKind;
            simulateTotalTime = duration.TotalSeconds is not 0 ? Convert.ToInt32(duration.TotalSeconds) : 1;
            string percentage = ((double)simulatePassedTime / simulateTotalTime).ToString("0%");
            Windows11UpdateText = string.Format(Windows11UpdateText1String, percentage);
            Windows10UpdateText = string.Format(Windows10UpdateText1String, percentage);
            simulateUpdateTimer.Interval = 1000;
            simulateUpdateTimer.Elapsed += OnElapsed;
            simulateUpdateTimer.Start();
            _blockAllKeys = blockAllKeys;
            _lockScreenAutomaticly = lockScreenAutomaticly;
            (Content as WindowsAPI.ComTypes.IUIElementProtected).ProtectedCursor = InputDesktopResourceCursor.CreateFromModule("PowerToolbox.exe", 101);
            int exStyle = GetWindowLongAuto((nint)AppWindow.Id.Value, WindowLongIndexFlags.GWL_EXSTYLE);
            SetWindowLongAuto((nint)AppWindow.Id.Value, WindowLongIndexFlags.GWL_EXSTYLE, exStyle & ~0x00040000 | 0x00000080);
            SystemSleepHelper.PreventForCurrentThread();
            StartHook();
            Activate();
        }

        #region 第一部分：模拟更新窗口——挂载的事件

        /// <summary>
        /// 窗口关闭后触发的事件
        /// </summary>
        private async void OnClosed(object sender, WindowEventArgs args)
        {
            await Task.Delay(1000);
            Current = null;
        }

        #endregion 第一部分：模拟更新窗口——挂载的事件

        #region 第二部分：模拟更新窗口——自定义事件

        /// <summary>
        /// 当指定的计时器间隔已过去而且计时器处于启用状态时发生的事件
        /// </summary>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            simulatePassedTime++;

            try
            {
                // 到达约定时间，自动停止
                if (simulatePassedTime > simulateTotalTime)
                {
                    synchronizationContext.Post(_ =>
                    {
                        StopLoaf();
                    }, null);

                    return;
                }

                synchronizationContext.Post(_ =>
                {
                    string percentage = ((double)simulatePassedTime / simulateTotalTime).ToString("0%");
                    Windows11UpdateText = string.Format(ResourceService.SimulateUpdateResource.GetString("Windows11UpdateText1"), percentage);
                    Windows10UpdateText = string.Format(ResourceService.SimulateUpdateResource.GetString("Windows10UpdateText1"), percentage);
                }, null);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SimulateUpdateWindow), nameof(OnElapsed), 1, e);
            }
        }

        #endregion 第二部分：模拟更新窗口——自定义事件

        /// <summary>
        /// 添加钩子
        /// </summary>
        private void StartHook()
        {
            try
            {
                // 安装键盘钩子
                if (hHook is 0)
                {
                    keyBoardHookProc = new HOOKPROC(OnKeyboardHookProc);

                    hHook = User32Library.SetWindowsHookEx(HOOKTYPE.WH_KEYBOARD_LL, keyBoardHookProc, Process.GetCurrentProcess().MainModule.BaseAddress, 0);

                    //如果设置钩子失败.
                    if (hHook is 0)
                    {
                        StopHook();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SimulateUpdateWindow), nameof(StartHook), 1, e);
            }
        }

        /// <summary>
        /// 取消钩子
        /// </summary>
        private void StopHook()
        {
            try
            {
                bool unHookResult = true;
                if (hHook is not 0)
                {
                    unHookResult = User32Library.UnhookWindowsHookEx(hHook);
                }

                if (!unHookResult)
                {
                    throw new Win32Exception();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(SimulateUpdateWindow), nameof(StopHook), 1, e);
            }
        }

        /// <summary>
        /// 自定义钩子消息处理
        /// </summary>
        public nint OnKeyboardHookProc(int nCode, nuint wParam, nint lParam)
        {
            // 处理键盘钩子消息
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT kbdllHookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                // Esc 键，退出摸鱼
                if (kbdllHookStruct.vkCode is Keys.Escape)
                {
                    StopLoaf();
                    return 1;
                }

                // 屏蔽所有键盘按键
                if (_blockAllKeys)
                {
                    // 左 Windows 徽标键
                    if (kbdllHookStruct.vkCode is Keys.LWin)
                    {
                        return 1;
                    }

                    // 右 Windows 徽标键
                    if (kbdllHookStruct.vkCode is Keys.LWin)
                    {
                        return 1;
                    }

                    // Ctrl 和 Esc 组合
                    if (kbdllHookStruct.vkCode is Keys.Escape && ((User32Library.GetKeyState(Keys.LControlKey) & 0x8000) is not 0 || (User32Library.GetKeyState(Keys.RControlKey) & 0x8000) is not 0))
                    {
                        return 1;
                    }

                    // Alt 和 F4 组合
                    if (kbdllHookStruct.vkCode is Keys.F4 && ((User32Library.GetKeyState(Keys.Alt) & 0x8000) is not 0))
                    {
                        return 1;
                    }

                    // Alt 和 Tab 组合
                    if (kbdllHookStruct.vkCode is Keys.Tab && ((User32Library.GetKeyState(Keys.Alt) & 0x8000) is not 0))
                    {
                        return 1;
                    }

                    // Ctrl Shift Esc 组合
                    if (kbdllHookStruct.vkCode is Keys.Escape && ((User32Library.GetKeyState(Keys.LControlKey) & 0x8000) is not 0 || (User32Library.GetKeyState(Keys.RControlKey) & 0x8000) is not 0) && ((User32Library.GetKeyState(Keys.LShiftKey) & 0x8000) is not 0 || (User32Library.GetKeyState(Keys.RShiftKey) & 0x8000) is not 0))
                    {
                        return 1;
                    }

                    // Alt 和 Space 组合
                    if (kbdllHookStruct.vkCode is Keys.Space && ((User32Library.GetKeyState(Keys.Alt) & 0x8000) is not 0))
                    {
                        return 1;
                    }
                }
            }
            return User32Library.CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 停止摸鱼
        /// </summary>
        public void StopLoaf()
        {
            Cursor.Show();
            StopHook();
            StopSimulateUpdate();
            if (_lockScreenAutomaticly)
            {
                User32Library.LockWorkStation();
            }
            // 恢复此线程曾经阻止的系统休眠和屏幕关闭。
            SystemSleepHelper.RestoreForCurrentThread();
            Close();
            User32Library.keybd_event(Keys.LWin, 0, KEYEVENTFLAGS.KEYEVENTF_KEYDOWN, 0);
            User32Library.keybd_event(Keys.D, 0, KEYEVENTFLAGS.KEYEVENTF_KEYDOWN, 0);
            User32Library.keybd_event(Keys.D, 0, KEYEVENTFLAGS.KEYEVENTF_KEYUP, 0);
            User32Library.keybd_event(Keys.LWin, 0, KEYEVENTFLAGS.KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// 停止模拟自动更新
        /// </summary>
        public void StopSimulateUpdate()
        {
            if (simulateUpdateTimer is not null)
            {
                simulateUpdateTimer.Stop();
                simulateUpdateTimer.Dispose();
                simulatePassedTime = 0;
                simulateUpdateTimer = null;
            }
        }

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(nint hWnd, WindowLongIndexFlags nIndex)
        {
            return IntPtr.Size is 8 ? User32Library.GetWindowLongPtr(hWnd, nIndex) : User32Library.GetWindowLong(hWnd, nIndex);
        }

        /// <summary>
        /// 更改窗口属性
        /// </summary>
        private static nint SetWindowLongAuto(nint hWnd, WindowLongIndexFlags nIndex, nint dwNewLong)
        {
            return IntPtr.Size is 8 ? User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong) : User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
        }
    }
}
