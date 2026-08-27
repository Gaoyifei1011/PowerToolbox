using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 描述适配器的本地标识符。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LUID
    {
        /// <summary>
        /// 指定包含 ID 的无符号小号的 DWORD。
        /// </summary>
        internal uint LowPart;

        /// <summary>
        /// 指定包含 ID 的有符号高号的 LONG。
        /// </summary>
        internal int HighPart;

        internal readonly long Value => (long)(((ulong)HighPart) << 32 | LowPart);

        public override readonly string ToString()
        {
            return Value.ToString();
        }
    }
}
