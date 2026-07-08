using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 介绍使用 DXGI 1.0 (或视频卡) 的适配器。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct DXGI_ADAPTER_DESC
    {
        /// <summary>
        /// 包含适配器说明的字符串。 在 功能级别 9 图形硬件上， GetDesc 返回描述字符串的“软件适配器”。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Description;

        /// <summary>
        /// 适配器硬件供应商的 PCI ID 或 ACPI ID。 如果此值小于或等于 0xFFFF，则为 PCI ID;否则，它是 ACPI ID。 在 功能级别 9 图形硬件上， GetDesc 为此值返回零。
        /// </summary>
        public uint VendorId;

        /// <summary>
        /// 适配器的硬件设备的 PCI ID 或 ACPI ID。 如果 VendorId 是 PCI ID，则它也是 PCI ID;否则，它是 ACPI ID。 在 功能级别 9 图形硬件上， GetDesc 为此值返回零。
        /// </summary>
        public uint DeviceId;

        /// <summary>
        /// 适配器的硬件子系统的 PCI ID 或 ACPI ID。 如果 VendorId 是 PCI ID，则它也是 PCI ID;否则，它是 ACPI ID。 在 功能级别 9 图形硬件上， GetDesc 为此值返回零。
        /// </summary>
        public uint SubSysId;

        /// <summary>
        /// 适配器的 PCI 或 ACPI 修订号。 如果 VendorId 是 PCI ID，则为 PCI 设备修订号;否则，它是 ACPI 设备修订号。 在 功能级别 9 图形硬件上， GetDesc 返回此值的零。
        /// </summary>
        public uint Revision;

        /// <summary>
        /// 不与 CPU 共享的专用视频内存的字节数。
        /// </summary>
        public nint DedicatedVideoMemory;

        /// <summary>
        /// 不与 CPU 共享的专用系统内存的字节数。 此内存在启动时从可用的系统内存中分配。
        /// </summary>
        public nint DedicatedSystemMemory;

        /// <summary>
        /// 共享系统内存的字节数。 这是适配器在操作期间可能消耗的系统内存的最大值。 驱动程序在管理和使用视频内存时使用的任何附带内存都是额外的。
        /// </summary>
        public nint SharedSystemMemory;

        /// <summary>
        /// 标识适配器的唯一值。 有关结构的定义，请参阅 LUID 。 LUID 在 dxgi.h 中定义。
        /// </summary>
        public LUID AdapterLuid;
    }
}
