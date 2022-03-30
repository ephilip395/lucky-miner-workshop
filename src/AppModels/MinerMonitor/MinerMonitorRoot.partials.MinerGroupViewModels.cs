﻿using Lucky.MinerMonitor.Vms;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.MinerMonitor {
    public static partial class MinerMonitorRoot {
        public class MinerGroupViewModels : ViewModelBase {
            public static MinerGroupViewModels Instance { get; private set; } = new MinerGroupViewModels();
            private readonly Dictionary<Guid, MinerGroupViewModel> _dicById = new Dictionary<Guid, MinerGroupViewModel>();

            public ICommand Add { get; private set; }

            private MinerGroupViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                foreach (var item in LuckyContext.MinerMonitorContext.MinerGroupSet.AsEnumerable().ToArray()) {
                    if (!_dicById.ContainsKey(item.Id)) {
                        _dicById.Add(item.Id, new MinerGroupViewModel(item));
                    }
                }
                AppRoot.BuildEventPath<MinerGroupSetInitedEvent>("矿工组集初始化后初始化Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                    _dicById.Clear();
                    foreach (var item in LuckyContext.MinerMonitorContext.MinerGroupSet.AsEnumerable().ToArray()) {
                        if (!_dicById.ContainsKey(item.Id)) {
                            _dicById.Add(item.Id, new MinerGroupViewModel(item));
                        }
                    }
                    this.OnPropertyChangeds();
                    MinersWindowViewModel.Instance.RefreshMinerTweaksSelectedMinerGroup(MinersWindowViewModel.Instance.MinerTweaks.ToArray());
                });
                this.Add = new DelegateCommand(() => {
                    new MinerGroupViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AppRoot.BuildEventPath<MinerGroupAddedEvent>("添加矿机分组后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (!_dicById.TryGetValue(message.Source.GetId(), out MinerGroupViewModel vm)) {
                            vm = new MinerGroupViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), vm);
                            OnPropertyChangeds();
                            MinerTweaksWindowVm.OnPropertyChanged(nameof(MinersWindowViewModel.SelectedMinerGroup));
                        }
                    });
                AppRoot.BuildEventPath<MinerGroupUpdatedEvent>("添加矿机分组后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out MinerGroupViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    });
                AppRoot.BuildEventPath<MinerGroupRemovedEvent>("移除了矿机组后刷新Vm内容", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                    if (_dicById.TryGetValue(message.Source.Id, out MinerGroupViewModel vm)) {
                        _dicById.Remove(vm.Id);
                        OnPropertyChangeds();
                        MinerTweaksWindowVm.OnPropertyChanged(nameof(MinersWindowViewModel.SelectedMinerGroup));
                    }
                });
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
                OnPropertyChanged(nameof(MinerGroupItems));
            }

            public List<MinerGroupViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }

            public bool TryGetMineWorkVm(Guid id, out MinerGroupViewModel minerGroupVm) {
                return _dicById.TryGetValue(id, out minerGroupVm);
            }

            private IEnumerable<MinerGroupViewModel> GetMinerGroupItems() {
                yield return MinerGroupViewModel.PleaseSelect;
                foreach (var item in List) {
                    yield return item;
                }
            }

            public List<MinerGroupViewModel> MinerGroupItems {
                get {
                    return GetMinerGroupItems().ToList();
                }
            }
        }
    }
}
