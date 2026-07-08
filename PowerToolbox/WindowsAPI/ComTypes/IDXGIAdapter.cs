using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDXGIAdapter 接口表示显示子系统 (包括一个或多个 GPU、DAC 和视频内存) 。
    /// </summary>
    [ComImport, Guid("2411E7E1-12AC-4CCF-BD14-9798E8534DC0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIAdapter
    {
        /// <summary>
        /// 将应用程序定义的数据设置为 对象，并将该数据与 GUID 相关联。
        /// </summary>
        /// <param name="Name">标识数据的 GUID。 在调用 GetPrivateData 时使用此 GUID 获取数据。</param>
        /// <param name="DataSize">对象数据的大小。</param>
        /// <param name="pData">指向对象数据的指针。</param>
        /// <returns>返回 DXGI_ERROR 值之一。</returns>
        [PreserveSig]
        int SetPrivateData(Guid Name, uint DataSize, nint pData);

        /// <summary>
        /// 在对象的专用数据中设置接口。
        /// </summary>
        /// <param name="Name">标识接口的 GUID。</param>
        /// <param name="pUnknown">指向要与设备对象关联的 IUnknown 派生接口的指针。 其引用计数在设置时递增，当 IDXGIObject 被销毁时或通过调用 SetPrivateData 或 SetPrivateDataInterface 使用相同的 GUID 覆盖数据时递减。</param>
        /// <returns>返回以下 DXGI_ERROR之一。</returns>
        [PreserveSig]
        int SetPrivateDataInterface(Guid Name, nint pUnknown);

        /// <summary>
        /// 获取指向对象数据的指针。
        /// </summary>
        /// <param name="Name">标识数据的 GUID。</param>
        /// <param name="pDataSize">数据的大小。</param>
        /// <param name="pData">指向数据的指针。</param>
        /// <returns>返回以下 DXGI_ERROR之一。</returns>
        [PreserveSig]
        int GetPrivateData(Guid Name, ref uint pDataSize, nint pData);

        /// <summary>
        /// 获取对象的父级。
        /// </summary>
        /// <param name="riid">所请求接口的 ID。</param>
        /// <param name="ppParent">指向父对象的指针的地址。</param>
        /// <returns>返回 DXGI_ERROR 值之一。</returns>
        [PreserveSig]
        int GetParent(Guid riid, out nint ppParent);

        /// <summary>
        /// 枚举适配器 (视频卡) 输出。
        /// </summary>
        /// <param name="Output">输出的索引。</param>
        /// <param name="ppOutput">指向由 Output 参数指定位置的 IDXGIOutput 接口的指针的地址。</param>
        /// <returns>
        /// 指示成功或失败 (看到 DXGI_ERROR) 的代码。 如果索引大于输出数，则返回DXGI_ERROR_NOT_FOUND。
        /// 如果适配器来自使用 D3D_DRIVER_TYPE_WARP 创建的设备，则适配器没有输出，因此返回DXGI_ERROR_NOT_FOUND。
        /// </returns>
        [PreserveSig]
        int EnumOutputs(uint Output, out nint ppOutput);

        /// <summary>
        /// 获取适配器 (或视频卡) 的 DXGI 1.0 说明。
        /// </summary>
        /// <param name="pDesc">指向描述适配器 的 DXGI_ADAPTER_DESC 结构的指针。 此参数不得为 NULL。 在功能级别 9 图形硬件上，GetDesc 为 DXGI_ADAPTER_DESC 的 VendorId、DeviceId、SubSysId 和 Revision 成员返回零，并为 Description 成员中的说明字符串返回“Software Adapter”。</param>
        /// <returns>如果成功，则返回S_OK;否则，如果 pDesc 参数为 NULL，则返回E_INVALIDARG。</returns>
        [PreserveSig]
        int GetDesc(out DXGI_ADAPTER_DESC pDesc);

        /// <summary>
        /// 检查系统是否支持图形组件的设备接口。
        /// </summary>
        /// <param name="InterfaceName">正在检查其支持的设备版本的接口的 GUID。 这通常应__uuidof (IDXGIDevice) ，它将返回 Direct3D 9 UMD (用户模式驱动程序的版本号) 二进制文件。 自 WDDM 2.3 以来，驱动程序包 (D3D9、D3D11 和 D3D12) 的所有驱动程序组件都需要共享单个版本号，因此无论使用哪个 API，都是查询驱动程序版本的好方法。</param>
        /// <param name="pUMDVersion">InterfaceName 的用户模式驱动程序版本。 仅当接口受支持时，才会返回此参数，否则此参数将为 NULL。</param>
        /// <returns>S_OK指示接口受支持，否则 (返回DXGI_ERROR_UNSUPPORTED 有关详细信息，请参阅 DXGI_ERROR) 。</returns>
        [PreserveSig]
        int CheckInterfaceSupport(Guid InterfaceName, out long pUMDVersion);
    }
}
