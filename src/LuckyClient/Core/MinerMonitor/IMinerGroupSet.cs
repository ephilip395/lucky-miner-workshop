using System;
using System.Collections.Generic;

namespace Lucky.Core.MinerMonitor {
    public interface IMinerGroupSet {
        bool Contains(Guid id);
        IEnumerable<MinerGroupData> AsEnumerable();
    }
}
