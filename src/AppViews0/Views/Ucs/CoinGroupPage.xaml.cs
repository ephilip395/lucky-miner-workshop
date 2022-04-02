﻿using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class CoinGroupPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "币组",
                IconName = "Icon_Group",
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                Width = 660,
                Height = 420
            }, ucFactory: (window) => new CoinGroupPage(), fixedSize: false);
        }

        public CoinGroupPageViewModel Vm { get; private set; }

        public CoinGroupPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new CoinGroupPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<GroupViewModel>(sender, e);
        }
    }
}
