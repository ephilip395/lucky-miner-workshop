﻿using Lucky.Core;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class EnvironmentVariableEdit : UserControl {
        public static void ShowWindow(CoinKernelViewModel coinKernelVm, EnvironmentVariable environmentVariable) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "环境变量",
                IsMaskTheParent = true,
                Width = 500,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                EnvironmentVariableEditViewModel vm = new EnvironmentVariableEditViewModel(coinKernelVm, environmentVariable);
                window.BuildCloseWindowOncePath(vm.Id);
                return new EnvironmentVariableEdit(vm);
            }, fixedSize: true);
        }

        public EnvironmentVariableEditViewModel Vm { get; private set; }

        public EnvironmentVariableEdit(EnvironmentVariableEditViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
