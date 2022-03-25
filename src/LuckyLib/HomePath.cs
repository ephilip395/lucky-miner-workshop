using Lucky.Core;
using System;
using System.IO;

namespace Lucky
{
    public static class HomePath
    {
        public static string AppDomainBaseDirectory { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string RootLockFileFullName { get; private set; } = Path.Combine(AppDomainBaseDirectory, LuckyKeyword.RootLockFileName);
        public static string RootConfigFileFullName { get; private set; } = Path.Combine(AppDomainBaseDirectory, LuckyKeyword.RootConfigFileName);
        public static string HomeDirFullName { get; private set; } = AppDomainBaseDirectory;
        public static string ServerJsonFileFullName { get; private set; }
        public static string SelfWorkServerJsonFileFullName { get; private set; }
        public static string MineWorkServerJsonFileFullName { get; private set; }
        public static string MineWorkLocalJsonFileFullName { get; private set; }
        public static string SelfWorkLocalJsonFileFullName { get; private set; }
        public static string GpuProfilesJsonFileFullName { get; private set; }

        public static string LocalDbFileFullName { get; private set; } = Path.Combine(HomeDirFullName, LuckyKeyword.LocalDbFileName);
        public static string ServerDbFileFullName { get; private set; } = Path.Combine(HomeDirFullName, LuckyKeyword.ServerDbFileName);

        static HomePath()
        {
            string tempPath = TempPath.TempDirFullName;
            string homeDirFullName = HomeDirFullName;
            if (!File.Exists(RootLockFileFullName))
            {
                if (File.Exists(RootConfigFileFullName))
                {
                    homeDirFullName = AppDomainBaseDirectory;
                }
                else
                {
                    homeDirFullName = tempPath;
                }
            }
            else
            {
                homeDirFullName = AppDomainBaseDirectory;
            }
            SetHomeDirFullName(homeDirFullName);
            ServerJsonFileFullName = Path.Combine(HomeDirFullName, LuckyKeyword.ServerJsonFileName);
            SelfWorkServerJsonFileFullName = Path.Combine(SelfWorkDirFullName, LuckyKeyword.ServerJsonFileName);
            MineWorkServerJsonFileFullName = Path.Combine(MineWorkDirFullName, LuckyKeyword.ServerJsonFileName);
            SelfWorkLocalJsonFileFullName = Path.Combine(SelfWorkDirFullName, LuckyKeyword.LocalJsonFileName);
            MineWorkLocalJsonFileFullName = Path.Combine(MineWorkDirFullName, LuckyKeyword.LocalJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(HomeDirFullName, LuckyKeyword.GpuProfilesFileName);
        }

        /// <summary>
        /// 挖矿端不需要调用该方法，因为挖矿端是根据所在目录下是否存在名称为home.config的文件而自动设置的。
        /// </summary>
        /// <param name="dirFullName"></param>
        public static void SetHomeDirFullName(string dirFullName)
        {
            if (dirFullName.EndsWith("\\"))
            {
                dirFullName = dirFullName.Substring(0, dirFullName.Length - 1);
            }
            HomeDirFullName = dirFullName;
            if (!Directory.Exists(dirFullName))
            {
                Directory.CreateDirectory(dirFullName);
            }
            LocalDbFileFullName = Path.Combine(HomeDirFullName, LuckyKeyword.LocalDbFileName);
            ServerDbFileFullName = Path.Combine(HomeDirFullName, LuckyKeyword.ServerDbFileName);
        }

        /// <summary>
        /// 返回形如server{version.Major.ToString()}.0.0.json的字符串。
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string GetServerJsonVersion(Version version)
        {
            return $"server-{version.Major}.0.0.json";
        }

        public static string ExportServerJsonFileName
        {
            get
            {
                return GetServerJsonVersion(EntryAssemblyInfo.CurrentVersion);
            }
        }

        public static string ExportServerJsonFileFullName
        {
            get
            {
                return Path.Combine(HomeDirFullName, ExportServerJsonFileName);
            }
        }

        public static bool IsLocalHome
        {
            get
            {
                string baseDir = AppDomainBaseDirectory;
                if (HomeDirFullName.Length + 1 != baseDir.Length)
                {
                    return false;
                }
                return HomeDirFullName + "\\" == baseDir;
            }
        }

        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(HomeDirFullName, LuckyKeyword.PackagesDirName);
                if (_sIsFirstCallPackageDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallPackageDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static string s_homeLogsDirFullName = null;
        public static string HomeLogsDirFullName
        {
            get
            {
                if (s_homeLogsDirFullName == null)
                {
                    s_homeLogsDirFullName = Path.Combine(HomeDirFullName, LuckyKeyword.LogsDirName);
                    if (!Directory.Exists(s_homeLogsDirFullName))
                    {
                        _ = Directory.CreateDirectory(s_homeLogsDirFullName);
                    }

                }
                return s_homeLogsDirFullName;
            }
        }

        private static bool _sIsFirstCallUpdaterDirFullName = true;
        /// <summary>
        /// 在家目录
        /// </summary>
        public static string UpdaterDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(HomeDirFullName, LuckyKeyword.UpdaterDirName);
                if (_sIsFirstCallUpdaterDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallUpdaterDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallSelfWorkDirFullName = true;
        public static string SelfWorkDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(HomeDirFullName, LuckyKeyword.SelfWorkDirName);
                if (_sIsFirstCallSelfWorkDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        _ = Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallSelfWorkDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallMineWorkDirFullName = true;
        public static string MineWorkDirFullName
        {
            get
            {
                string dirFullName = Path.Combine(HomeDirFullName, LuckyKeyword.MineWorkDirName);
                if (_sIsFirstCallMineWorkDirFullName)
                {
                    if (!Directory.Exists(dirFullName))
                    {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallMineWorkDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string UpdaterFileFullName
        {
            get
            {
                return Path.Combine(UpdaterDirFullName, LuckyKeyword.LuckyUpdaterFileName);
            }
        }

        public static string ReadServerJsonFile(WorkType workType)
        {
            switch (workType)
            {
                case WorkType.SelfWork:
                    return ReadSelfWorkServerJsonFile();
                case WorkType.MineWork:
                    return ReadMineWorkServerJsonFile();
                default:
                    if (File.Exists(ServerJsonFileFullName))
                    {
                        return File.ReadAllText(ServerJsonFileFullName);
                    }
                    break;
            }

            return string.Empty;
        }

        private static string ReadSelfWorkServerJsonFile()
        {
            if (File.Exists(SelfWorkServerJsonFileFullName))
            {
                return File.ReadAllText(SelfWorkServerJsonFileFullName);
            }
            else if (File.Exists(ServerJsonFileFullName))
            {
                return File.ReadAllText(ServerJsonFileFullName);
            }
            return string.Empty;
        }

        private static string ReadMineWorkServerJsonFile()
        {
            if (File.Exists(MineWorkServerJsonFileFullName))
            {
                return File.ReadAllText(MineWorkServerJsonFileFullName);
            }
            else if (File.Exists(ServerJsonFileFullName))
            {
                return File.ReadAllText(ServerJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteServerLitedbFile(byte[] data)
        {
            File.WriteAllBytes(ServerDbFileFullName, data);
        }

        public static void WriteServerJsonFile(string json)
        {
            File.WriteAllText(ServerJsonFileFullName, json);
        }

        public static void WriteSelfWorkServerJsonFile(string json)
        {
            File.WriteAllText(SelfWorkServerJsonFileFullName, json);
        }

        public static void WriteMineWorkServerJsonFile(string json)
        {
            File.WriteAllText(MineWorkServerJsonFileFullName, json);
        }

        public static string ReadLocalJsonFile(WorkType workType)
        {
            switch (workType)
            {
                case WorkType.SelfWork:
                    return ReadSelfWorkLocalJsonFile();
                case WorkType.MineWork:
                    return ReadMineWorkLocalJsonFile();
                default:
                    return string.Empty;
            }
        }

        private static string ReadMineWorkLocalJsonFile()
        {
            if (File.Exists(MineWorkLocalJsonFileFullName))
            {
                return File.ReadAllText(MineWorkLocalJsonFileFullName);
            }

            return string.Empty;
        }

        private static string ReadSelfWorkLocalJsonFile()
        {
            if (File.Exists(SelfWorkLocalJsonFileFullName))
            {
                return File.ReadAllText(SelfWorkLocalJsonFileFullName);
            }

            return string.Empty;
        }

        public static string ReadGpuProfilesJsonFile()
        {
            if (File.Exists(GpuProfilesJsonFileFullName))
            {
                return File.ReadAllText(GpuProfilesJsonFileFullName);
            }

            return string.Empty;
        }

        public static void WriteGpuProfilesJsonFile(string json)
        {
            File.WriteAllText(GpuProfilesJsonFileFullName, json);
        }
    }
}
