﻿using LiteDB;
using Newtonsoft.Json;
using Lucky.Gpus;
using Lucky.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lucky.Core.MinerServer {
    public class ClientData : SpeedDto, IClientData {
        private static readonly Dictionary<string, PropertyInfo> _reflectionUpdateProperties = new Dictionary<string, PropertyInfo>();

        public static bool TryGetReflectionUpdateProperty(string propertyName, out PropertyInfo propertyInfo) {
            return _reflectionUpdateProperties.TryGetValue(propertyName, out propertyInfo);
        }

        static ClientData() {
            Type type = typeof(ClientData);
            // 这算是一个安全措施，因为propertyName是来自客户端传入的，所以需要白名单。
            HashSet<string> propertyNames = new HashSet<string> {
                nameof(WorkerName),
                nameof(GroupId),
                nameof(WorkId),
                nameof(WindowsLoginName),
                nameof(WindowsPassword)
            };
            foreach (var propertyName in propertyNames) {
                _reflectionUpdateProperties.Add(propertyName, type.GetProperty(propertyName));
            }
        }

        public ClientData() : base() {
        }

        // 用户在内网群控端添加矿机时
        public static ClientData Create(string minerIp, out MinerData minerData) {
            minerData = MinerData.Create(minerIp);
            return Create(minerData);
        }

        // WebApiServer收到矿机签名变更事件但矿机不存在时
        public static ClientData Create(IMinerSign minerSign) {
            var minerData = MinerData.Create(minerSign);
            return Create(minerData);
        }

        // 内网群控端从litedb和WebApiServer从redis加载数据时
        public static ClientData Create(IMinerData data) {
            return new ClientData() {
                #region
                Id = data.Id,
                ClientId = data.ClientId,
                MACAddress = data.MACAddress,
                LocalIp = data.LocalIp,
                MinerIp = data.MinerIp,
                MinerName = data.MinerName,
                WorkerName = data.WorkerName,
                CreatedOn = data.CreatedOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                AESPassword = data.AESPassword,
                AESPasswordOn = data.AESPasswordOn,
                LoginName = data.LoginName,
                IsOuterUserEnabled = data.IsOuterUserEnabled,
                OuterUserId = data.OuterUserId,
                ReportOuterUserId = data.OuterUserId,
                MineContextId = Guid.Empty,
                DiskSpaceMb = 0,
                MineWorkId = Guid.Empty,
                MineWorkName = string.Empty,
                IsAutoBoot = false,
                IsAutoStart = false,
                AutoStartDelaySeconds = 15,
                IsAutoRestartKernel = false,
                AutoRestartKernelTimes = 10,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 0,
                IsNoShareRestartComputer = false,
                NoShareRestartComputerMinutes = 0,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 0,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 0,
                PeriodicRestartKernelMinutes = 10,
                PeriodicRestartComputerMinutes = 10,
                IsAutoStartByCpu = false,
                IsAutoStopByCpu = false,
                CpuGETemperatureSeconds = 60,
                CpuLETemperatureSeconds = 60,
                CpuStartTemperature = 40,
                CpuStopTemperature = 65,
                CpuPerformance = 0,
                CpuTemperature = 0,
                IsRaiseHighCpuEvent = false,
                HighCpuPercent = 80,
                HighCpuSeconds = 10,
                GpuDriver = string.Empty,
                GpuType = GpuType.Empty,
                OSName = string.Empty,
                OSVirtualMemoryMb = 0,
                GpuInfo = string.Empty,
                Version = string.Empty,
                IsMining = false,
                BootOn = DateTime.MinValue,
                MineStartedOn = DateTime.MinValue,
                MinerActiveOn = DateTime.MinValue,
                MainCoinCode = string.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinSpeed = 0,
                MainCoinPool = string.Empty,
                MainCoinWallet = string.Empty,
                Kernel = string.Empty,
                IsDualCoinEnabled = false,
                DualCoinPool = string.Empty,
                DualCoinWallet = string.Empty,
                DualCoinCode = string.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinSpeed = 0,
                KernelCommandLine = string.Empty,
                MainCoinPoolDelay = string.Empty,
                DualCoinPoolDelay = string.Empty,
                DiskSpace = string.Empty,
                IsFoundOneGpuShare = false,
                IsRejectOneGpuShare = false,
                IsGotOneIncorrectGpuShare = false,
                KernelSelfRestartCount = 0,
                TotalPhysicalMemoryMb = 0,
                LocalServerMessageTimestamp = Timestamp.UnixBaseTime,
                NetActiveOn = DateTime.MinValue,
                IsOnline = false,
                IsAutoDisableWindowsFirewall = true,
                IsDisableAntiSpyware = true,
                IsDisableUAC = true,
                IsDisableWAU = true,
                Is1080PillEnabled = true,
                IsAutoReboot = true,
                MainCoinSpeedOn = DateTime.MinValue,
                DualCoinSpeedOn = DateTime.MinValue,
                GpuTable = new GpuSpeedData[0],
                DualCoinPoolDelayNumber = 0,
                MainCoinPoolDelayNumber = 0,
                MainCoinRejectPercent = 0,
                DualCoinRejectPercent = 0,
                IsPreventDisplaySleep = true,
                IsLowSpeedRestartComputer = false,
                LowSpeedRestartComputerMinutes = 0,
                LowSpeed = 0,
                IsLowSpeedReOverClock = false,
                LowSpeedReOverClockMinutes = 0,
                OverClockLowSpeed = 0
                #endregion
            };
        }

        public static ClientData Clone(ClientData data) {
            return new ClientData() {
                #region
                Id = data.Id,
                DiskSpaceMb = data.DiskSpaceMb,
                MineContextId = data.MineContextId,
                MinerName = data.MinerName,
                MinerIp = data.MinerIp,
                CreatedOn = data.CreatedOn,
                MinerActiveOn = data.MinerActiveOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                MACAddress = data.MACAddress,
                LocalIp = data.LocalIp,
                ClientId = data.ClientId,
                IsAutoBoot = data.IsAutoBoot,
                IsAutoStart = data.IsAutoStart,
                AutoStartDelaySeconds = data.AutoStartDelaySeconds,
                IsAutoRestartKernel = data.IsAutoRestartKernel,
                AutoRestartKernelTimes = data.AutoRestartKernelTimes,
                IsNoShareRestartKernel = data.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = data.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = data.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = data.NoShareRestartComputerMinutes,
                IsPeriodicRestartKernel = data.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = data.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = data.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = data.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = data.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = data.PeriodicRestartKernelMinutes,
                IsAutoStopByCpu = data.IsAutoStopByCpu,
                IsAutoStartByCpu = data.IsAutoStartByCpu,
                CpuStopTemperature = data.CpuStopTemperature,
                CpuStartTemperature = data.CpuStartTemperature,
                CpuLETemperatureSeconds = data.CpuLETemperatureSeconds,
                CpuGETemperatureSeconds = data.CpuGETemperatureSeconds,
                CpuTemperature = data.CpuTemperature,
                CpuPerformance = data.CpuPerformance,
                IsRaiseHighCpuEvent = data.IsRaiseHighCpuEvent,
                HighCpuPercent = data.HighCpuPercent,
                HighCpuSeconds = data.HighCpuSeconds,
                GpuDriver = data.GpuDriver,
                GpuType = data.GpuType,
                OSName = data.OSName,
                OSVirtualMemoryMb = data.OSVirtualMemoryMb,
                TotalPhysicalMemoryMb = data.TotalPhysicalMemoryMb,
                GpuInfo = data.GpuInfo,
                Version = data.Version,
                IsMining = data.IsMining,
                BootOn = data.BootOn,
                MineStartedOn = data.MineStartedOn,
                MainCoinCode = data.MainCoinCode,
                MainCoinTotalShare = data.MainCoinTotalShare,
                MainCoinRejectShare = data.MainCoinRejectShare,
                MainCoinSpeed = data.MainCoinSpeed,
                MainCoinPool = data.MainCoinPool,
                MainCoinWallet = data.MainCoinWallet,
                Kernel = data.Kernel,
                IsDualCoinEnabled = data.IsDualCoinEnabled,
                DualCoinPool = data.DualCoinPool,
                DualCoinWallet = data.DualCoinWallet,
                DualCoinCode = data.DualCoinCode,
                DualCoinTotalShare = data.DualCoinTotalShare,
                DualCoinRejectShare = data.DualCoinRejectShare,
                DualCoinSpeed = data.DualCoinSpeed,
                KernelCommandLine = data.KernelCommandLine,
                GpuTable = data.GpuTable,
                MineWorkId = data.MineWorkId,
                MineWorkName = data.MineWorkName,
                WorkerName = data.WorkerName,
                DiskSpace = data.DiskSpace,
                MainCoinPoolDelay = data.MainCoinPoolDelay,
                DualCoinPoolDelay = data.DualCoinPoolDelay,
                IsFoundOneGpuShare = data.IsFoundOneGpuShare,
                IsRejectOneGpuShare = data.IsRejectOneGpuShare,
                IsGotOneIncorrectGpuShare = data.IsGotOneIncorrectGpuShare,
                KernelSelfRestartCount = data.KernelSelfRestartCount,
                LocalServerMessageTimestamp = data.LocalServerMessageTimestamp,
                LoginName = data.LoginName,
                IsOuterUserEnabled = data.IsOuterUserEnabled,
                OuterUserId = data.OuterUserId,
                ReportOuterUserId = data.ReportOuterUserId,
                NetActiveOn = data.NetActiveOn,
                IsOnline = data.IsOnline,
                IsDisableWAU = data.IsDisableWAU,
                IsDisableUAC = data.IsDisableUAC,
                AESPassword = data.AESPassword,
                AESPasswordOn = data.AESPasswordOn,
                Is1080PillEnabled = data.Is1080PillEnabled,
                IsDisableAntiSpyware = data.IsDisableAntiSpyware,
                IsAutoReboot = data.IsAutoReboot,
                IsAutoDisableWindowsFirewall = data.IsAutoDisableWindowsFirewall,
                MainCoinSpeedOn = data.MainCoinSpeedOn,
                DualCoinSpeedOn = data.DualCoinSpeedOn,
                DualCoinRejectPercent = data.DualCoinRejectPercent,
                MainCoinRejectPercent = data.MainCoinRejectPercent,
                MainCoinPoolDelayNumber = data.MainCoinPoolDelayNumber,
                DualCoinPoolDelayNumber = data.DualCoinPoolDelayNumber,
                IsPreventDisplaySleep = data.IsPreventDisplaySleep,
                IsLowSpeedRestartComputer = data.IsLowSpeedRestartComputer,
                LowSpeedRestartComputerMinutes = data.LowSpeedRestartComputerMinutes,
                LowSpeed = data.LowSpeed,
                IsLowSpeedReOverClock = data.IsLowSpeedReOverClock,
                LowSpeedReOverClockMinutes = data.LowSpeedReOverClockMinutes,
                OverClockLowSpeed = data.OverClockLowSpeed
                #endregion
            };
        }

        public static ClientData Create(ReportState state, string minerIp) {
            return new ClientData {
                Id = ObjectId.NewObjectId().ToString(),
                ClientId = state.ClientId,
                IsMining = state.IsMining,
                CreatedOn = DateTime.Now,
                MinerActiveOn = DateTime.Now,
                MinerIp = minerIp
            };
        }

        /// <summary>
        /// 从给定的speedData中提取出主币矿池延时，辅币矿池延时，主币拒绝率，辅币拒绝率。
        /// </summary>
        private static void Extract(
            ISpeedDto speedDto,
            out int mainCoinPoolDelayNumber,
            out int dualCoinPoolDelayNumber,
            out double mainCoinRejectPercent,
            out double dualCoinRejectPercent,
            out int diskSpaceMb) {
            #region
            mainCoinPoolDelayNumber = 0;
            dualCoinPoolDelayNumber = 0;
            mainCoinRejectPercent = 0.0;
            dualCoinRejectPercent = 0.0;
            if (!string.IsNullOrEmpty(speedDto.MainCoinPoolDelay)) {
                string text = speedDto.MainCoinPoolDelay.Trim();
                int count = 0;
                for (int i = 0; i < text.Length; i++) {
                    if (!char.IsNumber(text[i])) {
                        count = i;
                        break;
                    }
                }
                if (count != 0) {
                    mainCoinPoolDelayNumber = int.Parse(text.Substring(0, count));
                }
            }
            if (!string.IsNullOrEmpty(speedDto.DualCoinPoolDelay)) {
                string text = speedDto.DualCoinPoolDelay.Trim();
                int count = 0;
                for (int i = 0; i < text.Length; i++) {
                    if (!char.IsNumber(text[i])) {
                        count = i;
                        break;
                    }
                }
                if (count != 0) {
                    dualCoinPoolDelayNumber = int.Parse(text.Substring(0, count));
                }
            }
            if (speedDto.MainCoinTotalShare != 0) {
                mainCoinRejectPercent = (speedDto.MainCoinRejectShare * 100.0) / speedDto.MainCoinTotalShare;
            }
            if (speedDto.DualCoinTotalShare != 0) {
                dualCoinRejectPercent = (speedDto.DualCoinRejectShare * 100.0) / speedDto.DualCoinTotalShare;
            }
            diskSpaceMb = GetMinDiskSpaceMb(speedDto.DiskSpace);
            #endregion
        }

        private static int GetMinDiskSpaceMb(string diskSpace) {
            // C:\21.4 Gb;D:\9.2 Gb;E:\27.1 Gb
            if (string.IsNullOrEmpty(diskSpace)) {
                return 0;
            }
            string[] parts = diskSpace.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> list = new List<int>();
            foreach (string part in parts) {
                if (char.IsDigit(part[0])) {
                    if (double.TryParse(part.Substring(0, part.IndexOf(' ')), out double value)) {
                        list.Add((int)(value * LuckyKeyword.IntK));
                    }
                }
            }
            if (list.Count == 0) {
                return 0;
            }
            return list.Min();
        }

        public static ClientData Create(ISpeedDto speedDto, string minerIp) {
            Extract(
                speedDto,
                out int mainCoinPoolDelayNumber,
                out int dualCoinPoolDelayNumber,
                out double mainCoinRejectPercent,
                out double dualCoinRejectPercent,
                out int diskSpaceMb);
            return new ClientData() {
                #region
                Id = ObjectId.NewObjectId().ToString(),
                MineContextId = speedDto.MineContextId,
                MinerName = speedDto.MinerName,
                MinerIp = minerIp,
                CreatedOn = DateTime.Now,
                MinerActiveOn = DateTime.Now,
                GroupId = Guid.Empty,
                WorkId = Guid.Empty,// 这是服务端指定的作业，不受客户端的影响
                WindowsLoginName = string.Empty,
                WindowsPassword = string.Empty,
                MACAddress = speedDto.MACAddress,
                LocalIp = speedDto.LocalIp,
                ClientId = speedDto.ClientId,
                IsAutoBoot = speedDto.IsAutoBoot,
                IsAutoStart = speedDto.IsAutoStart,
                AutoStartDelaySeconds = speedDto.AutoStartDelaySeconds,
                IsAutoRestartKernel = speedDto.IsAutoRestartKernel,
                AutoRestartKernelTimes = speedDto.AutoRestartKernelTimes,
                IsNoShareRestartKernel = speedDto.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = speedDto.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = speedDto.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = speedDto.NoShareRestartComputerMinutes,
                IsPeriodicRestartKernel = speedDto.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = speedDto.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = speedDto.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = speedDto.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = speedDto.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = speedDto.PeriodicRestartKernelMinutes,
                IsAutoStopByCpu = speedDto.IsAutoStopByCpu,
                IsAutoStartByCpu = speedDto.IsAutoStartByCpu,
                CpuStopTemperature = speedDto.CpuStopTemperature,
                CpuStartTemperature = speedDto.CpuStartTemperature,
                CpuLETemperatureSeconds = speedDto.CpuLETemperatureSeconds,
                CpuGETemperatureSeconds = speedDto.CpuGETemperatureSeconds,
                CpuTemperature = speedDto.CpuTemperature,
                CpuPerformance = speedDto.CpuPerformance,
                IsRaiseHighCpuEvent = speedDto.IsRaiseHighCpuEvent,
                HighCpuPercent = speedDto.HighCpuPercent,
                HighCpuSeconds = speedDto.HighCpuSeconds,
                GpuDriver = speedDto.GpuDriver,
                GpuType = speedDto.GpuType,
                OSName = speedDto.OSName,
                OSVirtualMemoryMb = speedDto.OSVirtualMemoryMb,
                TotalPhysicalMemoryMb = speedDto.TotalPhysicalMemoryMb,
                GpuInfo = speedDto.GpuInfo,
                Version = speedDto.Version,
                IsMining = speedDto.IsMining,
                BootOn = speedDto.BootOn,
                MineStartedOn = speedDto.MineStartedOn,
                MainCoinCode = speedDto.MainCoinCode,
                MainCoinTotalShare = speedDto.MainCoinTotalShare,
                MainCoinRejectShare = speedDto.MainCoinRejectShare,
                MainCoinSpeed = speedDto.MainCoinSpeed,
                MainCoinPool = speedDto.MainCoinPool,
                MainCoinWallet = speedDto.MainCoinWallet,
                Kernel = speedDto.Kernel,
                IsDualCoinEnabled = speedDto.IsDualCoinEnabled,
                DualCoinPool = speedDto.DualCoinPool,
                DualCoinWallet = speedDto.DualCoinWallet,
                DualCoinCode = speedDto.DualCoinCode,
                DualCoinTotalShare = speedDto.DualCoinTotalShare,
                DualCoinRejectShare = speedDto.DualCoinRejectShare,
                DualCoinSpeed = speedDto.DualCoinSpeed,
                KernelCommandLine = speedDto.KernelCommandLine,
                MainCoinSpeedOn = speedDto.MainCoinSpeedOn,
                DualCoinSpeedOn = speedDto.DualCoinSpeedOn,
                GpuTable = speedDto.GpuTable,
                MineWorkId = speedDto.MineWorkId,
                MineWorkName = speedDto.MineWorkName,
                DiskSpace = speedDto.DiskSpace,
                MainCoinPoolDelay = speedDto.MainCoinPoolDelay,
                DualCoinPoolDelay = speedDto.DualCoinPoolDelay,
                IsFoundOneGpuShare = speedDto.IsFoundOneGpuShare,
                IsRejectOneGpuShare = speedDto.IsRejectOneGpuShare,
                IsGotOneIncorrectGpuShare = speedDto.IsGotOneIncorrectGpuShare,
                KernelSelfRestartCount = speedDto.KernelSelfRestartCount - 1,// 需要减1
                LocalServerMessageTimestamp = speedDto.LocalServerMessageTimestamp,
                AESPassword = string.Empty,
                AESPasswordOn = DateTime.MinValue,
                IsAutoDisableWindowsFirewall = speedDto.IsAutoDisableWindowsFirewall,
                IsDisableAntiSpyware = speedDto.IsDisableAntiSpyware,
                IsDisableUAC = speedDto.IsDisableUAC,
                IsDisableWAU = speedDto.IsDisableWAU,
                Is1080PillEnabled = speedDto.Is1080PillEnabled,
                IsAutoReboot = speedDto.IsAutoReboot,
                IsOnline = false,
                NetActiveOn = DateTime.MinValue,
                LoginName = string.Empty,
                IsOuterUserEnabled = speedDto.IsOuterUserEnabled,
                OuterUserId = speedDto.ReportOuterUserId,
                ReportOuterUserId = speedDto.ReportOuterUserId,
                WorkerName = string.Empty,
                DualCoinPoolDelayNumber = dualCoinPoolDelayNumber,
                MainCoinPoolDelayNumber = mainCoinPoolDelayNumber,
                MainCoinRejectPercent = mainCoinRejectPercent,
                DualCoinRejectPercent = dualCoinRejectPercent,
                DiskSpaceMb = diskSpaceMb,
                IsPreventDisplaySleep = speedDto.IsPreventDisplaySleep,
                IsLowSpeedRestartComputer = speedDto.IsLowSpeedRestartComputer,
                LowSpeedRestartComputerMinutes = speedDto.LowSpeedRestartComputerMinutes,
                LowSpeed = speedDto.LowSpeed,
                IsLowSpeedReOverClock = speedDto.IsLowSpeedReOverClock,
                LowSpeedReOverClockMinutes = speedDto.LowSpeedReOverClockMinutes,
                OverClockLowSpeed = speedDto.OverClockLowSpeed
                #endregion
            };
        }

        public void Update(MinerSign minerSign) {
            #region
            this.LoginName = minerSign.LoginName;
            this.ClientId = minerSign.ClientId;
            this.OuterUserId = minerSign.OuterUserId;
            this.AESPassword = minerSign.AESPassword;
            this.AESPasswordOn = minerSign.AESPasswordOn;
            #endregion
        }

        public SpeedData ToSpeedData() {
            return new SpeedData {
                #region
                SpeedOn = this.MinerActiveOn,
                AutoRestartKernelTimes = this.AutoRestartKernelTimes,
                AutoStartDelaySeconds = this.AutoStartDelaySeconds,
                BootOn = this.BootOn,
                ClientId = this.ClientId,
                CpuGETemperatureSeconds = this.CpuGETemperatureSeconds,
                CpuLETemperatureSeconds = this.CpuLETemperatureSeconds,
                CpuPerformance = this.CpuPerformance,
                CpuStartTemperature = this.CpuStartTemperature,
                CpuStopTemperature = this.CpuStopTemperature,
                CpuTemperature = this.CpuTemperature,
                DiskSpace = this.DiskSpace,
                DualCoinCode = this.DualCoinCode,
                DualCoinPool = this.DualCoinPool,
                DualCoinPoolDelay = this.DualCoinPoolDelay,
                DualCoinRejectShare = this.DualCoinRejectShare,
                DualCoinSpeed = this.DualCoinSpeed,
                DualCoinSpeedOn = this.DualCoinSpeedOn,
                DualCoinTotalShare = this.DualCoinTotalShare,
                DualCoinWallet = this.DualCoinWallet,
                GpuDriver = this.GpuDriver,
                GpuInfo = this.GpuInfo,
                GpuTable = this.GpuTable,
                GpuType = this.GpuType,
                HighCpuPercent = this.HighCpuPercent,
                HighCpuSeconds = this.HighCpuSeconds,
                IsAutoBoot = this.IsAutoBoot,
                IsAutoDisableWindowsFirewall = this.IsAutoDisableWindowsFirewall,
                IsAutoRestartKernel = this.IsAutoRestartKernel,
                IsAutoStart = this.IsAutoStart,
                IsAutoStartByCpu = this.IsAutoStartByCpu,
                IsAutoStopByCpu = this.IsAutoStopByCpu,
                IsDisableAntiSpyware = this.IsDisableAntiSpyware,
                IsDisableUAC = this.IsDisableUAC,
                IsDisableWAU = this.IsDisableWAU,
                Is1080PillEnabled = this.Is1080PillEnabled,
                IsAutoReboot = this.IsAutoReboot,
                IsDualCoinEnabled = this.IsDualCoinEnabled,
                IsFoundOneGpuShare = this.IsFoundOneGpuShare,
                IsGotOneIncorrectGpuShare = this.IsGotOneIncorrectGpuShare,
                IsMining = this.IsMining,
                IsNoShareRestartComputer = this.IsNoShareRestartComputer,
                IsNoShareRestartKernel = this.IsNoShareRestartKernel,
                IsOuterUserEnabled = this.IsOuterUserEnabled,
                ReportOuterUserId = this.ReportOuterUserId,
                IsPeriodicRestartComputer = this.IsPeriodicRestartComputer,
                IsPeriodicRestartKernel = this.IsPeriodicRestartKernel,
                IsRaiseHighCpuEvent = this.IsRaiseHighCpuEvent,
                IsRejectOneGpuShare = this.IsRejectOneGpuShare,
                Kernel = this.Kernel,
                KernelCommandLine = this.KernelCommandLine,
                KernelSelfRestartCount = this.KernelSelfRestartCount,
                LocalIp = this.LocalIp,
                LocalServerMessageTimestamp = this.LocalServerMessageTimestamp,
                MACAddress = this.MACAddress,
                MainCoinCode = this.MainCoinCode,
                MainCoinPool = this.MainCoinPool,
                MainCoinPoolDelay = this.MainCoinPoolDelay,
                MainCoinRejectShare = this.MainCoinRejectShare,
                MainCoinSpeed = this.MainCoinSpeed,
                MainCoinSpeedOn = this.MainCoinSpeedOn,
                MainCoinTotalShare = this.MainCoinTotalShare,
                MainCoinWallet = this.MainCoinWallet,
                MineContextId = this.MineContextId,
                MinerIp = this.MinerIp,
                MinerName = this.MinerName,
                MineStartedOn = this.MineStartedOn,
                MineWorkId = this.MineWorkId,
                MineWorkName = this.MineWorkName,
                NoShareRestartComputerMinutes = this.NoShareRestartComputerMinutes,
                NoShareRestartKernelMinutes = this.NoShareRestartKernelMinutes,
                OSName = this.OSName,
                OSVirtualMemoryMb = this.OSVirtualMemoryMb,
                PeriodicRestartComputerHours = this.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = this.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelHours = this.PeriodicRestartKernelHours,
                PeriodicRestartKernelMinutes = this.PeriodicRestartKernelMinutes,
                TotalPhysicalMemoryMb = this.TotalPhysicalMemoryMb,
                Version = this.Version,
                IsPreventDisplaySleep = this.IsPreventDisplaySleep,
                IsLowSpeedRestartComputer = this.IsLowSpeedRestartComputer,
                LowSpeedRestartComputerMinutes = this.LowSpeedRestartComputerMinutes,
                LowSpeed = this.LowSpeed,
                IsLowSpeedReOverClock = this.IsLowSpeedReOverClock,
                LowSpeedReOverClockMinutes = this.LowSpeedReOverClockMinutes,
                OverClockLowSpeed = this.OverClockLowSpeed
                #endregion
            };
        }

        /// <summary>
        /// 上报算力时。
        /// 因为只有MinerData具有的成员发生了变化时才需要持久化所以该非法输出isMinerDataChanged参数以表示MinerData的成员是否发生了变化。
        /// </summary>
        /// <param name="speedDto"></param>
        /// <param name="minerIp"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ISpeedDto speedDto, string minerIp, out bool isMinerDataChanged) {
            Update(speedDto, out isMinerDataChanged);
            this.MinerActiveOn = DateTime.Now;
            if (!isMinerDataChanged && minerIp != this.MinerIp) {
                isMinerDataChanged = true;
            }
            this.MinerIp = minerIp;
        }

        /// <summary>
        /// 因为只有MinerData具有的成员发生了变化时才需要持久化所以该非法输出isMinerDataChanged参数以表示MinerData的成员是否发生了变化。
        /// </summary>
        /// <param name="state"></param>
        /// <param name="minerIp"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ReportState state, string minerIp, out bool isMinerDataChanged) {
            isMinerDataChanged = false;
            this.IsMining = state.IsMining;
            this.MinerActiveOn = DateTime.Now;
            if (!isMinerDataChanged && minerIp != this.MinerIp) {
                isMinerDataChanged = true;
            }
            this.MinerIp = minerIp;
        }

        /// <summary>
        /// 上报算力时和拉取算力时。
        /// 因为只有MinerData具有的成员发生了变化时才需要持久化所以该非法输出isMinerDataChanged参数以表示MinerData的成员是否发生了变化。
        /// </summary>
        /// <param name="speedDto"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ISpeedData speedData, out bool isMinerDataChanged) {
            this.Update((ISpeedDto)speedData, out isMinerDataChanged);
            this.MinerActiveOn = speedData.SpeedOn;
        }

        /// <summary>
        /// 不更新MinerActiveOn
        /// </summary>
        /// <param name="speedDto"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ISpeedDto speedDto, out bool isMinerDataChanged) {
            #region
            isMinerDataChanged = false;
            if (speedDto == null) {
                return;
            }
            #region MinerData
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.ClientId != speedDto.ClientId;
            }
            this.ClientId = speedDto.ClientId;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.MACAddress != speedDto.MACAddress;
            }
            this.MACAddress = speedDto.MACAddress;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.LocalIp != speedDto.LocalIp;
            }
            this.LocalIp = speedDto.LocalIp;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.MinerName != speedDto.MinerName;
            }
            this.MinerName = speedDto.MinerName;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.IsOuterUserEnabled != speedDto.IsOuterUserEnabled;
            }
            this.IsOuterUserEnabled = speedDto.IsOuterUserEnabled;
            this.ReportOuterUserId = speedDto.ReportOuterUserId;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.OuterUserId != speedDto.ReportOuterUserId;
            }
            this.OuterUserId = speedDto.ReportOuterUserId;
            #endregion
            this.MineContextId = speedDto.MineContextId;
            this.IsAutoBoot = speedDto.IsAutoBoot;
            this.IsAutoStart = speedDto.IsAutoStart;
            this.AutoStartDelaySeconds = speedDto.AutoStartDelaySeconds;
            this.IsAutoRestartKernel = speedDto.IsAutoRestartKernel;
            this.AutoRestartKernelTimes = speedDto.AutoRestartKernelTimes;
            this.IsNoShareRestartKernel = speedDto.IsNoShareRestartKernel;
            this.NoShareRestartKernelMinutes = speedDto.NoShareRestartKernelMinutes;
            this.IsNoShareRestartComputer = speedDto.IsNoShareRestartComputer;
            this.NoShareRestartComputerMinutes = speedDto.NoShareRestartComputerMinutes;
            this.IsPeriodicRestartKernel = speedDto.IsPeriodicRestartKernel;
            this.PeriodicRestartKernelHours = speedDto.PeriodicRestartKernelHours;
            this.IsPeriodicRestartComputer = speedDto.IsPeriodicRestartComputer;
            this.PeriodicRestartComputerHours = speedDto.PeriodicRestartComputerHours;
            this.PeriodicRestartComputerMinutes = speedDto.PeriodicRestartComputerMinutes;
            this.PeriodicRestartKernelMinutes = speedDto.PeriodicRestartKernelMinutes;
            this.IsAutoStopByCpu = speedDto.IsAutoStopByCpu;
            this.IsAutoStartByCpu = speedDto.IsAutoStartByCpu;
            this.CpuStopTemperature = speedDto.CpuStopTemperature;
            this.CpuStartTemperature = speedDto.CpuStartTemperature;
            this.CpuLETemperatureSeconds = speedDto.CpuLETemperatureSeconds;
            this.CpuGETemperatureSeconds = speedDto.CpuGETemperatureSeconds;
            this.CpuPerformance = speedDto.CpuPerformance;
            this.CpuTemperature = speedDto.CpuTemperature;
            this.IsRaiseHighCpuEvent = speedDto.IsRaiseHighCpuEvent;
            this.HighCpuPercent = speedDto.HighCpuPercent;
            this.HighCpuSeconds = speedDto.HighCpuSeconds;
            this.GpuDriver = speedDto.GpuDriver;
            this.GpuType = speedDto.GpuType;
            this.OSName = speedDto.OSName;
            this.OSVirtualMemoryMb = speedDto.OSVirtualMemoryMb;
            this.GpuInfo = speedDto.GpuInfo;
            this.Version = speedDto.Version;
            this.IsMining = speedDto.IsMining;
            this.BootOn = speedDto.BootOn;
            this.MineStartedOn = speedDto.MineStartedOn;
            this.DiskSpace = speedDto.DiskSpace;
            this.MainCoinCode = speedDto.MainCoinCode;
            this.MainCoinTotalShare = speedDto.MainCoinTotalShare;
            this.MainCoinRejectShare = speedDto.MainCoinRejectShare;
            this.MainCoinSpeed = speedDto.MainCoinSpeed;
            this.MainCoinPool = speedDto.MainCoinPool;
            this.MainCoinWallet = speedDto.MainCoinWallet;
            this.Kernel = speedDto.Kernel;
            this.IsDualCoinEnabled = speedDto.IsDualCoinEnabled;
            this.DualCoinPool = speedDto.DualCoinPool;
            this.DualCoinWallet = speedDto.DualCoinWallet;
            this.DualCoinCode = speedDto.DualCoinCode;
            this.DualCoinTotalShare = speedDto.DualCoinTotalShare;
            this.DualCoinRejectShare = speedDto.DualCoinRejectShare;
            this.DualCoinSpeed = speedDto.DualCoinSpeed;
            this.KernelCommandLine = speedDto.KernelCommandLine;
            this.MainCoinSpeedOn = speedDto.MainCoinSpeedOn;
            this.DualCoinSpeedOn = speedDto.DualCoinSpeedOn;
            this.GpuTable = speedDto.GpuTable;
            this.MainCoinPoolDelay = speedDto.MainCoinPoolDelay;
            this.DualCoinPoolDelay = speedDto.DualCoinPoolDelay;
            this.IsFoundOneGpuShare = speedDto.IsFoundOneGpuShare;
            this.IsRejectOneGpuShare = speedDto.IsRejectOneGpuShare;
            this.IsGotOneIncorrectGpuShare = speedDto.IsGotOneIncorrectGpuShare;
            this.MineWorkId = speedDto.MineWorkId;
            this.MineWorkName = speedDto.MineWorkName;
            this.KernelSelfRestartCount = speedDto.KernelSelfRestartCount - 1;// 需要减1
            this.LocalServerMessageTimestamp = speedDto.LocalServerMessageTimestamp;
            this.TotalPhysicalMemoryMb = speedDto.TotalPhysicalMemoryMb;
            this.IsAutoDisableWindowsFirewall = speedDto.IsAutoDisableWindowsFirewall;
            this.IsDisableAntiSpyware = speedDto.IsDisableAntiSpyware;
            this.IsDisableUAC = speedDto.IsDisableUAC;
            this.IsDisableWAU = speedDto.IsDisableWAU;
            this.Is1080PillEnabled = speedDto.Is1080PillEnabled;
            this.IsPreventDisplaySleep = speedDto.IsPreventDisplaySleep;
            this.IsAutoReboot = speedDto.IsAutoReboot;
            this.IsLowSpeedRestartComputer = speedDto.IsLowSpeedRestartComputer;
            this.LowSpeedRestartComputerMinutes = speedDto.LowSpeedRestartComputerMinutes;
            this.LowSpeed = speedDto.LowSpeed;
            this.IsLowSpeedReOverClock = speedDto.IsLowSpeedReOverClock;
            this.LowSpeedReOverClockMinutes = speedDto.LowSpeedReOverClockMinutes;
            this.OverClockLowSpeed = speedDto.OverClockLowSpeed;
            Extract(
                speedDto,
                out int mainCoinPoolDelayNumber,
                out int dualCoinPoolDelayNumber,
                out double mainCoinRejectPercent,
                out double dualCoinRejectPercent,
                out int diskSpaceMb);
            this.MainCoinPoolDelayNumber = mainCoinPoolDelayNumber;
            this.DualCoinPoolDelayNumber = dualCoinPoolDelayNumber;
            this.MainCoinRejectPercent = mainCoinRejectPercent;
            this.DualCoinRejectPercent = dualCoinRejectPercent;
            this.DiskSpaceMb = diskSpaceMb;
            #endregion
        }

        public string Id { get; set; }

        /// <summary>
        /// 服务的指定的作业
        /// </summary>
        public Guid WorkId { get; set; }

        public string WorkerName { get; set; }

        public string WindowsLoginName { get; set; }

        private string _windowsPassword;
        public string WindowsPassword {
            get {
                return _windowsPassword;
            }
            set {
                if (!Base64Util.IsBase64OrEmpty(value)) {
                    value = string.Empty;
                }
                _windowsPassword = value;
            }
        }

        public DateTime CreatedOn { get; set; }

        public DateTime MinerActiveOn { get; set; }

        public Guid GroupId { get; set; }

        public DateTime NetActiveOn { get; set; }

        public DateTime GetActiveOn() {
            DateTime activeOn = this.MinerActiveOn;
            if (this.NetActiveOn > activeOn) {
                activeOn = this.NetActiveOn;
            }
            return activeOn;
        }

        public bool IsOnline { get; set; }

        public bool GetIsOnline(bool isOuterNet) {
            if (!IsOnline) {
                return false;
            }
            if (isOuterNet) {
                if (this.IsOuterUserEnabled) {
                    if (NetActiveOn.AddSeconds(60) < DateTime.Now) {
                        return false;
                    }
                }
                else if (NetActiveOn.AddSeconds(180) < DateTime.Now) {
                    return false;
                }
                return true;
            }
            if (NetActiveOn.AddSeconds(20) < DateTime.Now) {
                return false;
            }
            return true;
        }

        public string LoginName { get; set; }

        public string OuterUserId { get; set; }

        // 不会传到客户端
        [JsonIgnore]
        public string AESPassword { get; set; }

        public DateTime AESPasswordOn { get; set; }

        // 因为服务端需要根据如下几个字段排序所以会有这几个字段
        public int MainCoinPoolDelayNumber { get; set; }
        public int DualCoinPoolDelayNumber { get; set; }
        public double MainCoinRejectPercent { get; set; }
        public double DualCoinRejectPercent { get; set; }
        public int DiskSpaceMb { get; set; }
    }
}
