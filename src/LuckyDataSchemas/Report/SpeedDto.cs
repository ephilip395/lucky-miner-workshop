﻿using Lucky.Gpus;
using System;

namespace Lucky.Report
{
    /// <summary>
    /// 向服务器上报算力时的算力类型
    /// </summary>
    public class SpeedDto : ISpeedDto
    {
        private string _dualCoinCode;
        private int _totalPhysicalMemoryMb;

        public SpeedDto()
        {
            GpuTable = new GpuSpeedData[0];
        }

        public Guid MineContextId { get; set; }
        public DateTime LocalServerMessageTimestamp { get; set; }
        public int KernelSelfRestartCount { get; set; }
        public bool IsAutoBoot { get; set; }
        public bool IsAutoStart { get; set; }
        public int AutoStartDelaySeconds { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public int AutoRestartKernelTimes { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsNoShareRestartComputer { get; set; }
        public int NoShareRestartComputerMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }
        public int PeriodicRestartKernelMinutes { get; set; }
        public int PeriodicRestartComputerMinutes { get; set; }
        public bool IsAutoStopByCpu { get; set; }
        public int CpuGETemperatureSeconds { get; set; }
        public int CpuStopTemperature { get; set; }
        public bool IsAutoStartByCpu { get; set; }
        public int CpuLETemperatureSeconds { get; set; }
        public int CpuStartTemperature { get; set; }
        public string GpuDriver { get; set; }
        public GpuType GpuType { get; set; }
        // ReSharper disable once InconsistentNaming
        public string OSName { get; set; }
        // ReSharper disable once InconsistentNaming
        public int OSVirtualMemoryMb { get; set; }
        public int TotalPhysicalMemoryMb
        {
            get
            {
                // 因为有客户端版本的单位不正确传上来的是kb不是Mb所以如果值较大除以1024
                if (_totalPhysicalMemoryMb >= 100 * LuckyKeyword.IntK)
                {
                    _totalPhysicalMemoryMb /= LuckyKeyword.IntK;
                }
                return _totalPhysicalMemoryMb;
            }
            set
            {
                _totalPhysicalMemoryMb = value;
            }
        }
        public string DiskSpace { get; set; }

        public Guid ClientId { get; set; }

        public string MACAddress { get; set; }
        public string LocalIp { get; set; }

        public string Version { get; set; }

        public DateTime BootOn { get; set; }

        public DateTime? MineStartedOn { get; set; }

        public bool IsMining { get; set; }

        public string MinerIp { get; set; }

        public string GpuInfo { get; set; }

        public Guid MineWorkId { get; set; }

        public string MineWorkName { get; set; }

        public string MinerName { get; set; }

        private string _mainCoinCode;
        public string MainCoinCode
        {
            get => _mainCoinCode ?? string.Empty;
            set => _mainCoinCode = value;
        }

        public string MainCoinPool { get; set; }

        public string MainCoinWallet { get; set; }

        public string MainCoinPoolDelay { get; set; }

        public string Kernel { get; set; }

        public string DualCoinCode
        {
            get => _dualCoinCode ?? string.Empty;
            set => _dualCoinCode = value;
        }

        public string DualCoinPool { get; set; }

        public string DualCoinWallet { get; set; }

        public string DualCoinPoolDelay { get; set; }

        public int MainCoinTotalShare { get; set; }

        public int MainCoinRejectShare { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public int DualCoinTotalShare { get; set; }

        public int DualCoinRejectShare { get; set; }

        public double MainCoinSpeed { get; set; }

        public double DualCoinSpeed { get; set; }

        public string KernelCommandLine { get; set; }

        // 用于判断GPU算力表格中是否展示找到、拒绝、无效列
        public bool IsRejectOneGpuShare { get; set; }
        public bool IsFoundOneGpuShare { get; set; }
        public bool IsGotOneIncorrectGpuShare { get; set; }

        public int CpuPerformance { get; set; }

        public int CpuTemperature { get; set; }

        public bool IsRaiseHighCpuEvent { get; set; }

        public int HighCpuPercent { get; set; }

        public int HighCpuSeconds { get; set; }

        public bool IsOuterUserEnabled { get; set; }

        public bool IsAutoDisableWindowsFirewall { get; set; }

        public bool IsDisableUAC { get; set; }

        public bool IsDisableWAU { get; set; }

        public bool Is1080PillEnabled { get; set; }

        public bool IsDisableAntiSpyware { get; set; }

        public bool IsAutoReboot { get; set; }

        public DateTime MainCoinSpeedOn { get; set; }

        public DateTime DualCoinSpeedOn { get; set; }

        /// <summary>
        /// 内网群控时看到的外网群控用户
        /// </summary>
        public string ReportOuterUserId { get; set; }
        public bool IsPreventDisplaySleep { get; set; }
        public bool IsLowSpeedRestartComputer { get; set; }
        public int LowSpeedRestartComputerMinutes { get; set; }
        public double LowSpeed { get; set; }
        public bool IsLowSpeedReOverClock { get; set; }
        public int LowSpeedReOverClockMinutes { get; set; }
        public double OverClockLowSpeed { get; set; }

        public GpuSpeedData[] GpuTable { get; set; }

        public bool IsAdvParamsVisual { get; set; }
        public bool IsUseProxy { get; set; }
        public int ConnectionMethod { get; set; }
        public string ProxyServerAddress { get; set; }
        public int ProxyServerPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public bool IsProxyPool { get; set; }
        public bool IsProxyGroupMonitor { get; set; }
    }
}
