using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据加密校验结果数据模型
    /// </summary>
    public class DataVerifyResultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 加密 / 校验名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 加密 / 校验后的结果
        /// </summary>
        private string _result;

        public string Result
        {
            get { return _result; }

            set
            {
                if (!Equals(_result, value))
                {
                    _result = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
