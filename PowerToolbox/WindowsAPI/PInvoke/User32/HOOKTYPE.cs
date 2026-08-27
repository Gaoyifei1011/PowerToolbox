namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 要安装的挂钩过程的类型。
    /// </summary>
    internal enum HOOKTYPE
    {
        /// <summary>
        /// 安装挂钩过程，用于监视由于对话框、消息框、菜单或滚动条中的输入事件而生成的消息。 有关详细信息，请参阅 MessageProc 挂钩过程。
        /// </summary>
        WH_MSGFILTER = -1,

        /// <summary>
        /// 安装一个挂钩过程，用于记录发布到系统消息队列的输入消息。 此挂钩可用于记录宏。 有关详细信息，请参阅 JournalRecordProc 挂钩过程。
        /// </summary>
        WH_JOURNALRECORD = 0,

        /// <summary>
        /// 安装一个挂钩过程，该过程发布以前由 WH_JOURNALRECORD 挂钩过程记录的消息。 有关详细信息，请参阅 JournalPlaybackProc 挂钩过程。
        /// </summary>
        WH_JOURNALPLAYBACK = 1,

        /// <summary>
        /// 安装用于监视击键消息的挂钩过程。 有关详细信息，请参阅 KeyboardProc 挂钩过程。
        /// </summary>
        WH_KEYBOARD = 2,

        /// <summary>
        /// 安装用于监视发布到消息队列的消息的挂钩过程。 有关详细信息，请参阅 GetMsgProc 挂钩过程。
        /// </summary>
        WH_GETMESSAGE = 3,

        /// <summary>
        /// 安装一个挂钩过程，用于在系统将消息发送到目标窗口过程之前监视消息。 有关详细信息，请参阅 CallWndProc 挂钩过程。
        /// </summary>
        WH_CALLWNDPROC = 4,

        /// <summary>
        /// 安装用于接收对 CBT 应用程序有用的通知的挂钩过程。 有关详细信息，请参阅 CBTProc 挂钩过程。
        /// </summary>
        WH_CBT = 5,

        /// <summary>
        /// 安装挂钩过程，用于监视由于对话框、消息框、菜单或滚动条中的输入事件而生成的消息。 挂钩过程监视与调用线程位于同一桌面中的所有应用程序的消息。 有关详细信息，请参阅 SysMsgProc 挂钩过程。
        /// </summary>
        WH_SYSMSGFILTER = 6,

        /// <summary>
        /// 安装监视鼠标消息的挂钩过程。 有关详细信息，请参阅 MouseProc 挂钩过程。
        /// </summary>
        WH_MOUSE = 7,

        /// <summary>
        /// 每当调用GetMessage或PeekMessage函数时，如果从消息队列中得到的是非鼠标和键盘消息，则调用钩子函数
        /// </summary>
        WH_HARDWARE = 8,

        /// <summary>
        /// 安装可用于调试其他挂钩过程的挂钩过程。 有关详细信息，请参阅 DebugProc 挂钩过程。
        /// </summary>
        WH_DEBUG = 9,

        /// <summary>
        /// 安装一个挂钩过程，用于接收对 shell 应用程序有用的通知。 有关详细信息，请参阅 ShellProc 挂钩过程。
        /// </summary>
        WH_SHELL = 10,

        /// <summary>
        /// 安装将在应用程序的前台线程变为空闲状态时调用的挂钩过程。 此挂钩可用于在空闲时间执行低优先级任务。 有关详细信息，请参阅 ForegroundIdleProc 挂钩过程。
        /// </summary>
        WH_FOREGROUNDIDLE = 11,

        /// <summary>
        /// 安装一个挂钩过程，该过程在目标窗口过程处理消息后监视消息。 有关详细信息，请参阅 [HOOKPROC 回调函数] (nc-winuser-hookproc.md) 挂钩过程。
        /// </summary>
        WH_CALLWNDPROCRET = 12,

        /// <summary>
        /// 安装用于监视低级别键盘输入事件的挂钩过程。 有关详细信息，请参阅 [LowLevelKeyboardProc] (/windows/win32/winmsg/lowlevelkeyboardproc) 挂钩过程。
        /// </summary>
        WH_KEYBOARD_LL = 13,

        /// <summary>
        /// 安装用于监视低级别鼠标输入事件的挂钩过程。 有关详细信息，请参阅 LowLevelMouseProc 挂钩过程。
        /// </summary>
        WH_MOUSE_LL = 14
    }
}
