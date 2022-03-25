﻿using Lucky.Core;
using System;
using System.Linq;
using System.Windows.Input;

namespace Lucky.Vms {
    public class PoolKernelViewModel : ViewModelBase, IPoolKernel, IEditableViewModel {
        private Guid _id;
        private Guid _poolId;
        private Guid _kernelId;
        private string _args;
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public PoolKernelViewModel(IPoolKernel data) : this(data.GetId()) {
            _poolId = data.PoolId;
            _kernelId = data.KernelId;
            _args = data.Args;
        }

        public PoolKernelViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (LuckyContext.Instance.ServerContext.PoolKernelSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdatePoolKernelCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommandTpl<FormType?>((formType) => {
                VirtualRoot.Execute(new EditPoolKernelCommand(formType ?? FormType.Edit, this));
            });
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public Guid PoolId {
            get {
                return _poolId;
            }
            set {
                if (_poolId != value) {
                    _poolId = value;
                    OnPropertyChanged(nameof(PoolId));
                }
            }
        }

        public string PoolName {
            get {
                return PoolVm.Name;
            }
        }

        private PoolViewModel _poolVm;
        public PoolViewModel PoolVm {
            get {
                if (_poolVm == null || this.PoolId != _poolVm.Id) {
                    AppRoot.PoolVms.TryGetPoolVm(this.PoolId, out _poolVm);
                    if (_poolVm == null) {
                        _poolVm = PoolViewModel.Empty;
                    }
                }
                return _poolVm;
            }
        }

        private CoinKernelViewModel _coinKernelVm;
        public CoinKernelViewModel CoinKernelVm {
            get {
                if (_coinKernelVm == null) {
                    _coinKernelVm = AppRoot.CoinKernelVms.AllCoinKernels.FirstOrDefault(a => a.KernelId == this.KernelId && a.CoinId == this.PoolVm.CoinId);
                }
                return _coinKernelVm;
            }
        }

        public Guid KernelId {
            get => _kernelId;
            set {
                if (_kernelId != value) {
                    _kernelId = value;
                    OnPropertyChanged(nameof(KernelId));
                }
            }
        }

        public string DisplayName {
            get {
                return $"{Kernel.Code}{Kernel.Version}";
            }
        }

        public KernelViewModel Kernel {
            get {
                if (AppRoot.KernelVms.TryGetKernelVm(this.KernelId, out KernelViewModel kernel)) {
                    return kernel;
                }
                return KernelViewModel.Empty;
            }
        }

        public string Args {
            get { return _args; }
            set {
                if (_args != value) {
                    _args = value;
                    OnPropertyChanged(nameof(Args));
                    CoinViewModel coinVm = AppRoot.MinerProfileVm.CoinVm;
                    if (coinVm != null && coinVm.CoinProfile != null && (coinVm.CoinProfile.PoolId == this.PoolId || coinVm.CoinProfile.PoolId1 == this.PoolId)) {
                        LuckyContext.RefreshArgsAssembly.Invoke("当前选用的内核引用的矿池级内核上放置的矿池级内核参数发生了变更");
                    }
                }
            }
        }
    }
}
