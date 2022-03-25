﻿using Lucky.Gpus.Impl;
using Lucky.Vms;
using System.Collections.Generic;
using System.Windows.Media;

namespace Lucky
{
    public static partial class AppRoot
    {
        public class GpuViewModels : ViewModelBase
        {
            public static GpuViewModels Instance { get; private set; } = new GpuViewModels();

            private readonly Dictionary<int, GpuViewModel> _gpuVms = new Dictionary<int, GpuViewModel>();

            private string _fanSpeedMinText = "0 %";
            private string _fanSpeedMaxText = "0 %";
            private string _temperatureMinText = "0 ℃";
            private string _temperatureMaxText = "0 ℃";
            private string _memoryTemperatureMinText = "0 ℃";
            private string _memoryTemperatureMaxText = "0 ℃";
            private SolidColorBrush _temperatureMaxForeground;
            private readonly GpuViewModel _gpuAllVm;
            private GpuViewModels()
            {
                if (WpfUtil.IsInDesignMode)
                {
                    return;
                }
                foreach (var gpu in LuckyContext.Instance.GpuSet.AsEnumerable())
                {
                    _gpuVms.Add(gpu.Index, new GpuViewModel(gpu));
                }
                if (_gpuVms.ContainsKey(LuckyContext.GpuAllId))
                {
                    _gpuAllVm = _gpuVms[LuckyContext.GpuAllId];
                }
                else
                {
                    _gpuAllVm = new GpuViewModel(Gpu.GpuAll);
                }
                BuildEventPath<EPriceChangedEvent>("电价变更后更新电费显示", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        foreach (var gpuVm in _gpuVms.Values)
                        {
                            gpuVm.OnPropertyChanged(nameof(GpuViewModel.EChargeText));
                        }
                        GpuSpeedVms.OnPropertyChanged(nameof(GpuSpeedViewModels.ProfitCnyPerDayText));
                    });
                BuildEventPath<MaxTempChangedEvent>("高温红色阈值变更后更新显卡温度颜色", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        foreach (var gpuVm in _gpuVms.Values)
                        {
                            gpuVm.OnPropertyChanged(nameof(GpuViewModel.TemperatureForeground));
                        }
                    });
                BuildEventPath<PowerAppendChangedEvent>("功耗补偿变更后更新功耗显示", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        foreach (var gpuVm in _gpuVms.Values)
                        {
                            gpuVm.OnPropertyChanged(nameof(GpuViewModel.PowerUsageWText));
                        }
                        GpuSpeedVms.OnPropertyChanged(nameof(GpuSpeedViewModels.ProfitCnyPerDayText));
                    });
                BuildEventPath<GpuStateChangedEvent>("显卡状态变更后刷新VM内存", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        if (_gpuVms.ContainsKey(message.Source.Index))
                        {
                            GpuViewModel vm = _gpuVms[message.Source.Index];
                            vm.Temperature = message.Source.Temperature;
                            vm.MemoryTemperature = message.Source.MemoryTemperature;
                            vm.FanSpeed = message.Source.FanSpeed;
                            vm.PowerUsage = message.Source.PowerUsage;
                            vm.CoreClockDelta = message.Source.CoreClockDelta;
                            vm.MemoryClockDelta = message.Source.MemoryClockDelta;
                            vm.CoreClockDeltaMin = message.Source.CoreClockDeltaMin;
                            vm.CoreClockDeltaMax = message.Source.CoreClockDeltaMax;
                            vm.MemoryClockDeltaMin = message.Source.MemoryClockDeltaMin;
                            vm.MemoryClockDeltaMax = message.Source.MemoryClockDeltaMax;
                            vm.CurrentMemoryTimingLevel = message.Source.CurrentMemoryTimingLevel;
                            vm.Cool = message.Source.Cool;
                            vm.CoolMin = message.Source.CoolMin;
                            vm.CoolMax = message.Source.CoolMax;
                            vm.PowerCapacity = message.Source.PowerCapacity;
                            vm.PowerMin = message.Source.PowerMin;
                            vm.PowerMax = message.Source.PowerMax;
                            vm.TempLimit = message.Source.TempLimit;
                            vm.TempLimitDefault = message.Source.TempLimitDefault;
                            vm.TempLimitMax = message.Source.TempLimitMax;
                            vm.TempLimitMin = message.Source.TempLimitMin;
                            vm.CoreVoltage = message.Source.CoreVoltage;
                            vm.MemoryVoltage = message.Source.MemoryVoltage;
                            vm.VoltMin = message.Source.VoltMin;
                            vm.VoltMax = message.Source.VoltMax;
                            vm.VoltDefault = message.Source.VoltDefault;
                            if (_gpuAllVm != null)
                            {
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.TemperatureText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.MemoryTemperatureText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.FanSpeedText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.PowerUsageWText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.CoreClockDeltaMText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.MemoryClockDeltaMText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.CoreClockDeltaMinMaxMText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.MemoryClockDeltaMinMaxMText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.CoolMinMaxText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.PowerMinMaxText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.TempLimitMinMaxText));
                                _gpuAllVm.OnPropertyChanged(nameof(_gpuAllVm.EChargeText));
                                GpuSpeedVms.OnPropertyChanged(nameof(GpuSpeedViewModels.ProfitCnyPerDayText));
                            }
                            UpdateMinMax();
                        }
                    });
            }

            private void UpdateMinMax()
            {
                uint minFan = uint.MaxValue, maxFan = uint.MinValue;
                foreach (var item in _gpuVms.Values)
                {
                    if (item.Index == LuckyContext.GpuAllId)
                    {
                        continue;
                    }
                    if (item.FanSpeed > maxFan)
                    {
                        maxFan = item.FanSpeed;
                    }
                    if (item.FanSpeed < minFan)
                    {
                        minFan = item.FanSpeed;
                    }
                }
                this.FanSpeedMaxText = maxFan + " %";
                this.FanSpeedMinText = minFan + " %";
                int minTemp = int.MaxValue, maxTemp = int.MinValue;
                int memMinTemp = int.MaxValue, memMaxTemp = int.MinValue;
                foreach (var item in _gpuVms.Values)
                {
                    if (item.Index == LuckyContext.GpuAllId)
                    {
                        continue;
                    }
                    if (item.Temperature > maxTemp)
                    {
                        maxTemp = item.Temperature;
                    }
                    if (item.Temperature < minTemp)
                    {
                        minTemp = item.Temperature;
                    }
                    if (item.MemoryTemperature > memMaxTemp)
                    {
                        memMaxTemp = item.MemoryTemperature;
                    }
                    if (item.MemoryTemperature < memMinTemp)
                    {
                        memMinTemp = item.MemoryTemperature;
                    }
                }
                this.TemperatureMinText = minTemp + " ℃";
                this.TemperatureMaxText = maxTemp + " ℃";
                this.MemoryTemperatureMinText = memMinTemp + " ℃";
                this.MemoryTemperatureMaxText = memMaxTemp + " ℃";
                if (maxTemp >= MinerProfileVm.MaxTemp)
                {
                    this.TemperatureMaxForeground = WpfUtil.RedBrush;
                }
                else
                {
                    this.TemperatureMaxForeground = WpfUtil.BlackBrush;
                }
            }

            public GpuViewModel GpuAllVm
            {
                get
                {
                    return _gpuAllVm;
                }
            }

            public int Count
            {
                get
                {
                    if (_gpuAllVm != null)
                    {
                        return _gpuVms.Count - 1;
                    }
                    return _gpuVms.Count;
                }
            }

            public string TemperatureMinText
            {
                get => _temperatureMinText;
                set
                {
                    if (_temperatureMinText != value)
                    {
                        _temperatureMinText = value;
                        OnPropertyChanged(nameof(TemperatureMinText));
                    }
                }
            }

            public string TemperatureMaxText
            {
                get => _temperatureMaxText;
                set
                {
                    if (_temperatureMaxText != value)
                    {
                        _temperatureMaxText = value;
                        OnPropertyChanged(nameof(TemperatureMaxText));
                    }
                }
            }

            public string MemoryTemperatureMinText
            {
                get => _memoryTemperatureMinText;
                set
                {
                    if (_memoryTemperatureMinText != value)
                    {
                        _memoryTemperatureMinText = value;
                        OnPropertyChanged(nameof(MemoryTemperatureMinText));
                    }
                }
            }

            public string MemoryTemperatureMaxText
            {
                get => _memoryTemperatureMaxText;
                set
                {
                    if (_memoryTemperatureMaxText != value)
                    {
                        _memoryTemperatureMaxText = value;
                        OnPropertyChanged(nameof(MemoryTemperatureMaxText));
                    }
                }
            }

            public SolidColorBrush TemperatureMaxForeground
            {
                get => _temperatureMaxForeground;
                set
                {
                    if (_temperatureMaxForeground != value)
                    {
                        _temperatureMaxForeground = value;
                        OnPropertyChanged(nameof(TemperatureMaxForeground));
                    }
                }
            }

            public string FanSpeedMinText
            {
                get => _fanSpeedMinText;
                set
                {
                    if (_fanSpeedMinText != value)
                    {
                        _fanSpeedMinText = value;
                        OnPropertyChanged(nameof(FanSpeedMinText));
                    }
                }
            }

            public string FanSpeedMaxText
            {
                get { return _fanSpeedMaxText; }
                set
                {
                    if (_fanSpeedMaxText != value)
                    {
                        _fanSpeedMaxText = value;
                        OnPropertyChanged(nameof(FanSpeedMaxText));
                    }
                }
            }

            public bool TryGetGpuVm(int index, out GpuViewModel gpuVm)
            {
                return _gpuVms.TryGetValue(index, out gpuVm);
            }

            public IEnumerable<GpuViewModel> Items
            {
                get
                {
                    return _gpuVms.Values;
                }
            }
        }
    }
}
