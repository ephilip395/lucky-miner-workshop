using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class MinerFinderConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "矿机雷达版本",
                IconName = "Icon_MinerFinder",
                Width = 500,
                Height = 180,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new MinerFinderConfig();
                window.BuildCloseWindowOncePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public MinerFinderConfigViewModel Vm { get; private set; }

        public MinerFinderConfig() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new MinerFinderConfigViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
