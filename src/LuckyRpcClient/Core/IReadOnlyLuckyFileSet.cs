using Lucky.Core.MinerServer;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IReadOnlyLuckyFileSet {
        IEnumerable<LuckyFileData> AsEnumerable();
    }
}
