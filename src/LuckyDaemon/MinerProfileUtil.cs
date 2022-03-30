using LiteDB;
using Lucky.Core.Profile;
using Lucky.Core.Daemon;
using System.IO;
using LuckyJsonRpc;

namespace Lucky
{
    public static class MinerProfileUtil
    {
        #region IsAutoBoot
        public static bool GetIsAutoBoot()
        {
            var db = GetDb();
            if (db != null)
            {
                using (db)
                {
                    MinerProfileData data = db.GetCollection<MinerProfileData>().FindById(MinerProfileData.DefaultId);
                    if (data != null)
                    {
                        return data.IsAutoBoot;
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void SetAutoStart(bool isAutoBoot, bool isAutoStart)
        {
            var db = GetDb();
            if (db != null)
            {
                using (db)
                {
                    var col = db.GetCollection<MinerProfileData>();
                    MinerProfileData data = col.FindById(MinerProfileData.DefaultId);
                    if (data != null)
                    {
                        data.IsAutoBoot = isAutoBoot;
                        data.IsAutoStart = isAutoStart;
                        LuckyRegistry.SetIsAutoStart(isAutoStart);
                        col.Update(data);
                    }
                }
            }
        }

        public static void UpdateConnParams(ConnParams connParams)
        {

            var db = GetDb();
            if (db != null)
            {
                using (db)
                {
                    var col = db.GetCollection<MinerProfileData>();
                    MinerProfileData data = col.FindById(MinerProfileData.DefaultId);
                    if (data != null)
                    {
                        data.ConnectionMethod = connParams.ConnectionMethod;
                        if(connParams.ConnectionMethod == (int) ConnMethod.socks5)
                        {
                            data.ProxyServerAddress = connParams.ProxyServerAddress;
                            data.ProxyServerPort = connParams.ProxyServerPort;
                            data.ProxyUsername = connParams.ProxyUsername;
                            data.ProxyPassword = connParams.ProxyPassword;
                        }
                        col.Update(data);
                        LuckyRegistry.UpdateConnParams(connParams);
                    }
                }
            }
        }

        private static LiteDatabase GetDb()
        {
            string location = LuckyRegistry.GetLocation(LuckyAppType.MinerTweak);
            if (!string.IsNullOrEmpty(location))
            {
                string dbFile = Path.Combine(Path.GetDirectoryName(location), LuckyKeyword.LocalDbFileName);
                bool isDbFileExist = File.Exists(dbFile);
                if (!isDbFileExist)
                {
                    dbFile = Path.Combine(TempPath.TempDirFullName, LuckyKeyword.LocalDbFileName);
                    isDbFileExist = File.Exists(dbFile);
                }
                if (!isDbFileExist)
                {
                    return null;
                }
                return new LiteDatabase($"filename={dbFile}");
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
