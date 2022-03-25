using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class LuckyUpdaterConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "升级器版本",
                IconName = "Icon_Update",
                Width = 500,
                Height = 180,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new LuckyUpdaterConfig();
                window.BuildCloseWindowOncePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public LuckyUpdaterConfigViewModel Vm { get; private set; }

        public LuckyUpdaterConfig() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new LuckyUpdaterConfigViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
