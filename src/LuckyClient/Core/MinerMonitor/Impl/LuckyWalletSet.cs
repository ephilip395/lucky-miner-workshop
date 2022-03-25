using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Core.MinerMonitor.Impl
{
    public class LuckyWalletSet : SetBase, ILuckyWalletSet
    {
        private readonly Dictionary<Guid, LuckyWalletData> _dicById = new Dictionary<Guid, LuckyWalletData>();

        public LuckyWalletSet()
        {
            VirtualRoot.BuildCmdPath<AddLuckyWalletCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) =>
            {
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty)
                {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.Wallet))
                {
                    throw new ValidationException("LuckyWallet Wallet can't be null or empty");
                }
                if (_dicById.ContainsKey(message.Input.GetId()))
                {
                    return;
                }
                LuckyWalletData entity = new LuckyWalletData().Update(message.Input);
                RpcRoot.OfficialServer.LuckyWalletService.AddOrUpdateLuckyWalletAsync(entity, (response, e) =>
                {
                    if (response.IsSuccess())
                    {
                        _dicById.Add(entity.Id, entity);
                        VirtualRoot.RaiseEvent(new LuckyWalletAddedEvent(message.MessageId, entity));
                    }
                    else
                    {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            });
            VirtualRoot.BuildCmdPath<UpdateLuckyWalletCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) =>
            {
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty)
                {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.Wallet))
                {
                    throw new ValidationException("minerGroup Wallet can't be null or empty");
                }
                if (!_dicById.TryGetValue(message.Input.GetId(), out LuckyWalletData entity))
                {
                    return;
                }
                LuckyWalletData oldValue = new LuckyWalletData().Update(entity);
                entity.Update(message.Input);
                RpcRoot.OfficialServer.LuckyWalletService.AddOrUpdateLuckyWalletAsync(entity, (response, e) =>
                {
                    if (!response.IsSuccess())
                    {
                        entity.Update(oldValue);
                        VirtualRoot.RaiseEvent(new LuckyWalletUpdatedEvent(message.MessageId, entity));
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
                VirtualRoot.RaiseEvent(new LuckyWalletUpdatedEvent(message.MessageId, entity));
            });
            VirtualRoot.BuildCmdPath<RemoveLuckyWalletCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) =>
            {
                if (message == null || message.EntityId == Guid.Empty)
                {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId))
                {
                    return;
                }
                LuckyWalletData entity = _dicById[message.EntityId];
                RpcRoot.OfficialServer.LuckyWalletService.RemoveLuckyWalletAsync(entity.Id, (response, e) =>
                {
                    if (response.IsSuccess())
                    {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.RaiseEvent(new LuckyWalletRemovedEvent(message.MessageId, entity));
                    }
                    else
                    {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            });
        }

        protected override void Init()
        {
            RpcRoot.OfficialServer.LuckyWalletService.GetLuckyWalletsAsync((response, e) =>
            {
                if (response.IsSuccess())
                {
                    foreach (var item in response.Data)
                    {
                        if (!_dicById.ContainsKey(item.GetId()))
                        {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                }
                VirtualRoot.RaiseEvent(new LuckyWalletSetInitedEvent());
            });
        }

        public bool TryGetLuckyWallet(Guid id, out ILuckyWallet wallet)
        {
            InitOnce();
            var r = _dicById.TryGetValue(id, out LuckyWalletData g);
            wallet = g;
            return r;
        }

        public IEnumerable<ILuckyWallet> AsEnumerable()
        {
            InitOnce();
            return _dicById.Values;
        }
    }
}
