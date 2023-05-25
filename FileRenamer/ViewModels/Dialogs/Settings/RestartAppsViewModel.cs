﻿using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.ViewModels.Dialogs.Settings
{
    /// <summary>
    /// 应用重启对话框视图模型
    /// </summary>
    public sealed class RestartAppsViewModel
    {
        /// <summary>
        /// 重启应用
        /// </summary>
        public void OnRestartAppsClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Program.AppMutex.ReleaseMutex();
            Application.Restart();
        }
    }
}
