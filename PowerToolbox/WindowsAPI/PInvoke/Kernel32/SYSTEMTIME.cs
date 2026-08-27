using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 指定日期和时间，使用月份、日、年、工作日、小时、分钟、秒和毫秒的单个成员。 时间采用协调世界时 (UTC) 或本地时间，具体取决于正在调用的函数。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SYSTEMTIME
    {
        /// <summary>
        /// 年。 此成员的有效值为 1601 到 30827。
        /// </summary>
        internal ushort Year;

        /// <summary>
        /// 月份。
        /// </summary>
        internal ushort Month;

        /// <summary>
        /// 星期几。
        /// </summary>
        internal ushort DayOfWeek;

        /// <summary>
        /// 每月的日期。 此成员的有效值为 1 到 31。
        /// </summary>
        internal ushort Day;

        /// <summary>
        /// 小时。 此成员的有效值为 0 到 23。
        /// </summary>
        internal ushort Hour;

        /// <summary>
        /// 分钟。 此成员的有效值为 0 到 59。
        /// </summary>
        internal ushort Minute;

        /// <summary>
        /// 秒钟。 此成员的有效值为 0 到 59。
        /// </summary>
        internal ushort Second;

        /// <summary>
        /// 毫秒。 此成员的有效值为 0 到 999。
        /// </summary>
        internal ushort Milliseconds;
    }
}
