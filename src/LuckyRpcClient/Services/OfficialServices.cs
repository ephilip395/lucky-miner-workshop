namespace Lucky.Services
{
    using Official;

    public class OfficialServices
    {
        public readonly WsServerNodeService WsServerNodeService = new WsServerNodeService();
        public readonly FileUrlService FileUrlService = new FileUrlService();
        public readonly LuckyFileService LuckyFileService = new LuckyFileService();
        public readonly OverClockDataService OverClockDataService = new OverClockDataService();
        public readonly LuckyWalletService LuckyWalletService = new LuckyWalletService();
        public readonly KernelOutputKeywordService KernelOutputKeywordService = new KernelOutputKeywordService();
        public readonly CalcConfigService CalcConfigService = new CalcConfigService();
        public readonly CalcConfigBinaryService CalcConfigBinaryService = new CalcConfigBinaryService();
        public readonly ServerMessageService ServerMessageService = new ServerMessageService();
        public readonly ServerMessageBinaryService ServerMessageBinaryService = new ServerMessageBinaryService();
        public readonly UserService UserService = new UserService();
        public readonly AppSettingService AppSettingService = new AppSettingService();
        public readonly UserAppSettingService UserAppSettingService = new UserAppSettingService();
        public readonly ClientDataService ClientDataService = new ClientDataService();
        public readonly UserMinerGroupService UserMinerGroupService = new UserMinerGroupService();
        public readonly UserMineWorkService UserMineWorkService = new UserMineWorkService();
        public readonly AdminService AdminService = new AdminService();

        internal OfficialServices()
        {
        }
    }
}
