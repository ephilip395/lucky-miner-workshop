﻿using Lucky.Vms;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Views.Ucs {
    public partial class KernelEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核",
                FormType = formType,
                IconName = "Icon_Kernel",
                IsMaskTheParent = true,
                Width = 620,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                KernelViewModel vm = new KernelViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new KernelEdit(vm);
            }, fixedSize: false);
        }

        public KernelViewModel Vm { get; private set; }

        public KernelEdit(KernelViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void CoinKernelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<CoinKernelViewModel>(sender, e);
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
                new SysDicItemSelectViewModel(AppRoot.SysDicItemVms.KernelBrandItems, selected, onOk: selectedResult => {
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

        private void ButtonAddCoinKernel_Click(object sender, RoutedEventArgs e) {
            var coins = AppRoot.CoinVms.AllCoins.Where(a => Vm.PackageVm.AlgoIds.Contains(a.AlgoId) && Vm.CoinKernels.All(b => b.CoinId != a.Id));
            PopupKernel.Child = new CoinSelect(
                new CoinSelectViewModel(coins, null, onOk: selectedResult => {
                    if (selectedResult == null || selectedResult.Id == Guid.Empty) {
                        return;
                    }
                    VirtualRoot.Execute(new AddCoinKernelCommand(new CoinKernelViewModel(Guid.NewGuid()) {
                        Args = string.Empty,
                        CoinId = selectedResult.Id,
                        KernelId = Vm.Id
                    }));
                    PopupKernel.IsOpen = false;
                }) {
                    HideView = new DelegateCommand(() => {
                        PopupKernel.IsOpen = false;
                    })
                });
            PopupKernel.IsOpen = true;
        }

        private void KbButtonKernelInput_Clicked(object sender, RoutedEventArgs e) {
            OpenKernelInputPopup();
            e.Handled = true;
        }

        private void OpenKernelInputPopup() {
            var popup = PopupKernelInput;
            popup.IsOpen = true;
            var selected = Vm.KernelInputVm;
            popup.Child = new KernelInputSelect(
                new KernelInputSelectViewModel(selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.KernelInputVm != selectedResult) {
                            Vm.KernelInputVm = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void KbButtonKernelOutput_Clicked(object sender, RoutedEventArgs e) {
            OpenKernelOutputPopup();
            e.Handled = true;
        }

        private void OpenKernelOutputPopup() {
            var popup = PopupKernelOutput;
            popup.IsOpen = true;
            var selected = Vm.KernelOutputVm;
            popup.Child = new KernelOutputSelect(
                new KernelOutputSelectViewModel(selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.KernelOutputVm != selectedResult) {
                            Vm.KernelOutputVm = selectedResult;
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
