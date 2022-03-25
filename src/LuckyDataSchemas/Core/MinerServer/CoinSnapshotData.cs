﻿using LiteDB;
using System;

namespace Lucky.Core.MinerServer {
    public class CoinSnapshotData {
        public static CoinSnapshotData CreateEmpty(string coinCode) {
            return new CoinSnapshotData {
                Id = string.Empty,
                CoinCode = coinCode,
                Speed = 0,
                MainCoinMiningCount = 0,
                MainCoinOnlineCount = 0,
                Timestamp = DateTime.MinValue
            };
        }

        public CoinSnapshotData() {
            Id = ObjectId.NewObjectId().ToString();
        }

        [BsonId]
        public string Id { get; set; }
        public string CoinCode { get; set; }
        public double Speed { get; set; }
        public int MainCoinOnlineCount { get; set; }
        public int MainCoinMiningCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
