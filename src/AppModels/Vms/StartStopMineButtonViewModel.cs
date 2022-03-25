using Lucky.Hub;
using System.Windows.Input;

namespace Lucky.Vms
{
    public class StartStopMineButtonViewModel : ViewModelBase
    {
        public static StartStopMineButtonViewModel Instance { get; private set; } = new StartStopMineButtonViewModel();

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private StartStopMineButtonViewModel()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }

            _ = VirtualRoot.BuildCmdPath<StopMineCommand>(GetType(), LogEnum.DevConsole, path: message =>
              {
                  if (!LuckyContext.Instance.IsMining)
                  {
                      MinerProfile.IsMining = false;
                  }
                  LuckyContext.IsAutoStartCanceled = true;
                  LuckyContext.Instance.StopMineAsync(StopMineReason.LocalUserAction, () =>
                  {
                      if (!LuckyContext.Instance.IsMining)
                      {
                          MinerProfile.IsMining = false;
                      }
                  });
              });

            StartMine = new DelegateCommand(() =>
            {
                VirtualRoot.MyLocalInfo(nameof(StartStopMineButtonViewModel), $"手动开始挖矿", toConsole: true);
                LuckyContext.Instance.StartMine();
            });

            StopMine = new DelegateCommand(() =>
            {
                VirtualRoot.MyLocalInfo(nameof(StartStopMineButtonViewModel), $"手动停止挖矿", toConsole: true);
                VirtualRoot.Execute(new StopMineCommand());
            });
        }

        public void AutoStart()
        {
            bool isAutoStart = MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart;
            if (isAutoStart && !MinerProfile.IsMining)
            {
                LuckyConsole.UserInfo($"{MinerProfile.AutoStartDelaySeconds}秒后开始挖矿");
                MinerProfile.IsMining = true;
                IMessagePathId pathId = null;
                pathId = VirtualRoot.BuildViaTimesLimitPath<Per1SecondEvent>(
                    "自动开始挖矿倒计时",
                    LogEnum.None,
                    viaTimesLimit: MinerProfile.AutoStartDelaySeconds,
                    location: GetType(),
                    PathPriority.Normal,
                    path: message =>
                    {
                        if (!LuckyContext.IsAutoStartCanceled)
                        {
                            MineBtnText = $"倒计时{pathId.ViaTimesLimit}";
                        }
                        if (pathId.ViaTimesLimit == 0)
                        {
                            if (!LuckyContext.IsAutoStartCanceled)
                            {
                                VirtualRoot.MyLocalInfo(nameof(StartStopMineButtonViewModel), $"自动开始挖矿", toConsole: true);
                                LuckyContext.Instance.StartMine();
                            }
                        }
                    });
            }
        }

        private string _mineBtnText = "正在挖矿";

        public string MineBtnText
        {
            get => _mineBtnText;
            set
            {
                _mineBtnText = value;
                OnPropertyChanged(nameof(MineBtnText));
            }
        }

        public MinerProfileViewModel MinerProfile => AppRoot.MinerProfileVm;
    }
}
