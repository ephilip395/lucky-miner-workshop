using Lucky.Core;
using Lucky.Core.Kernels;
using Lucky.Core.Profile;
using Lucky.Mine;
using Lucky.MinerMonitor.Vms;
using Lucky.Ws;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Lucky.Vms
{
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile, IWsStateViewModel
    {
        public static MinerProfileViewModel Instance { get; private set; } = new MinerProfileViewModel();

        private readonly string _linkFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "行运矿工.lnk");

        private string _argsAssembly;
        private bool _isMining = LuckyContext.Instance.IsMining;
        private bool _isWsOnline;
        private string _wsDescription;
        private int _wsNextTrySecondsDelay;
        private DateTime _wsLastTryOn;
        private bool _isConnecting;
        private double _wsRetryIconAngle;
        private string _wsServerIp;

        public ICommand Up { get; private set; }
        public ICommand Down { get; private set; }
        public ICommand WsRetry { get; private set; }
        public ICommand CopyArgsAssembly { get; private set; }

        public MinerProfileViewModel()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            if (Instance != null)
            {
                throw new InvalidProgramException();
            }
            if (this.IsCreateShortcut)
            {
                CreateShortcut();
            }
            this.Up = new DelegateCommandTpl<string>(propertyName =>
            {
                WpfUtil.Up(this, propertyName);
            });
            this.Down = new DelegateCommandTpl<string>(propertyName =>
            {
                WpfUtil.Down(this, propertyName);
            });
            this.WsRetry = new DelegateCommand(() =>
            {
                RpcRoot.Client.LuckyDaemonService.StartOrStopWsAsync(isResetFailCount: true);
                IsConnecting = true;
            });
            string GetRowArgsAssembly()
            {
                string argsAssembly = this.ArgsAssembly ?? "无";
                if (argsAssembly.Contains("{logfile}"))
                {
                    argsAssembly = Regex.Replace(argsAssembly, "\\s\\S+\\s\"\\{logfile\\}\"", string.Empty);
                }
                return argsAssembly.Trim();
            }
            this.CopyArgsAssembly = new DelegateCommand(() =>
            {
                string argsAssembly = GetRowArgsAssembly();
                Clipboard.SetDataObject(argsAssembly, true);
                VirtualRoot.Out.ShowSuccess("命令行", header: "复制成功");
            });
            if (ClientAppType.IsMinerTweak)
            {
                if (this.IsSystemName)
                {
                    this.MinerName = LuckyKeyword.GetSafeMinerName(LuckyContext.ThisPcName);
                }
                VirtualRoot.BuildCmdPath<SetAutoStartCommand>(this.GetType(), LogEnum.None, message =>
                {
                    this.IsAutoStart = message.IsAutoStart;
                    this.IsAutoBoot = message.IsAutoBoot;
                });
                VirtualRoot.BuildEventPath<StartingMineFailedEvent>("开始挖矿失败", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        IsMining = false;
                        LuckyConsole.UserError(message.Message);
                    });
                // 群控端已经有一个执行RefreshWsStateCommand命令的路径了
                VirtualRoot.BuildCmdPath<RefreshWsStateCommand>(this.GetType(), LogEnum.DevConsole, message =>
                {
                    #region
                    if (message.WsClientState != null)
                    {
                        this.WsServerIp = message.WsClientState.WsServerIp;
                        this.IsWsOnline = message.WsClientState.Status == WsClientStatus.Open;
                        if (message.WsClientState.ToOut)
                        {
                            VirtualRoot.Out.ShowWarn(message.WsClientState.Description, autoHideSeconds: 3);
                        }
                        if (!message.WsClientState.ToOut || !this.IsWsOnline)
                        {
                            this.WsDescription = message.WsClientState.Description;
                        }
                        if (!this.IsWsOnline)
                        {
                            if (message.WsClientState.LastTryOn != DateTime.MinValue)
                            {
                                this.WsLastTryOn = message.WsClientState.LastTryOn;
                            }
                            if (message.WsClientState.NextTrySecondsDelay > 0)
                            {
                                WsNextTrySecondsDelay = message.WsClientState.NextTrySecondsDelay;
                            }
                        }
                    }
                    #endregion
                });
                VirtualRoot.BuildEventPath<Per1SecondEvent>("外网群控重试秒表倒计时", LogEnum.None, this.GetType(), PathPriority.Normal, path: message =>
                {
                    if (IsOuterUserEnabled && !IsWsOnline)
                    {
                        if (WsNextTrySecondsDelay > 0)
                        {
                            WsNextTrySecondsDelay--;
                        }
                        else if (WsLastTryOn == DateTime.MinValue)
                        {
                            this.RefreshWsDaemonState();
                        }
                        OnPropertyChanged(nameof(WsLastTryOnText));
                    }
                });
                VirtualRoot.BuildEventPath<WsServerOkEvent>("服务器Ws服务已可用", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
                {
                    if (IsOuterUserEnabled && !IsWsOnline)
                    {
                        StartOrStopWs();
                    }
                });
            }
            LuckyContext.SetRefreshArgsAssembly((reason) =>
            {
                LuckyConsole.DevDebug(() => $"RefreshArgsAssembly" + reason, ConsoleColor.Cyan);
                #region 确保双挖权重在合法的范围内
                if (CoinVm != null && CoinVm.CoinKernel != null && CoinVm.CoinKernel.Kernel != null)
                {
                    var coinKernelProfile = CoinVm.CoinKernel.CoinKernelProfile;
                    var kernelInput = CoinVm.CoinKernel.Kernel.KernelInputVm;
                    if (coinKernelProfile != null && kernelInput != null)
                    {
                        if (coinKernelProfile.IsDualCoinEnabled && !kernelInput.IsAutoDualWeight)
                        {
                            if (coinKernelProfile.DualCoinWeight > kernelInput.DualWeightMax)
                            {
                                coinKernelProfile.DualCoinWeight = kernelInput.DualWeightMax;
                            }
                            else if (coinKernelProfile.DualCoinWeight < kernelInput.DualWeightMin)
                            {
                                coinKernelProfile.DualCoinWeight = kernelInput.DualWeightMin;
                            }
                            LuckyContext.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfile.CoinKernelId, nameof(coinKernelProfile.DualCoinWeight), coinKernelProfile.DualCoinWeight);
                        }
                    }
                }
                #endregion
                LuckyContext.Instance.CurrentMineContext = MineContextFactory.CreateMineContext();
                if (LuckyContext.Instance.CurrentMineContext != null)
                {
                    this.ArgsAssembly = LuckyContext.Instance.CurrentMineContext.CommandLine;
                }
                else
                {
                    this.ArgsAssembly = string.Empty;
                }
            });
            AppRoot.BuildEventPath<AutoBootStartRefreshedEvent>("刷新开机启动和自动挖矿的展示", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    this.OnPropertyChanged(nameof(IsAutoBoot));
                    this.OnPropertyChanged(nameof(IsAutoStart));
                });
            AppRoot.BuildEventPath<ConnParamsRefreshedEvent>("刷新网络连接参数的展示", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    this.OnPropertyChanged(nameof(ConnectionMethod));
                    this.OnPropertyChanged(nameof(ProxyServerAddress));
                    this.OnPropertyChanged(nameof(ProxyServerPort));
                    this.OnPropertyChanged(nameof(ProxyUsername));
                    this.OnPropertyChanged(nameof(ProxyPassword));
                });
            AppRoot.BuildEventPath<MinerProfilePropertyChangedEvent>("MinerProfile设置变更后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    OnPropertyChanged(message.PropertyName);
                });

            VirtualRoot.BuildEventPath<LocalContextReInitedEventHandledEvent>("本地上下文视图模型集刷新后刷新界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message =>
                {
                    AllPropertyChanged();
                    if (CoinVm != null)
                    {
                        CoinVm.OnPropertyChanged(nameof(CoinVm.Wallets));
                        CoinVm.CoinKernel?.CoinKernelProfile.SelectedDualCoin?.OnPropertyChanged(nameof(CoinVm.Wallets));
                        CoinVm.CoinProfile?.OnPropertyChanged(nameof(CoinVm.CoinProfile.SelectedWallet));
                        CoinVm.CoinKernel?.CoinKernelProfile.SelectedDualCoin?.CoinProfile?.OnPropertyChanged(nameof(CoinVm.CoinProfile.SelectedDualCoinWallet));
                    }
                });
            VirtualRoot.BuildEventPath<CoinAddedEvent>("Vm集添加了新币种后刷新MinerProfileVm内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message =>
            {
                OnPropertyChanged(nameof(CoinVm));
            });
            VirtualRoot.BuildEventPath<CoinRemovedEvent>("Vm集删除了新币种后刷新MinerProfileVm内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message =>
            {
                OnPropertyChanged(nameof(CoinVm));
            });
        }

        public void RefreshWsDaemonState()
        {
            RpcRoot.Client.LuckyDaemonService.GetWsDaemonStateAsync((WsClientState state, Exception e) =>
            {
                if (state != null)
                {
                    this.WsDescription = state.Description;
                    this.WsServerIp = state.WsServerIp;
                    this.IsWsOnline = state.Status == WsClientStatus.Open;
                    if (state.NextTrySecondsDelay > 0)
                    {
                        this.WsNextTrySecondsDelay = state.NextTrySecondsDelay;
                    }
                    this.WsLastTryOn = state.LastTryOn;
                }
            });
        }

        #region IWsStateViewModel的成员

        // 由守护进程根据外网群控是否正常更新
        public bool IsWsOnline
        {
            get => _isWsOnline;
            set
            {
                if (_isWsOnline != value)
                {
                    _isWsOnline = value;
                    OnPropertyChanged(nameof(IsWsOnline));
                    OnPropertyChanged(nameof(WsStateText));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                }
            }
        }

        public string WsDescription
        {
            get
            {
                if (!IsOuterUserEnabled)
                {
                    return "未启用";
                }
                if (string.IsNullOrEmpty(OuterUserId))
                {
                    return "未填写用户";
                }
                if (string.IsNullOrEmpty(_wsDescription))
                {
                    return WsStateText;
                }
                return _wsDescription;
            }
            set
            {
                if (_wsDescription != value)
                {
                    _wsDescription = value;
                    OnPropertyChanged(nameof(WsDescription));
                }
            }
        }

        public string WsServerIp
        {
            get => _wsServerIp;
            set
            {
                _wsServerIp = value;
                OnPropertyChanged(nameof(WsServerIp));
            }
        }

        public int WsNextTrySecondsDelay
        {
            get
            {
                if (_wsNextTrySecondsDelay < 0)
                {
                    return 0;
                }
                return _wsNextTrySecondsDelay;
            }
            set
            {
                if (_wsNextTrySecondsDelay != value)
                {
                    _wsNextTrySecondsDelay = value;
                    OnPropertyChanged(nameof(WsNextTrySecondsDelay));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayText));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                    IsConnecting = value <= 0;
                }
            }
        }

        public DateTime WsLastTryOn
        {
            get => _wsLastTryOn;
            set
            {
                if (_wsLastTryOn != value)
                {
                    _wsLastTryOn = value;
                    OnPropertyChanged(nameof(WsLastTryOn));
                    OnPropertyChanged(nameof(WsLastTryOnText));
                }
            }
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if (_isConnecting != value)
                {
                    _isConnecting = value;
                    OnPropertyChanged(nameof(IsConnecting));
                    OnPropertyChanged(nameof(WsRetryText));
                    if (value)
                    {
                        VirtualRoot.SetInterval(TimeSpan.FromMilliseconds(100), perCallback: () =>
                        {
                            WsRetryIconAngle += 40;
                        }, stopCallback: () =>
                        {
                            WsRetryIconAngle = 0;
                            IsConnecting = false;
                        }, timeout: TimeSpan.FromSeconds(10), requestStop: () =>
                        {
                            return !IsConnecting;
                        });
                    }
                }
            }
        }

        #endregion

        public double WsRetryIconAngle
        {
            get { return _wsRetryIconAngle; }
            set
            {
                _wsRetryIconAngle = value;
                OnPropertyChanged(nameof(WsRetryIconAngle));
            }
        }

        public string WsRetryText
        {
            get
            {
                if (IsConnecting)
                {
                    return "重试中";
                }
                return "立即重试";
            }
        }

        public string WsLastTryOnText
        {
            get
            {
                if (IsWsOnline || WsLastTryOn == DateTime.MinValue)
                {
                    return string.Empty;
                }
                return Timestamp.GetTimeSpanBeforeText(WsLastTryOn);
            }
        }

        public string WsNextTrySecondsDelayText
        {
            get
            {
                int seconds = WsNextTrySecondsDelay;
                if (!IsOuterUserEnabled || IsWsOnline)
                {
                    return string.Empty;
                }
                return Timestamp.GetTimeSpanAfterText(seconds);
            }
        }

        public Visibility WsNextTrySecondsDelayVisible
        {
            get
            {
                if (!IsOuterUserEnabled || IsWsOnline)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public string WsStateText
        {
            get
            {
                if (IsWsOnline)
                {
                    return $"连接服务器成功 {WsServerIp}";
                }
                return $"离线 {WsServerIp}";
            }
        }

        public bool IsOuterUserEnabled
        {
            get => LuckyContext.Instance.MinerProfile.IsOuterUserEnabled;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsOuterUserEnabled != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsOuterUserEnabled), value);
                    OnPropertyChanged(nameof(IsOuterUserEnabled));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                    StartOrStopWs();
                }
            }
        }

        public string OuterUserId
        {
            get => LuckyContext.Instance.MinerProfile.OuterUserId;
            set
            {
                if (LuckyContext.Instance.MinerProfile.OuterUserId != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(OuterUserId), value);
                    OnPropertyChanged(nameof(OuterUserId));
                    OnPropertyChanged(nameof(OuterUserText));
                    StartOrStopWs();
                }
            }
        }

        public string OuterUserText
        {
            get
            {
                if (string.IsNullOrEmpty(OuterUserId))
                {
                    return "群控";
                }
                return OuterUserId;
            }
        }

        public bool IsAdvParamsVisual
        {
            get => LuckyContext.Instance.MinerProfile.IsAdvParamsVisual;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAdvParamsVisual != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAdvParamsVisual), value);
                    OnPropertyChanged(nameof(IsAdvParamsVisual));
                }
            }
        }


        public bool IsUseProxy
        {
            get => LuckyContext.Instance.MinerProfile.IsUseProxy;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsUseProxy != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsUseProxy), value);
                    OnPropertyChanged(nameof(IsUseProxy));
                }
            }
        }

        public int SelectedConnectionMethodIndex
        {
            get => ConnectionMethod - 1;
            set
            {
                ConnectionMethod = value + 1;
            }
        }

        public int ConnectionMethod
        {
            get => LuckyContext.Instance.MinerProfile.ConnectionMethod;
            set
            {
                if (LuckyContext.Instance.MinerProfile.ConnectionMethod != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(ConnectionMethod), value);
                    OnPropertyChanged(nameof(ConnectionMethod));
                }
            }
        }

        public string ProxyServerAddress
        {
            get => LuckyContext.Instance.MinerProfile.ProxyServerAddress;
            set
            {
                if (LuckyContext.Instance.MinerProfile.ProxyServerAddress != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(ProxyServerAddress), value);
                    OnPropertyChanged(nameof(ProxyServerAddress));
                }
            }
        }

        public int ProxyServerPort
        {
            get => LuckyContext.Instance.MinerProfile.ProxyServerPort;
            set
            {
                if (LuckyContext.Instance.MinerProfile.ProxyServerPort != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(ProxyServerPort), value);
                    OnPropertyChanged(nameof(ProxyServerPort));
                }
            }
        }

        public string ProxyUsername
        {
            get => LuckyContext.Instance.MinerProfile.ProxyUsername;
            set
            {
                if (LuckyContext.Instance.MinerProfile.ProxyUsername != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(ProxyUsername), value);
                    OnPropertyChanged(nameof(ProxyUsername));
                }
            }
        }

        public string ProxyPassword
        {
            get => LuckyContext.Instance.MinerProfile.ProxyPassword;
            set
            {
                if (LuckyContext.Instance.MinerProfile.ProxyPassword != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(ProxyPassword), value);
                    OnPropertyChanged(nameof(ProxyPassword));
                }
            }
        }

        public bool IsProxyPool
        {
            get => LuckyContext.Instance.MinerProfile.IsProxyPool;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsProxyPool != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsProxyPool), value);
                    OnPropertyChanged(nameof(IsProxyPool));
                }
            }
        }
        public bool IsProxyGroupMonitor
        {
            get => LuckyContext.Instance.MinerProfile.IsProxyGroupMonitor;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsProxyGroupMonitor != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsProxyGroupMonitor), value);
                    OnPropertyChanged(nameof(IsProxyGroupMonitor));
                }
            }
        }

        private void StartOrStopWs()
        {
            // 只要外网启用状态变更或用户变更就调用，不管是启用还是禁用也不管用户是否正确是否为空都要调用
            // 未启用时以及不正确的用户时调用是为了通知守护进程关闭连接
            RpcRoot.Client.LuckyDaemonService.StartOrStopWsAsync(isResetFailCount: false);
        }

        private void StartProcessProxy()
        {
            VirtualRoot.Out.ShowInfo("开启网络代理");
            //LuckyContext.Instance.StartProxyRouter();
        }

        private void StopProcessProxy()
        {
            VirtualRoot.Out.ShowInfo("停止网络代理");
            //LuckyContext.Instance.StartProxyRouter();

        }

        // 是否主矿池和备矿池都是用户名密码模式的矿池
        public bool IsAllMainCoinPoolIsUserMode
        {
            get
            {
                if (CoinVm == null || CoinVm.CoinProfile == null)
                {
                    return false;
                }
                var mainCoinPool = CoinVm.CoinProfile.MainCoinPool;
                if (mainCoinPool == null)
                {
                    return false;
                }
                if (mainCoinPool.NoPool1)
                {
                    return true;
                }
                if (CoinVm.CoinKernel.IsSupportPool1)
                {
                    var mainCoinPool1 = CoinVm.CoinProfile.MainCoinPool1;
                    if (mainCoinPool1 == null)
                    {
                        return mainCoinPool.IsUserMode;
                    }
                    return mainCoinPool.IsUserMode && mainCoinPool1.IsUserMode;
                }
                return mainCoinPool.IsUserMode;
            }
        }

        public IMineWork MineWork
        {
            get
            {
                return LuckyContext.Instance.MinerProfile.MineWork;
            }
        }

        public bool IsFreeClient
        {
            get
            {
                return MineWork == null || ClientAppType.IsMinerMonitor;
            }
        }

        public Guid Id
        {
            get { return LuckyContext.Instance.MinerProfile.GetId(); }
        }

        public Guid GetId()
        {
            return this.Id;
        }

        public string MinerName
        {
            get
            {
                string minerName = LuckyContext.Instance.MinerProfile.MinerName;
                return minerName;
            }
            set
            {
                if (LuckyContext.Instance.MinerProfile.MinerName != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(MinerName), value);
                    LuckyContext.RefreshArgsAssembly.Invoke("MinerProfile上放置的挖矿矿机名发生了变更");
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsSystemName
        {
            get { return LuckyContext.Instance.MinerProfile.IsSystemName; }
            set
            {
                if (IsMining)
                {
                    VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                    return;
                }
                if (LuckyContext.Instance.MinerProfile.IsSystemName != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsSystemName), value);
                    OnPropertyChanged(nameof(IsSystemName));
                }
                if (value)
                {
                    this.MinerName = LuckyContext.ThisPcName;
                }
            }
        }

        public bool IsShowInTaskbar
        {
            get => LuckyContext.Instance.MinerProfile.IsShowInTaskbar;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsShowInTaskbar != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowInTaskbar), value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public bool IsPreventDisplaySleep
        {
            get => LuckyContext.Instance.MinerProfile.IsPreventDisplaySleep;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsPreventDisplaySleep != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPreventDisplaySleep), value);
                    OnPropertyChanged(nameof(IsPreventDisplaySleep));
                }
            }
        }

        public bool IsNoUi
        {
            get { return LuckyRegistry.GetIsNoUi(); }
            set
            {
                if (LuckyRegistry.GetIsNoUi() != value)
                {
                    LuckyRegistry.SetIsNoUi(value);
                    OnPropertyChanged(nameof(IsNoUi));
                }
            }
        }

        public bool IsAutoNoUi
        {
            get { return LuckyContext.Instance.MinerProfile.IsAutoNoUi; }
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoNoUi != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoNoUi), value);
                    OnPropertyChanged(nameof(IsAutoNoUi));
                }
            }
        }

        public int AutoNoUiMinutes
        {
            get { return LuckyContext.Instance.MinerProfile.AutoNoUiMinutes; }
            set
            {
                if (LuckyContext.Instance.MinerProfile.AutoNoUiMinutes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoNoUiMinutes), value);
                    OnPropertyChanged(nameof(AutoNoUiMinutes));
                }
            }
        }

        public bool IsShowNotifyIcon
        {
            get => LuckyContext.Instance.MinerProfile.IsShowNotifyIcon;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsShowNotifyIcon != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowNotifyIcon), value);
                    OnPropertyChanged(nameof(IsShowNotifyIcon));
                    AppRoot.NotifyIcon?.RefreshIcon();
                }
            }
        }

        public bool IsCloseMeanExit
        {
            get => LuckyContext.Instance.MinerProfile.IsCloseMeanExit;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsCloseMeanExit != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsCloseMeanExit), value);
                    OnPropertyChanged(nameof(IsCloseMeanExit));
                }
            }
        }

        public bool Is1080PillEnabled
        {
            get => LuckyContext.Instance.MinerProfile.Is1080PillEnabled;
            set
            {
                if (LuckyContext.Instance.MinerProfile.Is1080PillEnabled != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(Is1080PillEnabled), value);
                    OnPropertyChanged(nameof(Is1080PillEnabled));
                }
            }
        }

        public string HotKey
        {
            get { return HotKeyUtil.GetHotKey(); }
            set
            {
                if (HotKeyUtil.GetHotKey() != value)
                {
                    if (HotKeyUtil.SetHotKey(value))
                    {
                        OnPropertyChanged(nameof(HotKey));
                    }
                }
            }
        }

        public string ArgsAssembly
        {
            get
            {
                return _argsAssembly;
            }
            set
            {
                _argsAssembly = value;
                OnPropertyChanged(nameof(ArgsAssembly));
            }
        }

        public bool IsAutoBoot
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoBoot;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoBoot != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoBoot), value);
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }
        }

        public bool IsAutoStart
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoStart;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoStart != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStart), value);
                    OnPropertyChanged(nameof(IsAutoStart));
                }
            }
        }

        public bool IsAutoDisableWindowsFirewall
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoDisableWindowsFirewall;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoDisableWindowsFirewall != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoDisableWindowsFirewall), value);
                    OnPropertyChanged(nameof(IsAutoDisableWindowsFirewall));
                }
            }
        }

        public bool IsDisableUAC
        {
            get => LuckyContext.Instance.MinerProfile.IsDisableUAC;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsDisableUAC != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsDisableUAC), value);
                    OnPropertyChanged(nameof(IsDisableUAC));
                }
            }
        }

        public bool IsDisableWAU
        {
            get => LuckyContext.Instance.MinerProfile.IsDisableWAU;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsDisableWAU != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsDisableWAU), value);
                    OnPropertyChanged(nameof(IsDisableWAU));
                }
            }
        }

        public bool IsDisableAntiSpyware
        {
            get => LuckyContext.Instance.MinerProfile.IsDisableAntiSpyware;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsDisableAntiSpyware != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsDisableAntiSpyware), value);
                    OnPropertyChanged(nameof(IsDisableAntiSpyware));
                }
            }
        }

        public bool IsNoShareRestartKernel
        {
            get => LuckyContext.Instance.MinerProfile.IsNoShareRestartKernel;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsNoShareRestartKernel != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartKernel), value);
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public int NoShareRestartKernelMinutes
        {
            get => LuckyContext.Instance.MinerProfile.NoShareRestartKernelMinutes;
            set
            {
                if (LuckyContext.Instance.MinerProfile.NoShareRestartKernelMinutes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public bool IsNoShareRestartComputer
        {
            get => LuckyContext.Instance.MinerProfile.IsNoShareRestartComputer;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsNoShareRestartComputer != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartComputer), value);
                    OnPropertyChanged(nameof(IsNoShareRestartComputer));
                }
            }
        }

        public int NoShareRestartComputerMinutes
        {
            get => LuckyContext.Instance.MinerProfile.NoShareRestartComputerMinutes;
            set
            {
                if (LuckyContext.Instance.MinerProfile.NoShareRestartComputerMinutes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartComputerMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel
        {
            get => LuckyContext.Instance.MinerProfile.IsPeriodicRestartKernel;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsPeriodicRestartKernel != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartKernel), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public int PeriodicRestartKernelHours
        {
            get => LuckyContext.Instance.MinerProfile.PeriodicRestartKernelHours;
            set
            {
                if (LuckyContext.Instance.MinerProfile.PeriodicRestartKernelHours != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public bool IsPeriodicRestartComputer
        {
            get => LuckyContext.Instance.MinerProfile.IsPeriodicRestartComputer;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsPeriodicRestartComputer != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartComputer), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int PeriodicRestartComputerHours
        {
            get => LuckyContext.Instance.MinerProfile.PeriodicRestartComputerHours;
            set
            {
                if (LuckyContext.Instance.MinerProfile.PeriodicRestartComputerHours != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public bool IsAutoRestartKernel
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoRestartKernel;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoRestartKernel != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoRestartKernel), value);
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public int AutoRestartKernelTimes
        {
            get => LuckyContext.Instance.MinerProfile.AutoRestartKernelTimes;
            set
            {
                if (value < 3)
                {
                    value = 3;
                }
                if (LuckyContext.Instance.MinerProfile.AutoRestartKernelTimes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoRestartKernelTimes), value);
                    OnPropertyChanged(nameof(AutoRestartKernelTimes));
                }
            }
        }

        public bool IsSpeedDownRestartComputer
        {
            get => LuckyContext.Instance.MinerProfile.IsSpeedDownRestartComputer;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsSpeedDownRestartComputer != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsSpeedDownRestartComputer), value);
                    OnPropertyChanged(nameof(IsSpeedDownRestartComputer));
                }
            }
        }

        public int PeriodicRestartKernelMinutes
        {
            get => LuckyContext.Instance.MinerProfile.PeriodicRestartKernelMinutes;
            set
            {
                if (LuckyContext.Instance.MinerProfile.PeriodicRestartKernelMinutes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelMinutes));
                    if (value < 0 || value > 60)
                    {
                        throw new ValidationException("无效的值");
                    }
                }
            }
        }

        public int PeriodicRestartComputerMinutes
        {
            get => LuckyContext.Instance.MinerProfile.PeriodicRestartComputerMinutes;
            set
            {
                if (LuckyContext.Instance.MinerProfile.PeriodicRestartComputerMinutes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerMinutes));
                    if (value < 0 || value > 60)
                    {
                        throw new ValidationException("无效的值");
                    }
                }
            }
        }

        public int RestartComputerSpeedDownPercent
        {
            get => LuckyContext.Instance.MinerProfile.RestartComputerSpeedDownPercent;
            set
            {
                if (LuckyContext.Instance.MinerProfile.RestartComputerSpeedDownPercent != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(RestartComputerSpeedDownPercent), value);
                    OnPropertyChanged(nameof(RestartComputerSpeedDownPercent));
                }
            }
        }

        public bool IsNetUnavailableStopMine
        {
            get => LuckyContext.Instance.MinerProfile.IsNetUnavailableStopMine;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsNetUnavailableStopMine != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNetUnavailableStopMine), value);
                    OnPropertyChanged(nameof(IsNetUnavailableStopMine));
                    if (value)
                    {
                        IsNetAvailableStartMine = true;
                    }
                }
            }
        }

        public int NetUnavailableStopMineMinutes
        {
            get => LuckyContext.Instance.MinerProfile.NetUnavailableStopMineMinutes;
            set
            {
                if (LuckyContext.Instance.MinerProfile.NetUnavailableStopMineMinutes != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NetUnavailableStopMineMinutes), value);
                    OnPropertyChanged(nameof(NetUnavailableStopMineMinutes));
                }
            }
        }

        public bool IsNetAvailableStartMine
        {
            get => LuckyContext.Instance.MinerProfile.IsNetAvailableStartMine;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsNetAvailableStartMine != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNetAvailableStartMine), value);
                    OnPropertyChanged(nameof(IsNetAvailableStartMine));
                }
            }
        }

        public int NetAvailableStartMineSeconds
        {
            get => LuckyContext.Instance.MinerProfile.NetAvailableStartMineSeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.NetAvailableStartMineSeconds != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NetAvailableStartMineSeconds), value);
                    OnPropertyChanged(nameof(NetAvailableStartMineSeconds));
                }
            }
        }

        public bool IsEChargeEnabled
        {
            get => LuckyContext.Instance.MinerProfile.IsEChargeEnabled;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsEChargeEnabled != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsEChargeEnabled), value);
                    OnPropertyChanged(nameof(IsEChargeEnabled));
                }
            }
        }

        public double EPrice
        {
            get => LuckyContext.Instance.MinerProfile.EPrice;
            set
            {
                if (LuckyContext.Instance.MinerProfile.EPrice != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(EPrice), value);
                    OnPropertyChanged(nameof(EPrice));
                }
            }
        }

        public bool IsPowerAppend
        {
            get => LuckyContext.Instance.MinerProfile.IsPowerAppend;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsPowerAppend != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPowerAppend), value);
                    OnPropertyChanged(nameof(IsPowerAppend));
                }
            }
        }

        public int PowerAppend
        {
            get => LuckyContext.Instance.MinerProfile.PowerAppend;
            set
            {
                if (LuckyContext.Instance.MinerProfile.PowerAppend != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PowerAppend), value);
                    OnPropertyChanged(nameof(PowerAppend));
                }
            }
        }

        public bool IsShowCommandLine
        {
            get { return LuckyContext.Instance.MinerProfile.IsShowCommandLine; }
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsShowCommandLine != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowCommandLine), value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        private void CreateShortcut()
        {
            try
            {
                if (!ClientAppType.IsMinerTweak)
                {
                    return;
                }
                bool isDo = !File.Exists(_linkFileFullName);
                if (!isDo)
                {
                    string targetPath = WindowsShortcut.GetTargetPath(_linkFileFullName);
                    isDo = !VirtualRoot.AppFileFullName.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
                }
                if (isDo)
                {
                    WindowsShortcut.CreateShortcut(_linkFileFullName, VirtualRoot.AppFileFullName);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
        }

        public bool IsCreateShortcut
        {
            get { return LuckyContext.Instance.MinerProfile.IsCreateShortcut; }
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsCreateShortcut != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsCreateShortcut), value);
                    OnPropertyChanged(nameof(IsCreateShortcut));
                    if (ClientAppType.IsMinerTweak)
                    {
                        if (value)
                        {
                            CreateShortcut();
                        }
                        else
                        {
                            File.Delete(_linkFileFullName);
                        }
                    }
                }
            }
        }

        public Guid CoinId
        {
            get => LuckyContext.Instance.MinerProfile.CoinId;
            set
            {
                if (LuckyContext.Instance.MinerProfile.CoinId != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CoinId), value);
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public int MaxTemp
        {
            get => LuckyContext.Instance.MinerProfile.MaxTemp;
            set
            {
                if (LuckyContext.Instance.MinerProfile.MaxTemp != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(MaxTemp), value);
                    OnPropertyChanged(nameof(MaxTemp));
                }
            }
        }

        public int AutoStartDelaySeconds
        {
            get => LuckyContext.Instance.MinerProfile.AutoStartDelaySeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.AutoStartDelaySeconds != value)
                {
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoStartDelaySeconds), value);
                    OnPropertyChanged(nameof(AutoStartDelaySeconds));
                }
            }
        }

        public bool IsAutoStopByCpu
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoStopByCpu;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoStopByCpu != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStopByCpu), value);
                    OnPropertyChanged(nameof(IsAutoStopByCpu));
                }
            }
        }

        public int CpuStopTemperature
        {
            get => LuckyContext.Instance.MinerProfile.CpuStopTemperature;
            set
            {
                if (LuckyContext.Instance.MinerProfile.CpuStopTemperature != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuStopTemperature), value);
                    OnPropertyChanged(nameof(CpuStopTemperature));
                }
            }
        }

        public int CpuGETemperatureSeconds
        {
            get => LuckyContext.Instance.MinerProfile.CpuGETemperatureSeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.CpuGETemperatureSeconds != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuGETemperatureSeconds), value);
                    OnPropertyChanged(nameof(CpuGETemperatureSeconds));
                }
            }
        }

        public bool IsAutoStartByCpu
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoStartByCpu;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoStartByCpu != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStartByCpu), value);
                    OnPropertyChanged(nameof(IsAutoStartByCpu));
                }
            }
        }

        public int CpuStartTemperature
        {
            get => LuckyContext.Instance.MinerProfile.CpuStartTemperature;
            set
            {
                if (LuckyContext.Instance.MinerProfile.CpuStartTemperature != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuStartTemperature), value);
                    OnPropertyChanged(nameof(CpuStartTemperature));
                }
            }
        }

        public int CpuLETemperatureSeconds
        {
            get => LuckyContext.Instance.MinerProfile.CpuLETemperatureSeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.CpuLETemperatureSeconds != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuLETemperatureSeconds), value);
                    OnPropertyChanged(nameof(CpuLETemperatureSeconds));
                }
            }
        }

        public bool IsAutoStopByGpu
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoStopByGpu;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoStopByGpu != value)
                {
                    LuckyContext.Instance.GpuTemperatureReset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStopByGpu), value);
                    OnPropertyChanged(nameof(IsAutoStopByGpu));
                }
            }
        }

        public int GpuStopTemperature
        {
            get => LuckyContext.Instance.MinerProfile.GpuStopTemperature;
            set
            {
                if (LuckyContext.Instance.MinerProfile.GpuStopTemperature != value)
                {
                    LuckyContext.Instance.GpuTemperatureReset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(GpuStopTemperature), value);
                    OnPropertyChanged(nameof(GpuStopTemperature));
                }
            }
        }

        public int GpuGETemperatureSeconds
        {
            get => LuckyContext.Instance.MinerProfile.GpuGETemperatureSeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.GpuGETemperatureSeconds != value)
                {
                    LuckyContext.Instance.GpuTemperatureReset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(GpuGETemperatureSeconds), value);
                    OnPropertyChanged(nameof(GpuGETemperatureSeconds));
                }
            }
        }

        public bool IsAutoStartByGpu
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoStartByGpu;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoStartByGpu != value)
                {
                    LuckyContext.Instance.GpuTemperatureReset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStartByGpu), value);
                    OnPropertyChanged(nameof(IsAutoStartByGpu));
                }
            }
        }

        public int GpuStartTemperature
        {
            get => LuckyContext.Instance.MinerProfile.GpuStartTemperature;
            set
            {
                if (LuckyContext.Instance.MinerProfile.GpuStartTemperature != value)
                {
                    LuckyContext.Instance.GpuTemperatureReset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(GpuStartTemperature), value);
                    OnPropertyChanged(nameof(GpuStartTemperature));
                }
            }
        }

        public int GpuLETemperatureSeconds
        {
            get => LuckyContext.Instance.MinerProfile.GpuLETemperatureSeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.GpuLETemperatureSeconds != value)
                {
                    LuckyContext.Instance.GpuTemperatureReset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(GpuLETemperatureSeconds), value);
                    OnPropertyChanged(nameof(GpuLETemperatureSeconds));
                }
            }
        }

        public bool IsRaiseHighCpuEvent
        {
            get => LuckyContext.Instance.MinerProfile.IsRaiseHighCpuEvent;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsRaiseHighCpuEvent != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsRaiseHighCpuEvent), value);
                    OnPropertyChanged(nameof(IsRaiseHighCpuEvent));
                }
            }
        }

        public int HighCpuBaseline
        {
            get => LuckyContext.Instance.MinerProfile.HighCpuBaseline;
            set
            {
                if (LuckyContext.Instance.MinerProfile.HighCpuBaseline != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(HighCpuBaseline), value);
                    OnPropertyChanged(nameof(HighCpuBaseline));
                }
            }
        }

        public int HighCpuSeconds
        {
            get => LuckyContext.Instance.MinerProfile.HighCpuSeconds;
            set
            {
                if (LuckyContext.Instance.MinerProfile.HighCpuSeconds != value)
                {
                    LuckyContext.Instance.CpuPackage.Reset();
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(HighCpuSeconds), value);
                    OnPropertyChanged(nameof(HighCpuSeconds));
                }
            }
        }

        public CoinViewModel CoinVm
        {
            get
            {
                if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm) || !coinVm.IsSupported)
                {
                    coinVm = AppRoot.CoinVms.MainCoins.Where(a => a.IsSupported).FirstOrDefault();
                    if (coinVm != null)
                    {
                        CoinId = coinVm.Id;
                    }
                }
                return coinVm;
            }
            set
            {
                if (value == null)
                {
                    value = AppRoot.CoinVms.MainCoins.Where(a => a.IsSupported).OrderBy(a => a.Code).FirstOrDefault();
                }
                if (value != null)
                {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    LuckyContext.RefreshArgsAssembly.Invoke("MinerProfile上引用的主挖币种发生了切换");
                    AppRoot.MinerProfileVm.OnPropertyChanged(nameof(AppRoot.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
                    foreach (var item in AppRoot.GpuSpeedViewModels.Instance.List)
                    {
                        item.OnPropertyChanged(nameof(item.GpuProfileVm));
                    }
                }
            }
        }

        public bool IsWorker
        {
            get
            {
                return MineWork != null && !ClientAppType.IsMinerMonitor;
            }
        }

        public bool IsMining
        {
            get => _isMining;
            set
            {
                _isMining = value;
                OnPropertyChanged(nameof(IsMining));
            }
        }

        public bool IsWorkerOrMining
        {
            get
            {
                return IsMining || IsWorker;
            }
        }

        public bool IsAutoReboot
        {
            get => LuckyContext.Instance.MinerProfile.IsAutoReboot;
            set
            {
                if (LuckyContext.Instance.MinerProfile.IsAutoReboot != value)
                {
                    Windows.Crash.SetAutoReboot(value);
                    LuckyContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoReboot), value);
                    OnPropertyChanged(nameof(IsAutoReboot));
                }
            }
        }
    }
}
