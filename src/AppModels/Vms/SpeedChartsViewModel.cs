﻿namespace Lucky.Vms {
    public class SpeedChartsViewModel : ViewModelBase {
        private SpeedChartViewModel _currentSpeedChartVm;
        private readonly SpeedChartViewModels _speedChartViewModels;

        public SpeedChartsViewModel() {
            _speedChartViewModels = new SpeedChartViewModels();
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppRoot.MinerProfileVm;
            }
        }

        public SpeedChartViewModel CurrentSpeedChartVm {
            get => _currentSpeedChartVm;
            set {
                if (_currentSpeedChartVm != value) {
                    _currentSpeedChartVm = value;
                    OnPropertyChanged(nameof(CurrentSpeedChartVm));
                }
            }
        }

        public SpeedChartViewModels SpeedChartVms {
            get {
                return _speedChartViewModels;
            }
        }
    }
}
