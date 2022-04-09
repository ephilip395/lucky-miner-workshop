using System.Linq;

namespace Lucky
{
    public static class LuckyKeyword
    {
        public const double DoubleK = 1024;
        public const int IntK = 1024;
        public const double DoubleM = 1024 * 1024;
        public const int IntM = 1024 * 1024;
        public const double DoubleG = 1024 * 1024 * 1024;
        public const ulong ULongG = 1024 * 1024 * 1024;
        public const long LongG = 1024 * 1024 * 1024;

        private const string _revision = "0";
        public const string Version = "1.1.2";
        public const string AssemblyVersion = Version + "." + _revision;
        public const string Lucky = "Lucky";
        public const string Copyright = "Copyright ©Lucky";
        public const string Company = "Lucky";

        public const string OfficialServerUrlBase = "https://cm.01orb.com";
        public const string OfficialWsServiceUrlBase = "wss://tm.01orb.com";
        //public const string OfficialServerUrlBase = "http://192.168.8.170:10000";
        //public const string OfficialWsServiceUrlBase = "ws://192.168.8.170:10001";
        public const string CloudFileDomain = "luckycm.oss-cn-beijing.aliyuncs.com";
        public const string CloudResUrlBase = "https://luckyres.oss-cn-beijing.aliyuncs.com";
        public const string CloudPackagesUrlBase = CloudResUrlBase + "/packages";
        public const string CloudToolsUrlBase = CloudResUrlBase + "/tools";

        public const string TempDirName = "Lucky";
        public const string RootLockFileName = "home.lock";
        public const string RootConfigFileName = "home.config";

        public const string LogsDirName = "Logs";
        public const string WebSocketSharpLogFileNameFormat = "websocket-sharp-{0}.log";
        public const string WebSocketSharpLogFileName = "websocket-sharp.log";
        public const string ServerJsonFileName = "server.json";
        public const string LocalJsonFileName = "local.json";
        public const string GpuProfilesFileName = "gpuProfiles.json";
        public const string PackagesDirName = "Packages";
        public const string UpdaterDirName = "Updater";
        public const string SelfWorkDirName = "SelfWork";
        public const string MineWorkDirName = "MineWork";
        public const string LuckyUpdaterFileName = "LuckyUpdater.exe";
        public const string ServerDbFileName = "server.litedb";
        public const string LocalDbFileName = "local.litedb";


        public const string WpfDesignOnly = "这是供WPF设计时使用的构造，不应在业务代码中被调用";

        public const string Localhost = "localhost";
        #region AppMutex
        public const string MinerTweakAppMutex = "luckycmclient";
        public const string MinerMonitorAppMutex = "luckycmcontrol";
        public const string MinerUpdaterAppMutex = "LuckyUpdaterAppMutex";
        public const string MinerFinderAppMutex = "MinerFinderAppMutex";
        #endregion

        public const string LuckyUpdaterProcessName = "LuckyUpdater";
        public const string MinerFinderProcessName = "MinerFinder";

        public const string HomeDirParameterName = "{家目录}";
        public const string TempDirParameterName = "{临时目录}";
        // MinerTweakPort和LuckyDaemonPort因为需要被内网群控访问所以需要约定固定的端口号
        public const int DefaultWebApiServerPort = 10000;
        public const int MinerTweakPort = 9001;
        public const int LuckyDaemonPort = 9002;
        public const int MinerMonitorPort = 9003;
        public const int DivertControllPort = 9005;
        public const string DNSServer0 = "39.106.182.178";
        public const string DNSServer1 = "8.141.173.238";
        public const string MinerFinderFileName = "MinerFinder.exe";
        public const string AtikmdagPatcherFileName = "AtikmdagPatcher.exe";
        public const string SwitchRadeonGpuFileName = "SwitchRadeonGpu.exe";
        public const string DevConsoleFileName = "DevConsole.exe";
        public const string LuckyDaemonFileName = "LuckyDaemon.exe";
        public const string LuckyNoDevFeeFileName = "LuckyNoDevFee.exe";
        public const string DivertProxyNuFileName = "DivertProxyNu.exe";


