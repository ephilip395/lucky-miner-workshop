﻿using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Views.Ucs {
    public partial class LocalMessages : UserControl {
        public LocalMessagesViewModel Vm { get; private set; }

        public LocalMessages() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new LocalMessagesViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }
    }
}
