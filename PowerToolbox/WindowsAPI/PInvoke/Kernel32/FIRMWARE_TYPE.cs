namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 指定固件类型
    /// </summary>
    public enum FIRMWARE_TYPE
    {
        /// <summary>
        /// 固件类型未知。
        /// </summary>
        FirmwareTypeUnknown,

        /// <summary>
        /// 在旧版 BIOS 模式下启动的计算机。
        /// </summary>
        FirmwareTypeBios,

        /// <summary>
        /// 以 UEFI 模式启动的计算机。
        /// </summary>
        FirmwareTypeUefi,

        /// <summary>
        /// 未实现。
        /// </summary>
        FirmwareTypeMax
    }
}
