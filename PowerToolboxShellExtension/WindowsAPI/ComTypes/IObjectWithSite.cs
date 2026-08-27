using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace PowerToolboxShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供一种简单的方法来支持容器中对象与其站点之间的通信。
    /// </summary>
    [GeneratedComInterface, Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
    internal partial interface IObjectWithSite
    {
        /// <summary>
        /// 允许容器向对象传递指向其站点接口的指针。
        /// </summary>
        /// <param name="pUnkSite">指向管理此对象的站点的 IUnknown 接口指针的指针。 如果 为 NULL，则对象应在对象不再知道其站点的任何现有站点上调用 Release 。</param>
        /// <returns>此方法在成功时返回S_OK。</returns>
        [PreserveSig]
        int SetSite(nint pUnkSite);

        /// <summary>
        /// 检索使用 SetSite 传递的最新站点。
        /// </summary>
        /// <param name="riid">应在 ppvSite 中返回的接口指针的 IID。</param>
        /// <param name="ppvSite">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppvSite 包含请求的接口指针，指向 上次在 SetSite 中看到的站点。 返回的特定接口取决于 riid 参数。 实质上，这两个参数的作用与 QueryInterface 中的参数相同。 如果相应的接口指针可用，则对象必须在该指针上调用 AddRef ，然后才能成功返回。 如果没有可用的站点，或者不支持请求的接口，则此方法必须 *ppvSite 为 NULL 并返回失败代码。</param>
        /// <returns>此方法在成功时返回S_OK。</returns>
        [PreserveSig]
        int GetSite(in Guid riid, out nint ppvSite);
    }
}
