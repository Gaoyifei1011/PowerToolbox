using PowerToolbox.Extensions.DataType.Enums;
using System;
using System.ComponentModel;
using System.Threading;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 右键菜单 ID 项
    /// </summary>
    internal class ContextMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单是否启用
        /// </summary>
        private bool _isEnabled;

        internal bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (!Equals(_isEnabled, value))
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsEnabled)));
                }
            }
        }

        /// <summary>
        /// 菜单 GUID
        /// </summary>
        internal Guid Clsid { get; set; }

        /// <summary>
        /// 菜单 GUID 显示字符串
        /// </summary>
        internal string ClsidString { get; set; }

        /// <summary>
        /// 菜单 DLL 路径
        /// </summary>
        internal string DllPath { get; set; }

        /// <summary>
        /// 菜单线程模型
        /// </summary>
        internal ApartmentState ThreadingMode { get; set; }

        /// <summary>
        /// 菜单阻止类型及原因
        /// </summary>
        internal BlockedClsidType BlockedClsidType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
