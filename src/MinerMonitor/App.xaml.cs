using Lucky.MinerMonitor;
using Lucky.View;
using Lucky.Views;
using Lucky.Views.Presents;
using Lucky.Views.Ucs;
using Lucky.Vms;
using Lucky.Ws;
using System;
using System.Diagnostics;
using System.Windows;
using ModernWpf;

namespace Lucky
{
    public partial class App : Application
    {
        public static bool IsMultiThreaded { get; } = false;

        public App()
        {
            Hub.MessagePathHub.EnableNotifyProperty();
            // 群控端独立一个子目录，从而挖矿客户端和群控端放在同一个目录时避免路径重复。
            HomePath.SetHomeDirFullName(System.IO.Path.Combine(HomePath.AppDomainBaseDirectory, LuckyKeyword.TempDirName));
            VirtualRoot.Out = NotiCenterWindowViewModel.Instance;
            Logger.SetDir(HomePath.HomeLogsDirFullName);
            WpfUtil.Init();
            AppUtil.Init(this);
            InitializeComponent();

            ThemeManager.Current.ApplicationTheme = LuckyRegistry.GetIsDarkMode() ? ApplicationTheme.Dark : ApplicationTheme.Light;

        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        protected override void OnExit(ExitEventArgs e)
        {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            RpcRoot.RpcUser?.Logout();
            base.OnExit(e);
            LuckyConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // 之所以提前到这里是因为升级之前可能需要下载升级器，下载升级器时需要下载器
            VirtualRoot.BuildCmdPath<ShowFileDownloaderCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
            {
                FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
            });
            // 升级
            VirtualRoot.BuildCmdPath<UpgradeCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
            {
                AppRoot.Upgrade(message.FileName, message.Callback);
            });
            // 注册
            VirtualRoot.BuildCmdPath<ShowSignUpPageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    SignUpPage.ShowWindow();
                });
            });

            if (AppUtil.GetMutex(LuckyKeyword.MinerMonitorAppMutex))
            {
                // 因为登录窗口会用到VirtualRoot.Out，而Out的延迟自动关闭消息会用到倒计时
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                NotiCenterWindow.ShowWindow();
                AppRoot.RemoteDesktop = MsRemoteDesktop.OpenRemoteDesktop;
                MinerMonitorRoot.Login(() =>
                {
                    MinerMonitorRoot.Init(new MinerMonitorWsClient());
                    _ = MinerMonitorRoot.MinerMonitorService;// 访问一下从而提前拉取本地服务数据
                    LuckyContext.Instance.Init(() =>
                    {
                        _appViewFactory.BuildPaths();
                        UIThread.Execute(() =>
                        {
                            MinerMonitorRoot.MinerTweaksWindowVm.OnPropertyChanged(nameof(MinerMonitorRoot.MinerTweaksWindowVm.NetTypeText));
                            if (RpcRoot.IsOuterNet)
                            {
                                MinerMonitorRoot.MinerTweaksWindowVm.QueryMinerTweaks();
                            }
                            else
                            {
                                VirtualRoot.BuildOncePath<ClientSetInitedEvent>("刷新矿机列表界面", LogEnum.DevConsole, pathId: PathId.Empty, this.GetType(), PathPriority.Normal, path: message =>
                                {
                                    MinerMonitorRoot.MinerTweaksWindowVm.QueryMinerTweaks();
                                });
                            }
                            AppRoot.NotifyIcon = ExtendedNotifyIcon.Create("群控端", isMinerMonitor: true);
                            VirtualRoot.Execute(new ShowMinerTweaksWindowCommand(isToggle: false));
                            if (DevMode.IsDevMode)
                            {
                                LuckyConsole.Init();
                            }
                            LuckyConsole.MainUiOk();
                        });
                    });
                }, btnCloseClick: () =>
                {
                    Shutdown();
                });
                #region 处理显示主界面命令
                VirtualRoot.BuildCmdPath<ShowMainWindowCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
                {
                    VirtualRoot.Execute(new ShowMinerTweaksWindowCommand(isToggle: message.IsToggle));
                });
                #endregion
                HttpServer.Start($"http://{LuckyKeyword.Localhost}:{LuckyKeyword.MinerMonitorPort}");
            }
            else
            {
                try
                {
                    _appViewFactory.ShowMainWindow(this);
                }
                catch (Exception)
                {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                        message: "另一个群控端正在运行但唤醒失败，请重试。",
                        title: "错误",
                        icon: "Icon_Error"));
                    Process currentProcess = Process.GetCurrentProcess();
                    Lucky.Windows.TaskKill.KillOtherProcess(currentProcess);
                }
            }
            base.OnStartup(e);
        }
    }
}
