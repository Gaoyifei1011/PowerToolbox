using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    public static class FirewallAPILibrary
    {
        private const string FirewallAPI = "FirewallAPI.dll";

        /// <summary>
        /// 枚举已在系统中创建的所有应用容器。
        /// </summary>
        /// <param name="Flags">可以设置为 NETISO_FLAG_FORCE_COMPUTE_BINARIES ，以确保在返回应用容器之前计算所有二进制文件。 如果调用方需要有关应用容器二进制文件的最新完整信息，则应设置此标志。 如果未设置此标志，则返回的数据可能已过时或不完整。</param>
        /// <param name="pdwCntPublicACs">应用容器数。</param>
        /// <param name="ppPublicACs">应用容器结构元素的列表。</param>
        /// <returns>如果成功，则返回ERROR_SUCCESS，否则返回错误值。如果内存不可用，将返回ERROR_OUTOFMEMORY。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationEnumAppContainers", PreserveSig = true, SetLastError = false)]
        public static extern uint NetworkIsolationEnumAppContainers(NETISO_FLAG Flags, out uint pdwCntPublicACs, out nint ppPublicACs);

        /// <summary>
        /// 用于释放分配给一个或多个应用容器的内存资源
        /// </summary>
        /// <param name="pPublicAppCs">要释放的应用容器内存资源。</param>
        /// <returns>返回 ERROR_SUCCESS。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationFreeAppContainers", PreserveSig = true, SetLastError = false)]
        public static extern int NetworkIsolationFreeAppContainers(nint pPublicAppCs);

        /// <summary>
        /// 用于检索有关一个或多个应用容器的配置信息。
        /// </summary>
        /// <param name="pdwNumPublicAppCs">appContainerSids 成员中的应用容器数。</param>
        /// <param name="appContainerSids">安全标识符 (允许发送环回流量的应用容器) SID。 用于调试目的。</param>
        /// <returns>如果成功，则返回ERROR_SUCCESS，否则返回错误值。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationGetAppContainerConfig", PreserveSig = true, SetLastError = false)]
        public static extern uint NetworkIsolationGetAppContainerConfig(out uint pdwNumPublicAppCs, out nint appContainerSids);

        /// <summary>
        /// 用于设置一个或多个应用容器的配置。
        /// </summary>
        /// <param name="dwNumPublicAppCs">appContainerSids 成员中的应用容器数。</param>
        /// <param name="appContainerSids">安全标识符 (允许发送环回流量的应用容器) SID。 用于调试目的。</param>
        /// <returns>如果成功，则返回ERROR_SUCCESS，否则返回错误值。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationSetAppContainerConfig", PreserveSig = true, SetLastError = false)]
        public static extern uint NetworkIsolationSetAppContainerConfig(int dwNumPublicAppCs, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] SID_AND_ATTRIBUTES[] appContainerSids);
    }
}
