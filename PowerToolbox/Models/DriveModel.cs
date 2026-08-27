using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.IO;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 驱动器数据模型
    /// </summary>
    internal class DriveModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        internal bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                }
            }
        }

        internal ImageSource DiskImage { get; set; }

        /// <summary>
        /// 驱动器名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 驱动器空间
        /// </summary>
        internal string Space { get; set; }

        /// <summary>
        /// 是否为系统卷
        /// </summary>
        internal bool IsSystemDrive { get; set; }

        /// <summary>
        /// 驱动器已使用空间百分比
        /// </summary>
        internal double DriveUsedPercentage { get; set; }

        /// <summary>
        /// 驱动器可用空间警告（可用空间在 5% - 10%）
        /// </summary>
        internal bool IsAvailableSpaceWarning { get; set; }

        /// <summary>
        /// 存储空间是否不可用（可用空间在 0% - 5%）
        /// </summary>
        internal bool IsAvailableSpaceError { get; set; }

        /// <summary>
        /// 驱动器信息
        /// </summary>
        internal DriveInfo DriveInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
