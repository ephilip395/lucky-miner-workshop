using Lucky.Controllers;
using Lucky.Core.MinerServer;
using Lucky.Ws;
using System;
using System.Collections.Generic;

namespace Lucky.Services.Client {
    public class LuckyDaemonService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<ILuckyDaemonController>();
        internal LuckyDaemonService() { }

        public void GetSelfWorkLocalJsonAsync(IMinerData client, Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                client.GetLocalIp(),
                LuckyKeyword.LuckyDaemonPort,
                _controllerName,
                nameof(ILuckyDaemonController.GetSelfWorkLocalJson),
                callback,
                timeountMilliseconds: 3000);
        }

        #region Localhost
        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void CloseDaemonAsync(Action callback) {
            RpcRoot.JsonRpc.FirePostAsync(
                LuckyKeyword.Localhost,
                LuckyKeyword.LuckyDaemonPort,
                _controllerName,
                nameof(ILuckyDaemonController.CloseDaemon),
                null,
                data: null,
                callback,
                timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void StartOrStopWsAsync(bool isResetFailCount) {
            RpcRoot.JsonRpc.FirePostAsync(
                LuckyKeyword.Localhost,
                LuckyKeyword.LuckyDaemonPort,
                _controllerName,
                nameof(ILuckyDaemonController.StartOrCloseWs),
                new Dictionary<string, string> {
                    ["isResetFailCount"] = isResetFailCount.ToString()
                }, 
                data: null, 
                timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void GetWsDaemonStateAsync(Action<WsClientState, Exception> callback) {
            RpcRoot.JsonRpc.GetAsync(
                LuckyKeyword.Localhost, 
                LuckyKeyword.LuckyDaemonPort, 
                _controllerName, 
                nameof(ILuckyDaemonController.GetWsDaemonState), 
                null, 
                callback, 
                timeountMilliseconds: 3000);
        }

        public void ReClientIdAsync(LuckyAppType appType) {
            RpcRoot.JsonRpc.FirePostAsync(
                LuckyKeyword.Localhost,
                LuckyKeyword.LuckyDaemonPort,
                _controllerName,
                nameof(ILuckyDaemonController.ReClientId),
                new Dictionary<string, string> {
                    ["newClientId"] = LuckyRegistry.GetClientId(appType).ToString()
                },
                data: null,
                timeountMilliseconds: 3000);
        }
        #endregion
    }
}
