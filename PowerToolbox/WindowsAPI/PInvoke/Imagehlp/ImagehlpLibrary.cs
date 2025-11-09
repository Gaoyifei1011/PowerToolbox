using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Imagehlp
{
    /// <summary>
    /// Imagehlp.dll 函数库
    /// </summary>
    public static class ImagehlpLibrary
    {
        private const string Imagehlp = "imagehlp.dll";

        /// <summary>
        /// 从给定文件中删除指定的证书。
        /// </summary>
        /// <param name="handle">要修改的图像文件的句柄。 必须打开此句柄才能FILE_READ_DATA和FILE_WRITE_DATA访问。</param>
        /// <param name="index">要删除的证书的索引。</param>
        /// <returns>如果函数成功，则返回值为 TRUE。如果函数失败，则返回值为 FALSE。</returns>
        [DllImport(Imagehlp, CharSet = CharSet.Unicode, EntryPoint = "ImageRemoveCertificate", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImageRemoveCertificate(nint handle, int index);
    }
}
