using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class LuckyWalletPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Lucky钱包",
                IconName = "Icon_Wallet",
                Width = 800,
                Height = 400,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new LuckyWalletPage(), fixedSize: true);
        }

        public LuckyWalletPageViewModel Vm { get; private set; }

        public LuckyWalletPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new LuckyWalletPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<LuckyWalletViewModel>(sender, e);
        }
    }
}
