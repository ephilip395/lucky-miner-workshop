using System;
using System.Reflection;

namespace Lucky
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class ClientAppType : Attribute
    {
        // IsMinerTweak和IsMinerMonitor具有排它性但不具有完备性，但该LuckyClient类库只被挖矿客户端和群控端使用所以也算是完备性的。
        public static LuckyAppType AppType { get; private set; }
        public static bool IsMinerTweak
        {
            get
            {
                return _isLuckyAppType && AppType == LuckyAppType.MinerTweak;
            }
        }
        public static bool IsMinerMonitor
        {
            get
            {
                return _isLuckyAppType && AppType == LuckyAppType.MinerMonitor;
            }
        }

        private static readonly bool _isLuckyAppType = false;
        static ClientAppType()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var clientAppType = assembly.GetCustomAttribute<ClientAppType>();
                if (clientAppType != null)
                {
                    _isLuckyAppType = true;
                    AppType = clientAppType.LuckyAppType;
                }
            }
        }

        public ClientAppType(LuckyAppType luckycmAppType)
        {
            this.LuckyAppType = luckycmAppType;
        }

        public LuckyAppType LuckyAppType { get; private set; }
    }
}
