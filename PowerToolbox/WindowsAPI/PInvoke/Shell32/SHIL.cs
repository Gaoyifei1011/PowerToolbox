namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 列表中包含的图像类型。
    /// </summary>
    public enum SHIL : int
    {
        /// <summary>
        /// 图像大小通常为 32x32 像素。 但是，如果在“显示属性”的“外观”选项卡的“效果”部分选择了“使用大图标”选项，则图像为 48x48 像素。
        /// </summary>
        SHIL_LARGE = 0x0,

        /// <summary>
        /// 这些图像是 Shell 标准小型图标大小 16x16，但大小可由用户自定义。
        /// </summary>
        SHIL_SMALL = 0x1,

        /// <summary>
        /// 这些图像是 Shell 标准特大图标大小。 这通常是 48x48，但大小可由用户自定义。
        /// </summary>
        SHIL_EXTRALARGE = 0x2,

        /// <summary>
        /// 这些映像是由使用 SM_CXSMICON 调用的 GetSystemMetrics 指定的大小，使用 SM_CYSMICON 调用 GetSystemMetrics。
        /// </summary>
        SHIL_SYSSMALL = 0x3,

        /// <summary>
        ///  Windows Vista 及更高版本。 图像通常为 256x256 像素。
        /// </summary>
        SHIL_JUMBO = 0x4,

        /// <summary>
        /// 用于验证目的的最大有效标志值。
        /// </summary>
        SHIL_LAST,
    }
}
