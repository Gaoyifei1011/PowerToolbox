using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件名称模型
    /// </summary>
    internal sealed class OldAndNewNameModel : INotifyPropertyChanged
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
        /// 文件的初始名称
        /// </summary>
        internal string OriginalFileName
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(OriginalFileName)));
                }
            }
        }

        /// <summary>
        /// 文件的初始路径
        /// </summary>
        internal string OriginalFilePath
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(OriginalFilePath)));
                }
            }
        }

        /// <summary>
        /// 文件新名称
        /// </summary>
        internal string NewFileName
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(NewFileName)));
                }
            }
        }

        /// <summary>
        /// 文件新名称
        /// </summary>
        internal string NewFilePath
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(NewFilePath)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
