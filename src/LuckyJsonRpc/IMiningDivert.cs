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

        void SwitchConnMethod(ConnMethod proxyType);

        void UpdateUpstream(ConnMethod proxyType, string endpoint, string username = "", string password = "");
       
    }
}
