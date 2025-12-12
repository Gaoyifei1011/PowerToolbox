using System.Runtime.InteropServices;
using System.Text;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        public const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

        /// <summary>
        /// 获取当前进程的 应用程序用户模型 ID 。
        /// </summary>
        /// <param name="applicationUserModelIdLength">输入时， applicationUserModelId 缓冲区的大小（以宽字符为单位）。 成功时，使用的缓冲区大小，包括 null 终止符。</param>
        /// <param name="applicationUserModelId">指向接收应用程序用户模型 ID 的缓冲区的指针。</param>
        /// <returns>如果该函数成功，则返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentApplicationUserModelId", PreserveSig = true, SetLastError = false)]
        public static extern int GetCurrentApplicationUserModelId(ref uint applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);

        /// <summary>
        /// 获取调用进程的包系列名称。
        /// </summary>
        /// <param name="packageFamilyNameLength">输入时， packageFamilyName 缓冲区的大小（以字符为单位），包括 null 终止符。 输出时，返回的包系列名称的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="packageFamilyName">包系列名称。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFamilyName", PreserveSig = true, SetLastError = false)]
        public static extern int GetCurrentPackageFamilyName(ref int packageFamilyNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder packageFamilyName);

        /// <summary>
        /// 获取调用进程的包全名。
        /// </summary>
        /// <param name="packageFullNameLength">输入时， packageFullName 缓冲区的大小（以字符为单位）。 输出时，返回包全名的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFullName", PreserveSig = true, SetLastError = false)]
        public static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        /// <summary>
        /// 检索当前进程的伪句柄。
        /// </summary>
        /// <returns>返回值是当前进程的伪句柄。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentProcess", PreserveSig = true, SetLastError = false)]
        public static extern nint GetCurrentProcess();

        /// <summary>
        /// 检索调用线程的最后错误代码值。 最后一个错误代码按线程进行维护。 多个线程不会覆盖彼此的最后一个错误代码。
        /// </summary>
        /// <returns>
        /// 返回值是调用线程的最后错误代码。
        /// 设置最后错误代码的每个函数的文档的返回值部分记录了函数设置最后错误代码的条件。 设置线程最后错误代码的大多数函数在失败时设置它。 但是，某些函数还会在成功时设置最后一个错误代码。 如果未记录函数以设置最后一个错误代码，则此函数返回的值只是要设置的最新最后一个错误代码;某些函数在成功时将最后一个错误代码设置为 0，而其他函数则不这样做。
        /// </returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetLastError", PreserveSig = true, SetLastError = false)]
        public static extern int GetLastError();

        /// <summary>
        /// 获取指定包的路径。
        /// </summary>
        /// <param name="packageFullName">包的全名。</param>
        /// <param name="pathLength">
        /// 指向包含包路径字符串中 WCHAR) 字符数 (的变量的指针，其中包含 null 终止符。
        /// 首先，将 NULL 传递给 路径 以获取字符数。 使用此数字为 路径分配内存空间。 然后传递此内存空间的地址以填充 路径。
        /// </param>
        /// <param name="path">指向接收包路径字符串（包括 null 终止符）的内存空间的指针。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetPackagePathByFullName", PreserveSig = true, SetLastError = false)]
        public static extern int GetPackagePathByFullName([MarshalAs(UnmanagedType.LPWStr)] string packageFullName, ref int pathLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path);

        /// <summary>
        /// 使应用程序能够通知系统它正在使用中，从而防止系统在应用程序运行时进入睡眠状态或关闭显示器。
        /// </summary>
        /// <param name="esFlags">线程的执行要求。</param>
        /// <returns>如果函数成功，则返回值为上一个线程执行状态。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "SetThreadExecutionState", PreserveSig = true, SetLastError = false)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }
}
