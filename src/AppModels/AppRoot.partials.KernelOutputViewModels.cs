﻿using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky {
    public static partial class AppRoot {
        public class KernelOutputViewModels : ViewModelBase {
            public static KernelOutputViewModels Instance { get; private set; } = new KernelOutputViewModels();
            private readonly Dictionary<Guid, KernelOutputViewModel> _dicById = new Dictionary<Guid, KernelOutputViewModel>();

            private KernelOutputViewModels() {
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
                        AllPropertyChanged();
                    });
                BuildEventPath<KernelOutputAddedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        var vm = new KernelOutputViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), vm);
                        OnPropertyChanged(nameof(AllKernelOutputVms));
                        OnPropertyChanged(nameof(PleaseSelectVms));
                    });
                BuildEventPath<KernelOutputUpdatedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputViewModel vm)) {
                            if (vm != null) {
                                vm.Update(message.Source);
                            }
                        }
                    });
                BuildEventPath<KernelOutputRemovedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Remove(message.Source.GetId());
                            OnPropertyChanged(nameof(AllKernelOutputVms));
                            OnPropertyChanged(nameof(PleaseSelectVms));
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in LuckyContext.Instance.ServerContext.KernelOutputSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new KernelOutputViewModel(item));
                }
            }

            public bool TryGetKernelOutputVm(Guid id, out KernelOutputViewModel kernelOutputVm) {
                return _dicById.TryGetValue(id, out kernelOutputVm);
            }

            public List<KernelOutputViewModel> AllKernelOutputVms {
                get {
                    return _dicById.Values.OrderBy(a => a.Name).ToList();
                }
            }

            private IEnumerable<KernelOutputViewModel> GetPleaseSelectVms() {
                yield return KernelOutputViewModel.PleaseSelect;
                foreach (var item in _dicById.Values.OrderBy(a => a.Name)) {
                    yield return item;
                }
            }

            public List<KernelOutputViewModel> PleaseSelectVms {
                get {
                    return GetPleaseSelectVms().ToList();
                }
            }
        }
    }
}
