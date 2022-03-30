﻿using Lucky.Core;
using Lucky.Core.MinerServer;
using Lucky.Core.MinerMonitor;
using Lucky.Gpus;
using Lucky.Vms;
using Lucky.Ws;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms
{
    public class MinersWindowViewModel : ViewModelBase, IWsStateViewModel
    {
        public static MinersWindowViewModel Instance { get; private set; } = new MinersWindowViewModel();

        private readonly List<CoinSnapshotViewModel> _coinSnapshotVms = null;
        private CoinSnapshotViewModel _coinSnapshotVm = CoinSnapshotViewModel.PleaseSelect;
        private ColumnsShowViewModel _columnsShow;
        private int _countDown = 10;
        private ObservableCollection<MinerViewModel> _minerClients = new ObservableCollection<MinerViewModel>();
        private MinerViewModel _currentMinerTweak;
        private MinerViewModel[] _selectedMinerTweaks = new MinerViewModel[0];
        private int _pageIndex = 1;
        private int _total;
        private EnumItem<MineStatus> _mineStatusEnumItem;
        private string _minerIp;
        private string _minerName;
        private string _version;
        private string _kernel;
        private string _wallet;
        private string _pool;
        private PoolViewModel _poolVm = PoolViewModel.PleaseSelect;
        private MineWorkViewModel _selectedMineWork = MineWorkViewModel.PleaseSelect;
        private MinerGroupViewModel _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
        private uint _maxTemp = 80;
        private int _frozenColumnCount = 8;
        private uint _minTemp = 40;
        private int _rejectPercent = 10;
        private readonly Dictionary<ClientDataSortField, SortDirection> _sortDirection = new Dictionary<ClientDataSortField, SortDirection>
        {
            [ClientDataSortField.MinerName] = SortDirection.Ascending,
            [ClientDataSortField.MainCoinSpeed] = SortDirection.Ascending,
            [ClientDataSortField.CpuTemperature] = SortDirection.Descending,
            [ClientDataSortField.DualCoinPoolDelay] = SortDirection.Descending,
            [ClientDataSortField.DualCoinRejectPercent] = SortDirection.Descending,
            [ClientDataSortField.KernelSelfRestartCount] = SortDirection.Descending,
            [ClientDataSortField.MainCoinPoolDelay] = SortDirection.Descending,
            [ClientDataSortField.MainCoinRejectPercent] = SortDirection.Descending,
            [ClientDataSortField.DiskSpace] = SortDirection.Ascending
        };
        private ClientDataSortField _sortField = ClientDataSortField.MinerName;
        private string _gpuName;
        private string _gpuDriver;
        private GpuType _gpuType = GpuType.Empty;

        private bool _isWsOnline;
        private string _wsDescription;
        private int _wsNextTrySecondsDelay;
        private DateTime _wsLastTryOn;
        private bool _isConnecting;
        private double _wsRetryIconAngle;

        private bool _isLoading = false;
        private double _lodingIconAngle;
        private ClientDataSortField _lastSortField = ClientDataSortField.MinerName;
        private string _wsServerIp;
        private readonly Dictionary<ClientDataSortField, SortDirection> _lastSortDirection;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                    OnPropertyChanged(nameof(IsNoRecordVisible));
                    if (value)
                    {
                        VirtualRoot.SetInterval(per: TimeSpan.FromMilliseconds(100), perCallback: () =>
                        {
                            this.LoadingIconAngle += 30;
                        }, stopCallback: () =>
                        {
                            this.IsLoading = false;
                        }, timeout: TimeSpan.FromSeconds(6), requestStop: () =>
                        {
                            return !IsLoading;
                        });
                    }
                }
            }
        }

        public double LoadingIconAngle
        {
            get => _lodingIconAngle;
            set
            {
                _lodingIconAngle = value;
                OnPropertyChanged(nameof(LoadingIconAngle));
            }
        }

        public GridLength MinerListHeight
        {
            get
            {
                if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(nameof(MinerListHeight), out IAppSetting appSetting) && appSetting.Value != null)
                {
                    if (double.TryParse(appSetting.Value.ToString(), out double value))
                    {
                        return new GridLength(value, GridUnitType.Star);
                    }
                }
                return new GridLength(3, GridUnitType.Star);
            }
            set
            {
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                {
                    Key = nameof(MinerListHeight),
                    Value = value.Value.ToString()
                }));
                OnPropertyChanged(nameof(MinerListHeight));
            }
        }

        public GridLength MinerDetailsHeight
        {
            get
            {
                if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(nameof(MinerDetailsHeight), out IAppSetting appSetting) && appSetting.Value != null)
                {
                    if (double.TryParse(appSetting.Value.ToString(), out double value))
                    {
                        return new GridLength(value, GridUnitType.Star);
                    }
                }
                return new GridLength(5, GridUnitType.Star);
            }
            set
            {
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                {
                    Key = nameof(MinerDetailsHeight),
                    Value = value.Value.ToString()
                }));
                OnPropertyChanged(nameof(MinerDetailsHeight));
            }
        }

        public GridLength ConsoleWidth
        {
            get
            {
                if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(nameof(ConsoleWidth), out IAppSetting appSetting) && appSetting.Value != null)
                {
                    if (double.TryParse(appSetting.Value.ToString(), out double value))
                    {
                        return new GridLength(value, GridUnitType.Pixel);
                    }
                }
                return new GridLength(756, GridUnitType.Pixel);
            }
            set
            {
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                {
                    Key = nameof(ConsoleWidth),
                    Value = value.Value.ToString()
                }));
                OnPropertyChanged(nameof(ConsoleWidth));
            }
        }

        public GridLength MinerDetailsWidth
        {
            get
            {
                if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(nameof(MinerDetailsWidth), out IAppSetting appSetting) && appSetting.Value != null)
                {
                    if (double.TryParse(appSetting.Value.ToString(), out double value))
                    {
                        return new GridLength(value, GridUnitType.Star);
                    }
                }
                return new GridLength(1, GridUnitType.Star);
            }
            set
            {
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                {
                    Key = nameof(MinerDetailsWidth),
                    Value = value.Value.ToString()
                }));
                OnPropertyChanged(nameof(MinerDetailsWidth));
            }
        }

        #region QueryMinerTweaks
        public void QueryMinerTweaks(bool isAuto = false)
        {
            if (!isAuto)
            {
                this.IsLoading = true;
            }
            else
            {
                this.IsLoading = false;
            }
            Guid? groupId = null;
            if (SelectedMinerGroup == null)
            {
                _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
                OnPropertyChanged(nameof(SelectedMinerGroup));
            }
            if (SelectedMinerGroup != MinerGroupViewModel.PleaseSelect)
            {
                groupId = SelectedMinerGroup.Id;
            }
            Guid? workId = null;
            if (SelectedMineWork == null)
            {
                _selectedMineWork = MineWorkViewModel.PleaseSelect;
                OnPropertyChanged(nameof(SelectedMineWork));
            }
            if (SelectedMineWork != MineWorkViewModel.PleaseSelect)
            {
                workId = SelectedMineWork.Id;
            }
            string coin = string.Empty;
            string wallet = string.Empty;
            if (workId == null || workId.Value == Guid.Empty)
            {
                if (this.CoinSnapshotVm != CoinSnapshotViewModel.PleaseSelect && this.CoinSnapshotVm != null)
                {
                    coin = this.CoinSnapshotVm.CoinVm.Code;
                }
                if (!string.IsNullOrEmpty(Wallet))
                {
                    wallet = this.Wallet;
                }
            }
            MinerMonitorRoot.MinerMonitorService.QueryClientsAsync(new QueryClientsRequest
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                WorkId = workId,
                GroupId = groupId,
                MinerIp = this.MinerIp,
                MinerName = this.MinerName,
                MineState = this.MineStatusEnumItem.Value,
                Coin = coin,
                Pool = this.Pool,
                Wallet = wallet,
                Version = this.Version,
                Kernel = this.Kernel,
                GpuType = GpuType,
                GpuName = GpuName,
                GpuDriver = GpuDriver,
                SortField = SortField,
                SortDirection = this._sortDirection[SortField]
            });
            this.ResetCountDown();
        }

        private void AddEventPath()
        {
            VirtualRoot.BuildEventPath<QueryClientsResponseEvent>("收到QueryClientsResponse响应后刷新界面", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
            {
                this.ResetCountDown();
                this.IsLoading = false;
                var response = message.Response;
                if (response.IsSuccess())
                {
                    #region 处理Response.Data
                    if (_lastSortField == this.SortField && _lastSortDirection[this.SortField] == this._sortDirection[this.SortField])
                    {
                        UIThread.Execute(() =>
                        {
                            if (response.Data.Count == 0)
                            {
                                _minerClients.Clear();
                            }
                            else
                            {
                                var toRemoves = _minerClients.Where(a => response.Data.All(b => b.Id != a.Id)).ToArray();
                                foreach (var item in toRemoves)
                                {
                                    _minerClients.Remove(item);
                                }
                                for (int i = 0; i < response.Data.Count; i++)
                                {
                                    var data = response.Data[i];
                                    var item = _minerClients.FirstOrDefault(a => a.Id == data.Id);
                                    if (item == null)
                                    {
                                        // 因为内网模式时是本地调用，未经过网络传输，所以MinerTweakViewModel内部的ClientData类型的
                                        // _data字段会和response.Data[i]是同一性的，所以这里Clone一次以防止Vm.Update的时候无效
                                        var clientData = data;
                                        MinerViewModel vm;
                                        if (!RpcRoot.IsOuterNet)
                                        {
                                            clientData = ClientData.Clone(data);
                                            vm = new MinerViewModel(clientData);
                                        }
                                        else
                                        {
                                            vm = new MinerViewModel(clientData);
                                        }
                                        _minerClients.Insert(i, vm);
                                    }
                                    else
                                    {
                                        item.Update(data);
                                    }
                                }
                                var items = _minerClients.ToArray();
                                RefreshMaxTempForeground(items);
                                RefreshRejectPercentForeground(items);
                            }
                            OnPropertyChanged(nameof(IsNoRecordVisible));
                        });
                    }
                    else
                    {
                        _lastSortField = this.SortField;
                        _lastSortDirection[this.SortField] = _sortDirection[_lastSortField];
                        if (response.Data.Count == 0)
                        {
                            // ObservableCollection<T>类型对象不支持在UI线程以外处理
                            UIThread.Execute(() =>
                            {
                                _minerClients.Clear();
                                OnPropertyChanged(nameof(IsNoRecordVisible));
                            });
                        }
                        else
                        {
                            List<MinerViewModel> vms = new List<MinerViewModel>(_minerClients);
                            var toRemoves = vms.Where(a => response.Data.All(b => b.Id != a.Id)).ToArray();
                            foreach (var item in toRemoves)
                            {
                                vms.Remove(item);
                            }
                            for (int i = 0; i < response.Data.Count; i++)
                            {
                                var data = response.Data[i];
                                var item = vms.FirstOrDefault(a => a.Id == data.Id);
                                if (item == null)
                                {
                                    // 因为内网模式时是本地调用，未经过网络传输，所以MinerTweakViewModel内部的ClientData类型的
                                    // _data字段会和response.Data[i]是同一性的，所以这里Clone一次以防止Vm.Update的时候无效
                                    var clientData = data;
                                    MinerViewModel vm;
                                    if (!RpcRoot.IsOuterNet)
                                    {
                                        clientData = ClientData.Clone(data);
                                        vm = new MinerViewModel(clientData);
                                    }
                                    else
                                    {
                                        vm = new MinerViewModel(clientData);
                                    }
                                    vms.Insert(i, vm);
                                }
                                else
                                {
                                    item.Update(data);
                                }
                            }
                            vms.Sort(new ClientDataComparer(_sortDirection[this.SortField], _sortField));
                            _minerClients = new ObservableCollection<MinerViewModel>(vms);
                            OnPropertyChanged(nameof(MinerTweaks));
                            OnPropertyChanged(nameof(IsNoRecordVisible));
                            var items = _minerClients.ToArray();
                            RefreshMaxTempForeground(items);
                            RefreshRejectPercentForeground(items);
                        }
                    }
                    RefreshPagingUi(response.Total);
                    var array = this.MinerTweaks.ToArray();
                    RefreshMaxTempForeground(array);
                    RefreshRejectPercentForeground(array);
                    #endregion
                    #region 处理Response.LatestSnapshots
                    foreach (var item in CoinSnapshotVms)
                    {
                        if (item == CoinSnapshotViewModel.PleaseSelect)
                        {
                            item.CoinSnapshotDataVm.MainCoinMiningCount = response.TotalMiningCount;
                            item.CoinSnapshotDataVm.MainCoinOnlineCount = response.TotalOnlineCount;
                            continue;
                        }
                        var data = response.LatestSnapshots.FirstOrDefault(a => a.CoinCode == item.CoinVm.Code);
                        if (data != null)
                        {
                            item.CoinSnapshotDataVm.Update(data);
                        }
                        else
                        {
                            item.CoinSnapshotDataVm.Update(CoinSnapshotData.CreateEmpty(item.CoinVm.Code));
                        }
                    }
                    #endregion
                }
            });
        }
        #endregion

        #region WpfCommands
        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }
        public ICommand SelfMineWork { get; private set; }

        public ICommand PageUp { get; private set; }
        public ICommand PageDown { get; private set; }
        public ICommand PageFirst { get; private set; }
        public ICommand PageLast { get; private set; }
        public ICommand PageRefresh { get; private set; }
        public ICommand AddMinerTweak { get; private set; }
        public ICommand RemoveMinerTweaks { get; private set; }
        public ICommand OneKeyWork { get; private set; }
        public ICommand OneKeyGroup { get; private set; }
        public ICommand OneKeyOverClock { get; private set; }
        public ICommand OneKeyWorkerNames { get; private set; }
        public ICommand OneKeySetting { get; private set; }
        public ICommand EnableRemoteDesktop { get; private set; }
        public ICommand RemoteDesktop { get; private set; }
        public ICommand BlockWAU { get; private set; }
        public ICommand PowerCfgOff { get; private set; }
        public ICommand VirtualMemory { get; private set; }
        public ICommand LocalIpConfig { get; private set; }
        public ICommand SwitchRadeonGpu { get; private set; }
        public ICommand WsRetry { get; private set; }
        public ICommand CopyMainCoinWallet { get; private set; }

        public ICommand SortByMinerName { get; private set; }
        public ICommand SortByMainCoinSpeed { get; private set; }
        public ICommand SortByMainCoinRejectPercent { get; private set; }
        public ICommand SortByDualCoinRejectPercent { get; private set; }
        public ICommand SortByMainCoinPoolDelay { get; private set; }
        public ICommand SortByDualCoinPoolDelay { get; private set; }
        public ICommand SortByCpuTemperature { get; private set; }
        public ICommand SortByKernelSelfRestartCount { get; private set; }
        public ICommand SortByDiskSpace { get; private set; }
        public ICommand SwitchService { get; private set; }
        public ICommand EmptyCommandForSelectdOne { get; private set; }
        public ICommand EmptyCommandForSelectedAny { get; private set; }

        #endregion

        #region ctor
        public MinersWindowViewModel()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            AddEventPath();
            var appSettings = VirtualRoot.LocalAppSettingSet;
            #region 状态栏的几条配置
            if (appSettings.TryGetAppSetting(LuckyKeyword.FrozenColumnCountAppSettingKey, out IAppSetting frozenColumnCountAppSetting) && frozenColumnCountAppSetting.Value != null)
            {
                if (int.TryParse(frozenColumnCountAppSetting.Value.ToString(), out int frozenColumnCount))
                {
                    _frozenColumnCount = frozenColumnCount;
                }
            }
            if (appSettings.TryGetAppSetting(LuckyKeyword.MaxTempAppSettingKey, out IAppSetting maxTempAppSetting) && maxTempAppSetting.Value != null)
            {
                if (uint.TryParse(maxTempAppSetting.Value.ToString(), out uint maxTemp))
                {
                    _maxTemp = maxTemp;
                }
            }
            if (appSettings.TryGetAppSetting(LuckyKeyword.MinTempAppSettingKey, out IAppSetting minTempAppSetting) && minTempAppSetting.Value != null)
            {
                if (uint.TryParse(minTempAppSetting.Value.ToString(), out uint minTemp))
                {
                    _minTemp = minTemp;
                }
            }
            if (appSettings.TryGetAppSetting(LuckyKeyword.RejectPercentAppSettingKey, out IAppSetting rejectPercentAppSetting) && rejectPercentAppSetting.Value != null)
            {
                if (int.TryParse(rejectPercentAppSetting.Value.ToString(), out int rejectPercent))
                {
                    _rejectPercent = rejectPercent;
                }
            }
            #endregion
            Guid columnsShowId = ColumnsShowData.PleaseSelect.Id;
            if (appSettings.TryGetAppSetting(LuckyKeyword.ColumnsShowIdAppSettingKey, out IAppSetting columnsShowAppSetting) && columnsShowAppSetting.Value != null)
            {
                if (Guid.TryParse(columnsShowAppSetting.Value.ToString(), out Guid guid))
                {
                    columnsShowId = guid;
                }
            }
            _columnsShow = ColumnsShows.List.FirstOrDefault(a => a.Id == columnsShowId);
            if (_columnsShow == null)
            {
                _columnsShow = ColumnsShows.List.FirstOrDefault();
            }
            _coinSnapshotVms = new List<CoinSnapshotViewModel> { CoinSnapshotViewModel.PleaseSelect };
            _coinSnapshotVms.AddRange(
                AppRoot.CoinVms.AllCoins.Select(
                    a => new CoinSnapshotViewModel(a, new CoinSnapshotDataViewModel(CoinSnapshotData.CreateEmpty(a.Code)))
                    )
                );
            this._mineStatusEnumItem = Enums.MineStatusEnumItems.FirstOrDefault(a => a.Value == MineStatus.All);
            this._pool = string.Empty;
            this._wallet = string.Empty;
            this.OneKeySetting = new DelegateCommand(() =>
            {
                VirtualRoot.Execute(new ShowMinerTweakSettingCommand(new MinerSettingViewModel(this.SelectedMinerTweaks)));
            }, IsSelectedAny);
            this.OneKeyWorkerNames = new DelegateCommand(() =>
            {
                #region
                if (this.SelectedMinerTweaks.Length == 1)
                {
                    var selectedMinerTweak = this.SelectedMinerTweaks[0];
                    WpfUtil.ShowInputDialog("群控名", selectedMinerTweak.WorkerName, "注意：下次挖矿生效", null, minerName =>
                    {
                        selectedMinerTweak.WorkerName = minerName;
                        VirtualRoot.Out.ShowSuccess("设置群控名成功，下次挖矿生效。", toConsole: true);
                    });
                }
                else
                {
                    MinerNamesSeterViewModel vm = null;
                    vm = new MinerNamesSeterViewModel(
                        prefix: "miner",
                        suffix: "01",
                        namesByObjectId: this.SelectedMinerTweaks.Select(a => new Tuple<string, string>(a.Id, string.Empty)).ToList(),
                        onOk: () =>
                        {
                            this.ResetCountDown();
                            MinerMonitorRoot.MinerMonitorService.UpdateClientsAsync(nameof(MinerViewModel.WorkerName), vm.NamesByObjectId.ToDictionary(a => a.Item1, a => (object)a.Item2), callback: (response, e) =>
                            {
                                if (response.IsSuccess())
                                {
                                    foreach (var kv in vm.NamesByObjectId)
                                    {
                                        var item = this.SelectedMinerTweaks.FirstOrDefault(a => a.Id == kv.Item1);
                                        if (item != null)
                                        {
                                            item.OnPropertyChanged(nameof(item.WorkerName));
                                        }
                                    }
                                    QueryMinerTweaks();
                                }
                            });
                        });
                    VirtualRoot.Execute(new ShowMinerNamesSeterCommand(vm));
                }
                #endregion
            }, IsSelectedAny);
            this.OneKeyWork = new DelegateCommandTpl<MineWorkViewModel>((work) =>
            {
                foreach (var item in SelectedMinerTweaks)
                {
                    item.SelectedMineWork = work;
                }
            }, (work) => IsSelectedAny());
            this.OneKeyGroup = new DelegateCommandTpl<MinerGroupViewModel>((group) =>
            {
                foreach (var item in SelectedMinerTweaks)
                {
                    item.SelectedMinerGroup = group;
                }
            }, (group) => IsSelectedAny());
            this.OneKeyOverClock = new DelegateCommand(() =>
            {
                if (this.SelectedMinerTweaks.Length == 1)
                {
                    VirtualRoot.Execute(new ShowGpuProfilesPageCommand(this));
                }
            }, IsSelectedOne);
            this.AddMinerTweak = new DelegateCommand(() =>
            {
                VirtualRoot.Execute(new ShowMinerTweakAddCommand());
            });
            this.RemoveMinerTweaks = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定删除选中的矿机吗？", title: "确认", onYes: () =>
                    {
                        this.ResetCountDown();
                        MinerMonitorRoot.MinerMonitorService.RemoveClientsAsync(SelectedMinerTweaks.Select(a => a.Id).ToList(), (response, e) =>
                        {
                            if (!response.IsSuccess())
                            {
                                VirtualRoot.Out.ShowError("删除矿机失败：" + response.ReadMessage(e), autoHideSeconds: 4, toConsole: true);
                            }
                            else
                            {
                                QueryMinerTweaks();
                            }
                        });
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.RestartWindows = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定重启选中的电脑吗？", title: "确认", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            MinerMonitorRoot.MinerMonitorService.RestartWindowsAsync(item);
                        }
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.ShutdownWindows = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定关闭选中的电脑吗？", title: "确认", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            MinerMonitorRoot.MinerMonitorService.ShutdownWindowsAsync(item);
                        }
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.StartMine = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定将选中的矿机开始挖矿吗？", title: "确认", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            // 不能直接调用item的StopMine命令，因为该命令内部会有弹窗确认
                            MinerMonitorRoot.MinerMonitorService.StartMineAsync(item, item.WorkId);
                        }
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.StopMine = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定将选中的矿机停止挖矿吗？", title: "确认", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            // 不能直接调用item的StopMine命令，因为该命令内部会有弹窗确认
                            MinerMonitorRoot.MinerMonitorService.StopMineAsync(item);
                        }
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.SelfMineWork = new DelegateCommand(() =>
            {
                MineWorkViewModel.SelfMineWork.Edit.Execute(FormType.Edit);
            }, IsSelectedOne);
            this.EnableRemoteDesktop = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定启用选中的矿机的Windows远程桌面功能吗？", title: "确认", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            MinerMonitorRoot.MinerMonitorService.EnableRemoteDesktopAsync(item);
                        }
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.RemoteDesktop = new DelegateCommand(() =>
            {
                #region
                string windowsLoginName = string.Empty;
                string windowsPassword = string.Empty;
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else if (SelectedMinerTweaks.Length == 1)
                {
                    var selectedVm = SelectedMinerTweaks[0];
                    windowsLoginName = selectedVm.WindowsLoginName;
                    windowsPassword = selectedVm.WindowsPassword;
                }
                VirtualRoot.Execute(new ShowRemoteDesktopLoginDialogCommand(new RemoteDesktopLoginViewModel(windowsLoginName)
                {
                    Title = "设置远程桌面",
                    Password = windowsPassword,
                    OnOk = vm =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            item.WindowsLoginName = vm.LoginName;
                            item.WindowsPassword = vm.Password;
                        }
                        VirtualRoot.Out.ShowSuccess("设置成功", toConsole: true);
                    }
                }));
                #endregion
            }, IsSelectedAny);
            this.BlockWAU = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else
                {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定禁用选中的矿机的Windows自动更新功能吗？", title: "确认", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            MinerMonitorRoot.MinerMonitorService.BlockWAUAsync(item);
                        }
                    }));
                }
                #endregion
            }, IsSelectedAny);
            this.PowerCfgOff = new DelegateCommand(() =>
            {
                VirtualRoot.Out.ShowSuccess("挖矿端启动时已自动关闭系统休眠", header: "提示", autoHideSeconds: 0);
            }, IsSelectedAny);
            this.VirtualMemory = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else if (SelectedMinerTweaks.Length == 1)
                {
                    VirtualRoot.Execute(new ShowMinerMonitorVirtualMemoryCommand(new VirtualMemoryViewModel(SelectedMinerTweaks[0])));
                }
                else
                {
                    string tail = "0. 这是统一设置虚拟内存，单选矿机时设置虚拟内存更直观；\n1. 尽量按照显存和虚拟内存一比一设置，比如6张6G显存的1060显卡应设置36G虚拟内存；\n2. 该操作优先将虚拟内存设置在操作系统所在的盘（通常是C盘，设置时会自动为系统盘留出1G空间，为非系统盘留出100M空间）;\n3. 只要整机剩余磁盘总和够大就能设置成功；\n4. 若统一设置虚拟内存不满足需求可以单选矿机分别设置。";
                    WpfUtil.ShowInputDialog("虚拟内存(Gb)", "0", tail, virtualMemoryGbText =>
                    {
                        if (!double.TryParse(virtualMemoryGbText, out double virtualMemoryGb))
                        {
                            return "数值格式错误，必须是数值（可带小数）";
                        }
                        return string.Empty;
                    }, virtualMemoryGbText =>
                    {
                        if (!double.TryParse(virtualMemoryGbText, out double virtualMemoryGb))
                        {
                            VirtualRoot.Out.ShowError("数值格式错误，必须是数值（可带小数）");
                        }
                        else
                        {
                            int virtualMemoryMb = Convert.ToInt32(virtualMemoryGb * LuckyKeyword.IntK);
                            Dictionary<string, int> data = new Dictionary<string, int>
                            {
                                ["Auto"] = virtualMemoryMb
                            };
                            foreach (var item in this.SelectedMinerTweaks)
                            {
                                MinerMonitorRoot.MinerMonitorService.SetVirtualMemoryAsync(item, data);
                            }
                        }
                    });
                }
                #endregion
            }, IsSelectedAny);
            this.LocalIpConfig = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else if (SelectedMinerTweaks.Length == 1)
                {
                    VirtualRoot.Execute(new ShowMinerMonitorLocalIpsCommand(new LocalIpConfigViewModel(SelectedMinerTweaks[0])));
                }
                #endregion
            }, IsSelectedOne);
            this.SwitchRadeonGpu = new DelegateCommand(() =>
            {
                #region
                var config = new DialogWindowViewModel(
                    isConfirmNo: true,
                    btnNoToolTip: "注意：关闭计算模式挖矿算力会减半",
                    message: $"过程大概需要花费5到10秒钟，最好矿机没有处在挖矿中否则内核会重启。", title: "A卡计算模式", onYes: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            MinerMonitorRoot.MinerMonitorService.SwitchRadeonGpuAsync(item, on: true);
                        }
                    }, onNo: () =>
                    {
                        foreach (var item in SelectedMinerTweaks)
                        {
                            MinerMonitorRoot.MinerMonitorService.SwitchRadeonGpuAsync(item, on: false);
                        }
                        return true;
                    }, btnYesText: "开启计算模式", btnNoText: "关闭计算模式");
                this.ShowSoftDialog(config);
                #endregion
            }, IsSelectedAny);
            this.CopyMainCoinWallet = new DelegateCommand(() =>
            {
                #region
                if (SelectedMinerTweaks.Length == 0)
                {
                    ShowNoRecordSelected();
                }
                else if (SelectedMinerTweaks.Length == 1)
                {
                    string wallet = SelectedMinerTweaks[0].MainCoinWallet ?? "无";
                    Clipboard.SetDataObject(wallet, true);
                    VirtualRoot.Out.ShowSuccess(wallet, header: "复制成功");
                }
                #endregion
            }, IsSelectedOne);
            this.PageUp = new DelegateCommand(() =>
            {
                this.PageIndex -= 1;
            });
            this.PageDown = new DelegateCommand(() =>
            {
                this.PageIndex += 1;
            });
            this.PageFirst = new DelegateCommand(() =>
            {
                this.PageIndex = 1;
            });
            this.PageLast = new DelegateCommand(() =>
            {
                this.PageIndex = PageCount;
            });
            this.PageRefresh = new DelegateCommand(() =>
            {
                QueryMinerTweaks();
            });
            this._lastSortDirection = new Dictionary<ClientDataSortField, SortDirection>(_sortDirection);
            #region SortBy
            this.SortByMinerName = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.MinerName)
                {
                    this.SortField = ClientDataSortField.MinerName;
                }
                else
                {
                    if (MinerNameSortDirection == SortDirection.Ascending)
                    {
                        MinerNameSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        MinerNameSortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByMainCoinSpeed = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.MainCoinSpeed)
                {
                    this.SortField = ClientDataSortField.MainCoinSpeed;
                }
                else
                {
                    if (MainCoinSpeedSortDirection == SortDirection.Ascending)
                    {
                        MainCoinSpeedSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        MainCoinSpeedSortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByMainCoinRejectPercent = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.MainCoinRejectPercent)
                {
                    this.SortField = ClientDataSortField.MainCoinRejectPercent;
                }
                else
                {
                    if (MainCoinRejectPercentSortDirection == SortDirection.Ascending)
                    {
                        MainCoinRejectPercentSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        MainCoinRejectPercentSortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByDualCoinRejectPercent = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.DualCoinRejectPercent)
                {
                    this.SortField = ClientDataSortField.DualCoinRejectPercent;
                }
                else
                {
                    if (DualCoinRejectPercentSortDirection == SortDirection.Ascending)
                    {
                        DualCoinRejectPercentSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        DualCoinRejectPercentSortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByMainCoinPoolDelay = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.MainCoinPoolDelay)
                {
                    this.SortField = ClientDataSortField.MainCoinPoolDelay;
                }
                else
                {
                    if (MainCoinPoolDelaySortDirection == SortDirection.Ascending)
                    {
                        MainCoinPoolDelaySortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        MainCoinPoolDelaySortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByDualCoinPoolDelay = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.DualCoinPoolDelay)
                {
                    this.SortField = ClientDataSortField.DualCoinPoolDelay;
                }
                else
                {
                    if (DualCoinPoolDelaySortDirection == SortDirection.Ascending)
                    {
                        DualCoinPoolDelaySortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        DualCoinPoolDelaySortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByCpuTemperature = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.CpuTemperature)
                {
                    this.SortField = ClientDataSortField.CpuTemperature;
                }
                else
                {
                    if (CpuTemperatureSortDirection == SortDirection.Ascending)
                    {
                        CpuTemperatureSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        CpuTemperatureSortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByKernelSelfRestartCount = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.KernelSelfRestartCount)
                {
                    this.SortField = ClientDataSortField.KernelSelfRestartCount;
                }
                else
                {
                    if (KernelSelfRestartCountSortDirection == SortDirection.Ascending)
                    {
                        KernelSelfRestartCountSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        KernelSelfRestartCountSortDirection = SortDirection.Ascending;
                    }
                }
            });
            this.SortByDiskSpace = new DelegateCommand(() =>
            {
                if (this.SortField != ClientDataSortField.DiskSpace)
                {
                    this.SortField = ClientDataSortField.DiskSpace;
                }
                else
                {
                    if (DiskSpaceSortDirection == SortDirection.Ascending)
                    {
                        DiskSpaceSortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        DiskSpaceSortDirection = SortDirection.Ascending;
                    }
                }
            });
            #endregion
            this.WsRetry = new DelegateCommand(() =>
            {
                if (!RpcRoot.IsOuterNet || MinerMonitorRoot.WsClient.IsOpen)
                {
                    IsWsOnline = true;
                    return;
                }
                MinerMonitorRoot.WsClient.OpenOrCloseWs(isResetFailCount: true);
                IsConnecting = true;
            });
            this.SwitchService = new DelegateCommand(() =>
            {
                if (RpcRoot.IsInnerNet)
                {
                    MinerMonitorRoot.Login(() =>
                    {
                        MinerMonitorRoot.WsClient.OpenOrCloseWs(isResetFailCount: true);
                        RpcRoot.SetIsOuterNet(true);
                    }, RpcRoot.OfficialServerAddress);
                }
                else
                {
                    RpcRoot.SetIsOuterNet(false);
                }
            });
            this.EmptyCommandForSelectdOne = new DelegateCommand(() => { }, IsSelectedOne);

            this.EmptyCommandForSelectedAny = new DelegateCommand(() => { }, IsSelectedAny);

            VirtualRoot.BuildEventPath<MinerMonitorServiceSwitchedEvent>("切换了群控后台客户端服务类型后刷新矿机列表", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
            {
                this.OnPropertyChanged(nameof(NetTypeToolTip));
                this.OnPropertyChanged(nameof(NetTypeText));
                if (message.ServiceType == MinerMonitorServiceType.Out)
                {
                    if (!MinerMonitorRoot.WsClient.IsOpen)
                    {
                        // 等它1秒
                        1.SecondsDelay().ContinueWith(t => this.QueryMinerTweaks());
                    }
                    else
                    {
                        this.QueryMinerTweaks();
                    }
                }
                else
                {
                    this.QueryMinerTweaks();
                }
            });
            VirtualRoot.BuildCmdPath<UpdateMinerTweakVmCommand>(this.GetType(), LogEnum.DevConsole, path: message =>
            {
                var vm = _minerClients.FirstOrDefault(a => a.Id == message.ClientData.Id);
                if (vm != null)
                {
                    vm.Update(message.ClientData);
                }
            });
            VirtualRoot.BuildCmdPath<RefreshWsStateCommand>(this.GetType(), LogEnum.DevConsole, message =>
            {
                #region
                if (message.WsClientState != null)
                {
                    this.WsServerIp = message.WsClientState.WsServerIp;
                    this.IsWsOnline = message.WsClientState.Status == WsClientStatus.Open;
                    if (message.WsClientState.ToOut)
                    {
                        VirtualRoot.Out.ShowWarn(message.WsClientState.Description, autoHideSeconds: 3);
                    }
                    if (!message.WsClientState.ToOut || !this.IsWsOnline)
                    {
                        this.WsDescription = message.WsClientState.Description;
                    }
                    if (!this.IsWsOnline)
                    {
                        if (message.WsClientState.LastTryOn != DateTime.MinValue)
                        {
                            this.WsLastTryOn = message.WsClientState.LastTryOn;
                        }
                        if (message.WsClientState.NextTrySecondsDelay > 0)
                        {
                            WsNextTrySecondsDelay = message.WsClientState.NextTrySecondsDelay;
                        }
                    }
                }
                #endregion
            });
            if (RpcRoot.IsOuterNet)
            {
                VirtualRoot.Execute(new RefreshWsStateCommand(MinerMonitorRoot.WsClient.GetState()));
            }
            VirtualRoot.BuildEventPath<Per1SecondEvent>("外网群控重试秒表倒计时", LogEnum.None, this.GetType(), PathPriority.Normal, path: message =>
            {
                if (!IsWsOnline)
                {
                    if (WsNextTrySecondsDelay > 0)
                    {
                        WsNextTrySecondsDelay--;
                    }
                    OnPropertyChanged(nameof(WsLastTryOnText));
                }
            });
        }
        #endregion

        private bool IsSelectedAny()
        {
            return this.SelectedMinerTweaks != null && this.SelectedMinerTweaks.Length != 0;
        }

        private bool IsSelectedOne()
        {
            return this.SelectedMinerTweaks != null && this.SelectedMinerTweaks.Length == 1;
        }

        public string NetTypeToolTip
        {
            get
            {
                if (RpcRoot.IsOuterNet)
                {
                    return "点击切换为内网群控";
                }
                return "点击切换为外网群控";
            }
        }

        #region IWsStateViewModel的成员

        // 由守护进程根据外网群控是否正常更新
        public bool IsWsOnline
        {
            get => _isWsOnline;
            set
            {
                if (_isWsOnline != value)
                {
                    _isWsOnline = value;
                    OnPropertyChanged(nameof(IsWsOnline));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                }
            }
        }

        public string WsDescription
        {
            get
            {
                if (string.IsNullOrEmpty(RpcRoot.RpcUser.LoginName))
                {
                    return $"未登录 {WsServerIp}";
                }
                if (string.IsNullOrEmpty(_wsDescription))
                {
                    return $"{WsStateText} {WsServerIp}";
                }
                return $"{_wsDescription} {WsServerIp}";
            }
            set
            {
                if (_wsDescription != value)
                {
                    _wsDescription = value;
                    OnPropertyChanged(nameof(WsDescription));
                }
            }
        }

        public int WsNextTrySecondsDelay
        {
            get
            {
                if (_wsNextTrySecondsDelay < 0)
                {
                    return 0;
                }
                return _wsNextTrySecondsDelay;
            }
            set
            {
                if (_wsNextTrySecondsDelay != value)
                {
                    _wsNextTrySecondsDelay = value;
                    OnPropertyChanged(nameof(WsNextTrySecondsDelay));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayText));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                    IsConnecting = value <= 0;
                }
            }
        }

        public DateTime WsLastTryOn
        {
            get => _wsLastTryOn;
            set
            {
                if (_wsLastTryOn != value)
                {
                    _wsLastTryOn = value;
                    OnPropertyChanged(nameof(WsLastTryOn));
                    OnPropertyChanged(nameof(WsLastTryOnText));
                }
            }
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if (_isConnecting != value)
                {
                    _isConnecting = value;
                    OnPropertyChanged(nameof(IsConnecting));
                    OnPropertyChanged(nameof(WsRetryText));
                    if (value)
                    {
                        VirtualRoot.SetInterval(TimeSpan.FromMilliseconds(100), perCallback: () =>
                        {
                            WsRetryIconAngle += 40;
                        }, stopCallback: () =>
                        {
                            WsRetryIconAngle = 0;
                            IsConnecting = false;
                        }, timeout: TimeSpan.FromSeconds(10), requestStop: () =>
                        {
                            return !IsConnecting;
                        });
                    }
                }
            }
        }

        #endregion

        public MinerMonitorRoot.MinerTweakConsoleViewModel MinerTweakConsoleVm
        {
            get
            {
                return MinerMonitorRoot.MinerTweakConsoleVm;
            }
        }

        public double WsRetryIconAngle
        {
            get { return _wsRetryIconAngle; }
            set
            {
                _wsRetryIconAngle = value;
                OnPropertyChanged(nameof(WsRetryIconAngle));
            }
        }

        public string WsRetryText
        {
            get
            {
                if (IsConnecting)
                {
                    return "重试中";
                }
                return "立即重试";
            }
        }

        public string WsLastTryOnText
        {
            get
            {
                if (IsWsOnline || WsLastTryOn == DateTime.MinValue)
                {
                    return string.Empty;
                }
                return Timestamp.GetTimeSpanBeforeText(WsLastTryOn);
            }
        }

        public string WsNextTrySecondsDelayText
        {
            get
            {
                int seconds = WsNextTrySecondsDelay;
                if (IsWsOnline)
                {
                    return string.Empty;
                }
                return Timestamp.GetTimeSpanAfterText(seconds);
            }
        }

        public Visibility WsNextTrySecondsDelayVisible
        {
            get
            {
                if (!RpcRoot.IsOuterNet)
                {
                    return Visibility.Collapsed;
                }
                if (IsWsOnline)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public string WsStateText
        {
            get
            {
                if (IsWsOnline)
                {
                    return "连接服务器成功";
                }
                return "离线";
            }
        }

        public string WsServerIp
        {
            get => _wsServerIp;
            set
            {
                _wsServerIp = value;
                OnPropertyChanged(nameof(WsServerIp));
            }
        }

        public MainMenuViewModel MainMenu
        {
            get
            {
                return MainMenuViewModel.Instance;
            }
        }

        public string NetTypeText
        {
            get
            {
                if (RpcRoot.IsOuterNet)
                {
                    return "外网群控";
                }
                return "内网群控";
            }
        }

        public int FrozenColumnCount
        {
            get => _frozenColumnCount;
            set
            {
                if (value >= 2)
                {
                    _frozenColumnCount = value;
                    OnPropertyChanged(nameof(FrozenColumnCount));
                    VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                    {
                        Key = LuckyKeyword.FrozenColumnCountAppSettingKey,
                        Value = value
                    }));
                }
            }
        }

        public List<int> FrozenColumns { get; } = new List<int> { 5, 4, 3 };

        public int RejectPercent
        {
            get => _rejectPercent;
            set
            {
                _rejectPercent = value;
                OnPropertyChanged(nameof(RejectPercent));
                RefreshRejectPercentForeground(this.MinerTweaks.ToArray());
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                {
                    Key = LuckyKeyword.RejectPercentAppSettingKey,
                    Value = value
                }));
            }
        }

        private void RefreshRejectPercentForeground(MinerViewModel[] vms)
        {
            foreach (MinerViewModel item in vms)
            {
                if (item.MainCoinRejectPercent >= this.RejectPercent)
                {
                    item.MainCoinRejectPercentForeground = WpfUtil.RedBrush;
                }
                else
                {
                    item.MainCoinRejectPercentForeground = WpfUtil.BlackBrush;
                }

                if (item.DualCoinRejectPercent >= this.RejectPercent)
                {
                    item.DualCoinRejectPercentForeground = WpfUtil.RedBrush;
                }
                else
                {
                    item.DualCoinRejectPercentForeground = WpfUtil.BlackBrush;
                }
            }
        }

        public uint MaxTemp
        {
            get => _maxTemp;
            set
            {
                if (value > this.MinTemp && value != _maxTemp)
                {
                    _maxTemp = value;
                    OnPropertyChanged(nameof(MaxTemp));
                    RefreshMaxTempForeground(this.MinerTweaks.ToArray());
                    VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                    {
                        Key = LuckyKeyword.MaxTempAppSettingKey,
                        Value = value
                    }));
                }
            }
        }

        public uint MinTemp
        {
            get => _minTemp;
            set
            {
                if (value < this.MaxTemp && value != _minTemp)
                {
                    _minTemp = value;
                    OnPropertyChanged(nameof(MinTemp));
                    RefreshMaxTempForeground(this.MinerTweaks.ToArray());
                    VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                    {
                        Key = LuckyKeyword.MinTempAppSettingKey,
                        Value = value
                    }));
                }
            }
        }

        private void RefreshMaxTempForeground(MinerViewModel[] vms)
        {
            foreach (MinerViewModel item in vms)
            {
                if (item.GpuTableVm == null)
                {
                    continue;
                }
                if (item.GpuTableVm.MaxTemp >= this.MaxTemp)
                {
                    item.GpuTableVm.TempForeground = WpfUtil.RedBrush;
                }
                else if (item.GpuTableVm.MaxTemp < this.MinTemp)
                {
                    item.GpuTableVm.TempForeground = WpfUtil.BlueBrush;
                }
                else
                {
                    item.GpuTableVm.TempForeground = WpfUtil.BlackBrush;
                }
                item.RefreshGpusForeground(this.MinTemp, this.MaxTemp);
            }
        }

        private void ShowNoRecordSelected()
        {
            VirtualRoot.Out.ShowWarn("没有选中记录", autoHideSeconds: 2);
        }

        public ColumnsShowViewModel ColumnsShow
        {
            get
            {
                return _columnsShow;
            }
            set
            {
                if (_columnsShow != value && value != null)
                {
                    if (_columnsShow != null)
                    {
                        VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                        {
                            Key = LuckyKeyword.ColumnsShowIdAppSettingKey,
                            Value = value.Id
                        }));
                    }
                    _columnsShow = value;
                    OnPropertyChanged(nameof(ColumnsShow));
                }
            }
        }

        public MinerMonitorRoot.ColumnsShowViewModels ColumnsShows
        {
            get
            {
                return MinerMonitorRoot.ColumnsShowVms;
            }
        }

        public int CountDown
        {
            get { return _countDown; }
            private set
            {
                _countDown = value;
                OnPropertyChanged(nameof(CountDown));
            }
        }

        public void DownCountDown()
        {
            this.CountDown -= 1;
            if (this.CountDown == 0)
            {
                this.IsLoading = false;
            }
        }

        public void ResetCountDown()
        {
            this.CountDown = 10;
        }

        private static readonly List<int> _pageSizeItems = new List<int>() { 10, 20, 30, 40 };
        public List<int> PageSizeItems
        {
            get
            {
                return _pageSizeItems;
            }
        }

        public bool IsPageUpEnabled
        {
            get
            {
                if (this.PageIndex <= 1)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsPageDownEnabled
        {
            get
            {
                if (this.PageIndex >= this.PageCount)
                {
                    return false;
                }
                return true;
            }
        }

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                // 注意PageIndex任何时候都应刷新而不是不等时才刷新
                _pageIndex = value;
                OnPropertyChanged(nameof(PageIndex));
                QueryMinerTweaks();
            }
        }

        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)this.Total / this.PageSize);
            }
        }

        public int PageSize
        {
            get
            {
                if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting("MinerList" + nameof(PageSize), out IAppSetting appSetting) && appSetting.Value != null)
                {
                    if (int.TryParse(appSetting.Value.ToString(), out int value))
                    {
                        return value;
                    }
                }
                return 20;
            }
            set
            {
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
                {
                    Key = "MinerList" + nameof(PageSize),
                    Value = value.ToString()
                }));
                OnPropertyChanged(nameof(PageSize));
                this.PageIndex = 1;
            }
        }

        public SortDirection MinerNameSortDirection
        {
            get { return _sortDirection[ClientDataSortField.MinerName]; }
            set
            {
                if (_sortDirection[ClientDataSortField.MinerName] != value)
                {
                    _sortDirection[ClientDataSortField.MinerName] = value;
                    OnPropertyChanged(nameof(MinerNameSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection MainCoinSpeedSortDirection
        {
            get { return _sortDirection[ClientDataSortField.MainCoinSpeed]; }
            set
            {
                if (_sortDirection[ClientDataSortField.MainCoinSpeed] != value)
                {
                    _sortDirection[ClientDataSortField.MainCoinSpeed] = value;
                    OnPropertyChanged(nameof(MainCoinSpeedSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection MainCoinRejectPercentSortDirection
        {
            get { return _sortDirection[ClientDataSortField.MainCoinRejectPercent]; }
            set
            {
                if (_sortDirection[ClientDataSortField.MainCoinRejectPercent] != value)
                {
                    _sortDirection[ClientDataSortField.MainCoinRejectPercent] = value;
                    OnPropertyChanged(nameof(MainCoinRejectPercentSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection DualCoinRejectPercentSortDirection
        {
            get { return _sortDirection[ClientDataSortField.DualCoinRejectPercent]; }
            set
            {
                if (_sortDirection[ClientDataSortField.DualCoinRejectPercent] != value)
                {
                    _sortDirection[ClientDataSortField.DualCoinRejectPercent] = value;
                    OnPropertyChanged(nameof(DualCoinRejectPercentSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection MainCoinPoolDelaySortDirection
        {
            get { return _sortDirection[ClientDataSortField.MainCoinPoolDelay]; }
            set
            {
                if (_sortDirection[ClientDataSortField.MainCoinPoolDelay] != value)
                {
                    _sortDirection[ClientDataSortField.MainCoinPoolDelay] = value;
                    OnPropertyChanged(nameof(MainCoinPoolDelaySortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection DualCoinPoolDelaySortDirection
        {
            get { return _sortDirection[ClientDataSortField.DualCoinPoolDelay]; }
            set
            {
                if (_sortDirection[ClientDataSortField.DualCoinPoolDelay] != value)
                {
                    _sortDirection[ClientDataSortField.DualCoinPoolDelay] = value;
                    OnPropertyChanged(nameof(DualCoinPoolDelaySortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection CpuTemperatureSortDirection
        {
            get { return _sortDirection[ClientDataSortField.CpuTemperature]; }
            set
            {
                if (_sortDirection[ClientDataSortField.CpuTemperature] != value)
                {
                    _sortDirection[ClientDataSortField.CpuTemperature] = value;
                    OnPropertyChanged(nameof(CpuTemperatureSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection KernelSelfRestartCountSortDirection
        {
            get { return _sortDirection[ClientDataSortField.KernelSelfRestartCount]; }
            set
            {
                if (_sortDirection[ClientDataSortField.KernelSelfRestartCount] != value)
                {
                    _sortDirection[ClientDataSortField.KernelSelfRestartCount] = value;
                    OnPropertyChanged(nameof(KernelSelfRestartCountSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public SortDirection DiskSpaceSortDirection
        {
            get { return _sortDirection[ClientDataSortField.DiskSpace]; }
            set
            {
                if (_sortDirection[ClientDataSortField.DiskSpace] != value)
                {
                    _sortDirection[ClientDataSortField.DiskSpace] = value;
                    OnPropertyChanged(nameof(DiskSpaceSortDirection));
                    this.PageIndex = 1;
                }
            }
        }

        public ClientDataSortField SortField
        {
            get { return _sortField; }
            set
            {
                if (_sortField != value)
                {
                    _sortField = value;
                    OnPropertyChanged(nameof(SortField));
                    OnPropertyChanged(nameof(IsSortByMinerName));
                    OnPropertyChanged(nameof(IsSortByMainCoinSpeed));
                    OnPropertyChanged(nameof(IsSortByMainCoinRejectPercent));
                    OnPropertyChanged(nameof(IsSortByDualCoinRejectPercent));
                    OnPropertyChanged(nameof(IsSortByMainCoinPoolDelay));
                    OnPropertyChanged(nameof(IsSortByDualCoinPoolDelay));
                    OnPropertyChanged(nameof(IsSortByCpuTemperature));
                    OnPropertyChanged(nameof(IsSortByKernelSelfRestartCount));
                    OnPropertyChanged(nameof(IsSortByDiskSpace));
                    this.PageIndex = 1;
                }
            }
        }

        public bool IsSortByMinerName
        {
            get
            {
                return SortField == ClientDataSortField.MinerName;
            }
        }

        public bool IsSortByMainCoinSpeed
        {
            get
            {
                return SortField == ClientDataSortField.MainCoinSpeed;
            }
        }

        public bool IsSortByMainCoinRejectPercent
        {
            get
            {
                return SortField == ClientDataSortField.MainCoinRejectPercent;
            }
        }

        public bool IsSortByDualCoinRejectPercent
        {
            get
            {
                return SortField == ClientDataSortField.DualCoinRejectPercent;
            }
        }

        public bool IsSortByMainCoinPoolDelay
        {
            get
            {
                return SortField == ClientDataSortField.MainCoinPoolDelay;
            }
        }

        public bool IsSortByDualCoinPoolDelay
        {
            get
            {
                return SortField == ClientDataSortField.DualCoinPoolDelay;
            }
        }

        public bool IsSortByCpuTemperature
        {
            get
            {
                return SortField == ClientDataSortField.CpuTemperature;
            }
        }

        public bool IsSortByKernelSelfRestartCount
        {
            get
            {
                return SortField == ClientDataSortField.KernelSelfRestartCount;
            }
        }

        public bool IsSortByDiskSpace
        {
            get
            {
                return SortField == ClientDataSortField.DiskSpace;
            }
        }

        public int Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        private void RefreshPagingUi(int total)
        {
            _total = total;
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(PageCount));
            OnPropertyChanged(nameof(IsPageDownEnabled));
            OnPropertyChanged(nameof(IsPageUpEnabled));
            if (Total == 0)
            {
                _pageIndex = 0;
                OnPropertyChanged(nameof(PageIndex));
            }
            else if (PageIndex == 0)
            {
                _pageIndex = 1;
                OnPropertyChanged(nameof(PageIndex));
            }
        }

        public void RefreshMinerTweaksSelectedMinerGroup(MinerViewModel[] vms)
        {
            foreach (var minerClient in vms)
            {
                minerClient.OnPropertyChanged(nameof(minerClient.SelectedMinerGroup));
            }
        }

        public void RefreshMinerTweaksSelectedMineWork(MinerViewModel[] vms)
        {
            foreach (var minerClient in vms)
            {
                minerClient.OnPropertyChanged(nameof(minerClient.SelectedMineWork));
            }
        }

        public ObservableCollection<MinerViewModel> MinerTweaks
        {
            get
            {
                return _minerClients;
            }
        }

        public Visibility IsNoRecordVisible
        {
            get
            {
                if (this.IsLoading)
                {
                    return Visibility.Collapsed;
                }
                if (_minerClients.Count == 0)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public MinerViewModel CurrentMinerTweak
        {
            get { return _currentMinerTweak; }
            set
            {
                _currentMinerTweak = value;
                OnPropertyChanged(nameof(CurrentMinerTweak));
                VirtualRoot.RaiseEvent(new MinerTweakSelectionChangedEvent(value));
            }
        }

        public MinerViewModel[] SelectedMinerTweaks
        {
            get { return _selectedMinerTweaks; }
            set
            {
                _selectedMinerTweaks = value;
                OnPropertyChanged(nameof(SelectedMinerTweaks));
            }
        }

        public List<CoinSnapshotViewModel> CoinSnapshotVms
        {
            get
            {
                return _coinSnapshotVms;
            }
        }

        public CoinSnapshotViewModel CoinSnapshotVm
        {
            get { return _coinSnapshotVm; }
            set
            {
                if (_coinSnapshotVm != value)
                {
                    _coinSnapshotVm = value;
                    OnPropertyChanged(nameof(CoinSnapshotVm));
                    this._pool = string.Empty;
                    this._poolVm = PoolViewModel.PleaseSelect;
                    OnPropertyChanged(nameof(PoolVm));
                    OnPropertyChanged(nameof(IsMainCoinSelected));
                    this.PageIndex = 1;
                }
            }
        }

        private IEnumerable<CoinViewModel> GetDualCoinVmItems()
        {
            yield return CoinViewModel.PleaseSelect;
            yield return CoinViewModel.DualCoinEnabled;
            foreach (var item in AppRoot.CoinVms.AllCoins)
            {
                yield return item;
            }
        }
        public List<CoinViewModel> DualCoinVmItems
        {
            get
            {
                return GetDualCoinVmItems().ToList();
            }
        }

        public bool IsMainCoinSelected
        {
            get
            {
                if (CoinSnapshotVm == CoinSnapshotViewModel.PleaseSelect)
                {
                    return false;
                }
                return true;
            }
        }

        public string Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                OnPropertyChanged(nameof(Pool));
                this.PageIndex = 1;
            }
        }

        public PoolViewModel PoolVm
        {
            get => _poolVm;
            set
            {
                if (_poolVm != value)
                {
                    _poolVm = value;
                    if (value == null)
                    {
                        Pool = string.Empty;
                    }
                    else
                    {
                        Pool = value.Server;
                    }
                    OnPropertyChanged(nameof(PoolVm));
                }
            }
        }

        public string Wallet
        {
            get => _wallet;
            set
            {
                if (_wallet != value)
                {
                    _wallet = value;
                    OnPropertyChanged(nameof(Wallet));
                    this.PageIndex = 1;
                }
            }
        }

        public string MinerIp
        {
            get => _minerIp;
            set
            {
                if (_minerIp != value)
                {
                    _minerIp = value;
                    OnPropertyChanged(nameof(MinerIp));
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!IPAddress.TryParse(value, out IPAddress _))
                        {
                            throw new ValidationException("IP地址格式不正确");
                        }
                    }
                    this.PageIndex = 1;
                }
            }
        }
        public string MinerName
        {
            get => _minerName;
            set
            {
                if (_minerName != value)
                {
                    _minerName = value;
                    OnPropertyChanged(nameof(MinerName));
                    this.PageIndex = 1;
                }
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                if (_version != value)
                {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    this.PageIndex = 1;
                }
            }
        }

        public string Kernel
        {
            get => _kernel;
            set
            {
                if (_kernel != value)
                {
                    _kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                    this.PageIndex = 1;
                }
            }
        }

        public MinerMonitorRoot.MineWorkViewModels MineWorkVms
        {
            get
            {
                return MinerMonitorRoot.MineWorkVms;
            }
        }

        public MinerMonitorRoot.MinerGroupViewModels MinerGroupVms
        {
            get
            {
                return MinerMonitorRoot.MinerGroupVms;
            }
        }

        public MineWorkViewModel SelectedMineWork
        {
            get => _selectedMineWork;
            set
            {
                _selectedMineWork = value;
                OnPropertyChanged(nameof(SelectedMineWork));
                OnPropertyChanged(nameof(IsMineWorkSelected));
                this.PageIndex = 1;
            }
        }

        public bool IsMineWorkSelected
        {
            get
            {
                if (SelectedMineWork != MineWorkViewModel.PleaseSelect)
                {
                    return true;
                }
                return false;
            }
        }

        public MinerGroupViewModel SelectedMinerGroup
        {
            get => _selectedMinerGroup;
            set
            {
                _selectedMinerGroup = value;
                OnPropertyChanged(nameof(SelectedMinerGroup));
                this.PageIndex = 1;
            }
        }

        public EnumItem<MineStatus> MineStatusEnumItem
        {
            get => _mineStatusEnumItem;
            set
            {
                if (_mineStatusEnumItem != value)
                {
                    _mineStatusEnumItem = value;
                    OnPropertyChanged(nameof(MineStatusEnumItem));
                    this.PageIndex = 1;
                }
            }
        }

        public GpuType GpuType
        {
            get => _gpuType;
            set
            {
                if (_gpuType != value)
                {
                    _gpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                    OnPropertyChanged(nameof(IsNvidiaIconVisible));
                    OnPropertyChanged(nameof(IsAmdIconVisible));
                    this.PageIndex = 1;
                }
            }
        }

        public Visibility IsNvidiaIconVisible
        {
            get
            {
                if (GpuType == GpuType.NVIDIA)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAmdIconVisible
        {
            get
            {
                if (GpuType == GpuType.AMD)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public EnumItem<GpuType> GpuTypeEnumItem
        {
            get
            {
                return Enums.GpuTypeEnumItems.FirstOrDefault(a => a.Value == GpuType);
            }
            set
            {
                if (GpuType != value.Value)
                {
                    GpuType = value.Value;
                    OnPropertyChanged(nameof(GpuTypeEnumItem));
                }
            }
        }

        public string GpuName
        {
            get { return _gpuName; }
            set
            {
                if (_gpuName != value)
                {
                    _gpuName = value;
                    OnPropertyChanged(nameof(GpuName));
                    this.PageIndex = 1;
                }
            }
        }

        public string GpuDriver
        {
            get { return _gpuDriver; }
            set
            {
                if (_gpuDriver != value)
                {
                    _gpuDriver = value;
                    OnPropertyChanged(nameof(GpuDriver));
                    this.PageIndex = 1;
                }
            }
        }

        public bool IsInnerNet
        {
            get => RpcRoot.IsInnerNet;
        }
    }
}
