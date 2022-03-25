﻿using System;
using System.Linq;
using System.Windows.Input;

namespace Lucky.Vms {
    public class KernelOutputPageViewModel : ViewModelBase {
        public ICommand Add { get; private set; }

        public KernelOutputPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {
                new KernelOutputViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
            _currentKernelOutputVm = AppRoot.KernelOutputVms.AllKernelOutputVms.FirstOrDefault();
        }

        private KernelOutputViewModel _currentKernelOutputVm;

        public KernelOutputViewModel CurrentKernelOutputVm {
            get {
                return _currentKernelOutputVm;
            }
            set {
                if (_currentKernelOutputVm != value) {
                    _currentKernelOutputVm = value;
                    OnPropertyChanged(nameof(CurrentKernelOutputVm));
                }
            }
        }

        public AppRoot.KernelOutputViewModels KernelOutputVms {
            get {
                return AppRoot.KernelOutputVms;
            }
        }
    }
}
