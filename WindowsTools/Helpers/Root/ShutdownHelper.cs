﻿using System;
using WindowsTools.WindowsAPI.PInvoke.Advapi32;
using WindowsTools.WindowsAPI.PInvoke.Kernel32;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 关机重启辅助类
    /// </summary>
    public static partial class ShutdownHelper
    {
        /// <summary>
        /// 重启电脑
        /// </summary>
        public static void Restart(string message, TimeSpan timeout)
        {
            Advapi32Library.OpenProcessToken(Kernel32Library.GetCurrentProcess(), 0x00000020, out IntPtr tokenHandle);
            TOKEN_PRIVILEGES tokenPrivileges = new()
            {
                PrivilegeCount = 1,
                Attributes = 2
            };
            Advapi32Library.LookupPrivilegeValue(string.Empty, "SeShutdownPrivilege", out tokenPrivileges.Luid);
            Advapi32Library.AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);
            Advapi32Library.InitiateSystemShutdownEx(null, message, Convert.ToUInt32(timeout.TotalSeconds), false, true, SHTDN_REASON.SHTDN_REASON_MAJOR_OTHER);

            tokenPrivileges.Attributes = 0;
            Advapi32Library.AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
