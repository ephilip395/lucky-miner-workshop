using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucky.Core;
using System;

namespace Lucky {
    [TestClass]
    public class SpeedTests {
        [TestMethod]
        public void Test1() {
            double speed = 170 * 1000 * 1000;
            Console.WriteLine(speed.ToNearSpeed(180));
        }
    }
}
