using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lucky.Vms {
    public class KernelOutputSelectViewModel : ViewModelBase {
        private KernelOutputViewModel _selectedResult;
        public readonly Action<KernelOutputViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public KernelOutputSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public KernelOutputSelectViewModel(KernelOutputViewModel selected, Action<KernelOutputViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public KernelOutputViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<KernelOutputViewModel> PleaseSelectVms {
            get {
                return AppRoot.KernelOutputVms.PleaseSelectVms;
            }
        }
    }
}
