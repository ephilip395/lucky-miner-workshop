namespace Lucky.Services {
    using Client;

    public class ClientServices {
        public readonly MinerTweakService MinerTweakService = new MinerTweakService();
        public readonly LuckyDaemonService LuckyDaemonService = new LuckyDaemonService();
        public readonly MinerMonitorService MinerMonitorService = new MinerMonitorService();

        internal ClientServices() { }
    }
}
