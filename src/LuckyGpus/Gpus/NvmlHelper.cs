﻿using Lucky.Gpus.Nvml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lucky.Gpus {
    public class NvmlHelper {
        public class NvGpu {
            public NvGpu() { }

            public int GpuIndex { get; set; }
            public int BusId { get; set; }
            public string Name { get; set; }

            public ulong TotalMemory { get; set; }

            public override string ToString() {
                return VirtualRoot.JsonSerializer.Serialize(this);
            }
        }

        #region static NvmlInit
        private static readonly string _nvmlDllFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation", "NVSMI", "nvml.dll");
        private static readonly string _system32nvmlDllFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "nvml.dll");
        private static bool _isNvmlInited = false;
        private static readonly object _locker = new object();
        private static bool NvmlInit() {
            if (_isNvmlInited) {
                return _isNvmlInited;
            }
            lock (_locker) {
                if (_isNvmlInited) {
                    return _isNvmlInited;
                }
                _isNvmlInited = true;
                try {
#if DEBUG
                    NTStopwatch.Start();
#endif
                    if (!File.Exists(_system32nvmlDllFileFullName) && File.Exists(_nvmlDllFileFullName)) {
                        File.Copy(_nvmlDllFileFullName, _system32nvmlDllFileFullName);
                    }
                    var nvmlReturn = NvmlNativeMethods.NvmlInit();
                    _isNvmlInited = nvmlReturn == nvmlReturn.Success;
#if DEBUG
                    var elapsedMilliseconds = NTStopwatch.Stop();
                    if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                        LuckyConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {nameof(NvmlHelper)}.{nameof(NvmlInit)}()");
                    }
#endif
                    return _isNvmlInited;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return false;
            }
        }
        #endregion

        private readonly List<nvmlDevice> _nvmlDevices = new List<nvmlDevice>();

        public NvmlHelper() { }

        public List<NvGpu> GetGpus() {
            List<NvGpu> results = new List<NvGpu>();
            try {
                if (NvmlInit()) {
                    _nvmlDevices.Clear();
                    uint deviceCount = 0;
                    var r = NvmlNativeMethods.nvmlDeviceGetCount(ref deviceCount);
                    CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetCount)} {r.ToString()}");
                    for (int i = 0; i < deviceCount; i++) {
                        NvGpu gpu = new NvGpu {
                            GpuIndex = i
                        };
                        nvmlDevice nvmlDevice = new nvmlDevice();
                        r = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                        _nvmlDevices.Add(nvmlDevice);
                        CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetHandleByIndex)}({((uint)i).ToString()}) {r.ToString()}");
                        r = NvmlNativeMethods.nvmlDeviceGetName(nvmlDevice, out string name);
                        CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetName)} {r.ToString()}");
                        nvmlMemory memory = new nvmlMemory();
                        r = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(nvmlDevice, ref memory);
                        CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetMemoryInfo)} {r.ToString()}");
                        // short gpu name
                        if (!string.IsNullOrEmpty(name)) {
                            name = name.Replace("GeForce GTX ", string.Empty);
                            name = name.Replace("GeForce ", string.Empty);
                        }
                        nvmlPciInfo pci = new nvmlPciInfo();
                        r = NvmlNativeMethods.nvmlDeviceGetPciInfo(nvmlDevice, ref pci);
                        CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetPciInfo)} {r.ToString()}");
                        gpu.Name = name;
                        gpu.BusId = (int)pci.bus;
                        gpu.TotalMemory = memory.total;
                        results.Add(gpu);
                    }
                }
            }
            catch {
            }

            return results;
        }

        private bool TryGetNvmlDevice(int gpuIndex, out nvmlDevice nvmlDevice) {
            nvmlDevice = default;
            if (gpuIndex < 0 || gpuIndex >= _nvmlDevices.Count) {
                return false;
            }
            nvmlDevice = _nvmlDevices[gpuIndex];
            return true;
        }

        private readonly HashSet<int> _isFirstGetPowerUsage = new HashSet<int>();
        // NVAPI貌似没有读取功耗的接口，所以只能使用NVML
        public uint GetPowerUsage(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            uint power = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetPowerUsage(nvmlDevice, ref power);
                power = (uint)(power / 1000.0);
                if (!_isFirstGetPowerUsage.Contains(gpuIndex)) {
                    _isFirstGetPowerUsage.Add(gpuIndex);
                    CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetPowerUsage)} {r.ToString()}");
                }
            }
            catch {
            }
            return power;
        }

        private readonly HashSet<int> _isFirstGetTemperature = new HashSet<int>();
        public uint GetTemperature(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            uint temp = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetTemperature(nvmlDevice, nvmlTemperatureSensors.Gpu, ref temp);
                if (!_isFirstGetTemperature.Contains(gpuIndex)) {
                    _isFirstGetTemperature.Add(gpuIndex);
                    CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetTemperature)} {r.ToString()}");
                }
            }
            catch {
            }
            return temp;
        }

        private readonly HashSet<int> _isFirstGetFanSpeed = new HashSet<int>();
        public uint GetFanSpeed(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            uint fanSpeed = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetFanSpeed(nvmlDevice, ref fanSpeed);
                if (!_isFirstGetFanSpeed.Contains(gpuIndex)) {
                    _isFirstGetFanSpeed.Add(gpuIndex);
                    CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlDeviceGetFanSpeed)} {r.ToString()}");
                }
            }
            catch {
            }
            return fanSpeed;
        }

        // 注意：因为转化为Version对象时会将457.09格式的字符串变成457.9格式的Version，为了保留前缀0这里输出原始字符串
        public void GetVersion(out string driverVersion, out string nvmlVersion) {
            driverVersion = "0.0";
            nvmlVersion = "0.0";
            if (!NvmlInit()) {
                return;
            }
            try {
                var r = NvmlNativeMethods.nvmlSystemGetDriverVersion(out driverVersion);
                CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlSystemGetDriverVersion)} {r.ToString()}");
                r = NvmlNativeMethods.nvmlSystemGetNVMLVersion(out nvmlVersion);
                CheckResult(r, () => $"{nameof(NvmlNativeMethods.nvmlSystemGetNVMLVersion)} {r.ToString()}");
                if (string.IsNullOrEmpty(nvmlVersion)) {
                    nvmlVersion = "0.0";
                }
            }
            catch {
            }
        }

        private static void CheckResult(nvmlReturn r, Func<string> getMessage) {
            if (r != nvmlReturn.Success) {
                LuckyConsole.DevError(getMessage);
            }
        }
    }
}
