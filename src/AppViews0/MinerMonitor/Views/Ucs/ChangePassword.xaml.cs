using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class ChangePassword : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "修改密码",
                IsMaskTheParent = true,
                Width = 300,
                CloseVisible = Visibility.Visible,
                IconName = "Icon_Password"
            }, ucFactory: (window) => {
                var uc = new ChangePassword();
                window.BuildCloseWindowOncePath(uc.Vm.Id);
                return uc;
            }, beforeShow: (window, uc)=> {
                uc.DoFocus();
            }, fixedSize: true);
        }

        public ChangePasswordViewModel Vm { get; private set; }

        public ChangePassword() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new ChangePasswordViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DoFocus() {
            this.PbOldPassword.Focus();
        }
    }
}
