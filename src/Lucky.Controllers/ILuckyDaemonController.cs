using Lucky.Core;
using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.VirtualMemory;
using Lucky.Ws;
using System;
using System.Collections.Generic;

namespace Lucky.Controllers {
    public interface ILuckyDaemonController {
        DataResponse<IntPtr> ShowConsole();
        WsClientState GetWsDaemonState();
        void ReClientId(Guid clientId);
        ResponseBase EnableRemoteDesktop();
        ResponseBase BlockWAU();
        ResponseBase SwitchRadeonGpu(bool on);
        void CloseDaemon();
        string GetSelfWorkLocalJson();
        void SaveSelfWorkLocalJson(WorkRequest request);
        string GetGpuProfilesJson();
        void SaveGpuProfilesJson();
        // TODO:应该新增一个Action用于处理配置合集
        void SetAutoBootStart(bool autoBoot, bool autoStart);
        void StartOrCloseWs(bool isResetFailCount);
        ResponseBase RestartWindows(object request);
        ResponseBase ShutdownWindows(object request);
        ResponseBase UpgradeLucky(UpgradeLuckyRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(object request);
        List<OperationResultDto> GetOperationResults(long afterTime);
        List<DriveDto> GetDrives();
        ResponseBase SetVirtualMemory(DataRequest<Dictionary<string, int>> request);
        List<LocalIpDto> GetLocalIps();
        ResponseBase SetLocalIps(DataRequest<List<LocalIpInput>> request);
    }
}
