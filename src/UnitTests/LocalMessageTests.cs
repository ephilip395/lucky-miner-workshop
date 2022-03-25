using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Lucky {
    [TestClass]
    public class LocalMessageTests {
        [TestMethod]
        public void BenchmarkTest() {
            VirtualRoot.Execute(new ClearLocalMessageSetCommand());
            int times = 2000;
            Assert.IsTrue(times > LuckyKeyword.LocalMessageSetCapacity);
            // 触发LocalMessageSet对AddLocalMessageCommand命令的订阅
            _ = LuckyContext.Instance.LocalMessageSet;
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.MyLocalInfo(nameof(LocalMessageTests), content);
            }
            Assert.AreEqual(LuckyKeyword.LocalMessageSetCapacity, LuckyContext.Instance.LocalMessageSet.AsEnumerable().Count());
        }
    }
}
