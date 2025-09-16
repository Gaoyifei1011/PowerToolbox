using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.Backdrop;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.Pages;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Comctl32;
using PowerToolbox.WindowsAPI.PInvoke.Dwmapi;
using PowerToolbox.WindowsAPI.PInvoke.Gdi32;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using PowerToolbox.WindowsAPI.PInvoke.Uxtheme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

// 抑制 CA1806，CA1822 警告
#pragma warning disable CA1806,CA1822

namespace PowerToolbox.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public class MainWindow : Form
    {
        private readonly IntPtr hwndTitleBar;
        private readonly IntPtr hwndXamlIsland;
        private readonly IntPtr hwndMaximizeButton;
        private readonly int AUTO_HIDE_TASKBAR_HEIGHT = 2;
        private readonly int lightTintColor = ColorTranslator.ToWin32(Color.FromArgb(243, 243, 243));
        private readonly int darkTintColor = ColorTranslator.ToWin32(Color.FromArgb(32, 32, 32));
        private readonly Container container = new();
        private readonly WNDPROC TitleBarWndProc;
        private readonly SUBCLASSPROC windowClassProc;

        private readonly bool isDarkTheme;
        private bool trackingMouse = false;
        private int nativeTopBorderHeight = 1;
        private bool isDialogOpening = false;

        private DesktopWindowXamlSource desktopWindowXamlSource = new();

        public bool ExtendsContentIntoTitleBar { get; set; } = true;

        public UIElement Content { get; set; }

        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Current = this;
            Content = new MainPage();

            desktopWindowXamlSource.Content = Content;
            IDesktopWindowXamlSourceNative2 desktopWindowXamlSourceNative = desktopWindowXamlSource as IDesktopWindowXamlSourceNative2;
            desktopWindowXamlSourceNative.AttachToWindow(Handle);
            desktopWindowXamlSourceNative.GetWindowHandle(out hwndXamlIsland);
            desktopWindowXamlSource.TakeFocusRequested += OnTakeFocusRequested;

            Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            MinimumSize = new Size(Convert.ToInt32(1024 * ((double)DeviceDpi) / 96), Convert.ToInt32(768 * ((double)DeviceDpi / 96)));
            Size = new Size(Convert.ToInt32(1024 * ((double)DeviceDpi) / 96), Convert.ToInt32(768 * ((double)DeviceDpi / 96)));
            StartPosition = FormStartPosition.CenterScreen;
            Text = ResourceService.WindowResource.GetString("WindowTitle");

            if (RuntimeHelper.IsElevated)
            {
                Text += ResourceService.WindowResource.GetString("RunningAdministrator");
            }

            RightToLeft = LanguageService.RightToLeft;
            RightToLeftLayout = LanguageService.RightToLeft is RightToLeft.Yes;

            windowClassProc = new SUBCLASSPROC(OnWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Handle, windowClassProc, 0, IntPtr.Zero);

            // 注册标题栏窗口类
            TitleBarWndProc = new WNDPROC(OnTitleBarWndProc);
            WNDCLASSEX wcex = new()
            {
                cbSize = Marshal.SizeOf<WNDCLASSEX>(),
                style = WNDCLASS_STYLES.CS_DBLCLKS,
                hbrBackground = IntPtr.Zero,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = Process.GetCurrentProcess().Handle,
                hIcon = IntPtr.Zero,
                hCursor = Cursors.Arrow.Handle,
                lpszMenuName = null,
                lpszClassName = "DesktopWindowTitleBarArea",
                lpfnWndProc = TitleBarWndProc,
                hIconSm = IntPtr.Zero
            };
            User32Library.RegisterClassEx(ref wcex);

            // 隐藏原生标题栏上的图标
            WTA_OPTIONS wtaOptions = new()
            {
                dwFlags = WTNCA.WTNCA_NODRAWCAPTION | WTNCA.WTNCA_NODRAWICON | WTNCA.WTNCA_NOSYSMENU | WTNCA.WTNCA_NOMIRRORHELP,
                dwMask = 2 | 4
            };

            UxthemeLibrary.SetWindowThemeAttribute(Handle, WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT, ref wtaOptions, (uint)Marshal.SizeOf<WTA_OPTIONS>());

            // 1. 刷新窗口边框
            // 2. 防止窗口显示时背景闪烁: https://stackoverflow.com/questions/69715610/how-to-initialize-the-background-color-of-win32-app-to-something-other-than-whit
            User32Library.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_FRAMECHANGED | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOCOPYBITS);

            // 创建标题栏窗口，它是主窗口的子窗口。将它置于 XAML Islands 窗口之上以防止鼠标事件被吞掉
            // 出于未知的原因，必须添加 WS_EX_LAYERED 样式才能发挥作用，见
            // https://github.com/microsoft/terminal/blob/0ee2c74cd432eda153f3f3e77588164cde95044f/src/cascadia/WindowsTerminal/NonClientIslandWindow.cpp#L79
            // WS_EX_NOREDIRECTIONBITMAP 可以避免 WS_EX_LAYERED 导致的额外内存开销
            // WS_MINIMIZEBOX 和 WS_MAXIMIZEBOX 使得鼠标悬停时显示文字提示，Win11 的贴靠布局不依赖它们
            WindowExStyle windowExStyle = WindowExStyle.WS_EX_LAYERED | WindowExStyle.WS_EX_NOPARENTNOTIFY | WindowExStyle.WS_EX_NOREDIRECTIONBITMAP | WindowExStyle.WS_EX_NOACTIVATE;
            if (RightToLeft is RightToLeft.Yes && RightToLeftLayout)
            {
                windowExStyle |= WindowExStyle.WS_EX_LAYOUTRTL;
            }
            hwndTitleBar = User32Library.CreateWindowEx(windowExStyle, "DesktopWindowTitleBarArea", string.Empty, WindowStyle.WS_CHILD | WindowStyle.WS_MINIMIZEBOX | WindowStyle.WS_MAXIMIZEBOX, 0, 0, 0, 0, Handle, IntPtr.Zero, Process.GetCurrentProcess().Handle, IntPtr.Zero);
            User32Library.SetLayeredWindowAttributes(hwndTitleBar, 0, 255, LWA.LWA_ALPHA);

            if (Environment.Version.Build >= 22000)
            {
                // 如果鼠标正位于一个按钮上，贴靠布局弹窗会出现在按钮下方。我们利用这个特性来修正贴靠布局弹窗的位置
                // FIXME: 以管理员身份运行时这不起作用。Office 也有这个问题，所以可能没有解决方案
                windowExStyle = WindowExStyle.WS_EX_LEFT;
                if (RightToLeft is RightToLeft.Yes && RightToLeftLayout)
                {
                    windowExStyle |= WindowExStyle.WS_EX_LAYOUTRTL;
                }
                hwndMaximizeButton = User32Library.CreateWindowEx(windowExStyle, "Button", string.Empty, WindowStyle.WS_VISIBLE | WindowStyle.WS_CHILD | WindowStyle.WS_DISABLED | (WindowStyle)0x0000000B, 0, 0, 0, 0, hwndTitleBar, IntPtr.Zero, Process.GetCurrentProcess().Handle, IntPtr.Zero);
            }

            (Content as MainPage).IsWindowMinimizeEnabled = MinimizeBox;
            (Content as MainPage).IsWindowMaximizeEnabled = MaximizeBox;
            (Content as MainPage).IsWindowMaximized = User32Library.IsZoomed(Handle);
            (Content as MainPage).ChangeButtonEnabledState(CaptionButton.Minimize, MinimizeBox);
            (Content as MainPage).ChangeButtonEnabledState(CaptionButton.Maximize, MaximizeBox);
            (Content as MainPage).ChangeMaximizeButtonIcon(User32Library.IsZoomed(Handle));

            (Content as MainPage).AppTitlebar.SizeChanged += (s, e) =>
            {
                ResizeTitleBarWindow();
            };

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
                Shell32Library.DragAcceptFiles(Handle, true);
            }

            AlwaysShowBackdropService.PropertyChanged += OnServicePropertyChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;

            (Window.Current.CoreWindow as object as ICoreWindowInterop).GetWindowHandle(out IntPtr coreWindowhandle);
            User32Library.SetWindowPos(coreWindowhandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOOWNERZORDER);
        }

        #region 第一部分：窗口类内置需要重载的事件

        /// <summary>
        /// 处置由主窗体占用的资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && container is not null)
            {
                container.Dispose();
            }
        }

        /// <summary>
        /// 窗口激活时触发的事件
        /// </summary>
        protected override void OnActivated(EventArgs args)
        {
            base.OnActivated(args);

            if (desktopWindowXamlSource is not null)
            {
                (Content as MainPage).ChangeButtonActiveState(true);
                VisualStateManager.GoToState(Content as MainPage, "TitlebarTitleActivate", false);
            }
        }

        /// <summary>
        /// 窗口不处于焦点状态时触发的事件
        /// </summary>
        protected override void OnDeactivate(EventArgs args)
        {
            base.OnDeactivate(args);

            if (desktopWindowXamlSource is not null)
            {
                (Content as MainPage).ChangeButtonActiveState(false);
                VisualStateManager.GoToState(Content as MainPage, "TitlebarTitleDeactive", false);
            }
        }

        /// <summary>
        /// 窗口 DPI 发生变化时触发的事件
        /// </summary>
        protected override void OnDpiChanged(DpiChangedEventArgs args)
        {
            base.OnDpiChanged(args);
            MinimumSize = new Size(Convert.ToInt32(1024 * ((double)DeviceDpi) / 96), Convert.ToInt32(768 * ((double)DeviceDpi / 96)));
            Rectangle rectangle = args.SuggestedRectangle;
            UpdateFrameBorderThickness();
            User32Library.SetWindowPos(Handle, IntPtr.Zero, rectangle.Left, rectangle.Top, rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);
        }

        /// <summary>
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override async void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);

            if (Current is not null)
            {
                args.Cancel = true;

                int count = 0;
                DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();
                try
                {
                    count = DownloadSchedulerService.DownloadSchedulerList.Count;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(OnFormClosing), 1, e);
                }
                finally
                {
                    DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                }

                // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
                if (count > 0)
                {
                    Show();

                    // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                    ContentDialogResult result = await ShowDialogAsync(new ClosingWindowDialog());

                    if (result is ContentDialogResult.Primary)
                    {
                        if (RuntimeHelper.IsElevated)
                        {
                            User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_REMOVE);
                            User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                        }

                        desktopWindowXamlSource.TakeFocusRequested -= OnTakeFocusRequested;
                        ThemeService.PropertyChanged -= OnServicePropertyChanged;
                        BackdropService.PropertyChanged -= OnServicePropertyChanged;
                        TopMostService.PropertyChanged -= OnServicePropertyChanged;
                        Comctl32Library.RemoveWindowSubclass(Handle, windowClassProc, 0);

                        Current = null;
                        desktopWindowXamlSource.Dispose();
                        desktopWindowXamlSource = null;
                        (global::Windows.UI.Xaml.Application.Current as XamlIslandsApp).Dispose();
                    }
                    else if (result is ContentDialogResult.Secondary)
                    {
                        if (!Equals((Content as MainPage).GetCurrentPageType(), typeof(DownloadManagerPage)))
                        {
                            (Content as MainPage).NavigateTo(typeof(DownloadManagerPage));
                        }
                    }
                }
                else
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_REMOVE);
                        User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                    }

                    desktopWindowXamlSource.TakeFocusRequested -= OnTakeFocusRequested;
                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                    TopMostService.PropertyChanged -= OnServicePropertyChanged;
                    Comctl32Library.RemoveWindowSubclass(Handle, windowClassProc, 0);

                    Current = null;
                    desktopWindowXamlSource.Dispose();
                    desktopWindowXamlSource = null;
                    (global::Windows.UI.Xaml.Application.Current as XamlIslandsApp).Dispose();
                }
            }
        }

        /// <summary>
        /// 窗口创建时触发的事件
        /// </summary>
        protected override void OnHandleCreated(EventArgs args)
        {
            base.OnHandleCreated(args);
            UpdateFrameBorderThickness();

            if (Environment.Version.Build >= 22000)
            {
                // 初始化双缓冲绘图
                UxthemeLibrary.BufferedPaintInit();
                UpdateFrameMargins();
            }
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            SetWindowTheme();
            TopMost = TopMostService.TopMostValue;
            SetClassicMenuTheme();
        }

        /// <summary>
        /// 窗体移动时关闭浮出窗口
        /// </summary>
        protected override void OnMove(EventArgs args)
        {
            base.OnMove(args);
            if (Content is not null && Content.XamlRoot is not null)
            {
                // 窗口移动时，校对并纠正弹出窗口位置错误的问题
                foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
                {
                    ElementCompositeMode compositeMode = popup.CompositeMode;
                    popup.CompositeMode = compositeMode is ElementCompositeMode.SourceOver ? ElementCompositeMode.MinBlend : ElementCompositeMode.SourceOver;
                    popup.CompositeMode = compositeMode;
                }
            }
        }

        /// <summary>
        /// 窗口大小改变时发生的事件
        /// </summary>
        protected override void OnSizeChanged(EventArgs args)
        {
            base.OnSizeChanged(args);

            if (Content is not null && Content.XamlRoot is not null)
            {
                foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
                {
                    // 关闭内容对话框
                    if (popup.Child as ContentDialog is not null)
                    {
                        (popup.Child as ContentDialog).Hide();
                    }

                    // 关闭浮出控件
                    if (popup.Child is FlyoutPresenter || popup.Child is MenuFlyoutPresenter)
                    {
                        popup.IsOpen = false;
                    }
                }
            }

            // 修改 CoreWindow 窗口的大小
            (Window.Current.CoreWindow as object as ICoreWindowInterop).GetWindowHandle(out IntPtr coreWindowhandle);
            User32Library.SetWindowPos(coreWindowhandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOOWNERZORDER);
        }

        /// <summary>
        /// 窗口样式发生改变时触发的事件
        /// </summary>
        protected override void OnStyleChanged(EventArgs args)
        {
            base.OnStyleChanged(args);
            (Content as MainPage).IsWindowMinimizeEnabled = MinimizeBox;
            (Content as MainPage).IsWindowMaximizeEnabled = MaximizeBox;
            (Content as MainPage).IsWindowMaximized = User32Library.IsZoomed(Handle);
            (Content as MainPage).ChangeButtonEnabledState(CaptionButton.Minimize, MinimizeBox);
            (Content as MainPage).ChangeButtonEnabledState(CaptionButton.Maximize, MaximizeBox);
            (Content as MainPage).ChangeMaximizeButtonIcon(User32Library.IsZoomed(Handle));
        }

        #endregion 第一部分：窗口类内置需要重载的事件

        #region 第二部分：自定义事件

        /// <summary>
        /// 当主机桌面应用程序收到从 DesktopWindowXamlSource 对象 (获取焦点的请求时发生，用户位于 DesktopWindowXamlSource 中的最后一个可聚焦元素上，然后按 Tab) 。
        /// </summary>
        private void OnTakeFocusRequested(DesktopWindowXamlSource sender, DesktopWindowXamlSourceTakeFocusRequestedEventArgs args)
        {
            XamlSourceFocusNavigationReason reason = args.Request.Reason;

            if (reason < XamlSourceFocusNavigationReason.Left)
            {
                sender.NavigateFocus(args.Request);
            }
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            BeginInvoke(() =>
            {
                if (string.Equals(args.PropertyName, nameof(AlwaysShowBackdropService.AlwaysShowBackdropValue)))
                {
                    SetWindowBackdrop();
                }
                if (string.Equals(args.PropertyName, nameof(ThemeService.AppTheme)))
                {
                    SetWindowTheme();
                    SetClassicMenuTheme();
                }
                if (string.Equals(args.PropertyName, nameof(BackdropService.AppBackdrop)))
                {
                    SetWindowBackdrop();
                }
                if (string.Equals(args.PropertyName, nameof(TopMostService.TopMostValue)))
                {
                    TopMost = TopMostService.TopMostValue;
                }
            });
        }

        #endregion 第二部分：自定义事件

        #region 第三部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr OnWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 窗口大小发生变化时对应的消息
                case WindowMessage.WM_SIZE:
                    {
                        if (wParam.ToUInt32() is not 1)
                        {
                            int width = (int)LOWORD((uint)lParam);
                            int height = (int)HIWORD((uint)lParam);

                            if (!User32Library.IsWindowVisible(Handle) && User32Library.IsZoomed(Handle))
                            {
                                // 初始化过程中此函数会被调用两次。如果窗口以最大化显示，则两次传入的尺寸不一致。第一次调用此函数时主窗口尚未显示，因此无法最大化，我们必须估算最大化窗口的尺寸。不执行这个操作可能导致窗口显示时展示 NavigationView 导航展开的动画。
                                Screen screen = Screen.FromHandle(Handle);

                                if (screen is not null)
                                {
                                    // 最大化窗口的尺寸为当前屏幕工作区的尺寸
                                    width = screen.WorkingArea.Right - screen.Bounds.Left;
                                    height = screen.WorkingArea.Bottom - screen.Bounds.Top;
                                }
                            }

                            (Content as MainPage).IsWindowMaximized = User32Library.IsZoomed(Handle);
                            (Content as MainPage).ChangeMaximizeButtonIcon(User32Library.IsZoomed(Handle));

                            // Win10 中上边框被涂黑来显示系统原始边框，Win11 中 DWM 绘制的上边框也位于客户区内，
                            // 很可能是为了和 Win10 兼容。XAML Islands 不应该和上边框重叠。
                            int topBorderHeight = GetTopBorderHeight();

                            // SWP_NOZORDER 确保 XAML Islands 窗口始终在标题栏窗口下方，否则主窗口在调整大小时会闪烁
                            User32Library.SetWindowPos(hwndXamlIsland, IntPtr.Zero, 0, topBorderHeight, width, height - topBorderHeight, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
                        }
                        ResizeTitleBarWindow();
                        return IntPtr.Zero;
                    }
                // 重绘窗口时对应的窗口消息
                case WindowMessage.WM_PAINT:
                    {
                        if (Environment.Version.Build < 22621)
                        {
                            IntPtr hdc = User32Library.BeginPaint(Handle, out PAINTSTRUCT ps);
                            if (hdc.Equals(IntPtr.Zero))
                            {
                                return IntPtr.Zero;
                            }

                            int topBorderHeight = Environment.Version.Build >= 22000 ? 0 : GetTopBorderHeight();

                            // Win10 中在顶部绘制黑色实线以显示系统原始边框，见 UpdateFrameMargins
                            if (ps.rcPaint.top < topBorderHeight)
                            {
                                RECT rcTopBorder = ps.rcPaint;
                                rcTopBorder.bottom = topBorderHeight;

                                IntPtr hBrush = Gdi32Library.GetStockObject(StockObject.BLACK_BRUSH);
                                User32Library.FillRect(hdc, rcTopBorder, hBrush);
                            }

                            // 绘制客户区，它会在调整窗口尺寸时短暂可见
                            if (ps.rcPaint.bottom > topBorderHeight)
                            {
                                RECT rcRest = ps.rcPaint;
                                rcRest.top = topBorderHeight;

                                if (Content is not null)
                                {
                                    bool isDarkBrush = (Content as FrameworkElement).ActualTheme is ElementTheme.Dark;
                                    IntPtr backgroundBrush = Gdi32Library.CreateSolidBrush(isDarkBrush ? darkTintColor : lightTintColor);

                                    if (!Equals(isDarkBrush, isDarkTheme))
                                    {
                                        isDarkBrush = isDarkTheme;
                                        Gdi32Library.DeleteObject(backgroundBrush);
                                        backgroundBrush = Gdi32Library.CreateSolidBrush(isDarkBrush ? darkTintColor : lightTintColor);
                                    }

                                    if (isDarkBrush && Environment.Version.Build < 22000)
                                    {
                                        // 这里我们想要黑色背景而不是原始边框
                                        // 来自 https://github.com/microsoft/terminal/blob/0ee2c74cd432eda153f3f3e77588164cde95044f/src/cascadia/WindowsTerminal/NonClientIslandWindow.cpp#L1030-L1047
                                        BP_PAINTPARAMS bp_PaintParams = new()
                                        {
                                            cbSize = (uint)Marshal.SizeOf<BP_PAINTPARAMS>(),
                                            dwFlags = BPPF.BPPF_NOCLIP | BPPF.BPPF_ERASE
                                        };
                                        IntPtr buf = UxthemeLibrary.BeginBufferedPaint(hdc, ref rcRest, BP_BUFFERFORMAT.BPBF_TOPDOWNDIB, ref bp_PaintParams, out IntPtr opaqueDc);
                                        if (!Equals(buf, IntPtr.Zero) && !Equals(opaqueDc, IntPtr.Zero))
                                        {
                                            User32Library.FillRect(opaqueDc, rcRest, backgroundBrush);
                                            RECT rect = new();
                                            UxthemeLibrary.BufferedPaintSetAlpha(buf, ref rect, 255);
                                            UxthemeLibrary.EndBufferedPaint(buf, true);
                                        }
                                    }
                                    else
                                    {
                                        User32Library.FillRect(hdc, rcRest, backgroundBrush);
                                    }
                                }
                            }
                            User32Library.EndPaint(Handle, ref ps);
                            return IntPtr.Zero;
                        }

                        break;
                    }
                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        SetWindowTheme();
                        SetClassicMenuTheme();

                        if (Content is MainPage mainPage)
                        {
                            BeginInvoke(() =>
                            {
                                SetPopupControlTheme(mainPage.WindowTheme);
                            });

                            if (mainPage.GetFrameContent() is ThemeSwitchPage themeSwitchPage)
                            {
                                BeginInvoke(async () =>
                                {
                                    await themeSwitchPage.InitializeSystemThemeSettingsAsync();
                                });
                            }
                        }
                        break;
                    }
                // 处理非客户区左键按下的窗口消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            if ((Content as MainPage).TitlebarMenuFlyout.IsOpen)
                            {
                                (Content as MainPage).TitlebarMenuFlyout.Hide();
                            }
                        }
                        break;
                    }
                // 处理客户区右键按下后释放的窗口消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (ExtendsContentIntoTitleBar && wParam.ToUInt32() is (int)HITTEST.HTCAPTION)
                        {
                            // 显示自定义标题栏右键菜单
                            if (wParam.ToUInt32() is 2 && Content is not null && Content.XamlRoot is not null)
                            {
                                Point clientPoint = PointToClient(Cursor.Position);
                                FlyoutShowOptions options = new()
                                {
                                    Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                                    ShowMode = FlyoutShowMode.Standard,
                                    Position = RightToLeft is RightToLeft.Yes
                                        ? InfoHelper.SystemVersion.Build >= 22000
                                            ? new global::Windows.Foundation.Point((ClientSize.Width - clientPoint.X) / ((double)DeviceDpi / 96), clientPoint.Y / ((double)DeviceDpi / 96))
                                            : new global::Windows.Foundation.Point(ClientSize.Width - clientPoint.X, clientPoint.Y)
                                        : InfoHelper.SystemVersion.Build >= 22000
                                            ? new global::Windows.Foundation.Point(clientPoint.X / ((double)DeviceDpi / 96), clientPoint.Y / ((double)DeviceDpi / 96))
                                            : new global::Windows.Foundation.Point(clientPoint.X, clientPoint.Y)
                                };

                                (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                            }

                            // 我们自己处理标题栏右键，不知为何 DefWindowProc 没有作用
                            // 在标题栏上按下右键，在其他地方释放也会收到此消息。确保只有在标题栏上释放时才显示菜单
                            User32Library.GetWindowRect(hwndTitleBar, out RECT titleBarRect);
                            if (!User32Library.PtInRect(ref titleBarRect, Cursor.Position))
                            {
                                break;
                            }

                            return IntPtr.Zero;
                        }

                        break;
                    }
                // 重新计算窗口大小时对应的窗口消息
                case WindowMessage.WM_NCCALCSIZE:
                    {
                        if (ExtendsContentIntoTitleBar)
                        {
                            // 移除标题栏的逻辑基本来自 Windows Terminal
                            // https://github.com/microsoft/terminal/blob/0ee2c74cd432eda153f3f3e77588164cde95044f/src/cascadia/WindowsTerminal/NonClientIslandWindow.cpp
                            if (wParam.Equals(UIntPtr.Zero))
                            {
                                return IntPtr.Zero;
                            }

                            // 保存原始上边框位置
                            int originalTop = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam).rgrc[0].top;

                            // 应用默认边框
                            IntPtr result = Comctl32Library.DefSubclassProc(Handle, WindowMessage.WM_NCCALCSIZE, wParam, lParam);

                            if (!Equals(result, IntPtr.Zero))
                            {
                                return result;
                            }

                            // 重新应用原始上边框，因此我们完全移除了默认边框中的上边框和标题栏，但保留了其他方向的边框
                            NCCALCSIZE_PARAMS nccalcsize_params = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);
                            RECT clientRect = nccalcsize_params.rgrc[0];
                            clientRect.top = originalTop;

                            // WM_NCCALCSIZE 在 WM_SIZE 前
                            if (User32Library.IsZoomed(Handle))
                            {
                                // 最大化的窗口的实际尺寸比屏幕的工作区更大一点，这是为了将可调整窗口大小的区域隐藏在屏幕外面
                                clientRect.top += GetResizeHandleHeight();

                                // 如果有自动隐藏的任务栏，我们在它的方向稍微减小客户区，这样用户就可以用鼠标呼出任务栏
                                Screen screen = Screen.FromHandle(Handle);

                                // 检查是否有自动隐藏的任务栏
                                APPBARDATA appbarData = new()
                                {
                                    cbSize = Marshal.SizeOf<APPBARDATA>()
                                };

                                IntPtr appbarResult = Shell32Library.SHAppBarMessage(ABM.ABM_GETSTATE, ref appbarData);

                                if (!Equals(appbarResult, IntPtr.Zero))
                                {
                                    if (HasAutoHideTaskbar(screen, ABE.Top))
                                    {
                                        clientRect.top += AUTO_HIDE_TASKBAR_HEIGHT;
                                    }

                                    if (HasAutoHideTaskbar(screen, ABE.Bottom))
                                    {
                                        clientRect.bottom -= AUTO_HIDE_TASKBAR_HEIGHT;
                                    }

                                    if (HasAutoHideTaskbar(screen, ABE.Left))
                                    {
                                        clientRect.left += AUTO_HIDE_TASKBAR_HEIGHT;
                                    }

                                    if (HasAutoHideTaskbar(screen, ABE.Right))
                                    {
                                        clientRect.right -= AUTO_HIDE_TASKBAR_HEIGHT;
                                    }
                                }
                            }

                            nccalcsize_params.rgrc[0] = clientRect;
                            Marshal.StructureToPtr(nccalcsize_params, lParam, true);

                            // 如果在 WM_SIZE 中处理会导致窗口闪烁
                            UpdateFrameMargins();
                            return IntPtr.Zero;
                        }
                        else
                        {
                            UpdateFrameMargins();
                            break;
                        }
                    }
                // 确定窗口坐标位置对应的消息
                case WindowMessage.WM_NCHITTEST:
                    {
                        if (ExtendsContentIntoTitleBar)
                        {
                            // 让 OS 处理左右下三边，由于我们移除了标题栏，上边框会被视为客户区
                            IntPtr result = Comctl32Library.DefSubclassProc(Handle, WindowMessage.WM_NCHITTEST, UIntPtr.Zero, lParam);

                            if (!Equals(result, (IntPtr)HITTEST.HTCLIENT))
                            {
                                return result;
                            }

                            // XAML Islands 和它上面的标题栏窗口都会吞掉鼠标事件，因此能到达这里的唯一机会是上边框。保险起见做一些额外检查。
                            if (!User32Library.IsZoomed(Handle))
                            {
                                User32Library.GetWindowRect(Handle, out RECT rcWindow);
                                if (Cursor.Position.Y < rcWindow.top + GetResizeHandleHeight())
                                {
                                    return (IntPtr)HITTEST.HTTOP;
                                }
                            }

                            return (IntPtr)HITTEST.HTCAPTION;
                        }

                        break;
                    }
                // 窗口键盘按键对应的窗口消息
                case WindowMessage.WM_KEYDOWN:
                    {
                        if (wParam.ToUInt32() is (int)Keys.Tab)
                        {
                            // 处理焦点
                            if (desktopWindowXamlSource is not null)
                            {
                                XamlSourceFocusNavigationReason reason = (User32Library.GetKeyState(Keys.ShiftKey) & (int)Keys.F17) is not 0 ?
                        XamlSourceFocusNavigationReason.Last : XamlSourceFocusNavigationReason.First;
                                desktopWindowXamlSource.NavigateFocus(new XamlSourceFocusNavigationRequest(reason));
                            }

                            return IntPtr.Zero;
                        }
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam.ToUInt32() & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new global::Windows.Foundation.Point(0, 15),
                                ShowMode = FlyoutShowMode.Standard
                            };
                            (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                            return IntPtr.Zero;
                        }
                        else if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            // 禁用按 Alt 键会激活窗口菜单的行为，它使用户界面无法交互
                            if (lParam.Equals(IntPtr.Zero))
                            {
                                return IntPtr.Zero;
                            }

                            if (lParam.ToInt32() is (int)Keys.Space)
                            {
                                FlyoutShowOptions options = new()
                                {
                                    Position = new global::Windows.Foundation.Point(0, 45),
                                    ShowMode = FlyoutShowMode.Standard
                                };
                                (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                            }

                            return IntPtr.Zero;
                        }

                        break;
                    }
                // 当菜单处于活动状态并且用户按下与任何助记键或快捷键不对应的键时发送的消息
                case WindowMessage.WM_MENUCHAR:
                    {
                        // 防止按 Alt+Key 时发出铃声
                        return new IntPtr((0 & 0xffff) | ((1 & 0xffff) << 16));
                    }
                // 提升权限时允许应用接收拖放消息
                case WindowMessage.WM_DROPFILES:
                    {
                        Task.Run(() =>
                        {
                            List<string> filesList = [];
                            char[] dragFileCharArray = new char[260];
                            uint filesCount = Shell32Library.DragQueryFile(wParam, 0xffffffffu, null, 0);

                            for (uint index = 0; index < filesCount; index++)
                            {
                                Array.Clear(dragFileCharArray, 0, dragFileCharArray.Length);
                                if (Shell32Library.DragQueryFile(wParam, index, dragFileCharArray, (uint)dragFileCharArray.Length) > 0)
                                {
                                    filesList.Add(new string(dragFileCharArray).Replace("\0", string.Empty));
                                }
                            }

                            Shell32Library.DragQueryPoint(wParam, out Point point);
                            Shell32Library.DragFinish(wParam);

                            BeginInvoke(async () =>
                            {
                                await (Content as MainPage).SendReceivedFilesListAsync(filesList);
                            });
                        });

                        break;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr OnTitleBarWndProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 绘制按钮时触发的消息
                case WindowMessage.WM_CTLCOLORBTN:
                    {
                        // 使原生按钮控件透明，虽然整个标题栏窗口都是不可见的
                        return IntPtr.Zero;
                    }
                // 确定窗口坐标位置对应的消息
                case WindowMessage.WM_NCHITTEST:
                    {
                        Point cursorPos = new((int)LOWORD((uint)lParam.ToInt32()), (int)HIWORD((uint)lParam.ToInt32()));
                        User32Library.MapWindowPoints(IntPtr.Zero, hwndTitleBar, ref cursorPos, 2);
                        User32Library.GetClientRect(hwndTitleBar, out RECT titleBarClientRect);
                        if (!User32Library.PtInRect(ref titleBarClientRect, cursorPos))
                        {
                            // 先检查鼠标是否在窗口内。在标题栏按钮上按下鼠标时我们会捕获光标，从而收到 WM_MOUSEMOVE 和 WM_LBUTTONUP 消息。
                            // 它们使用 WM_NCHITTEST 测试鼠标位于哪个区域
                            return new IntPtr((int)HITTEST.HTNOWHERE);
                        }

                        if (!User32Library.IsZoomed(Handle) && cursorPos.Y + GetTopBorderHeight() < GetResizeHandleHeight())
                        {
                            // 鼠标位于上边框
                            return new IntPtr((int)HITTEST.HTTOP);
                        }

                        // 标题栏按钮的宽度和高度
                        Size buttonSizeInDips = new(46, 32);
                        double dpiScale = (double)DeviceDpi / 96;
                        double buttonWidthInPixels = buttonSizeInDips.Width * dpiScale;
                        double buttonHeightInPixels = buttonSizeInDips.Height * dpiScale;

                        if (cursorPos.Y >= buttonHeightInPixels)
                        {
                            // 鼠标位于标题按钮下方，如果标题栏很宽，这里也可以拖动
                            return new IntPtr((int)HITTEST.HTCAPTION);
                        }

                        // 从右向左检查鼠标是否位于某个标题栏按钮上
                        long cursorToRight = titleBarClientRect.right - cursorPos.X;
                        if (cursorToRight < buttonWidthInPixels)
                        {
                            return new IntPtr((int)HITTEST.HTCLOSE);
                        }
                        else if (cursorToRight < buttonWidthInPixels * 2)
                        {
                            // 支持 Win11 的贴靠布局
                            return MaximizeBox ? new IntPtr((int)HITTEST.HTMAXBUTTON) : new IntPtr((int)HITTEST.HTCLIENT);
                        }
                        else if (cursorToRight < buttonWidthInPixels * 3)
                        {
                            return MinimizeBox ? new IntPtr((int)HITTEST.HTMINBUTTON) : new IntPtr((int)HITTEST.HTCLIENT);
                        }
                        else
                        {
                            // 不在任何标题栏按钮上则在可拖拽区域
                            return new IntPtr((int)HITTEST.HTCAPTION);
                        }
                    }
                // 窗口鼠标在工作区移动时收到的窗口消息
                case WindowMessage.WM_MOUSEMOVE:
                    {
                        Point cursorPos = new((int)LOWORD((uint)lParam.ToInt32()), (int)HIWORD((uint)lParam.ToInt32()));
                        User32Library.MapWindowPoints(hwndTitleBar, IntPtr.Zero, ref cursorPos, 2);
                        wParam = (UIntPtr)(uint)User32Library.SendMessage(hwndTitleBar, WindowMessage.WM_NCHITTEST, UIntPtr.Zero, new IntPtr(MakeLParam(cursorPos.X, cursorPos.Y)));
                        goto case WindowMessage.WM_NCMOUSEMOVE;
                    }
                // 窗口鼠标在非工作区移动时收到的窗口消息
                case WindowMessage.WM_NCMOUSEMOVE:
                    {
                        if (Content is MainPage mainPage)
                        {
                            // 将 hover 状态通知 CaptionButtons。标题栏窗口拦截了 XAML Islands 中的标题栏
                            // 控件的鼠标消息，标题栏按钮的状态由我们手动控制。
                            if (wParam.Equals((UIntPtr)(uint)HITTEST.HTTOP) || wParam.Equals((UIntPtr)(uint)HITTEST.HTCAPTION))
                            {
                                mainPage.LeaveButtons();

                                // 将 HTTOP 传给主窗口才能通过上边框调整窗口高度
                                return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                            }
                            else if (wParam.Equals((UIntPtr)(uint)HITTEST.HTMINBUTTON) || wParam.Equals((UIntPtr)(uint)HITTEST.HTMAXBUTTON) || wParam.Equals((UIntPtr)(uint)HITTEST.HTCLOSE))
                            {
                                if (wParam.ToUInt32() is (uint)HITTEST.HTMINBUTTON)
                                {
                                    mainPage.HoverButton(CaptionButton.Minimize);
                                }
                                else if (wParam.ToUInt32() is (uint)HITTEST.HTMAXBUTTON)
                                {
                                    mainPage.HoverButton(CaptionButton.Maximize);
                                }
                                else if (wParam.ToUInt32() is (uint)HITTEST.HTCLOSE)
                                {
                                    mainPage.HoverButton(CaptionButton.Close);
                                }

                                // 追踪鼠标以确保鼠标离开标题栏时我们能收到 WM_NCMOUSELEAVE 消息，否则无法
                                // 可靠的收到这个消息，尤其是在用户快速移动鼠标的时候。
                                if (!trackingMouse && Msg is WindowMessage.WM_NCMOUSEMOVE)
                                {
                                    TRACKMOUSEEVENT ev = new()
                                    {
                                        cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>(),
                                        dwFlags = TRACKMOUSEEVENT_FLAGS.TME_LEAVE | TRACKMOUSEEVENT_FLAGS.TME_NONCLIENT,
                                        hwndTrack = hwndTitleBar,
                                        dwHoverTime = 0xFFFFFFFF
                                    };
                                    User32Library.TrackMouseEvent(ev);
                                    trackingMouse = true;
                                }
                            }
                            else
                            {
                                mainPage.LeaveButtons();
                            }
                        }

                        break;
                    }
                // 光标离开之前对 TrackMouseEvent 的调用中指定的窗口的非工作区域时，发布到窗口的消息
                case WindowMessage.WM_NCMOUSELEAVE:
                    {
                        // 我们需要检查鼠标是否真的离开了标题栏按钮，因为在某些情况下 OS 会错误汇报。
                        // 比如：鼠标在关闭按钮上停留了一段时间，系统会显示文字提示，这时按下左键，便会收
                        // 到 WM_NCMOUSELEAVE，但此时鼠标并没有离开标题栏按钮
                        Point cursorPos = new((int)LOWORD((uint)lParam.ToInt32()), (int)HIWORD((uint)lParam.ToInt32()));
                        // 先检查鼠标是否在主窗口上，如果正在显示文字提示，会返回 _hwndTitleBar
                        IntPtr hwndUnderCursor = User32Library.WindowFromPoint(cursorPos);
                        if (!Equals(hwndUnderCursor, Handle) && !Equals(hwndUnderCursor, hwndTitleBar))
                        {
                            (Content as MainPage).LeaveButtons();
                        }
                        else
                        {
                            // 然后检查鼠标在标题栏上的位置
                            IntPtr hit = User32Library.SendMessage(hwndTitleBar, WindowMessage.WM_NCHITTEST, UIntPtr.Zero, new IntPtr(MakeLParam(cursorPos.X, cursorPos.Y)));
                            if (!Equals(hit, new IntPtr((int)HITTEST.HTMINBUTTON)) && !Equals(hit, new IntPtr((int)HITTEST.HTMAXBUTTON)) && !Equals(hit, new IntPtr((int)HITTEST.HTCLOSE)))
                            {
                                (Content as MainPage).LeaveButtons();
                            }
                        }

                        trackingMouse = false;
                        break;
                    }
                // 光标离开之前对 TrackMouseEvent 的调用中指定的窗口的工作区域时，发布到窗口的消息
                case WindowMessage.WM_MOUSELEAVE:
                    {
                        // 我们需要检查鼠标是否真的离开了标题栏按钮，因为在某些情况下 OS 会错误汇报。
                        // 比如：鼠标在关闭按钮上停留了一段时间，系统会显示文字提示，这时按下左键，便会收到 WM_NCMOUSELEAVE，但此时鼠标并没有离开标题栏按钮
                        Point cursorPos = new((int)LOWORD((uint)lParam.ToInt32()), (int)HIWORD((uint)lParam.ToInt32()));
                        // 先检查鼠标是否在主窗口上，如果正在显示文字提示，会返回 _hwndTitleBar
                        IntPtr hwndUnderCursor = User32Library.WindowFromPoint(cursorPos);
                        if (!Equals(hwndUnderCursor, Handle) && !Equals(hwndUnderCursor, hwndTitleBar))
                        {
                            (Content as MainPage).LeaveButtons();
                        }
                        else
                        {
                            // 然后检查鼠标在标题栏上的位置
                            IntPtr hit = User32Library.SendMessage(hwndTitleBar, WindowMessage.WM_NCHITTEST, UIntPtr.Zero, new IntPtr(MakeLParam(cursorPos.X, cursorPos.Y)));
                            if (!Equals(hit, new IntPtr((int)HITTEST.HTMINBUTTON)) && !Equals(hit, new IntPtr((int)HITTEST.HTMAXBUTTON)) && !Equals(hit, new IntPtr((int)HITTEST.HTCLOSE)))
                            {
                                (Content as MainPage).LeaveButtons();
                            }
                        }

                        trackingMouse = false;
                        break;
                    }
                // 处理非客户区左键按下的窗口消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        // 手动处理标题栏上的点击。如果在标题栏按钮上，则通知 CaptionButtons，否则将消息传递
                        // 给主窗口。
                        if (wParam.Equals(new UIntPtr((int)HITTEST.HTTOP)) || wParam.Equals(new UIntPtr((int)HITTEST.HTCAPTION)))
                        {
                            // 将 HTTOP 传给主窗口才能通过上边框调整窗口高度
                            return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                        }
                        else if (wParam.Equals(new UIntPtr((int)HITTEST.HTMINBUTTON)) || wParam.Equals(new UIntPtr((int)HITTEST.HTMAXBUTTON)) || wParam.Equals(new UIntPtr((int)HITTEST.HTCLOSE)))
                        {
                            if (wParam.ToUInt32() is (uint)HITTEST.HTMINBUTTON)
                            {
                                (Content as MainPage).PressButton(CaptionButton.Minimize);
                            }
                            else if (wParam.ToUInt32() is (uint)HITTEST.HTMAXBUTTON)
                            {
                                (Content as MainPage).PressButton(CaptionButton.Maximize);
                            }
                            else if (wParam.ToUInt32() is (uint)HITTEST.HTCLOSE)
                            {
                                (Content as MainPage).PressButton(CaptionButton.Close);
                            }

                            // 在标题栏按钮上按下左键后我们便捕获光标，这样才能在释放时得到通知。注意捕获光标后
                            // 便不会再收到 NC 族消息，这就是为什么我们要处理 WM_MOUSEMOVE 和 WM_LBUTTONUP
                            User32Library.SetCapture(hwndTitleBar);
                        }
                        return IntPtr.Zero;
                    }
                // 处理非客户区左键双击的窗口消息
                case WindowMessage.WM_NCLBUTTONDBLCLK:
                    {
                        // 手动处理标题栏上的点击。如果在标题栏按钮上，则通知 CaptionButtons，否则将消息传递
                        // 给主窗口。
                        if (wParam.Equals(new UIntPtr((int)HITTEST.HTTOP)) || wParam.Equals(new UIntPtr((int)HITTEST.HTCAPTION)))
                        {
                            // 将 HTTOP 传给主窗口才能通过上边框调整窗口高度
                            return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                        }
                        else if (wParam.Equals(new UIntPtr((int)HITTEST.HTMINBUTTON)) || wParam.Equals(new UIntPtr((int)HITTEST.HTMAXBUTTON)) || wParam.Equals(new UIntPtr((int)HITTEST.HTCLOSE)))
                        {
                            if (wParam.ToUInt32() is (uint)HITTEST.HTMINBUTTON)
                            {
                                (Content as MainPage).PressButton(CaptionButton.Minimize);
                            }
                            else if (wParam.ToUInt32() is (uint)HITTEST.HTMINBUTTON)
                            {
                                (Content as MainPage).PressButton(CaptionButton.Maximize);
                            }
                            else if (wParam.ToUInt32() is (uint)HITTEST.HTCLOSE)
                            {
                                (Content as MainPage).PressButton(CaptionButton.Close);
                            }

                            // 在标题栏按钮上按下左键后我们便捕获光标，这样才能在释放时得到通知。注意捕获光标后
                            // 便不会再收到 NC 族消息，这就是为什么我们要处理 WM_MOUSEMOVE 和 WM_LBUTTONUP
                            User32Library.SetCapture(hwndTitleBar);
                        }
                        return IntPtr.Zero;
                    }
                // 处理客户区左键释放的窗口消息
                case WindowMessage.WM_LBUTTONUP:
                    {
                        User32Library.ReleaseCapture();
                        Point cursorPos = new((int)LOWORD((uint)lParam.ToInt32()), (int)HIWORD((uint)lParam.ToInt32()));
                        User32Library.MapWindowPoints(hwndTitleBar, IntPtr.Zero, ref cursorPos, 2);
                        wParam = (UIntPtr)(uint)User32Library.SendMessage(hwndTitleBar, WindowMessage.WM_NCHITTEST, UIntPtr.Zero, new IntPtr(MakeLParam(cursorPos.X, cursorPos.Y)));
                        goto case WindowMessage.WM_NCLBUTTONUP;
                    }
                // 处理非客户区左键释放的窗口消息
                case WindowMessage.WM_NCLBUTTONUP:
                    {
                        // 处理鼠标在标题栏上释放。如果位于标题栏按钮上，则传递给 CaptionButtons，不在则将消息传递给主窗口
                        // 给主窗口。
                        if (wParam.Equals(new UIntPtr((int)HITTEST.HTTOP)) || wParam.Equals(new UIntPtr((int)HITTEST.HTCAPTION)))
                        {
                            // 在可拖拽区域或上边框释放左键，将此消息传递给主窗口
                            (Content as MainPage).ReleaseButtons();
                            return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                        }
                        else if (wParam.Equals(new UIntPtr((int)HITTEST.HTMINBUTTON)) || wParam.Equals(new UIntPtr((int)HITTEST.HTMAXBUTTON)) || wParam.Equals(new UIntPtr((int)HITTEST.HTCLOSE)))
                        {
                            if (wParam.ToUInt32() is (uint)HITTEST.HTMINBUTTON)
                            {
                                (Content as MainPage).ReleaseButton(CaptionButton.Minimize);
                            }
                            else if (wParam.ToUInt32() is (uint)HITTEST.HTMAXBUTTON)
                            {
                                (Content as MainPage).ReleaseButton(CaptionButton.Maximize);
                            }
                            else if (wParam.ToUInt32() is (uint)HITTEST.HTCLOSE)
                            {
                                (Content as MainPage).ReleaseButton(CaptionButton.Close);
                            }
                            (Content as MainPage).ReleaseButtons();
                        }
                        return IntPtr.Zero;
                    }
                // 处理非客户区右键按下的窗口消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        // 不关心右键，将它们传递给主窗口
                        return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                    }
                // 处理非客户区右键释放的窗口消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        // 不关心右键，将它们传递给主窗口
                        return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                    }
                // 处理非客户区右键双击的窗口消息
                case WindowMessage.WM_NCRBUTTONDBLCLK:
                    {
                        // 不关心右键，将它们传递给主窗口
                        return User32Library.SendMessage(Handle, Msg, wParam, lParam);
                    }
            }
            return User32Library.DefWindowProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第三部分：窗口过程

        #region 第四部分：窗口属性设置

        /// <summary>
        /// 设置应用的主题色
        /// </summary>
        public void SetWindowTheme()
        {
            if (Enum.TryParse(ThemeService.AppTheme, out ElementTheme theme))
            {
                (Content as MainPage).WindowTheme = theme;
            }
        }

        /// <summary>
        /// 添加窗口背景色
        /// </summary>
        public void SetWindowBackdrop()
        {
            if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[0]))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundDefault", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[1]))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                (Content as MainPage).Background = new MicaBrush(MicaKind.Base, Content as FrameworkElement, this, AlwaysShowBackdropService.AlwaysShowBackdropValue);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[2]))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                (Content as MainPage).Background = new MicaBrush(MicaKind.BaseAlt, Content as FrameworkElement, this, AlwaysShowBackdropService.AlwaysShowBackdropValue);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[3]))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                (Content as MainPage).Background = new DesktopAcrylicBrush(DesktopAcrylicKind.Default, Content as FrameworkElement, this, AlwaysShowBackdropService.AlwaysShowBackdropValue, true);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[4]))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                (Content as MainPage).Background = new DesktopAcrylicBrush(DesktopAcrylicKind.Base, Content as FrameworkElement, this, AlwaysShowBackdropService.AlwaysShowBackdropValue, true);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[5]))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                (Content as MainPage).Background = new DesktopAcrylicBrush(DesktopAcrylicKind.Thin, Content as FrameworkElement, this, AlwaysShowBackdropService.AlwaysShowBackdropValue, true);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private static void SetClassicMenuTheme()
        {
            if (string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]))
            {
                if (global::Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                    UxthemeLibrary.FlushMenuThemes();
                }
                else
                {
                    UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                    UxthemeLibrary.FlushMenuThemes();
                }
            }
            else if (string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[1]))
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                UxthemeLibrary.FlushMenuThemes();
            }
            else if (string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[2]))
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                UxthemeLibrary.FlushMenuThemes();
            }
        }

        /// <summary>
        /// 设置所有弹出控件主题
        /// </summary>
        private void SetPopupControlTheme(ElementTheme elementTheme)
        {
            foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
            {
                popup.RequestedTheme = elementTheme;

                if (popup.Child is FlyoutPresenter flyoutPresenter)
                {
                    flyoutPresenter.RequestedTheme = elementTheme;
                }

                if (popup.Child is Grid grid && grid.Name is "OuterOverflowContentRootV2")
                {
                    grid.RequestedTheme = elementTheme;
                }
            }
        }

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            return IntPtr.Size is 8 ? User32Library.GetWindowLongPtr(hWnd, nIndex) : User32Library.GetWindowLong(hWnd, nIndex);
        }

        /// <summary>
        /// 更改窗口属性
        /// </summary>
        private static IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size is 8 ? User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong) : User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
        }

        /// <summary>
        /// 更新此窗口周围绘制的外部边框的宽度
        /// </summary>
        private void UpdateFrameBorderThickness()
        {
            // Win10 中窗口边框始终只有一个像素宽，Win11 中的窗口边框宽度和 DPI 缩放有关
            if (Environment.Version.Build >= 22000)
            {
                DwmapiLibrary.DwmGetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_VISIBLE_FRAME_BORDER_THICKNESS, out IntPtr value, sizeof(uint));
                nativeTopBorderHeight = value.ToInt32();
            }
        }

        /// <summary>
        /// 更新此窗口周围绘制的外部边框的边距
        /// </summary>
        private void UpdateFrameMargins()
        {
            if (Environment.Version.Build < 22000)
            {
                Margins margins = new();

                if (GetTopBorderHeight() > 0)
                {
                    // 在 Win10 中，移除标题栏时上边框也被没了。我们的解决方案是：使用 DwmExtendFrameIntoClientArea 将边框扩展到客户区，然后在顶部绘制了一个黑色实线来显示系统原始边框（这种情况下操作系统将黑色视为透明）。因此我们有完美的上边框！
                    // 见 https://docs.microsoft.com/en-us/windows/win32/dwm/customframe#extending-the-client-frame
                    // 有的软件自己绘制了假的上边框，如 Chromium 系、WinUI 3 等，但窗口失去焦点时边框是半透明的，无法完美模拟。
                    // 我们选择扩展到标题栏高度，这是最好的选择。一个自然的想法是，既然上边框只有一个像素高，我们扩展一个像素即可，可惜因为 DWM 的 bug，这会使窗口失去焦点时上边框变为透明。那么能否传一个负值，让边框扩展到整个客户区？这大部分情况下可以工作，有一个小 bug：不显示边框颜色的设置下深色模式的边框会变为纯黑而不是半透明。
                    RECT frame = new();
                    User32Library.AdjustWindowRectExForDpi(ref frame, (WindowStyle)GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE), false, 0, Convert.ToUInt32(DeviceDpi));
                    margins.Top -= frame.top;

                    DwmapiLibrary.DwmExtendFrameIntoClientArea(Handle, ref margins);
                }
            }
        }

        /// <summary>
        /// 获取窗口上边框的高度
        /// </summary>
        private int GetTopBorderHeight()
        {
            return ExtendsContentIntoTitleBar && !User32Library.IsZoomed(Handle) ? nativeTopBorderHeight : 0;
        }

        /// <summary>
        /// 调整标题栏区域
        /// </summary>
        private void ResizeTitleBarWindow()
        {
            if (ExtendsContentIntoTitleBar && !Equals(hwndTitleBar, IntPtr.Zero))
            {
                // 获取标题栏的边框矩形
                Grid titleBar = (Content as MainPage).AppTitlebar;

                global::Windows.Foundation.Rect rect = new(0, 0, titleBar.ActualWidth, titleBar.ActualHeight);
                rect = titleBar.TransformToVisual(Content).TransformBounds(rect);

                double dpiScale = (double)DeviceDpi / 96;

                // 将标题栏窗口置于 XAML Islands 窗口上方
                int titleBarWidth = (int)Math.Ceiling(rect.Width * dpiScale);
                User32Library.SetWindowPos(hwndTitleBar, IntPtr.Zero, (int)Math.Floor(rect.X * dpiScale), Convert.ToInt32(Math.Floor(rect.Y * dpiScale)) + GetTopBorderHeight(), titleBarWidth, (int)Math.Floor(rect.Height * dpiScale + 1), SetWindowPosFlags.SWP_SHOWWINDOW);

                if (!Equals(hwndMaximizeButton, IntPtr.Zero))
                {
                    double captionButtonHeightInDips = (Content as MainPage).CaptionButtons.Height;
                    int captionButtonHeightInPixels = (int)Math.Ceiling(captionButtonHeightInDips * dpiScale);
                    User32Library.MoveWindow(hwndMaximizeButton, 0, 0, titleBarWidth, captionButtonHeightInPixels, false);
                }

                // 设置标题栏窗口的最大化样式，这样才能展示正确的文字提示
                WindowStyle style = (WindowStyle)GetWindowLongAuto(hwndTitleBar, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(hwndTitleBar, WindowLongIndexFlags.GWL_STYLE, (IntPtr)(User32Library.IsZoomed(Handle) ? style | WindowStyle.WS_MAXIMIZE : style & ~WindowStyle.WS_MAXIMIZE));
            }
        }

        private int GetResizeHandleHeight()
        {
            // 没有 SM_CYPADDEDBORDER
            return User32Library.GetSystemMetricsForDpi(SM.SM_CXPADDEDBORDER, DeviceDpi) + User32Library.GetSystemMetricsForDpi(SM.SM_CYSIZEFRAME, DeviceDpi);
        }

        /// <summary>
        /// 检查显示器的每一条边是否都隐藏了任务栏
        /// </summary>

        private bool HasAutoHideTaskbar(Screen screen, ABE edge)
        {
            APPBARDATA appbarData = new()
            {
                cbSize = Marshal.SizeOf<APPBARDATA>(),
                uEdge = edge,
                rc = new RECT()
                {
                    left = screen.Bounds.Left,
                    right = screen.Bounds.Right,
                    top = screen.Bounds.Top,
                    bottom = screen.Bounds.Bottom
                }
            };

            IntPtr hTaskbar = Shell32Library.SHAppBarMessage(ABM.ABM_GETAUTOHIDEBAREX, ref appbarData);
            return !hTaskbar.Equals(IntPtr.Zero);
        }

        private int MakeLParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | LoWord);
        }

        private uint HIWORD(uint dword)
        {
            return (dword >> 16) & 0xffff;
        }

        private uint LOWORD(uint dword)
        {
            return dword & 0xffff;
        }

        #endregion 第四部分：窗口属性设置

        #region 第五部分：显示对话框和应用通知

        /// <summary>
        /// 显示内容对话框
        /// </summary>
        public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog contentDialog)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!isDialogOpening && contentDialog is not null && Content is not null)
            {
                isDialogOpening = true;

                try
                {
                    contentDialog.XamlRoot = Content.XamlRoot;
                    dialogResult = await contentDialog.ShowAsync();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(ShowDialogAsync), 1, e);
                }
                isDialogOpening = false;
            }

            return dialogResult;
        }

        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            if (teachingTip is not null && Content is Page page && page.Content is Grid grid)
            {
                try
                {
                    grid.Children.Add(teachingTip);

                    teachingTip.IsOpen = true;
                    await Task.Delay(duration);
                    teachingTip.IsOpen = false;

                    // 应用内通知关闭动画显示耗费 300 ms
                    await Task.Delay(300);
                    grid.Children.Remove(teachingTip);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(ShowNotificationAsync), 1, e);
                }
            }
        }

        #endregion 第五部分：显示对话框和应用通知
    }
}
