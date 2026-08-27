namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// 类型为 DWORD 的变量，指定用于筛选添加到设备信息集的设备信息元素的控制选项。 此参数可以是零个或多个以下标志的按位 OR。
    /// </summary>
    internal enum DIGCF
    {
        /// <summary>
        /// 仅为指定的设备接口类返回与系统默认设备接口关联的设备（如果已设置设备接口）。
        /// </summary>
        DIGCF_DEFAULT = 0x00000001,

        /// <summary>
        /// 仅返回系统中当前存在的设备。
        /// </summary>
        DIGCF_PRESENT = 0x00000002,

        /// <summary>
        /// 返回所有设备安装类或所有设备接口类的已安装设备列表。
        /// </summary>
        DIGCF_ALLCLASSES = 0x00000004,

        /// <summary>
        /// 仅返回属于当前硬件配置文件一部分的设备。
        /// </summary>
        DIGCF_PROFILE = 0x00000008,

        /// <summary>
        /// 返回支持指定设备接口类的设备。 如果 枚举器 参数指定 设备实例 ID，则必须在 标志 参数中设置此标志。
        /// </summary>
        DIGCF_DEVICEINTERFACE = 0x00000010,
    }
}
