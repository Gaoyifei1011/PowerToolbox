using System;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 驱动操作数据模型
    /// </summary>
    internal sealed class DriverOperationModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 驱动操作是否正在进行中
        /// </summary>
        internal bool IsOperating
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 驱动操作信息
        /// </summary>
        internal string DriverOperation
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DriverOperation)));
                }
            }
        }

        /// <summary>
        /// 驱动 INF 名称
        /// </summary>
        internal string DriverInfName { get; set; }

        /// <summary>
        /// 驱动操作唯一 ID
        /// </summary>
        internal Guid DriverOperationGuid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
