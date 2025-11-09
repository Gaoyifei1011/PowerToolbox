using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含有关低级别键盘输入事件的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class KBDLLHOOKSTRUCT
    {
        /// <summary>
        /// 虚拟密钥代码。 代码必须是 1 到 254 范围内的值。
        /// </summary>
        public Keys vkCode;

        /// <summary>
        /// 密钥的硬件扫描代码。
        /// </summary>
        public int scanCode;

        /// <summary>
        /// 扩展键标志、事件注入标志、上下文代码和转换状态标志。 此成员指定如下。 应用程序可以使用以下值来测试击键标志。 测试LLKHF_INJECTED (位 4) 将告知是否已注入事件。 如果是，则测试LLKHF_LOWER_IL_INJECTED (位 1) 会告诉你事件是否是从以较低完整性级别运行的进程注入的。
        /// </summary>
        public KBDLLHOOKSTRUCTFLAGS flags;

        /// <summary>
        /// 此消息的时间戳，相当于 GetMessageTime 为此消息返回的时间戳。
        /// </summary>
        public uint time;

        /// <summary>
        /// 与消息关联的其他信息。
        /// </summary>
        public nuint dwExtraInfo;
    }
}
