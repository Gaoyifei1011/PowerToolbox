using System;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 驱动数据模型
    /// </summary>
    public class DriverModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 驱动是否已经被选择
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 驱动名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 驱动 INF 名称
        /// </summary>
        public string DriverInfName { get; set; }

        /// <summary>
        /// 驱动 OEM INF 名称
        /// </summary>
        public string DriverOEMInfName { get; set; }

        /// <summary>
        /// 驱动类别
        /// </summary>
        public string DriverType { get; set; }

        /// <summary>
        /// 驱动制造商
        /// </summary>
        public string DriverManufacturer { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        public Version DriverVersion { get; set; }

        /// <summary>
        /// 驱动日期
        /// </summary>
        public DateTimeOffset DriverDate { get; set; }

        /// <summary>
        /// 驱动大小
        /// </summary>
        public string DriverSize { get; set; }

        /// <summary>
        /// 驱动路径
        /// </summary>
        public string DriverLocation { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string SignatureName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
