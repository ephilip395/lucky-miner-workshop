namespace Lucky.Core.MinerMonitor.Impl {
    public class MinerMonitorContext : IMinerMonitorContext {
        public MinerMonitorContext() {
            this.UserAppSettingSet = new UserAppSettingSet();
            this.MineWorkSet = new MineWorkSet();
            this.MinerGroupSet = new MinerGroupSet();
            this.ColumnsShowSet = new ColumnsShowSet();
            this.LuckyWalletSet = new LuckyWalletSet();
        }

        public IUserAppSettingSet UserAppSettingSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public ILuckyWalletSet LuckyWalletSet { get; private set; }
    }
}
