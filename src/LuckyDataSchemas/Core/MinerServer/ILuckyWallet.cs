using System;

namespace Lucky.Core.MinerServer {
    public interface ILuckyWallet : IDbEntity<Guid> {
        string CoinCode { get; }
        string Wallet { get; }
    }
}
