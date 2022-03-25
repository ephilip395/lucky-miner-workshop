﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Core.Impl
{
    internal class SysDicItemSet : SetBase, ISysDicItemSet
    {
        private readonly IServerContext _context;
        private readonly Dictionary<Guid, Dictionary<string, SysDicItemData>> _dicByDicId
            = new Dictionary<Guid, Dictionary<string, SysDicItemData>>();
        private readonly Dictionary<Guid, SysDicItemData> _dicById = new Dictionary<Guid, SysDicItemData>();

        public SysDicItemSet(IServerContext context)
        {
            _context = context;
            _context.AddCmdPath<AddSysDicItemCommand>(LogEnum.DevConsole,
                action: (message) =>
                {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty)
                    {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("dicitem code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId()))
                    {
                        return;
                    }
                    if (!_dicByDicId.ContainsKey(message.Input.DicId))
                    {
                        _dicByDicId.Add(message.Input.DicId, new Dictionary<string, SysDicItemData>(StringComparer.OrdinalIgnoreCase));
                    }
                    if (_dicByDicId[message.Input.DicId].ContainsKey(message.Input.Code))
                    {
                        throw new ValidationException("编码重复");
                    }
                    SysDicItemData entity = new SysDicItemData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    _dicByDicId[message.Input.DicId].Add(entity.Code, entity);
                    Repositories.IRepository<SysDicItemData> repository = context.CreateCompositeRepository<SysDicItemData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new SysDicItemAddedEvent(message.MessageId, entity));
                }, location: GetType());
            _context.AddCmdPath<UpdateSysDicItemCommand>(LogEnum.DevConsole,
                action: (message) =>
                {
                    InitOnce();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty)
                    {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("sysDicItem code can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out SysDicItemData entity))
                    {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input))
                    {
                        return;
                    }
                    string oldCode = entity.Code;
                    _ = entity.Update(message.Input);
                    // 如果编码变更了 
                    if (oldCode != entity.Code)
                    {
                        _dicByDicId[entity.DicId].Remove(oldCode);
                        _dicByDicId[entity.DicId].Add(entity.Code, entity);
                    }
                    Repositories.IRepository<SysDicItemData> repository = context.CreateCompositeRepository<SysDicItemData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new SysDicItemUpdatedEvent(message.MessageId, entity));
                }, location: GetType());

            _context.AddCmdPath<RemoveSysDicItemCommand>(LogEnum.DevConsole,
                action: (message) =>
                {
                    InitOnce();
                    if (message == null || message.EntityId == Guid.Empty)
                    {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.TryGetValue(message.EntityId, out SysDicItemData entity)
                        || !_context.SysDicSet.TryGetSysDic(entity.DicId, out ISysDic sysDic))
                    {
                        return;
                    }
                    switch (sysDic.Code)
                    {
                        case LuckyKeyword.KernelBrandSysDicCode:
                            if (LuckyContext.Instance.ServerContext.KernelSet.AsEnumerable().Any(a => a.BrandId == message.EntityId))
                            {
                                VirtualRoot.Out.ShowWarn("该内核品牌字典项关联有内核品牌不能删除，请先解除关联");
                                return;
                            }
                            break;
                        case LuckyKeyword.PoolBrandSysDicCode:
                            if (LuckyContext.Instance.ServerContext.PoolSet.AsEnumerable().Any(a => a.BrandId == message.EntityId))
                            {
                                VirtualRoot.Out.ShowWarn("该矿池品牌字典项关联有矿池品牌不能删除，请先解除关联");
                                return;
                            }
                            break;
                        case LuckyKeyword.AlgoSysDicCode:
                            if (LuckyContext.Instance.ServerContext.PackageSet.AsEnumerable().Any(a => a.AlgoIds.Contains(message.EntityId)))
                            {
                                string s = "该";
                                if (LuckyContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(message.EntityId, out ISysDicItem item))
                                {
                                    s = item.Code;
                                }
                                VirtualRoot.Out.ShowWarn($"{s}算法字典项关联有内核不能删除，请先解除关联");
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    _ = _dicById.Remove(entity.Id);
                    if (_dicByDicId.TryGetValue(entity.DicId, out Dictionary<string, SysDicItemData> dicItemDic))
                    {
                        _ = dicItemDic.Remove(entity.Code);
                    }
                    Repositories.IRepository<SysDicItemData> repository = context.CreateCompositeRepository<SysDicItemData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new SysDicItemRemovedEvent(message.MessageId, entity));
                }, location: GetType());
        }

        protected override void Init()
        {
            Repositories.IRepository<SysDicItemData> repository = _context.CreateCompositeRepository<SysDicItemData>();
            foreach (SysDicItemData item in repository.GetAll())
            {
                if (!_dicById.ContainsKey(item.GetId()))
                {
                    _dicById.Add(item.GetId(), item);
                }
                if (!_dicByDicId.ContainsKey(item.DicId))
                {
                    _dicByDicId.Add(item.DicId, new Dictionary<string, SysDicItemData>(StringComparer.OrdinalIgnoreCase));
                }
                if (!_dicByDicId[item.DicId].ContainsKey(item.Code))
                {
                    _dicByDicId[item.DicId].Add(item.Code, item);
                }
            }
        }

        public bool ContainsKey(Guid dicItemId)
        {
            InitOnce();
            return _dicById.ContainsKey(dicItemId);
        }

        public bool ContainsKey(Guid dicId, string dicItemCode)
        {
            InitOnce();
            return _dicByDicId.TryGetValue(dicId, out _) && _dicByDicId[dicId].ContainsKey(dicItemCode);
        }

        public bool ContainsKey(string dicCode, string dicItemCode)
        {
            InitOnce();
            return _context.SysDicSet.TryGetSysDic(dicCode, out ISysDic sysDic)
&& _dicByDicId.ContainsKey(sysDic.GetId()) && _dicByDicId[sysDic.GetId()].ContainsKey(dicItemCode);
        }

        public bool TryGetDicItem(Guid dicItemId, out ISysDicItem dicItem)
        {
            InitOnce();
            bool r = _dicById.TryGetValue(dicItemId, out SysDicItemData di);
            dicItem = di;
            return r;
        }

        public bool TryGetDicItem(string dicCode, string dicItemCode, out ISysDicItem dicItem)
        {
            InitOnce();
            if (!_context.SysDicSet.TryGetSysDic(dicCode, out ISysDic sysDic))
            {
                dicItem = null;
                return false;
            }
            if (!_dicByDicId.TryGetValue(sysDic.GetId(), out Dictionary<string, SysDicItemData> items))
            {
                dicItem = null;
                return false;
            }
            bool r = items.TryGetValue(dicItemCode, out SysDicItemData di);
            dicItem = di;
            return r;
        }

        public bool TryGetDicItem(Guid dicId, string dicItemCode, out ISysDicItem dicItem)
        {
            InitOnce();
            if (!_dicByDicId.TryGetValue(dicId, out Dictionary<string, SysDicItemData> items))
            {
                dicItem = null;
                return false;
            }
            bool r = items.TryGetValue(dicItemCode, out SysDicItemData di);
            dicItem = di;
            return r;
        }

        public IEnumerable<ISysDicItem> GetSysDicItems(string dicCode)
        {
            InitOnce();
            return !_context.SysDicSet.TryGetSysDic(dicCode, out ISysDic sysDic)
                ? new List<ISysDicItem>()
                : (IEnumerable<ISysDicItem>)_dicByDicId[sysDic.GetId()].Values.ToList();
        }

        public IEnumerable<ISysDicItem> AsEnumerable()
        {
            InitOnce();
            return _dicById.Values;
        }
    }
}
