using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace FileRenamer.Views.CustomControls.Navigation
{
    /// <summary>
    /// ��չ��ĵ����ؼ�����������
    /// </summary>
    public partial class ExtendedNavigationViewItem : NavigationViewItem
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }

            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ExtendedNavigationViewItem), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }

            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExtendedNavigationViewItem), new PropertyMetadata(null));

        public string ToolTip
        {
            get { return (string)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(string), typeof(ExtendedNavigationViewItem), new PropertyMetadata(string.Empty));

        public ExtendedNavigationViewItem()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(ToolTip))
            {
                ToolTip NavigationItemToolTip = new ToolTip();
                NavigationItemToolTip.Content = string.Format("{0} ", ToolTip);
                NavigationItemToolTip.Placement = PlacementMode.Bottom;
                NavigationItemToolTip.VerticalOffset = 20;
                ToolTipService.SetToolTip(this, NavigationItemToolTip);
            }
            Loaded -= OnLoaded;
        }

        /// <summary>
        /// ��������ؼ���ʱ��������
        /// </summary>
        protected override void OnTapped(TappedRoutedEventArgs args)
        {
            base.OnTapped(args);

            ICommand clickCommand = Command;
            clickCommand?.Execute(CommandParameter);
        }
    }
}
