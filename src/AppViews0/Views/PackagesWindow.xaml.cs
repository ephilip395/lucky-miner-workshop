﻿using Lucky.Vms;
using System;
using System.Windows.Input;

namespace Lucky.Views {
    public partial class PackagesWindow : BlankWindow {
        private static readonly object _locker = new object();
        private static PackagesWindow _instance = null;
        public static void ShowWindow() {
            UIThread.Execute(() => {
                if (_instance == null) {
                    lock (_locker) {
                        if (_instance == null) {
                            _instance = new PackagesWindow();
                            _instance.Show();
                        }
                    }
                }
                else {
                    _instance.ShowWindow(false);
                }
            });
        }

        public PackagesWindowViewModel Vm { get; private set; }

        public PackagesWindow() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new PackagesWindowViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.TbUcName.Text = nameof(PackagesWindow);
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            _instance = null;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<PackageViewModel>(sender, e);
        }
    }
}
