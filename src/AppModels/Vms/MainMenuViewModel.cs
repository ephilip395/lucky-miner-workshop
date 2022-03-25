using Lucky.User;
using System.Windows;
using System.Windows.Media;

namespace Lucky.Vms {
    public class MainMenuViewModel : ViewModelBase {
        public static MainMenuViewModel Instance { get; private set; } = new MainMenuViewModel();

        public MainMenuViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.BuildEventPath<MinerMonitorServiceSwitchedEvent>("群控后台客户端服务类型切换后刷新菜单的展示状态", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                this.OnPropertyChanged(nameof(LoginName));
                this.OnPropertyChanged(nameof(IsMinerMonitorLocalOrOuterAdminVisible));
                this.OnPropertyChanged(nameof(IsMinerMonitorOuterAdmin));
                this.OnPropertyChanged(nameof(IsMinerMonitorOuterAdminVisible));
                this.OnPropertyChanged(nameof(IsMinerMonitorOuterVisible));
                this.OnPropertyChanged(nameof(IsMinerMonitorLocalVisible));
            });
        }

        public string LoginName {
            get {
                return RpcRoot.RpcUser.LoginName;
            }
        }

        public SolidColorBrush TopItemForeground {
            get {
                if (ClientAppType.IsMinerTweak) {
                    return WpfUtil.WhiteBrush;
                }
                return WpfUtil.BlackBrush;
            }
        }

        public Visibility IsMinerMonitorLocalOrOuterAdminVisible {
            get {
                if (RpcRoot.IsOuterNet) {
                    return IsMinerMonitorOuterAdminVisible;
                }
                return IsMinerMonitorLocalVisible;
            }
        }

        public bool IsMinerMonitorOuterAdmin {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return true;
                }
                if (ClientAppType.IsMinerMonitor) {
                    if (!RpcRoot.IsLogined) {
                        return false;
                    }
                    if (!RpcRoot.RpcUser.LoginedUser.IsAdmin()) {
                        return false;
                    }
                    if (RpcRoot.IsOuterNet) {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// 登录的是外网群控管理员
        /// </summary>
        public Visibility IsMinerMonitorOuterAdminVisible {
            get {
                return IsMinerMonitorOuterAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是外网登录用户，包括普通用户和Admin
        /// </summary>
        public Visibility IsMinerMonitorOuterVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerMonitor) {
                    if (RpcRoot.IsLogined && RpcRoot.IsOuterNet) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是内网群控
        /// </summary>
        public Visibility IsMinerMonitorLocalVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerMonitor) {
                    if (RpcRoot.IsInnerNet) {
                        return Visibility.Visible;
                    }
                    return Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
