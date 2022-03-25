using Lucky.MinerMonitor.Vms;
using System.Windows;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class MinerTweakSetting : BlankWindow {
        public static void ShowWindow(MinerTweakSettingViewModel vm) {
            Window window = new MinerTweakSetting(vm);
            window.BuildCloseWindowOncePath(vm.Id);
            //window.MousePosition();
            window.ShowSoftDialog();
        }

        public MinerTweakSetting(MinerTweakSettingViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            var owner = WpfUtil.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
        }
    }
}
