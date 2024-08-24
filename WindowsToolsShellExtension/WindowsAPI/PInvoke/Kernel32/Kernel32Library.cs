﻿using System;
using System.Runtime.InteropServices;

namespace WindowsToolsShellExtension.WindowsAPI.PInvoke.Kernel32
{
    public static partial class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        /// <summary>
        /// 关闭打开的对象句柄。
        /// </summary>
        /// <param name="hObject">打开对象的有效句柄。</param>
        /// <returns>
        /// 如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。
        /// 如果应用程序在调试器下运行，则如果函数收到无效的句柄值或伪句柄值，该函数将引发异常。
        /// 如果两次关闭句柄，或者对 FindFirstFile 函数返回的句柄调用 CloseHandle，而不是调用 FindClose 函数，则可能会出现这种情况。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "CloseHandle", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 创建新进程及其主线程。 新进程在调用进程的安全上下文中运行。
        /// 如果调用进程正在模拟其他用户，则新进程将令牌用于调用进程，而不是模拟令牌。 若要在模拟令牌表示的用户的安全上下文中运行新进程，请使用 CreateProcessAsUser 或 CreateProcessWithLogonW 函数。
        /// </summary>
        /// <param name="lpApplicationName">
        /// 要执行的模块的名称。 此模块可以是基于 Windows 的应用程序。 它可以是某种其他类型的模块 (例如 MS-DOS 或 OS/2) （如果本地计算机上提供了相应的子系统）。
        /// 字符串可以指定要执行的模块的完整路径和文件名，也可以指定部分名称。 对于部分名称，函数使用当前驱动器和当前目录来完成规范。 函数不会使用搜索路径。 此参数必须包含文件扩展名;不采用默认扩展名。
        /// <param name="lpApplicationName"> 参数可以为 NULL。 在这种情况下，模块名称必须是 lpCommandLine 字符串中第一个空格分隔的标记。 如果使用包含空格的长文件名，请使用带引号的字符串来指示文件名结束和参数开始的位置;否则，文件名不明确。
        /// 如果可执行模块是 16 位应用程序， lpApplicationName 应为 NULL， lpCommandLine 指向的字符串应指定可执行模块及其参数。
        /// 若要运行批处理文件，必须启动命令解释器;将 lpApplicationName 设置为 cmd.exe 并将 lpCommandLine 设置为以下参数：/c 加上批处理文件的名称。
        /// </param>
        /// <param name="lpCommandLine">
        /// 要执行的命令行。
        /// 此字符串的最大长度为 32，767 个字符，包括 Unicode 终止 null 字符。 如果 lpApplicationName 为 NULL，则 lpCommandLine 的模块名称部分限制为 MAX_PATH 个字符。
        /// 此函数的 Unicode 版本 CreateProcessW 可以修改此字符串的内容。 因此，此参数不能是指向只读内存(的指针，例如 const 变量或文本字符串) 。 如果此参数是常量字符串，该函数可能会导致访问冲突。
        /// <param name="lpCommandLine"> 参数可以为 NULL。 在这种情况下，函数使用 lpApplicationName 指向的字符串作为命令行。
        /// 如果 lpApplicationName 和 lpCommandLine 均为非 NULL，则 lpApplicationName 指向的以 null 结尾的字符串将指定要执行的模块， 而 lpCommandLine 指向的以 null 结尾的字符串将指定命令行。 新进程可以使用 GetCommandLine 检索整个命令行。 用 C 编写的控制台进程可以使用 argc 和 argv 参数来分析命令行。 由于 argv[0] 是模块名称，因此 C 程序员通常将模块名称重复为命令行中的第一个标记。
        /// 如果 lpApplicationName 为 NULL，则命令行的第一个空格分隔标记将指定模块名称。 如果使用包含空格的长文件名，请使用带引号的字符串来指示文件名结束和参数开始的位置 (请参阅 lpApplicationName 参数) 的说明。 如果文件名不包含扩展名，则追加.exe。 因此，如果文件扩展名为 .com，则此参数必须包含 .com 扩展名。 如果文件名以不带扩展名的句点 (.) 结尾，或者文件名包含路径，则不会追加.exe。 如果文件名不包含目录路径，系统会按以下顺序搜索可执行文件：
        /// 1.从中加载应用程序的目录。
        /// 2.父进程的当前目录。
        /// 3.32 位 Windows 系统目录。 使用 GetSystemDirectory 函数获取此目录的路径。
        /// 4.16 位 Windows 系统目录。 没有获取此目录的路径的函数，但会对其进行搜索。 此目录的名称为 System。
        /// 5.Windows 目录。 使用 GetWindowsDirectory 函数获取此目录的路径。
        /// 6.PATH 环境变量中列出的目录。 请注意，此函数不会搜索应用程序路径注册表项指定的每个 应用程序 路径。 若要在搜索序列中包含此每个应用程序的路径，请使用 ShellExecute 函数。
        /// 系统向命令行字符串添加一个终止 null 字符，以将文件名与参数分开。 这会将原始字符串划分为两个字符串以供内部处理。
        /// </param>
        /// <param name="lpProcessAttributes">
        /// 指向 SECURITY_ATTRIBUTES 结构的指针，该结构确定返回的新进程对象的句柄是否可以由子进程继承。 如果 lpProcessAttributes 为 NULL，则不能继承句柄。
        /// 结构的 lpSecurityDescriptor 成员为新进程指定安全描述符。 如果 lpProcessAttributes 为 NULL 或 lpSecurityDescriptor 为 NULL，则进程将获取默认安全描述符。 进程的默认安全描述符中的 ACL 来自创建者的主令牌。Windowsxp： 进程的默认安全描述符中的 ACL 来自创建者的主要令牌或模拟令牌。 此行为随 Windows XP SP2 和 Windows Server 2003 更改。
        /// </param>
        /// <param name="lpThreadAttributes">
        /// 指向 SECURITY_ATTRIBUTES 结构的指针，该结构确定返回的新线程对象的句柄是否可以由子进程继承。 如果 lpProcessAttributes 为 NULL，则不能继承句柄。
        /// 结构的 lpSecurityDescriptor 成员指定主线程的安全描述符。 如果 lpThreadAttributes 为 NULL 或 lpSecurityDescriptor 为 NULL，则线程获取默认安全描述符。 线程的默认安全描述符中的 ACL 来自进程令牌。Windowsxp： 线程的默认安全描述符中的 ACL 来自创建者的主令牌或模拟令牌。 此行为随 Windows XP SP2 和 Windows Server 2003 更改。
        /// </param>
        /// <param name="bInheritHandles">
        /// 如果此参数为 TRUE，则调用进程中的每个可继承句柄都由新进程继承。 如果参数为 FALSE，则不继承句柄。 请注意，继承的句柄与原始句柄具有相同的值和访问权限。 有关可继承句柄的其他讨论，请参阅备注。
        /// 终端服务： 不能跨会话继承句柄。 此外，如果此参数为 TRUE，则必须在与调用方相同的会话中创建进程。
        /// 受保护的流程灯(PPL) 进程： 当 PPL 进程创建非 PPL 进程时，将阻止泛型句柄继承，因为不允许将PROCESS_DUP_HANDLE从非 PPL 进程转换为 PPL 进程。 请参阅 进程安全性和访问权限
        /// </param>
        /// <param name="dwCreationFlags">
        /// 控制优先级类和进程的创建的标志。 有关值的列表，请参阅 进程创建标志。
        /// 此参数还控制新进程的优先级类，该类用于确定进程线程的计划优先级。 有关值的列表，请参阅 GetPriorityClass。 如果未指定任何优先级类标志，则优先级类默认为 NORMAL_PRIORITY_CLASS ，除非创建过程的优先级类 IDLE_PRIORITY_CLASS 或 BELOW_NORMAL_PRIORITY_CLASS。 在这种情况下，子进程接收调用进程的默认优先级类。
        /// 如果 dwCreationFlags 参数的值为 0：
        /// 1.进程同时继承调用方和父级控制台的错误模式。
        /// 2.假定新进程的环境块包含 ANSI 字符(请参阅 lpEnvironment 参数以获取) 的其他信息。
        /// 3.基于 16 位 Windows 的应用程序在共享的虚拟 DOS 计算机中运行， (VDM) 。
        /// </param>
        /// <param name="lpEnvironment">
        /// 指向新进程的环境块的指针。 如果此参数为 NULL，则新进程使用调用进程的 环境。环境块由以 null 结尾的字符串的以 null 结尾的块组成。
        /// 每个字符串采用以下格式：名字=value\0
        /// 由于等号用作分隔符，因此不得在环境变量的名称中使用。环境块可以包含 Unicode 或 ANSI 字符。 如果 lpEnvironment 指向的环境块包含 Unicode 字符，请确保 dwCreationFlags 包含 CREATE_UNICODE_ENVIRONMENT。如果进程的环境块的总大小超过 32，767 个字符，则此函数的 ANSI 版本 CreateProcessA 将失败。
        /// 请注意，ANSI 环境块以两个零字节结尾：一个字节用于最后一个字符串，另一个用于终止该块。 Unicode 环境块以四个零字节结尾：两个作为最后一个字符串，另外两个用于终止该块。
        /// </param>
        /// <param name="lpCurrentDirectory">
        /// 进程当前目录的完整路径。 字符串还可以指定 UNC 路径。如果此参数为 NULL，则新进程将具有与调用进程相同的当前驱动器和目录。 (此功能主要用于需要启动应用程序并指定其初始驱动器和工作目录的 shell。)
        /// </param>
        /// <param name="lpStartupInfo">
        /// 指向 STARTUPINFO 或 STARTUPINFOEX 结构的指针。若要设置扩展属性，请使用 STARTUPINFOEX 结构并在 dwCreationFlags 参数中指定 EXTENDED_STARTUPINFO_PRESENT。当不再需要 STARTUPINFO 或 STARTUPINFOEX 中的句柄时，必须使用 CloseHandle 关闭它们。
        /// 调用方负责确保 STARTUPINFO 中的标准句柄字段包含有效的句柄值。 即使 dwFlags 成员指定了 STARTF_USESTDHANDLES，这些字段也保持不变地复制到子进程，而无需验证。 不正确的值可能导致子进程行为不端或崩溃。 使用应用程序验证程序运行时验证工具检测无效句柄。
        /// </param>
        /// <param name="lpProcessInformation">
        /// 指向 PROCESS_INFORMATION 结构的指针，该结构接收有关新进程的标识信息。当不再需要 PROCESS_INFORMATION 中的句柄时，必须使用 CloseHandle 将其关闭。
        /// </param>
        /// <returns>
        /// 如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。
        /// 请注意，函数在进程完成初始化之前返回 。 如果找不到所需的 DLL 或无法初始化，则进程将终止。 若要获取进程的终止状态，请调用 GetExitCodeProcess。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "CreateProcessW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CreateProcess([MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName, [MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles, CREATE_PROCESS_FLAGS dwCreationFlags, IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        /// <summary>
        /// 获取调用进程的包路径。
        /// </summary>
        /// <param name="length">输入时， 路径 缓冲区的大小（以字符为单位）。 输出时，返回的包路径的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="path">包路径。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [LibraryImport(Kernel32, EntryPoint = "GetCurrentPackagePath", SetLastError = false), PreserveSig]
        public static unsafe partial uint GetCurrentPackagePath(ref int length, char* path);

        /// <summary>
        /// 检索创建调用进程时指定的 STARTUPINFO 结构的内容。
        /// </summary>
        /// <param name="lpStartupInfo">指向接收启动信息的 STARTUPINFO 结构的指针。</param>
        [LibraryImport(Kernel32, EntryPoint = "GetStartupInfoW", SetLastError = false), PreserveSig]
        public static partial void GetStartupInfo(out STARTUPINFO lpStartupInfo);
    }
}
