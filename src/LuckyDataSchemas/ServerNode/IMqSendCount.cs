namespace Lucky.ServerNode {
    public interface IMqSendCount {
        string RoutingKey { get; }
        long Count { get; }
    }
}
