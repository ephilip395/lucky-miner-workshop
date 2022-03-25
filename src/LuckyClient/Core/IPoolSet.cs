using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IPoolSet {
        bool Contains(Guid poolId);
        bool TryGetPool(Guid poolId, out IPool pool);
        string GetPoolDelayText(Guid poolId, bool isDual);
        IEnumerable<IPool> AsEnumerable();
    }
}
