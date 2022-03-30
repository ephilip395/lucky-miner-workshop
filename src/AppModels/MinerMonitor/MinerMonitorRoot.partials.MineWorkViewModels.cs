using Lucky.Core;
using Lucky.MinerMonitor.Vms;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.MinerMonitor
{
    public static partial class MinerMonitorRoot
    {
        public class MineWorkViewModels : ViewModelBase
        {
            public static MineWorkViewModels Instance { get; private set; } = new MineWorkViewModels();
            private readonly Dictionary<Guid, MineWorkViewModel> _dicById = new Dictionary<Guid, MineWorkViewModel>();
            public ICommand Add { get; private set; }

            private MineWorkViewModels()
            {
                if (WpfUtil.IsInDesignMode)
                {
                    return;
                }
                foreach (var item in LuckyContext.MinerMonitorContext.MineWorkSet.AsEnumerable().ToArray())
                {
                    if (!_dicById.ContainsKey(item.Id))
                    {
                        _dicById.Add(item.Id, new MineWorkViewModel(item));
                    }
                }
                AppRoot.BuildEventPath<MineWorkSetInitedEvent>("作业集初始化后初始化Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
                {
                    _dicById.Clear();
                    foreach (var item in LuckyContext.MinerMonitorContext.MineWorkSet.AsEnumerable().ToArray())
                    {
                        if (!_dicById.ContainsKey(item.Id))
                        {
                            _dicById.Add(item.Id, new MineWorkViewModel(item));
                        }
                    }
                    OnPropertyChangeds();
                    MinersWindowViewModel.Instance.RefreshMinerTweaksSelectedMineWork(MinersWindowViewModel.Instance.MinerTweaks.ToArray());
                });
                this.Add = new DelegateCommand(() =>
                {
                    new MineWorkViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AppRoot.BuildEventPath<MineWorkAddedEvent>("添加作业后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        if (!_dicById.TryGetValue(message.Source.GetId(), out MineWorkViewModel vm))
                        {
                            vm = new MineWorkViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), vm);
                            OnPropertyChangeds();
                            if (message.Source.GetId() == MinerTweaksWindowVm.SelectedMineWork.GetId())
                            {
                                MinerTweaksWindowVm.SelectedMineWork = MineWorkViewModel.PleaseSelect;
                            }
                        }
                    });
                AppRoot.BuildEventPath<MineWorkUpdatedEvent>("添加作业后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        if (_dicById.TryGetValue(message.Source.GetId(), out MineWorkViewModel vm))
                        {
                            vm.Update(message.Source);
                        }
                    });
                AppRoot.BuildEventPath<MineWorkRemovedEvent>("移除了作业后刷新Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
                {
                    if (_dicById.TryGetValue(message.Source.Id, out MineWorkViewModel vm))
                    {
                        _dicById.Remove(vm.Id);
                        OnPropertyChangeds();
                        if (vm.Id == MinerTweaksWindowVm.SelectedMineWork.GetId())
                        {
                            MinerTweaksWindowVm.SelectedMineWork = MineWorkViewModel.PleaseSelect;
                        }
                    }
                });
            }

            private void OnPropertyChangeds()
            {
                OnPropertyChanged(nameof(List));
                OnPropertyChanged(nameof(MineWorkVmItems));
            }

            public List<MineWorkViewModel> List
            {
                get
                {
                    return _dicById.Values.ToList();
                }
            }

            private IEnumerable<MineWorkViewModel> GetMineWorkVmItems()
            {
                yield return MineWorkViewModel.PleaseSelect;
                yield return MineWorkViewModel.SelfMineWork;
                foreach (var item in List)
                {
                    yield return item;
                }
            }

            public List<MineWorkViewModel> MineWorkVmItems
            {
                get
                {
                    return GetMineWorkVmItems().ToList();
                }
            }

            public bool TryGetMineWorkVm(Guid id, out MineWorkViewModel mineWorkVm)
            {
                if (id.IsSelfMineWorkId())
                {
                    mineWorkVm = MineWorkViewModel.SelfMineWork;
                    return true;
                }
                return _dicById.TryGetValue(id, out mineWorkVm);
            }
        }
    }
}
