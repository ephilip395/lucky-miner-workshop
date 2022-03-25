using Lucky.Controllers;
using System;
using System.Diagnostics;
using System.IO;

namespace Lucky.Services.Client
{
    public class MinerMonitorService
    {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IMinerMonitorController>();
        internal MinerMonitorService()
        {
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        /// <param name="callback"></param>
        public void ShowMainWindowAsync(Action<bool, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync<bool>(
                LuckyKeyword.Localhost,
                LuckyKeyword.MinerMonitorPort,
                _controllerName,
                nameof(IMinerMonitorController.ShowMainWindow),
                callback);
        }

        /// <summary>
        /// 本机同步网络调用
        /// </summary>
        public void CloseMinerMonitorAsync(Action callback)
        {
            string location = LuckyRegistry.GetLocation(LuckyAppType.MinerMonitor);
            if (string.IsNullOrEmpty(location) || !File.Exists(location))
            {
                callback?.Invoke();
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(location);
            if (Process.GetProcessesByName(processName).Length == 0)
            {
                callback?.Invoke();
                return;
            }
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(
                LuckyKeyword.Localhost,
                LuckyKeyword.MinerMonitorPort,
                _controllerName,
                nameof(IMinerMonitorController.CloseMinerMonitor),
                new object(),
                callback: (response, e) =>
                {
                    if (!response.IsSuccess())
                    {
                        try
                        {
                            Windows.TaskKill.Kill(processName, waitForExit: true);
                        }
                        catch (Exception ex)
                        {
                            Logger.ErrorDebugLine(ex);
                        }
                    }
                    callback?.Invoke();
                }, timeountMilliseconds: 2000);
        }
    }
}
