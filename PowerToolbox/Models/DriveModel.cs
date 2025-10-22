using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.IO;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 驱动器数据模型
    /// </summary>
    public class DriveModel : INotifyPropertyChanged
    {
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

        public ImageSource DiskImage { get; set; }

        /// <summary>
        /// 驱动器名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 驱动器空间
        /// </summary>
        public string Space { get; set; }

        /// <summary>
        /// 是否为系统卷
        /// </summary>
        public bool IsSytemDrive { get; set; }

        /// <summary>
        /// 驱动器已使用空间百分比
        /// </summary>
        public double DriveUsedPercentage { get; set; }

        /// <summary>
        /// 驱动器可用空间警告（可用空间在 5% - 10%）
        /// </summary>
        public bool IsAvailableSpaceWarning { get; set; }

        /// <summary>
        /// 存储空间是否不可用（可用空间在 0% - 5%）
        /// </summary>
        public bool IsAvailableSpaceError { get; set; }

        /// <summary>
        /// 驱动器信息
        /// </summary>
        public DriveInfo DriveInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
