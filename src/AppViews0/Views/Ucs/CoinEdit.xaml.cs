﻿using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class CoinEdit : UserControl {
        public static void ShowWindow(FormType formType, CoinViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "币种",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 500,
                CloseVisible = Visibility.Visible,
                IconName = "Icon_Coin",
            }, ucFactory: (window) =>
            {
                CoinViewModel vm = new CoinViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new CoinEdit(vm);
            }, fixedSize: true);
        }

        public CoinViewModel Vm { get; private set; }

        public CoinEdit(CoinViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void KbButtonAlgo_Clicked(object sender, RoutedEventArgs e) {
            OpenAlgoPopup();
            e.Handled = true;
        }

        private void OpenAlgoPopup() {
            var popup = PopupAlgo;
            popup.IsOpen = true;
            var selected = Vm.AlgoItem;
            popup.Child = new SysDicItemSelect(
                new SysDicItemSelectViewModel(AppRoot.SysDicItemVms.AlgoItems, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.AlgoItem != selectedResult) {
                            Vm.AlgoItem = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void KbButtonNKernelBrand_Clicked(object sender, RoutedEventArgs e) {
            var popup = PopupNKernelBrand;
            popup.IsOpen = true;
            var selected = Vm.NKernelBrand;
            popup.Child = new SysDicItemSelect(
                new SysDicItemSelectViewModel(AppRoot.SysDicItemVms.KernelBrandsSelect, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.NKernelBrand != selectedResult) {
                            Vm.NKernelBrand = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void KbButtonAKernelBrand_Clicked(object sender, RoutedEventArgs e) {
            var popup = PopupAKernelBrand;
            popup.IsOpen = true;
            var selected = Vm.AKernelBrand;
            popup.Child = new SysDicItemSelect(
                new SysDicItemSelectViewModel(AppRoot.SysDicItemVms.KernelBrandsSelect, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.AKernelBrand != selectedResult) {
                            Vm.AKernelBrand = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }
    }
}
