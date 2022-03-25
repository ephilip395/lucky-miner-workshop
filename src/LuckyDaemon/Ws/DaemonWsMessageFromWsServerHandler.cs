﻿using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucky.Ws {
    /// <summary>
    /// 响应来自WsServer的消息。
    /// </summary>
    public static class DaemonWsMessageFromWsServerHandler {
        private static readonly Dictionary<string, Action<Action<WsMessage>, WsMessage>> _handlers = new Dictionary<string, Action<Action<WsMessage>, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
            [WsMessage.GetConsoleOutLines] = (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsLuckyOpened() && message.TryGetData(out long afterTime)) {
                    RpcRoot.Client.MinerTweakService.GetConsoleOutLinesAsync(LuckyKeyword.Localhost, afterTime, (data, e) => {
                        if (data != null && data.Count != 0) {
                            sendAsync(new WsMessage(message.Id, WsMessage.ConsoleOutLines) {
                                Data = data
                            });
                        }
                    });
                }
            },
            [WsMessage.GetLocalMessages] = (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsLuckyOpened() && message.TryGetData(out long afterTime)) {
                    RpcRoot.Client.MinerTweakService.GetLocalMessagesAsync(LuckyKeyword.Localhost, afterTime, (data, e) => {
                        if (data != null && data.Count != 0) {
                            sendAsync(new WsMessage(message.Id, WsMessage.LocalMessages) {
                                Data = data
                            });
                        }
                    });
                }
            },
            [WsMessage.GetDrives] = (sendAsync, message) => {
                sendAsync(new WsMessage(message.Id, WsMessage.Drives) {
                    Data = VirtualRoot.DriveSet.AsEnumerable().ToList()
                });
            },
            [WsMessage.GetLocalIps] = (sendAsync, message) => {
                sendAsync(new WsMessage(message.Id, WsMessage.LocalIps) {
                    Data = VirtualRoot.LocalIpSet.AsEnumerable().ToList()
                });
            },
            [WsMessage.GetOperationResults] = (sendAsync, message) => {
                if (message.TryGetData(out long afterTime)) {
                    var data = VirtualRoot.OperationResultSet.Gets(afterTime);
                    if (data != null && data.Count != 0) {
                        sendAsync(new WsMessage(message.Id, WsMessage.OperationResults) {
                            Data = data
                        });
                    }
                }
            },
            [WsMessage.GetSpeed] = (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsLuckyOpened()) {
                    RpcRoot.Client.MinerTweakService.WsGetSpeedAsync((data, ex) => {
                        sendAsync(new WsMessage(message.Id, WsMessage.Speed) {
                            Data = data
                        });
                    });
                }
            },
            [WsMessage.GetSelfWorkLocalJson] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    string json = VirtualRoot.DaemonOperation.GetSelfWorkLocalJson();
                    sendAsync(new WsMessage(message.Id, WsMessage.SelfWorkLocalJson) {
                        Data = json
                    });
                });
            },
            [WsMessage.SaveSelfWorkLocalJson] = (sendAsync, message) => {
                if (message.TryGetData(out WorkRequest workRequest)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SaveSelfWorkLocalJson(workRequest);
                    });
                }
            },
            [WsMessage.GetGpuProfilesJson] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    string json = VirtualRoot.DaemonOperation.GetGpuProfilesJson();
                    sendAsync(new WsMessage(message.Id, WsMessage.GpuProfilesJson) {
                        Data = json
                    });
                });
            },
            [WsMessage.SaveGpuProfilesJson] = (sendAsync, message) => {
                if (message.TryGetData(out string json)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SaveGpuProfilesJson(json);
                    });
                }
            },
            [WsMessage.SetAutoBootStart] = (sendAsync, message) => {
                if (message.TryGetData(out SetAutoBootStartRequest data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetAutoBootStart(data.AutoBoot, data.AutoStart);
                    });
                }
            },
            [WsMessage.EnableRemoteDesktop] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.EnableRemoteDesktop();
                });
            },
            [WsMessage.SetVirtualMemory] = (sendAsync, message) => {
                if (message.TryGetData(out Dictionary<string, int> data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetVirtualMemory(data);
                    });
                }
            },
            [WsMessage.SetLocalIps] = (sendAsync, message) => {
                if (message.TryGetData(out List<LocalIpInput> data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetLocalIps(data);
                    });
                }
            },
            [WsMessage.SwitchRadeonGpu] = (sendAsync, message) => {
                if (message.TryGetData(out bool on)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SwitchRadeonGpu(on);
                    });
                }
            },
            [WsMessage.BlockWAU] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.BlockWAU();
                });
            },
            [WsMessage.RestartWindows] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.RestartWindows();
                });
            },
            [WsMessage.ShutdownWindows] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.ShutdownWindows();
                });
            },
            [WsMessage.UpgradeLucky] = (sendAsync, message) => {
                if (message.TryGetData(out string luckycmFileName)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.UpgradeLucky(new UpgradeLuckyRequest {
                            LuckyFileName = luckycmFileName
                        });
                    });
                }
            },
            [WsMessage.StartMine] = (sendAsync, message) => {
                if (message.TryGetData(out WorkRequest request)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.StartMine(request);
                    });
                }
            },
            [WsMessage.StopMine] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.StopMine();
                });
            }
        };

        public static bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return _handlers.TryGetValue(messageType, out handler);
        }
    }
}
