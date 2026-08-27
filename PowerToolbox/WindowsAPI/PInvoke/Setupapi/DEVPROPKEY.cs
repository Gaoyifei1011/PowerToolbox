using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// 在 Windows Vista 和更高版本的 Windows 中，DEVPROPKEY 结构表示统一设备属性 模型中设备属性的设备属性键。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DEVPROPKEY
    {
        /// <summary>
        /// 指定属性类别的 DEVPROPGUID 类型的值。
        /// </summary>
        internal Guid fmtid;

        /// <summary>
        /// 一个 DEVPROPID 类型的值，用于唯一标识属性类别中的 属性。 出于内部系统原因，属性标识符必须大于或等于 2。
        /// </summary>
        internal uint pid;
    }
}
