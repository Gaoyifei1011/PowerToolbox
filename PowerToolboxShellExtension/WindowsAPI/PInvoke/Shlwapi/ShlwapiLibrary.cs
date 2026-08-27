using System.Runtime.InteropServices;

namespace PowerToolboxShellExtension.WindowsAPI.PInvoke.Shlwapi
{
    /// <summary>
    /// Shlwapi.dll 函数库
    /// </summary>
    internal static partial class ShlwapiLibrary
    {
        private const string Shlwapi = "shlwapi.dll";

        /// <summary>
        /// 尝试通过查询具有 GetWindow 方法的各种接口，从组件对象模型 (COM) 对象检索窗口句柄。
        /// </summary>
        /// <param name="punk">指向 COM 对象的指针，此函数将尝试从中获取窗口句柄。</param>
        /// <param name="phwnd">指向 HWND 的指针，此函数成功返回时接收窗口句柄。 如果未获取窗口句柄，此参数将设置为 NULL。</param>
        /// <returns>如果成功返回窗口句柄，则返回S_OK，否则返回 COM 错误代码。 如果未找到合适的接口，该函数将返回E_NOINTERFACE。 否则，该函数返回由相应接口的 GetWindow 方法返回的 HRESULT。</returns>
        [LibraryImport(Shlwapi, EntryPoint = "IUnknown_GetWindow", SetLastError = false), PreserveSig]
        internal static partial int IUnknown_GetWindow(nint punk, out nint phwnd);
    }
}
