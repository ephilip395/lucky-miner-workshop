using Lucky.Vms;
using System.Linq;

namespace Lucky.MinerMonitor.Vms {
    public class LuckyWalletPageViewModel : ViewModelBase {
        private CoinViewModel _currentCoin;

        public LuckyWalletPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            _currentCoin = CoinVms.MainCoins.FirstOrDefault();
        }

        public AppRoot.CoinViewModels CoinVms {
            get {
                return AppRoot.CoinVms;
            }
        }

        public CoinViewModel CurrentCoin {
            get => _currentCoin;
            set {
                _currentCoin = value;
                OnPropertyChanged(nameof(CurrentCoin));
            }
        }
    }
}
