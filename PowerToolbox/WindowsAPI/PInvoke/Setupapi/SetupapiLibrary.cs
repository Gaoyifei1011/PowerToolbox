using System;
using System.Runtime.InteropServices;
using System.Text;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// Setupapi.dll 函数库
    /// </summary>
    public static class SetupapiLibrary
    {
        private const string Setupapi = "setupapi.dll";

        /// <summary>
        /// SetupCopyOEMInf 函数将指定的 .inf 文件复制到 %windir%/Inf 目录。
        /// 此函数的调用方必须具有管理权限，否则该函数将失败。
        /// </summary>
        /// <param name="SourceInfFileName">源 .inf 文件的完整路径。 应使用以 null 结尾的字符串。 此路径的大小不应超过 MAX_PATH，包括终止 NULL。</param>
        /// <param name="OEMSourceMediaLocation">要存储在预编译的 .inf （.pnf） 中的源位置信息。 此位置信息特定于指定的源媒体类型。 应使用以 null 结尾的字符串。 此路径的大小不应超过 MAX_PATH，包括终止 NULL。</param>
        /// <param name="OEMSourceMediaType">位置信息引用的源媒体类型。</param>
        /// <param name="CopyStyle">指定如何将 .inf 文件复制到 .inf 目录中。</param>
        /// <param name="DestinationInfFileName">指向缓冲区的指针，用于在复制到 Inf 目录时接收分配给它的 .inf 文件名。 缓冲区（如果指定）通常应以长度 MAX_PATH。 如果指定了SP_COPY_NOOVERWRITE标志，并且 SetupCopyOEMInf 函数因返回代码ERROR_FILE_EXISTS而失败，则此缓冲区包含现有 .inf 文件的名称。 如果指定了SP_COPY_OEMINF_CATALOG_ONLY标志，则如果 Inf 目录中已存在 .inf 文件，则此缓冲区将包含目标 .inf 文件名。 否则，此缓冲区设置为空字符串。 此参数可以 NULL。</param>
        /// <param name="DestinationInfFileNameSize">如果未指定缓冲区，则 DestinationInfFileName 缓冲区的大小（以字符为单位或零）。 如果指定 了 destinationInfFileName，并且此缓冲区大小小于返回目标 .inf 文件名（包括完整路径）所需的大小，则此函数将失败。 在本例中，GetLastError 返回ERROR_INSUFFICIENT_BUFFER。</param>
        /// <param name="RequiredSize">指向接收存储目标 .inf 文件名所需的大小（以字符为单位）的变量的指针，包括终止 NULL。 如果指定了SP_COPY_OEMINF_CATALOG_ONLY标志，则仅当 Inf 目录中已存在 .inf 文件时，此变量才会接收字符串长度。 否则，此变量设置为零。 此参数可以 NULL。</param>
        /// <param name="DestinationInfFileNameComponent">指向成功返回时设置的字符串的指针（或ERROR_FILE_EXISTS），指向存储在 DestinationInfFileName 参数中路径的文件名组件的开头。 如果指定了SP_COPY_OEMINF_CATALOG_ONLY标志，DestinationInfFileName 参数可能是空字符串。 在这种情况下，字符指针设置为成功返回时 NULL。 此参数可以 NULL。</param>
        /// <returns>此函数返回 WINSETUPAPI BOOL。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupCopyOEMInfW", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupCopyOEMInf([MarshalAs(UnmanagedType.LPWStr)] string SourceInfFileName, [MarshalAs(UnmanagedType.LPWStr)] string OEMSourceMediaLocation, SPOST OEMSourceMediaType, SP_COPY CopyStyle, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder DestinationInfFileName, uint DestinationInfFileNameSize, ref uint RequiredSize, nint DestinationInfFileNameComponent);

        /// <summary>
        /// SetupDiDestroyDeviceInfoList 函数删除设备信息集并释放所有相关内存。
        /// </summary>
        /// <param name="deviceInfoSet">设置为删除的设备信息的句柄。</param>
        /// <returns>如果成功，该函数将返回 TRUE 。 否则，它将返回 FALSE ，并且可以通过调用 GetLastError 来检索记录的错误。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupDiDestroyDeviceInfoList", PreserveSig = true, SetLastError = false)]
        public static extern bool SetupDiDestroyDeviceInfoList(nint deviceInfoSet);

        /// <summary>
        /// SetupDiEnumDeviceInfo 函数返回一个SP_DEVINFO_DATA结构，该结构指定设备信息集中的设备信息元素。
        /// </summary>
        /// <param name="deviceInfoSet">设备信息集的句柄，为其返回表示设备信息元素的SP_DEVINFO_DATA结构。</param>
        /// <param name="memberIndex">要检索的设备信息元素的从零开始的索引。</param>
        /// <param name="deviceInfoData">指向 SP_DEVINFO_DATA 结构的指针，用于接收有关枚举设备信息元素的信息。 调用方必须设置 DeviceInfoData。cbSize 为 sizeof(SP_DEVINFO_DATA)。</param>
        /// <returns>如果成功，该函数将返回 TRUE 。 否则，它将返回 FALSE。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupDiEnumDeviceInfo", PreserveSig = true, SetLastError = false)]
        public static extern bool SetupDiEnumDeviceInfo(nint deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

        /// <summary>
        /// SetupDiGetClassDevs 函数返回 设备信息集 的句柄，其中包含本地计算机请求的设备信息元素。
        /// </summary>
        /// <param name="ClassGuid">指向 设备设置类的 GUID 的指针 或 设备接口类。</param>
        /// <param name="enumerator">
        /// 指向 NULL 终止字符串的指针，该字符串指定：
        /// 即插即用（PnP）的标识符（ID）枚举器。 此 ID 可以是值的全局唯一标识符（GUID）或符号名称。 例如，“PCI”可用于指定 PCI PnP 值。 PnP 值符号名称的其他示例包括“USB”、“PCMCIA”和“SCSI”。
        /// PnP 设备实例 ID。 指定 PnP 设备实例 ID 时，必须在 Flags 参数中设置DIGCF_DEVICEINTERFACE。
        /// 此指针是可选的，可以 NULL。 如果未使用 枚举 值来选择设备，请将 枚举器 设置为 NULL
        /// </param>
        /// <param name="hwndParent">要用于在设备信息集中安装设备实例的用户界面的顶级窗口的句柄。 此句柄是可选的，可以 NULL。</param>
        /// <param name="flags">类型为 DWORD 的变量，指定用于筛选添加到设备信息集的设备信息元素的控制选项。 此参数可以是零个或多个以下标志的按位 OR。 </param>
        /// <returns>如果操作成功，SetupDiGetClassDevs 将句柄返回到 设备信息集，其中包含与提供的参数匹配的所有已安装设备。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetClassDevs", PreserveSig = true, SetLastError = false)]
        public static extern nint SetupDiGetClassDevs(Guid ClassGuid, [MarshalAs(UnmanagedType.LPWStr)] string enumerator, nint hwndParent, DIGCF flags);

        /// <summary>
        /// SetupDiGetDeviceProperty 函数检索设备实例属性。
        /// </summary>
        /// <param name="deviceInfoSet">包含要检索设备实例属性的设备实例的设备信息集的句柄。</param>
        /// <param name="deviceInfoData">指向 SP_DEVINFO_DATA 结构的指针，该结构表示要为其检索设备实例属性的设备实例。</param>
        /// <param name="propertyKey">指向 DEVPROPKEY 结构的指针，该结构表示所请求的设备实例属性的设备属性键。</param>
        /// <param name="propertyType">指向 DEVPROPTYPE 类型变量的指针，该变量接收请求的设备实例属性的 property-data-type 标识符，其中 property-data-type 标识符是基数据类型标识符与 base-data 类型修饰符之间的按位 OR。</param>
        /// <param name="propertyBuffer">指向接收请求的设备实例属性的缓冲区的指针。 仅当缓冲区大到足以保存所有属性值数据时，SetupDiGetDeviceProperty 才检索请求的属性。 指针可以为 NULL。 如果指针设置为 NULL 并提供 RequiredSize ， 则 SetupDiGetDeviceProperty 将在 *RequiredSize 中返回属性的大小（以字节为单位）。</param>
        /// <param name="propertyBufferSize">PropertyBuffer 缓冲区的大小（以字节为单位）。 如果 PropertyBuffer 设置为 NULL， 则 PropertyBufferSize 必须设置为零。</param>
        /// <param name="requiredSize">指向 DWORD 类型变量的指针，该变量接收设备实例属性（如果检索到属性）的大小（以字节为单位），如果缓冲区不够大，则接收所需的缓冲区大小。 此指针可以设置为 NULL。</param>
        /// <param name="flags">该参数必须设置为零。</param>
        /// <returns>SetupDiGetDeviceProperty 如果成功，则返回 TRUE 。 否则，它将返回 FALSE。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDevicePropertyW", PreserveSig = true, SetLastError = false)]
        public static extern bool SetupDiGetDeviceProperty(nint deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref DEVPROPKEY propertyKey, out DEVPROP_TYPE propertyType, nint propertyBuffer, int propertyBufferSize, out int requiredSize, int flags);

        /// <summary>
        /// SetupGetInfDriverStoreLocation 函数检索驱动程序存储中 INF 文件 的完全限定文件名（目录路径和文件名），该文件对应于系统 INF 文件目录中的指定 INF 文件或驱动程序存储中的指定 INF 文件。
        /// </summary>
        /// <param name="FileName">指向 NULL 终止的字符串的指针，该字符串包含系统 INF 文件目录中 INF 文件的名称和完整目录路径（ 可选）。 或者，此参数是指向以 NULL 结尾的字符串的指针，该字符串包含驱动程序存储中 INF 文件的完全限定文件名（目录路径和文件名）。</param>
        /// <param name="AlternatePlatformInfo">保留供系统使用。</param>
        /// <param name="LocaleName">保留供系统使用。</param>
        /// <param name="ReturnBuffer">指向缓冲区的指针，其中函数返回一个 NULL 终止的字符串，其中包含指定 INF 文件的完全限定文件名。 此参数可以设置为 NULL。 支持的最大路径大小为MAX_PATH。</param>
        /// <param name="ReturnBufferSize"></param>
        /// <param name="RequiredSize">指向接收 ReturnBuffer 缓冲区的大小（以字符为单位）的 DWORD 类型的变量的指针。</param>
        /// <returns>如果 SetupGetInfDriverStoreLocation 成功，则函数返回 true ;否则，该函数返回 FALSE。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupGetInfDriverStoreLocationW", PreserveSig = true, SetLastError = false)]
        public static extern bool SetupGetInfDriverStoreLocation([MarshalAs(UnmanagedType.LPWStr)] string FileName, nint AlternatePlatformInfo, nint LocaleName, StringBuilder ReturnBuffer, int ReturnBufferSize, out int RequiredSize);

        /// <summary>
        /// SetupUninstallOEMInf 函数卸载指定的 .inf 文件和任何关联的 .pnf 文件。 如果 .inf 文件随用于签名驱动程序的目录一起安装，则也会删除该目录。 此函数的调用方必须具有管理权限，否则该函数将失败。
        /// </summary>
        /// <param name="InfFileName">要卸载的 Windows Inf 目录中 .inf 文件的文件名（不带路径）。</param>
        /// <param name="Flags">可以按如下所示设置此参数。</param>
        /// <param name="Reserved">设置为 null。</param>
        /// <returns>此函数返回 WINSETUPAPI BOOL。</returns>
        [DllImport(Setupapi, CharSet = CharSet.Unicode, EntryPoint = "SetupUninstallOEMInfW", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupUninstallOEMInf([MarshalAs(UnmanagedType.LPWStr)] string InfFileName, SUOI_Flags Flags, nint Reserved);
    }
}
