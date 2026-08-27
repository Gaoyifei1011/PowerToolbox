namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// 位置信息引用的源媒体类型。
    /// </summary>
    internal enum SPOST
    {
        /// <summary>
        /// .pnf 文件中未存储任何源媒体信息。 在这种情况下，将忽略 OEMSourceMediaLocation 的值。
        /// </summary>
        SPOST_NONE = 0,

        /// <summary>
        /// OEMSourceMediaLocation 包含源媒体的路径。 例如，如果媒体位于软盘上，则此路径可能是“A：\”。 如果 OEMSourceMediaLocationNULL，则路径假定为 .inf 所在的路径。 如果 .inf 在该位置具有相应的 .pnf，则 .pnf 文件的源媒体信息将传输到目标 .pnf 文件。
        /// </summary>
        SPOST_PATH = 1,

        /// <summary>
        /// OEMSourceMediaLocation 包含通用资源定位符（URL），用于指定从中检索 .inf/驱动程序文件的 Internet 位置。 如果 OEMSourceMediaLocationNULL，则假定使用了默认的代码下载管理器位置。
        /// </summary>
        SPOST_URL = 2,

        SPOST_MAX = 3
    }
}
