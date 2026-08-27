using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolboxShellExtension.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// Advapi32.dll 函数库
    /// </summary>
    internal static partial class Advapi32Library
    {
        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// 关闭指定注册表项的句柄。
        /// </summary>
        /// <param name="hKey">要关闭的打开键的句柄。 该句柄必须由 RegOpenKeyEx 函数打开。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegCloseKey", SetLastError = false), PreserveSig]
        internal static partial int RegCloseKey(nuint hKey);

        /// <summary>
        /// 枚举指定打开的注册表项的子项。 函数在每次调用时检索有关一个子项的信息。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。</param>
        /// <param name="dwIndex">要检索的子项的索引。 首次调用 RegEnumKeyEx 函数时，此参数应为零，然后针对后续调用递增。由于子项未排序，因此任何新子项都将具有任意索引。 这意味着函数可以按任意顺序返回子项。</param>
        /// <param name="lpName">指向接收子项名称（包括终止 null 字符）的缓冲区的指针。 函数仅将子项的名称（而不是完整键层次结构）复制到缓冲区。如果函数失败，则不会将任何信息复制到此缓冲区。</param>
        /// <param name="lpcchName">指向变量的指针，该变量指定 由 lpName 参数指定的缓冲区的大小（以字符为单位）。 此大小应包含终止 null 字符。 如果函数成功， 则 lpcName 指向的变量包含缓冲区中存储的字符数，不包括终止 null 字符。若要确定所需的缓冲区大小，请使用 RegQueryInfoKey 函数确定 由 hKey 参数标识的键的最大子项的大小。</param>
        /// <param name="lpReserved">此参数是保留的，必须为 NULL。</param>
        /// <param name="lpClass">指向接收枚举子项的用户定义类的缓冲区的指针。 此参数可以为 NULL。</param>
        /// <param name="lpcbClass">指向变量的指针，该变量指定 由 lpClass 参数指定的缓冲区的大小（以字符为单位）。 大小应包括终止 null 字符。 如果函数成功， 则 lpcClass 包含缓冲区中存储的字符数，不包括终止 null 字符。 仅当 lpClass 为 NULL 时，此参数才能为 NULL。</param>
        /// <param name="lpftLastWriteTime">指向 FILETIME 结构的指针，该结构接收上次写入枚举子项的时间。 此参数可以为 NULL。</param>
        /// <returns>
        /// 如果函数成功，则返回值为 ERROR_SUCCESS。
        /// 如果函数失败，则返回值为 系统错误代码。 如果没有其他可用的子项，函数将返回ERROR_NO_MORE_ITEMS。
        /// 如果 lpName 缓冲区太小而无法接收密钥的名称，则函数将返回ERROR_MORE_DATA。
        /// </returns>
        [LibraryImport(Advapi32, EntryPoint = "RegEnumKeyExW"), PreserveSig]
        internal static partial int RegEnumKeyEx(nuint hKey, int dwIndex, [Out, MarshalAs(UnmanagedType.LPArray)] char[] lpName, ref int lpcchName, [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] lpReserved, [In, Out, MarshalAs(UnmanagedType.LPArray)] char[] lpClass, [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] lpcbClass, [Out, MarshalAs(UnmanagedType.LPArray)] long[] lpftLastWriteTime);

        /// <summary>
        /// 打开指定的注册表项。 请注意，键名称不区分大小写。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。</param>
        /// <param name="lpSubKey">要打开的注册表子项的名称。键名称不区分大小写。</param>
        /// <param name="ulOptions">指定打开键时要应用的选项。</param>
        /// <param name="samDesired">一个掩码，指定要打开的密钥的所需访问权限。 如果密钥的安全描述符不允许调用进程的请求访问，函数将失败。</param>
        /// <param name="phkResult">一个变量的指针，此变量指向已打开键的句柄。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegOpenKeyExW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        internal static partial int RegOpenKeyEx(nuint hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey, int ulOptions, RegistryAccessRights samDesired, out nuint phkResult);

        /// <summary>
        /// 检索与打开的注册表项关联的指定值名称的类型和数据。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。 必须使用KEY_QUERY_VALUE访问权限打开密钥。</param>
        /// <param name="lpValueName">注册表值的名称。</param>
        /// <param name="lpReserved">此参数是保留的，必须为 NULL。</param>
        /// <param name="lpType">指向一个变量的指针，该变量接收一个代码，指示存储在指定值中的数据的类型。</param>
        /// <param name="lpData">指向接收值数据的缓冲区的指针。</param>
        /// <param name="lpcbData">指向一个变量的指针，该变量指定 lpData 参数指向的缓冲区的大小（以字节为单位）。</param>
        /// <returns>
        /// 如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 系统错误代码。
        /// 如果 lpData 缓冲区太小，无法接收数据，函数将返回ERROR_MORE_DATA。如果 lpValueName 注册表值不存在，该函数将返回ERROR_FILE_NOT_FOUND。
        /// </returns>
        [LibraryImport(Advapi32, EntryPoint = "RegQueryValueExW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        internal static partial int RegQueryValueEx(nuint hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpValueName, int lpReserved, out RegistryValueKind lpType, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] lpData, ref int lpcbData);
    }
}
