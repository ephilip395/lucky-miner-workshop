﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace Lucky {
    [TestClass]
    public class ResourceDictionaryTests {
        [TestMethod]
        public void TestMethod1() {
            ResourceDictionary dic = new ResourceDictionary();
            Assert.IsNull(dic["aaa"]);
        }
    }
}
