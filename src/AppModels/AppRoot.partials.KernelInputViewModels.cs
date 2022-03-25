﻿using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky {
    public static partial class AppRoot {
        public class KernelInputViewModels : ViewModelBase {
            public static KernelInputViewModels Instance { get; private set; } = new KernelInputViewModels();
            private readonly Dictionary<Guid, KernelInputViewModel> _dicById = new Dictionary<Guid, KernelInputViewModel>();

            private KernelInputViewModels() {
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
                        OnPropertyChangeds();
                    });
                BuildEventPath<KernelInputAddedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        var vm = new KernelInputViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), vm);
                        OnPropertyChangeds();
                    });
                BuildEventPath<KernelInputUpdatedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelInputViewModel vm)) {
                            if (vm != null) {
                                bool isSupportDualMine = vm.IsSupportDualMine;
                                string args = vm.Args;
                                vm.Update(message.Source);
                                if (args != vm.Args) {
                                    CoinViewModel coinVm = MinerProfileVm.CoinVm;
                                    if (coinVm != null && coinVm.CoinKernel != null && coinVm.CoinKernel.Kernel.KernelInputId == vm.Id) {
                                        LuckyContext.RefreshArgsAssembly.Invoke("当前选用的内核引用的内核输入的形参发生了变更");
                                    }
                                }
                                if (isSupportDualMine != vm.IsSupportDualMine) {
                                    foreach (var coinKernelVm in CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Source.GetId())) {
                                        coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                                    }
                                }
                            }
                        }
                    });
                BuildEventPath<KernelInputRemovedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Remove(message.Source.GetId());
                            OnPropertyChangeds();
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in LuckyContext.Instance.ServerContext.KernelInputSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new KernelInputViewModel(item));
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(AllKernelInputVms));
                OnPropertyChanged(nameof(PleaseSelectVms));
            }

            public bool TryGetKernelInputVm(Guid id, out KernelInputViewModel kernelInputVm) {
                return _dicById.TryGetValue(id, out kernelInputVm);
            }

            public List<KernelInputViewModel> AllKernelInputVms {
                get {
                    return _dicById.Values.OrderBy(a => a.Name).ToList();
                }
            }

            private IEnumerable<KernelInputViewModel> GetPleaseSelectVms() {
                yield return KernelInputViewModel.PleaseSelect;
                foreach (var item in _dicById.Values.OrderBy(a => a.Name)) {
                    yield return item;
                }
            }

            public List<KernelInputViewModel> PleaseSelectVms {
                get {
                    return GetPleaseSelectVms().ToList();
                }
            }
        }
    }
}
