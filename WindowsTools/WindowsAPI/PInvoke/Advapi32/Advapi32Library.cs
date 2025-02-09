﻿using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// Advapi32.dll 函数库
    /// </summary>
    public static class Advapi32Library
    {
        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// AdjustTokenPrivileges 函数启用或禁用指定访问令牌中的特权。 启用或禁用访问令牌中的特权需要TOKEN_ADJUST_PRIVILEGES访问权限。
        /// </summary>
        /// <param name="TokenHandle">包含要修改的权限的访问令牌的句柄。 句柄必须具有TOKEN_ADJUST_PRIVILEGES令牌的访问权限。 如果 PreviousState 参数不为 NULL，则句柄还必须具有TOKEN_QUERY访问权限。</param>
        /// <param name="DisableAllPrivileges">指定函数是否禁用令牌的所有特权。 如果此值为 TRUE，则函数将禁用所有特权并忽略 NewState 参数。 如果为 FALSE，则函数根据 NewState 参数指向的信息修改权限。</param>
        /// <param name="NewState">指向 TOKEN_PRIVILEGES 结构的指针，该结构指定特权及其属性的数组。 如果 DisableAllPrivileges 参数为 FALSE， 则 AdjustTokenPrivileges 函数将启用、禁用或删除令牌的这些特权。 下表描述了 AdjustTokenPrivileges 函数基于特权属性执行的操作。</param>
        /// <param name="BufferLength">指定 PreviousState 参数指向的缓冲区的大小（以字节为单位）。 如果 PreviousState 参数为 NULL，则此参数可以为 零。</param>
        /// <param name="PreviousState">
        /// 指向缓冲区的指针，该缓冲区由 函数填充TOKEN_PRIVILEGES 结构，该结构包含函数修改的任何特权的先前状态。 也就是说，如果此函数修改了权限，则权限及其以前的状态包含在 PreviousState 引用的TOKEN_PRIVILEGES结构中。 如果 TOKEN_PRIVILEGES 的 PrivilegeCount 成员为零，则此函数未更改任何特权。 此参数可以为 NULL。
        /// 如果指定的缓冲区太小，无法接收修改的权限的完整列表，则函数将失败，并且不会调整任何特权。 在这种情况下， 函数将 ReturnLength 参数指向的变量设置为保存修改的权限的完整列表所需的字节数。
        /// </param>
        /// <param name="ReturnLength">指向变量的指针，该变量接收 PreviousState 参数指向的缓冲区的所需大小（以字节为单位）。 如果 PreviousState 为 NULL，此参数可以为 NULL。</param>
        /// <returns>如果该函数成功，则返回值为非零值。</returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, EntryPoint = "AdjustTokenPrivileges", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

        /// <summary>
        /// 启动指定计算机的关闭和可选重启，并选择性地记录关闭的原因。
        /// </summary>
        /// <param name="lpMachineName">要关闭的计算机的网络名称。 如果 lpMachineNameNULL 或空字符串，则该函数将关闭本地计算机。</param>
        /// <param name="lpMessage">要显示在关闭对话框中的消息。 如果没有消息，则可以 NULL 此参数。</param>
        /// <param name="dwTimeout">
        /// 应显示关闭对话框的时间长度（以秒为单位）。 显示此对话框时，AbortSystemShutdown 函数可以停止关闭。
        /// 如果 dwTimeout 不为零，InitiateSystemShutdownEx 在指定计算机上显示对话框。 该对话框显示调用函数的用户的名称，显示由 lpMessage 参数指定的消息，并提示用户注销。 创建对话框时会发出蜂鸣声，并保留在系统中的其他窗口之上。 对话框可以移动，但不能关闭。 计时器在关闭前将倒计时剩余时间。
        /// 如果 dwTimeout 为零，则计算机关闭而不显示对话框，AbortSystemShutdown无法停止关闭。
        /// </param>
        /// <param name="bForceAppsClosed">如果此参数 TRUE，则具有未保存更改的应用程序将被强行关闭。 如果此参数 FALSE，系统将显示一个对话框，指示用户关闭应用程序。</param>
        /// <param name="bRebootAfterShutdown">如果此参数 TRUE，则计算机在关闭后立即重启。 如果此参数 FALSE，系统会将所有缓存刷新到磁盘并安全地关闭系统。</param>
        /// <param name="dwReason">
        /// 启动关闭的原因。 此参数必须是系统关闭原因代码之一。
        /// 如果此参数为零，则默认值为未定义的关闭，该关闭记录为“找不到此原因的标题”。 默认情况下，它也是计划外关闭。 根据系统的配置方式，计划外关闭会触发创建包含系统状态信息的文件，该文件可能会延迟关闭。 因此，不要对此参数使用零。
        /// </param>
        /// <returns>如果函数成功，则返回值为非零。如果函数失败，则返回值为零。</returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, EntryPoint = "InitiateSystemShutdownExW", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InitiateSystemShutdownEx([MarshalAs(UnmanagedType.LPWStr)] string lpMachineName, [MarshalAs(UnmanagedType.LPWStr)] string lpMessage, uint dwTimeout, [MarshalAs(UnmanagedType.Bool)] bool bForceAppsClosed, [MarshalAs(UnmanagedType.Bool)] bool bRebootAfterShutdown, SHTDN_REASON dwReason);

        /// <summary>
        /// LookupPrivilegeValue 函数检索指定系统上用于本地表示指定特权名称的 本地唯一标识符（LUID）。
        /// </summary>
        /// <param name="lpSystemName">指向以 null 结尾的字符串的指针，该字符串指定检索特权名称的系统的名称。 如果指定了 null 字符串，则该函数将尝试在本地系统上查找特权名称。</param>
        /// <param name="lpName">指向以 null 结尾的字符串的指针，该字符串指定 Winnt.h 头文件中定义的特权名称。 例如，此参数可以指定常量、SE_SECURITY_NAME或其相应的字符串“SeSecurityPrivilege”。</param>
        /// <param name="lpLuid">指向接收 LUID 的变量的指针，该变量在 lpSystemName 参数指定的系统上已知特权。</param>
        /// <returns>如果函数成功，该函数将返回非零。如果函数失败，则返回零。 </returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue([MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPWStr)] string lpName, out LUID lpLuid);

        /// <summary>
        /// OpenProcessToken 函数打开与进程关联的访问令牌。
        /// </summary>
        /// <param name="processHandle">打开其访问令牌的进程句柄。 此过程必须具有PROCESS_QUERY_LIMITED_INFORMATION访问权限。 有关详细信息，请参阅 进程安全和访问权限 。</param>
        /// <param name="desiredAccess">指定一个访问掩码，该掩码 指定对访问令牌的请求访问类型。 这些请求的访问类型与令牌 (DACL) 的自由访问控制列表 进行比较，以确定授予或拒绝哪些访问权限。</param>
        /// <param name="tokenHandle">指向在函数返回时标识新打开的访问令牌的句柄的指针。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, EntryPoint = "OpenProcessToken", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        /// <summary>
        /// 通知调用方对指定注册表项的属性或内容的更改。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。</param>
        /// <param name="watchSubtree">如果此参数为 TRUE，则函数将报告指定键及其子项中的更改。 如果参数为 FALSE，则函数仅报告指定键中的更改。</param>
        /// <param name="notifyFilter">一个值，该值指示应报告的更改。</param>
        /// <param name="hEvent">事件的句柄。 如果 fAsynchronous 参数为 TRUE，则函数将立即返回 ，并通过发出此事件信号来报告更改。 如果 fAsynchronous 为 FALSE，则忽略 hEvent 。</param>
        /// <param name="asynchronous">如果此参数为 TRUE，则函数将立即返回并通过向指定事件发出信号来报告更改。 如果此参数为 FALSE，则函数在发生更改之前不会返回 。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "RegNotifyChangeKeyValue", PreserveSig = true, SetLastError = false)]
        public static extern int RegNotifyChangeKeyValue(IntPtr hKey, [MarshalAs(UnmanagedType.Bool)] bool watchSubtree, REG_NOTIFY_FILTER notifyFilter, IntPtr hEvent, [MarshalAs(UnmanagedType.Bool)] bool asynchronous);
    }
}
