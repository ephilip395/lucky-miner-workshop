using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;

namespace Lucky {
    [TestClass]
    public class NetTests {
        [TestMethod]
        public void ReadmeExample() {
            using (TcpClient tcpClient = new TcpClient("127.0.0.1", LuckyKeyword.LuckyDaemonPort)) {
                Assert.IsTrue(tcpClient.Connected, "该测试需要挖矿端守护进程已运行");
            }
        }

        [TestMethod]
        public void ServicePointTest() {
            Console.WriteLine(System.Net.ServicePointManager.DefaultConnectionLimit);
        }
    }
}
