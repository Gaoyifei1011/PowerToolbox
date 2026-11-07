using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 视觉效果数据模型
    /// </summary>
    internal sealed class VisualEffectsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 视觉效果名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 视觉效果标签
        /// </summary>
        internal string VisualTag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}