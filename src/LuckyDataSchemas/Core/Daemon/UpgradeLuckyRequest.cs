namespace Lucky.Core.Daemon
{
    public class UpgradeLuckyRequest : IRequest
    {
        public string LuckyFileName { get; set; }

        public UpgradeLuckyRequest() { }
    }
}
