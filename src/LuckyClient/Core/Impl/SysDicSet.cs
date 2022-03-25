﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Core.Impl
{
    internal class SysDicSet : SetBase, ISysDicSet
    {
        private readonly Dictionary<string, SysDicData> _dicByCode = new Dictionary<string, SysDicData>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, SysDicData> _dicById = new Dictionary<Guid, SysDicData>();

        private readonly IServerContext _context;
        public SysDicSet(IServerContext context)
        {
            _context = context;

            context.AddCmdPath<AddSysDicCommand>(LogEnum.DevConsole,
                action: message =>
                {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty)
                    {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("dic code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId()))
                    {
                        return;
                    }
                    if (_dicByCode.ContainsKey(message.Input.Code))
                    {
                        throw new ValidationException("编码重复");
                    }
                    SysDicData entity = new SysDicData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    _dicByCode.Add(entity.Code, entity);
                    Repositories.IRepository<SysDicData> repository = context.CreateServerRepository<SysDicData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new SysDicAddedEvent(message.MessageId, entity));
                }, location: GetType());

            context.AddCmdPath<UpdateSysDicCommand>(LogEnum.DevConsole,
                action: message =>
                {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty)
                    {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("sysDic code can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out SysDicData entity))
                    {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input))
                    {
                        return;
                    }
                    _ = entity.Update(message.Input);
                    Repositories.IRepository<SysDicData> repository = context.CreateServerRepository<SysDicData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new SysDicUpdatedEvent(message.MessageId, entity));
                }, location: GetType());

            context.AddCmdPath<RemoveSysDicCommand>(LogEnum.DevConsole,
                action: message =>
                {
                    InitOnce();
                    if (message == null || message.EntityId == Guid.Empty)
                    {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId))
                    {
                        return;
                    }
                    SysDicData entity = _dicById[message.EntityId];
                    List<Guid> toRemoves = context.SysDicItemSet.GetSysDicItems(entity.Code).Select(a => a.GetId()).ToList();
                    foreach (Guid id in toRemoves)
                    {
                        VirtualRoot.Execute(new RemoveSysDicItemCommand(id));
                    }
                    _ = _dicById.Remove(entity.Id);
                    if (_dicByCode.ContainsKey(entity.Code))
                    {
                        _ = _dicByCode.Remove(entity.Code);
                    }
                    Repositories.IRepository<SysDicData> repository = context.CreateServerRepository<SysDicData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new SysDicRemovedEvent(message.MessageId, entity));
                }, location: GetType());
        }

        protected override void Init()
        {
            Repositories.IRepository<SysDicData> repository = _context.CreateServerRepository<SysDicData>();
            foreach (SysDicData item in repository.GetAll())
            {
                if (!_dicById.ContainsKey(item.GetId()))
                {
                    _dicById.Add(item.GetId(), item);
                }
                if (!_dicByCode.ContainsKey(item.Code))
                {
                    _dicByCode.Add(item.Code, item);
                }
            }
        }

        public bool ContainsKey(Guid sysDicId)
        {
            InitOnce();
            return _dicById.ContainsKey(sysDicId);
        }

        public bool ContainsKey(string sysDicCode)
        {
            InitOnce();
            return _dicByCode.ContainsKey(sysDicCode);
        }

        public bool TryGetSysDic(string sysDicCode, out ISysDic sysDic)
        {
            InitOnce();
            bool r = _dicByCode.TryGetValue(sysDicCode, out SysDicData d);
            sysDic = d;
            return r;
        }

        public bool TryGetSysDic(Guid sysDicId, out ISysDic sysDic)
        {
            InitOnce();
            bool r = _dicById.TryGetValue(sysDicId, out SysDicData d);
            sysDic = d;
            return r;
        }

        public IEnumerable<ISysDic> AsEnumerable()
        {
            InitOnce();
            return _dicById.Values;
        }
    }
}
