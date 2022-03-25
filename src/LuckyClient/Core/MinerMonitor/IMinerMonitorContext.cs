using Lucky.AppSetting;

namespace Lucky.Core.MinerMonitor {
    public interface IMinerMonitorContext {
        IUserAppSettingSet UserAppSettingSet { get; }
        IMineWorkSet MineWorkSet { get; }
        IMinerGroupSet MinerGroupSet { get; }
        IColumnsShowSet ColumnsShowSet { get; }
        ILuckyWalletSet LuckyWalletSet { get; }
    }
}
