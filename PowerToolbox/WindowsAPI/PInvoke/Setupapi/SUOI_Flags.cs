namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    internal enum SUOI_Flags
    {
        SUOI_NONE = 0x0000,

        /// <summary>
        /// SetupUninstallOEMInf 函数首先检查是否使用 .inf 文件安装了任何设备。 无需将设备作为使用 .inf 文件进行检测。
        /// 如果未设置此标志，并且该函数查找使用此 .inf 文件安装的当前安装设备，则不会删除 .inf 文件。
        /// 如果设置了此标志，则会删除 .inf 文件，该函数是否查找随此 .inf 文件一起安装的设备。
        /// </summary>
        SUOI_FORCEDELETE = 0x0001
    }
}
