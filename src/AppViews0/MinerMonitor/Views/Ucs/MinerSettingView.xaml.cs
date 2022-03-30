using Lucky.MinerMonitor.Vms;
using System.Windows;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerSettingView : Window
    {
        public static void ShowWindow(MinerSettingViewModel vm)
        {
            Window window = new MinerSettingView(vm);
            window.BuildCloseWindowOncePath(vm.Id);
            window.ShowSoftDialog();
        }

        public MinerSettingView(MinerSettingViewModel vm)
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
