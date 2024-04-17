﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Views.Windows;

namespace WindowsTools.Helpers.Controls
{
    /// <summary>
    /// 扩展后的内容对话框辅助类，只允许在同一时间段内打开一个内容对话框
    /// </summary>
    public static class ContentDialogHelper
    {
        private static bool isDialogOpening = false;

        /// <summary>
        /// 显示对话框
        /// </summary>
        public static async Task<ContentDialogResult> ShowAsync(ContentDialog dialog, FrameworkElement element)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!isDialogOpening && dialog is not null && element is not null)
            {
                isDialogOpening = true;
                MainWindow.Current.FormBorderStyle = FormBorderStyle.FixedSingle;
                MainWindow.Current.MaximizeBox = false;
                dialog.XamlRoot = element.XamlRoot;
                dialog.RequestedTheme = element.ActualTheme;
                element.ActualThemeChanged += (sender, args) =>
                {
                    dialog.RequestedTheme = element.ActualTheme;
                };
                dialogResult = await dialog.ShowAsync();
                MainWindow.Current.FormBorderStyle = FormBorderStyle.Sizable;
                MainWindow.Current.MaximizeBox = true;
                isDialogOpening = false;
            }
            return dialogResult;
        }
    }
}
