using Lucky.Core;
using Lucky.Core.Profile;
using Lucky.Core.Profiles;
using Lucky.Gpus;
using Lucky.Mine;
using Lucky.ServerNode;
using System;
using System.Linq;

namespace Lucky.Report {
    public class ReportDataProvider : IReportDataProvider {
        public ReportDataProvider() {
            if (ClientAppType.IsMinerTweak) {
                VirtualRoot.BuildOncePath<HasBoot5SecondEvent>("登录服务器并报告一次0算力", LogEnum.DevConsole, pathId: PathId.Empty, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        // 报告0算力从而告知服务器该客户端当前在线的币种
                        //ReportSpeed();
                    });

                VirtualRoot.BuildEventPath<Per2MinuteEvent>("每两分钟上报一次", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        // 如果服务端通过Ws通道最近获取过算力就不用上报算力了，因为获取的算力会通过Mq走内网传播给这里上报的目的服务器，
                        // 而Daemon进程也会每2分钟周期走Ws通道上报一次算力，从而结果就是优先使用Ws通道上报算力，只要Ws通道在周期地上报
                        // 算力则就不会走Http通道上报算力了。
                        if (WsGetSpeedOn.AddSeconds(130) > message.BornOn) {
                            return;
                        }
                        //ReportSpeed();
                    });

                VirtualRoot.BuildEventPath<MineStartedEvent>("开始挖矿后报告状态", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        //ReportSpeed();
                    });

