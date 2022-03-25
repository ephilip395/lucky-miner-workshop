using Lucky.Core;
using Lucky.Core.MinerTweak;
using Lucky.Gpus;
using Lucky.Mine;
using Lucky.Notifications;
using Lucky.RemoteDesktop;
using Lucky.View;
using Lucky.Views;
using Lucky.Views.Ucs;
using Lucky.Views.Presents;
using Lucky.Vms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using ModernWpf;

namespace Lucky
{
    public partial class App : Application, IApp
    {
        public static bool IsMultiThreaded { get; } = false;

        public App()
        {
            Hub.MessagePathHub.EnableNotifyProperty();
            if (LuckyRegistry.GetIsNoUi())
            {
                LuckyConsole.Enabled = false;
            }
            VirtualRoot.Out = NotiCenterWindowViewModel.Instance;
            Logger.SetDir(TempPath.TempLogsDirFullName);
            WpfUtil.Init();
            AppUtil.Init(this);
            AppUtil.IsHotKeyEnabled = true;
            InitializeComponent();
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            PresetManager.Current.ColorPreset = "Default";

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
            // 可处理升级和下载命令
            BuildCommonPaths();

            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade))
            {
                // 升级挖矿端
                // 启动计时器以放置后续的逻辑中用到计时器
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                // 执行升级命令
                VirtualRoot.Execute(new UpgradeCommand(CommandLineArgs.Upgrade, () =>
                {
                    UIThread.Execute(() => { Environment.Exit(0); });
                }));
            }
            else
            {
                // 正常启动
                DoRun();
            }
            base.OnStartup(e);
        }

        private void DoRun()
        {
            if (AppUtil.GetMutex(LuckyKeyword.MinerTweakAppMutex))
            {
                // 显示通知窗口
                NotiCenterWindow.ShowWindow();
                Logger.InfoDebugLine($"==================Lucky.exe {EntryAssemblyInfo.CurrentVersionStr}==================");
                // 显示 Splash Window
                // 在另一个UI线程运行欢迎界面以确保欢迎界面的响应不被耗时的主界面初始化过程阻塞
                // 注意：必须确保SplashWindow没有用到任何其它界面用到的依赖对象
                SplashWindow splashWindow = null;
                SplashWindow.ShowWindowAsync(window =>
                {
                    splashWindow = window;
                });
                // 检查 WMI 服务
                if (!Lucky.Windows.WMI.IsWmiEnabled)
                {
                    DialogWindow.ShowHardDialog(new DialogWindowViewModel(
                        message: "行运矿工无法运行所需的组件，因为本机未开启WMI服务，行运矿工需要使用WMI服务检测windows的内存、显卡等信息，请先手动开启WMI。",
                        title: "提醒",
                        icon: "Icon_Error"));
                    Shutdown();
                    Environment.Exit(0);
                }
                // 检查当前用户是不是管理员
                if (!Lucky.Windows.Role.IsAdministrator)
                {
                    _ = NotiCenterWindowViewModel.Instance.Manager
                        .CreateMessage()
                        .Warning("提示", "请以管理员身份运行。")
                        .WithButton("点击以管理员身份运行", button =>
                        {
                            WpfUtil.RunAsAdministrator();
                        })
                        .Dismiss().WithButton("忽略", button =>
                        {

                        }).Queue();
                }
                // 可处理命令 -- this
                BuildPaths();
                LuckyContext.Instance.Init(() =>
                {
                    // 可处理注册命令
                    _ = VirtualRoot.BuildCmdPath<ShowSignUpPageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
                        {
                            UIThread.Execute(() =>
                            {
                                SignUpPage.ShowWindow();
                            });
                        });

                    // 可处理命令 - _appViewFactory
                    _appViewFactory.BuildPaths();

                    if (VirtualRoot.IsLTWin10)
                    {
                        // Windows 版本 < Win10
                        VirtualRoot.MyLocalWarn(nameof(App), AppRoot.LowWinMessage, toConsole: true);
                    }

                    if (LuckyContext.Instance.GpuSet.Count == 0)
                    {
                        // GPU 数量 == 0
                        VirtualRoot.MyLocalError(nameof(App), "没有矿卡或矿卡未驱动。", toConsole: true);
                    }

                    if (LuckyContext.WorkType != WorkType.None && LuckyContext.Instance.ServerContext.CoinSet.Count == 0)
                    {
                        // 检查加密货币的集合是否存在
                        // 需要访问阿里云来获取
                        VirtualRoot.MyLocalError(nameof(App), "访问阿里云失败，请尝试更换本机dns解决此问题。", toConsole: true);
                    }

                    UIThread.Execute(() =>
                    {
                        Window mainWindow = null;
                        AppRoot.NotifyIcon = ExtendedNotifyIcon.Create("行运矿工", isMinerMonitor: false);
                        if (LuckyRegistry.GetIsNoUi() && LuckyContext.Instance.MinerProfile.IsAutoStart)
                        {
                            // 无界面启动
                            VirtualRoot.Out.ShowSuccess("以无界面模式启动，可在选项页调整设置", header: "行运矿工");
                        }
                        else
                        {
                            // 显示主窗口
                            _appViewFactory.ShowMainWindow(isToggle: false, out mainWindow);
                        }
                        // 退出 Splash Window
                        splashWindow?.Dispatcher.Invoke((Action)delegate ()
                        {
                            splashWindow?.OkClose();
                        });
                        // 启动时 Windows 状态栏显式的是SplashWindow的任务栏图标，SplashWindow关闭后
                        // 激活主窗口的Windows任务栏图标
                        _ = (mainWindow?.Activate());

                        // 开始和停止挖矿的按钮
                        StartStopMineButtonViewModel.Instance.AutoStart();

                        // 注意：因为推迟到这里才启动的计时器，所以别忘了在Upgrade、和Action情况时启动计时器
                        // 启动时间计时器
                        VirtualRoot.StartTimer(new WpfTimingEventProducer());

                        // 执行命令后参数的挖矿客户端命令
                        if (CommandLineArgs.Action.TryParse(out MinerTweakActionType resourceType))
                        {
                            VirtualRoot.Execute(new MinerTweakActionCommand(resourceType));
                        }

                        // 控制台输出主UI准备完毕
                        if (DevMode.IsDevMode)
                        {
                            LuckyConsole.Init();
                        }
                        LuckyConsole.MainUiOk();
                    });

                    _ = Task.Factory.StartNew(() =>
                      {
                          Core.Profiles.IWorkProfile minerProfile = LuckyContext.Instance.MinerProfile;
                          if (minerProfile.IsDisableUAC)
                          {
                              // 禁用 UAC
                              _ = Lucky.Windows.UAC.DisableUAC();
                          }
                          if (minerProfile.IsAutoDisableWindowsFirewall)
                          {
                              // 禁用 Windows 防火墙
                              _ = Firewall.DisableFirewall();
                          }
                          if (minerProfile.IsDisableWAU)
                          {
                              // 禁用 Windows 自动更新
                              Lucky.Windows.WAU.DisableWAUAsync();
                          }
                          if (minerProfile.IsDisableAntiSpyware)
                          {
                              // 禁用反间谍软件功能
                              Lucky.Windows.Defender.DisableAntiSpyware();
                          }
                          // Windows 崩溃后自动重启
                          Lucky.Windows.Crash.SetAutoReboot(minerProfile.IsAutoReboot);
                          if (!Firewall.IsMinerTweakRuleExists())
                          {
                              // 增加挖矿规则
                              Firewall.AddMinerTweakRule();
                          }
                          try
                          {
                              // 打开本机的 HTTP 服务器
                              HttpServer.Start($"http://{LuckyKeyword.Localhost}:{LuckyKeyword.MinerTweakPort}");

                              // 运行挖矿守护进程
                              Daemon.DaemonUtil.RunLuckyDaemon();

                              // 运行流量转发守护进程
                              DivertProxyNu.DivertProxyNuUtil.RunLuckyDivertProxyNu();
                              
                          }
                          catch (Exception ex)
                          {
                              Logger.ErrorDebugLine(ex);
                          }
                      });
                });
            }
            else
            {
                // 获取互斥锁失败
                try
                {
                    _appViewFactory.ShowMainWindow(this);
                }
                catch (Exception)
                {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                        message: "另一个行运矿工正在运行但唤醒失败，请重试。",
                        title: "错误",
                        icon: "Icon_Error"));
                    Process currentProcess = Process.GetCurrentProcess();
                    Lucky.Windows.TaskKill.KillOtherProcess(currentProcess);
                }
            }
        }

        private void BuildCommonPaths()
        {
            // 可执行下载命令
            // 之所以提前到这里是因为升级之前可能需要下载升级器，下载升级器时需要下载器
            VirtualRoot.BuildCmdPath<ShowFileDownloaderCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
              });
            // 可执行升级命令
            VirtualRoot.BuildCmdPath<UpgradeCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  AppRoot.Upgrade(message.FileName, message.Callback);
              });
        }

        private void BuildPaths()
        {
            // 可执行矿工客户端命令
            VirtualRoot.BuildCmdPath<MinerTweakActionCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  #region
                  switch (message.ActionType)
                  {
                      // 打开镭GPU选项
                      case MinerTweakActionType.SwitchRadeonGpuOn:
                          VirtualRoot.Execute(new SwitchRadeonGpuCommand(on: true));
                          break;
                      // 关闭镭GPU选项
                      case MinerTweakActionType.SwitchRadeonGpuOff:
                          VirtualRoot.Execute(new SwitchRadeonGpuCommand(on: false));
                          break;
                      // 关闭 Windows 自动升级功能
                      case MinerTweakActionType.BlockWAU:
                          VirtualRoot.Execute(new BlockWAUCommand());
                          break;
                      default:
                          break;
                  }
                  #endregion
              });
            #region 处理显示主界面命令
            VirtualRoot.BuildCmdPath<ShowMainWindowCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  UIThread.Execute(() =>
                  {
                      _appViewFactory.ShowMainWindow(message.IsToggle, out _);
                      // 使状态栏显示显示最新状态
                      if (LuckyContext.Instance.IsMining)
                      {
                          var mainCoin = LuckyContext.Instance.LockedMineContext.MainCoin;
                          if (mainCoin == null)
                          {
                              return;
                          }
                          var coinShare = LuckyContext.Instance.CoinShareSet.GetOrCreate(mainCoin.GetId());
                          VirtualRoot.RaiseEvent(new ShareChangedEvent(PathId.Empty, coinShare));
                          if ((LuckyContext.Instance.LockedMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null)
                          {
                              coinShare = LuckyContext.Instance.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                              VirtualRoot.RaiseEvent(new ShareChangedEvent(PathId.Empty, coinShare));
                          }
                          AppRoot.GpuSpeedVms.Refresh();
                      }
                  });
              });
            #endregion
            #region 周期确保守护进程在运行
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("周期确保守护进程在运行", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    Daemon.DaemonUtil.RunLuckyDaemon();
                    DivertProxyNu.DivertProxyNuUtil.RunLuckyDivertProxyNu();
                });
            #endregion
            #region 开始和停止挖矿后
            VirtualRoot.BuildEventPath<StartingMineEvent>("开始挖矿时更新挖矿按钮状态", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    AppRoot.MinerProfileVm.IsMining = true;
                    // 因为无界面模式不一定会构建挖矿状态按钮，所以放在这里而不放在挖矿按钮的VM中
                    StartStopMineButtonViewModel.Instance.MineBtnText = "正在挖矿";
                });
            VirtualRoot.BuildEventPath<MineStartedEvent>("启动1080ti小药丸、启动DevConsole? 更新挖矿按钮状态", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Start();
                });
            VirtualRoot.BuildEventPath<MineStopedEvent>("停止挖矿后停止1080ti小药丸 挖矿停止后更新界面挖矿状态", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    AppRoot.MinerProfileVm.IsMining = false;
                    // 因为无界面模式不一定会构建挖矿状态按钮，所以放在这里而不放在挖矿按钮的VM中
                    StartStopMineButtonViewModel.Instance.MineBtnText = "尚未开始";
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Stop();
                });
            #endregion
            #region 处理禁用win10系统更新
            VirtualRoot.BuildCmdPath<BlockWAUCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  Lucky.Windows.WindowsUtil.BlockWAU().ContinueWith(t =>
                  {
                      if (t.Exception == null)
                      {
                          VirtualRoot.MyLocalInfo(nameof(App), "禁用windows系统更新成功", OutEnum.Success);
                      }
                      else
                      {
                          VirtualRoot.MyLocalError(nameof(App), "禁用windows系统更新失败", OutEnum.Error);
                      }
                  });
              });
            #endregion
            #region 优化windows
            VirtualRoot.BuildCmdPath<Win10OptimizeCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  Lucky.Windows.WindowsUtil.Win10Optimize(e =>
                  {
                      if (e == null)
                      {
                          VirtualRoot.MyLocalInfo(nameof(App), "优化Windows成功", OutEnum.Success);
                      }
                      else
                      {
                          VirtualRoot.MyLocalError(nameof(App), "优化Windows失败", OutEnum.Error);
                      }
                  });
              });
            #endregion
            #region 处理开启A卡计算模式
            VirtualRoot.BuildCmdPath<SwitchRadeonGpuCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
              {
                  if (AdlHelper.IsHasATIGpu)
                  {
                      AppRoot.SwitchRadeonGpu(message.On);
                  }
              });
            #endregion
            #region 处理A卡驱动签名
            VirtualRoot.BuildCmdPath<AtikmdagPatcherCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
            {
                if (AdlHelper.IsHasATIGpu)
                {
                    AppRoot.OpenAtikmdagPatcher();
                }
            });
            #endregion
            #region 启用或禁用windows远程桌面
            VirtualRoot.BuildCmdPath<EnableRemoteDesktopCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
            {
                if (LuckyRegistry.GetIsRdpEnabled())
                {
                    return;
                }
                string msg = "确定启用Windows远程桌面吗？";
                DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                    message: msg,
                    title: "确认",
                    onYes: () =>
                    {
                        LuckyRegistry.SetIsRdpEnabled(true);
                        Firewall.AddRdpRule();
                    }));
            });
            #endregion
            #region 启用或禁用windows开机自动登录
            VirtualRoot.BuildCmdPath<EnableOrDisableWindowsAutoLoginCommand>(location: this.GetType(), LogEnum.DevConsole, path: message =>
            {
                if (Lucky.Windows.OS.Instance.IsAutoAdminLogon)
                {
                    VirtualRoot.Execute(new UnTopmostCommand());
                    Lucky.Windows.Cmd.RunClose("control", "userpasswords2");
                    return;
                }
                if (Lucky.Windows.OS.Instance.IsGEWindows2004)
                {
                    WindowsAutoLogon.ShowWindow();
                }
                else
                {
                    VirtualRoot.Execute(new UnTopmostCommand());
                    Lucky.Windows.Cmd.RunClose("control", "userpasswords2");
                }
            });
            #endregion
        }
    }
}
