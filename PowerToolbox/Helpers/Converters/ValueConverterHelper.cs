using System;
using Windows.UI.Xaml;

namespace PowerToolbox.Helpers.Converters
{
    /// <summary>
    /// 值类型 / 内容转换辅助类
    /// </summary>
    public static class ValueConverterHelper
    {
        /// <summary>
        /// 计算当前文件的下载进度
        /// </summary>
        public static double DownloadProgress(double finishedSize, double totalSize)
        {
            return totalSize is 0 ? 0 : Math.Round(finishedSize / totalSize * 100, 2);
        }

        /// <summary>
        /// 布尔值取反
        /// </summary>
        public static bool BooleanReverseConvert(bool value)
        {
            return !value;
        }

        /// <summary>
        /// 字符串布尔值转换
        /// </summary>
        public static bool StringToBooleanConvert(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool ObjectCompareReverseConvert(object value, object comparedValue)
        {
            return !Equals(value, comparedValue);
        }

        /// <summary>
        /// 布尔值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility BooleanToVisibilityReverseConvert(bool value)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 整数值与控件显示值转换
        /// </summary>
        public static Visibility IntToVisibilityConvert(int value)
        {
            return value is not 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 整数值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility IntToVisibilityReverseConvert(int value)
        {
            return value is 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 字符串与控件显示值转换
        /// </summary>
        public static Visibility StringToVisibilityConvert(string value)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 字符串与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility StringToVisibilityReverseConvert(string value)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 对象值与控件显示值转换
        /// </summary>
        public static Visibility ObjectToVisibilityConvert(object value, object comparedValue)
        {
            return Equals(value, comparedValue) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
