using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 描述与用户操作关联的动画效果。 指定SPI_GETANIMATION或SPI_SETANIMATION操作值时，此结构与 SystemParametersInfo 函数一起使用。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct ANIMATIONINFO
    {
        /// <summary>
        /// 结构大小（以字节为单位）。 调用方必须将其设置为 sizeof(ANIMATIONINFO)。
        /// </summary>
        internal uint cbSize;

        /// <summary>
        /// 如果此成员为非零，则启用最小化和还原动画;否则会禁用它。
        /// </summary>
        internal int iMinAnimate;
    }
}
