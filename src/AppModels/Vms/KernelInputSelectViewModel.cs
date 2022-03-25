using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lucky.Vms {
    public class KernelInputSelectViewModel : ViewModelBase {
        private KernelInputViewModel _selectedResult;
        public readonly Action<KernelInputViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public KernelInputSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public KernelInputSelectViewModel(KernelInputViewModel selected, Action<KernelInputViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public KernelInputViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<KernelInputViewModel> PleaseSelectVms {
            get {
                return AppRoot.KernelInputVms.PleaseSelectVms;
            }
        }
    }
}
