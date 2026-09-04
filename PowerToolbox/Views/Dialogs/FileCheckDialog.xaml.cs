using Microsoft.UI.Xaml.Controls;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 下载文件检查对话框
    /// </summary>
    internal sealed partial class FileCheckDialog : ContentDialog
    {
        #region 第一部分：构造函数

        internal FileCheckDialog()
        {
            InitializeComponent();
        }

        #endregion 第一部分：构造函数
    }
}
