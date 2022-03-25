using Lucky.Vms;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lucky.Views.Ucs
{
    public partial class MinerProfileOption : UserControl
    {
        public MinerProfileViewModel Vm { get; private set; }

        public MinerProfileOption()
        {
            this.Vm = AppRoot.MinerProfileVm;
            this.DataContext = AppRoot.MinerProfileVm;
            InitializeComponent();
            this.OnLoaded(window =>
            {
                VirtualRoot.BuildEventPath<SignUpedEvent>(
                    "注册了新外网群控用户后自动填入外网群控用户名", 
                    LogEnum.None, this.GetType(), PathPriority.Normal, path: message =>
                {
                    this.Vm.OuterUserId = message.LoginName;
                });
            });
        }

        private void ButtonHotKey_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z)
            {
                Vm.HotKey = e.Key.ToString();
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }



        private void TbWsDaemonStateDescription_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Vm.RefreshWsDaemonState();
        }
    }
}
