using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lucky {
    public partial interface IA {
        void A();
    }

    public partial interface IA {
        void A1();
    }

    [TestClass]
    public class PartialTests {
        [TestMethod]
        public void TestMethod1() {
        }
    }
}
