﻿using Lucky.Core.MinerMonitor;
using Lucky.MinerMonitor.Vms;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.MinerMonitor {
    public static partial class MinerMonitorRoot {
        public class ColumnsShowViewModels : ViewModelBase {
            public static ColumnsShowViewModels Instance { get; private set; } = new ColumnsShowViewModels();

            private readonly Dictionary<Guid, ColumnsShowViewModel> _dicById = new Dictionary<Guid, ColumnsShowViewModel>();

            public ICommand Add { get; private set; }

            private ColumnsShowViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                this.Add = new DelegateCommand(() => {
                    WpfUtil.ShowInputDialog("视图名称", string.Empty, string.Empty, columnsShowName => {
                        if (string.IsNullOrEmpty(columnsShowName)) {
                            return "视图名称是必须的";
                        }
                        return string.Empty;
                    }, onOk: columnsShowName => {
                        ColumnsShowData entity = new ColumnsShowData {
                            ColumnsShowName = columnsShowName,
                            LastActivedOnText = true,
                            BootTimeSpanText = true,
                            MineTimeSpanText = true,
                            Work = true,
                            MinerGroup = true,
                            MinerName = true,
                            WorkerName = true,
                            LocalIp = true,
                            MinerIp = true,
                            GpuType = true,
                            MainCoinCode = true,
                            MainCoinSpeedText = true,
                            MainCoinRejectPercentText = true
                        };
                        LuckyContext.MinerMonitorContext.ColumnsShowSet.AddOrUpdate(entity);
                    });
                });
                AppRoot.BuildEventPath<ColumnsShowAddedOrUpdatedEvent>("添加或修改了视图后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (!_dicById.TryGetValue(message.Source.GetId(), out ColumnsShowViewModel vm)) {
                            vm = new ColumnsShowViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), vm);
                            OnPropertyChanged(nameof(List));
                            MinerTweaksWindowVm.ColumnsShow = vm;
                        }
                        else {
                            vm.Update(message.Source);
                        }
                    });
                AppRoot.BuildEventPath<ColumnsRemovedEvent>("删除了视图后刷新Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                    if (_dicById.ContainsKey(message.Source.Id)) {
                        _dicById.Remove(message.Source.Id);
                        OnPropertyChanged(nameof(List));
                        if (_dicById.TryGetValue(ColumnsShowData.PleaseSelect.Id, out ColumnsShowViewModel vm)) {
                            MinerTweaksWindowVm.ColumnsShow = vm;
                        }
                    }
                });

                foreach (var item in LuckyContext.MinerMonitorContext.ColumnsShowSet.GetAll()) {
                    _dicById.Add(item.Id, new ColumnsShowViewModel(item));
                }
            }

            public List<ColumnsShowViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
