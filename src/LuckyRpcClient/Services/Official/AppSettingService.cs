using Lucky.Controllers;
using Lucky.Core;
using Lucky.Core.MinerServer;
using Lucky.ServerNode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Services.Official
{
    public class AppSettingService
    {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IAppSettingController>();

        internal AppSettingService()
        {
        }

        #region GetJsonFileVersionAsync
        public void GetJsonFileVersionAsync(LuckyAppType appType, string key, Action<ServerStateResponse> callback)
        {
            HashSet<string> macAddresses = new HashSet<string>();
            foreach (Core.MinerTweak.LocalIpDto localIp in VirtualRoot.LocalIpSet.AsEnumerable().ToArray())
            {
                _ = macAddresses.Add(localIp.MACAddress);
            }
            JsonFileVersionRequest request = new JsonFileVersionRequest
            {
                Key = key,
                ClientId = LuckyRegistry.GetClientId(appType),
                MACAddress = macAddresses.ToArray()
            };
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IAppSettingController.GetServerState),
                request,
                callback: (ServerStateResponse response, Exception e) =>
                {
                    if (e != null)
                    {
                        Logger.ErrorDebugLine(e);
                    }
                    if (response == null)
                    {
                        response = ServerStateResponse.Empty;
                        Logger.WarnWriteLine("询问服务器状态。");
                    }
                    if (response.NeedReClientId)
                    {
                        LuckyRegistry.ReClientId(ClientAppType.AppType);
                        RpcRoot.Client.LuckyDaemonService.ReClientIdAsync(appType);
                        Logger.InfoDebugLine("检测到本机标识存在重复，已重新生成");
                    }
                    callback?.Invoke(response);
                }, timeountMilliseconds: 10 * 1000);
        }
        #endregion

        #region SetAppSettingAsync
        public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase, Exception> callback)
        {
            DataRequest<AppSettingData> request = new DataRequest<AppSettingData>()
            {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAppSettingController.SetAppSetting),
                data: request,
                callback);
        }
        #endregion
    }
}
