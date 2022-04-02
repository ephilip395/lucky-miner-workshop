using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class AboutPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "关于",
                IconName = "Icon_About",
                Width = 520,
                Height = 320,
                CloseVisible = System.Windows.Visibility.Visible,
                IsMaskTheParent = false,
                IsChildWindow = true
            }, ucFactory: (window) => {
                return new AboutPage();
            }, fixedSize: true);
        }

        public AboutPageViewModel Vm { get; private set; }

        public AboutPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new AboutPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
