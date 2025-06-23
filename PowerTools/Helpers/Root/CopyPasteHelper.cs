﻿using PowerTools.Services.Root;
using System;
using System.Diagnostics.Tracing;
using System.Windows.Forms;

namespace PowerTools.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    public static class CopyPasteHelper
    {
        /// <summary>
        /// 复制字符串内容到剪贴板
        /// </summary>
        public static bool CopyToClipboard(string content)
        {
            try
            {
                Clipboard.SetDataObject(content);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerTools), nameof(CopyPasteHelper), nameof(CopyToClipboard), 1, e);
                return false;
            }
        }
    }
}
