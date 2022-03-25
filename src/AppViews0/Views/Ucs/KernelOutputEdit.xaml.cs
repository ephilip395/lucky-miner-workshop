﻿using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class KernelOutputEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输出",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 900,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_KernelOutput"
            }, ucFactory: (window) =>
            {
                KernelOutputViewModel vm = new KernelOutputViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new KernelOutputEdit(vm);
            }, fixedSize: true);
        }

        public KernelOutputViewModel Vm { get; private set; }

        public KernelOutputEdit(KernelOutputViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
