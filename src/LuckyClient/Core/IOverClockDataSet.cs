﻿using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IOverClockDataSet {
        bool TryGetOverClockData(Guid id, out IOverClockData data);
        IEnumerable<IOverClockData> AsEnumerable();
    }
}
