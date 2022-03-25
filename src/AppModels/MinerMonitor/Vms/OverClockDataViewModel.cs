﻿using Lucky.Core.MinerServer;
using Lucky.Gpus;
using Lucky.Vms;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms {
    public class OverClockDataViewModel : ViewModelBase, IOverClockData, IEditableViewModel {
        private Guid _id;
        private Guid _coinId;
        private string _name;
        private GpuType _gpuType;
        private int _coreClockDelta;
        private int _memoryClockDelta;
        private int _powerCapacity;
        private int _cool;
        private int _tempLimit;
        private bool _isAutoFanSpeed;
        private int _coreVoltage;
        private int _memoryVoltage;
        private int _currentMemoryTimingLevel;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public OverClockDataViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public OverClockDataViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (LuckyContext.Instance.OverClockDataSet.TryGetOverClockData(this.Id, out IOverClockData group)) {
                    VirtualRoot.Execute(new UpdateOverClockDataCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddOverClockDataCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommandTpl<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditOverClockDataCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveOverClockDataCommand(this.Id));
                }));
            });
        }

        public OverClockDataViewModel(IOverClockData data) : this(data.GetId()) {
            _coinId = data.CoinId;
            _name = data.Name;
            _gpuType = data.GpuType;
            _coreClockDelta = data.CoreClockDelta;
            _memoryClockDelta = data.MemoryClockDelta;
            _powerCapacity = data.PowerCapacity;
            _tempLimit = data.TempLimit;
            _cool = data.Cool;
            _coreVoltage = data.CoreVoltage;
            _memoryVoltage = data.MemoryVoltage;
            _currentMemoryTimingLevel = data.CurrentMemoryTimingLevel;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Guid CoinId {
            get => _coinId;
            set {
                _coinId = value;
                OnPropertyChanged(nameof(CoinId));
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public GpuType GpuType {
            get => _gpuType;
            set {
                _gpuType = value;
                OnPropertyChanged(nameof(GpuType));
                OnPropertyChanged(nameof(IsNvidiaIconVisible));
                OnPropertyChanged(nameof(IsAmdIconVisible));
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                if (GpuType == GpuType.NVIDIA) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAmdIconVisible {
            get {
                if (GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public EnumItem<GpuType> GpuTypeEnumItem {
            get {
                return Enums.GpuTypeEnumItems.FirstOrDefault(a => a.Value == GpuType);
            }
            set {
                if (GpuType != value.Value) {
                    GpuType = value.Value;
                    OnPropertyChanged(nameof(GpuTypeEnumItem));
                }
            }
        }

        public int CoreClockDelta {
            get => _coreClockDelta;
            set {
                _coreClockDelta = value;
                OnPropertyChanged(nameof(CoreClockDelta));
                OnPropertyChanged(nameof(Tooltip));
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                _memoryClockDelta = value;
                OnPropertyChanged(nameof(MemoryClockDelta));
                OnPropertyChanged(nameof(Tooltip));
            }
        }

        public int PowerCapacity {
            get => _powerCapacity;
            set {
                _powerCapacity = value;
                OnPropertyChanged(nameof(PowerCapacity));
                OnPropertyChanged(nameof(Tooltip));
            }
        }

        public int TempLimit {
            get => _tempLimit;
            set {
                _tempLimit = value;
                OnPropertyChanged(nameof(TempLimit));
            }
        }

        public bool IsAutoFanSpeed {
            get => _isAutoFanSpeed;
            set {
                _isAutoFanSpeed = value;
                OnPropertyChanged(nameof(IsAutoFanSpeed));
            }
        }

        public int Cool {
            get => _cool;
            set {
                _cool = value;
                OnPropertyChanged(nameof(Cool));
                OnPropertyChanged(nameof(Tooltip));
            }
        }

        public int CoreVoltage {
            get => _coreVoltage;
            set {
                _coreVoltage = value;
                OnPropertyChanged(nameof(CoreVoltage));
            }
        }

        public int MemoryVoltage {
            get => _memoryVoltage;
            set {
                _memoryVoltage = value;
                OnPropertyChanged(nameof(MemoryVoltage));
            }
        }

        public int CurrentMemoryTimingLevel {
            get => _currentMemoryTimingLevel;
            set {
                _currentMemoryTimingLevel = value;
                OnPropertyChanged(nameof(CurrentMemoryTimingLevel));
            }
        }

        private CoinViewModel _coinVm;
        public CoinViewModel CoinVm {
            get {
                if (_coinVm == null) {
                    if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out _coinVm)) {
                        _coinVm = CoinViewModel.Empty;
                    }
                }
                return _coinVm;
            }
        }

        public string Tooltip {
            get {
                return $"核心{CoreClockDelta.ToString()}M, 显存{MemoryClockDelta.ToString()}M, 功耗{PowerCapacity.ToString()}%, 风扇{(IsAutoFanSpeed ? "自动" : Cool + "%")}, 温度阈值{TempLimit.ToString()}℃";
            }
        }

        public MainMenuViewModel MainMenu {
            get {
                return MainMenuViewModel.Instance;
            }
        }
    }
}
