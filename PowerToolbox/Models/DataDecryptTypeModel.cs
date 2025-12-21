using PowerToolbox.Extensions.DataType.Enums;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据解密类型数据模型
    /// </summary>
    public class DataDecryptTypeModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 数据解密类型是否被选择
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
        /// 数据解密类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据解密类型
        /// </summary>
        public DataDecryptType DataDecryptType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
