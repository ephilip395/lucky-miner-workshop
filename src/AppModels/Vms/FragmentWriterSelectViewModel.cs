using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lucky.Vms {
    public class FragmentWriterSelectViewModel : ViewModelBase {
        private FragmentWriterViewModel _selectedResult;
        public readonly Action<FragmentWriterViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public FragmentWriterSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public FragmentWriterSelectViewModel(Action<FragmentWriterViewModel> onOk) {
            OnOk = onOk;
        }

        public FragmentWriterViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<FragmentWriterViewModel> FragmentWriterVms {
            get {
                return AppRoot.FragmentWriterVms.List;
            }
        }
    }
}
