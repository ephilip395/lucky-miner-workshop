﻿using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class OverClockDataPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "超频菜谱",
                IconName = "Icon_OverClock",
                Width = 800,
                Height = 400,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new OverClockDataPage(), fixedSize: true);
        }

        public OverClockDataPageViewModel Vm { get; private set; }

        public OverClockDataPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new OverClockDataPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<OverClockDataViewModel>(sender, e);
        }
    }
}
