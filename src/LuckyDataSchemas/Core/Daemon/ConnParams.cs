
namespace Lucky.Core.Daemon
{
    public class ConnParams
    {
        private int _connectionMethod;

        private string _proxyServerAddress;
        private int _proxyServerPort;
        private string _proxyUsername;
        private string _proxyPassword;

        public int ConnectionMethod
        {
            get => _connectionMethod;
            set
            {
                if (value < 1 || value > 3)
                {
                    _connectionMethod = 1;
                }
                else
                {
                    _connectionMethod = value;
                }
            }
        }


        public string ProxyServerAddress
        {
            get => _proxyServerAddress;
            set
            {
                if (value == null)
                {
                    _proxyServerAddress = "";
                }
                else
                {
                    _proxyServerAddress = value;
                }
            }
        }

        public int ProxyServerPort
        {
            get => _proxyServerPort;
            set
            {
                if (value < 1 || value > 65535)
                {
                    _proxyServerPort = 1080;
                }
                else
                {
                    _proxyServerPort = value;
                }
            }
        }
        public string ProxyUsername
        {
            get => _proxyUsername;
            set
            {
                if (value == null)
                {
                    _proxyUsername = "";
                }
                else
                {
                    _proxyUsername = value;
                }
            }
        }
        public string ProxyPassword
        {
            get => _proxyPassword;
            set
            {
                if (value == null)
                {
                    _proxyPassword = "";
                }
                else
                {
                    _proxyPassword = value;
                }
            }
        }
    }
}
