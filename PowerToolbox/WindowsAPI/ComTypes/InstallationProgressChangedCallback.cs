using System;
using WUApiLib;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 定义 Invoke 方法，该方法处理有关异步安装或卸载正在进行的进度的通知。 此接口由调用 IUpdateInstaller.BeginInstall 方法或 IUpdateInstaller.BeginUninstall 方法的程序员实现。
    /// </summary>
    internal sealed class InstallationProgressChangedCallback : IInstallationProgressChangedCallback
    {
        internal IInstallationJob InstallationJob { get; private set; }

        internal IInstallationProgressChangedCallbackArgs CallbackArgs { get; private set; }

        internal event EventHandler InstallationProgressChanged;

        /// <summary>
        /// 处理异步安装或卸载过程中的更改通知，该通知是通过调用 IUpdateInstaller.BeginInstall 方法或 IUpdateInstaller.BeginUninstall 方法启动的。
        /// </summary>
        public void Invoke(IInstallationJob installationJob, IInstallationProgressChangedCallbackArgs callbackArgs)
        {
            InstallationJob = installationJob;
            CallbackArgs = callbackArgs;
            InstallationProgressChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
