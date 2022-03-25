using Lucky.Core;
using Lucky.Core.Impl;
using Lucky.MinerMonitor.Impl;
using Lucky.MinerMonitor.Vms;
using Lucky.Ws;
using System;

namespace Lucky.MinerMonitor
{
    public static partial class MinerMonitorRoot
    {
        public static IWsClient WsClient { get; private set; } = EmptyWsClient.Instance;
        public static MinerTweakConsoleViewModel MinerTweakConsoleVm { get; private set; } = new MinerTweakConsoleViewModel();
        public static MinerTweakMessagesViewModel MinerTweakMessagesVm { get; private set; } = new MinerTweakMessagesViewModel();
        public static MinerTweakOperationResultsViewModel MinerTweakOperationResultsVm { get; private set; } = new MinerTweakOperationResultsViewModel();
        public static IReadOnlyLuckyFileSet ReadOnlyLuckyFileSet { get; private set; } = new ReadOnlyLuckyFileSet();

        public static readonly LocalMinerMonitorService LocalMinerMonitorService = new LocalMinerMonitorService();
        private static readonly ServerMinerMonitorService _serverMinerMonitorService = new ServerMinerMonitorService();

        public static IMinerMonitorService MinerMonitorService
        {
            get
            {
                if (RpcRoot.IsOuterNet)
                {
                    return _serverMinerMonitorService;
                }
                else
                {
                    return LocalMinerMonitorService;
                }
            }
        }

        public static void Init(IWsClient wsClient)
        {
            WsClient = wsClient;
        }

        public static void Login(Action onLoginSuccess, string serverHost = null, Action btnCloseClick = null)
        {
            WpfUtil.Login(onLoginSuccess: () =>
            {
                LuckyContext.MinerMonitorContext.UserAppSettingSet.Init(RpcRoot.RpcUser.LoginedUser.UserAppSettings);
                onLoginSuccess?.Invoke();
            }, serverHost, btnCloseClick);
        }

        public static MinerTweaksWindowViewModel MinerTweaksWindowVm
        {
            get
            {
                return MinerTweaksWindowViewModel.Instance;
            }
        }

        public static ColumnsShowViewModels ColumnsShowVms
        {
            get
            {
                return ColumnsShowViewModels.Instance;
            }
        }

        public static MinerGroupViewModels MinerGroupVms
        {
            get
            {
                return MinerGroupViewModels.Instance;
            }
        }

        public static MineWorkViewModels MineWorkVms
        {
            get
            {
                return MineWorkViewModels.Instance;
            }
        }

        public static OverClockDataViewModels OverClockDataVms
        {
            get
            {
                return OverClockDataViewModels.Instance;
            }
        }

        public static LuckyWalletViewModels LuckyWalletVms
        {
            get
            {
                return LuckyWalletViewModels.Instance;
            }
        }

        private static bool _isMinerTweakMessagesVisible = false;
        public static bool IsMinerTweakMessagesVisible
        {
            get { return _isMinerTweakMessagesVisible; }
        }

        public static void SetIsMinerTweakMessagesVisible(bool value)
        {
            _isMinerTweakMessagesVisible = value;
            if (value)
            {
                MinerTweakMessagesVm.SendGetLocalMessagesMqMessage(isFast: true);
            }
        }
    }
}