        public const string LuckyNoDevFeeKey = "Lucky.NoDevFee.LuckyNoDevFee.exe";
        public const string DivertProxyNuKey = "DivertProxyNu.DivertProxyNu.exe";
        public const string LuckyDaemonKey = "Lucky.Daemon.LuckyDaemon.exe";
        public const string MinerMonitorCmdParameterName = "--minerstudio";
        public const string AutoStartCmdParameterName = "--AutoStart";
        public const string UpgradeCmdParameterName = "upgrade=";
        public const string ActionCmdParameterName = "action=";

        public const int LocalMessageSetCapacity = 1000;
        public const int ServerMessageSetCapacity = 1000;
        // 矿工名中不可以包含的字符
        private static readonly char[] _invalidMinerNameChars = { '.', ' ', '-', '_' };
        public static string GetSafeMinerName(string rawMinerName)
        {
            if (string.IsNullOrEmpty(rawMinerName))
            {
                return "temp";
            }
            return new string(rawMinerName.ToCharArray().Where(a => !_invalidMinerNameChars.Contains(a)).ToArray());
        }
        // 因为界面上输入框不好体现输入的空格，所以这里对空格进行转义
        public const string SpaceKeyword = "space";

        // 如果没有使用分隔符分割序号的话无法表达两位数的序号，此时这种情况基本都是用ABCDEFGH……表达的后续的两位数
        private static readonly string[] IndexChars = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n" };
        public static string GetIndexChar(int index, string separator)
        {
            if (index <= 9 || !string.IsNullOrEmpty(separator))
            {
                return index.ToString();
            }
            return IndexChars[index - 10];
        }

        #region 目录名
        public const string DaemonDirName = "Daemon";
        public const string NoDevFeeDirName = "NoDevFee";
        public const string DivertProxyNuDirName = "DivertProxyNu";
        public const string CoinIconsDirName = "CoinIcons";
        public const string DownloadDirName = "Download";
        public const string KernelsDirName = "Kernels";
        public const string ToolsDirName = "Tools";
        #endregion

        #region URL
        public const string CoinIconsUrlPath = "https://" + CloudFileDomain + "/asset/image/coin-icon";
        #endregion

        #region 注册表
        public const string ClientIdRegistryKey = "ClientId";
        public const string NoDevFeeVersionRegistryKey = "NoDevFeeVersion";
        public const string DivertProxyNuVersionRegistryKey = "DivertProxyNuVersion";
        public const string IsOuterUserEnabledRegistryKey = "IsOuterUserEnabled";
        public const string OuterUserIdRegistryKey = "OuterUserId";
        public const string IsAdvParamsVisualRegistryKey = "IsAdvParamsVisual";
        public const string IsUseProxyRegistryKey = "IsUseProxy";
        public const string ConnectionMethodRegistryKey = "ConnectionMethod";
        public const string ProxyServerAddressRegistryKey = "ProxyServerAddress";
        public const string ProxyServerPortRegistryKey = "ProxyServerPort";
        public const string ProxyUsernameRegistryKey = "ProxyUsername";
        public const string ProxyPasswordRegistryKey = "ProxyPassword";
        public const string IsProxyPoolRegistryKey = "IsProxyPool";
        public const string IsProxyGroupMonitorRegistryKey = "IsProxyGroupMonitor";
        public const string DaemonVersionRegistryKey = "DaemonVersion";
        public const string LoginNameRegistryKey = "LoginName";
        public const string CurrentVersionTagRegistryKey = "CurrentVersionTag";
        public const string CurrentVersionRegistryKey = "CurrentVersion";
        public const string ArgumentsRegistryKey = "Arguments";
        public const string WorkTypeRegistryKey = "WorkType";
        public const string LocationRegistryKey = "Location";
        #endregion

