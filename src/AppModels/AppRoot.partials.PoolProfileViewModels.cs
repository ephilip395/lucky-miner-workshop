﻿using Lucky.Vms;
using System;
using System.Collections.Generic;

namespace Lucky {
    public static partial class AppRoot {
        public class PoolProfileViewModels : ViewModelBase {
            public static PoolProfileViewModels Instance { get; private set; } = new PoolProfileViewModels();
            private readonly Dictionary<Guid, PoolProfileViewModel> _dicById = new Dictionary<Guid, PoolProfileViewModel>();

            private PoolProfileViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                BuildEventPath<PoolProfilePropertyChangedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.PoolId, out PoolProfileViewModel vm)) {
                            vm.OnPropertyChanged(message.PropertyName);
                        }
                    });
                VirtualRoot.BuildEventPath<LocalContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Clear();
                    });
            }

            private readonly object _locker = new object();
            public PoolProfileViewModel GetOrCreatePoolProfile(Guid poolId) {
                if (!_dicById.TryGetValue(poolId, out PoolProfileViewModel poolProfile)) {
                    lock (_locker) {
                        if (!_dicById.TryGetValue(poolId, out poolProfile)) {
                            poolProfile = new PoolProfileViewModel(LuckyContext.Instance.MinerProfile.GetPoolProfile(poolId));
                            _dicById.Add(poolId, poolProfile);
                        }
                    }
                }
                return poolProfile;
            }
        }
    }
}
