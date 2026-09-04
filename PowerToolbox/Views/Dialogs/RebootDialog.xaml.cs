using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    internal sealed partial class RebootDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string InstallDriverRebootString = ResourceService.DialogResource.GetString("InstallDriverReboot");
        private readonly string UnInstallDriverRebootString = ResourceService.DialogResource.GetString("UnInstallDriverReboot");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：构造函数

        internal RebootDialog(DriverInstallKind driverInstallKind)
        {
            InitializeComponent();
            switch (driverInstallKind)
            {
                case DriverInstallKind.InstallDriver:
                    {
                        Content = InstallDriverRebootString;
                        break;
                    }
                case DriverInstallKind.UnInstallDriver:
                    {
                        Content = UnInstallDriverRebootString;
                        break;
                    }
            }
        }

        #endregion 第二部分：构造函数
    }
}
