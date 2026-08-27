using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// Kernel.AppCore.dll 函数库
    /// </summary>
    internal static class KernelAppCoreLibrary
    {
        private const string KernelAppCore = "kernel.appcore.dll";

        /// <summary>
        /// 获取调用进程的包信息，选项用于指定要为包检索的文件夹路径的类型。
        /// </summary>
        /// <param name="flags">指定如何检索包信息的包 常量 。 支持 PACKAGE_FILTER_* 标志。</param>
        /// <param name="packagePathType">指示要为原始安装文件夹或可变文件夹) (包检索的文件夹路径的类型。</param>
        /// <param name="bufferLength">输入时， 缓冲区的大小（以字节为单位）。 输出时返回的结构数组的大小（以字节为单位）。</param>
        /// <param name="buffer">包信息，表示为 PACKAGE_INFO 结构的数组。</param>
        /// <param name="count">缓冲区中的结构数。</param>
        /// <returns>如果该函数成功，则返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。</returns>
        [DllImport(KernelAppCore, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageInfo", PreserveSig = true, SetLastError = false)]
        internal static extern int GetCurrentPackageInfo(PACKAGE_FLAGS flags, ref uint bufferLength, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, out uint count);
    }
}
