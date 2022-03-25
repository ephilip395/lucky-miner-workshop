using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class MinerNamesSeter : UserControl {
        public static void ShowWindow(MinerNamesSeterViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "群控名",
                IsMaskTheParent = true,
                Width = 270,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerName"
            }, ucFactory: (window) => {
                window.BuildCloseWindowOncePath(vm.Id);
                return new MinerNamesSeter(vm);
            }, fixedSize: true);
        }

        public MinerNamesSeter(MinerNamesSeterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
