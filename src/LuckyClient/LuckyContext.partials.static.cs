﻿using Lucky.Core;
using Lucky.Core.MinerMonitor;
using Lucky.Core.MinerMonitor.Impl;
using Lucky.Core.Profile;
using Lucky.JsonDb;
using Lucky.Repositories;
using System;
using System.Linq;

namespace Lucky
{
    public partial class LuckyContext
    {
        private static Version _serverVersion = EntryAssemblyInfo.CurrentVersion;
        public static Version ServerVersion
        {
            get
            {
                return _serverVersion;
            }
        }
        public static void SetServerVersion(Version serverVersion)
        {
            _serverVersion = serverVersion;
        }

        public const int SpeedHistoryLengthByMinute = 10;
        public const int GpuAllId = -1;
        private static readonly LuckyContext _instance = new LuckyContext();
        public static ILuckyContext Instance { get; private set; } = _instance;

        private static WorkType _workType;
        private static string _workerName;

        public static WorkType WorkType
        {
            get { return _workType; }
        }

        public static IMinerMonitorContext MinerMonitorContext { get; private set; } = new MinerMonitorContext();

        private static Guid _id = LuckyRegistry.GetClientId(ClientAppType.AppType);
        public static Guid Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// 表示是否是使用server.json只读数据库文件。
        /// 只有DevMode模式的挖矿端才会返回False，否则都是True。
        /// </summary>
        public static bool IsJsonServer
        {
            get
            {
                if (ClientAppType.IsMinerTweak && DevMode.IsDevMode)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 是否是使用local.json作为数据库而不是local.litedb。
        /// 有两种情况该属性会被赋值为true：1. 群控端；2. 作业模式的挖矿端。
        /// </summary>
        public static bool IsJsonLocal
        {
            get { return ClientAppType.IsMinerMonitor || _workType != WorkType.None; }
        }

        public static string ThisPcName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public static Action<string> RefreshArgsAssembly { get; private set; } = (reason) => { };
        public static void SetRefreshArgsAssembly(Action<string> action)
        {
            RefreshArgsAssembly = action;
        }
        private static bool _isUiVisible = false;
        public static bool IsUiVisible
        {
            get { return _isUiVisible; }
            set
            {
                _isUiVisible = value;
                if (value)
                {
                    MainWindowRendedOn = DateTime.Now;
                }
            }
        }
        private static DateTime _mainWindowRendedOn = DateTime.MinValue;
        public static DateTime MainWindowRendedOn
        {
            get { return _mainWindowRendedOn; }
            private set
            {
                _mainWindowRendedOn = value;
            }
        }

        public static bool IsAutoStartCanceled = false;

        public static bool IsKernelBrand
        {
            get
            {
                return KernelBrandId != Guid.Empty;
            }
        }

        private static Guid? kernelBrandId = null;
        public static Guid KernelBrandId
        {
            get
            {
                if (!kernelBrandId.HasValue)
                {
                    lock (_locker)
                    {
                        if (!kernelBrandId.HasValue)
                        {
                            kernelBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, LuckyKeyword.KernelBrandId);
                        }
                    }
                }
                return kernelBrandId.Value;
            }
        }

        public static bool IsPoolBrand
        {
            get
            {
                return PoolBrandId != Guid.Empty;
            }
        }

        private static Guid? poolBrandId = null;
        public static Guid PoolBrandId
        {
            get
            {
                if (!poolBrandId.HasValue)
                {
                    lock (_locker)
                    {
                        if (!poolBrandId.HasValue)
                        {
                            poolBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, LuckyKeyword.PoolBrandId);
                        }
                    }
                }
                return poolBrandId.Value;
            }
        }

        public static bool IsBrandSpecified
        {
            get
            {
                return KernelBrandId != Guid.Empty || PoolBrandId != Guid.Empty;
            }
        }

        private static LocalJsonDb _localJsonDb;
        public static ILocalJsonDb LocalJsonDb
        {
            get
            {
                LocalJsonInit();
                return _localJsonDb;
            }
        }

        public static void ReInitLocalJson()
        {
            _localJsonInited = false;
        }

