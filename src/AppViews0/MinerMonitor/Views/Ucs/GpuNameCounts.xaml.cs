﻿using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class GpuNameCounts : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Gpu名",
                IconName = "Icon_Gpu",
                Width = 800,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
            }, ucFactory: (window) => new GpuNameCounts());
        }

        public GpuNameCountsViewModel Vm { get; private set; }

        public GpuNameCounts() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new GpuNameCountsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void TbKeyword_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                this.Vm.Keyword = this.TbKeyword.Text;
            }
        }
    }
}
