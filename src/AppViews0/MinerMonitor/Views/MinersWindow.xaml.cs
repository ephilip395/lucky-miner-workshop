using Lucky.MinerMonitor.Views.Ucs;
using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Views.Ucs;
using Lucky.Ws;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Lucky.MinerMonitor.Views
{
    public partial class MinersWindow : Window
    {
        private static MinersWindow _instance = null;
        public static MinersWindow ShowWindow(bool isToggle)
        {
            if (_instance == null)
            {
                _instance = new MinersWindow();
                _instance.Show();
            }
            else
            {
                if (_instance.WindowState == WindowState.Minimized)
                {
                    _instance.WindowState = WindowState.Normal;
                }
                else if (!_instance.IsVisible)
                {
                    _instance.Show();
                    _instance.Activate();
                }
                else if (isToggle)
                {
                    _instance.Hide();
                }
                else
                {
                    _instance.Show();
                    _instance.Activate();
                }
            }
            return _instance;
        }

        public MinersWindowViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweaksWindowVm;
            }
        }

        private HwndSource hwndSource;
        private MinersWindow()
        {
            if (WpfUtil.IsInDesignMode2)
            {
                return;
            }
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            this.DataContext = Vm;
            this.Loaded += (sender, e) =>
            {
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(Win32Proc.WindowProc));
                this.WindowState = WindowState.Maximized;
            };
            InitializeComponent();
            DateTime lastGetServerMessageOn = DateTime.MinValue;
            this.BuildEventPath<Per1SecondEvent>("刷新倒计时秒表", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    #region
                    var minerClients = Vm.MinerTweaks.ToArray();
                    if (Vm.CountDown > 0)
                    {
                        Vm.DownCountDown();
                        foreach (var item in minerClients)
                        {
                            item.OnPropertyChanged(nameof(item.LastActivedOnText));
                        }
                        if (RpcRoot.IsOuterNet && Vm.CountDown == 2)
                        {
                            // 外网群控时在矿机列表页数据刷新前2秒通过Ws刷新矿机的算力数据
                            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetSpeed)
                            {
                                Data = Vm.MinerTweaks.Select(a => a.ClientId).ToList()
                            });
                        }
                    }
                    else if (Vm.CountDown == 0)
                    {
                        Vm.ResetCountDown();
                        MinerMonitorRoot.MinerTweaksWindowVm.QueryMinerTweaks(isAuto: true);
                    }
                    #endregion
                });
            NotiCenterWindow.Bind(this, ownerIsTopmost: true);
            MinerMonitorRoot.MinerTweaksWindowVm.QueryMinerTweaks();
        }

        protected override void OnClosed(EventArgs e)
        {
            _instance = null;
            base.OnClosed(e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

    }
}