        #region LocalJsonInit
        private static bool _localJsonInited = false;
        // 从磁盘读取local.json反序列化为LocalJson对象
        private static void LocalJsonInit()
        {
            if (!_localJsonInited)
            {
                lock (_locker)
                {
                    if (!_localJsonInited)
                    {
                        _localJsonInited = true;
                        string localJson = HomePath.ReadLocalJsonFile(_workType);
                        LocalJsonDb localJsonDb = null;
                        if (!string.IsNullOrEmpty(localJson))
                        {
                            localJsonDb = VirtualRoot.JsonSerializer.Deserialize<LocalJsonDb>(localJson);
                        }
                        if (localJsonDb == null)
                        {
                            if (ClientAppType.IsMinerTweak)
                            {
                                localJsonDb = JsonDb.LocalJsonDb.ConvertFromLuckyContext();
                                VirtualRoot.MyLocalWarn(nameof(LuckyContext), "当前作业由本机数据自动生成，因为本机没有作业记录，请先在群控端创建或编辑作业。", OutEnum.Warn, toConsole: true);
                            }
                            else
                            {
                                localJsonDb = new LocalJsonDb();
                            }
                        }
                        _localJsonDb = localJsonDb;

                        if (ClientAppType.IsMinerTweak)
                        {
                            #region 因为是群控作业，将开机启动和自动挖矿设置为true
                            var repository = new LiteDbReadWriteRepository<MinerProfileData>(HomePath.LocalDbFileFullName);
                            MinerProfileData localProfile = repository.GetByKey(MinerProfileData.DefaultId);
                            if (localProfile != null)
                            {
                                MinerProfileData.CopyWorkIgnoreValues(localProfile, _localJsonDb.MinerProfile);
                                // 如果是作业模式则必须设置为开机自动重启
                                if (!localProfile.IsAutoBoot)
                                {
                                    localProfile.IsAutoBoot = true;
                                    repository.Update(localProfile);
                                }
                            }
                            _localJsonDb.MinerProfile.IsAutoBoot = true;
                            LuckyRegistry.SetIsAutoStart(true);
                            #endregion

                            #region 矿机名
                            if (!string.IsNullOrEmpty(_workerName))
                            {
                                _localJsonDb.MinerProfile.MinerName = _workerName;
                            }
                            else
                            {
                                // 当用户使用群控作业但没有指定群控矿机名时使用从local.litedb中读取的矿工名
                                if (string.IsNullOrEmpty(_localJsonDb.MinerProfile.MinerName))
                                {
                                    if (localProfile != null)
                                    {
                                        _localJsonDb.MinerProfile.MinerName = localProfile.MinerName;
                                    }
                                    // 如果local.litedb中也没有矿机名则使用去除了特殊符号的本机机器名作为矿机名
                                    if (string.IsNullOrEmpty(_localJsonDb.MinerProfile.MinerName))
                                    {
                                        _localJsonDb.MinerProfile.MinerName = LuckyKeyword.GetSafeMinerName(ThisPcName);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
        }
        #endregion

        private static ServerJsonDb _serverJsonDb;
        public static IServerJsonDb ServerJsonDb
        {
            get
            {
                ServerJsonInit();
                return _serverJsonDb;
            }
        }

        public static void ReInitServerJson()
        {
            _serverJsonInited = false;
        }

        #region ServerJsonInit
        private static bool _serverJsonInited = false;
        // 从磁盘读取server.json反序列化为ServerJson对象
        private static void ServerJsonInit()
        {
            if (!_serverJsonInited)
            {
                lock (_locker)
                {
                    if (!_serverJsonInited)
                    {
                        _serverJsonInited = true;
                        string serverJson = HomePath.ReadServerJsonFile(_workType);
                        if (!string.IsNullOrEmpty(serverJson))
                        {
                            ServerJsonDb data = VirtualRoot.JsonSerializer.Deserialize<ServerJsonDb>(serverJson) ?? new ServerJsonDb();
                            try
                            {
                                _serverJsonDb = data;
                                if (KernelBrandId != Guid.Empty)
                                {
                                    var kernelToRemoves = data.Kernels.Where(a => a.BrandId != KernelBrandId).ToArray();
                                    foreach (var item in kernelToRemoves)
                                    {
                                        data.Kernels.Remove(item);
                                    }
                                    var coinKernelToRemoves = data.CoinKernels.Where(a => kernelToRemoves.Any(b => b.Id == a.KernelId)).ToArray();
                                    foreach (var item in coinKernelToRemoves)
                                    {
                                        data.CoinKernels.Remove(item);
                                    }
                                    var poolKernelToRemoves = data.PoolKernels.Where(a => kernelToRemoves.Any(b => b.Id == a.KernelId)).ToArray();
                                    foreach (var item in poolKernelToRemoves)
                                    {
                                        data.PoolKernels.Remove(item);
                                    }
                                }
                                if (PoolBrandId != Guid.Empty)
                                {
                                    var poolToRemoves = data.Pools.Where(a => a.BrandId != PoolBrandId && data.Pools.Any(b => b.CoinId == a.CoinId && b.BrandId == poolBrandId)).ToArray();
                                    foreach (var item in poolToRemoves)
                                    {
                                        data.Pools.Remove(item);
                                    }
                                    var poolKernelToRemoves = data.PoolKernels.Where(a => poolToRemoves.Any(b => b.Id == a.PoolId)).ToArray();
                                    foreach (var item in poolKernelToRemoves)
                                    {
                                        data.PoolKernels.Remove(item);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        else
                        {
                            _serverJsonDb = new ServerJsonDb();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
