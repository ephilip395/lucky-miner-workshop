﻿using System.Linq;

namespace Lucky.Vms {
    public class CoinGroupPageViewModel : ViewModelBase {
        public CoinGroupPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this._currentGroup = GroupVms.List.FirstOrDefault();
        }

        private GroupViewModel _currentGroup;
        public GroupViewModel CurrentGroup {
            get { return _currentGroup; }
            set {
                if (_currentGroup != value) {
                    _currentGroup = value;
                    OnPropertyChanged(nameof(CurrentGroup));
                }
            }
        }

        public AppRoot.GroupViewModels GroupVms {
            get {
                return AppRoot.GroupVms;
            }
        }
    }
}
