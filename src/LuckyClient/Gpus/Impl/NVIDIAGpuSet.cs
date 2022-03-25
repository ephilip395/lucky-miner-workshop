﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucky.Gpus.Impl {
    internal class NVIDIAGpuSet : IGpuSet {
        private readonly Dictionary<int, Gpu> _gpus = new Dictionary<int, Gpu>() {
            [LuckyContext.GpuAllId] = Gpu.GpuAll
        };
        private readonly string _driverVersionRaw = "0.0";
        private readonly Version _driverVersion = new Version();

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private readonly NvapiHelper _nvapiHelper = new NvapiHelper();
        private readonly NvmlHelper _nvmlHelper = new NvmlHelper();
        public NVIDIAGpuSet(ILuckyContext luckycmContext) {
            this.OverClock = new GpuOverClock(_nvapiHelper);
            this.Properties = new List<GpuSetProperty>();
            var gpus = _nvmlHelper.GetGpus();
            if (gpus.Count > 0) {
                foreach (var item in gpus) {
                    var gpu = Gpu.Create(GpuType.NVIDIA, item.GpuIndex, item.BusId.ToString(), item.Name);
                    gpu.TotalMemory = item.TotalMemory;
                    _gpus.Add(item.GpuIndex, gpu);
                }
                _nvmlHelper.GetVersion(out _driverVersionRaw, out string nvmlVersion);
                Version.TryParse(_driverVersionRaw, out _driverVersion);
                this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", _driverVersion));
                try {
                    var item = luckycmContext.ServerContext.SysDicItemSet.GetSysDicItems(LuckyKeyword.CudaVersionSysDicCode)
                        .Select(a => new { Version = double.Parse(a.Value), a })
                        .OrderByDescending(a => a.Version)
                        .FirstOrDefault(a => _driverVersion.Major >= a.Version);
                    if (item != null) {
                        this.Properties.Add(new GpuSetProperty(LuckyKeyword.CudaVersionSysDicCode, "Cuda版本", item.a.Code));
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                this.Properties.Add(new GpuSetProperty("NVMLVersion", "NVML版本", nvmlVersion));
                Dictionary<string, string> kvs = new Dictionary<string, string> {
                    ["CUDA_DEVICE_ORDER"] = "PCI_BUS_ID"
                };
                foreach (var kv in kvs) {
                    var property = new GpuSetProperty(kv.Key, kv.Key, kv.Value);
                    this.Properties.Add(property);
                }
                Task.Factory.StartNew(() => {
                    OverClock.RefreshGpuState(LuckyContext.GpuAllId);
                    // 这里会耗时5秒
                    foreach (var kv in kvs) {
                        Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                    }
                });
            }
        }

        public void LoadGpuState() {
            for (int i = 0; i < Count; i++) {
                LoadGpuState(i);
            }
        }

        public void LoadGpuState(int gpuIndex) {
            if (gpuIndex == LuckyContext.GpuAllId) {
                return;
            }
            var gpu = _gpus[gpuIndex];
            uint power = _nvmlHelper.GetPowerUsage(gpuIndex);
            _nvapiHelper.GetTemperature(gpu, out uint coreTemperature, out uint memoryTemperature);
            if (!_nvapiHelper.GetFanSpeed(gpu.GetOverClockId(), out uint fanSpeed)) {
                fanSpeed = _nvmlHelper.GetFanSpeed(gpuIndex);
            }
            bool isChanged = gpu.Temperature != coreTemperature || gpu.MemoryTemperature != memoryTemperature || gpu.PowerUsage != power || gpu.FanSpeed != fanSpeed;
            gpu.Temperature = (int)coreTemperature;
            gpu.MemoryTemperature = (int)memoryTemperature;
            if (power != 0) {
                gpu.PowerUsage = power;
            }
            gpu.FanSpeed = fanSpeed;

            if (isChanged) {
                VirtualRoot.RaiseEvent(new GpuStateChangedEvent(Guid.Empty, gpu));
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.NVIDIA;
            }
        }

        public string DriverVersion {
            get {
                var cudaVersion = this.Properties.FirstOrDefault(a => a.Code == LuckyKeyword.CudaVersionSysDicCode);
                if (cudaVersion != null) {
                    return $"{this._driverVersionRaw} {cudaVersion.Value}";
                }
                return this._driverVersionRaw;
            }
        }

        public bool IsLowDriverVersion {
            get {
                return this._driverVersion < LuckyContext.Instance.MinNvidiaDriverVersion;
            }
        }

        public bool TryGetGpu(int index, out IGpu gpu) {
            bool r = _gpus.TryGetValue(index, out Gpu g);
            gpu = g;
            return r;
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; }

        public DateTime HighTemperatureOn { get; set; }

        public DateTime LowTemperatureOn { get; set; }

        public IEnumerable<IGpu> AsEnumerable() {
            return _gpus.Values;
        }
    }
}
