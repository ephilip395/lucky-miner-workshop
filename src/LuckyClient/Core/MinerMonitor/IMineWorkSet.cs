using System;
using System.Collections.Generic;

namespace Lucky.Core.MinerMonitor {
    public interface IMineWorkSet {
        bool Contains(Guid mineWorkId);
        IEnumerable<MineWorkData> AsEnumerable();
    }
}
