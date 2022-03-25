using System;
using System.Collections.Generic;

namespace Lucky.MinerMonitor {
    /// <summary>
    /// MinerMonitor 的内网模式时使用的接口，内网模式只比外网模式多一个“添加矿机”的按钮。
    /// </summary>
    public interface ILocalMinerMonitorService : IMinerMonitorService {
        void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback);
    }
}
