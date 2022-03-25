using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyJsonRpc
{
    public interface IMiningDivert
    {
        void StartOne(MiningParams mps);

        void SwitchProxy(ProxyType proxyType);

        void UpdateUpstream(ProxyType proxyType, string endpoint, string username = "", string password = "");
       
    }
}
