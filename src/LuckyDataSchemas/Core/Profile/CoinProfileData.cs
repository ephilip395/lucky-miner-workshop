﻿using LiteDB;
using System;

namespace Lucky.Core.Profile {
    public class CoinProfileData : ICoinProfile, IProfile, IDbEntity<Guid> {
        public CoinProfileData() {
            LowSpeedRestartComputerMinutes = 5;
        }

        public static CoinProfileData CreateDefaultData(Guid coinId, Guid poolId, string wallet, Guid coinKernelId) {
            return new CoinProfileData() {
                PoolId = poolId,
                PoolId1 = default,
                Wallet = wallet,
                CoinId = coinId,
                CoinKernelId = coinKernelId,
                IsHideWallet = default,
                DualCoinPoolId = default,
                DualCoinWallet = string.Empty,
                IsDualCoinHideWallet = default,
                CalcInput = 1,
                IsLowSpeedRestartComputer = false,
                LowSpeed = 0,
                LowSpeedRestartComputerMinutes = 10,
                IsLowSpeedReOverClock = false,
                LowSpeedReOverClockMinutes = 10,
                OverClockLowSpeed = 0
            };
        }

        public Guid GetId() {
            return this.CoinId;
        }

        [BsonId]
        public Guid CoinId { get; set; }

        public Guid PoolId { get; set; }
        public Guid PoolId1 { get; set; }
        public string Wallet { get; set; }
        public bool IsHideWallet { get; set; }
        public Guid CoinKernelId { get; set; }
        public Guid DualCoinPoolId { get; set; }
        public string DualCoinWallet { get; set; }
        public bool IsDualCoinHideWallet { get; set; }

        public double CalcInput { get; set; }

        public bool IsLowSpeedRestartComputer { get; set; }

        public int LowSpeedRestartComputerMinutes { get; set; }

        public double LowSpeed { get; set; }

        public bool IsLowSpeedReOverClock { get; set; }

        public int LowSpeedReOverClockMinutes { get; set; }

        public double OverClockLowSpeed { get; set; }

        // 检测内存状态是否变更时使用
        public override string ToString() {
            return this.BuildSign().ToString();
        }
    }
}
