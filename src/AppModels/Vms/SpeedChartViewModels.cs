﻿using System.Collections.Generic;

namespace Lucky.Vms {
    public class SpeedChartViewModels {
        private readonly Dictionary<int, SpeedChartViewModel> _dicByGpuIndex = new Dictionary<int, SpeedChartViewModel>();

        public SpeedChartViewModels() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            if (AppRoot.MinerProfileVm.CoinVm != null) {
                foreach (var item in AppRoot.GpuSpeedVms.All) {
                    _dicByGpuIndex.Add(item.GpuVm.Index, new SpeedChartViewModel(item));
                }
            }
        }

        public bool ContainsKey(int gpuIndex) {
            return _dicByGpuIndex.ContainsKey(gpuIndex);
        }

        public SpeedChartViewModel this[int index] {
            get {
                if (_dicByGpuIndex.ContainsKey(index)) {
                    return _dicByGpuIndex[index];
                }
                return null;
            }
        }

        public IEnumerable<SpeedChartViewModel> Items {
            get {
                return _dicByGpuIndex.Values;
            }
        }
    }
}
