namespace PowerToolbox.WindowsAPI.PInvoke.Cfgmgr32
{
    public enum CM_GET_DEVICE_INTERFACE_LIST_FLAGS : uint
    {
        /// <summary>
        /// 该函数提供一个列表，其中包含与当前处于活动状态的设备关联的设备接口，以及与指定的 GUID 和设备实例 ID 匹配（如果有）。
        /// </summary>
        CM_GET_DEVICE_INTERFACE_LIST_PRESENT = 0,

        /// <summary>
        /// 该函数提供一个列表，其中包含与与指定 GUID 和设备实例 ID 匹配的所有设备关联的设备接口（如果有）。
        /// </summary>
        CM_GET_DEVICE_INTERFACE_LIST_ALL_DEVICES = 1
    }
}
