using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class SignUpPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "创建账号",
                IconName = "Icon_SignUp",
                Width = 370,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => {
                SignUpPageViewModel vm = new SignUpPageViewModel();
                window.BuildCloseWindowOncePath(vm.Id);
                return new SignUpPage(vm);
            }, beforeShow: (window, uc)=> {
                uc.DoFocus();
            }, fixedSize: true);
        }

        public SignUpPageViewModel Vm { get; private set; }

        public SignUpPage(SignUpPageViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void DoFocus() {
            this.TbLoginName.Focus();
        }
    }
}
