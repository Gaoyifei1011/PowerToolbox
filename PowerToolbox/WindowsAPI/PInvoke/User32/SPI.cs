using System;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    [Flags]
    internal enum SPI : uint
    {
        /// <summary>
        /// 确定警告蜂鸣器是否已打开。
        /// pvParam 参数必须指向一个 BOOL 变量，该变量在蜂鸣器处于打开状态时接收 TRUE; 如果关闭，则为 FALSE。
        /// </summary>
        SPI_GETBEEP = 1,

        /// <summary>
        /// 打开或关闭警告蜂鸣器。 uiParam 参数为 on 指定 TRUE，为 off 指定 FALSE。
        /// </summary>
        SPI_SETBEEP = 2,

        /// <summary>
        /// 检索两个鼠标阈值和鼠标加速。 pvParam 参数必须指向接收这些值的三个整数数组。
        /// </summary>
        SPI_GETMOUSE = 3,

        /// <summary>
        /// 设置两个鼠标阈值和鼠标加速。 pvParam 参数必须指向指定这些值的三个整数数组。
        /// </summary>
        SPI_SETMOUSE = 4,

        /// <summary>
        /// 检索确定窗口大小边框宽度的边框乘数因子。 pvParam 参数必须指向接收此值的整数变量。
        /// </summary>
        SPI_GETBORDER = 5,

        /// <summary>
        /// 设置确定窗口大小边框宽度的边框乘数因子。 uiParam 参数指定新值。
        /// </summary>
        SPI_SETBORDER = 6,

        /// <summary>
        /// 检索键盘重复速度设置，该值的范围从 0 (大约每秒 2.5 次重复) 到 31 次 (大约每秒 30 次重复) 。 实际重复率取决于硬件，并且可能从线性刻度变化多达 20%。 pvParam 参数必须指向接收设置的 DWORD 变量。
        /// </summary>
        SPI_GETKEYBOARDSPEED = 10,

        /// <summary>
        /// 设置键盘重复速度设置。 uiParam 参数必须指定介于 0 (大约每秒 2.5 次重复) 到 31 (大约每秒 30 次重复) 范围内的值。 实际重复率取决于硬件，并且可能从线性刻度变化多达 20%。 如果 uiParam 大于 31，则 参数设置为 31。
        /// </summary>
        SPI_SETKEYBOARDSPEED = 11,

        SPI_LANGDRIVER = 12,

        /// <summary>
        /// 设置或检索图标单元格的宽度（以像素为单位）。 系统使用此矩形在大图标视图中排列图标。
        /// 若要设置此值，请将 uiParam 设置为新值，并将 pvParam 设置为 NULL。 不能将此值设置为小于 SM_CXICON。
        /// 若要检索此值， pvParam 必须指向接收当前值的整数。
        /// </summary>
        SPI_ICONHORIZONTALSPACING = 13,

        /// <summary>
        /// 检索屏幕节省程序超时值（以秒为单位）。 pvParam 参数必须指向接收值的整数变量。
        /// </summary>
        SPI_GETSCREENSAVETIMEOUT = 14,

        /// <summary>
        /// 将屏幕节省程序超时值设置为 uiParam 参数的值。 此值是系统在激活屏幕保护程序之前必须处于空闲状态的时间量（以秒为单位）。
        /// 如果计算机已进入省电模式或系统锁定状态，则会发生ERROR_OPERATION_IN_PROGRESS异常。
        /// </summary>
        SPI_SETSCREENSAVETIMEOUT = 15,

        /// <summary>
        /// 确定是否启用屏幕保存。 pvParam 参数必须指向一个 BOOL 变量，如果启用屏幕保存，则接收 TRUE;否则，该变量必须指向 FALSE。
        /// Windows 7、Windows Server 2008 R2 和 Windows 2000： 即使未启用屏幕保存，函数也会返回 TRUE 。
        /// </summary>
        SPI_GETSCREENSAVEACTIVE = 16,

        /// <summary>
        /// 设置屏幕保护程序的状态。 uiParam 参数指定 TRUE 以激活屏幕保存，或指定 FALSE 以停用屏幕保存。
        /// 如果计算机已进入省电模式或系统锁定状态，则会发生ERROR_OPERATION_IN_PROGRESS异常。
        /// </summary>
        SPI_SETSCREENSAVEACTIVE = 17,

        SPI_GETGRIDGRANULARITY = 18,
        SPI_SETGRIDGRANULARITY = 19,

        /// <summary>
        /// 使用 SPI_SETDESKWALLPAPER 标志时， SystemParametersInfo 返回 TRUE ，除非 (出现错误，例如) 指定的文件不存在时。
        /// </summary>
        SPI_SETDESKWALLPAPER = 20,

        /// <summary>
        /// 通过使 Windows 从 WIN.INI 文件中读取 Pattern= 设置来设置当前桌面模式。
        /// </summary>
        SPI_SETDESKPATTERN = 21,

        /// <summary>
        /// 检索键盘重复延迟设置，该值介于 0 (大约 250 毫秒延迟) 到 3 (大约 1 秒延迟) 。 与每个值关联的实际延迟可能因硬件而异。 pvParam 参数必须指向接收设置的整数变量。
        /// </summary>
        SPI_GETKEYBOARDDELAY = 22,

        /// <summary>
        /// 设置键盘重复延迟设置。 uiParam 参数必须指定 0、1、2 或 3，其中 0 设置最短延迟约 250 毫秒) ,3 设置最长延迟 (大约 1 秒) 。 与每个值关联的实际延迟可能因硬件而异。
        /// </summary>
        SPI_SETKEYBOARDDELAY = 23,

        /// <summary>
        /// 设置或检索图标单元格的高度（以像素为单位）。
        /// 若要设置此值，请将 uiParam 设置为新值，并将 pvParam 设置为 NULL。 不能将此值设置为小于 SM_CYICON。
        /// 若要检索此值， pvParam 必须指向接收当前值的整数。
        /// </summary>
        SPI_ICONVERTICALSPACING = 24,

        /// <summary>
        /// 确定是否启用图标标题包装。 pvParam 参数必须指向接收 TRUE（如果启用）的 BOOL 变量;否则，该变量必须指向 FALSE。
        /// </summary>
        SPI_GETICONTITLEWRAP = 25,

        /// <summary>
        /// 打开或关闭图标标题环绕。 uiParam 参数为 on 指定 TRUE，为 off 指定 FALSE。
        /// </summary>
        SPI_SETICONTITLEWRAP = 26,

        /// <summary>
        /// 确定弹出菜单相对于相应的菜单栏项是左对齐还是右对齐。 pvParam 参数必须指向接收 TRUE（如果右对齐）的 BOOL 变量;否则，该变量必须指向 FALSE。
        /// </summary>
        SPI_GETMENUDROPALIGNMENT = 27,

        /// <summary>
        /// 设置弹出菜单的对齐值。 uiParam 参数为右对齐指定 TRUE，为左对齐指定 FALSE。
        /// </summary>
        SPI_SETMENUDROPALIGNMENT = 28,

        /// <summary>
        /// 将双击矩形的宽度设置为 uiParam 参数的值。
        /// 双击矩形是双击的第二次单击必须属于该矩形，才能将其注册为双击。
        /// 若要检索双击矩形的宽度，请使用 SM_CXDOUBLECLK 标志调用 GetSystemMetrics。
        /// </summary>
        SPI_SETDOUBLECLKWIDTH = 29,

        /// <summary>
        /// 将双击矩形的高度设置为 uiParam 参数的值。
        /// 双击矩形是一个矩形，双击的第二次单击必须属于该矩形，才能将其注册为双击。
        /// 若要检索双击矩形的高度，请使用 SM_CYDOUBLECLK 标志调用 GetSystemMetrics。
        /// </summary>
        SPI_SETDOUBLECLKHEIGHT = 30,

        /// <summary>
        /// 检索当前图标标题字体的逻辑字体信息。 uiParam 参数指定 LOGFONT 结构的大小，pvParam 参数必须指向要填充的 LOGFONT 结构。
        /// </summary>
        SPI_GETICONTITLELOGFONT = 31,

        /// <summary>
        /// 将鼠标的双击时间设置为 uiParam 参数的值。 如果 uiParam 值大于 5000 毫秒，系统会将双击时间设置为 5000 毫秒。
        /// 双击时间是双击的第一次和第二次单击之间可能发生的最大毫秒数。 还可以调用 SetDoubleClickTime 函数来设置双击时间。 若要获取当前双击时间，请调用 GetDoubleClickTime 函数。
        /// </summary>
        SPI_SETDOUBLECLICKTIME = 32,

        /// <summary>
        /// 交换或还原鼠标左键和右键的含义。 uiParam 参数指定 TRUE 以交换按钮的含义，或指定 FALSE 以还原其原始含义。
        /// 若要检索当前设置，请使用 SM_SWAPBUTTON 标志调用 GetSystemMetrics。
        /// </summary>
        SPI_SETMOUSEBUTTONSWAP = 33,

        /// <summary>
        /// 设置用于图标标题的字体。 uiParam 参数指定 LOGFONT 结构的大小，pvParam 参数必须指向 LOGFONT 结构。
        /// </summary>
        SPI_SETICONTITLELOGFONT = 34,

        SPI_GETFASTTASKSWITCH = 35,
        SPI_SETFASTTASKSWITCH = 36,

        /// <summary>
        /// 设置打开或关闭整个窗口的拖动。 uiParam 参数为 on 指定 TRUE，为 off 指定 FALSE。
        /// </summary>
        SPI_SETDRAGFULLWINDOWS = 37,

        /// <summary>
        /// 确定是否启用拖动整个窗口。 pvParam 参数必须指向一个 BOOL 变量（如果启用）接收 TRUE;否则，该变量必须指向 FALSE。
        /// </summary>
        SPI_GETDRAGFULLWINDOWS = 38,

        /// <summary>
        /// 检索与非最小化窗口的非工作区相关联的度量值。 pvParam 参数必须指向接收信息的 NONCLIENTMETRICS 结构。 将此 结构的 cbSize 成员和 uiParam 参数设置为 sizeof(NONCLIENTMETRICS)。
        /// Windows Server 2003 和 Windows XP/2000： 请参阅 NONCLIENTMETRICS 的备注。
        /// </summary>
        SPI_GETNONCLIENTMETRICS = 41,

        /// <summary>
        /// 设置与非小窗口的非工作区关联的指标。 pvParam 参数必须指向包含新参数的 NONCLIENTMETRICS 结构。 将此 结构的 cbSize 成员和 uiParam 参数设置为 sizeof(NONCLIENTMETRICS)。 此外，LOGFONT 结构的 lfHeight 成员必须是负值。
        /// </summary>
        SPI_SETNONCLIENTMETRICS = 42,

        /// <summary>
        /// 检索与最小化窗口关联的指标。 pvParam 参数必须指向接收信息的 MINIMIZEDMETRICS 结构。 将此 结构的 cbSize 成员和 uiParam 参数设置为 sizeof(MINIMIZEDMETRICS)。
        /// </summary>
        SPI_GETMINIMIZEDMETRICS = 43,

        /// <summary>
        /// 设置与最小化窗口关联的指标。 pvParam 参数必须指向包含新参数的 MINIMIZEDMETRICS 结构。 将此 结构的 cbSize 成员和 uiParam 参数设置为 sizeof(MINIMIZEDMETRICS)。
        /// </summary>
        SPI_SETMINIMIZEDMETRICS = 44,

        /// <summary>
        /// 检索与图标关联的指标。 pvParam 参数必须指向接收信息的 ICONMETRICS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(ICONMETRICS)。
        /// </summary>
        SPI_GETICONMETRICS = 45,

        /// <summary>
        /// 设置与图标关联的指标。 pvParam 参数必须指向包含新参数的 ICONMETRICS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(ICONMETRICS)。
        /// </summary>
        SPI_SETICONMETRICS = 46,

        /// <summary>
        /// 设置工作区的大小。 工作区域是系统任务栏或应用程序桌面工具栏未遮挡的屏幕部分。 pvParam 参数是指向 RECT 结构的指针，该结构指定以虚拟屏幕坐标表示的新工作区矩形。 在具有多个显示监视器的系统中，函数设置包含指定矩形的监视器的工作区域。
        /// </summary>
        SPI_SETWORKAREA = 47,

        /// <summary>
        /// 检索主显示器上工作区的大小。 工作区域是系统任务栏或应用程序桌面工具栏未遮挡的屏幕部分。 pvParam 参数必须指向接收工作区坐标的 RECT 结构，以物理像素大小表示。 调用方的任何 DPI 虚拟化模式都不会影响此输出。
        /// 若要获取主显示监视器以外的监视器的工作区域，请调用 GetMonitorInfo 函数。
        /// </summary>
        SPI_GETWORKAREA = 48,

        SPI_SETPENWINDOWS = 49,

        /// <summary>
        /// 检索有关 FilterKeys 辅助功能的信息。 pvParam 参数必须指向接收信息的 FILTERKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(FILTERKEYS)。
        /// </summary>
        SPI_GETFILTERKEYS = 50,

        /// <summary>
        /// 设置 FilterKeys 辅助功能的参数。 pvParam 参数必须指向包含新参数的 FILTERKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(FILTERKEYS)。
        /// </summary>
        SPI_SETFILTERKEYS = 51,

        /// <summary>
        /// 检索有关 ToggleKeys 辅助功能的信息。 pvParam 参数必须指向接收信息的 TOGGLEKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(TOGGLEKEYS)。
        /// </summary>
        SPI_GETTOGGLEKEYS = 52,

        /// <summary>
        /// 设置 ToggleKeys 辅助功能的参数。 pvParam 参数必须指向包含新参数的 TOGGLEKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(TOGGLEKEYS)。
        /// </summary>
        SPI_SETTOGGLEKEYS = 53,

        /// <summary>
        /// 检索有关 MouseKeys 辅助功能的信息。 pvParam 参数必须指向接收信息的 MOUSEKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(MOUSEKEYS)。
        /// </summary>
        SPI_GETMOUSEKEYS = 54,

        /// <summary>
        /// 设置 MouseKeys 辅助功能的参数。 pvParam 参数必须指向包含新参数的 MOUSEKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(MOUSEKEYS)。
        /// </summary>
        SPI_SETMOUSEKEYS = 55,

        /// <summary>
        /// 确定“显示声音”辅助功能标志是打开还是关闭。 如果它处于打开状态，则用户要求应用程序在仅以声音形式呈现信息的情况下直观显示信息。 pvParam 参数必须指向一个 BOOL 变量，如果功能处于打开状态，则接收 TRUE;如果关闭，则为 FALSE。
        /// 使用此值等效于使用 SM_SHOWSOUNDS 调用 GetSystemMetrics。 这是建议的调用。
        /// </summary>
        SPI_GETSHOWSOUNDS = 56,

        /// <summary>
        /// 打开或关闭 ShowSounds 辅助功能。 uiParam 参数为 on 指定 TRUE，为 off 指定 FALSE。
        /// </summary>
        SPI_SETSHOWSOUNDS = 57,

        /// <summary>
        /// 检索有关 StickyKeys 辅助功能的信息。 pvParam 参数必须指向接收信息的 STICKYKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(STICKYKEYS)。
        /// </summary>
        SPI_GETSTICKYKEYS = 58,

        /// <summary>
        /// 设置 StickyKeys 辅助功能的参数。 pvParam 参数必须指向包含新参数的 STICKYKEYS 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(STICKYKEYS)。
        /// </summary>
        SPI_SETSTICKYKEYS = 59,

        /// <summary>
        /// 检索有关与辅助功能关联的超时时间段的信息。 pvParam 参数必须指向接收信息的 ACCESSTIMEOUT 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(ACCESSTIMEOUT)。
        /// </summary>
        SPI_GETACCESSTIMEOUT = 60,

        /// <summary>
        /// 设置与辅助功能关联的超时期限。 pvParam 参数必须指向包含新参数的 ACCESSTIMEOUT 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(ACCESSTIMEOUT)。
        /// </summary>
        SPI_SETACCESSTIMEOUT = 61,

        /// <summary>
        /// 不支持此参数。
        /// Windows Server 2003 和 Windows XP/2000： 用户应通过控制面板控制此设置。
        /// </summary>
        SPI_GETSERIALKEYS = 62,

        /// <summary>
        /// 不支持此参数。
        /// Windows Server 2003 和 Windows XP/2000： 用户应通过控制面板控制此设置。
        /// </summary>
        SPI_SETSERIALKEYS = 63,

        /// <summary>
        /// 检索有关 SoundSentry 辅助功能的信息。 pvParam 参数必须指向接收信息的 SOUNDSENTRY 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(SOUNDSENTRY)。
        /// </summary>
        SPI_GETSOUNDSENTRY = 64,

        /// <summary>
        /// 设置 SoundSentry 辅助功能的参数。 pvParam 参数必须指向包含新参数的 SOUNDSENTRY 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(SOUNDSENTRY)。
        /// </summary>
        SPI_SETSOUNDSENTRY = 65,

        /// <summary>
        /// 检索有关 HighContrast 辅助功能的信息。 pvParam 参数必须指向接收信息的 HIGHCONTRAST 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(HIGHCONTRAST)。
        /// </summary>
        SPI_GETHIGHCONTRAST = 66,

        /// <summary>
        /// 设置 HighContrast 辅助功能的参数。 pvParam 参数必须指向包含新参数的 HIGHCONTRAST 结构。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(HIGHCONTRAST)。
        /// </summary>
        SPI_SETHIGHCONTRAST = 67,

        /// <summary>
        /// 确定用户是否依赖于键盘而不是鼠标，并希望应用程序显示本来会隐藏的键盘接口。 如果用户依赖键盘，pvParam 参数必须指向接收 TRUE 的 BOOL 变量;否则为 FALSE。
        /// </summary>
        SPI_GETKEYBOARDPREF = 68,

        /// <summary>
        /// 	设置键盘首选项。 如果用户依赖于键盘而不是鼠标，并且希望应用程序显示本来隐藏的键盘接口， 则 uiParam 参数指定 TRUE ; 否则，uiParam 为 FALSE 。
        /// </summary>
        SPI_SETKEYBOARDPREF = 69,

        /// <summary>
        /// 确定屏幕审阅者实用工具是否正在运行。 屏幕审阅者实用工具将文本信息定向到输出设备，例如语音合成器或盲文显示器。 设置此标志后，应用程序应在以图形方式呈现信息的情况下提供文本信息。
        /// pvParam 参数是指向 BOOL 变量的指针，如果屏幕审阅者实用工具正在运行，则接收 TRUE，否则接收 FALSE。
        /// </summary>
        SPI_GETSCREENREADER = 70,

        /// <summary>
        /// 确定屏幕评审实用工具是否正在运行。 uiParam 参数为 on 指定 TRUE，为 off 指定 FALSE。
        /// </summary>
        SPI_SETSCREENREADER = 71,

        /// <summary>
        /// 检索与用户操作关联的动画效果。 pvParam 参数必须指向接收信息的 ANIMATIONINFO 结构。 将此 结构的 cbSize 成员和 uiParam 参数设置为 sizeof(ANIMATIONINFO)。
        /// </summary>
        SPI_GETANIMATION = 72,

        /// <summary>
        /// 设置与用户操作关联的动画效果。 pvParam 参数必须指向包含新参数的 ANIMATIONINFO 结构。 将此 结构的 cbSize 成员和 uiParam 参数设置为 sizeof(ANIMATIONINFO)。
        /// </summary>
        SPI_SETANIMATION = 73,

        /// <summary>
        /// 确定是否启用字体平滑功能。 此功能使用字体抗锯齿，通过绘制不同灰色级别的像素，使字体曲线看起来更平滑。
        /// pvParam 参数必须指向一个 BOOL 变量，该变量在启用该功能时接收 TRUE; 如果功能未启用，则为 FALSE。
        /// </summary>
        SPI_GETFONTSMOOTHING = 74,

        /// <summary>
        /// 启用或禁用字体平滑功能，该功能使用字体抗锯齿功能，通过绘制不同灰度级别的像素，使字体曲线看起来更平滑。
        /// 若要启用该功能，请将 uiParam 参数设置为 TRUE。 若要禁用该功能，请将 uiParam 设置为 FALSE。
        /// </summary>
        SPI_SETFONTSMOOTHING = 75,

        /// <summary>
        /// 设置用于检测拖动操作开始的矩形的宽度（以像素为单位）。 将 uiParam 设置为新值。 若要检索拖动宽度，请使用SM_CXDRAG标志调用 GetSystemMetrics。
        /// </summary>
        SPI_SETDRAGWIDTH = 76,

        /// <summary>
        /// 设置用于检测拖动操作开始的矩形的高度（以像素为单位）。 将 uiParam 设置为新值。 若要检索拖动高度，请使用SM_CYDRAG标志调用 GetSystemMetrics。
        /// </summary>
        SPI_SETDRAGHEIGHT = 77,

        SPI_SETHANDHELD = 78,

        /// <summary>
        /// 不支持此参数。
        /// Windows Server 2003 和 Windows XP/2000： 检索屏幕保存的低功耗阶段的超时值。 pvParam 参数必须指向接收值的整数变量。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_GETLOWPOWERTIMEOUT = 79,

        /// <summary>
        /// 不支持此参数。 请改为检查 GUID_VIDEO_POWERDOWN_TIMEOUT 电源设置。
        /// Windows Server 2003 和 Windows XP/2000： 检索屏幕保存的关机阶段的超时值。 pvParam 参数必须指向接收值的整数变量。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_GETPOWEROFFTIMEOUT = 80,

        /// <summary>
        /// 不支持此参数。
        /// Windows Server 2003 和 Windows XP/2000： 为屏幕保存的低功耗阶段设置超时值（以秒为单位）。 uiParam 参数指定新值。 pvParam 参数必须为 NULL。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_SETLOWPOWERTIMEOUT = 81,

        /// <summary>
        /// 不支持此参数。 相反，请将 GUID_VIDEO_POWERDOWN_TIMEOUT 电源设置设置为超时值。
        /// Windows Server 2003 和 Windows XP/2000： 为屏幕保存的关机阶段设置超时值（以秒为单位）。 uiParam 参数指定新值。 pvParam 参数必须为 NULL。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_SETPOWEROFFTIMEOUT = 82,

        /// <summary>
        /// 不支持此参数。
        /// Windows Server 2003 和 Windows XP/2000： 确定是否启用了屏幕保存的低功耗阶段。 pvParam 参数必须指向接收 TRUE（如果已启用）的 BOOL 变量; 如果禁用，则为 FALSE。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_GETLOWPOWERACTIVE = 83,

        /// <summary>
        /// 不支持此参数。 启用屏幕保存的关机阶段后， GUID_VIDEO_POWERDOWN_TIMEOUT 电源设置大于零。
        /// Windows Server 2003 和 Windows XP/2000： 确定是否启用了屏幕保存的关机阶段。 pvParam 参数必须指向接收 TRUE（如果已启用）的 BOOL 变量;如果禁用，则为 FALSE。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_GETPOWEROFFACTIVE = 84,

        /// <summary>
        /// 不支持此参数。
        /// Windows Server 2003 和 Windows XP/2000： 激活或停用屏幕保存的低功耗阶段。 将 uiParam 设置为 1 可激活，将零设置为停用。 pvParam 参数必须为 NULL。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_SETLOWPOWERACTIVE = 85,

        /// <summary>
        /// 不支持此参数。 请改为设置 GUID_VIDEO_POWERDOWN_TIMEOUT 电源设置。
        /// Windows Server 2003 和 Windows XP/2000： 激活或停用屏幕保存的关机阶段。 将 uiParam 设置为 1 可激活，将零设置为停用。 pvParam 参数必须为 NULL。 仅 32 位应用程序支持此标志。
        /// </summary>
        SPI_SETPOWEROFFACTIVE = 86,

        /// <summary>
        /// 重新加载系统游标。 将 uiParam 参数设置为零，将 pvParam 参数设置为 NULL。
        /// </summary>
        SPI_SETCURSORS = 87,

        /// <summary>
        /// 重新加载系统图标。 将 uiParam 参数设置为零，将 pvParam 参数设置为 NULL。
        /// </summary>
        SPI_SETICONS = 88,

        /// <summary>
        /// 检索系统默认输入语言的输入区域设置标识符。 pvParam 参数必须指向接收此值的 HKL 变量。 有关详细信息，请参阅 语言、区域设置和键盘布局。
        /// </summary>
        SPI_GETDEFAULTINPUTLANG = 89,

        /// <summary>
        /// 设置系统 shell 和应用程序的默认输入语言。 指定的语言必须使用当前系统字符集显示。 pvParam 参数必须指向包含默认语言的输入区域设置标识符的 HKL 变量。
        /// </summary>
        SPI_SETDEFAULTINPUTLANG = 90,

        /// <summary>
        /// 设置用于在输入语言之间切换的热键集。 不使用 uiParam 和 pvParam 参数。 值通过再次读取注册表来设置键盘属性表中的快捷键。 必须先设置注册表，然后才能使用此标志。 注册表中的路径为 HKEY_CURRENT_USER\键盘布局\切换. 有效值为“1”= Alt+SHIFT，“2”= CTRL+SHIFT，“3”= none。
        /// </summary>
        SPI_SETLANGTOGGLE = 91,

        SPI_GETWINDOWSEXTENSION = 92,

        /// <summary>
        /// 启用或禁用鼠标跟踪功能，该功能通过简要显示光标跟踪并快速擦除它们来提高鼠标光标移动的可见性。
        /// 若要禁用该功能，请将 uiParam 参数设置为零或 1。 若要启用该功能，请将 uiParam 设置为大于 1 的值，以指示在跟踪中绘制的游标数。
        /// </summary>
        SPI_SETMOUSETRAILS = 93,

        /// <summary>
        /// 确定是否启用鼠标跟踪功能。 此功能通过简要显示光标的踪迹并快速擦除它们来提高鼠标光标移动的可见性。
        /// pvParam 参数必须指向接收值的整数变量。 如果值为零或 1，则禁用该功能。 如果值大于 1，则启用该功能，该值指示在跟踪中绘制的游标数。 不使用 uiParam 参数。
        /// </summary>
        SPI_GETMOUSETRAILS = 94,

        /// <summary>
        /// 确定是否启用对齐到默认按钮功能。 如果启用，鼠标光标会自动移动到对话框的默认按钮，例如 “确定” 或 “应用”。 pvParam 参数必须指向一个 BOOL 变量，如果功能处于打开状态，则接收 TRUE;如果关闭，则为 FALSE。
        /// </summary>
        SPI_GETSNAPTODEFBUTTON = 95,

        /// <summary>
        /// 启用或禁用快照到默认按钮功能。 如果启用，鼠标光标会自动移动到对话框的默认按钮，例如 “确定” 或 “应用”。 将 uiParam 参数设置为 TRUE 以启用该功能，将 设置为 FALSE 以禁用该功能。 应用程序在显示对话框时应使用 ShowWindow 函数，以便对话管理器可以定位鼠标光标。
        /// </summary>
        SPI_SETSNAPTODEFBUTTON = 96,

        SPI_SETSCREENSAVERRUNNING = 97,
        SPI_SCREENSAVERRUNNING = SPI_SETSCREENSAVERRUNNING,

        /// <summary>
        /// 检索鼠标指针必须保留的矩形的宽度（以像素为单位），以便 TrackMouseEvent 生成 WM_MOUSEHOVER 消息。 pvParam 参数必须指向接收宽度的 UINT 变量。
        /// </summary>
        SPI_GETMOUSEHOVERWIDTH = 98,

        /// <summary>
        /// 设置鼠标指针必须保留的矩形的宽度（以像素为单位），以便 TrackMouseEvent 生成 WM_MOUSEHOVER 消息。 将 uiParam 参数设置为新的宽度。
        /// </summary>
        SPI_SETMOUSEHOVERWIDTH = 99,

        /// <summary>
        /// 检索鼠标指针必须保留的矩形的高度（以像素为单位），以便 TrackMouseEvent 生成 WM_MOUSEHOVER 消息。 pvParam 参数必须指向接收高度的 UINT 变量。
        /// </summary>
        SPI_GETMOUSEHOVERHEIGHT = 100,

        /// <summary>
        /// 设置鼠标指针必须保留的矩形的高度（以像素为单位），以便 TrackMouseEvent 生成 WM_MOUSEHOVER 消息。 将 uiParam 参数设置为新高度。
        /// </summary>
        SPI_SETMOUSEHOVERHEIGHT = 101,

        /// <summary>
        /// 检索鼠标指针必须停留在悬停矩形中的时间（以毫秒为单位），以便 TrackMouseEvent 生成 WM_MOUSEHOVER 消息。 pvParam 参数必须指向接收时间的 UINT 变量。
        /// </summary>
        SPI_GETMOUSEHOVERTIME = 102,

        /// <summary>
        /// 设置鼠标指针必须停留在悬停矩形中的时间（以毫秒为单位），以便 TrackMouseEvent 生成 WM_MOUSEHOVER 消息。 仅当在调用 TrackMouseEvent 时在 dwHoverTime 参数中传递HOVER_DEFAULT时，才使用此选项。 将 uiParam 参数设置为新时间。
        /// 指定的时间应介于 USER_TIMER_MAXIMUM 和 USER_TIMER_MINIMUM 之间。 如果 uiParam 小于 USER_TIMER_MINIMUM，函数将使用 USER_TIMER_MINIMUM。 如果 uiParam 大于 USER_TIMER_MAXIMUM，则将 USER_TIMER_MAXIMUM函数。
        /// Windows Server 2003 和 Windows XP： 在 Windows Server 2003 SP1 和 Windows XP 与 SP2 之前，操作系统不会强制使用 USER_TIMER_MAXIMUM 和 USER_TIMER_MINIMUM 。
        /// </summary>
        SPI_SETMOUSEHOVERTIME = 103,

        /// <summary>
        /// 检索移动垂直鼠标滚轮时要滚动的行数。 pvParam 参数必须指向接收行数的 UINT 变量。 默认值为 3。
        /// </summary>
        SPI_GETWHEELSCROLLLINES = 104,

        /// <summary>
        /// 设置移动垂直鼠标滚轮时要滚动的行数。 从 uiParam 参数设置行数。
        /// 行数是在不使用修饰键的情况下滚动鼠标滚轮时要滚动的建议行数。 如果数字为 0，则不应发生滚动。 如果要滚动的行数大于可查看的行数，特别是当它 WHEEL_PAGESCROLL (#defined 为 UINT_MAX) ，则滚动操作应解释为单击滚动条页面向下或向上页区域单击一次。
        /// </summary>
        SPI_SETWHEELSCROLLLINES = 105,

        /// <summary>
        /// 检索鼠标光标位于子菜单项上时系统在显示快捷菜单之前等待的时间（以毫秒为单位）。 pvParam 参数必须指向接收延迟时间的 DWORD 变量。
        /// </summary>
        SPI_GETMENUSHOWDELAY = 106,

        /// <summary>
        /// 将 uiParam 设置为鼠标光标位于子菜单项上时系统在显示快捷菜单之前等待的时间（以毫秒为单位）。
        /// </summary>
        SPI_SETMENUSHOWDELAY = 107,

        /// <summary>
        /// 检索移动水平鼠标滚轮时要滚动的字符数。 pvParam 参数必须指向接收行数的 UINT 变量。 默认值为 3。
        /// </summary>
        SPI_GETWHEELSCROLLCHARS = 108,

        /// <summary>
        /// 设置移动水平鼠标滚轮时要滚动的字符数。 从 uiParam 参数设置字符数。
        /// </summary>
        SPI_SETWHEELSCROLLCHARS = 109,

        /// <summary>
        /// 确定输入法状态窗口是否以每个用户) (可见。 pvParam 参数必须指向一个 BOOL 变量，该变量在状态窗口可见时接收 TRUE;如果状态窗口不可见，则为 FALSE。
        /// </summary>
        SPI_GETSHOWIMEUI = 110,

        /// <summary>
        /// 设置输入法状态窗口是否以每个用户为基础可见。 uiParam 参数为 on 指定 TRUE，为 off 指定 FALSE。
        /// </summary>
        SPI_SETSHOWIMEUI = 111,

        /// <summary>
        /// 检索当前鼠标速度。 鼠标速度根据鼠标移动的距离确定指针移动的距离。 pvParam 参数必须指向一个整数，该整数接收的值范围为 1 (最慢) 到 20 (最快) 。 默认值为 10。 该值可由最终用户使用鼠标控制面板应用程序设置，也可以由使用 SPI_SETMOUSESPEED的应用程序设置。
        /// </summary>
        SPI_GETMOUSESPEED = 112,

        /// <summary>
        /// 设置当前鼠标速度。 pvParam 参数是介于 1 (最慢) 到 20 (最快) 之间的整数。 默认值为 10。 此值通常是使用鼠标控制面板应用程序设置的。
        /// </summary>
        SPI_SETMOUSESPEED = 113,

        /// <summary>
        /// 确定屏幕保护程序当前是否在调用进程的窗口工作站上运行。 如果屏幕保护程序当前正在运行，pvParam 参数必须指向接收 TRUE 的 BOOL 变量，否则为 FALSE。 请注意，只有交互式窗口工作站 WinSta0 可以运行屏幕保护程序。
        /// </summary>
        SPI_GETSCREENSAVERRUNNING = 114,

        /// <summary>
        /// 检索桌面壁纸的位图文件的完整路径。 pvParam 参数必须指向缓冲区才能接收以 null 结尾的路径字符串。 将 uiParam 参数设置为 pvParam 缓冲区的大小（以字符为单位）。 返回的字符串不超过 MAX_PATH 个字符。 如果没有桌面壁纸，则返回的字符串为空。
        /// </summary>
        SPI_GETDESKWALLPAPER = 115,

        /// <summary>
        /// 确定是启用还是禁用音频说明。 pvParam 参数是指向 AUDIODESCRIPTION 结构的指针。 将此结构的 cbSize 成员和 uiParam 参数设置为 sizeof(AUDIODESCRIPTION)。
        /// 虽然有视觉障碍的用户可能会听到视频内容中的音频，但视频中有很多没有相应音频的操作。 视频中发生情况的特定音频说明可帮助这些用户更好地了解内容。 此标志使你能够确定音频说明是否已启用，以及使用哪种语言。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETAUDIODESCRIPTION = 116,

        /// <summary>
        /// 打开或关闭音频说明功能。 pvParam 参数是指向 AUDIODESCRIPTION 结构的指针。
        /// 虽然有视力障碍的用户可能会听到视频内容中的音频，但视频中有很多没有相应音频的操作。 视频中发生情况的特定音频说明可帮助这些用户更好地了解内容。 通过此标志，可以启用或禁用其提供的语言的音频说明。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETAUDIODESCRIPTION = 117,

        /// <summary>
        /// 确定屏幕保护程序是否需要密码才能显示 Windows 桌面。 如果屏幕保护程序需要密码，pvParam 参数必须指向接收 TRUE 的 BOOL 变量，否则为 FALSE。 忽略 uiParam 参数。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETSCREENSAVESECURE = 118,

        /// <summary>
        /// 设置屏幕保护程序是否要求用户输入密码才能显示 Windows 桌面。 uiParam 参数是 BOOL 变量。 忽略 pvParam 参数。 将 uiParam 设置为 TRUE 以要求密码，将 FALSE 设置为不需要密码。
        /// 如果计算机已进入省电模式或系统锁定状态，则会发生ERROR_OPERATION_IN_PROGRESS异常。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETSCREENSAVESECURE = 119,

        /// <summary>
        /// 检索线程在系统认为消息无响应之前无需调度消息即可完成的毫秒数。 pvParam 参数必须指向接收值的整数变量。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETHUNGAPPTIMEOUT = 120,

        /// <summary>
        /// 将挂起的应用程序超时设置为 uiParam 参数的值。 此值是线程在系统认为消息无响应之前无需分派消息即可到达的毫秒数。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETHUNGAPPTIMEOUT = 121,

        /// <summary>
        /// 检索系统在终止不响应关闭请求的应用程序之前等待的毫秒数。 pvParam 参数必须指向接收值的整数变量。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETWAITTOKILLTIMEOUT = 122,

        /// <summary>
        /// 将应用程序关闭请求超时设置为 uiParam 参数的值。 此值是系统在终止不响应关闭请求的应用程序之前等待的毫秒数。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETWAITTOKILLTIMEOUT = 123,

        /// <summary>
        /// 检索服务控制管理器在终止不响应关闭请求的服务之前等待的毫秒数。 pvParam 参数必须指向接收值的整数变量。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETWAITTOKILLSERVICETIMEOUT = 124,

        /// <summary>
        /// 将服务关闭请求超时设置为 uiParam 参数的值。 此值是系统在终止不响应关闭请求的服务之前等待的毫秒数。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETWAITTOKILLSERVICETIMEOUT = 125,

        /// <summary>
        /// 检索使用鼠标将窗口拖动到监视器或监视器阵列边缘触发停靠行为的阈值（以像素为单位）。 默认阈值为 1。 pvParam 参数必须指向接收值的 DWORD 变量。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETMOUSEDOCKTHRESHOLD = 126,

        /// <summary>
        /// 设置使用鼠标将窗口拖动到监视器或监视器数组边缘时触发停靠行为的阈值（以像素为单位）。 默认阈值为 1。 pvParam 参数必须指向包含新值的 DWORD 变量。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETMOUSEDOCKTHRESHOLD = 127,

        /// <summary>
        /// 检索使用笔将窗口拖动到监视器或监视器阵列边缘触发停靠行为的阈值（以像素为单位）。 默认值为 30。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETPENDOCKTHRESHOLD = 128,

        /// <summary>
        /// 设置使用笔将窗口拖动到监视器或监视器阵列边缘时触发停靠行为的阈值（以像素为单位）。 默认阈值为 30。 pvParam 参数必须指向包含新值的 DWORD 变量。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETPENDOCKTHRESHOLD = 129,

        /// <summary>
        /// 确定是否启用窗口排列。 pvParam 参数必须指向一个 BOOL 变量（如果启用）接收 TRUE;否则，该变量必须指向 FALSE。
        /// 窗口排列方式通过简化拖动或调整窗口大小时的默认行为，减少了移动顶级窗口和调整窗口大小所需的鼠标、笔或触摸交互次数。
        /// 以下参数检索单个窗口排列设置：
        /// SPI_GETDOCKMOVING
        /// SPI_GETMOUSEDOCKTHRESHOLD
        /// SPI_GETMOUSEDRAGOUTTHRESHOLD
        /// SPI_GETMOUSESIDEMOVETHRESHOLD
        /// SPI_GETPENDOCKTHRESHOLD
        /// SPI_GETPENDRAGOUTTHRESHOLD
        /// SPI_GETPENSIDEMOVETHRESHOLD
        /// SPI_GETSNAPSIZING
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETWINARRANGING = 130,

        /// <summary>
        /// 设置是否启用窗口排列。 对于 on，将 pvParam 设置为 TRUE ，将 FALSE 设置为 off。
        /// 窗口排列方式通过简化拖动或调整窗口大小时的默认行为，减少了移动顶级窗口和调整窗口大小所需的鼠标、笔或触摸交互次数。
        /// 以下参数设置单个窗口排列设置：
        /// SPI_SETDOCKMOVING
        /// SPI_SETMOUSEDOCKTHRESHOLD
        /// SPI_SETMOUSEDRAGOUTTHRESHOLD
        /// SPI_SETMOUSESIDEMOVETHRESHOLD
        /// SPI_SETPENDOCKTHRESHOLD
        /// SPI_SETPENDRAGOUTTHRESHOLD
        /// SPI_SETPENSIDEMOVETHRESHOLD
        /// SPI_SETSNAPSIZING
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETWINARRANGING = 131,

        /// <summary>
        /// 检索使用鼠标将窗口从监视器边缘或监视器数组向中心拖动来触发取消停靠行为的阈值（以像素为单位）。 默认阈值为 20。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETMOUSEDRAGOUTTHRESHOLD = 132,

        /// <summary>
        /// 设置使用鼠标将窗口从监视器或监视器阵列边缘拖动到其中心时触发取消停靠行为的阈值（以像素为单位）。 默认阈值为 20。 pvParam 参数必须指向包含新值的 DWORD 变量。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETMOUSEDRAGOUTTHRESHOLD = 133,

        /// <summary>
        /// 检索使用笔将窗口从监视器或监视器阵列的边缘拖动到其中心，从而触发取消停靠行为的阈值（以像素为单位）。 默认阈值为 30。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETPENDRAGOUTTHRESHOLD = 134,

        /// <summary>
        /// 设置使用笔将窗口从监视器或监视器阵列边缘拖动到其中心时触发取消停靠行为的阈值（以像素为单位）。 默认阈值为 30。 pvParam 参数必须指向包含新值的 DWORD 变量。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETPENDRAGOUTTHRESHOLD = 135,

        /// <summary>
        /// 从监视器或监视器数组的顶部检索阈值（以像素为单位），其中使用鼠标拖动时将还原垂直最大化的窗口。 默认阈值为 50。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETMOUSESIDEMOVETHRESHOLD = 136,

        /// <summary>
        /// 设置从监视器顶部开始的阈值（以像素为单位），在使用鼠标拖动时，垂直最大化窗口将还原到该窗口。 默认阈值为 50。 pvParam 参数必须指向包含新值的 DWORD 变量。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETMOUSESIDEMOVETHRESHOLD = 137,

        /// <summary>
        /// 检索监视器或监视器数组顶部的阈值（以像素为单位），其中使用鼠标拖动时将还原垂直最大化的窗口。 默认阈值为 50。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETPENSIDEMOVETHRESHOLD = 138,

        /// <summary>
        /// 设置监视器顶部的阈值（以像素为单位），在使用笔拖动时，垂直最大化窗口将还原到该窗口。 默认阈值为 50。 pvParam 参数必须指向包含新值的 DWORD 变量。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETPENSIDEMOVETHRESHOLD = 139,

        /// <summary>
        /// 确定拖动标题栏时是否还原最大化窗口。 pvParam 参数必须指向一个 BOOL 变量（如果启用）接收 TRUE;否则，该变量必须指向 FALSE。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETDRAGFROMMAXIMIZE = 140,

        /// <summary>
        /// 设置拖动最大化窗口标题栏时是否还原其标题栏。 对于 on，将 pvParam 设置为 TRUE ，将 FALSE 设置为 off。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETDRAGFROMMAXIMIZE = 141,

        /// <summary>
        /// 确定窗口在调整到监视器或监视器数组的顶部或底部时是否垂直最大化。 pvParam 参数必须指向一个 BOOL 变量（如果启用）接收 TRUE;否则，该变量必须指向 FALSE。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETSNAPSIZING = 142,

        /// <summary>
        /// 设置窗口在调整到监视器的顶部或底部时是垂直最大化的。 对于 on，将 pvParam 设置为 TRUE ，将 FALSE 设置为 off。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETSNAPSIZING = 143,

        /// <summary>
        /// 确定将窗口移动到监视器或监视器数组的上边缘、左边缘或右边缘时，窗口是否停靠。 pvParam 参数必须指向一个 BOOL 变量（如果启用）接收 TRUE;否则，该变量必须指向 FALSE。
        /// 使用 SPI_GETWINARRANGING 确定是否启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETDOCKMOVING = 144,

        /// <summary>
        /// 设置当窗口移动到监视器或监视器阵列上的顶部、左侧或右侧停靠目标时，窗口是停靠的。 对于 on，将 pvParam 设置为 TRUE ，将 FALSE 设置为 off。
        /// SPI_GETWINARRANGING 必须为 TRUE 才能启用此行为。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETDOCKMOVING = 145,

        SPI_GETTOUCHPREDICTIONPARAMETERS = 156,
        SPI_SETTOUCHPREDICTIONPARAMETERS = 157,

        /// <summary>
        /// 检索一个值，该值确定 Windows 8 是使用硬件的默认缩放平台显示应用，还是转到下一个更高的平台。 此值基于当前“放大屏幕上的所有内容”设置，位于电脑设置的“轻松访问”部分中：1 为打开，0 为关闭。
        /// 应用可以为以下每种缩放平台提供文本和图像资源：100%、140% 和 180%。 提供针对特定规模优化的单独资源可避免因调整大小而导致的失真。 Windows 8 根据多种因素（包括屏幕大小和像素密度）确定适当的缩放平台。 选择“放大屏幕上的所有内容” (SPI_GETLOGICALDPIOVERRIDE 返回值 1) 时，Windows 将使用下一个更高平台的资源。 例如，在 Windows 确定应使用 SCALE_100_PERCENT刻度的硬件的情况下，此替代会导致 Windows 使用 SCALE_140_PERCENT 缩放值，假定它不违反其他约束。
        /// </summary>
        SPI_GETLOGICALDPIOVERRIDE = 158,

        /// <summary>
        /// 请勿使用。
        /// </summary>
        SPI_SETLOGICALDPIOVERRIDE = 159,

        SPI_GETMENURECT = 162,
        SPI_SETMENURECT = 163,

        /// <summary>
        /// 从 Windows 11 版本 24H2 开始： 检索有关精确式触摸板的详细信息，包括与触摸板相关的用户设置和系统信息。
        /// pvParam 参数必须指向TOUCHPAD_PARAMETERS结构。
        /// uiParam 参数必须指定结构的大小。
        /// 必须将 TOUCHPAD_PARAMETERS 结构中的 versionNumber 字段的值设置为所使用的结构版本的适当值。
        /// </summary>
        SPI_GETTOUCHPADPARAMETERS = 174,

        /// <summary>
        /// 从 Windows 11 版本 24H2 开始： 设置有关精确式触摸板的详细信息，包括与触摸板相关的用户设置和系统信息。
        /// pvParam 参数必须指向TOUCHPAD_PARAMETERS结构。
        /// uiParam 参数必须指定结构的大小。
        /// 必须将 TOUCHPAD_PARAMETERS 结构中的 versionNumber 字段的值设置为所使用的结构版本的适当值。
        /// </summary>
        SPI_SETTOUCHPADPARAMETERS = 175,

        /// <summary>
        /// 确定活动窗口跟踪 (激活鼠标打开的窗口，) 打开还是关闭。 pvParam 参数必须指向一个 BOOL 变量，该变量对于 on 接收 TRUE，对于 off 接收 FALSE。
        /// </summary>
        SPI_GETACTIVEWINDOWTRACKING = 4096,

        /// <summary>
        /// 设置活动窗口跟踪 (激活鼠标打开的窗口) 打开或关闭。 对于 on，将 pvParam 设置为 TRUE ，将 FALSE 设置为 off。
        /// </summary>
        SPI_SETACTIVEWINDOWTRACKING = 4097,

        /// <summary>
        /// 确定是否启用菜单动画功能。 必须打开此主控开关才能启用菜单动画效果。 pvParam 参数必须指向一个 BOOL 变量，该变量在启用动画时接收 TRUE;如果禁用动画，则为 FALSE。
        /// 如果启用了动画， SPI_GETMENUFADE 指示菜单是使用淡出动画还是幻灯片动画。
        /// </summary>
        SPI_GETMENUANIMATION = 4098,

        /// <summary>
        /// 启用或禁用菜单动画。 此主控开关必须处于打开状态，才能发生任何菜单动画。 pvParam 参数是 BOOL 变量;将 pvParam 设置为 TRUE 以启用动画，将 FALSE 设置为禁用动画。
        /// 如果启用了动画， SPI_GETMENUFADE 指示菜单是使用淡出动画还是幻灯片动画。
        /// </summary>
        SPI_SETMENUANIMATION = 4099,

        /// <summary>
        /// 确定是否启用组合框的滑动打开效果。 pvParam 参数必须指向一个 BOOL 变量，该变量对于 enabled 接收 TRUE，对于 disabled 接收 FALSE。
        /// </summary>
        SPI_GETCOMBOBOXANIMATION = 4100,

        /// <summary>
        /// 启用或禁用组合框的幻灯片打开效果。 将 pvParam 参数设置为 TRUE 以启用渐变效果，将 设置为 FALSE 以禁用渐变效果。
        /// </summary>
        SPI_SETCOMBOBOXANIMATION = 4101,

        /// <summary>
        /// 确定是否启用列表框的平滑滚动效果。 pvParam 参数必须指向一个 BOOL 变量，该变量对于 enabled 接收 TRUE，对于 disabled 接收 FALSE。
        /// </summary>
        SPI_GETLISTBOXSMOOTHSCROLLING = 4102,

        /// <summary>
        /// 启用或禁用列表框的平滑滚动效果。 将 pvParam 参数设置为 TRUE 以启用平滑滚动效果，或 将 FALSE 设置为禁用它。
        /// </summary>
        SPI_SETLISTBOXSMOOTHSCROLLING = 4103,

        /// <summary>
        /// 确定是否启用窗口标题栏的渐变效果。 pvParam 参数必须指向一个 BOOL 变量，该变量对于 enabled 接收 TRUE，对于 disabled 接收 FALSE。 有关渐变效果的详细信息，请参阅 GetSysColor 函数。
        /// </summary>
        SPI_GETGRADIENTCAPTIONS = 4104,

        /// <summary>
        /// 启用或禁用窗口标题栏的渐变效果。 将 pvParam 参数设置为 TRUE 以启用它，将 设置为 FALSE 以禁用它。 仅当系统的颜色深度超过 256 种颜色时，才有可能产生渐变效果。
        /// </summary>
        SPI_SETGRADIENTCAPTIONS = 4105,

        /// <summary>
        /// 确定菜单访问键是否始终带有下划线。 pvParam 参数必须指向一个 BOOL 变量，该变量在菜单访问键始终带有下划线时接收 TRUE;如果仅在键盘激活菜单时才有下划线，则为 FALSE。
        /// </summary>
        SPI_GETKEYBOARDCUES = 4106,

        /// <summary>
        /// 设置菜单访问键字母的下划线。 pvParam 参数是 BOOL 变量。 将 pvParam 设置为 TRUE 以始终为菜单访问键添加下划线，将 FALSE 设置为仅在从键盘激活菜单时为菜单访问键添加下划线。
        /// </summary>
        SPI_SETKEYBOARDCUES = 4107,

        /// <summary>
        /// 与 SPI_GETKEYBOARDCUES 相同。
        /// </summary>
        SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES,

        /// <summary>
        /// 与 SPI_SETKEYBOARDCUES 相同。
        /// </summary>
        SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES,

        /// <summary>
        /// 确定是否会将通过活动窗口跟踪激活的窗口带到顶部。 pvParam 参数必须指向一个 BOOL 变量，该变量对于 on 接收 TRUE，对于 off 接收 FALSE。
        /// </summary>
        SPI_GETACTIVEWNDTRKZORDER = 4108,

        /// <summary>
        /// 确定是否应将通过活动窗口跟踪激活的窗口带到顶部。 对于 on，将 pvParam 设置为 TRUE ，将 FALSE 设置为 off。
        /// </summary>
        SPI_SETACTIVEWNDTRKZORDER = 4109,

        /// <summary>
        /// 确定是否启用用户界面元素的热跟踪，例如菜单栏上的菜单名称。 pvParam 参数必须指向一个 BOOL 变量，该变量对于 enabled 接收 TRUE，对于 disabled 接收 FALSE。
        /// 热跟踪意味着，当光标移动到某个项上时，它将被突出显示，但未选中。 可以查询此值，以确定是否在应用程序的用户界面中使用热跟踪。
        /// </summary>
        SPI_GETHOTTRACKING = 4110,

        /// <summary>
        /// 启用或禁用用户界面元素（例如菜单栏上的菜单名称）的热跟踪。 将 pvParam 参数设置为 TRUE 以启用它，将 设置为 FALSE 以禁用它。
        /// 热跟踪意味着当光标移动到某个项上时，它将被突出显示，但未选中。
        /// </summary>
        SPI_SETHOTTRACKING = 4111,

        /// <summary>
        /// 确定是否启用菜单淡出动画。 pvParam 参数必须指向一个 BOOL 变量，该变量在启用淡出动画时接收 TRUE，在可禁用时接收 FALSE。 如果禁用淡出动画，菜单将使用幻灯片动画。 除非启用了菜单动画，否则将忽略此标志，可以使用 SPI_SETMENUANIMATION 标志执行此操作。 有关详细信息，请参阅 AnimateWindow。
        /// </summary>
        SPI_GETMENUFADE = 4114,

        /// <summary>
        /// 启用或禁用菜单淡出动画。 将 pvParam 设置为 TRUE 以启用菜单淡出效果，或 将 FALSE 设置为禁用它。 如果禁用淡出动画，菜单将使用幻灯片动画。 仅当系统的颜色深度超过 256 种颜色时，才能实现菜单淡出效果。 除非还设置了 SPI_MENUANIMATION ，否则将忽略此标志。 有关详细信息，请参阅 AnimateWindow。
        /// </summary>
        SPI_SETMENUFADE = 4115,

        /// <summary>
        /// 确定是否启用选择淡化效果。 pvParam 参数必须指向一个 BOOL 变量，如果启用，则接收 TRUE;如果禁用，则接收 FALSE。
        /// 选择淡出效果会导致用户选择的菜单项暂时停留在屏幕上，同时在菜单关闭后逐渐消失。
        /// </summary>
        SPI_GETSELECTIONFADE = 4116,

        /// <summary>
        /// 将 pvParam 设置为 TRUE 以启用选择淡化效果，将 设置为 FALSE 以禁用它。
        /// 选择淡出效果会导致用户选择的菜单项暂时停留在屏幕上，同时在菜单关闭后逐渐消失。 仅当系统的颜色深度超过 256 种颜色时，才能实现选择淡出效果。
        /// </summary>
        SPI_SETSELECTIONFADE = 4117,

        /// <summary>
        /// 确定是否启用工具提示动画。 pvParam 参数必须指向一个 BOOL 变量，如果启用，则接收 TRUE;如果禁用，则接收 FALSE。 如果启用了工具提示动画， SPI_GETTOOLTIPFADE 指示工具提示是使用淡出动画还是幻灯片动画。
        /// </summary>
        SPI_GETTOOLTIPANIMATION = 4118,

        /// <summary>
        /// 将 pvParam 设置为 TRUE 以启用工具提示动画，或 将 FALSE 设置为禁用它。 如果启用，可以使用 SPI_SETTOOLTIPFADE 指定淡出动画或幻灯片动画。
        /// </summary>
        SPI_SETTOOLTIPANIMATION = 4119,

        /// <summary>
        /// 如果启用了SPI_SETTOOLTIPANIMATION，SPI_GETTOOLTIPFADE指示工具提示动画是使用淡出效果还是幻灯片效果。 pvParam 参数必须指向一个 BOOL 变量，该变量对于淡出动画接收 TRUE或 FALSE（对于幻灯片动画）。
        /// </summary>
        SPI_GETTOOLTIPFADE = 4120,

        /// <summary>
        /// 如果启用了 SPI_SETTOOLTIPANIMATION 标志，请使用 SPI_SETTOOLTIPFADE 来指示 ToolTip 动画是使用淡化效果还是幻灯片效果。 对于淡化动画，请将 pvParam 设置为 TRUE ，将幻灯片动画设置为 FALSE 。 仅当系统的颜色深度超过 256 种颜色时，才能实现工具提示淡出效果。 有关幻灯片和淡化效果的详细信息，请参阅 AnimateWindow 函数。
        /// </summary>
        SPI_SETTOOLTIPFADE = 4121,

        /// <summary>
        /// 确定光标周围是否有阴影。 pvParam 参数必须指向一个 BOOL 变量，该变量在启用阴影时接收 TRUE;如果禁用，则为 FALSE。 仅当系统的颜色深度超过 256 种颜色时，才会显示此效果。
        /// </summary>
        SPI_GETCURSORSHADOW = 4122,

        /// <summary>
        /// 启用或禁用光标周围的阴影。 pvParam 参数是 BOOL 变量。 将 pvParam 设置为 TRUE 以启用阴影，将 FALSE 设置为禁用阴影。 仅当系统的颜色深度超过 256 种颜色时，才会显示此效果。
        /// </summary>
        SPI_SETCURSORSHADOW = 4123,

        /// <summary>
        /// 检索鼠标声纳功能的状态。 pvParam 参数必须指向一个 BOOL 变量，如果启用，则接收 TRUE，否则接收 FALSE。
        /// </summary>
        SPI_GETMOUSESONAR = 4124,

        /// <summary>
        /// 打开或关闭 Sonar 辅助功能。 当用户按下并释放 Ctrl 键时，此功能简要显示鼠标指针周围的几个同心圆。 pvParam 参数为 on 指定 TRUE，为 off 指定 FALSE。 默认值为 off。
        /// </summary>
        SPI_SETMOUSESONAR = 4125,

        /// <summary>
        /// 检索鼠标 ClickLock 功能的状态。 pvParam 参数必须指向接收 TRUE（如果启用）的 BOOL 变量;否则，该变量必须指向 FALSE。
        /// </summary>
        SPI_GETMOUSECLICKLOCK = 4126,

        /// <summary>
        /// 打开或关闭鼠标 ClickLock 辅助功能。 当单击鼠标主按钮并按住 SPI_SETMOUSECLICKLOCKTIME指定的时间时，此功能会暂时锁定该按钮。 pvParam 参数为 on 指定 TRUE，为 off 指定 FALSE。 默认值为 off。 有关详细信息，请参阅备注和 AboutMouse 输入。
        /// </summary>
        SPI_SETMOUSECLICKLOCK = 4127,

        /// <summary>
        /// 检索鼠标消失功能的状态。 pvParam 参数必须指向一个 BOOL 变量，如果启用，则接收 TRUE，否则接收 FALSE。
        /// </summary>
        SPI_GETMOUSEVANISH = 4128,

        /// <summary>
        /// 打开或关闭“消失”功能。 此功能在用户键入时隐藏鼠标指针;当用户移动鼠标时，指针再次出现。 pvParam 参数为 on 指定 TRUE，为 off 指定 FALSE。 默认值为 off。
        /// </summary>
        SPI_SETMOUSEVANISH = 4129,

        /// <summary>
        /// 确定本机用户菜单是否具有平面菜单外观。 pvParam 参数必须指向一个 BOOL 变量，如果设置了平面菜单外观，则返回 TRUE，否则返回 FALSE。
        /// </summary>
        SPI_GETFLATMENU = 4130,

        /// <summary>
        /// 启用或禁用本机用户菜单的平面菜单外观。 将 pvParam 设置为 TRUE 可启用平面菜单外观，将 设置为 FALSE 以禁用它。
        /// 启用后，菜单栏将 COLOR_MENUBAR 用于菜单栏背景， COLOR_MENU 用于菜单弹出背景， COLOR_MENUHILIGHT 填充当前菜单选择， COLOR_HILIGHT 当前菜单选择的轮廓。 如果禁用，则使用与 Windows 2000 中相同的指标和颜色绘制菜单。
        /// </summary>
        SPI_SETFLATMENU = 4131,

        /// <summary>
        /// 确定是否启用投影效果。 pvParam 参数必须指向一个 BOOL 变量，如果启用，则返回 TRUE;如果禁用，则返回 FALSE。
        /// </summary>
        SPI_GETDROPSHADOW = 4132,

        /// <summary>
        /// 启用或禁用投影效果。 将 pvParam 设置为 TRUE 可启用投影效果，将 设置为 FALSE 以禁用它。 还必须在窗口类样式中 CS_DROPSHADOW 。
        /// </summary>
        SPI_SETDROPSHADOW = 4133,

        /// <summary>
        /// 检索 BOOL ，指示应用程序是否可以通过调用 SendInput 函数来模拟键盘或鼠标输入来重置屏幕保护程序计时器。 pvParam 参数必须指向一个 BOOL 变量，如果模拟输入将被阻止，则接收 TRUE，否则为 FALSE。
        /// </summary>
        SPI_GETBLOCKSENDINPUTRESETS = 4134,

        /// <summary>
        /// 通过调用 SendInput 函数来模拟键盘或鼠标输入，确定应用程序是否可以重置屏幕保护程序计时器。 如果屏幕保护程序不会通过模拟输入停用， 则 uiParam 参数指定 TRUE ;如果屏幕保护程序将由模拟输入停用，则指定 FALSE 。
        /// </summary>
        SPI_SETBLOCKSENDINPUTRESETS = 4135,

        /// <summary>
        /// 确定是启用或禁用 UI 效果。 pvParam 参数必须指向一个 BOOL 变量，如果启用所有 UI 效果，则接收 TRUE;如果禁用，则为 FALSE。
        /// </summary>
        SPI_GETUIEFFECTS = 4158,

        /// <summary>
        /// 启用或禁用 UI 效果。 将 pvParam 参数设置为 TRUE 可启用所有 UI 效果，将 FALSE 设置为禁用所有 UI 效果。
        /// </summary>
        SPI_SETUIEFFECTS = 4159,

        /// <summary>
        /// 确定是启用或禁用重叠内容。 pvParam 参数必须指向接收 TRUE（如果启用）的 BOOL 变量;否则，该变量必须指向 FALSE。
        /// 背景图像、纹理背景、文档上的水印、alpha 混合和透明度等显示功能会降低前景和背景之间的对比度，使视力不佳的用户更难在屏幕上看到对象。 此标志使你能够确定此类重叠内容是否已禁用。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETDISABLEOVERLAPPEDCONTENT = 4160,

        /// <summary>
        /// 打开或关闭重叠的内容 (，例如背景图像和水印) 。 pvParam 参数是 BOOL 变量。 将 pvParam 设置为 TRUE 可禁用重叠内容，将 FALSE 设置为启用重叠内容。
        /// 背景图像、纹理背景、文档上的水印、alpha 混合和透明度等显示功能会降低前景和背景之间的对比度，使视力不佳的用户更难在屏幕上看到对象。 通过此标志，可以启用或禁用所有此类重叠内容。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETDISABLEOVERLAPPEDCONTENT = 4161,

        /// <summary>
        /// 确定是启用或禁用动画。 pvParam 参数必须指向接收 TRUE（如果启用动画）的 BOOL 变量;否则，该变量必须指向 FALSE。
        /// 显示功能（如闪烁、闪烁、闪烁和移动内容）可能会导致照片敏感型癫痫用户癫痫发作。 通过此标志，可以确定是否在工作区中禁用了此类动画。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETCLIENTAREAANIMATION = 4162,

        /// <summary>
        /// 打开或关闭工作区动画。 pvParam 参数是 BOOL 变量。 将 pvParam 设置为 TRUE 以在工作区中启用动画和其他暂时性效果，或 将 FALSE 设置为禁用它们。
        /// 显示功能（如闪烁、闪烁、闪烁和移动内容）可能会导致照片敏感型癫痫用户癫痫发作。 使用此标志可以启用或禁用所有此类动画。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETCLIENTAREAANIMATION = 4163,

        /// <summary>
        /// 确定是否启用 ClearType。 pvParam 参数必须指向一个 BOOL 变量，如果启用 ClearType，则接收 TRUE;否则，该变量必须指向 FALSE。
        /// ClearType 是一种软件技术，可提高液晶显示器 (液晶显示器) 显示器上文本的可读性。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETCLEARTYPE = 4168,

        /// <summary>
        /// 打开或关闭 ClearType。 pvParam 参数是 BOOL 变量。 将 pvParam 设置为 TRUE 以启用 ClearType，或 将 FALSE 设置为禁用它。
        /// ClearType 是一种软件技术，可提高 LCD 监视器上文本的可读性。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETCLEARTYPE = 4169,

        SPI_GETSPEECHRECOGNITION = 4170,
        SPI_SETSPEECHRECOGNITION = 4171,
        SPI_GETCARETBROWSING = 4172,
        SPI_SETCARETBROWSING = 4173,

        /// <summary>
        /// 从 Windows 8 开始： 确定活动输入设置是否具有每个线程的本地 (、 TRUE) 或全局 (会话、 FALSE) 范围。 pvParam 参数必须指向 BOOL 变量。
        /// </summary>
        SPI_GETTHREADLOCALINPUTSETTINGS = 4174,

        /// <summary>
        /// 从 Windows 8 开始： 确定活动输入设置是否具有每个线程的本地 (、 TRUE) 或全局 (会话、 FALSE) 范围。 pvParam 参数必须是由 PVOID 强制转换的 BOOL 变量。
        /// </summary>
        SPI_SETTHREADLOCALINPUTSETTINGS = 4175,

        /// <summary>
        /// 从 Windows 8 开始： 确定是启用或禁用系统语言栏。 如果启用语言栏，pvParam 参数必须指向接收 TRUE 的 BOOL 变量;否则，该变量必须指向 FALSE。
        /// </summary>
        SPI_GETSYSTEMLANGUAGEBAR = 4176,

        /// <summary>
        /// 从 Windows 8 开始： 打开或关闭旧语言栏功能。 pvParam 参数是指向 BOOL 变量的指针。 将 pvParam 设置为 TRUE 以启用旧语言栏，或 将 FALSE 设置为禁用它。 标志在 Windows 8 上受支持，其中旧语言栏被输入切换器替换，因此默认处于关闭状态。 打开旧语言栏是出于兼容性原因而提供的，对输入切换器没有影响。
        /// </summary>
        SPI_SETSYSTEMLANGUAGEBAR = 4177,

        /// <summary>
        /// 检索用户输入后的时间（以毫秒为单位），在此期间，系统不允许应用程序强制自己进入前台。 pvParam 参数必须指向接收时间的 DWORD 变量。
        /// </summary>
        SPI_GETFOREGROUNDLOCKTIMEOUT = 8192,

        /// <summary>
        /// 设置用户输入后的时间（以毫秒为单位），在此期间，系统不允许应用程序强制自己进入前台。 将 pvParam 设置为新的超时值。
        /// 调用线程必须能够更改前台窗口，否则调用将失败。
        /// </summary>
        SPI_SETFOREGROUNDLOCKTIMEOUT = 8193,

        /// <summary>
        /// 检索活动窗口跟踪延迟（以毫秒为单位）。 pvParam 参数必须指向接收时间的 DWORD 变量。
        /// </summary>
        SPI_GETACTIVEWNDTRKTIMEOUT = 8194,

        /// <summary>
        /// 设置活动窗口跟踪延迟。 将 pvParam 设置为激活鼠标指针下的窗口之前要延迟的毫秒数。
        /// </summary>
        SPI_SETACTIVEWNDTRKTIMEOUT = 8195,

        /// <summary>
        /// 检索 SetForegroundWindow 在拒绝前台切换请求时闪烁任务栏按钮的次数。 pvParam 参数必须指向接收值的 DWORD 变量。
        /// </summary>
        SPI_GETFOREGROUNDFLASHCOUNT = 8196,

        /// <summary>
        /// 设置 SetForegroundWindow 在拒绝前台切换请求时闪烁任务栏按钮的次数。 将 pvParam 设置为闪烁的次数。
        /// </summary>
        SPI_SETFOREGROUNDFLASHCOUNT = 8197,

        /// <summary>
        /// 检索编辑控件中的插入点宽度（以像素为单位）。 pvParam 参数必须指向接收此值的 DWORD 变量。
        /// </summary>
        SPI_GETCARETWIDTH = 8198,

        /// <summary>
        /// 设置编辑控件中的插入点宽度。 将 pvParam 设置为所需的宽度（以像素为单位）。 默认值和最小值为 1。
        /// </summary>
        SPI_SETCARETWIDTH = 8199,

        /// <summary>
        /// 检索锁定主鼠标按钮之前的时间延迟。 pvParam 参数必须指向接收时间延迟（以毫秒为单位）的 DWORD。 仅当 SPI_SETMOUSECLICKLOCK 设置为 TRUE 时，才会启用此功能。
        /// </summary>
        SPI_GETMOUSECLICKLOCKTIME = 8200,

        /// <summary>
        /// 调整锁定主鼠标按钮之前的时间延迟。 uiParam 参数应设置为 0。 pvParam 参数指向指定以毫秒为单位的时间延迟的 DWORD。 例如，为 1 秒延迟指定 1000。 默认值为 1200。
        /// </summary>
        SPI_SETMOUSECLICKLOCKTIME = 8201,

        /// <summary>
        /// 检索字体平滑的类型。 pvParam 参数必须指向接收信息的 UINT。 可能的值为 FE_FONTSMOOTHINGSTANDARD 和 FE_FONTSMOOTHINGCLEARTYPE。
        /// </summary>
        SPI_GETFONTSMOOTHINGTYPE = 8202,

        /// <summary>
        /// 设置字体平滑类型。 如果使用标准抗锯齿，则 pvParam 参数为FE_FONTSMOOTHINGSTANDARD，如果使用 ClearType，则为FE_FONTSMOOTHINGCLEARTYPE。 默认值为 FE_FONTSMOOTHINGSTANDARD。
        /// 还必须设置SPI_SETFONTSMOOTHING 。
        /// </summary>
        SPI_SETFONTSMOOTHINGTYPE = 8203,

        /// <summary>
        /// 检索 在 ClearType 平滑处理中使用的对比度值。 pvParam 参数必须指向接收信息的 UINT。 有效的对比度值为 1000 到 2200。 默认值为 1400。
        /// </summary>
        SPI_GETFONTSMOOTHINGCONTRAST = 8204,

        /// <summary>
        /// 设置 ClearType 平滑中使用的对比度值。 pvParam 参数是对比度值。 有效的对比度值为 1000 到 2200。 默认值为 1400。
        /// SPI_SETFONTSMOOTHINGTYPE 还必须设置为 FE_FONTSMOOTHINGCLEARTYPE。
        /// </summary>
        SPI_SETFONTSMOOTHINGCONTRAST = 8205,

        /// <summary>
        /// 检索使用 DrawFocusRect 绘制的焦点矩形的左边缘和右边缘的宽度（以像素为单位）。 pvParam 参数必须指向 UINT。
        /// </summary>
        SPI_GETFOCUSBORDERWIDTH = 8206,

        /// <summary>
        /// 将使用 DrawFocusRect 绘制的焦点矩形的左边缘和右边缘的高度设置为 pvParam 参数的值。
        /// </summary>
        SPI_SETFOCUSBORDERWIDTH = 8207,

        /// <summary>
        /// 检索使用 DrawFocusRect 绘制的焦点矩形的上边缘和下边缘的高度（以像素为单位）。 pvParam 参数必须指向 UINT 值。
        /// </summary>
        SPI_GETFOCUSBORDERHEIGHT = 8208,

        /// <summary>
        /// 将使用 DrawFocusRect 绘制的焦点矩形的上边缘和下边缘的高度设置为 pvParam 参数的值。
        /// </summary>
        SPI_SETFOCUSBORDERHEIGHT = 8209,

        /// <summary>
        /// 检索字体平滑方向。 pvParam 参数必须指向接收信息的 UINT。 可能 的值FE_FONTSMOOTHINGORIENTATIONBGR ( 蓝-绿-红) 和 FE_FONTSMOOTHINGORIENTATIONRGB (红-绿-蓝) 。
        /// </summary>
        SPI_GETFONTSMOOTHINGORIENTATION = 8210,

        /// <summary>
        /// 设置字体平滑方向。 pvParam 参数FE_FONTSMOOTHINGORIENTATIONBGR (蓝-绿-红) 或FE_FONTSMOOTHINGORIENTATIONRGB (红-绿-蓝) 。
        /// </summary>
        SPI_SETFONTSMOOTHINGORIENTATION = 8211,

        SPI_GETMINIMUMHITRADIUS = 8212,
        SPI_SETMINIMUMHITRADIUS = 8213,

        /// <summary>
        /// 检索应显示通知弹出窗口的时间（以秒为单位）。 pvParam 参数必须指向接收消息持续时间的 ULONG。
        /// 有视觉障碍或认知障碍（如 ADHD 和阅读障碍）的用户可能需要更长的时间才能阅读通知消息中的文本。 使用此标志可以检索消息持续时间。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_GETMESSAGEDURATION = 8214,

        /// <summary>
        /// 设置通知弹出窗口应显示的时间（以秒为单位）。 pvParam 参数指定消息持续时间。
        /// 有视觉障碍或认知障碍（如 ADHD 和阅读障碍）的用户可能需要更长的时间才能阅读通知消息中的文本。 使用此标志可以设置消息持续时间。
        /// Windows Server 2003 和 Windows XP/2000： 不支持此参数。
        /// </summary>
        SPI_SETMESSAGEDURATION = 8215,

        /// <summary>
        /// 检索当前联系人可视化设置。 pvParam 参数必须指向接收设置的 ULONG 变量。 有关详细信息，请参阅 联系人可视化。
        /// </summary>
        SPI_GETCONTACTVISUALIZATION = 8216,

        /// <summary>
        /// 设置当前联系人可视化设置。 pvParam 参数必须指向标识设置的 ULONG 变量。
        /// </summary>
        SPI_SETCONTACTVISUALIZATION = 8217,

        /// <summary>
        /// 检索当前手势可视化设置。 pvParam 参数必须指向接收设置的 ULONG 变量。 有关详细信息，请参阅 手势可视化。
        /// </summary>
        SPI_GETGESTUREVISUALIZATION = 8218,

        /// <summary>
        /// 设置当前手势可视化效果设置。 pvParam 参数必须指向标识设置的 ULONG 变量。
        /// </summary>
        SPI_SETGESTUREVISUALIZATION = 8219,

        /// <summary>
        /// 检索滚轮按钮输入的路由设置。 路由设置确定是将滚轮按钮输入发送到具有焦点 (前台) 的应用，还是将鼠标光标下的应用发送到应用。
        /// pvParam 参数必须指向接收路由选项的 DWORD 变量。 如果值为零或MOUSEWHEEL_ROUTING_FOCUS，则鼠标滚轮输入将传送到具有焦点的应用。 如果值为 1 或 MOUSEWHEEL_ROUTING_HYBRID (默认) ，则鼠标滚轮输入会传送到具有焦点的应用， (桌面应用) 或鼠标光标下的应用 (Windows 应用商店应用) 。 不使用 uiParam 参数。
        /// </summary>
        SPI_GETMOUSEWHEELROUTING = 8220,

        /// <summary>
        /// 设置鼠标滚轮输入的路由设置。 路由设置确定是将鼠标滚轮输入发送到具有焦点 (前台) 的应用，还是将鼠标光标下的应用发送到应用。
        /// pvParam 参数必须指向接收路由选项的 DWORD 变量。 将 uiParam 参数设置为零。
        /// 如果值为零 (MOUSEWHEEL_ROUTING_FOCUS) ，则鼠标滚轮输入将传送到具有焦点的应用。 如果值为 1 (MOUSEWHEEL_ROUTING_HYBRID) ，鼠标滚轮输入将传送到具有焦点的应用 (桌面应用) 或鼠标指针下的应用 (Windows 应用商店应用) 。
        /// 从 Windows 10 开始： 如果值为 2 (MOUSEWHEEL_ROUTING_MOUSE_POS) ，则鼠标滚轮输入将传送到鼠标指针下的应用。 这是新的默认值，MOUSEWHEEL_ROUTING_HYBRID在“设置”中不再可用。
        /// </summary>
        SPI_SETMOUSEWHEELROUTING = 8221,

        /// <summary>
        /// 检索当前笔手势可视化设置。 pvParam 参数必须指向接收设置的 ULONG 变量。
        /// </summary>
        SPI_GETPENVISUALIZATION = 8222,

        /// <summary>
        /// 设置当前笔手势可视化设置。 pvParam 参数必须指向标识设置的 ULONG 变量。
        /// </summary>
        SPI_SETPENVISUALIZATION = 8223,

        SPI_GETPENARBITRATIONTYPE = 8224,
        SPI_SETPENARBITRATIONTYPE = 8225,
        SPI_GETCARETTIMEOUT = 8226,
        SPI_SETCARETTIMEOUT = 8227,
        SPI_GETHANDEDNESS = 8228,
        SPI_SETHANDEDNESS = 8229,
    }
}