                VirtualRoot.BuildEventPath<MineStopedEvent>("停止挖矿后报告状态", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        //RpcRoot.OfficialServer.ReportService.ReportStateAsync(LuckyContext.Id, isMining: false);
                    });
            }
        }

        public DateTime WsGetSpeedOn { get; set; }

        private ICoin _sLastSpeedMainCoin;
        private ICoin _sLastSpeedDualCoin;
        public SpeedDto CreateSpeedDto() {
            ILuckyContext luckycmContext = LuckyContext.Instance;
            IWorkProfile workProfile = luckycmContext.MinerProfile;
            string localIps = VirtualRoot.FormatLocalIps(out string macAddress);
            Guid mineContextId = Guid.Empty;
            if (luckycmContext.CurrentMineContext != null) {
                mineContextId = luckycmContext.CurrentMineContext.Id;
            }
            SpeedDto speedDto = new SpeedDto {
                MineContextId = mineContextId,
                MainCoinSpeedOn = DateTime.MinValue,
                DualCoinSpeedOn = DateTime.MinValue,
                IsAutoDisableWindowsFirewall = workProfile.IsAutoDisableWindowsFirewall,
                IsDisableAntiSpyware = workProfile.IsDisableAntiSpyware,
                IsDisableUAC = workProfile.IsDisableUAC,
                IsDisableWAU = workProfile.IsDisableWAU,
                Is1080PillEnabled = workProfile.Is1080PillEnabled,
                IsPreventDisplaySleep = workProfile.IsPreventDisplaySleep,
                IsAutoReboot = workProfile.IsAutoReboot,
                LocalServerMessageTimestamp = VirtualRoot.LocalServerMessageSetTimestamp,
                KernelSelfRestartCount = 0,
                IsAutoBoot = workProfile.IsAutoBoot,
                IsAutoStart = workProfile.IsAutoStart,
                AutoStartDelaySeconds = workProfile.AutoStartDelaySeconds,
                Version = EntryAssemblyInfo.CurrentVersionStr,
                BootOn = luckycmContext.CreatedOn,
                MineStartedOn = null,
                IsMining = luckycmContext.IsMining,
                MineWorkId = Guid.Empty,
                MineWorkName = string.Empty,
                MinerName = workProfile.MinerName,
                GpuInfo = luckycmContext.GpuSetInfo,
                ClientId = LuckyContext.Id,
                MACAddress = macAddress,
                LocalIp = localIps,
                MainCoinCode = string.Empty,
                MainCoinWallet = string.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinSpeed = 0,
                DualCoinCode = string.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinSpeed = 0,
                DualCoinPool = string.Empty,
                DualCoinWallet = string.Empty,
                IsDualCoinEnabled = false,
                Kernel = string.Empty,
                MainCoinPool = string.Empty,
                OSName = Windows.OS.Instance.WindowsEdition,
                GpuDriver = luckycmContext.GpuSet.DriverVersion,
                GpuType = luckycmContext.GpuSet.GpuType,
                OSVirtualMemoryMb = VirtualRoot.DriveSet.OSVirtualMemoryMb,
                TotalPhysicalMemoryMb = (int)(Windows.Ram.Instance.TotalPhysicalMemory / LuckyKeyword.IntM),
                KernelCommandLine = string.Empty,
                DiskSpace = VirtualRoot.DriveSet.ToDiskSpaceString(),
                IsAutoRestartKernel = workProfile.IsAutoRestartKernel,
                AutoRestartKernelTimes = workProfile.AutoRestartKernelTimes,
                IsNoShareRestartKernel = workProfile.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = workProfile.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = workProfile.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = workProfile.NoShareRestartComputerMinutes,
                IsPeriodicRestartComputer = workProfile.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = workProfile.PeriodicRestartComputerHours,
                IsPeriodicRestartKernel = workProfile.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = workProfile.PeriodicRestartKernelHours,
                PeriodicRestartComputerMinutes = workProfile.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = workProfile.PeriodicRestartKernelMinutes,
                IsAutoStartByCpu = workProfile.IsAutoStartByCpu,
                IsAutoStopByCpu = workProfile.IsAutoStopByCpu,
                CpuGETemperatureSeconds = workProfile.CpuGETemperatureSeconds,
                CpuLETemperatureSeconds = workProfile.CpuLETemperatureSeconds,
                CpuStartTemperature = workProfile.CpuStartTemperature,
                CpuStopTemperature = workProfile.CpuStopTemperature,
                MainCoinPoolDelay = string.Empty,
                DualCoinPoolDelay = string.Empty,
                MinerIp = string.Empty,
                IsFoundOneGpuShare = false,
                IsGotOneIncorrectGpuShare = false,
                IsRejectOneGpuShare = false,
                CpuPerformance = luckycmContext.CpuPackage.Performance,
                CpuTemperature = luckycmContext.CpuPackage.Temperature,
                IsRaiseHighCpuEvent = workProfile.IsRaiseHighCpuEvent,
                HighCpuPercent = workProfile.HighCpuBaseline,
                HighCpuSeconds = workProfile.HighCpuSeconds,
                IsOuterUserEnabled = workProfile.IsOuterUserEnabled,
                ReportOuterUserId = LuckyRegistry.GetOuterUserId(),
                IsUseProxy = workProfile.IsUseProxy,
                ConnectionMethod = workProfile.ConnectionMethod,
                ProxyServerAddress = workProfile.ProxyServerAddress,
                ProxyServerPort = workProfile.ProxyServerPort,
                ProxyUsername = workProfile.ProxyUsername,
                ProxyPassword = workProfile.ProxyPassword,
                IsProxyPool = workProfile.IsProxyPool,
                IsProxyGroupMonitor = workProfile.IsProxyGroupMonitor,
                IsLowSpeedRestartComputer = false,
                LowSpeedRestartComputerMinutes = 0,
                LowSpeed = 0,
                IsLowSpeedReOverClock = false,
                LowSpeedReOverClockMinutes = 0,
                OverClockLowSpeed = 0,
                GpuTable = luckycmContext.GpusSpeed.AsEnumerable().Where(a => a.Gpu.Index != LuckyContext.GpuAllId).Select(a => a.ToGpuSpeedData()).ToArray()
            };
            if (workProfile.MineWork != null) {
                speedDto.MineWorkId = workProfile.MineWork.GetId();
                speedDto.MineWorkName = workProfile.MineWork.Name;
            }
            #region 当前选中的币种是什么
            if (luckycmContext.ServerContext.CoinSet.TryGetCoin(workProfile.CoinId, out ICoin mainCoin)) {
                speedDto.MainCoinCode = mainCoin.Code;
                ICoinProfile coinProfile = workProfile.GetCoinProfile(mainCoin.GetId());
                speedDto.MainCoinWallet = coinProfile.Wallet;
                speedDto.IsLowSpeedRestartComputer = coinProfile.IsLowSpeedRestartComputer;
                speedDto.LowSpeed = coinProfile.LowSpeed;
                speedDto.LowSpeedRestartComputerMinutes = coinProfile.LowSpeedRestartComputerMinutes;
                speedDto.IsLowSpeedReOverClock = coinProfile.IsLowSpeedReOverClock;
                speedDto.LowSpeedReOverClockMinutes = coinProfile.LowSpeedReOverClockMinutes;
                speedDto.OverClockLowSpeed = coinProfile.OverClockLowSpeed;
                if (luckycmContext.ServerContext.PoolSet.TryGetPool(coinProfile.PoolId, out IPool mainCoinPool)) {
                    speedDto.MainCoinPool = mainCoinPool.Server;
                    if (luckycmContext.IsMining) {
                        speedDto.MainCoinPoolDelay = luckycmContext.ServerContext.PoolSet.GetPoolDelayText(mainCoinPool.GetId(), isDual: false);
                    }
                    if (mainCoinPool.IsUserMode) {
                        IPoolProfile mainCoinPoolProfile = workProfile.GetPoolProfile(coinProfile.PoolId);
                        speedDto.MainCoinWallet = mainCoinPoolProfile.UserName;
                    }
                }
                else {
                    speedDto.MainCoinPool = string.Empty;
                }
                if (luckycmContext.ServerContext.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out ICoinKernel coinKernel)) {
                    if (luckycmContext.ServerContext.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                        speedDto.Kernel = kernel.GetFullName();
                        if (luckycmContext.ServerContext.KernelOutputSet.TryGetKernelOutput(kernel.KernelOutputId, out IKernelOutput kernelOutput)) {
                            speedDto.IsFoundOneGpuShare = kernelOutput.GetIsFoundOneGpuShare();
                            speedDto.IsGotOneIncorrectGpuShare = kernelOutput.GetIsGotOneIncorrectGpuShare();
                            speedDto.IsRejectOneGpuShare = kernelOutput.GetIsRejectOneGpuShare();
                        }
                        ICoinKernelProfile coinKernelProfile = workProfile.GetCoinKernelProfile(coinProfile.CoinKernelId);
                        speedDto.IsDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
                        if (coinKernelProfile.IsDualCoinEnabled) {
                            if (luckycmContext.ServerContext.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                                speedDto.DualCoinCode = dualCoin.Code;
                                ICoinProfile dualCoinProfile = workProfile.GetCoinProfile(dualCoin.GetId());
                                speedDto.DualCoinWallet = dualCoinProfile.DualCoinWallet;
                                if (luckycmContext.ServerContext.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                                    speedDto.DualCoinPool = dualCoinPool.Server;
                                    if (luckycmContext.IsMining) {
                                        speedDto.DualCoinPoolDelay = luckycmContext.ServerContext.PoolSet.GetPoolDelayText(dualCoinPool.GetId(), isDual: true);
                                    }
                                    if (dualCoinPool.IsUserMode) {
                                        IPoolProfile dualCoinPoolProfile = workProfile.GetPoolProfile(dualCoinProfile.DualCoinPoolId);
                                        speedDto.DualCoinWallet = dualCoinPoolProfile.UserName;
                                    }
                                }
                                else {
                                    speedDto.DualCoinPool = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (luckycmContext.IsMining) {
                var mineContext = luckycmContext.LockedMineContext;
                if (mineContext != null) {
                    speedDto.KernelSelfRestartCount = mineContext.KernelSelfRestartCount;
                    if (mineContext.MineStartedOn != DateTime.MinValue) {
                        speedDto.MineStartedOn = mineContext.MineStartedOn;
                    }
                    speedDto.KernelCommandLine = mineContext.CommandLine;
                }
                // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                if (_sLastSpeedMainCoin == null || _sLastSpeedMainCoin == luckycmContext.LockedMineContext.MainCoin) {
                    _sLastSpeedMainCoin = luckycmContext.LockedMineContext.MainCoin;
                    Guid coinId = luckycmContext.LockedMineContext.MainCoin.GetId();
                    IGpusSpeed gpuSpeeds = luckycmContext.GpusSpeed;
                    IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(LuckyContext.GpuAllId);
                    speedDto.MainCoinSpeed = totalSpeed.MainCoinSpeed.Value;
                    speedDto.MainCoinSpeedOn = totalSpeed.MainCoinSpeed.SpeedOn;
                    ICoinShare share = luckycmContext.CoinShareSet.GetOrCreate(coinId);
                    speedDto.MainCoinTotalShare = share.TotalShareCount;
                    speedDto.MainCoinRejectShare = share.RejectShareCount;
                }
                else {
                    _sLastSpeedMainCoin = luckycmContext.LockedMineContext.MainCoin;
                }
                if (luckycmContext.LockedMineContext is IDualMineContext dualMineContext) {
                    // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                    if (_sLastSpeedDualCoin == null || _sLastSpeedDualCoin == dualMineContext.DualCoin) {
                        _sLastSpeedDualCoin = dualMineContext.DualCoin;
                        Guid coinId = dualMineContext.DualCoin.GetId();
                        IGpusSpeed gpuSpeeds = luckycmContext.GpusSpeed;
                        IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(LuckyContext.GpuAllId);
                        speedDto.DualCoinSpeed = totalSpeed.DualCoinSpeed.Value;
                        speedDto.DualCoinSpeedOn = totalSpeed.DualCoinSpeed.SpeedOn;
                        ICoinShare share = luckycmContext.CoinShareSet.GetOrCreate(coinId);
                        speedDto.DualCoinTotalShare = share.TotalShareCount;
                        speedDto.DualCoinRejectShare = share.RejectShareCount;
                    }
                    else {
                        _sLastSpeedDualCoin = dualMineContext.DualCoin;
                    }
                }
            }
            return speedDto;
        }

        //private void ReportSpeed() {
        //    try {
        //        SpeedDto speedDto = CreateSpeedDto();
        //        RpcRoot.OfficialServer.ReportBinaryService.ReportSpeedAsync(speedDto, (response, e) => {
        //            if (response.IsSuccess()) {
        //                AppVersionChangedEvent.PublishIfNewVersion(response.ServerState.MinerTweakVersion);
        //                if (response.NewServerMessages.Count != 0) {
        //                    VirtualRoot.Execute(new ReceiveServerMessageCommand(response.NewServerMessages));
        //                }
        //                else {
        //                    VirtualRoot.Execute(new LoadNewServerMessageCommand(response.ServerState.MessageTimestamp));
        //                }
        //                VirtualRoot.Execute(new LoadKernelOutputKeywordCommand(response.ServerState.OutputKeywordTimestamp));
        //                if (response.ServerState.WsStatus == WsStatus.Online) {
        //                    VirtualRoot.RaiseEvent(new WsServerOkEvent());
        //                }
        //            }
        //            else {
        //                Logger.ErrorDebugLine(e);
        //            }
        //        });
        //    }
        //    catch (Exception e) {
        //        Logger.ErrorDebugLine(e);
        //    }
        //}
    }
}