        #region LocalAppSettingKey
        public const string UseDevicesAppSettingKey = "UseDevices";
        public const string UpdaterVersionAppSettingKey = "UpdaterVersion";
        public const string MinerFinderVersionAppSettingKey = "MinerFinderVersion";
        public const string AtikmdagPatcherVersionAppSettingKey = "AtikmdagPatcherVersion";
        public const string SwitchRadeonGpuVersionAppSettingKey = "SwitchRadeonGpuVersion";
        public const string ServerJsonVersionAppSettingKey = "ServerJsonVersion";
        #endregion

        #region ServerAppSettingKey
        public const string ColumnsShowIdAppSettingKey = "ColumnsShowId";
        public const string IsEnableVirtualizationAppSettingKey = "IsEnableVirtualization";
        public const string FrozenColumnCountAppSettingKey = "FrozenColumnCount";
        public const string MaxTempAppSettingKey = "MaxTemp";
        public const string MinTempAppSettingKey = "MinTemp";
        public const string RejectPercentAppSettingKey = "RejectPercent";
        public const string LuckyUpdaterFileNameAppSettingKey = "LuckyUpdaterFileName";
        public const string MinerFinderFileNameAppSettingKey = " MinerFinderFileName";
        public const string AtikmdagPatcherFileNameAppSettingKey = " AtikmdagPatcherFileName";
        public const string SwitchRadeonGpuFileNameAppSettingKey = " SwitchRadeonGpuFileName";
        #endregion

        #region 系统字典编码
        public const string KernelBrandSysDicCode = "KernelBrand";
        public const string PoolBrandSysDicCode = "PoolBrand";
        public const string AlgoSysDicCode = "Algo";
        public const string CudaVersionSysDicCode = "CudaVersion";
        public const string ThisSystemSysDicCode = "ThisSystem";
        public const string OsVmPerGpuSysDicItemCode = "OsVmPerGpu";
        #endregion

        #region 打在程序集中的定位关键字
        public const string KernelBrandId = "KernelBrandId";
        public const string PoolBrandId = "PoolBrandId";
        #endregion

        #region 正则表达式组名
        public const string TotalSpeedGroupName = "totalSpeed";
        public const string TotalSpeedUnitGroupName = "totalSpeedUnit";
        public const string TotalShareGroupName = "totalShare";
        public const string AcceptShareGroupName = "acceptShare";
        public const string RejectShareGroupName = "rejectShare";
        public const string IncorrectShareGroupName = "incorrectShare";
        public const string RejectPercentGroupName = "rejectPercent";
        public const string GpuIndexGroupName = "gpu";
        public const string GpuSpeedGroupName = "gpuSpeed";
        public const string GpuSpeedUnitGroupName = "gpuSpeedUnit";
        public const string PoolDelayGroupName = "poolDelay";
        #endregion

        public const string LogFileParameterName = "{logfile}";
        public const string MinerNameParameterName = "{{MinerName}}";

        #region 上下文变量名
        public const string MainCoinParameterName = "mainCoin";
        public const string UserNameParameterName = "userName";
        public const string PasswordParameterName = "password";
        public const string PasswordDefaultValue = "x";
        public const string WalletParameterName = "wallet";
        public const string HostParameterName = "host";
        public const string PortParameterName = "port";
        public const string PoolParameterName = "pool";
        public const string WorkerParameterName = "worker";
        public const string Worker1ParameterName = "worker1";
        public const string Host1ParameterName = "host1";
        public const string Port1ParameterName = "port1";
        public const string Pool1ParameterName = "pool1";
        public const string UserName1ParameterName = "userName1";
        public const string Password1ParameterName = "password1";
        public const string Wallet1ParameterName = "wallet1";
        public const string DualCoinParameterName = "dualCoin";
        public const string DualWalletParameterName = "dualWallet";
        public const string DualUserNameParameterName = "dualUserName";
        public const string DualPasswordParameterName = "dualPassword";
        public const string DualHostParameterName = "dualHost";
        public const string DualPortParameterName = "dualPort";
        public const string DualPoolParameterName = "dualPool";
        #endregion
    }
}
