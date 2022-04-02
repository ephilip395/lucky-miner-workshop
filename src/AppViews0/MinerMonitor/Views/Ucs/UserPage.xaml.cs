﻿using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class UserPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "用户",
                IconName = "Icon_User",
                Width = 1200,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
            }, ucFactory: (window) => new UserPage());
        }

        public UserPageViewModel Vm { get; private set; }

        public UserPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new UserPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.BuildEventPath<Per20SecondEvent>("外网群控用户列表页面打开着时周期刷新", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                    Vm.Refresh();
                });
                window.BuildEventPath<UserEnabledEvent>("外网群控用户列表页面打开着时，用户启用后刷新Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                    UIThread.Execute(() => {
                        var userVm = Vm.QueryResults.FirstOrDefault(a => a.LoginName == message.Source.LoginName);
                        if (userVm != null) {
                            userVm.IsEnabled = true;
                        }
                    });
                });
                window.BuildEventPath<UserDisabledEvent>("外网群控用户列表页面打开着时，用户禁用后刷新Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                    UIThread.Execute(() => {
                        var userVm = Vm.QueryResults.FirstOrDefault(a => a.LoginName == message.Source.LoginName);
                        if (userVm != null) {
                            userVm.IsEnabled = false;
                        }
                    });
                });
            });
        }

        public void TextBoxPageIndex_KeyUp(object sender, KeyEventArgs e) {
            WpfUtil.TextBoxPageIndex_KeyUp(sender, e);
        }
    }
}
