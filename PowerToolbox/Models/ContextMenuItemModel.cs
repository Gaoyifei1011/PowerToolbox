using PowerToolbox.Extensions.DataType.Enums;
using System;
using System.ComponentModel;
using System.Threading;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 右键菜单 ID 项
    /// </summary>
    public class ContextMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单是否启用
        /// </summary>
        private bool _isEnabled;

        public bool IsEnabled
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
        public Guid Clsid { get; set; }

        /// <summary>
        /// 菜单 GUID 显示字符串
        /// </summary>
        public string ClsidString { get; set; }

        /// <summary>
        /// 菜单 DLL 路径
        /// </summary>
        public string DllPath { get; set; }

        /// <summary>
        /// 菜单线程模型
        /// </summary>
        public ApartmentState ThreadingMode { get; set; }

        /// <summary>
        /// 菜单阻止类型及原因
        /// </summary>
        public BlockedClsidType BlockedClsidType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
