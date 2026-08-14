using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using System;

namespace PowerToolbox.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        public static Visibility IsCurrentItem(SelectorBarItem selectedItem, SelectorBarItem comparedItem)
        {
            return Equals(selectedItem, comparedItem) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查下载任务状态
        /// </summary>
        public static Visibility CheckDownloadProgressStateVisibility(DownloadProgressState downloadProgressState, DownloadProgressState comparedDownloadProgressState, bool needReverse)
        {
            return needReverse ? Equals(downloadProgressState, comparedDownloadProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(downloadProgressState, comparedDownloadProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查文件是否正在下载中
        /// </summary>
        public static Visibility CheckDownloadingStateVisibility(DownloadProgressState downloadProgressState)
        {
            return downloadProgressState is DownloadProgressState.Queued || downloadProgressState is DownloadProgressState.Downloading ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查文件解锁状态
        /// </summary>
        public static Visibility CheckFileUnlockStateVisibility(FileUnlockState fileUnlockState, FileUnlockState comparedFileUnlockState)
        {
            return Equals(fileUnlockState, comparedFileUnlockState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查更新应用状态
        /// </summary>
        public static Visibility CheckUpdateAppResultKindVisibility(UpdateAppResultKind updateAppResultKind, UpdateAppResultKind comparedUpdateAppResultKind)
        {
            return Equals(updateAppResultKind, comparedUpdateAppResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查模拟更新类型
        /// </summary>
        public static Visibility CheckSimulateUpdateKindVisibility(SimulateUpdateKind simulateUpdateKind, SimulateUpdateKind comparedSimulateUpdateKind)
        {
            return Equals(simulateUpdateKind, comparedSimulateUpdateKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查选择的保存图标格式
        /// </summary>
        public static Visibility CheckIconFormatVisibility(object iconFormatKey, object comparedIconFormatKey)
        {
            return string.Equals(Convert.ToString(iconFormatKey), Convert.ToString(comparedIconFormatKey)) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
