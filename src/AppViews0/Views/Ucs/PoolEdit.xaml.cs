﻿using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class PoolEdit : UserControl {
        public static void ShowWindow(FormType formType, PoolViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "矿池",
                FormType = formType,
                IconName = "Icon_Pool",
                IsMaskTheParent = true,
                Width = 540,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) =>
            {
                PoolViewModel vm = new PoolViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new PoolEdit(vm);
            }, fixedSize: true);
        }

        public PoolViewModel Vm { get; private set; }

        public PoolEdit(PoolViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<PoolKernelViewModel>(sender, e);
        }

        private void KbButtonBrand_Clicked(object sender, RoutedEventArgs e) {
            OpenBrandPopup();
            e.Handled = true;
        }

        private void OpenBrandPopup() {
            var popup = PopupBrand;
            popup.IsOpen = true;
            var selected = Vm.BrandItem;
            popup.Child = new SysDicItemSelect(
                new SysDicItemSelectViewModel(AppRoot.SysDicItemVms.PoolBrandItems, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.BrandItem != selectedResult) {
                            Vm.BrandItem = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void PathPoolProfileServer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TbPoolProfileServer.Focus();
        }
    }
}
