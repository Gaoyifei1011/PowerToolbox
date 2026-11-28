using PowerToolbox.Services.Root;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace PowerToolbox.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    internal static class CopyPasteHelper
    {
        /// <summary>
        /// 复制字符串内容到剪贴板
        /// </summary>
        internal static bool CopyTextToClipboard(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            try
            {
                Clipboard.SetText(content);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(CopyPasteHelper), nameof(CopyTextToClipboard), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 复制文件到剪贴板
        /// </summary>
        internal static bool CopyFileToClipBoard(ReadOnlyCollection<string> fileCollection)
        {
            if (fileCollection is null || fileCollection.Count is 0)
            {
                return false;
            }

            try
            {
                StringCollection stringCollection = [];
                stringCollection.AddRange([.. fileCollection]);
                Clipboard.SetFileDropList(stringCollection);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(CopyPasteHelper), nameof(CopyFileToClipBoard), 1, e);
                return false;
            }
        }
    }
}
