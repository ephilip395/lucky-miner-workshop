using Lucky.Core;
using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.Core.MinerServer;
using Lucky.Ws;
using System;
using System.Collections.Generic;

namespace Lucky.MinerMonitor.Impl
{
    public class ServerMinerMonitorService : IServerMinerMonitorService
    {
        internal ServerMinerMonitorService()
        {
        }

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                // 如果是刚刚登录，Ws连接可能正在连接中还没连上了就不用提示和服务器失去连接了。
                if ((DateTime.Now - RpcRoot.LoginedOn).Seconds < 2)
                {
                    return;
                }
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.QueryClientDatas)
            {
                Data = query
            });
        }
        #endregion

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback)
        {
            RpcRoot.OfficialServer.ClientDataService.UpdateClientAsync(objectId, propertyName, value, callback);
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback)
        {
            RpcRoot.OfficialServer.ClientDataService.UpdateClientsAsync(propertyName, values, callback);
        }
        #endregion

        #region RemoveClientsAsync
        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback)
        {
            RpcRoot.OfficialServer.ClientDataService.RemoveClientsAsync(objectIds, callback);
        }
        #endregion

        #region EnableRemoteDesktopAsync
        public void EnableRemoteDesktopAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.EnableRemoteDesktop)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region GetConsoleOutLinesAsync
        public void GetConsoleOutLinesAsync(IMinerData client, long afterTime)
        {
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetConsoleOutLines)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        public void FastGetConsoleOutLinesAsync(IMinerData client, long afterTime)
        {
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.FastGetConsoleOutLines)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        #endregion

        #region GetLocalMessagesAsync
        public void GetLocalMessagesAsync(IMinerData client, long afterTime)
        {
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetLocalMessages)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        public void FastGetLocalMessagesAsync(IMinerData client, long afterTime)
        {
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.FastGetLocalMessages)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        #endregion

        #region GetOperationResultsAsync
        public void GetOperationResultsAsync(IMinerData client, long afterTime)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetOperationResults)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        public void FastGetOperationResultsAsync(IMinerData client, long afterTime)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.FastGetOperationResults)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        #endregion

        #region BlockWAUAsync
        public void BlockWAUAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.BlockWAU)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SwitchRadeonGpuAsync
        public void SwitchRadeonGpuAsync(IMinerData client, bool on)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SwitchRadeonGpu)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = on
                }
            });
        }
        #endregion

        #region RestartWindowsAsync
        public void RestartWindowsAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.RestartWindows)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region ShutdownWindowsAsync
        public void ShutdownWindowsAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ShutdownWindows)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region UpgradeLuckyAsync
        // ReSharper disable once InconsistentNaming
        public void UpgradeLuckyAsync(IMinerData client, string luckycmFileName)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.UpgradeLucky)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = luckycmFileName
                }
            });
        }
        #endregion

        #region SetAutoBootStartAsync
        public void SetAutoBootStartAsync(IMinerData client, SetAutoBootStartRequest request)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SetAutoBootStart)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = request
                }
            });
        }
        #endregion

        #region UpdateConnParamsAsync
        public void UpdateConnParamsAsync(IMinerData client, ConnParams connParams)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.UpdateConnParams)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = connParams
                }
            });
        }
        #endregion

        #region StartMineAsync
        public void StartMineAsync(IMinerData client, Guid workId)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            // localJson和serverJson在服务端将消息通过ws通道发送给挖矿端前根据workId填充
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.StartMine)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = workId
                }
            });
        }
        #endregion

        #region StopMineAsync
        public void StopMineAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.StopMine)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region GetDrivesAsync
        public void GetDrivesAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetDrives)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SetVirtualMemoryAsync
        public void SetVirtualMemoryAsync(IMinerData client, Dictionary<string, int> data)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SetVirtualMemory)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = data
                }
            });
        }
        #endregion

        #region GetLocalIpsAsync
        public void GetLocalIpsAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetLocalIps)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SetLocalIpsAsync
        public void SetLocalIpsAsync(IMinerData client, List<LocalIpInput> data)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SetLocalIps)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = data
                }
            });
        }
        #endregion

        #region GetSelfWorkLocalJsonAsync
        public void GetSelfWorkLocalJsonAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetSelfWorkLocalJson)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SaveSelfWorkLocalJsonAsync
        public void SaveSelfWorkLocalJsonAsync(IMinerData client, string localJson, string serverJson)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
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
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SaveSelfWorkLocalJson)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = request
                }
            });
        }
        #endregion

        #region GetGpuProfilesJsonAsync
        public void GetGpuProfilesJsonAsync(IMinerData client)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetGpuProfilesJson)
            {
                Data = new WrapperClientId
                {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SaveGpuProfilesJsonAsync
        public void SaveGpuProfilesJsonAsync(IMinerData client, string json)
        {
            if (!MinerMonitorRoot.WsClient.IsOpen)
            {
                return;
            }
            MinerMonitorRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SaveGpuProfilesJson)
            {
                Data = new WrapperClientIdData
                {
                    ClientId = client.ClientId,
                    Data = json
                }
            });
        }
        #endregion
    }
}
