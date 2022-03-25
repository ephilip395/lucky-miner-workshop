﻿using System;
using System.Collections.Generic;

namespace Lucky.Core.Profiles.Impl {
    public class WalletSet : SetBase {
        private readonly Dictionary<Guid, WalletData> _dicById = new Dictionary<Guid, WalletData>();

        private readonly ILuckyContext _luckycmContext;
        public WalletSet(ILuckyContext luckycmContext) {
            _luckycmContext = luckycmContext;
            VirtualRoot.BuildCmdPath<AddWalletCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnce();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!luckycmContext.ServerContext.CoinSet.Contains(message.Input.CoinId)) {
                    throw new ValidationException("there is not coin with id " + message.Input.CoinId);
                }
                if (string.IsNullOrEmpty(message.Input.Address)) {
                    throw new ValidationException("wallet code and Address can't be null or empty");
                }
                if (_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                WalletData entity = new WalletData().Update(message.Input);
                _dicById.Add(entity.Id, entity);
                var repository = luckycmContext.ServerContext.CreateLocalRepository<WalletData>();
                repository.Add(entity);

                VirtualRoot.RaiseEvent(new WalletAddedEvent(message.MessageId, entity));
            });
            VirtualRoot.BuildCmdPath<UpdateWalletCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnce();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!luckycmContext.ServerContext.CoinSet.Contains(message.Input.CoinId)) {
                    throw new ValidationException("there is not coin with id " + message.Input.CoinId);
                }
                if (string.IsNullOrEmpty(message.Input.Address)) {
                    throw new ValidationException("wallet Address can't be null or empty");
                }
                if (string.IsNullOrEmpty(message.Input.Name)) {
                    throw new ValidationException("wallet name can't be null or empty");
                }
                if (!_dicById.TryGetValue(message.Input.GetId(), out WalletData entity)) {
                    return;
                }
                entity.Update(message.Input);
                var repository = luckycmContext.ServerContext.CreateLocalRepository<WalletData>();
                repository.Update(entity);

                VirtualRoot.RaiseEvent(new WalletUpdatedEvent(message.MessageId, entity));
            });
            VirtualRoot.BuildCmdPath<RemoveWalletCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) => {
                InitOnce();
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                WalletData entity = _dicById[message.EntityId];
                _dicById.Remove(entity.Id);
                var repository = luckycmContext.ServerContext.CreateLocalRepository<WalletData>();
                repository.Remove(entity.Id);

                VirtualRoot.RaiseEvent(new WalletRemovedEvent(message.MessageId, entity));
            });
        }

        public void Refresh() {
            _dicById.Clear();
            base.DeferReInit();
        }

        protected override void Init() {
            var repository = _luckycmContext.ServerContext.CreateLocalRepository<WalletData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.Id)) {
                    _dicById.Add(item.Id, item);
                }
            }
        }

        public bool TryGetWallet(Guid walletId, out IWallet wallet) {
            InitOnce();
            bool r = _dicById.TryGetValue(walletId, out WalletData wlt);
            wallet = wlt;
            return r;
        }

        public IEnumerable<IWallet> GetWallets() {
            InitOnce();
            return _dicById.Values;
        }
    }
}
