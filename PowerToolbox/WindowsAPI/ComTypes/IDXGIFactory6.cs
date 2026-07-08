using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDXGIFactory 接口实现用于生成 DXGI 对象的方法， (处理全屏转换) 。
    /// </summary>
    [ComImport, Guid("C1B6694F-FF09-44A9-B03C-77900A0A1D17"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IDXGIFactory6
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
        /// 枚举适配器 (视频卡) 。
        /// </summary>
        /// <param name="Adapter">要枚举的适配器的索引。</param>
        /// <param name="ppAdapter">指向适配器参数指定位置的 IDXGIAdapter 接口的指针的地址。 此参数不得为 NULL。</param>
        /// <returns>如果成功，则返回S_OK;否则，如果索引大于或等于本地系统中的适配器数，则返回 DXGI_ERROR_NOT_FOUND;如果 ppAdapter 参数为 NULL，则返回 DXGI_ERROR_INVALID_CALL。</returns>
        [PreserveSig]
        int EnumAdapters(uint Adapter, [MarshalAs(UnmanagedType.Interface)] out IDXGIAdapter ppAdapter);

        /// <summary>
        /// 允许 DXGI 监视应用程序的消息队列中的 alt-enter 键序列 (这会导致应用程序从窗口切换到全屏，反之亦然) 。
        /// </summary>
        /// <param name="WindowHandle">要监视的窗口的句柄。 此参数可以为 NULL;但前提是 Flags 也为 0。</param>
        /// <param name="Flags">
        /// 以下一个或多个值。
        /// DXGI_MWA_NO_WINDOW_CHANGES - 阻止 DXGI 监视应用程序消息队列;这会使 DXGI 无法响应模式更改。
        /// DXGI_MWA_NO_ALT_ENTER - 阻止 DXGI 响应 alt-enter 序列。
        /// DXGI_MWA_NO_PRINT_SCREEN - 阻止 DXGI 响应打印屏幕键。
        /// </param>
        /// <returns>DXGI_ERROR_INVALID_CALLWindowHandle 是否无效或E_OUTOFMEMORY。</returns>
        [PreserveSig]
        int MakeWindowAssociation(nint WindowHandle, uint Flags);

        /// <summary>
        /// 获取一个窗口，用户通过该窗口控制全屏切换和切换。
        /// </summary>
        /// <param name="pWindowHandle">指向窗口句柄的指针。</param>
        /// <returns>返回指示成功或失败的代码。 S_OK 指示成功， DXGI_ERROR_INVALID_CALL 指示 pWindowHandle 已作为 NULL 传入。</returns>
        [PreserveSig]
        int GetWindowAssociation(out nint pWindowHandle);

        /// <summary>
        /// 创建交换链。
        /// </summary>
        /// <param name="pDevice">对于 Direct3D 11 和早期版本的 Direct3D，这是指向交换链的 Direct3D 设备的指针。 对于 Direct3D 12，这是指向直接命令队列的指针， (引用 ID3D12CommandQueue) 。 此参数不能为 NULL。</param>
        /// <param name="pDesc">指向交换链说明 DXGI_SWAP_CHAIN_DESC 结构的指针。 此参数不能为 NULL。</param>
        /// <param name="ppSwapChain">指向变量的指针，该变量接收指向 CreateSwapChain 创建的交换链的 IDXGISwapChain 接口的指针。</param>
        /// <returns>DXGI_ERROR_INVALID_CALL 如果 pDesc 或 ppSwapChain 为 NULL，则DXGI_STATUS_OCCLUDED请求全屏模式且不可用，或者E_OUTOFMEMORY。 也可能返回由传入的设备类型定义的其他错误代码。</returns>
        [PreserveSig]
        int CreateSwapChain(nint pDevice, nint pDesc, out nint ppSwapChain);

        /// <summary>
        /// 创建表示软件适配器的适配器接口。
        /// </summary>
        /// <param name="Module">软件适配器的 dll 的句柄。 可以使用 GetModuleHandle 或 LoadLibrary 获取 HMODULE。</param>
        /// <param name="ppAdapter">指向适配器</param>
        /// <returns>指示成功或失败的返回代码 。</returns>
        [PreserveSig]
        int CreateSoftwareAdapter(nint Module, [MarshalAs(UnmanagedType.Interface)] out IDXGIAdapter ppAdapter);

        /// <summary>
        /// 枚举两个适配器 (视频卡) 带或不带输出。
        /// </summary>
        /// <param name="Adapter">要枚举的适配器的索引。</param>
        /// <param name="ppAdapter">指向适配器参数指定位置处 IDXGIAdapter1 接口的指针的地址。此参数不得为 NULL。</param>
        /// <returns>如果成功，则返回S_OK;否则，如果索引大于或等于本地系统中的适配器数，则返回 DXGI_ERROR_NOT_FOUND;如果 ppAdapter 参数为 NULL，则返回 DXGI_ERROR_INVALID_CALL。</returns>
        [PreserveSig]
        int EnumAdapters1(uint Adapter, out nint ppAdapter);

        /// <summary>
        /// 通知应用程序可能需要重新创建工厂并重新枚举适配器。
        /// </summary>
        /// <returns>如果新适配器可用或当前适配器即将消失，则为 FALSE。 TRUE，不更改适配器。IsCurrent 返回 FALSE 以通知调用应用程序重新枚举适配器。</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsCurrent();

        /// <summary>
        /// 确定是否使用立体声模式。
        /// </summary>
        /// <returns>指示是否使用立体声模式。 TRUE 表示可以使用立体声模式;否则为 FALSE。</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsWindowedStereoEnabled();

        /// <summary>
        /// 创建与 HWND 句柄关联的交换链，该句柄指向交换链的输出窗口。
        /// </summary>
        /// <param name="pDevice">对于 Direct3D 11 和早期版本的 Direct3D，这是指向交换链的 Direct3D 设备的指针。 对于 Direct3D 12，这是指向直接命令队列的指针， (引用 ID3D12CommandQueue) 。 此参数不能为 NULL。</param>
        /// <param name="hWnd">与 CreateSwapChainForHwnd 创建的交换链关联的 HWND 句柄。 此参数不能为 NULL。</param>
        /// <param name="pDesc">指向交换链说明 DXGI_SWAP_CHAIN_DESC1 结构的指针。 此参数不能为 NULL。</param>
        /// <param name="pFullscreenDesc">指向 DXGI_SWAP_CHAIN_FULLSCREEN_DESC 结构的指针，用于描述全屏交换链。 可以选择设置此参数以创建全屏交换链。 将其设置为 NULL 可创建窗口交换链。</param>
        /// <param name="pRestrictToOutput">
        /// 指向要限制内容的输出 的 IDXGIOutput 接口的指针。 还必须在 IDXGISwapChain1：:P resent1 调用中传递 DXGI_PRESENT_RESTRICT_TO_OUTPUT 标志，以强制内容在任何其他输出上显示为黑屏。 如果要将内容限制为不同的输出，则必须创建新的交换链。 但是，可以根据 DXGI_PRESENT_RESTRICT_TO_OUTPUT 标志有条件地限制内容。
        /// 如果不想将内容限制为输出目标，请将此参数设置为 NULL 。
        /// </param>
        /// <param name="ppSwapChain">指向变量的指针，该变量接收指向 CreateSwapChainForHwnd 创建的交换链的 IDXGISwapChain1 接口的指针。</param>
        /// <returns>
        /// CreateSwapChainForHwnd 返回：
        /// 如果成功创建了交换链，S_OK。
        /// 如果内存不可用，则E_OUTOFMEMORY以完成操作。
        /// DXGI_ERROR_INVALID_CALL 调用应用程序提供无效数据时，例如，如果 pDesc 或 ppSwapChain 为 NULL，或者 pDesc 数据成员无效。
        /// 可能是 DXGI_ERROR 主题中描述的其他错误代码，这些错误代码由传递给 pDevice 的设备类型定义。
        /// </returns>
        [PreserveSig]
        int CreateSwapChainForHwnd(nint pDevice, nint hWnd, nint pDesc, nint pFullscreenDesc, nint pRestrictToOutput, out nint ppSwapChain);

        /// <summary>
        /// 为交换链的输出窗口创建与 CoreWindow 对象关联的交换链。
        /// </summary>
        /// <param name="pDevice">对于 Direct3D 11 和早期版本的 Direct3D，这是指向交换链的 Direct3D 设备的指针。 对于 Direct3D 12，这是指向直接命令队列的指针， (引用 ID3D12CommandQueue) 。 此参数不能为 NULL。</param>
        /// <param name="pWindow">指向 CoreWindow 对象的指针，该对象与 CreateSwapChainForCoreWindow 创建的交换链相关联。</param>
        /// <param name="pDesc">指向交换链说明 DXGI_SWAP_CHAIN_DESC1 结构的指针。 此参数不能为 NULL。</param>
        /// <param name="pRestrictToOutput">指向交换链限制的 IDXGIOutput 接口的指针。 如果交换链移动到其他输出，则内容为黑色。 可以选择将此参数设置为使用 DXGI_PRESENT_RESTRICT_TO_OUTPUT 限制此输出内容的输出目标。 如果未将此参数设置为限制输出目标上的内容，则可以将其设置为 NULL。</param>
        /// <param name="ppSwapChain">指向变量的指针，该变量接收指向 CreateSwapChainForCoreWindow 创建的交换链的 IDXGISwapChain1 接口的指针。</param>
        /// <returns>
        /// CreateSwapChainForCoreWindow 返回：
        /// S_OK成功创建交换链。
        /// E_OUTOFMEMORY内存不可用以完成操作。
        /// DXGI_ERROR_INVALID_CALL 调用应用程序是否提供了无效数据，例如，如果 pDesc 或 ppSwapChain 为 NULL。
        /// 可能是 DXGI_ERROR 主题中介绍的其他错误代码，这些错误代码由传递给 pDevice 的设备类型定义。
        /// </returns>
        [PreserveSig]
        int CreateSwapChainForCoreWindow(nint pDevice, nint pWindow, nint pDesc, nint pRestrictToOutput, out nint ppSwapChain);

        /// <summary>
        /// 标识创建共享资源对象的适配器。
        /// </summary>
        /// <param name="hResource">共享资源对象的句柄。 IDXGIResource1：：CreateSharedHandle 方法返回此句柄。</param>
        /// <param name="pLuid">指向一个变量的指针，该变量接收本地唯一标识符 (LUID) 标识适配器的值。 LUID 在 Dxgi.h 中定义。 LUID 是一个 64 位值，保证仅在生成它的操作系统上是唯一的。 只有在重新启动操作系统之前， 才保证 LUID 的唯一性。</param>
        /// <returns>
        /// GetSharedResourceAdapterLuid 返回：
        /// S_OK是否标识了适配器。
        /// 如果hResource 无效，DXGI_ERROR_INVALID_CALL。
        /// 可能是 DXGI_ERROR 主题中描述的其他错误代码。
        /// </returns>
        [PreserveSig]
        int GetSharedResourceAdapterLuid(nint hResource, out nint pLuid);

        /// <summary>
        /// 注册应用程序窗口以接收立体声状态更改的通知消息。
        /// </summary>
        /// <param name="WindowHandle">当立体声状态发生更改时，要向其发送通知消息的窗口的句柄。</param>
        /// <param name="wMsg">标识要发送的通知消息。</param>
        /// <param name="pdwCookie">指向应用程序可以传递给 IDXGIFactory2：：UnregisterStereoStatus 方法的键值的指针，用于取消注册 wMsg 指定的通知消息。</param>
        /// <returns>
        /// RegisterStereoStatusWindow 返回：
        /// S_OK是否成功注册窗口。
        /// E_OUTOFMEMORY内存不可用以完成操作。
        /// 可能是 DXGI_ERROR 主题中描述的其他错误代码。
        /// </returns>
        [PreserveSig]
        int RegisterStereoStatusWindow(nint WindowHandle, uint wMsg, out uint pdwCookie);

        /// <summary>
        /// 使用事件信号注册以接收立体声状态更改的通知。
        /// </summary>
        /// <param name="hEvent">发生立体声状态更改通知时操作系统设置的事件对象的句柄。 CreateEvent 或 OpenEvent 函数返回此句柄。</param>
        /// <param name="pdwCookie">指向键值的指针，应用程序可以传递给 IDXGIFactory2：：UnregisterStereoStatus 方法，以取消注册 hEvent 指定的通知事件。</param>
        /// <returns>
        /// RegisterStereoStatusEvent 返回：
        /// S_OK是否成功注册了事件。
        /// 如果内存不可用，则E_OUTOFMEMORY以完成操作。
        /// 可能是 DXGI_ERROR 主题中描述的其他错误代码。
        /// </returns>
        [PreserveSig]
        int RegisterStereoStatusEvent(nint hEvent, out uint pdwCookie);

        /// <summary>
        /// 取消注册窗口或事件，以阻止它在立体声状态更改时接收通知。
        /// </summary>
        /// <param name="dwCookie">要注销的窗口或事件的键值。 IDXGIFactory2：：RegisterStereoStatusWindow 或 IDXGIFactory2：：RegisterStereoStatusEvent 方法返回此值。</param>
        [PreserveSig]
        void UnregisterStereoStatus(uint dwCookie);

        /// <summary>
        /// 注册应用程序窗口，以接收有关遮挡状态更改的通知消息。
        /// </summary>
        /// <param name="WindowHandle">窗口的句柄，用于在发生遮挡状态更改时向其发送通知消息。</param>
        /// <param name="wMsg">标识要发送的通知消息。</param>
        /// <param name="pdwCookie">指向键值的指针，应用程序可以传递给 IDXGIFactory2：：UnregisterOcclusionStatus 方法，以取消注册 wMsg 指定的通知消息。</param>
        /// <returns>
        /// RegisterOcclusionStatusWindow 返回：
        /// S_OK是否成功注册了窗口。
        /// 如果内存不可用，则E_OUTOFMEMORY以完成操作。
        /// DXGI_ERROR_INVALID_CALLWindowHandle 是否不是有效的窗口句柄，或者不是当前进程拥有的窗口句柄。
        /// 可能是 DXGI_ERROR 主题中描述的其他错误代码。
        /// </returns>
        [PreserveSig]
        int RegisterOcclusionStatusWindow(nint WindowHandle, uint wMsg, out uint pdwCookie);

        /// <summary>
        /// 使用事件信号注册以接收有关封闭状态更改的通知。
        /// </summary>
        /// <param name="hEvent">发生封闭状态更改通知时操作系统设置的事件对象的句柄。 CreateEvent 或 OpenEvent 函数返回此句柄。</param>
        /// <param name="pdwCookie">指向应用程序可以传递给 IDXGIFactory2：：UnregisterOcclusionStatus 方法的键值的指针，用于取消注册 hEvent 指定的通知事件。</param>
        /// <returns>
        /// RegisterOcclusionStatusEvent 返回：
        /// S_OK 方法是否成功注册事件。
        /// E_OUTOFMEMORY内存不可用以完成操作。
        /// DXGI_ERROR_INVALID_CALLhEvent 是否不是有效的句柄或事件句柄。
        /// 可能是 DXGI_ERROR 主题中描述的其他错误代码。
        /// </returns>
        [PreserveSig]
        int RegisterOcclusionStatusEvent(nint hEvent, out uint pdwCookie);

        /// <summary>
        /// 取消注册窗口或事件，以在封闭状态更改时阻止它接收通知。
        /// </summary>
        /// <param name="dwCookie">要注销的窗口或事件的键值。 IDXGIFactory2：：RegisterOcclusionStatusWindow 或 IDXGIFactory2：：RegisterOcclusionStatusEvent 方法返回此值。</param>
        [PreserveSig]
        void UnregisterOcclusionStatus(uint dwCookie);

        /// <summary>
        /// 创建一个交换链，可用于将 Direct3D 内容发送到 DirectComposition API、 Windows.UI.Xaml 框架或 Windows UI 库 (WinUI) XAML 进行撰写。
        /// </summary>
        /// <param name="pDevice">对于 Direct3D 11 和早期版本的 Direct3D，这是指向交换链的 Direct3D 设备的指针。 对于 Direct3D 12，这是指向直接命令队列的指针， (引用 ID3D12CommandQueue) 。 此参数不能为 NULL。 组合交换链不支持软件驱动程序 （如 D3D_DRIVER_TYPE_REFERENCE）。</param>
        /// <param name="pDesc">
        /// 指向交换链说明 DXGI_SWAP_CHAIN_DESC1 结构的指针。 此参数不能为 NULL。
        /// 必须在 DXGI_SWAP_CHAIN_DESC1 的 SwapEffect 成员中指定 DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL值， 因为 CreateSwapChainForComposition 仅支持 翻转演示文稿模型。
        /// 还必须在 DXGI_SWAP_CHAIN_DESC1 的 缩放 成员中指定 DXGI_SCALING_STRETCH值。
        /// </param>
        /// <param name="pRestrictToOutput">
        /// 指向要限制内容的输出 的 IDXGIOutput 接口的指针。 还必须在 IDXGISwapChain1：:P resent1 调用中传递 DXGI_PRESENT_RESTRICT_TO_OUTPUT 标志，以强制内容在任何其他输出上显示为黑屏。 如果要将内容限制为其他输出，则必须创建新的交换链。 但是，可以根据 DXGI_PRESENT_RESTRICT_TO_OUTPUT 标志有条件地限制内容。
        /// 如果不想将内容限制为输出目标，请将此参数设置为 NULL 。
        /// </param>
        /// <param name="ppSwapChain">指向变量的指针，该变量接收指向 CreateSwapChainForComposition 创建的交换链的 IDXGISwapChain1 接口的指针。
        /// </param>
        /// <returns>
        /// CreateSwapChainForComposition 返回：
        /// S_OK成功创建交换链。
        /// E_OUTOFMEMORY内存不可用以完成操作。
        /// DXGI_ERROR_INVALID_CALL 调用应用程序是否提供了无效数据，例如，如果 pDesc 或 ppSwapChain 为 NULL。
        /// 可能是 DXGI_ERROR 主题中介绍的其他错误代码，这些错误代码由传递给 pDevice 的设备类型定义。
        /// </returns>
        [PreserveSig]
        int CreateSwapChainForComposition(nint pDevice, nint pDesc, nint pRestrictToOutput, out nint ppSwapChain);

        /// <summary>
        /// 获取创建 Microsoft DirectX 图形基础结构 (DXGI) 对象时使用的标志。
        /// </summary>
        /// <returns>创建标志。</returns>
        [PreserveSig]
        uint GetCreationFlags();

        /// <summary>
        /// 输出指定 LUID 的 IDXGIAdapter 。
        /// </summary>
        /// <param name="AdapterLuid">标识适配器的唯一值。 有关结构的定义，请参阅 LUID 。 LUID 在 dxgi.h 中定义。</param>
        /// <param name="riid">全局唯一标识符 (ppvAdapter 参数引用的 IDXGIAdapter 对象的 GUID) 。</param>
        /// <param name="ppvAdapter">指向适配器的 IDXGIAdapter 接口指针的地址。 此参数不能为 NULL。</param>
        /// <returns>如果成功，则返回S_OK;否则为错误代码。 有关错误代码的列表，请参阅 DXGI_ERROR。 另请参阅 Direct3D 12 返回代码。</returns>
        [PreserveSig]
        int EnumAdapterByLuid(nint AdapterLuid, Guid riid, [MarshalAs(UnmanagedType.Interface)] out IDXGIAdapter ppvAdapter);

        /// <summary>
        /// 提供一个适配器，该适配器可以提供给 D3D12CreateDevice 以使用 WARP 呈现器。
        /// </summary>
        /// <param name="riid">全局唯一标识符 (ppvAdapter 参数引用的 IDXGIAdapter 对象的 GUID) 。</param>
        /// <param name="ppvAdapter">指向适配器的 IDXGIAdapter 接口指针的地址。 此参数不能为 NULL。</param>
        /// <returns>如果成功，则返回S_OK;否则为错误代码。 有关错误代码的列表，请参阅 DXGI_ERROR。 另请参阅 Direct3D 12 返回代码。</returns>
        [PreserveSig]
        int EnumWarpAdapter(Guid riid, [MarshalAs(UnmanagedType.Interface)] out IDXGIAdapter ppvAdapter);

        /// <summary>
        /// 用于检查硬件功能支持。
        /// </summary>
        /// <param name="Feature">指定要查询支持的 DXGI_FEATURE 的一个成员。</param>
        /// <param name="pFeatureSupportData">指定指向缓冲区的指针，该缓冲区将填充描述功能支持的数据。</param>
        /// <param name="FeatureSupportDataSize">pFeatureSupportData 的大小（以字节为单位）。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int CheckFeatureSupport(nint Feature, nint pFeatureSupportData, uint FeatureSupportDataSize);

        /// <summary>
        /// 根据给定的 GPU 首选项枚举图形适配器。
        /// </summary>
        /// <param name="Adapter">要枚举的适配器的索引。 索引按 GpuPreference 中指定的首选项顺序排列，例如，如果指定 了DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE ，则性能最高的适配器位于索引 0，第二高适配器位于索引 1 处，依此推。</param>
        /// <param name="GpuPreference">应用的 GPU 首选项。</param>
        /// <param name="riid">全局唯一标识符 (ppvAdapter 参数引用的 IDXGIAdapter 对象的 GUID) 。</param>
        /// <param name="ppvAdapter">指向适配器的 IDXGIAdapter 接口指针的地址。此参数不能为 NULL。</param>
        /// <returns>如果成功 ， 则返回S_OK;否则为错误代码。 有关错误代码的列表，请参阅 DXGI_ERROR。</returns>
        [PreserveSig]
        int EnumAdapterByGpuPreference(uint Adapter, DXGI_GPU_PREFERENCE GpuPreference, in Guid riid, [MarshalAs(UnmanagedType.Interface)] out IDXGIAdapter ppvAdapter);
    }
}
