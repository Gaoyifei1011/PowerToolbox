using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.NewDev
{
    /// <summary>
    /// NewDev.dll 函数库
    /// </summary>
    public static class NewDevLibrary
    {
        private const string NewDev = "newDev.dll";

        /// <summary>
        /// DiInstallDriver 函数预安装 驱动程序存储中的驱动程序，然后在驱动程序支持的系统中存在的设备上安装驱动程序。
        /// </summary>
        /// <param name="hwndParent">DiInstallDriver 的顶级窗口句柄用于显示与安装设备关联的任何用户界面组件。 此参数是可选的，可以设置为 NULL。</param>
        /// <param name="InfPath">指向 NULL 终止的字符串的指针，该字符串为 驱动程序包提供 INF 文件的完全限定路径。</param>
        /// <param name="Flags">
        /// 一个类型为 DWORD 的值，该值指定一个或多个标志（标志 通常设置为零）或一个或多个标志的组合。
        /// 如果 标志 为零，DiInstallDriver 仅当驱动程序与设备上当前安装的驱动程序匹配时，才会在设备上安装指定的驱动程序。 有关 Windows 如何为设备选择驱动程序的信息，请参阅 Windows 如何选择驱动程序。
        /// 如果 标志 包括DIIRFLAG_FORCE_INF，DiInstallDriver 在匹配的设备上安装指定的驱动程序，无论驱动程序是否比设备上当前安装的驱动程序更好。 如果还指定了DIIRFLAG_INSTALL_AS_SET，则忽略DIIRFLAG_FORCE_INF。
        /// 如果 标志 包括DIIRFLAG_INSTALL_AS_SET（在 Windows 10 版本 1709 及更高版本上受支持），InfPath 应指定目录而不是 INF 文件的完全限定路径，DiInstallDriver 将安装该目录中具有特殊行为的所有 INF 文件。 的所有 驱动程序包都将暂存到 驱动程序存储，但尚未在设备上安装。 在下一次关闭系统时，这些驱动程序包将可供今后安装在设备上，它们将安装在最适合的任何设备上，以便设备在下一次启动系统时准备就绪。
        /// </param>
        /// <param name="NeedReboot">指向 DiInstallDriver 类型的 BOOL 类型的值的指针，用于指示是否需要系统重启才能完成安装。 此参数是可选的，可以 NULL。 如果提供参数并且需要系统重启才能完成安装，DiInstallDriver 将值设置为 true。 在这种情况下，调用方必须提示用户重启系统。 如果提供了此参数，并且不需要系统重启才能完成安装，DiInstallDriver 将值设置为 FALSE。 如果参数 NULL 并且需要系统重启才能完成安装，DiInstallDriver 将显示系统重启对话框。</param>
        /// <returns>DiInstallDriver 如果函数成功在 驱动程序存储中预安装指定的 驱动程序包，则返回 true。 如果函数在系统中的一个或多个设备上成功安装驱动程序，则 DiInstallDriver 也会返回 true。 如果在驱动程序存储中未成功安装驱动程序包，DiInstallDriver 返回 FALSE。</returns>
        [DllImport(NewDev, CharSet = CharSet.Unicode, EntryPoint = "DiInstallDriverW", PreserveSig = true, SetLastError = false)]
        public static extern bool DiInstallDriver(nint hwndParent, [MarshalAs(UnmanagedType.LPWStr)] string InfPath, uint Flags, out bool NeedReboot);

        /// <summary>
        /// DiUninstallDriver 函数通过安装具有另一个匹配驱动程序包的设备（如果可用）安装驱动程序包，或者如果没有其他匹配驱动程序包可用，则从安装驱动程序包的任何设备中删除驱动程序包。 然后，从 驱动程序存储中删除指定的驱动程序包。
        /// </summary>
        /// <param name="hwndParent">DiUninstallDriver 的顶级窗口句柄用于显示与安装设备关联的任何用户界面组件。 此参数是可选的，可以设置为 NULL。</param>
        /// <param name="InfPath">指向 NULL 终止的字符串的指针，该字符串为 驱动程序包提供 INF 文件的完全限定路径。</param>
        /// <param name="Flags">
        /// 一个类型为 DWORD 的值，指定以下标志中的零个或多个：DIURFLAG_NO_REMOVE_INF。 通常，此标志应设置为零。
        /// 如果此标志为零，则 DiUninstallDriver 使用其他匹配驱动程序包安装这些设备（如果可用），或者如果没有其他匹配驱动程序包可用，则从安装驱动程序包的任何设备中删除驱动程序包。 但是，如果此标志设置为DIURFLAG_NO_REMOVE_INF，DiUninstallDriver 从安装的任何设备中删除驱动程序包，但不从驱动程序存储中删除驱动程序包。
        /// </param>
        /// <param name="NeedReboot">指向 BOOL 类型的值的指针，该值 DiUninstallDriver 设置为指示是否需要系统重启才能完成卸载。 此参数是可选的，可以 NULL。 如果提供参数并且需要系统重启才能完成卸载，DiUninstallDriver 将值设置为 true。 在这种情况下，调用方必须提示用户重启系统。 如果提供了此参数，并且不需要系统重启才能完成卸载，DiUninstallDriver 将值设置为 FALSE。 如果参数 NULL 并且需要系统重启才能完成卸载，DiUninstallDriver 将显示系统重启对话框。</param>
        /// <returns>DiInstallDriver 如果函数成功在 驱动程序存储中预安装指定的 驱动程序包，则返回 true。 如果函数在系统中的一个或多个设备上成功安装驱动程序，则 DiInstallDriver 也会返回 true。 如果在驱动程序存储中未成功安装驱动程序包，DiInstallDriver 返回 FALSE。</returns>
        [DllImport(NewDev, CharSet = CharSet.Unicode, EntryPoint = "DiUninstallDriverW", PreserveSig = true, SetLastError = false)]
        public static extern bool DiUninstallDriver(nint hwndParent, [MarshalAs(UnmanagedType.LPWStr)] string InfPath, DIURFLAG Flags, out bool NeedReboot);
    }
}
