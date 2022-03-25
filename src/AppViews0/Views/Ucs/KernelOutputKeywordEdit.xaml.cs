﻿using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class KernelOutputKeywordEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputKeywordViewModel data) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = $"内核输出关键字({(DevMode.IsDevMode ? "服务器" : "自定义")})",
                IsMaskTheParent = true,
                Width = 540,
                FormType = formType,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                KernelOutputKeywordViewModel vm = new KernelOutputKeywordViewModel(data);
                window.BuildCloseWindowOncePath(vm.Id);
                return new KernelOutputKeywordEdit(vm);
            }, fixedSize: true);
        }

        public KernelOutputKeywordViewModel Vm { get; private set; }

        public KernelOutputKeywordEdit(KernelOutputKeywordViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
