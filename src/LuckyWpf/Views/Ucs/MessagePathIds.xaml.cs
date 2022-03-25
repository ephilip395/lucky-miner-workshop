﻿using Lucky.Hub;
using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class MessagePathIds : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "集线器",
                IconName = "Icon_Logo",
                Width = 1400,
                Height = 600,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new MessagePathIds();
                return uc;
            }, fixedSize: false);
        }

        public MessagePathIdsViewModel Vm { get; private set; }

        public MessagePathIds() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new MessagePathIdsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(onLoad: window => {
                VirtualRoot.MessageHub.PathAdded += OnPathConnected;
                VirtualRoot.MessageHub.PathRemoved += OnPathDisconnected;
            }, onUnload: window => {
                VirtualRoot.MessageHub.PathAdded -= OnPathConnected;
                VirtualRoot.MessageHub.PathRemoved -= OnPathDisconnected;
            });
        }

        private void OnPathConnected(IMessagePathId pathId) {
            UIThread.Execute(() => {
                Vm.PathIds.Add(pathId);
            });
        }

        private void OnPathDisconnected(IMessagePathId pathId) {
            UIThread.Execute(() => {
                Vm.PathIds.Remove(pathId);
            });
        }
    }
}
