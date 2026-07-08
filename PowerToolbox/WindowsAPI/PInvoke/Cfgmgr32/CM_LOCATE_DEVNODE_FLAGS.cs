namespace PowerToolbox.WindowsAPI.PInvoke.Cfgmgr32
{
    public enum CM_LOCATE_DEVNODE_FLAGS : uint
    {
        /// <summary>
        /// 仅当设备当前在设备树中配置设备时，该函数才会检索指定设备的设备实例句柄。
        /// </summary>
        CM_LOCATE_DEVNODE_NORMAL = 0x00000000,

        /// <summary>
        /// 如果设备当前在设备树中配置设备，或者设备是当前未在设备树中配置的 非代表设备，该函数将检索指定设备的设备实例句柄。
        /// </summary>
        CM_LOCATE_DEVNODE_PHANTOM = 0x00000001,

        /// <summary>
        /// 如果设备当前在设备树或从设备树中删除设备的过程中配置了设备，该函数将检索指定设备的设备实例句柄。 如果设备正在删除过程中，该函数将取消删除设备。
        /// </summary>
        CM_LOCATE_DEVNODE_CANCELREMOVE = 0x00000002,

        /// <summary>
        /// 未使用。
        /// </summary>
        CM_LOCATE_DEVNODE_NOVALIDATION = 0x00000004,

        CM_LOCATE_DEVNODE_BITS = 0x00000007
    }
}
