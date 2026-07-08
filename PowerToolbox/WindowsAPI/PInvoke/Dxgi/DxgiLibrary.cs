using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Dxgi
{
    /// <summary>
    /// Dxgi.dll 函数库
    /// </summary>
    public static class DxgiLibrary
    {
        private const string Dxgi = "dxgi.dll";

        /// <summary>
        /// 创建可用于生成其他 DXGI 对象的 DXGI 1.0 工厂。
        /// </summary>
        /// <param name="riid">全局唯一标识符 (ppFactory 参数引用的 IDXGIFactory 对象的 GUID) 。</param>
        /// <param name="ppFactory">指向 IDXGIFactory 对象的指针的地址。</param>
        /// <returns>如果成功 ， 则返回S_OK;否则， 返回以下 DXGI_ERROR之一。</returns>
        [DllImport(Dxgi, CharSet = CharSet.Unicode, EntryPoint = "CreateDXGIFactory", PreserveSig = true, SetLastError = false)]
        public static extern int CreateDXGIFactory(ref Guid riid, out nint ppFactory);
    }
}
