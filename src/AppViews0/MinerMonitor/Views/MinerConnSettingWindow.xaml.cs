
using System.Windows;
using System.Windows.Input;
using Lucky.MinerMonitor.Vms;

namespace Lucky.MinerMonitor.Views
{
    /// <summary>
    /// Interaction logic for MinerConnSettingWindow.xaml
    /// </summary>
    public partial class MinerConnSettingWindow : Window
    {
        public static void ShowWindow(MinerConnSettingViewModel vm)
        {
            Window window = new MinerConnSettingWindow(vm);
            window.BuildCloseWindowOncePath(vm.Id);
            window.ShowSoftDialog();
        }

        public MinerConnSettingWindow(MinerConnSettingViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
            var owner = WpfUtil.GetTopWindow();
            if (this != owner)
            {
                this.Owner = owner;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grid1.Focus();
        }
    }
}
