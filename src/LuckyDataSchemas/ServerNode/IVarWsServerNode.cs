namespace Lucky.ServerNode {
    public interface IVarWsServerNode : IVarServerState {
        int MinerTweakWsSessionCount { get; }
        int MinerMonitorWsSessionCount { get; }
        int MinerTweakSessionCount { get; }
        int MinerMonitorSessionCount { get; }
    }
}
