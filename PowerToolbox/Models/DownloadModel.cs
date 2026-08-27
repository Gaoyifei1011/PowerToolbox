using Microsoft.UI.Xaml.Media;
using PowerToolbox.Extensions.DataType.Enums;
using System.ComponentModel;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerToolbox.Models
{
    internal sealed class DownloadModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 下载操作是否正在进行中
        /// </summary>
        private bool _isOperating;

        internal bool IsOperating
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
        /// 文件图标
        /// </summary>
        private ImageSource _iconImage;

        internal ImageSource IconImage
        {
            get { return _iconImage; }

            set
            {
                if (!Equals(_iconImage, value))
                {
                    _iconImage = value;
                    PropertyChanged?.Invoke(this, new(nameof(IconImage)));
                }
            }
        }

        /// <summary>
        /// 任务在下载状态时，获取的GID码。该值唯一
        /// </summary>
        internal string DownloadID { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// 文件下载状态
        /// </summary>
        private DownloadProgressState _downloadProgressState;

        internal DownloadProgressState DownloadProgressState
        {
            get { return _downloadProgressState; }

            set
            {
                if (!Equals(_downloadProgressState, value))
                {
                    _downloadProgressState = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadProgressState)));
                }
            }
        }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        private double _completedSize;

        internal double CompletedSize
        {
            get { return _completedSize; }

            set
            {
                if (!Equals(_completedSize, value))
                {
                    _completedSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(CompletedSize)));
                }
            }
        }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        private double _totalSize;

        internal double TotalSize
        {
            get { return _totalSize; }

            set
            {
                if (!Equals(_totalSize, value))
                {
                    _totalSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(TotalSize)));
                }
            }
        }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        private double _downloadSpeed;

        internal double DownloadSpeed
        {
            get { return _downloadSpeed; }

            set
            {
                if (!Equals(_downloadSpeed, value))
                {
                    _downloadSpeed = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadSpeed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
