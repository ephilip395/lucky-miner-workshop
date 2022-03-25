using Lucky.Controllers;
using Lucky.Core;
using Lucky.Core.Daemon;
using Lucky.Core.Impl;
using Lucky.Core.MinerTweak;
using Lucky.Core.MinerServer;
using Lucky.Core.MinerMonitor;
using Lucky.JsonDb;
using Lucky.VirtualMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Lucky.MinerMonitor.Impl
{
    public class LocalMinerMonitorService : ILocalMinerMonitorService
    {
        private readonly string _daemonControllerName = ControllerUtil.GetControllerName<ILuckyDaemonController>();

        private readonly IClientDataSet _clientDataSet;

        public LocalMinerMonitorService()
        {
            _clientDataSet = new ClientDataSet();
        }

        #region AddClientsAsync
        public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback)
        {
            try
            {
                foreach (var clientIp in clientIps)
                {
                    ClientData clientData = _clientDataSet.AsEnumerable().FirstOrDefault(a => a.MinerIp == clientIp);
                    if (clientData != null)
                    {
                        continue;
                    }
                    _clientDataSet.AddClient(clientIp);
                }
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e)
            {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region RemoveClientsAsync
        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback)
        {
            try
            {
                foreach (var objectId in objectIds)
                {
                    _clientDataSet.RemoveByObjectId(objectId);
                }
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e)
            {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query)
        {
            try
            {
                var data = _clientDataSet.QueryClients(
                    user: null,
                    query,
                    out int total,
                    out CoinSnapshotData[] latestSnapshots,
                    out int totalOnlineCount,
                    out int totalMiningCount);
                VirtualRoot.RaiseEvent(new QueryClientsResponseEvent(QueryClientsResponse.Ok(data, total, latestSnapshots, totalMiningCount, totalOnlineCount)));
            }
            catch (Exception e)
            {
                VirtualRoot.RaiseEvent(new QueryClientsResponseEvent(ResponseBase.ServerError<QueryClientsResponse>(e.Message)));
            }
        }
        #endregion

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback)
        {
            try
            {
                _clientDataSet.UpdateClient(objectId, propertyName, value);
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e)
            {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback)
        {
            try
            {
                _clientDataSet.UpdateClients(propertyName, values);
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e)
            {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region EnableRemoteDesktopAsync
        public void EnableRemoteDesktopAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.EnableRemoteDesktop), null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetConsoleOutLinesAsync
        public void GetConsoleOutLinesAsync(IMinerData client, long afterTime)
        {
            RpcRoot.Client.MinerTweakService.GetConsoleOutLinesAsync(client.GetLocalIp(), afterTime, (data, e) =>
            {
                if (data != null && data.Count > 0)
                {
                    VirtualRoot.RaiseEvent(new ClientConsoleOutLinesEvent(client.ClientId, data));
                }
            });
        }
        public void FastGetConsoleOutLinesAsync(IMinerData client, long afterTime)
        {
            GetConsoleOutLinesAsync(client, afterTime);
        }
        #endregion

        #region GetLocalMessagesAsync
        public void GetLocalMessagesAsync(IMinerData client, long afterTime)
        {
            RpcRoot.Client.MinerTweakService.GetLocalMessagesAsync(client.GetLocalIp(), afterTime, (data, e) =>
            {
                if (data != null && data.Count > 0)
                {
                    VirtualRoot.RaiseEvent(new ClientLocalMessagesEvent(client.ClientId, data));
                }
            });
        }
        public void FastGetLocalMessagesAsync(IMinerData client, long afterTime)
        {
            GetLocalMessagesAsync(client, afterTime);
        }
        #endregion

        #region GetOperationResultsAsync
        public void GetOperationResultsAsync(IMinerData client, long afterTime)
        {
            RpcRoot.JsonRpc.GetAsync<List<OperationResultData>>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.GetOperationResults), new Dictionary<string, string>
            {
                ["afterTime"] = afterTime.ToString()
            }, (data, e) =>
            {
                if (data != null && data.Count > 0)
                {
                    VirtualRoot.RaiseEvent(new ClientOperationResultsEvent(client.ClientId, data));
                }
            }, timeountMilliseconds: 3000);
        }
        public void FastGetOperationResultsAsync(IMinerData client, long afterTime)
        {
            GetOperationResultsAsync(client, afterTime);
        }
        #endregion

        #region BlockWAUAsync
        public void BlockWAUAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.BlockWAU), null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region SwitchRadeonGpuAsync
        public void SwitchRadeonGpuAsync(IMinerData client, bool on)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.SwitchRadeonGpu), new Dictionary<string, string>
            {
                ["on"] = on.ToString()
            }, null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region RestartWindowsAsync
        public void RestartWindowsAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.RestartWindows), new object(), null, timeountMilliseconds: 3000);
        }
        #endregion

        #region ShutdownWindowsAsync
        public void ShutdownWindowsAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.ShutdownWindows), new object(), null, timeountMilliseconds: 3000);
        }
        #endregion

        #region UpgradeLuckyAsync
        // ReSharper disable once InconsistentNaming
        public void UpgradeLuckyAsync(IMinerData client, string luckycmFileName)
        {
            UpgradeLuckyRequest request = new UpgradeLuckyRequest
            {
                LuckyFileName = luckycmFileName
            };
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.UpgradeLucky), request, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region SetAutoBootStartAsync
        public void SetAutoBootStartAsync(IMinerData client, SetAutoBootStartRequest request)
        {
            RpcRoot.JsonRpc.FirePostAsync(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.SetAutoBootStart), new Dictionary<string, string>
            {
                ["autoBoot"] = request.AutoBoot.ToString(),
                ["autoStart"] = request.AutoStart.ToString()
            }, data: null, callback: null, timeountMilliseconds: 3000);
        }
        #endregion

        #region StartMineAsync
        public void StartMineAsync(IMinerData client, Guid workId)
        {
            string localJson = string.Empty, serverJson = string.Empty;
            if (workId != Guid.Empty)
            {
                localJson = MinerMonitorPath.ReadMineWorkLocalJsonFile(workId).Replace(LuckyKeyword.MinerNameParameterName, client.WorkerName);
                serverJson = MinerMonitorPath.ReadMineWorkServerJsonFile(workId);
            }
            WorkRequest request = new WorkRequest
            {
                WorkId = workId,
                WorkerName = client.WorkerName,
                LocalJson = localJson,
                ServerJson = serverJson
            };
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(
                client.GetLocalIp(), 
                LuckyKeyword.LuckyDaemonPort,
                _daemonControllerName, 
                nameof(ILuckyDaemonController.StartMine), 
                request, 
                null, 
                timeountMilliseconds: 3000);
        }
        #endregion

        #region StopMineAsync
        public void StopMineAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.StopMine), new object(), null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetDrivesAsync
        public void GetDrivesAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<List<DriveDto>>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.GetDrives), null, (data, e) =>
            {
                VirtualRoot.RaiseEvent(new GetDrivesResponsedEvent(client.ClientId, data));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SetVirtualMemoryAsync
        public void SetVirtualMemoryAsync(IMinerData client, Dictionary<string, int> data)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.SetVirtualMemory), new DataRequest<Dictionary<string, int>>
            {
                Data = data
            }, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetLocalIpsAsync
        public void GetLocalIpsAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<List<LocalIpDto>>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.GetLocalIps), null, (data, e) =>
            {
                VirtualRoot.RaiseEvent(new GetLocalIpsResponsedEvent(client.ClientId, data));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SetLocalIpsAsync
        public void SetLocalIpsAsync(IMinerData client, List<LocalIpInput> data)
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.SetLocalIps), new DataRequest<List<LocalIpInput>>
            {
                Data = data
            }, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetSelfWorkLocalJsonAsync
        public void GetSelfWorkLocalJsonAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<string>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.GetSelfWorkLocalJson), null, (json, e) =>
            {
                VirtualRoot.RaiseEvent(new GetSelfWorkLocalJsonResponsedEvent(client.ClientId, json));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SaveSelfWorkLocalJsonAsync
        public void SaveSelfWorkLocalJsonAsync(IMinerData client, string localJson, string serverJson)
        {
            if (string.IsNullOrEmpty(localJson) || string.IsNullOrEmpty(serverJson))
            {
                return;
            }
            WorkRequest request = new WorkRequest
            {
                WorkId = MineWorkData.SelfMineWorkId,
                WorkerName = client.WorkerName,
                LocalJson = localJson.Replace(LuckyKeyword.MinerNameParameterName, client.WorkerName),
                ServerJson = serverJson
            };
            RpcRoot.JsonRpc.FirePostAsync(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.SaveSelfWorkLocalJson), null, request, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetGpuProfilesJsonAsync
        public void GetGpuProfilesJsonAsync(IMinerData client)
        {
            RpcRoot.JsonRpc.PostAsync<string>(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.GetGpuProfilesJson), null, (json, e) =>
            {
                GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json) ?? new GpuProfilesJsonDb();
                VirtualRoot.RaiseEvent(new GetGpuProfilesResponsedEvent(client.ClientId, data));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SaveGpuProfilesJsonAsync
        public void SaveGpuProfilesJsonAsync(IMinerData client, string json)
        {
            HttpContent content = new StringContent(json);
            RpcRoot.HttpRpcHelper.FirePostAsync(client.GetLocalIp(), LuckyKeyword.LuckyDaemonPort, _daemonControllerName, nameof(ILuckyDaemonController.SaveGpuProfilesJson), null, content, null, timeountMilliseconds: 3000);
        }
        #endregion
    }
}
