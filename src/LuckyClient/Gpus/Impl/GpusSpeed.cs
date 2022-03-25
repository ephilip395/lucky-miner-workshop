﻿using Lucky.Core;
using Lucky.Core.Impl;
using Lucky.Mine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Gpus.Impl {
    internal class GpusSpeed : SetBase, IGpusSpeed {
        private readonly Dictionary<int, GpuSpeed> _currentGpuSpeed = new Dictionary<int, GpuSpeed>();
        private readonly Dictionary<int, List<IGpuSpeed>> _gpuSpeedHistory = new Dictionary<int, List<IGpuSpeed>>();
        private readonly Dictionary<int, AverageSpeedWithHistory> _averageGpuSpeed = new Dictionary<int, AverageSpeedWithHistory>();

        private readonly ILuckyContext _luckycmContext;
        public GpusSpeed(ILuckyContext luckycmContext) {
            _luckycmContext = luckycmContext;
            VirtualRoot.BuildEventPath<Per10MinuteEvent>("周期清除过期的历史算力", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    ClearOutOfDateHistory();
                });

            VirtualRoot.BuildEventPath<MineStopedEvent>("停止挖矿后产生一次0算力", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    var now = DateTime.Now;
                    foreach (var gpu in _luckycmContext.GpuSet.AsEnumerable()) {
                        SetCurrentSpeed(gpuIndex: gpu.Index, speed: 0.0, isDual: false, now: now);
                        if (message.MineContext is IDualMineContext dualMineContext) {
                            SetCurrentSpeed(gpuIndex: gpu.Index, speed: 0.0, isDual: true, now: now);
                        }
                    }
                });

            VirtualRoot.BuildEventPath<MineStartedEvent>("挖矿开始时产生一次0算力0份额", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    var now = DateTime.Now;
                    _luckycmContext.CoinShareSet.UpdateShare(message.MineContext.MainCoin.GetId(), 0, 0, now);
                    _luckycmContext.GpusSpeed.ResetShare();
                    foreach (var gpu in _luckycmContext.GpuSet.AsEnumerable()) {
                        SetCurrentSpeed(gpuIndex: gpu.Index, speed: 0.0, isDual: false, now: now);
                    }
                    if (message.MineContext is IDualMineContext dualMineContext) {
                        _luckycmContext.CoinShareSet.UpdateShare(dualMineContext.DualCoin.GetId(), 0, 0, now);
                        foreach (var gpu in _luckycmContext.GpuSet.AsEnumerable()) {
                            SetCurrentSpeed(gpuIndex: gpu.Index, speed: 0.0, isDual: true, now: now);
                        }
                    }
                });
        }

        protected override void Init() {
            foreach (var gpu in _luckycmContext.GpuSet.AsEnumerable()) {
                _currentGpuSpeed.Add(gpu.Index, new GpuSpeed(gpu, mainCoinSpeed: new Speed(), dualCoinSpeed: new Speed()));
                _gpuSpeedHistory.Add(gpu.Index, new List<IGpuSpeed>());
                _averageGpuSpeed.Add(gpu.Index, new AverageSpeedWithHistory());
            }
        }

        private void ClearOutOfDateHistory() {
            InitOnce();
            DateTime now = DateTime.Now;
            lock (_gpuSpeedHistory) {
                foreach (var averageSpeed in _averageGpuSpeed.Values) {
                    averageSpeed.SpeedHistory.Add(averageSpeed.Speed);
                    averageSpeed.DualSpeedHistory.Add(averageSpeed.DualSpeed);
                }
                foreach (var historyList in _gpuSpeedHistory.Values.ToArray()) {
                    var toRemoves = historyList.Where(a => a.MainCoinSpeed.SpeedOn.AddMinutes(LuckyContext.SpeedHistoryLengthByMinute) < now).ToArray();
                    foreach (var item in toRemoves) {
                        historyList.Remove(item);
                    }
                }
            }
        }

        public IGpuSpeed CurrentSpeed(int gpuIndex) {
            InitOnce();
            if (_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return gpuSpeed;
            }
            return GpuSpeed.Empty;
        }

        public AverageSpeed GetAverageSpeed(int gpuIndex) {
            InitOnce();
            if (_averageGpuSpeed.TryGetValue(gpuIndex, out AverageSpeedWithHistory averageSpeed)) {
                if (averageSpeed.SpeedHistory.Count != 0) {
                    return new AverageSpeed() {
                        Speed = averageSpeed.SpeedHistory.Average(),
                        DualSpeed = averageSpeed.DualSpeedHistory.Average()
                    };
                }
                return averageSpeed.ToAverageSpeed();
            }
            return AverageSpeed.Empty;
        }

        public void IncreaseFoundShare(int gpuIndex) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            gpuSpeed.IncreaseMainCoinFoundShare();
            VirtualRoot.RaiseEvent(new FoundShareIncreasedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
        }

        public void IncreaseAcceptShare(int gpuIndex) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            gpuSpeed.IncreaseMainCoinAcceptShare();
            VirtualRoot.RaiseEvent(new AcceptShareIncreasedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
        }

        public void SetAcceptShare(int gpuIndex, int acceptShare) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            if (gpuSpeed.MainCoinSpeed.AcceptShare != acceptShare) {
                gpuSpeed.SetMainCoinAcceptShare(acceptShare);
                VirtualRoot.RaiseEvent(new AcceptShareSetedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
            }
        }

        public void SetRejectShare(int gpuIndex, int rejectShare) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            if (gpuSpeed.MainCoinSpeed.RejectShare != rejectShare) {
                gpuSpeed.SetMainCoinRejectShare(rejectShare);
                VirtualRoot.RaiseEvent(new RejectShareSetedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
            }
        }

        public void SetIncorrectShare(int gpuIndex, int incorrectShare) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            if (gpuSpeed.MainCoinSpeed.IncorrectShare != incorrectShare) {
                gpuSpeed.SetMainCoinIncorrectShare(incorrectShare);
                VirtualRoot.RaiseEvent(new IncorrectShareSetedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
            }
        }

        public void IncreaseRejectShare(int gpuIndex) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            gpuSpeed.IncreaseMainCoinRejectShare();
            VirtualRoot.RaiseEvent(new RejectShareIncreasedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
        }

        public void IncreaseIncorrectShare(int gpuIndex) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            gpuSpeed.IncreaseMainCoinIncorrectShare();
            VirtualRoot.RaiseEvent(new IncorrectShareIncreasedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
        }

        public void ResetShare() {
            InitOnce();
            foreach (var gpuSpeed in _currentGpuSpeed.Values) {
                gpuSpeed.ResetShare();
                VirtualRoot.RaiseEvent(new GpuShareChangedEvent(Guid.Empty, gpuSpeed: gpuSpeed));
            }
        }

        private Guid _mainCoinId;
        private void CheckReset() {
            Guid mainCoinId = _luckycmContext.MinerProfile.CoinId;
            if (this._mainCoinId != mainCoinId) {
                this._mainCoinId = mainCoinId;
                // 切换币种了，清空历史算力
                lock (_gpuSpeedHistory) {
                    foreach (var item in _gpuSpeedHistory) {
                        item.Value.Clear();
                    }
                }
                // 切换币种了，将所有显卡的当前算力置为0
                foreach (var item in _currentGpuSpeed.Values) {
                    item.Reset();
                }
                foreach (var avgSpeed in _averageGpuSpeed.Values) {
                    avgSpeed.Reset();
                }
            }
        }

        public void SetCurrentSpeed(int gpuIndex, double speed, bool isDual, DateTime now) {
            InitOnce();
            if (!_currentGpuSpeed.TryGetValue(gpuIndex, out GpuSpeed gpuSpeed)) {
                return;
            }
            CheckReset();
            lock (_gpuSpeedHistory) {
                // 将当前的旧算力加入历史列表
                if (_gpuSpeedHistory.TryGetValue(gpuSpeed.Gpu.Index, out List<IGpuSpeed> list)) {
                    list.Add(gpuSpeed.Clone());
                }
            }
            bool isChanged = false;
            // 如果变化幅度大于等于百分之一或者距离上一次算力记录的时间超过了10秒钟则视为算力变化
            if (isDual) {
                isChanged = gpuSpeed.DualCoinSpeed.SpeedOn.AddSeconds(10) < now || gpuSpeed.DualCoinSpeed.Value.IsChange(speed, 0.01);
                if (isChanged) {
                    gpuSpeed.UpdateDualCoinSpeed(speed, now);
                }
            }
            else {
                isChanged = gpuSpeed.MainCoinSpeed.SpeedOn.AddSeconds(10) < now || gpuSpeed.MainCoinSpeed.Value.IsChange(speed, 0.01);
                if (isChanged) {
                    gpuSpeed.UpdateMainCoinSpeed(speed, now);
                }
            }
            if (_averageGpuSpeed.TryGetValue(gpuIndex, out AverageSpeedWithHistory averageSpeed)) {
                if (isDual) {
                    var array = _gpuSpeedHistory[gpuIndex].Where(a => a.DualCoinSpeed.Value != 0).ToArray();
                    if (array.Length != 0) {
                        averageSpeed.DualSpeed = array.Average(a => a.DualCoinSpeed.Value);
                    }
                }
                else {
                    var array = _gpuSpeedHistory[gpuIndex].Where(a => a.MainCoinSpeed.Value != 0).ToArray();
                    if (array.Length != 0) {
                        averageSpeed.Speed = array.Average(a => a.MainCoinSpeed.Value);
                    }
                }
            }
            if (isChanged) {
                VirtualRoot.RaiseEvent(new GpuSpeedChangedEvent(isDual: isDual, Guid.Empty, gpuSpeed: gpuSpeed));
            }
        }

        public List<IGpuSpeed> GetGpuSpeedHistory(int index) {
            InitOnce();
            if (!_gpuSpeedHistory.ContainsKey(index)) {
                return new List<IGpuSpeed>();
            }
            return _gpuSpeedHistory[index];
        }

        public IEnumerable<IGpuSpeed> AsEnumerable() {
            InitOnce();
            return _currentGpuSpeed.Values;
        }
    }
}
