using System;
using System.Runtime.InteropServices;

namespace PowerToolboxShellExtension.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Shell32.dll 函数库
    /// </summary>
    internal static partial class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

        /// <summary>
        /// 对指定文件执行操作。
        /// </summary>
        /// <param name="hwnd">用于显示 UI 或错误消息的父窗口的句柄。 如果操作未与窗口关联，则此值可以为 NULL 。</param>
        /// <param name="lpOperation">指向以 null 结尾的字符串（在本例中称为 谓词）的指针，指定要执行的操作。 可用谓词集取决于特定的文件或文件夹。 通常，对象的快捷菜单中可用的操作是可用的谓词。</param>
        /// <param name="lpFile">指向 以 null 结尾的字符串的指针，该字符串指定要对其执行指定谓词的文件或对象。 若要指定 Shell 命名空间对象，请传递完全限定分析名称。 请注意，并非所有对象都支持所有谓词。 例如，并非所有文档类型都支持“print”谓词。 如果将相对路径用于 lpDirectory 参数，请不要对 lpFile 使用相对路径。</param>
        /// <param name="lpParameters">如果 lpFile 指定可执行文件，则此参数是指向以 null 结尾的字符串的指针，该字符串指定要传递给应用程序的参数。 此字符串的格式由要调用的谓词决定。 如果 lpFile 指定文档文件， 则 lpParameters 应为 NULL。</param>
        /// <param name="lpDirectory">指向 以 null 结尾的字符串的指针，该字符串指定操作) 目录的默认 (。 如果此值为 NULL，则使用当前工作目录。 如果在 lpFile 中提供了相对路径，请不要对 lpDirectory 使用相对路径。</param>
        /// <param name="nShowCmd">指定应用程序在打开时如何显示应用程序的标志。 如果 lpFile 指定文档文件，则标志将直接传递给关联的应用程序。 由应用程序决定如何处理它。 它可以是在 ShowWindow 函数的 nCmdShow 参数中指定的任何值。</param>
        /// <returns>如果函数成功，则返回大于 32 的值。 如果函数失败，它将返回一个错误值，该值指示失败的原因。 返回值被强制转换为 HINSTANCE，以实现与 16 位 Windows 应用程序的向后兼容性。</returns>
        [LibraryImport(Shell32, EntryPoint = "ShellExecuteW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        internal static partial int ShellExecute(nint hwnd, [MarshalAs(UnmanagedType.LPWStr)] string lpOperation, [MarshalAs(UnmanagedType.LPWStr)] string lpFile, string lpParameters, [MarshalAs(UnmanagedType.LPWStr)] string lpDirectory, int nShowCmd);

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
        [LibraryImport(Shell32, EntryPoint = "SHGetKnownFolderPath", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        internal static partial int SHGetKnownFolderPath(in Guid rfid, KNOWN_FOLDER_FLAG dwFlags, nint hToken, [MarshalAs(UnmanagedType.LPWStr)] out string pszPath);
    }
}
