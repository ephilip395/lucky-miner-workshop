using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class GpuProfilesPage : UserControl
    {
        public static void ShowWindow(MinersWindowViewModel minerClientsWindowVm)
        {
            if (minerClientsWindowVm.SelectedMinerTweaks == null && minerClientsWindowVm.SelectedMinerTweaks.Length != 1)
            {
                return;
            }
            var minerClientVm = minerClientsWindowVm.SelectedMinerTweaks[0];
            ContainerWindow.ShowWindow(new ContainerWindowViewModel
            {
                Title = $"超频 - 基于矿机{minerClientVm.MinerName}({minerClientVm.MinerIp})",
                IconName = "Icon_OverClock",
                Width = 800,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) =>
            {
                var vm = new GpuProfilesPageViewModel(minerClientsWindowVm);
                window.BuildCloseWindowOncePath(vm.Id);
                var uc = new GpuProfilesPage(vm);
                var client = minerClientsWindowVm.SelectedMinerTweaks[0];
                void onSelectedMinerTweaksChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(minerClientsWindowVm.SelectedMinerTweaks))
                    {
                        List<MinerViewModel> toRemoves = new List<MinerViewModel>();
                        foreach (var item in vm.MinerTweakVms)
                        {
                            if (item != minerClientVm)
                            {
                                var exist = minerClientsWindowVm.SelectedMinerTweaks.FirstOrDefault(a => a == item);
                                if (exist == null)
                                {
                                    toRemoves.Add(item);
                                }
                            }
                        }
                        foreach (var item in toRemoves)
                        {
                            vm.MinerTweakVms.Remove(item);
                        }
                        List<MinerViewModel> toAdds = new List<MinerViewModel>();
                        foreach (var item in minerClientsWindowVm.SelectedMinerTweaks)
                        {
                            var exist = vm.MinerTweakVms.FirstOrDefault(a => a == item);
                            if (exist == null)
                            {
                                toAdds.Add(item);
                            }
                        }
                        foreach (var item in toAdds)
                        {
                            vm.MinerTweakVms.Add(item);
                        }
                    }
                }

                minerClientsWindowVm.PropertyChanged += onSelectedMinerTweaksChanged;
                uc.Unloaded += (object sender, RoutedEventArgs e) =>
                {
                    minerClientsWindowVm.PropertyChanged -= onSelectedMinerTweaksChanged;
                };
                window.BuildEventPath<GetGpuProfilesResponsedEvent>("收到GetGpuProfilesJson的响应", LogEnum.DevConsole, typeof(GpuProfilesPage), PathPriority.Normal, path: message =>
                {
                    if (message.ClientId != minerClientVm.ClientId)
                    {
                        return;
                    }
                    vm.SetData(message.Data);
                });
                MinerMonitorRoot.MinerMonitorService.GetGpuProfilesJsonAsync(minerClientVm);
                return uc;
            }, fixedSize: false);
        }

        public GpuProfilesPageViewModel Vm { get; private set; }

        public GpuProfilesPage(GpuProfilesPageViewModel vm)
        {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
