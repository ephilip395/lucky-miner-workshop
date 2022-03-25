using System.Collections.Generic;

namespace Lucky.ServerNode {
    public interface IWebApiServerState : IServerState {
        List<WsServerNodeState> WsServerNodes { get; }
        int CaptchaCount { get; }
    }
}
