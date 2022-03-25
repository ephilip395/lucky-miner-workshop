using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lucky {
    [TestClass]
    public class EnumTests {
        [TestMethod]
        public void IsDefinedTest() {
            Assert.IsTrue(Enum.IsDefined(typeof(LuckyAppType), LuckyAppType.MinerTweak.ToString()));
            Assert.IsTrue(Enum.IsDefined(typeof(LuckyAppType), LuckyAppType.MinerTweak.GetName()));
            Assert.IsTrue(Enum.IsDefined(typeof(LuckyAppType), 0));
            Assert.IsFalse(Enum.IsDefined(typeof(LuckyAppType), 1000));
        }

        [TestMethod]
        public void ToStringTest() {
            Assert.AreEqual(nameof(LuckyAppType.MinerTweak), LuckyAppType.MinerTweak.ToString());
            Assert.AreEqual(nameof(LuckyAppType.MinerTweak), LuckyAppType.MinerTweak.GetName());
        }

        [TestMethod]
        public void BenchmarkTest() {
            int n = 100000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                LuckyAppType.MinerTweak.ToString();
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                LuckyAppType.MinerTweak.GetName();
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
