namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 视觉效果选项
    /// </summary>
    public class VisualEffects
    {
        /// <summary>
        /// 视觉效果方案
        /// </summary>
        public int VisualEffectsPlan { get; set; }

        /// <summary>
        /// 保存任务栏缩略图预览
        /// </summary>
        public bool SaveTaskbarThumbnailPreview { get; set; }

        /// <summary>
        /// 窗口内的动画控件和元素
        /// </summary>
        public bool AnimationControlsAndElementsInsideWindow { get; set; }

        /// <summary>
        /// 淡入淡出或滑动菜单到视图
        /// </summary>
        public bool FadeinAndOutOrSlideMenuToView { get; set; }

        /// <summary>
        /// 滑动打开组合框
        /// </summary>
        public bool SlideToOpenCombobox { get; set; }

        /// <summary>
        /// 平滑滚动列表框
        /// </summary>
        public bool SmoothScrollListbox { get; set; }

        /// <summary>
        /// 平滑屏幕字体边缘
        /// </summary>
        public bool SmoothScreenFontEdges { get; set; }

        /// <summary>
        /// 启用速览
        /// </summary>
        public bool EnablePeek { get; set; }

        /// <summary>
        /// 任务栏中的动画
        /// </summary>
        public bool TaskbarAnimations { get; set; }

        /// <summary>
        /// 拖动时显示窗口内容
        /// </summary>
        public bool ShowWindowContentsWhileDragging { get; set; }

        /// <summary>
        /// 显示缩略图，而不是显示图标
        /// </summary>
        public bool ShowThumbnail { get; set; }

        /// <summary>
        /// 显示亚透明的选择长方形
        /// </summary>
        public bool ShowSemitransparentSelectedRectangle { get; set; }

        /// <summary>
        /// 在窗口下显示阴影
        /// </summary>
        public bool ShowShadowUnderWindow { get; set; }

        /// <summary>
        /// 在单击后淡出菜单
        /// </summary>
        public bool FadeoutMenuAfterClicking { get; set; }

        /// <summary>
        /// 在视图中淡入淡出或滑动工具提示
        /// </summary>
        public bool FadeinFadeoutOrSlideToolTipInView { get; set; }

        /// <summary>
        /// 在鼠标指针下显示阴影
        /// </summary>
        public bool ShowShadowUnderMousePointer { get; set; }

        /// <summary>
        /// 在桌面上为图标标签使用阴影
        /// </summary>
        public bool UseShadowForIconLabelsOnDesktop { get; set; }

        /// <summary>
        /// 在最大化和最小化时显示窗口动画
        /// </summary>
        public bool ShowAnimationWhenMaximizingOrMinimizing { get; set; }
    }
}
