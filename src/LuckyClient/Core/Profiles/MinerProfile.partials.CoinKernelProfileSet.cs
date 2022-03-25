﻿using Lucky.Core.MinerServer;
using Lucky.Core.Profile;
using Lucky.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lucky.Core.Profiles {
    internal partial class MinerProfile {
        private class CoinKernelProfileSet {
            private readonly Dictionary<Guid, CoinKernelProfile> _dicById = new Dictionary<Guid, CoinKernelProfile>();

            private readonly ILuckyContext _luckycmContext;
            private readonly object _locker = new object();

            public CoinKernelProfileSet(ILuckyContext luckycmContext) {
                _luckycmContext = luckycmContext;
            }

            public void Refresh() {
                _dicById.Clear();
            }

            public ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId) {
                if (_dicById.ContainsKey(coinKernelId)) {
                    return _dicById[coinKernelId];
                }
                lock (_locker) {
                    if (_dicById.ContainsKey(coinKernelId)) {
                        return _dicById[coinKernelId];
                    }
                    CoinKernelProfile coinKernelProfile = CoinKernelProfile.Create(_luckycmContext, coinKernelId);
                    _dicById.Add(coinKernelId, coinKernelProfile);

                    return coinKernelProfile;
                }
            }

            public IEnumerable<ICoinKernelProfile> GetCoinKernelProfiles() {
                return _dicById.Values;
            }

            public void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value) {
                CoinKernelProfile coinKernelProfile = (CoinKernelProfile)GetCoinKernelProfile(coinKernelId);
                coinKernelProfile.SetValue(propertyName, value);
            }

            private class CoinKernelProfile : ICoinKernelProfile {
                private static readonly CoinKernelProfile Empty = new CoinKernelProfile(new CoinKernelProfileData());
                public static CoinKernelProfile Create(ILuckyContext luckycmContext, Guid coinKernelId) {
                    if (luckycmContext.ServerContext.CoinKernelSet.TryGetCoinKernel(coinKernelId, out ICoinKernel coinKernel)) {
                        var repository = luckycmContext.ServerContext.CreateLocalRepository<CoinKernelProfileData>();
                        CoinKernelProfileData data = repository.GetByKey(coinKernelId);
                        if (data == null) {
                            double dualCoinWeight = GetDualCoinWeight(luckycmContext, coinKernel.KernelId);
                            data = CoinKernelProfileData.CreateDefaultData(coinKernel.GetId(), dualCoinWeight);
                        }
                        if (luckycmContext.ServerContext.GroupSet.TryGetGroup(coinKernel.DualCoinGroupId, out IGroup group)) {
                            var coinIds = luckycmContext.ServerContext.CoinGroupSet.GetGroupCoinIds(coinKernel.DualCoinGroupId);
                            if (!coinIds.Contains(data.DualCoinId)) {
                                data.DualCoinId = coinIds.FirstOrDefault();
                            }
                        }
                        CoinKernelProfile coinProfile = new CoinKernelProfile(data);

                        var defaultInputSegments = coinKernel.InputSegments.Where(a => a.IsDefault && a.TargetGpu.IsSupportedGpu(LuckyContext.Instance.GpuSet.GpuType)).ToArray();
                        string touchedArgs = coinProfile.TouchedArgs;
                        if (coinProfile.CustomArgs == null) {
                            coinProfile.CustomArgs = string.Empty;
                        }
                        if (string.IsNullOrEmpty(touchedArgs)) {
                            foreach (var defaultInputSegment in defaultInputSegments) {
                                if (!coinProfile.CustomArgs.Contains(defaultInputSegment.Segment)) {
                                    if (coinProfile.CustomArgs.Length == 0) {
                                        coinProfile.CustomArgs += defaultInputSegment.Segment;
                                    }
                                    else {
                                        coinProfile.CustomArgs += " " + defaultInputSegment.Segment;
                                    }
                                }
                            }
                        }
                        else {
                            foreach (var defaultInputSegment in defaultInputSegments) {
                                if (!touchedArgs.Contains(defaultInputSegment.Segment) && !coinProfile.CustomArgs.Contains(defaultInputSegment.Segment)) {
                                    if (coinProfile.CustomArgs.Length == 0) {
                                        coinProfile.CustomArgs += defaultInputSegment.Segment;
                                    }
                                    else {
                                        coinProfile.CustomArgs += " " + defaultInputSegment.Segment;
                                    }
                                }
                            }
                        }
                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                // 获取默认双挖权重
                private static double GetDualCoinWeight(ILuckyContext luckycmContext, Guid kernelId) {
                    double dualCoinWeight = 0;
                    if (luckycmContext.ServerContext.KernelSet.TryGetKernel(kernelId, out IKernel kernel)) {
                        if (luckycmContext.ServerContext.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                            dualCoinWeight = (kernelInput.DualWeightMin + kernelInput.DualWeightMax) / 2;
                        }
                    }
                    return dualCoinWeight;
                }

                private readonly CoinKernelProfileData _data;
                private CoinKernelProfile(CoinKernelProfileData data) {
                    _data = data ?? throw new ArgumentNullException(nameof(data));
                }

                [IgnoreReflectionSet]
                public Guid CoinKernelId {
                    get => _data.CoinKernelId;
                    private set {
                        _data.CoinKernelId = value;
                    }
                }

                public bool IsDualCoinEnabled {
                    get => _data.IsDualCoinEnabled;
                    private set {
                        _data.IsDualCoinEnabled = value;
                    }
                }
                public Guid DualCoinId {
                    get => _data.DualCoinId;
                    private set {
                        _data.DualCoinId = value;
                    }
                }

                public double DualCoinWeight {
                    get => _data.DualCoinWeight;
                    private set {
                        _data.DualCoinWeight = value;
                    }
                }

                public bool IsAutoDualWeight {
                    get => _data.IsAutoDualWeight;
                    private set {
                        _data.IsAutoDualWeight = value;
                    }
                }

                public string CustomArgs {
                    get => _data.CustomArgs;
                    private set {
                        _data.CustomArgs = value;
                    }
                }

                public string TouchedArgs {
                    get { return _data.TouchedArgs; }
                    set {
                        _data.TouchedArgs = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> _sProperties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (_sProperties == null) {
                            _sProperties = GetPropertiesCanSet<CoinKernelProfile>();
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
                                var repository = LuckyContext.Instance.ServerContext.CreateLocalRepository<CoinKernelProfileData>();
                                repository.Update(_data);
                                VirtualRoot.RaiseEvent(new CoinKernelProfilePropertyChangedEvent(this.CoinKernelId, propertyName));
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
