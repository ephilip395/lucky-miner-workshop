namespace Lucky.ServerNode {
    public interface IActionCount {
        string ActionName { get; }
        long Count { get; }
    }
}
