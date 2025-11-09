using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace ThemeSwitch.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Shell32.dll 函数库
    /// </summary>
    public static class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

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
    }
}
