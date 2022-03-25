using System.IO;

namespace Lucky
{
    public static partial class MinerTweakTempPath
    {
        static MinerTweakTempPath()
        {
            string daemonDirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.DaemonDirName);
            string noDevFeeDirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.NoDevFeeDirName);
            string divertProxyNuDirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.DivertProxyNuDirName);
            if (!Directory.Exists(daemonDirFullName))
            {
                Directory.CreateDirectory(daemonDirFullName);
            }
            if (!Directory.Exists(noDevFeeDirFullName))
            {
                Directory.CreateDirectory(noDevFeeDirFullName);
            }
            if (!Directory.Exists(divertProxyNuDirFullName))
            {
                Directory.CreateDirectory(divertProxyNuDirFullName);
            }
            DaemonFileFullName = Path.Combine(daemonDirFullName, LuckyKeyword.LuckyDaemonFileName);
            NoDevFeeFileFullName = Path.Combine(noDevFeeDirFullName, LuckyKeyword.LuckyNoDevFeeFileName);
            DevConsoleFileFullName = Path.Combine(daemonDirFullName, LuckyKeyword.DevConsoleFileName);
            DivertProxyNuFileFullName = Path.Combine(divertProxyNuDirFullName, LuckyKeyword.DivertProxyNuFileName);
            Upgrade();
        }

        public static string GetIconFileFullName(string coinIcon)
        {
            if (string.IsNullOrEmpty(coinIcon))
            {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(CoinIconsDirFullName, coinIcon);
            return iconFileFullName;
        }

        public static readonly string DaemonFileFullName;
        public static readonly string NoDevFeeFileFullName;

        public static readonly string DevConsoleFileFullName;
        public static readonly string DivertProxyNuFileFullName;

        private static bool _sIsFirstCallCoinIconDirFullName = true;
        public static string CoinIconsDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.CoinIconsDirName);
                if (_sIsFirstCallCoinIconDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallCoinIconDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallDownloadDirFullName = true;
        public static string DownloadDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.DownloadDirName);
                if (_sIsFirstCallDownloadDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallDownloadDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallKernelsDirFullName = true;
        public static string KernelsDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.KernelsDirName);
                if (_sIsFirstCallKernelsDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallKernelsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallToolsDirFullName = true;
        /// <summary>
        /// 在临时目录。因为工具并非常用的程序文件，在用户第一次使用时才会下载。
        /// </summary>
        public static string ToolsDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.ToolsDirName);
                if (_sIsFirstCallToolsDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallToolsDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string MinerFinderFileFullName
        {
            get
            {
                string dir = Path.Combine(ToolsDirFullName, "MinerFinder");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return Path.Combine(dir, LuckyKeyword.MinerFinderFileName);
            }
        }

        public static string AtikmdagPatcherFileFullName
        {
            get
            {
                string dir = Path.Combine(ToolsDirFullName, "AtikmdagPatcher");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return Path.Combine(dir, LuckyKeyword.AtikmdagPatcherFileName);
            }
        }

        public static string SwitchRadeonGpuFileFullName
        {
            get
            {
                string dir = Path.Combine(ToolsDirFullName, "SwitchRadeonGpu");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return Path.Combine(dir, LuckyKeyword.SwitchRadeonGpuFileName);
            }
        }
    }
}
