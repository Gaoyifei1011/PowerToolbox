using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FileRenamer.Views.CustomControls.BasicInput
{
    /// <summary>
    /// ��չ��İ�ť����������ָ��λ�ڿؼ���ʱ��ʾ���α�
    /// </summary>
    public sealed partial class ExtendedButton : Button
    {
        private CoreCursor DefaultCursor { get; } = new CoreCursor(CoreCursorType.Arrow, 0);

        public ExtendedButton()
        {
            InitializeComponent();
        }

        public CoreCursorType Cursor
        {
            get { return (CoreCursorType)GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(CoreCursorType), typeof(ExtendedButton), new PropertyMetadata(CoreCursorType.Arrow));

        protected override void OnPointerEntered(PointerRoutedEventArgs args)
        {
            base.OnPointerEntered(args);
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(Cursor, 0);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs args)
        {
            base.OnPointerExited(args);
            Window.Current.CoreWindow.PointerCursor = DefaultCursor;
        }
    }
}
