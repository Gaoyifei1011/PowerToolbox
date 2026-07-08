using PowerToolbox.WindowsAPI.PInvoke.Setupapi;
using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Cfgmgr32
{
    /// <summary>
    /// Cfgmgr32.dll 函数库
    /// </summary>
    public static class Cfgmgr32Library
    {
        private const string Cfgmgr32 = "cfgmgr32.dll";

        /// <summary>
        /// CM_Locate_DevNode 函数获取与本地计算机上的指定 设备实例 ID 关联的设备节点的设备实例句柄。
        /// </summary>
        /// <param name="pdnDevInst">指向 CM_Locate_DevNode 检索的设备实例句柄的指针。 检索的句柄绑定到本地计算机。</param>
        /// <param name="pDeviceID">指向表示 设备实例 ID的 NULL 终止字符串的指针。 如果此值 NULL，或者如果它指向零长度字符串，则该函数将检索设备实例句柄设备树根处的设备。</param>
        /// <param name="ulFlags">ULONG 类型的变量，它提供以下标志值，这些标志值在调用方提供设备实例标识符时适用。</param>
        /// <returns>如果操作成功，CM_Locate_DevNode 返回CR_SUCCESS。 否则，该函数将返回 Cfgmgr32.h中定义的 CR_Xxx 错误代码之一。</returns>
        [DllImport(Cfgmgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Locate_DevNodeW", PreserveSig = true, SetLastError = false)]
        public static extern int CM_Locate_DevNode(out uint pdnDevInst, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceID, CM_LOCATE_DEVNODE_FLAGS ulFlags);

        /// <summary>
        /// CM_Get_DevNode_Property 函数检索设备实例属性。
        /// </summary>
        /// <param name="devInst">绑定到本地计算机的设备实例句柄。</param>
        /// <param name="PropertyKey">指向 DEVPROPKEY 结构的指针，该结构表示所请求的设备实例属性的设备属性键。</param>
        /// <param name="PropertyType">指向 DEVPROPTYPE 类型变量的指针，该变量接收请求的设备实例属性的 property-data-type 标识符，其中 property-data-type 标识符是基数据类型标识符与 base-data 类型修饰符（如果修改基数据类型）之间的按位 OR。</param>
        /// <param name="PropertyBuffer">指向接收请求的设备实例属性的缓冲区的指针。 仅 当缓冲区大到足以保存所有属性值数据时，CM_Get_DevNode_Property才检索请求的属性。 指针可以为 NULL。</param>
        /// <param name="PropertyBufferSize"></param>
        /// <param name="ulFlags">保留。 必须设置为零。</param>
        /// <returns>如果操作成功，函数将返回CR_SUCCESS。 否则，它将返回 Cfgmgr32.h 中定义的CR_前缀错误代码之一。</returns>
        [DllImport(Cfgmgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_DevNode_PropertyW", PreserveSig = true, SetLastError = false)]
        public static extern int CM_Get_DevNode_Property(uint devInst, ref DEVPROPKEY PropertyKey, out DEVPROPTYPE PropertyType, nint PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);

        /// <summary>
        /// CM_Get_Device_Interface_List_Size 函数检索必须传递给 CM_Get_Device_Interface_List 函数的缓冲区大小。
        /// </summary>
        /// <param name="pulLen">调用方提供的指针指向接收缓冲区所需长度（以字符为单位）的位置，用于保存由 CM_Get_Device_Interface_List返回的多个 Unicode 字符串。</param>
        /// <param name="InterfaceClassGuid">提供一个 GUID，用于标识 设备接口类。</param>
        /// <param name="pDeviceID">调用方提供的指向 NULL 终止字符串的指针，该字符串表示 设备实例 ID。 如果指定，该函数将为指定的类检索设备接口支持的符号链接名称的长度。 如果此值 NULL，或者如果该值指向零长度字符串，则该函数将检索属于指定类的所有接口的符号链接名称的长度。</param>
        /// <param name="ulFlags">包含以下调用方提供的标志</param>
        /// <returns>如果操作成功，该函数将返回 CR_SUCCESS。 否则，它会返回一个错误代码，其中 CR_ 前缀，如 Cfgmgr32.h 中定义。</returns>
        [DllImport(Cfgmgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_Device_Interface_List_SizeW", PreserveSig = true, SetLastError = false)]
        public static extern int CM_Get_Device_Interface_List_Size(out uint pulLen, in Guid InterfaceClassGuid, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceID, CM_GET_DEVICE_INTERFACE_LIST_FLAGS ulFlags);

        /// <summary>
        /// CM_Get_Device_Interface_List 函数检索属于指定 设备接口类的设备接口实例列表。
        /// </summary>
        /// <param name="InterfaceClassGuid">提供标识设备接口类的 GUID。</param>
        /// <param name="pDeviceID">调用方提供的指向 NULL 终止字符串的指针，该字符串表示 设备实例 ID。 如果指定，该函数将检索设备为指定类支持的设备接口。 如果此值 NULL，或者如果该值指向零长度字符串，则该函数将检索属于指定类的所有接口。</param>
        /// <param name="Buffer">调用方提供的指向接收多个 NULL 终止的 Unicode 字符串的缓冲区的指针，每个字符串表示接口实例的符号链接名称。</param>
        /// <param name="BufferLen">调用方提供的值，该值指定由 缓冲区指向的缓冲区的长度（以字符为单位）。 调用 CM_Get_Device_Interface_List_Size 以确定所需的缓冲区大小。</param>
        /// <param name="ulFlags">包含以下调用方提供的标志</param>
        /// <returns></returns>
        [DllImport(Cfgmgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_Device_Interface_ListW", PreserveSig = true, SetLastError = false)]
        public static extern int CM_Get_Device_Interface_List(in Guid InterfaceClassGuid, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceID, [Out] byte[] Buffer, uint BufferLen, CM_GET_DEVICE_INTERFACE_LIST_FLAGS ulFlags);
    }
}
