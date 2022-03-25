using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IDaemonOperation {
        bool IsLuckyOpened();
        void CloseDaemon();
        ResponseBase EnableRemoteDesktop();
        ResponseBase BlockWAU();
        ResponseBase SwitchRadeonGpu(bool on);
        string GetSelfWorkLocalJson();
        bool SaveSelfWorkLocalJson(WorkRequest request);
        string GetGpuProfilesJson();
        ResponseBase RestartWindows();
        bool SaveGpuProfilesJson(string json);
        bool SetAutoBootStart(bool autoBoot, bool autoStart);
        ResponseBase ShutdownWindows();
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine();
        ResponseBase UpgradeLucky(UpgradeLuckyRequest request);
        ResponseBase SetVirtualMemory(Dictionary<string, int> data);
        ResponseBase SetLocalIps(List<LocalIpInput> data);
    }
}