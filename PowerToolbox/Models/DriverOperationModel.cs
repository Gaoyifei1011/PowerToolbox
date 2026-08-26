using System;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 驱动操作数据模型
    /// </summary>
    public class DriverOperationModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 驱动操作是否正在进行中
        /// </summary>
        private bool _isOperating;

        public bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 驱动操作信息
        /// </summary>
        private string _driverOperation;

        public string DriverOperation
        {
            get { return _driverOperation; }

            set
            {
                if (!string.Equals(_driverOperation, value))
                {
                    _driverOperation = value;
                    PropertyChanged?.Invoke(this, new(nameof(DriverOperation)));
                }
            }
        }

        /// <summary>
        /// 驱动 INF 名称
        /// </summary>
        public string DriverInfName { get; set; }

        /// <summary>
        /// 驱动操作唯一 ID
        /// </summary>
        public Guid DriverOperationGuid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
