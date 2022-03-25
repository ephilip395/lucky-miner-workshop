using Lucky.Controllers;
using Lucky.Core;
using Lucky.Core.MinerServer;
using Lucky.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Lucky.Services.Client {
    public class MinerTweakService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IMinerTweakController>();
        internal MinerTweakService() {
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void ShowMainWindowAsync(Action<bool, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                LuckyKeyword.Localhost,
                LuckyKeyword.MinerTweakPort,
                _controllerName,
                nameof(IMinerTweakController.ShowMainWindow),
                callback);
        }

        /// <summary>
        /// 本机同步网络调用
        /// </summary>
        public void CloseLuckyAsync(Action callback) {
            string location = LuckyRegistry.GetLocation(LuckyAppType.MinerTweak);
            if (string.IsNullOrEmpty(location) || !File.Exists(location)) {
                callback?.Invoke();
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(location);
            if (Process.GetProcessesByName(processName).Length == 0) {
                callback?.Invoke();
                return;
            }
            RpcRoot.JsonRpc.PostAsync(
                LuckyKeyword.Localhost,
                LuckyKeyword.MinerTweakPort,
                _controllerName,
                nameof(IMinerTweakController.CloseLucky),
                new object { },
                callback: (ResponseBase response, Exception e) => {
                    if (!response.IsSuccess()) {
                        try {
                            Windows.TaskKill.Kill(processName, waitForExit: true);
                        }
                        catch (Exception ex) {
                            Logger.ErrorDebugLine(ex);
                        }
                    }
                    callback?.Invoke();
                }, timeountMilliseconds: 2000);
        }

        public void GetSpeedAsync(IMinerData client, Action<SpeedDto, Exception> callback) {
            RpcRoot.JsonRpc.GetAsync(
                client.GetLocalIp(),
                LuckyKeyword.MinerTweakPort,
                _controllerName,
                nameof(IMinerTweakController.GetSpeed),
                null,
                callback,
                timeountMilliseconds: 4000);
        }

        public void WsGetSpeedAsync(Action<SpeedDto, Exception> callback) {
            RpcRoot.JsonRpc.GetAsync(
                LuckyKeyword.Localhost,
                LuckyKeyword.MinerTweakPort,
                _controllerName,
                nameof(IMinerTweakController.WsGetSpeed),
                null,
                callback,
                timeountMilliseconds: 3000);
        }

        public void GetConsoleOutLinesAsync(string clientIp, long afterTime, Action<List<ConsoleOutLine>, Exception> callback) {
            RpcRoot.JsonRpc.GetAsync(
                clientIp,
                LuckyKeyword.MinerTweakPort,
                _controllerName,
                nameof(IMinerTweakController.GetConsoleOutLines),
                new Dictionary<string, string> {
                    ["afterTime"] = afterTime.ToString()
                },
                callback,
                timeountMilliseconds: 3000);
        }

        public void GetLocalMessagesAsync(string clientIp, long afterTime, Action<List<LocalMessageDto>, Exception> callback) {
            RpcRoot.JsonRpc.GetAsync(
                clientIp,
                LuckyKeyword.MinerTweakPort,
                _controllerName,
                nameof(IMinerTweakController.GetLocalMessages),
                new Dictionary<string, string> {
                    ["afterTime"] = afterTime.ToString()
                },
                callback,
                timeountMilliseconds: 3000);
        }
    }
}
