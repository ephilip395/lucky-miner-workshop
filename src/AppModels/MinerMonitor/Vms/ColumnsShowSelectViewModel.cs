using Lucky.Vms;
using System;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms {
    public class ColumnsShowSelectViewModel : ViewModelBase {
        private ColumnsShowViewModel _selectedResult;
        public readonly Action<ColumnsShowViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public ColumnsShowSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public ColumnsShowSelectViewModel(ColumnsShowViewModel selected, Action<ColumnsShowViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public ColumnsShowViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public MinerMonitorRoot.ColumnsShowViewModels ColumnsShowVms {
            get {
                return MinerMonitorRoot.ColumnsShowVms;
            }
        }
    }
}
