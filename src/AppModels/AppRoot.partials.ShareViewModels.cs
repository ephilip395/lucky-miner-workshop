﻿using Lucky.Vms;
using System;
using System.Collections.Generic;

namespace Lucky {
    public static partial class AppRoot {
        public class ShareViewModels {
            public static ShareViewModels Instance { get; private set; } = new ShareViewModels();
            private readonly Dictionary<Guid, ShareViewModel> _dicByCoinId = new Dictionary<Guid, ShareViewModel>();

            private ShareViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                BuildEventPath<ShareChangedEvent>("收益变更后调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicByCoinId.TryGetValue(message.Source.CoinId, out ShareViewModel shareVm)) {
                            shareVm.Update(message.Source);
                        }
                    });
            }

            private readonly object _locker = new object();
            public ShareViewModel GetOrCreate(Guid coinId) {
                if (!LuckyContext.Instance.ServerContext.CoinSet.Contains(coinId)) {
                    return new ShareViewModel(coinId);
                }
                if (!_dicByCoinId.TryGetValue(coinId, out ShareViewModel shareVm)) {
                    lock (_locker) {
                        if (!_dicByCoinId.TryGetValue(coinId, out shareVm)) {
                            shareVm = new ShareViewModel(coinId);
                            _dicByCoinId.Add(coinId, shareVm);
                        }
                    }
                }
                return shareVm;
            }
        }
    }
}
