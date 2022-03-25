﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucky.Report;
using Lucky.Ws;
using System;
using System.IO;
using System.Text;

namespace Lucky {
    [TestClass]
    public class WsMessageTests {
        [TestMethod]
        public void Test1() {
            SpeedDto speedDto1 = VirtualRoot.JsonSerializer.Deserialize<SpeedDto>(File.ReadAllText(TestUtil.SpeedDataJsonFileFullName));
            WsMessage message = new WsMessage(Guid.NewGuid(), WsMessage.Speed) {
                Data = speedDto1
            };
            byte[] data = Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(message));
            double dataSize = data.Length / LuckyKeyword.DoubleK;
            Console.WriteLine($"原始大小 {dataSize.ToString()} kb");
            data = message.SignToBytes(HashUtil.Sha1("password1"));
            double dataNewSize = data.Length / LuckyKeyword.DoubleK;
            Assert.IsTrue(VirtualRoot.BinarySerializer.IsGZipped(data));
            Console.WriteLine($"序列化后大小 {dataNewSize.ToString()} kb，是原来大小的 {(dataNewSize * 100 / dataSize).ToString()} %");
            message = VirtualRoot.BinarySerializer.Deserialize<WsMessage>(data);
            Assert.IsTrue(message.TryGetData(out SpeedDto speedDto2));
            Assert.AreEqual(speedDto1.ClientId, speedDto2.ClientId);
        }
    }
}
