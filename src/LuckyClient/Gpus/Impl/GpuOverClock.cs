using System;

namespace Lucky.Gpus.Impl {
    public class GpuOverClock : IOverClock {
        private readonly IGpuHelper _gpuHelper;
        public GpuOverClock(IGpuHelper gpuHelper) {
            _gpuHelper = gpuHelper;
        }

        public void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == LuckyContext.GpuAllId) {
                return;
            }
            try {
                OverClockRange range = _gpuHelper.GetClockRange(gpu);
                gpu.UpdateState(range);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.RaiseEvent(new GpuStateChangedEvent(Guid.Empty, gpu));
        }

        public void OverClock(int gpuIndex, OverClockValue value) {
            if (gpuIndex == LuckyContext.GpuAllId) {
                foreach (var gpu in LuckyContext.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == LuckyContext.GpuAllId) {
                        continue;
                    }
                    _gpuHelper.OverClock(gpu, value);
                }
            }
            else {
                if (!LuckyContext.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    return;
                }
                _gpuHelper.OverClock(gpu, value);
            }
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            if (gpuIndex == LuckyContext.GpuAllId) {
                foreach (var gpu in LuckyContext.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == LuckyContext.GpuAllId) {
                        continue;
                    }
                    _gpuHelper.SetFanSpeed(gpu, value);
                }
            }
            else {
                if (!LuckyContext.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    return;
                }
                _gpuHelper.SetFanSpeed(gpu, value);
            }
        }

        public void Restore() {
            OverClock(gpuIndex: LuckyContext.GpuAllId, OverClockValue.RestoreValue);
            RefreshGpuState(LuckyContext.GpuAllId);
        }

        public void RefreshGpuState(int gpuIndex) {
            if (gpuIndex == LuckyContext.GpuAllId) {
                foreach (var gpu in LuckyContext.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == LuckyContext.GpuAllId) {
                        continue;
                    }
                    RefreshGpuState(gpu);
                }
            }
            else {
                if (LuckyContext.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    RefreshGpuState(gpu);
                }
            }
        }
    }
}
