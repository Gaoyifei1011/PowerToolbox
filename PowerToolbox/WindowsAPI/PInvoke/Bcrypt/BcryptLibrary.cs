using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Bcrypt
{
    public static class BcryptLibrary
    {
        public const string Bcrypt = "bcrypt.dll";

        /// <summary>
        /// BCryptOpenAlgorithmProvider 函数加载并初始化 CNG 提供程序。
        /// </summary>
        /// <param name="phAlgorithm">指向接收 CNG 提供程序句柄的 BCRYPT_ALG_HANDLE 变量的指针。 使用此句柄后，请将其传递给 BCryptCloseAlgorithmProvider 函数来释放它。</param>
        /// <param name="pszAlgId">指向标识所请求加密算法的以 null 结尾的 Unicode 字符串的指针。 这可以是 的标准 CNG 算法标识符之一，也可以是另一个已注册算法的标识符。</param>
        /// <param name="pszImplementation">指向标识要加载的特定提供程序的以 null 结尾的 Unicode 字符串的指针。 这是加密基元提供程序的已注册别名。 此参数是可选的，如果需要，则可以 NULL。 如果此参数 NULL，则将加载指定算法的默认提供程序。</param>
        /// <param name="dwFlags">修改函数行为的标志。</param>
        /// <returns>返回一个状态代码，指示函数的成功或失败。</returns>
        [DllImport(Bcrypt, CharSet = CharSet.Unicode, EntryPoint = "BCryptOpenAlgorithmProvider", PreserveSig = true, SetLastError = false)]
        public static extern uint BCryptOpenAlgorithmProvider(out nint phAlgorithm, [MarshalAs(UnmanagedType.LPWStr)] string pszAlgId, [MarshalAs(UnmanagedType.LPWStr)] string pszImplementation, uint dwFlags);

        /// <summary>
        /// BCryptCloseAlgorithmProvider 函数关闭算法提供程序。
        /// </summary>
        /// <param name="hAlgorithm">表示要关闭的算法提供程序的句柄。 通过调用 BCryptOpenAlgorithmProvider 函数来获取此句柄。</param>
        /// <param name="dwFlags">一组标志，用于修改此函数的行为。 此函数未定义任何标志。</param>
        /// <returns>返回一个状态代码，指示函数的成功或失败。</returns>
        [DllImport(Bcrypt, CharSet = CharSet.Unicode, EntryPoint = "BCryptCloseAlgorithmProvider", PreserveSig = true, SetLastError = false)]
        public static extern uint BCryptCloseAlgorithmProvider(nint hAlgorithm, uint dwFlags);

        /// <summary>
        /// 调用 BCryptCreateHash 函数来创建哈希或 消息身份验证代码 （MAC） 对象。
        /// </summary>
        /// <param name="hAlgorithm">支持哈希或 MAC 接口的算法提供程序的句柄。 此句柄是通过调用 BCryptOpenAlgorithmProvider 函数获取的，也可以是 CNG 算法伪句柄。</param>
        /// <param name="phHash">指向 BCRYPT_HASH_HANDLE 值的指针，该值接收表示哈希或 MAC 对象的句柄。 此句柄用于后续哈希或 MAC 函数，例如 BCryptHashData 函数。 使用此句柄后，请将其传递给 BCryptDestroyHash 函数来释放它。</param>
        /// <param name="pbHashObject">
        /// 指向接收哈希或 MAC 对象的缓冲区的指针。 cbHashObject 参数包含此缓冲区的大小。 可以通过调用 BCryptGetProperty 函数从算法句柄获取 BCRYPT_OBJECT_LENGTH 属性来获取此缓冲区的所需大小。 这将为指定的算法提供哈希或 MAC 对象的大小。
        /// 只有在 phHash 参数指向的句柄被销毁后，才能释放此内存。
        /// 如果此参数 NULL 的值和 cbHashObject 参数的值为零，则哈希对象的内存由此函数分配，并由 BCryptDestroyHash 释放。 Windows 7： 从 Windows 7 开始，可以使用此内存管理功能。
        /// </param>
        /// <param name="cbHashObject">
        /// pbHashObject 缓冲区的大小（以字节为单位）。
        /// 如果此参数的值为零且 pbHashObject 参数的值为 NULL，则密钥对象的内存由此函数分配，并由 BCryptDestroyHash 释放。 Windows 7： 从 Windows 7 开始，可以使用此内存管理功能。
        /// </param>
        /// <param name="pbSecret">
        /// 指向包含用于 MAC 的密钥的缓冲区的指针。 cbSecret 参数包含此缓冲区的大小。 如果与哈希算法一起使用，则必须使用 BCryptOpenAlgorithmProvider 中的 BCRYPT_ALG_HANDLE_HMAC 标志将算法提升到 HMAC。
        /// 若要计算哈希，请将此参数设置为 NULL。
        /// </param>
        /// <param name="cbSecret">pbSecret 缓冲区的大小（以字节为单位）。 如果未使用任何键，请将此参数设置为零。</param>
        /// <param name="dwFlags">修改函数行为的标志。</param>
        /// <returns></returns>
        [DllImport(Bcrypt, CharSet = CharSet.Unicode, EntryPoint = "BCryptCreateHash", PreserveSig = true, SetLastError = false)]
        public static extern uint BCryptCreateHash(nint hAlgorithm, out nint phHash, nint pbHashObject, uint cbHashObject, nint pbSecret, uint cbSecret, uint dwFlags);

        /// <summary>
        /// BCryptDestroyHash 函数 (MAC) 对象销毁哈希或消息身份验证代码。
        /// </summary>
        /// <param name="hHash">要销毁的哈希或 MAC 对象的句柄。 此句柄是使用 BCryptCreateHash 函数获取的。</param>
        /// <returns>返回指示函数成功或失败的状态代码。</returns>
        [DllImport(Bcrypt, CharSet = CharSet.Unicode, EntryPoint = "BCryptDestroyHash", PreserveSig = true, SetLastError = false)]
        public static extern uint BCryptDestroyHash(nint hHash);

        /// <summary>
        /// BCryptHashData 函数对数据缓冲区执行单向哈希或 消息身份验证代码（MAC）。
        /// </summary>
        /// <param name="hHash">用于执行操作的哈希或 MAC 对象的句柄。 通过调用 BCryptCreateHash 函数来获取此句柄。</param>
        /// <param name="pbInput">指向包含要处理的数据的缓冲区的指针。 cbInput 参数包含此缓冲区中的字节数。 此函数不会修改此缓冲区的内容。</param>
        /// <param name="cbInput">pbInput 缓冲区中的字节数。</param>
        /// <param name="dwFlags">一组标志，用于修改此函数的行为。 当前未定义任何标志，因此此参数应为零。</param>
        /// <returns>返回一个状态代码，指示函数的成功或失败。</returns>
        [DllImport(Bcrypt, CharSet = CharSet.Unicode, EntryPoint = "BCryptHashData", PreserveSig = true, SetLastError = false)]
        public static extern uint BCryptHashData(nint hHash, byte[] pbInput, int cbInput, uint dwFlags);

        /// <summary>
        /// BCryptFinishHash 函数 (MAC 检索哈希或消息身份验证代码) 之前调用 BCryptHashData 所累积的数据的值。
        /// </summary>
        /// <param name="hHash">用于计算哈希或 MAC 的哈希或 MAC 对象的句柄。 此句柄是通过调用 BCryptCreateHash 函数获取的。 调用此函数后，传递给此函数的哈希句柄不能再次使用，除非在调用 BCryptDestroyHash 时。</param>
        /// <param name="pbOutput">指向接收哈希或 MAC 值的缓冲区的指针。 cbOutput 参数包含此缓冲区的大小。</param>
        /// <param name="cbOutput">
        /// pbOutput 缓冲区的大小（以字节为单位）。 此大小必须与哈希值或 MAC 值的大小完全匹配。
        /// 可以通过调用 BCryptGetProperty 函数获取 BCRYPT_HASH_LENGTH 属性来获取 大小 。 这将提供指定算法的哈希或 MAC 值的大小。
        /// </param>
        /// <param name="dwFlags">一组标志，用于修改此函数的行为。 当前未定义任何标志，因此此参数应为零。</param>
        /// <returns>返回指示函数成功或失败的状态代码。</returns>
        [DllImport(Bcrypt, CharSet = CharSet.Unicode, EntryPoint = "BCryptFinishHash", PreserveSig = true, SetLastError = false)]
        public static extern uint BCryptFinishHash(nint hHash, byte[] pbOutput, int cbOutput, uint dwFlags);
    }
}
