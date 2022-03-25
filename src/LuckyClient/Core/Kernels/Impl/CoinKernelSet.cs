﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Core.Kernels.Impl {
    internal class CoinKernelSet : SetBase, ICoinKernelSet {
        private readonly Dictionary<Guid, CoinKernelData> _dicById = new Dictionary<Guid, CoinKernelData>();

        private readonly IServerContext _context;
        public CoinKernelSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddCoinKernelCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!context.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id" + message.Input.CoinId);
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (_dicById.Values.Any(a => a.CoinId == message.Input.CoinId && a.KernelId == message.Input.KernelId)) {
                        return;
                    }
                    CoinKernelData entity = new CoinKernelData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = context.CreateServerRepository<CoinKernelData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new CoinKernelAddedEvent(message.MessageId, entity));

                    if (context.CoinSet.TryGetCoin(message.Input.CoinId, out ICoin coin)) {
                        IPool[] pools = context.PoolSet.AsEnumerable().Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (IPool pool in pools) {
                            Guid poolKernelId = Guid.NewGuid();
                            var poolKernel = new PoolKernelData() {
                                Id = poolKernelId,
                                Args = string.Empty,
                                KernelId = message.Input.KernelId,
                                PoolId = pool.GetId()
                            };
                            VirtualRoot.Execute(new AddPoolKernelCommand(poolKernel));
                        }
                    }
                }, location: this.GetType());
            context.AddCmdPath<UpdateCoinKernelCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!context.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id" + message.Input.CoinId);
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out CoinKernelData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<CoinKernelData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new CoinKernelUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveCoinKernelCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnce();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    CoinKernelData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    var repository = context.CreateServerRepository<CoinKernelData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new CoinKernelRemovedEvent(message.MessageId, entity));
                    if (context.CoinSet.TryGetCoin(entity.CoinId, out ICoin coin)) {
                        List<Guid> toRemoves = new List<Guid>();
                        IPool[] pools = context.PoolSet.AsEnumerable().Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (IPool pool in pools) {
                            foreach (PoolKernelData poolKernel in context.PoolKernelSet.AsEnumerable().Where(a => a.PoolId == pool.GetId() && a.KernelId == entity.KernelId).ToArray()) {
                                toRemoves.Add(poolKernel.Id);
                            }
                        }
                        foreach (Guid poolKernelId in toRemoves) {
                            VirtualRoot.Execute(new RemovePoolKernelCommand(poolKernelId));
                        }
                    }
                }, location: this.GetType());
            context.AddEventPath<FileWriterRemovedEvent>("移除文件书写器后移除引用关系", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                action: message => {
                    var repository = context.CreateServerRepository<CoinKernelData>();
                    var entities = _dicById.Values.Where(a => a.FileWriterIds.Contains(message.Source.GetId())).ToArray();
                    foreach (var entity in entities) {
                        entity.FileWriterIds = new List<Guid>(entity.FileWriterIds.Where(a => a != message.Source.GetId()));
                        repository.Update(entity);
                        VirtualRoot.RaiseEvent(new CoinKernelUpdatedEvent(message.MessageId, entity));
                    }
                });
            context.AddEventPath<FragmentWriterRemovedEvent>("移除命令行片段书写器后移除引用关系", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                action: message => {
                    var repository = context.CreateServerRepository<CoinKernelData>();
                    var entities = _dicById.Values.Where(a => a.FragmentWriterIds.Contains(message.Source.GetId())).ToArray();
                    foreach (var entity in entities) {
                        entity.FragmentWriterIds = new List<Guid>(entity.FragmentWriterIds.Where(a => a != message.Source.GetId()));
                        repository.Update(entity);
                        VirtualRoot.RaiseEvent(new CoinKernelUpdatedEvent(message.MessageId, entity));
                    }
                });
        }

        public int Count {
            get {
                InitOnce();
                return _dicById.Count;
            }
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<CoinKernelData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
                }
            }
        }

        public bool Contains(Guid kernelId) {
            InitOnce();
            return _dicById.ContainsKey(kernelId);
        }

        public bool TryGetCoinKernel(Guid kernelId, out ICoinKernel kernel) {
            InitOnce();
            var r = _dicById.TryGetValue(kernelId, out CoinKernelData k);
            kernel = k;
            return r;
        }

        public IEnumerable<ICoinKernel> AsEnumerable() {
            InitOnce();
            return _dicById.Values;
        }
    }
}
