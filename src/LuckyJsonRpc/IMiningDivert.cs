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

        void SwitchConnMethod(ConnMethod connMethod);

        void UpdateUpstream(ConnMethod connMethod, string endpoint, string username = "", string password = "");

    }
}
