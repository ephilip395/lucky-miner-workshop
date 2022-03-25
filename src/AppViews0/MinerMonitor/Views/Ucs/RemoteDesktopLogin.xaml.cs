using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class RemoteDesktopLogin : UserControl
    {
        public static void ShowWindow(RemoteDesktopLoginViewModel vm)
        {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel
            {
                Title = vm.Title,
                IconName = "Icon_RemoteDesktop",
                Width = 400,
                Height = 190,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) =>
            {
                window.BuildCloseWindowOncePath(vm.Id);
                return new RemoteDesktopLogin(vm);
            }, beforeShow: (window, uc) =>
            {
                uc.DoFocus();
            }, fixedSize: true);
        }

        public RemoteDesktopLogin(RemoteDesktopLoginViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void DoFocus()
        {
            if (string.IsNullOrEmpty(TbLoginName.Text))
            {
                TbLoginName.Focus();
            }
            else if (string.IsNullOrEmpty(PbPassword.Password))
            {
                PbPassword.Focus();
            }
        }
    }
}
