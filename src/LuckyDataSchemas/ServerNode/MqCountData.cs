﻿using System.Collections.Generic;

namespace Lucky.ServerNode {
    public class MqCountData {
        public MqCountData() {
            this.ReceivedCounts = new List<MqReceivedCountData>();
            this.SendCounts = new List<MqSendCountData>();
        }

        public List<MqReceivedCountData> ReceivedCounts { get; set; }
        public List<MqSendCountData> SendCounts { get; set; }
    }
}
