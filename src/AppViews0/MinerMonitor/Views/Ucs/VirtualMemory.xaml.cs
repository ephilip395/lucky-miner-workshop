using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow(VirtualMemoryViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "远程设置虚拟内存",
                IconName = "Icon_VirtualMemory",
                CloseVisible = Visibility.Visible,
                Width = 800,
                MinWidth = 450,
                Height = 360,
                MinHeight = 360,
                IsMaskTheParent = true,
            }, ucFactory: (window) => {
                MinerMonitorRoot.MinerMonitorService.GetDrivesAsync(vm.MinerTweakVm);
                window.BuildEventPath<GetDrivesResponsedEvent>("收到了GetDrives的响应时绑定到界面", LogEnum.DevConsole, typeof(VirtualMemory), PathPriority.Normal, path: message => {
                    if (message.ClientId != vm.MinerTweakVm.ClientId) {
                        return;
                    }
                    vm.Drives = message.Data.Select(a => new DriveViewModel(a)).ToList();
                });
                return new VirtualMemory(vm);
            }, fixedSize: false);
        }

        public VirtualMemoryViewModel Vm { get; private set; }

        public VirtualMemory(VirtualMemoryViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
