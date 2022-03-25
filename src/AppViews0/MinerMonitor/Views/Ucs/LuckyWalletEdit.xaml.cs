using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class LuckyWalletEdit : UserControl {
        public static void ShowWindow(FormType formType, LuckyWalletViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Lucky钱包",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 520,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Wallet"
            }, ucFactory: (window) => {
                LuckyWalletViewModel vm = new LuckyWalletViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new LuckyWalletEdit(vm);
            }, fixedSize: true);
        }

        public LuckyWalletViewModel Vm { get; private set; }

        public LuckyWalletEdit(LuckyWalletViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
