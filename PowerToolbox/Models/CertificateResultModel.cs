using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件证书操作结果数据模型
    /// </summary>
    internal sealed class CertificateResultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否处于修改状态中
        /// </summary>
        internal bool IsModifyingNow
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyingNow)));
                }
            }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        internal string FileName
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileName)));
                }
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        internal string FilePath
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(FilePath)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
