﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Core.MinerMonitor.Impl {
    public class ColumnsShowSet : SetBase, IColumnsShowSet {
        private readonly Dictionary<Guid, ColumnsShowData> _dicById = new Dictionary<Guid, ColumnsShowData>();

        public ColumnsShowSet() {
        }

        protected override void Init() {
            var repository = VirtualRoot.CreateLocalRepository<ColumnsShowData>();
            var columnsList = repository.GetAll();
            foreach (var item in columnsList) {
                _dicById.Add(item.Id, item);
            }
            if (!_dicById.ContainsKey(ColumnsShowData.PleaseSelect.Id)) {
                _dicById.Add(ColumnsShowData.PleaseSelect.Id, ColumnsShowData.PleaseSelect);
                repository.Add(ColumnsShowData.PleaseSelect);
            }
        }

        public List<ColumnsShowData> GetAll() {
            InitOnce();
            return _dicById.Values.ToList();
        }

        public void AddOrUpdate(ColumnsShowData data) {
            InitOnce();
            lock (_dicById) {
                var repository = VirtualRoot.CreateLocalRepository<ColumnsShowData>();
                if (_dicById.TryGetValue(data.Id, out ColumnsShowData entity)) {
                    entity.Update(data);
                    repository.Update(entity);
                }
                else {
                    _dicById.Add(data.Id, data);
                    repository.Add(data);
                }
            }
            VirtualRoot.RaiseEvent(new ColumnsShowAddedOrUpdatedEvent(Guid.Empty, data));
        }

        public void Remove(Guid id) {
            InitOnce();
            ColumnsShowData entity;
            lock (_dicById) {
                if (_dicById.TryGetValue(id, out entity)) {
                    _dicById.Remove(id);
                    var repository = VirtualRoot.CreateLocalRepository<ColumnsShowData>();
                    repository.Remove(id);
                }
            }
            if (entity != null) {
                VirtualRoot.RaiseEvent(new ColumnsRemovedEvent(Guid.Empty, entity));
            }
        }
    }
}
