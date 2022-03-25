using System;

namespace Lucky.MinerMonitor.Vms {
    public interface IWsStateViewModel {
        bool IsWsOnline { get; set; }
        string WsDescription { get; set; }
        int WsNextTrySecondsDelay { get; set; }
        DateTime WsLastTryOn { get; set; }
        bool IsConnecting { get; set; }
    }
}
