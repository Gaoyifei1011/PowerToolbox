using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace PowerToolboxShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供用于查找 GUID 标识的服务的通用访问机制。
    /// </summary>
    [GeneratedComInterface, Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    internal partial interface IServiceProvider
    {
        /// <summary>
        /// 充当通过 IServiceProvider 实现公开的任何服务的工厂方法。
        /// </summary>
        /// <param name="guidService">服务 (SID) 的唯一标识符。</param>
        /// <param name="riid">调用方要为服务接收的接口的唯一标识符。</param>
        /// <param name="ppvObject">调用方分配的变量的地址，用于在成功从此函数返回时接收服务的接口指针。 当不再需要服务时，调用方负责通过此接口指针调用 Release 。</param>
        /// <returns>成功S_OK。</returns>
        [PreserveSig]
        int QueryService(in Guid guidService, in Guid riid, out nint ppvObject);
    }
}
