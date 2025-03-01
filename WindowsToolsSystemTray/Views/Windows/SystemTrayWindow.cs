using System.ComponentModel;
using System.Windows.Forms;

namespace WindowsToolsSystemTray.Views.Windows
{
    /// <summary>
    /// ���̳���������
    /// </summary>
    public class SystemTrayWindow : Form
    {
        private readonly Container components = new();

        public static SystemTrayWindow Current { get; private set; }

        public SystemTrayWindow()
        {
            Current = this;
        }

        /// <summary>
        /// ������������ռ�õ���Դ
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && components is not null)
            {
                components.Dispose();
            }
        }
    }
}
