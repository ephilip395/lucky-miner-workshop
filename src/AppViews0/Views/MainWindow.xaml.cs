using Lucky.Views.Ucs;
using Lucky.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernWpf;

namespace Lucky.Views
{
    public partial class MainWindow : Window
    {

        public MainWindowViewModel Vm { get; private set; }

        private HwndSource hwndSource;

        public MainWindow()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            if (!LuckyConsole.Enabled)
            {
                LuckyConsole.Enabled = true;
            }
            Vm = new MainWindowViewModel();
            DataContext = Vm;
            MinHeight = 430;
            MinWidth = 640;
            Width = AppRoot.MainWindowWidth;
            Height = AppRoot.MainWindowHeight;
#if DEBUG
            NTStopwatch.Start();
#endif

            Loaded += (sender, e) =>
            {
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(Win32Proc.WindowProc));
            };
            InitializeComponent();
            // 上面几行是为了看见设计视图

            DateTime lastGetServerMessageOn = DateTime.MinValue;

            // 可视状态改变
            IsVisibleChanged += (sender, e) =>
            {
                LuckyContext.IsUiVisible = IsVisible;
            };

            // 状态改变
            StateChanged += (s, e) =>
            {
                #region
                // 任务栏显示
                ShowInTaskbar = Vm.MinerProfile.IsShowInTaskbar || WindowState != WindowState.Minimized;
                #endregion
            };


            // 主界面上的 Tab 选择改变
            MainTabControl.SelectionChanged += (sender, e) =>
            {
                // 延迟创建，以加快主界面的启动
                #region
                object selectedItem = MainTabControl.SelectedItem;
                if (selectedItem == TabItemSpeedTable)
                {
                    if (SpeedTableContainer.Child == null)
                    {
                        SpeedTableContainer.Child = GetSpeedTable();
                    }
                }
                else if (selectedItem == TabItemOverClockTable)
                {
                    if (OverClockTableContainer.Child == null)
                    {
                        OverClockTableContainer.Child = GetOverClockTable();
                    }
                }
                else if (selectedItem == TabItemMinerProfileOption)
                {
                    if (MinerProfileOptionContainer.Child == null)
                    {
                        MinerProfileOptionContainer.Child = new MinerProfileOption();
                    }
                }
                else if (selectedItem == TabItemConnectionMethodOption)
                {
                    if (ConnectionMethodOptionContainer.Child == null)
                    {
                        ConnectionMethodOptionContainer.Child = new ConnectionMethodOption();
                    }
                }
                else if (selectedItem == TabItemGroupMonitorOption)
                {
                    if (GroupMonitorOptionContainer.Child == null)
                    {
                        GroupMonitorOptionContainer.Child = new GroupMonitorOption();
                    }
                }
                else if (selectedItem == TabItemOutput)
                {
                    if (OutputContainer.Child == null)
                    {
                        OutputContainer.Child = new Output();
                    }
                }
                else if (selectedItem == TabItemDeveloperTools)
                {
                    if (DeveloperToolsContainer.Child == null)
                    {
                        DeveloperToolsContainer.Child = new DeveloperTools();
                    }
                }
                #endregion
            };

            NotiCenterWindow.Bind(this, ownerIsTopmost: true);

            _ = VirtualRoot.BuildCmdPath<TopmostCommand>(GetType(), LogEnum.DevConsole, path: message => { UIThread.Execute(() => { if (!Topmost) { Topmost = true; } }); });
            _ = VirtualRoot.BuildCmdPath<UnTopmostCommand>(GetType(), LogEnum.DevConsole, path: message => { UIThread.Execute(() => { if (Topmost) { Topmost = false; } }); });
            _ = VirtualRoot.BuildCmdPath<CloseMainWindowCommand>(location: GetType(), LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(
                    () =>
                    {
                        if (message.IsAutoNoUi) { SwitchToNoUi(); }
                        else { Close(); }
                    });
            });
            this.BuildEventPath<Per1MinuteEvent>(
                "挖矿中时自动切换为无界面模式",
                LogEnum.DevConsole,
                location: GetType(),
                PathPriority.Normal,
                path: message =>
                {
                    if (LuckyContext.IsUiVisible && LuckyContext.Instance.MinerProfile.IsAutoNoUi && LuckyContext.Instance.IsMining)
                    {
                        if (LuckyContext.MainWindowRendedOn.AddMinutes(LuckyContext.Instance.MinerProfile.AutoNoUiMinutes) < message.BornOn)
                        {
                            VirtualRoot.MyLocalInfo(nameof(MainWindow), $"挖矿中界面展示{LuckyContext.Instance.MinerProfile.AutoNoUiMinutes}分钟后自动切换为无界面模式，可在选项页调整配置");
                            VirtualRoot.Execute(new CloseMainWindowCommand(isAutoNoUi: true));
                        }
                    }
                });
#if DEBUG
            NTStopwatch.ElapsedValue elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds)
            {
                LuckyConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {GetType().Name}.ctor");
            }
#endif
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (LuckyContext.Instance.MinerProfile.IsCloseMeanExit)
            {
                VirtualRoot.Execute(new CloseLuckyCommand("手动操作，关闭主界面意为退出"));
            }
            else
            {
                e.Cancel = true;
                SwitchToNoUi();
            }
        }

        private void SwitchToNoUi()
        {
            AppRoot.Disable();
            Hide();
            VirtualRoot.Out.ShowSuccess("已切换为无界面模式运行");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grid1.Focus();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private SpeedTable _speedTable;
        private SpeedTable GetSpeedTable()
        {
            if (_speedTable == null)
            {
                _speedTable = new SpeedTable();
            }
            return _speedTable;
        }

        private OverClockTable _overClockTable;
        private OverClockTable GetOverClockTable()
        {
            if (_overClockTable == null)
            {
                _overClockTable = new OverClockTable();
            }
            return _overClockTable;
        }

        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
                    break;
                case WindowState.Maximized:
                    Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
                    break;
                case WindowState.Minimized:
                    break;
                default:
                    break;
            }
        }

        private void ThemeBtn_Click(object sender, RoutedEventArgs e)
        {

            Vm.IsDarkMode = !Vm.IsDarkMode;
            UIThread.Execute(() =>
            {
                if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                }
                else
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                }
            });

        }
    }
}
