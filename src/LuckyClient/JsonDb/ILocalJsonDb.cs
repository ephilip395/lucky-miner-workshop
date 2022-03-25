using Lucky.Core;
using Lucky.Core.MinerServer;
using Lucky.Core.Profile;

namespace Lucky.JsonDb {
    public interface ILocalJsonDb : IJsonDb {
        CoinKernelProfileData[] CoinKernelProfiles { get; }
        CoinProfileData[] CoinProfiles { get; }
        MinerProfileData MinerProfile { get; }
        MineWorkData MineWork { get; }
        PoolProfileData[] PoolProfiles { get; }
        PoolData[] Pools { get; }
        WalletData[] Wallets { get; }
    }
}