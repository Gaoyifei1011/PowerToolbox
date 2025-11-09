using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Shell32.dll 函数库
    /// </summary>
    public static class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

        /// <summary>
        /// 注册窗口是否接受已删除的文件。
        /// </summary>
        /// <param name="hwnd">正在注册是否接受已删除文件的窗口的标识符。</param>
        /// <param name="fAccept">一个值，该值指示 hWnd 参数标识的窗口是否接受已删除的文件。 如果接受已删除的文件，则此值为 TRUE ;如果值为 FALSE ，则表示停止接受已删除的文件。</param>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "DragAcceptFiles", PreserveSig = true, SetLastError = false)]
        public static extern void DragAcceptFiles(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool fAccept);

        /// <summary>
        /// 检索由于成功拖放操作而删除的文件的名称。
        /// </summary>
        /// <param name="hDrop">包含已删除文件的文件名的结构的标识符。</param>
        /// <param name="iFile">要查询的文件的索引。 如果此参数的值为 0xFFFFFFFF， 则 DragQueryFile 将返回已删除的文件计数。 如果此参数的值介于零和删除的文件总数之间， 则 DragQueryFile 会将具有相应值的文件名复制到 lpszFile 参数指向的缓冲区。</param>
        /// <param name="lpszFile">在函数返回时接收已删除文件的文件名的缓冲区的地址。 此文件名是以 null 结尾的字符串。 如果此参数为 NULL，DragQueryFile 将返回此缓冲区的所需大小（以字符为单位）。</param>
        /// <param name="cch">lpszFile 缓冲区的大小（以字符为单位）。</param>
        /// <returns>
        /// 非零值表示调用成功。
        /// 当函数将文件名复制到缓冲区时，返回值是复制的字符计数，不包括终止 null 字符。
        /// 如果索引值为0xFFFFFFFF，则返回值是已删除文件的计数。 请注意，索引变量本身返回不变，因此保持0xFFFFFFFF。
        /// 如果索引值介于零和已删除文件总数之间，并且 lpszFile 缓冲区地址为 NULL，则返回值是缓冲区所需的大小（以字符为单位）， 不包括 终止 null 字符。
        /// </returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "DragQueryFileW", PreserveSig = true, SetLastError = false)]
        public static extern uint DragQueryFile(nuint hDrop, uint iFile, [Out, MarshalAs(UnmanagedType.LPArray)] char[] lpszFile, uint cch);

        /// <summary>
        /// 检索在拖放操作期间删除文件时鼠标指针的位置。
        /// </summary>
        /// <param name="hDrop">述已删除文件的放置结构的句柄。</param>
        /// <param name="lppt">指向 POINT 结构的指针，当此函数成功返回时，该结构接收删除文件时鼠标指针的坐标。</param>
        /// <returns>如果删除发生在窗口的工作区中，则为 TRUE;否则为 FALSE。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "DragQueryPoint", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DragQueryPoint(nuint hDrop, out Point lppt);

        /// <summary>
        /// 描述已删除的文件的结构的标识符。 此句柄是从WM_DROPFILES消息的 wParam 参数检索的。
        /// </summary>
        /// <param name="hDrop">释放系统分配用于将文件名传输到应用程序的内存。</param>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "DragFinish", PreserveSig = true, SetLastError = false)]
        public static extern void DragFinish(nuint hDrop);

        /// <summary>
        /// 返回与指定文件路径关联的 ITEMIDLIST 结构。
        /// </summary>
        /// <param name="pszPath">指向包含路径的以 null 结尾的 Unicode 字符串的指针。 此字符串的长度应不超过 MAX_PATH 个字符，包括终止 null 字符。</param>
        /// <returns>返回指向对应于路径的 ITEMIDLIST 结构的指针。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "ILCreateFromPathW", PreserveSig = true, SetLastError = false)]
        public static extern nint ILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath);

        /// <summary>
        /// 释放 Shell 分配的 ITEMIDLIST 结构。
        /// </summary>
        /// <param name="pidl">指向要释放的 ITEMIDLIST 结构的指针。 此参数可以为 NULL。</param>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "ILFree", PreserveSig = true, SetLastError = false)]
        public static extern void ILFree(nint pidl);

        /// <summary>
        /// 对指定文件执行操作。
        /// </summary>
        /// <param name="lpExecInfo">指向 SHELLEXECUTEINFO 结构的指针，该结构包含并接收有关正在执行的应用程序的信息。</param>
        /// <returns>如果成功，则返回 TRUE ;否则为 FALSE。 调用 GetLastError 获取扩展错误信息。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "ShellExecuteExW", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        /// <summary>
        /// 从分析名称创建和初始化命令行管理程序项对象。
        /// </summary>
        /// <param name="pszPath">指向显示名称的指针。</param>
        /// <param name="pbc">
        /// 自选。指向绑定上下文的指针，用于将参数作为输入和输出传递给分析函数。
        /// 这些传递的参数通常特定于数据源，并由数据源所有者记录。
        /// 例如，文件系统数据源接受正在使用STR_FILE_SYS_BIND_DATA绑定上下文参数分析的名称（作为 WIN32_FIND_DATA 结构）。
        /// 可以传递STR_PARSE_PREFER_FOLDER_BROWSING以指示在可能的情况下使用文件系统数据源分析 URL。
        /// 使用 CreateBindCtx 构造绑定上下文对象，并使用IBindCtx::RegisterObjectParam 填充值。
        /// 有关这些键的完整列表，请参阅绑定上下文字符串键。有关使用此参数的示例，请参阅使用参数进行分析示例。
        /// 如果没有数据传递到分析函数或从分析函数接收任何数据，则此值可以为NULL。
        /// </param>
        /// <param name="riid">对接口的 IID 的引用，以通过ppv（通常为IID_IShellItem或IID_IShellItem2）进行检索。</param>
        /// <param name="ppv">此方法成功返回时，包含 riid 中请求的接口指针。这通常是IShellItem或IShellItem2。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "SHCreateItemFromParsingName", PreserveSig = true, SetLastError = false)]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        /// <summary>
        /// 检索由文件夹的 KNOWNFOLDERID 标识的已知文件夹的完整路径。
        /// </summary>
        /// <param name="rfid">对标识文件夹的 KNOWNFOLDERID 的引用。</param>
        /// <param name="dwFlags">指定特殊检索选项的标志。 此值可以为 0;否则，一个或多个 KNOWN_FOLDER_FLAG 值。</param>
        /// <param name="hToken">
        /// 表示特定用户的 访问令牌 。 如果此参数为 NULL（这是最常见的用法），则该函数会请求当前用户的已知文件夹。
        /// 通过传递该用户的 hToken 请求特定用户的文件夹。 这通常在具有足够权限检索给定用户的令牌的服务上下文中完成。 必须使用 TOKEN_QUERY 和 TOKEN_IMPERSONATE 权限打开该令牌。 在某些情况下，还需要包含 TOKEN_DUPLICATE。 除了传递用户的 hToken 外，还必须装载该特定用户的注册表配置单元。 有关访问控制问题的进一步讨论，请参阅访问控制。
        /// 为 hToken 参数分配值 -1 表示默认用户。 这允许 SHGetKnownFolderPath 的客户端查找文件夹位置(，例如默认用户的 桌面 文件夹) 。 创建任何新用户帐户时，默认用户用户配置文件将重复，并包含文档和桌面等特殊文件夹。 添加到“默认用户”文件夹的任何项目也会显示在任何新用户帐户中。 请注意，访问“默认用户”文件夹需要管理员权限。
        /// </param>
        /// <param name="pszPath">
        /// 此方法返回时，包含指向以 null 结尾的 Unicode 字符串的指针的地址，该字符串指定已知文件夹的路径。 调用进程负责通过调用 CoTaskMemFree 不再需要此资源后释放此资源，无论 SHGetKnownFolderPath 是否成功。 返回的路径不包括尾随反斜杠。 例如，返回“C：\Users”而不是“C：\Users\”。
        /// </param>
        /// <returns>如果成功，则返回S_OK，否则返回错误值</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "SHGetKnownFolderPath", PreserveSig = true, SetLastError = false)]
        public static extern int SHGetKnownFolderPath(Guid rfid, KNOWN_FOLDER_FLAG dwFlags, nint hToken, [MarshalAs(UnmanagedType.LPWStr)] out string pszPath);

        /// <summary>
        /// 打开 Windows 资源管理器窗口，其中选定了特定文件夹中的指定项目。
        /// </summary>
        /// <param name="pidlFolder">指向指定文件夹的完全限定项 ID 列表的指针。</param>
        /// <param name="cidl">选择数组 apidl 中的项计数。 如果 cidl 为零，则 pidlFolder 必须指向描述要选择的单个项的完全指定的 ITEMIDLIST 。 此函数打开父文件夹并选择该项目。</param>
        /// <param name="apidl">指向 PIDL 结构数组的指针，每个结构都是在 pidlFolder 引用的目标文件夹中选择的项。</param>
        /// <param name="dwFlags">可选标志。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "SHOpenFolderAndSelectItems", ExactSpelling = false, PreserveSig = true)]
        public static extern int SHOpenFolderAndSelectItems(nint pidlFolder, uint cidl, nint apidl, uint dwFlags);
    }
}
