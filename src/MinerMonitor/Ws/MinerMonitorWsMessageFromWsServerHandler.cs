﻿using Lucky.Core;
using Lucky.Core.MinerTweak;
using Lucky.Core.MinerServer;
using Lucky.JsonDb;
using Lucky.MinerMonitor;
using Lucky.VirtualMemory;
using System;
using System.Collections.Generic;

namespace Lucky.Ws {
    /// <summary>
    /// 响应来自WsServer的消息，通常是将Ws消息翻译为Hub消息。
    /// </summary>
    public static class MinerMonitorWsMessageFromWsServerHandler {
        private static readonly Dictionary<string, Action<Action<WsMessage>, WsMessage>> _handlers = new Dictionary<string, Action<Action<WsMessage>, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
            [WsMessage.ServerTime] = (sendAsync, message) => {
                if (message.TryGetData(out long serverTime)) {
                    if (Math.Abs(serverTime - Timestamp.GetTimestamp()) > 30) {
                        VirtualRoot.Out.ShowWarn("您的电脑时间与网络时间不同步，请同步Windows的时间，不调整也不影响功能。");
                    }
                }
            },
            [WsMessage.ClientDatas] = (sendAsync, message) => {
                if (message.TryGetData(out QueryClientsResponse response)) {
                    VirtualRoot.RaiseEvent(new QueryClientsResponseEvent(response));
                }
            },
            [WsMessage.ConsoleOutLines] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<ConsoleOutLine> data)) {
                    VirtualRoot.RaiseEvent(new ClientConsoleOutLinesEvent(wrapperClientIdData.ClientId, data));
                }
            },
            [WsMessage.LocalMessages] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalMessageDto> data)) {
                    VirtualRoot.RaiseEvent(new ClientLocalMessagesEvent(wrapperClientIdData.ClientId, data));
                }
            },
            [WsMessage.OperationResults] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<OperationResultData> data)) {
                    VirtualRoot.RaiseEvent(new ClientOperationResultsEvent(wrapperClientIdData.ClientId, data));
                }
            },
            [WsMessage.Drives] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<DriveDto> data)) {
                    VirtualRoot.RaiseEvent(new GetDrivesResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            },
            [WsMessage.LocalIps] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalIpDto> data)) {
                    VirtualRoot.RaiseEvent(new GetLocalIpsResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            },
            [WsMessage.OperationReceived] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                    VirtualRoot.RaiseEvent(new ClientOperationReceivedEvent(wrapperClientId.ClientId));
                }
            },
            [WsMessage.SelfWorkLocalJson] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out string json)) {
                    VirtualRoot.RaiseEvent(new GetSelfWorkLocalJsonResponsedEvent(wrapperClientIdData.ClientId, json));
                }
            },
            [WsMessage.GpuProfilesJson] = (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out string json)) {
                    GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json) ?? new GpuProfilesJsonDb();
                    VirtualRoot.RaiseEvent(new GetGpuProfilesResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            }
        };

        public static bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return _handlers.TryGetValue(messageType, out handler);
        }
    }
}
