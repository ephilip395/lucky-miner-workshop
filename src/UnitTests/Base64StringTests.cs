﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucky.Cryptography;
using System;
using System.Linq;
using System.Text;

namespace Lucky {
    [TestClass]
    public class Base64StringTests {
        [TestMethod]
        public void Test1() {
            for (int i = 0; i < 10000; i++) {
                var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(base64String));
                Assert.IsTrue(base64String.All(a => Base64Util.IsBase64Char(a)));
            }
        }

        [TestMethod]
        public void Test2() {
            for (int i = 0; i < 100; i++) {
                var key = RSAUtil.GetRASKey();
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(key.PublicKey));
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(key.PrivateKey));
            }
        }

        [TestMethod]
        public void Test3() {
            for (int i = 0; i < 1000; i++) {
                string base64Str = Convert.ToBase64String(Encoding.UTF8.GetBytes(VirtualRoot.GetRandomString(i)));
                Console.WriteLine(base64Str);
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(base64Str));
            }
        }
    }
}
