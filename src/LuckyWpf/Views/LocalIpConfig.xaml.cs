﻿using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Views.Ucs {
    public partial class LocalIpConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "管理本机 IP",
                IconName = "Icon_Ip",
                Width = 450,
                IsMaskTheParent = true,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new LocalIpConfig();
                window.BuildCloseWindowOncePath(uc.Vm.Id);
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e)=> {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                window.BuildEventPath<LocalIpSetInitedEvent>("本机IP集刷新后刷新IP设置页", LogEnum.DevConsole, location: typeof(LocalIpConfig), PathPriority.Normal,
                    path: message => {
                        UIThread.Execute(uc.Vm.Refresh);
                    });
                return uc;
            }, fixedSize: true);
        }

        public LocalIpConfigViewModel Vm { get; private set; }

        private LocalIpConfig() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new LocalIpConfigViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
