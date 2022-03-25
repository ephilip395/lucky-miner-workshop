﻿using Lucky.Core;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky {
    public static partial class AppRoot {
        public class PoolKernelViewModels : ViewModelBase {
            public static PoolKernelViewModels Instance { get; private set; } = new PoolKernelViewModels();

            private readonly Dictionary<Guid, PoolKernelViewModel> _dicById = new Dictionary<Guid, PoolKernelViewModel>();
            private PoolKernelViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新视图界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.BelowNormal,
                    path: message => {
                        OnPropertyChanged(nameof(AllPoolKernels));
                    });
                BuildEventPath<PoolKernelAddedEvent>("刷新矿池内核VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            if (PoolVms.TryGetPoolVm(message.Source.PoolId, out PoolViewModel poolVm)) {
                                _dicById.Add(message.Source.GetId(), new PoolKernelViewModel(message.Source));
                                poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                            }
                        }
                    });
                BuildEventPath<PoolKernelRemovedEvent>("刷新矿池内核VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            var vm = _dicById[message.Source.GetId()];
                            _dicById.Remove(message.Source.GetId());
                            if (PoolVms.TryGetPoolVm(vm.PoolId, out PoolViewModel poolVm)) {
                                poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                            }
                        }
                    });
                BuildEventPath<PoolKernelUpdatedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out PoolKernelViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (IPoolKernel item in LuckyContext.Instance.ServerContext.PoolKernelSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new PoolKernelViewModel(item));
                }
            }

            public List<PoolKernelViewModel> AllPoolKernels {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
