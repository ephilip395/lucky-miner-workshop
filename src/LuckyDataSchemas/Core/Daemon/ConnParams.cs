
namespace Lucky.Core.Daemon
{
    public class ConnParams
    {
        public int ConnectionMethod { get; set; }
        public string ProxyServerAddress { get; set; }
        public int ProxyServerPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
    }
}
