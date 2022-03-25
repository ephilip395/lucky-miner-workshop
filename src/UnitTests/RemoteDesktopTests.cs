using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucky.RemoteDesktop;

namespace Lucky {
    [TestClass]
    public class RemoteDesktopTests {
        private static readonly object _locker = new object();
        [TestMethod]
        public void TestMethod1() {
            lock (_locker) {
                LuckyRegistry.SetIsRdpEnabled(true);
                Assert.IsTrue(LuckyRegistry.GetIsRdpEnabled());
                LuckyRegistry.SetIsRdpEnabled(false);
                Assert.IsFalse(LuckyRegistry.GetIsRdpEnabled());
            }
        }

        [TestMethod]
        public void EnableFirewallTest() {
            lock (_locker) {
                Firewall.EnableFirewall();
                FirewallStatus state = Firewall.Status(FirewallDomain.Domain);
                Assert.AreEqual(FirewallStatus.Enabled, state);
                state = Firewall.Status(FirewallDomain.Private);
                Assert.AreEqual(FirewallStatus.Enabled, state);
                state = Firewall.Status(FirewallDomain.Public);
                Assert.AreEqual(FirewallStatus.Enabled, state);
            }
        }

        [TestMethod]
        public void DisableFirewallTest() {
            lock (_locker) {
                Firewall.DisableFirewall();
                FirewallStatus state = Firewall.Status(FirewallDomain.Domain);
                Assert.AreEqual(FirewallStatus.Disabled, state);
                state = Firewall.Status(FirewallDomain.Private);
                Assert.AreEqual(FirewallStatus.Disabled, state);
                state = Firewall.Status(FirewallDomain.Public);
                Assert.AreEqual(FirewallStatus.Disabled, state);
            }
        }

        [TestMethod]
        public void RdpRuleTest() {
            lock (_locker) {
                Firewall.EnableFirewall();
                Firewall.AddRdpRule();
                Assert.IsTrue(Firewall.IsRdpRuleExists());
                Firewall.DisableFirewall();
                Assert.IsTrue(Firewall.IsRdpRuleExists());
                Firewall.RemoveRdpRule();
            }
        }

        [TestMethod]
        public void MinerTweakRuleTest() {
            lock (_locker) {
                Firewall.EnableFirewall();
                Firewall.AddMinerTweakRule();
                Assert.IsTrue(Firewall.IsMinerTweakRuleExists());
                Firewall.DisableFirewall();
                Assert.IsTrue(Firewall.IsMinerTweakRuleExists());
                Firewall.RemoveMinerTweakRule();
                Assert.IsFalse(Firewall.IsMinerTweakRuleExists());
            }
        }
    }
}
