﻿using System;
using System.Collections.Generic;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 下载速度转换辅助类
    /// </summary>
    public static class SpeedHelper
    {
        private static readonly Dictionary<string, int> speedDict = new()
        {
            { "GB/s",1024*1024*1024 },
            { "MB/s",1024*1024 },
            { "KB/s",1024 }
        };

        /// <summary>
        /// 下载速度文字显示格式化
        /// </summary>
        public static string ConvertSpeedToString(double speed)
        {
            if (speed / speedDict["GB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / speedDict["GB/s"], 2), "GB/s");
            }
            else if (speed / speedDict["MB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / speedDict["MB/s"], 2), "MB/s");
            }
            else if (speed / speedDict["KB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / speedDict["KB/s"], 2), "KB/s");
            }
            else
            {
                return string.Format("{0}{1}", speed, "Byte/s");
            }
        }
    }
}
