﻿using Lucky.Core;
using Lucky.Core.Impl;
using System;

namespace Lucky.Gpus.Impl {
    internal class GpuSpeed : IGpuSpeed {
        public static readonly GpuSpeed Empty = new GpuSpeed(Impl.Gpu.GpuAll, new Speed(), new Speed());

        private readonly Speed _mainCoinSpeed, _dualCoinSpeed;
        public GpuSpeed(IGpu gpu, Speed mainCoinSpeed, Speed dualCoinSpeed) {
            this.Gpu = gpu;
            _mainCoinSpeed = mainCoinSpeed;
            _dualCoinSpeed = dualCoinSpeed;
            FoundShareOn = DateTime.MinValue;
        }

        public DateTime FoundShareOn { get; private set; }

        public IGpu Gpu { get; private set; }

        public ISpeed MainCoinSpeed {
            get { return _mainCoinSpeed; }
        }

        public ISpeed DualCoinSpeed {
            get { return _dualCoinSpeed; }
        }

        public void Reset() {
            _mainCoinSpeed.Reset();
            _dualCoinSpeed.Reset();
            FoundShareOn = DateTime.MinValue;
        }

        public void ResetShare() {
            _mainCoinSpeed.ResetShare();
            _dualCoinSpeed.ResetShare();
            FoundShareOn = DateTime.MinValue;
        }

        public void IncreaseMainCoinFoundShare() {
            _mainCoinSpeed.FoundShare++;
            FoundShareOn = DateTime.Now;
        }

        public void IncreaseMainCoinAcceptShare() {
            _mainCoinSpeed.AcceptShare++;
        }

        public void SetMainCoinAcceptShare(int acceptShare) {
            _mainCoinSpeed.AcceptShare = acceptShare;
        }

        public void SetMainCoinRejectShare(int rejectShare) {
            _mainCoinSpeed.RejectShare = rejectShare;
        }

        public void SetMainCoinIncorrectShare(int incorrectShare) {
            _mainCoinSpeed.IncorrectShare = incorrectShare;
        }

        public void IncreaseMainCoinRejectShare() {
            _mainCoinSpeed.RejectShare++;
        }

        public void IncreaseMainCoinIncorrectShare() {
            _mainCoinSpeed.IncorrectShare++;
        }

        public void UpdateMainCoinSpeed(double speed, DateTime speedOn) {
            _mainCoinSpeed.Value = speed;
            _mainCoinSpeed.SpeedOn = speedOn;
        }

        public void UpdateDualCoinSpeed(double speed, DateTime speedOn) {
            _dualCoinSpeed.Value = speed;
            _dualCoinSpeed.SpeedOn = speedOn;
        }
    }
}
