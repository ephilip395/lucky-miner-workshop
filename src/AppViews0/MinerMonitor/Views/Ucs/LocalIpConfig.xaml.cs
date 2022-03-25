using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class LocalIpConfig : UserControl
    {
        public static void ShowWindow(LocalIpConfigViewModel vm)
        {
            ContainerWindow.ShowWindow(new Lucky.Vms.ContainerWindowViewModel
            {
                Title = "远程管理矿机 IP",
                IconName = "Icon_Ip",
                Width = 450,
                IsMaskTheParent = true,
                FooterVisible = Visibility.Collapsed,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) =>
            {
                var uc = new LocalIpConfig(vm);
                window.BuildCloseWindowOncePath(uc.Vm.Id);
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        window.DragMove();
                    }
                };
                window.BuildEventPath<GetLocalIpsResponsedEvent>("收到了获取挖矿端Ip的响应", LogEnum.DevConsole, typeof(LocalIpConfig), PathPriority.Normal, path: message =>
                {
                    if (message.ClientId != vm.MinerTweakVm.ClientId)
                    {
                        return;
                    }
                    vm.LocalIpVms = message.Data.Select(a => new Lucky.Vms.LocalIpViewModel(a)).ToList();
                });
                MinerMonitorRoot.MinerMonitorService.GetLocalIpsAsync(vm.MinerTweakVm);
                return uc;
            }, fixedSize: true);
        }

        public LocalIpConfigViewModel Vm
        {
            get; private set;
        }

        public LocalIpConfig(LocalIpConfigViewModel vm)
        {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
