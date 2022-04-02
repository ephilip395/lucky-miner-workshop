﻿using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Views.Ucs {
    public partial class Icons : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "图标",
                IconName = "Icon_Icon",
                CloseVisible = System.Windows.Visibility.Visible,
                Width = 1440,
                Height = 800
            },
            ucFactory: (window) => new Icons());
        }

        public IconsViewModel Vm { get; private set; }

        public Icons() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new IconsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                var window = Window.GetWindow(this);
                window.DragMove();
            }
        }
    }
}
