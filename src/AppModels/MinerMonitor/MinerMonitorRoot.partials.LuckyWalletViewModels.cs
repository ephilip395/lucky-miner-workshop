using Lucky.MinerMonitor.Vms;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.MinerMonitor {
    public static partial class MinerMonitorRoot {
        public class LuckyWalletViewModels : ViewModelBase {
            public static LuckyWalletViewModels Instance { get; private set; } = new LuckyWalletViewModels();
            private readonly Dictionary<Guid, LuckyWalletViewModel> _dicById = new Dictionary<Guid, LuckyWalletViewModel>();

            public ICommand Add { get; private set; }

            private LuckyWalletViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                AppRoot.BuildEventPath<LuckyWalletSetInitedEvent>("Lucky钱包集初始化后", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        Init(refresh: true);
                    });
                this.Add = new DelegateCommand(() => {
                    new LuckyWalletViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AppRoot.BuildEventPath<LuckyWalletAddedEvent>("添加Lucky钱包后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new LuckyWalletViewModel(message.Source));
                            if (AppRoot.CoinVms.TryGetCoinVm(message.Source.CoinCode, out CoinViewModel coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.LuckyWallets));
                            }
                        }
                    });
                AppRoot.BuildEventPath<LuckyWalletUpdatedEvent>("更新Lucky钱包后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out LuckyWalletViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    });
                AppRoot.BuildEventPath<LuckyWalletRemovedEvent>("删除Lucky钱包后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Remove(message.Source.GetId());
                        if (AppRoot.CoinVms.TryGetCoinVm(message.Source.CoinCode, out CoinViewModel coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.LuckyWallets));
                        }
                    });
            }

            private void Init(bool refresh) {
                _dicById.Clear();
                foreach (var item in LuckyContext.MinerMonitorContext.LuckyWalletSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new LuckyWalletViewModel(item));
                }
                if (refresh) {
                    foreach (var coinVm in AppRoot.CoinVms.AllCoins) {
                        coinVm.OnPropertyChanged(nameof(coinVm.LuckyWallets));
                    }
                }
            }

            public bool TryGetMineWorkVm(Guid id, out LuckyWalletViewModel luckycmWalletVm) {
                return _dicById.TryGetValue(id, out luckycmWalletVm);
            }

            public IEnumerable<LuckyWalletViewModel> Items {
                get {
                    return _dicById.Values;
                }
            }
        }
    }
}
