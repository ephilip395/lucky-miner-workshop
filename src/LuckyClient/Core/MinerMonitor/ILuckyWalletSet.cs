using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Core.MinerMonitor {
    public interface ILuckyWalletSet {
        bool TryGetLuckyWallet(Guid id, out ILuckyWallet luckycmWallet);
        IEnumerable<ILuckyWallet> AsEnumerable();
    }
}
