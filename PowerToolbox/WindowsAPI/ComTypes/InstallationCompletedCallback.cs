using System;
using WUApiLib;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 处理指示异步安装或卸载已完成的通知。 此接口由调用 IUpdateInstaller.BeginInstall 或 IUpdateInstaller.BeginUninstall 方法的程序员实现。
    /// </summary>
    internal class InstallationCompletedCallback : IInstallationCompletedCallback
    {
        internal IInstallationJob InstallationJob { get; private set; }

        internal IInstallationCompletedCallbackArgs CallbackArgs { get; private set; }

        internal event EventHandler InstallationCompleted;

        /// <summary>
        /// 处理通过调用 IUpdateInstaller.BeginInstall 或 IUpdateInstaller.BeginUninstall 启动的异步安装或卸载完成的通知。
        /// </summary>
        public void Invoke(IInstallationJob installationJob, IInstallationCompletedCallbackArgs callbackArgs)
        {
            InstallationJob = installationJob;
            CallbackArgs = callbackArgs;
            InstallationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
