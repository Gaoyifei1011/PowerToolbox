using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开方法，这些方法为应用程序提供显示进度对话框的选项。 此接口由进度对话框对象 (CLSID_ProgressDialog) 导出。 此对象是向用户显示操作进度的通用方法。 它通常在删除、上传、复制、移动或下载大量文件时使用。
    /// </summary>
    [ComImport, Guid("EBBC7C04-315E-11d2-B62F-006097DF5BD4"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IProgressDialog
    {
        /// <summary>
        /// 启动进度对话框。
        /// </summary>
        /// <param name="hwndParent">对话框的父窗口的句柄。</param>
        /// <param name="punkEnableModless">保留。 设置为 NULL。</param>
        /// <param name="dwFlags">控制进度对话框操作的标志。 </param>
        /// <param name="pvResevered">保留。 设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int StartProgressDialog(nint hwndParent, [MarshalAs(UnmanagedType.IUnknown)] object punkEnableModless, PROGDLG dwFlags, nint pvResevered);

        /// <summary>
        /// 停止进度对话框并将其从屏幕中删除。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int StopProgressDialog();

        /// <summary>
        /// 设置进度对话框的标题。
        /// </summary>
        /// <param name="pwzTitle">指向包含对话框标题的以 null 结尾的 Unicode 字符串的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pwzTitle);

        /// <summary>
        /// 指定在对话框中运行的 Audio-Video 交错 (AVI) 剪辑。
        /// </summary>
        /// <param name="hInstAnimation">应从中加载 AVI 资源的模块的实例句柄。</param>
        /// <param name="idAnimation">AVI 资源标识符。 若要创建此值，请使用 MAKEINTRESOURCE 宏。 控件从 hInstAnimation 指定的模块加载 AVI 资源。</param>
        /// <returns>如果成功，则返回 S_OK ，否则返回错误值。 在 Windows Vista 及更高版本中， 始终返回 S_OK。</returns>
        [PreserveSig]
        int SetAnimation(nint hInstAnimation, uint idAnimation);

        /// <summary>
        /// 检查用户是否已取消操作。
        /// </summary>
        /// <returns>如果用户已取消操作，则为 TRUE;否则为 FALSE。</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool HasUserCanceled();

        /// <summary>
        /// 汇报进度对话框，其中包含操作的当前状态。
        /// </summary>
        /// <param name="dwCompleted">应用程序定义的值，指示调用方法时已完成的操作比例。</param>
        /// <param name="dwTotal">应用程序定义的值，指定当操作完成时 dwCompleted 将具有的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetProgress(uint dwCompleted, uint dwTotal);

        /// <summary>
        /// 汇报包含操作的当前状态的进度对话框。
        /// </summary>
        /// <param name="ullCompleted">一个应用程序定义的值，指示调用方法时已完成操作的比例。</param>
        /// <param name="ullTotal">一个应用程序定义的值，该值指定操作完成时 ullCompleted 将具有的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetProgress64(ulong ullCompleted, ulong ullTotal);

        /// <summary>
        /// 在进度对话框中显示一条消息。
        /// </summary>
        /// <param name="dwLineNum">
        /// 要显示文本的行号。 目前有三行 - 1、2 和 3。 如果在调用 IProgressDialog：：StartProgressDialog 时 dwFlags 参数中包含 PROGDLG_AUTOTIME 标志，则只能使用第 1 行和第 2 行。 预计时间将显示在第 3 行。
        /// </param>
        /// <param name="pwzString">包含文本的以 null 结尾的 Unicode 字符串。</param>
        /// <param name="fCompactPath">如果 路径字符串太大而无法容纳在行上，则为 TRUE。 路径使用 PathCompactPath 进行压缩。</param>
        /// <param name="pvResevered">保留。 设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetLine(uint dwLineNum, [MarshalAs(UnmanagedType.LPWStr)] string pwzString, [MarshalAs(UnmanagedType.VariantBool)] bool fCompactPath, nint pvResevered);

        /// <summary>
        /// 设置在用户取消操作时要显示的消息。
        /// </summary>
        /// <param name="pwzCancelMsg">指向以 null 结尾的 Unicode 字符串的指针，该字符串包含要显示的消息。</param>
        /// <param name="pvResevered">保留。 设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetCancelMsg([MarshalAs(UnmanagedType.LPWStr)] string pwzCancelMsg, nint pvResevered);

        /// <summary>
        /// 将进度对话框计时器重置为零。
        /// </summary>
        /// <param name="dwTimerAction">指示计时器要执行的操作的标志。</param>
        /// <param name="pvResevered">保留。 设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Timer(PDTIMER dwTimerAction, [MarshalAs(UnmanagedType.IUnknown)] object pvResevered);
    }
}
