using Lucky.MinerMonitor.Vms;
using System.Windows;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerSettingWindow : Window
    {
        public static void ShowWindow(MinerSettingViewModel vm)
        {
            Window window = new MinerSettingWindow(vm);
            window.BuildCloseWindowOncePath(vm.Id);
            window.ShowSoftDialog();
        }

        public MinerSettingWindow(MinerSettingViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
            var owner = WpfUtil.GetTopWindow();
            if (this != owner)
            {
                this.Owner = owner;
            }
        }
    }
}
