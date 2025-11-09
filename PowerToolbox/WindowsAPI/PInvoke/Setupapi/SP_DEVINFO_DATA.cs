using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// SP_DEVINFO_DATA结构定义作为设备信息集成员的设备实例。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SP_DEVINFO_DATA
    {
        /// <summary>
        /// SP_DEVINFO_DATA结构的大小（以字节为单位）。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 设备的安装类的 GUID。
        /// </summary>
        public Guid ClassGuid;

        /// <summary>
        /// 设备实例的不透明句柄 (也称为 开发节点) 的句柄。
        /// 某些函数（如 SetupDiXxx 函数）采用整个SP_DEVINFO_DATA结构作为输入，以识别设备信息集中的设备。 其他函数（如 cm_Xxx 函数（如 CM_Get_DevNode_Status）将此 DevInst 句柄作为输入。
        /// </summary>
        public int DevInst;

        /// <summary>
        /// 保留。 仅限内部使用。
        /// </summary>
        public nint Reserved;
    }
}
