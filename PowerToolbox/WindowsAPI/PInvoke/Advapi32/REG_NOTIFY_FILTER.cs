using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 该值指示应报告的更改
    /// </summary>
    [Flags]
    internal enum REG_NOTIFY_FILTER
    {
        /// <summary>
        /// 通知调用方是添加还是删除了子项。
        /// </summary>
        REG_NOTIFY_CHANGE_NAME = 0x00000001,

        /// <summary>
        /// 向调用方通知项属性（例如安全描述符信息）的更改。
        /// </summary>
        REG_NOTIFY_CHANGE_ATTRIBUTES = 0x00000002,

        /// <summary>
        /// 向调用方通知项值的更改。 这包括添加或删除值，或更改现有值。
        /// </summary>
        REG_NOTIFY_CHANGE_LAST_SET = 0x00000004,

        /// <summary>
        /// 向调用方通知项的安全描述符的更改。
        /// </summary>
        REG_NOTIFY_CHANGE_SECURITY = 0x00000008,

        /// <summary>
        /// 指示注册的生存期不得绑定到发出 RegNotifyChangeKeyValue 调用的线程的生存期。
        /// </summary>
        REG_NOTIFY_THREAD_AGNOSTIC = 0x10000000
    }
}
