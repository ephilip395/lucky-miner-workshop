using Lucky.MinerMonitor.Vms;
using Lucky.Vms;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lucky.MinerMonitor {
    public static partial class MinerMonitorRoot {
        public class MinerTweakMessagesViewModel : ViewModelBase {
            private readonly ObservableCollection<LocalMessageDtoViewModel> _vms = new ObservableCollection<LocalMessageDtoViewModel>();
            private readonly object _locker = new object();
            private MinerTweakViewModel _minerClientVm;

            private static bool _called = false;

            public MinerTweakMessagesViewModel() {
                if (_called) {
                    throw new InvalidProgramException("只能调用一次");
                }
                _called = true;
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                if (ClientAppType.IsMinerMonitor) {
                    VirtualRoot.BuildEventPath<MinerTweakSelectionChangedEvent>("刷新矿机消息列表", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                        bool isChanged = true;
                        if (message.MinerTweakVm != null && this._minerClientVm != null && this._minerClientVm.ClientId == message.MinerTweakVm.ClientId) {
                            isChanged = false;
                        }
                        if (isChanged) {
                            lock (_locker) {
                                _vms.Clear();
                                this._minerClientVm = message.MinerTweakVm;
                                OnPropertyChanged(nameof(IsNoRecord));
                            }
                            SendGetLocalMessagesMqMessage(isFast: true);
                        }
                    });
                    VirtualRoot.BuildEventPath<ClientLocalMessagesEvent>("将收到的挖矿端本地消息展示到消息列表", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                        path: message => {
                            if (this._minerClientVm == null || this._minerClientVm.ClientId != message.ClientId) {
                                return;
                            }
                            if (message.Data == null || message.Data.Count == 0) {
                                return;
                            }
                            UIThread.Execute(() => {
                                foreach (var item in message.Data) {
                                    _vms.Insert(0, new LocalMessageDtoViewModel(item));
                                }
                                OnPropertyChanged(nameof(IsNoRecord));
                            });
                        });
                    VirtualRoot.BuildEventPath<Per5SecondEvent>("周期获取当前选中的那台矿机的本地消息", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                        SendGetLocalMessagesMqMessage(isFast: false);
                    });
                }
            }

            public ObservableCollection<LocalMessageDtoViewModel> ClientLocalMessages {
                get {
                    return _vms;
                }
            }

            public bool IsNoRecord {
                get {
                    return _vms.Count == 0;
                }
            }

            private DateTime _preSendMqMessageOn = DateTime.MinValue;
            private MinerTweakViewModel _preMinerTweakVm;
            public void SendGetLocalMessagesMqMessage(bool isFast) {
                if (this._minerClientVm == null || !IsMinerTweakMessagesVisible) {
                    return;
                }
                foreach (var vm in _vms) {
                    vm.OnPropertyChanged(nameof(LocalMessageDtoViewModel.TimestampText));
                }
                if (_preSendMqMessageOn.AddSeconds(4) > DateTime.Now && _preMinerTweakVm == _minerClientVm) {
                    return;
                }
                _preSendMqMessageOn = DateTime.Now;
                _preMinerTweakVm = _minerClientVm;
                long afterTime = 0;
                var minerClientVm = this._minerClientVm;
                lock (_locker) {
                    var item = _vms.FirstOrDefault();
                    if (item != null) {
                        afterTime = item.Timestamp;
                    }
                }
                if (isFast) {
                    MinerMonitorService.FastGetLocalMessagesAsync(minerClientVm, afterTime);
                }
                else {
                    MinerMonitorService.GetLocalMessagesAsync(minerClientVm, afterTime);
                }
            }
        }
    }
}
