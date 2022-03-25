using Lucky.Core.MinerServer;
using Lucky.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.Core.Impl {
    public class OverClockDataSet : SetBase, IOverClockDataSet {
        private readonly Dictionary<Guid, OverClockData> _dicById = new Dictionary<Guid, OverClockData>();
        private readonly ILuckyContext _luckycmContext;

        public OverClockDataSet(ILuckyContext luckycmContext) {
            _luckycmContext = luckycmContext;
            VirtualRoot.BuildCmdPath<AddOverClockDataCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) => {
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.Name)) {
                    throw new ValidationException("OverClockData name can't be null or empty");
                }
                if (_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                OverClockData entity = new OverClockData().Update(message.Input);
                RpcRoot.OfficialServer.OverClockDataService.AddOrUpdateOverClockDataAsync(entity, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Add(entity.Id, entity);
                        VirtualRoot.RaiseEvent(new OverClockDataAddedEvent(message.MessageId, entity));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            });
            VirtualRoot.BuildCmdPath<UpdateOverClockDataCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) => {
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.Name)) {
                    throw new ValidationException("minerGroup name can't be null or empty");
                }
                if (!_dicById.TryGetValue(message.Input.GetId(), out OverClockData entity)) {
                    return;
                }
                OverClockData oldValue = new OverClockData().Update(entity);
                entity.Update(message.Input);
                RpcRoot.OfficialServer.OverClockDataService.AddOrUpdateOverClockDataAsync(entity, (response, e) => {
                    if (!response.IsSuccess()) {
                        entity.Update(oldValue);
                        VirtualRoot.RaiseEvent(new OverClockDataUpdatedEvent(message.MessageId, entity));
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
                VirtualRoot.RaiseEvent(new OverClockDataUpdatedEvent(message.MessageId, entity));
            });
            VirtualRoot.BuildCmdPath<RemoveOverClockDataCommand>(location: this.GetType(), LogEnum.DevConsole, path: (message) => {
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                OverClockData entity = _dicById[message.EntityId];
                RpcRoot.OfficialServer.OverClockDataService.RemoveOverClockDataAsync(entity.Id, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.RaiseEvent(new OverClockDataRemovedEvent(message.MessageId, entity));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            });
        }

        protected override void Init() {
            RpcRoot.OfficialServer.OverClockDataService.GetOverClockDatasAsync((response, e) => {
                if (response.IsSuccess()) {
                    IEnumerable<OverClockData> query;
                    if (_luckycmContext.GpuSet.GpuType.IsEmpty()) {
                        query = response.Data;
                    }
                    else {
                        query = response.Data.Where(a => a.GpuType == _luckycmContext.GpuSet.GpuType);
                    }
                    foreach (var item in query) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                }
                VirtualRoot.RaiseEvent(new OverClockDataSetInitedEvent());
            });
        }

        public bool TryGetOverClockData(Guid id, out IOverClockData data) {
            InitOnce();
            var r = _dicById.TryGetValue(id, out OverClockData temp);
            data = temp;
            return r;
        }

        public IEnumerable<IOverClockData> AsEnumerable() {
            InitOnce();
            return _dicById.Values;
        }
    }
}
