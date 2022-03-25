using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface ICoinGroupSet {
        List<Guid> GetGroupCoinIds(Guid groupId);
        IEnumerable<ICoinGroup> AsEnumerable();
    }
}
