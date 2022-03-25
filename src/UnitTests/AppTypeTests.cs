using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lucky {
    [TestClass]
    public class AppTypeTests {
        [TestMethod]
        public void IsMinerTweakTest() {
            Assert.IsTrue(typeof(Daemon.DaemonUtil).Assembly.GetManifestResourceInfo(LuckyKeyword.LuckyDaemonKey) != null);
        }

        [TestMethod]
        public void IsMinerMonitorTest() {
            var assembly = typeof(MsRemoteDesktop).Assembly;
            Type type = assembly.GetType(typeof(MsRemoteDesktop).FullName);
            Assert.IsNotNull(type);
            type = assembly.GetType("Lucky.aaaaa");
            Assert.IsNull(type);
        }
    }
}
