﻿using Lucky.Hub;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class RestartWindows : UserControl {
        public static void ShowDialog(RestartWindowsViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "重启电脑",
                Width = 400,
                Height = 200,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_Restart"
            }, ucFactory: (window) => {
                RestartWindows uc = new RestartWindows(vm);
                window.BuildCloseWindowOncePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public RestartWindowsViewModel Vm { get; private set; }

        private bool _isCanceled = false;
        public RestartWindows(RestartWindowsViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
            this.OnLoaded(window => {
                IMessagePathId messagePathId = null;
                messagePathId = window.BuildViaTimesLimitPath<Per1SecondEvent>("重启倒计时", LogEnum.None, Vm.Seconds, location: this.GetType(), PathPriority.Normal, path: message => {
                    if (_isCanceled) {
                        return;
                    }
                    Vm.Seconds = Vm.Seconds - 1;
                    if (messagePathId.ViaTimesLimit == 0) {
                        Windows.Power.Restart();
                    }
                });
            });
        }

        private void KbCancelButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            _isCanceled = true;
            VirtualRoot.Execute(new CloseWindowCommand(this.Vm.Id));
            VirtualRoot.MyLocalInfo(nameof(RestartWindows), "取消重启电脑");
        }
    }
}
