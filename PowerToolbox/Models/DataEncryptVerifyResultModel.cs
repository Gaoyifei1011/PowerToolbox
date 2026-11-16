using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据加密校验结果数据模型
    /// </summary>
    internal sealed class DataVerifyResultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 加密 / 校验名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 加密 / 校验后的结果
        /// </summary>
        internal string Result
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Result)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
