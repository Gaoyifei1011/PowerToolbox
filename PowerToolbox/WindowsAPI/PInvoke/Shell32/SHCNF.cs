using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    [Flags]
    public enum SHCNF
    {
        /// <summary>
        /// dwItem1 和 dwItem2 是 ITEMIDLIST 结构的地址，表示受更改影响的项 () 。 每个 ITEMIDLIST 都必须相对于桌面文件夹。
        /// </summary>
        SHCNF_IDLIST = 0x0000,

        /// <summary>
        /// dwItem1 和 dwItem2 是包含受更改影响的项的完整路径名称的最大 长度MAX_PATH 以 null 结尾的字符串的地址。
        /// </summary>
        SHCNF_PATHA = 0x0001,

        /// <summary>
        /// dwItem1 和 dwItem2 是以 null 结尾的字符串的地址，这些字符串表示受更改影响的打印机 () 的友好名称。
        /// </summary>
        SHCNF_PRINTERA = 0x0002,

        /// <summary>
        /// dwItem1 和 dwItem2 参数是 DWORD 值。
        /// </summary>
        SHCNF_DWORD = 0x0003,

        /// <summary>
        /// dwItem1 和 dwItem2 是包含受更改影响的项的完整路径名称的最大 长度MAX_PATH 以 null 结尾的字符串的地址。
        /// </summary>
        SHCNF_PATHW = 0x0005,

        /// <summary>
        /// dwItem1 和 dwItem2 是以 null 结尾的字符串的地址，这些字符串表示受更改影响的打印机 () 的友好名称。
        /// </summary>
        SHCNF_PRINTERW = 0x0006,

        SHCNF_TYPE = 0x00FF,

        /// <summary>
        /// 在通知传递到所有受影响的组件之前，函数不应返回。 由于此标志修改了其他数据类型标志，因此不能单独使用它。
        /// </summary>
        SHCNF_FLUSH = 0x1000,

        /// <summary>
        /// 函数应开始向所有受影响的组件传递通知，但应在通知过程开始后立即返回。 由于此标志修改了其他数据类型标志，因此它不能由自己使用。 此标志包括 SHCNF_FLUSH。
        /// </summary>
        SHCNF_FLUSHNOWAIT = 0x2000,

        /// <summary>
        /// 通知已注册所有子级的客户端。
        /// </summary>
        SHCNF_NOTIFYRECURSIVE = 0x10000,
    }
}
