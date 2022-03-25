using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Core.Impl
{
    public class ReadOnlyLuckyFileSet : SetBase, IReadOnlyLuckyFileSet
    {
        private readonly Dictionary<Guid, LuckyFileData> _dicById = new Dictionary<Guid, LuckyFileData>();
        private DateTime _timestamp = DateTime.MinValue;

        public ReadOnlyLuckyFileSet()
        {
            VirtualRoot.BuildCmdPath<RefreshLuckyFileSetCommand>(this.GetType(), LogEnum.DevConsole, path: message =>
            {
                Refresh();
            });
        }

        protected override void Init()
        {
            RpcRoot.OfficialServer.LuckyFileService.GetLuckyFilesAsync(_timestamp, (response, e) =>
            {
                if (response.IsSuccess())
                {
                    if (response.Data.Count > 0)
                    {
                        _dicById.Clear();
                        _timestamp = response.Timestamp;
                        foreach (var item in response.Data)
                        {
                            _dicById.Add(item.Id, item);
                        }
                        VirtualRoot.RaiseEvent(new LuckyFileSetInitedEvent());
                    }
                }
                else
                {
                    Logger.ErrorDebugLine(e.GetInnerMessage(), e);
                }
            });
        }

        private void Refresh()
        {
            base.DeferReInit();
            InitOnce();
        }

        public IEnumerable<LuckyFileData> AsEnumerable()
        {
            Refresh();
            return _dicById.Values;
        }
    }
}
