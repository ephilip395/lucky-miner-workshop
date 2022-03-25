using Lucky.Core;
using LuckyJsonRpc;
using System;
using StreamJsonRpc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Lucky.Mine
{
    class MiningDivertClient
    {
        private static IMiningDivert GetDivert()
        {
            TcpClient client = new TcpClient("localhost", LuckyKeyword.DivertControllPort);
            Stream stream = client.GetStream();
            return JsonRpc.Attach<IMiningDivert>(stream);
        }

        public static void StartOne(MiningParams mps)
        {

            IMiningDivert divert = GetDivert();
            divert.StartOne(mps);
            ((IDisposable)divert).Dispose();
        }


        public static void SwitchProxy(ProxyType proxyType)
        {

            IMiningDivert divert = GetDivert();
            divert.SwitchProxy(proxyType);
            ((IDisposable)divert).Dispose();

        }

        public static void UpdateUpstream(ProxyType proxyType, string endpoint, string username="", string password="")
        {

            IMiningDivert divert = GetDivert();
            divert.UpdateUpstream(proxyType, endpoint, username, password);
            ((IDisposable)divert).Dispose();

        }



    }
}
