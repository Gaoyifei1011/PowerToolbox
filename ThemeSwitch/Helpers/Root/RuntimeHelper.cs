using System.Security.Principal;
using ThemeSwitch.WindowsAPI.PInvoke.Kernel32;

namespace ThemeSwitch.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; private set; }

        public static bool IsElevated { get; private set; }

        static RuntimeHelper()
        {
            IsInMsixContainer();
            IsRunningElevated();
        }

        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        private static void IsInMsixContainer()
        {
            int length = 0;
            IsMSIX = Kernel32Library.GetCurrentPackageFamilyName(ref length, null) is not (int)Kernel32Library.APPMODEL_ERROR_NO_PACKAGE;
        }

        /// <summary>
        /// 判断应用是否以管理员身份运行
        /// </summary>
        private static void IsRunningElevated()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            IsElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
