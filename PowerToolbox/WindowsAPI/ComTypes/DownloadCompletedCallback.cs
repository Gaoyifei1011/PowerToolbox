using System;
using WUApiLib;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供异步下载完成时使用的回调。 此接口由调用 IUpdateDownloader：：BeginDownload 方法的程序员实现。
    /// </summary>
    internal class DownloadCompletedCallback : IDownloadCompletedCallback
    {
        internal IDownloadJob DownloadJob { get; private set; }

        internal IDownloadCompletedCallbackArgs CallbackArgs { get; private set; }

        internal event EventHandler DownloadCompleted;

        /// <summary>
        /// 处理通过调用 IUpdateInstaller.BeginInstall 或 IUpdateInstaller.BeginUninstall 启动的异步安装或卸载完成的通知。
        /// </summary>
        public void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
        {
            DownloadJob = downloadJob;
            CallbackArgs = callbackArgs;
            DownloadCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
