﻿using Lucky.Vms;
using System;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class KernelDownloading : UserControl {
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsMaskTheParent = true,
                Title = "下载挖矿内核",
                IconName = "Icon_Download",
                Width = 294,
                Height = 120,
                CloseVisible = System.Windows.Visibility.Visible,
            },
            ucFactory: (window) => {
                var uc = new KernelDownloading {
                    CloseWindow = window.Close
                };
                return uc;
            },
            beforeShow: (window, uc) => {
                if (kernelId != Guid.Empty) {
                    uc.Vm.Download(kernelId, (isSuccess, message) => {
                        if (isSuccess) {
                            uc.CloseWindow();
                        }
                        downloadComplete(isSuccess, message);
                    });
                    uc.Vm.OnPropertyChanged(nameof(uc.Vm.DownloadingVms));
                }
            }, fixedSize: true);
        }

        private Action CloseWindow;

        public KernelDownloadingViewModel Vm { get; private set; }

        public KernelDownloading() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new KernelDownloadingViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
