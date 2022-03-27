﻿using System;

namespace Lucky.Core.Profile {
    public interface IMinerProfile : IEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }
        string MinerName { get; }
        bool IsSystemName { get; }

        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }

        bool IsNetUnavailableStopMine { get; }
        int NetUnavailableStopMineMinutes { get; }
        bool IsNetAvailableStartMine { get; }
        int NetAvailableStartMineSeconds { get; }

        bool IsNoShareRestartComputer { get; }
        int NoShareRestartComputerMinutes { get; }

        bool IsPeriodicRestartKernel { get; }
        int PeriodicRestartKernelHours { get; }
        int PeriodicRestartKernelMinutes { get; }

        bool IsPeriodicRestartComputer { get; }
        int PeriodicRestartComputerHours { get; }
        int PeriodicRestartComputerMinutes { get; }

        bool IsAutoRestartKernel { get; }
        int AutoRestartKernelTimes { get; }

        bool IsSpeedDownRestartComputer { get; }
        int RestartComputerSpeedDownPercent { get; }

        bool IsEChargeEnabled { get; }

        double EPrice { get; }

        bool IsPowerAppend { get; }
        int PowerAppend { get; }
        int MaxTemp { get; }
        int AutoStartDelaySeconds { get; }
        bool IsAutoDisableWindowsFirewall { get; }
        bool IsDisableUAC { get; }
        bool IsDisableWAU { get; }
        bool IsDisableAntiSpyware { get; }
        bool IsAutoReboot { get; }
        bool IsShowInTaskbar { get; }
        bool IsAutoNoUi { get; }
        int AutoNoUiMinutes { get; }
        bool IsShowNotifyIcon { get; }
        bool IsPreventDisplaySleep { get; }
        bool IsCloseMeanExit { get; }
        bool Is1080PillEnabled { get; }
        bool IsShowCommandLine { get; }
        bool IsAutoBoot { get; }
        bool IsAutoStart { get; }
        bool IsCreateShortcut { get; }
        bool IsAutoStopByCpu { get; }
        int CpuStopTemperature { get; }
        int CpuGETemperatureSeconds { get; }
        bool IsAutoStartByCpu { get; }
        int CpuStartTemperature { get; }
        int CpuLETemperatureSeconds { get; }
        bool IsAutoStopByGpu { get; }
        int GpuStopTemperature { get; }
        int GpuGETemperatureSeconds { get; }
        bool IsAutoStartByGpu { get; }
        int GpuStartTemperature { get; }
        int GpuLETemperatureSeconds { get; }
        bool IsRaiseHighCpuEvent { get; }
        int HighCpuBaseline { get; }
        int HighCpuSeconds { get; }
        bool IsOuterUserEnabled { get; }
        /// <summary>
        /// LoginName、Email或Mobile
        /// </summary>
        string OuterUserId { get; }

        bool IsUseProxy { get; }

        bool IsAdvParamsVisual { get; }

        int ConnectionMethod { get; }
        string ProxyServerAddress { get; }
        int ProxyServerPort { get; }
        string ProxyUsername { get; }
        string ProxyPassword { get; }
        bool IsProxyPool { get; }
        bool IsProxyGroupMonitor { get; }
    }
}
