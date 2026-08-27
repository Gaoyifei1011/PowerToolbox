namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 评估的返回值
    /// </summary>
    internal enum WINSAT_RESULT : uint
    {
        /// <summary>
        /// 评估已成功完成。
        /// </summary>
        WINSAT_STATUS_COMPLETED_SUCCESS = 0x40033,

        /// <summary>
        /// 由于系统活动，评估无法完成。
        /// </summary>
        WINSAT_ERROR_ASSESSMENT_INTERFERENCE = 0x80040034,

        /// <summary>
        /// 由于内部或系统错误，评估无法完成。
        /// </summary>
        WINSAT_ERROR_COMPLETED_ERROR = 0x80040035,

        /// <summary>
        /// 评估已取消。
        /// </summary>
        WINSAT_ERROR_WINSAT_CANCELED = 0x80040036,

        /// <summary>
        /// 传递给 WinSAT 的命令行无效。
        /// </summary>
        WINSAT_ERROR_COMMAND_LINE_INVALID = 0x80040037,

        /// <summary>
        /// 用户没有足够的权限来运行 WinSAT。
        /// </summary>
        WINSAT_ERROR_ACCESS_DENIED = 0x80040038,

        /// <summary>
        /// 另一个 WinSAT.exe 副本已在运行;计算机上一次只能运行一个 WinSAT.exe 实例。
        /// </summary>
        WINSAT_ERROR_WINSAT_ALREADY_RUNNING = 0x80040039
    }
}
