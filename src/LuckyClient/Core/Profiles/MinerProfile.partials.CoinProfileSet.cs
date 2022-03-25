﻿using Lucky.Core.Profile;
using Lucky.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lucky.Core.Profiles {
    internal partial class MinerProfile {
        private class CoinProfileSet {
            private readonly Dictionary<Guid, CoinProfile> _dicById = new Dictionary<Guid, CoinProfile>();
            private readonly ILuckyContext _luckycmContext;
            private readonly object _locker = new object();
            public CoinProfileSet(ILuckyContext luckycmContext) {
                _luckycmContext = luckycmContext;
            }

            public void Refresh() {
                _dicById.Clear();
            }

            public ICoinProfile GetCoinProfile(Guid coinId) {
                if (_dicById.ContainsKey(coinId)) {
                    return _dicById[coinId];
                }
                lock (_locker) {
                    if (_dicById.ContainsKey(coinId)) {
                        return _dicById[coinId];
                    }
                    CoinProfile coinProfile = CoinProfile.Create(_luckycmContext, coinId);
                    _dicById.Add(coinId, coinProfile);
                    return coinProfile;
                }
            }

            public IEnumerable<ICoinProfile> GetCoinProfiles() {
                return _dicById.Values;
            }

            public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
                CoinProfile coinProfile = (CoinProfile)GetCoinProfile(coinId);
                coinProfile.SetValue(propertyName, value);
            }

            private class CoinProfile : ICoinProfile {
                private static readonly CoinProfile Empty = new CoinProfile(new CoinProfileData());

                public static CoinProfile Create(ILuckyContext luckycmContext, Guid coinId) {
                    if (luckycmContext.ServerContext.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                        var data = GetCoinProfileData(coin.GetId());
                        if (data == null) {
                            Guid poolId = Guid.Empty;
                            IPool pool = luckycmContext.ServerContext.PoolSet.AsEnumerable().OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId);
                            if (pool != null) {
                                poolId = pool.GetId();
                            }
                            string wallet = coin.TestWallet;
                            Guid coinKernelId = GetDefaultCoinKernelId(coin);
                            data = CoinProfileData.CreateDefaultData(coinId, poolId, wallet, coinKernelId);
                        }
                        else {
                            if (!luckycmContext.ServerContext.CoinKernelSet.TryGetCoinKernel(data.CoinKernelId, out ICoinKernel coinKernel)) {
                                data.CoinKernelId = GetDefaultCoinKernelId(coin);
                            }
                        }
                        CoinProfile coinProfile = new CoinProfile(data);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                /// <summary>
                /// 选择默认内核
                /// </summary>
                /// <param name="coin"></param>
                /// <returns></returns>
                private static Guid GetDefaultCoinKernelId(ICoin coin) {
                    var root = LuckyContext.Instance;
                    Guid coinKernelId = Guid.Empty;
                    bool noneGpu = false;
                    if (root.GpuSet.GpuType.IsEmpty()) {
                        noneGpu = true;
                    }
                    List<ICoinKernel> coinKernels;
                    if (noneGpu) {
                        coinKernels = root.ServerContext.CoinKernelSet.AsEnumerable().Where(a => a.CoinId == coin.GetId()).ToList();
                    }
                    else {
                        coinKernels = root.ServerContext.CoinKernelSet.AsEnumerable().Where(a => a.CoinId == coin.GetId() && a.SupportedGpu.IsSupportedGpu(root.GpuSet.GpuType)).ToList();
                    }
                    var items = new List<Tuple<Guid, IKernel>>(coinKernels.Count);
                    foreach (var item in coinKernels) {
                        if (root.ServerContext.KernelSet.TryGetKernel(item.KernelId, out IKernel kernel)) {
                            items.Add(new Tuple<Guid, IKernel>(item.GetId(), kernel));
                        }
                    }
                    items = items.OrderBy(a => a.Item2.Code).ThenByDescending(a => a.Item2.Version).ToList();
                    Guid kernelBrandId = coin.GetKernelBrandId(root.GpuSet.GpuType);
                    if (kernelBrandId == Guid.Empty) {
                        coinKernelId = items.Select(a => a.Item1).FirstOrDefault();
                    }
                    else {
                        coinKernelId = items.Where(a => a.Item2.BrandId == kernelBrandId).Select(a => a.Item1).FirstOrDefault();
                    }
                    return coinKernelId;
                }

                private readonly CoinProfileData _data;

                private static CoinProfileData GetCoinProfileData(Guid coinId) {
                    var repository = LuckyContext.Instance.ServerContext.CreateLocalRepository<CoinProfileData>();
                    var result = repository.GetByKey(coinId);
                    return result;
                }

                private CoinProfile(CoinProfileData data) {
                    _data = data ?? throw new ArgumentNullException(nameof(data));
                }

                [IgnoreReflectionSet]
                public Guid CoinId {
                    get => _data.CoinId;
                    private set {
                        _data.CoinId = value;
                    }
                }

                public Guid PoolId {
                    get => _data.PoolId;
                    private set {
                        _data.PoolId = value;
                    }
                }

                public Guid PoolId1 {
                    get => _data.PoolId1;
                    private set {
                        _data.PoolId1 = value;
                    }
                }

                public string Wallet {
                    get => _data.Wallet;
                    private set {
                        _data.Wallet = value;
                    }
                }

                public bool IsHideWallet {
                    get => _data.IsHideWallet;
                    private set {
                        _data.IsHideWallet = value;
                    }
                }

                public Guid CoinKernelId {
                    get => _data.CoinKernelId;
                    private set {
                        _data.CoinKernelId = value;
                    }
                }
                public Guid DualCoinPoolId {
                    get => _data.DualCoinPoolId;
                    private set {
                        _data.DualCoinPoolId = value;
                    }
                }

                public string DualCoinWallet {
                    get => _data.DualCoinWallet;
                    private set {
                        _data.DualCoinWallet = value;
                    }
                }

                public bool IsDualCoinHideWallet {
                    get => _data.IsDualCoinHideWallet;
                    private set {
                        _data.IsDualCoinHideWallet = value;
                    }
                }

                public double CalcInput {
                    get => _data.CalcInput;
                    private set {
                        _data.CalcInput = value;
                    }
                }

                public bool IsLowSpeedRestartComputer {
                    get => _data.IsLowSpeedRestartComputer;
                    private set {
                        _data.IsLowSpeedRestartComputer = value;
                    }
                }

                public int LowSpeedRestartComputerMinutes {
                    get => _data.LowSpeedRestartComputerMinutes;
                    private set {
                        _data.LowSpeedRestartComputerMinutes = value;
                    }
                }

                public double LowSpeed {
                    get => _data.LowSpeed;
                    private set {
                        _data.LowSpeed = value;
                    }
                }

                public bool IsLowSpeedReOverClock {
                    get => _data.IsLowSpeedReOverClock;
                    private set {
                        _data.IsLowSpeedReOverClock = value;
                    }
                }

                public int LowSpeedReOverClockMinutes {
                    get => _data.LowSpeedReOverClockMinutes;
                    private set {
                        _data.LowSpeedReOverClockMinutes = value;
                    }
                }

                public double OverClockLowSpeed {
                    get => _data.OverClockLowSpeed;
                    private set {
                        _data.OverClockLowSpeed = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> _sProperties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (_sProperties == null) {
                            _sProperties = GetPropertiesCanSet<CoinProfile>();
                        }
                        return _sProperties;
                    }
                }

                public void SetValue(string propertyName, object value) {
                    if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                        if (propertyInfo.CanWrite) {
                            // 这里的反射赋值没有经过序列化和反序列化且由接口约束了类型一定相同所以可以直接赋值和比较
                            var oldValue = propertyInfo.GetValue(this, null);
                            if (oldValue != value) {
                                propertyInfo.SetValue(this, value, null);
                                var repository = LuckyContext.Instance.ServerContext.CreateLocalRepository<CoinProfileData>();
                                repository.Update(_data);
                                VirtualRoot.RaiseEvent(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                            }
                        }
                    }
                }

                public override string ToString() {
                    if (_data == null) {
                        return string.Empty;
                    }
                    return _data.ToString();
                }
            }
        }
    }
}
