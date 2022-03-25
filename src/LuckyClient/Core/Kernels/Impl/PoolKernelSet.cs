﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Core.Kernels.Impl {
    public class PoolKernelSet : SetBase, IPoolKernelSet {
        private readonly IServerContext _context;
        private readonly Dictionary<Guid, PoolKernelData> _dicById = new Dictionary<Guid, PoolKernelData>();

        public PoolKernelSet(IServerContext context) {
            _context = context;
            _context.AddCmdPath<AddPoolKernelCommand>(LogEnum.DevConsole,
                action: message => {
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        var entity = new PoolKernelData().Update(message.Input);
                        _dicById.Add(message.Input.GetId(), entity);
                        var repository = context.CreateServerRepository<PoolKernelData>();
                        repository.Add(entity);
                        VirtualRoot.RaiseEvent(new PoolKernelAddedEvent(message.MessageId, message.Input));
                    }
                }, location: this.GetType());
            _context.AddCmdPath<RemovePoolKernelCommand>(LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.EntityId)) {
                        var entity = _dicById[message.EntityId];
                        _dicById.Remove(message.EntityId);
                        var repository = context.CreateServerRepository<PoolKernelData>();
                        repository.Remove(message.EntityId);
                        VirtualRoot.RaiseEvent(new PoolKernelRemovedEvent(message.MessageId, entity));
                    }
                }, location: this.GetType());
            _context.AddCmdPath<UpdatePoolKernelCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_context.PoolSet.Contains(message.Input.PoolId)) {
                        throw new ValidationException("there is no pool with id" + message.Input.PoolId);
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out PoolKernelData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<PoolKernelData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new PoolKernelUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        public int Count {
            get {
                InitOnce();
                return _dicById.Count;
            }
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<PoolKernelData>();
            List<PoolKernelData> list = repository.GetAll().ToList();
            foreach (IPool pool in _context.PoolSet.AsEnumerable().ToArray()) {
                foreach (ICoinKernel coinKernel in _context.CoinKernelSet.AsEnumerable().Where(a => a.CoinId == pool.CoinId).ToArray()) {
                    PoolKernelData poolKernel = list.FirstOrDefault(a => a.PoolId == pool.GetId() && a.KernelId == coinKernel.KernelId);
                    if (poolKernel != null) {
                        _dicById.Add(poolKernel.GetId(), poolKernel);
                    }
                    else {
                        Guid poolKernelId = Guid.NewGuid();
                        _dicById.Add(poolKernelId, new PoolKernelData() {
                            Id = poolKernelId,
                            Args = string.Empty,
                            KernelId = coinKernel.KernelId,
                            PoolId = pool.GetId()
                        });
                    }
                }
            }
        }

        public bool Contains(Guid kernelId) {
            InitOnce();
            return _dicById.ContainsKey(kernelId);
        }

        public bool TryGetPoolKernel(Guid kernelId, out IPoolKernel kernel) {
            InitOnce();
            var r = _dicById.TryGetValue(kernelId, out PoolKernelData k);
            kernel = k;
            return r;
        }

        public IEnumerable<IPoolKernel> AsEnumerable() {
            InitOnce();
            return _dicById.Values;
        }
    }
}
