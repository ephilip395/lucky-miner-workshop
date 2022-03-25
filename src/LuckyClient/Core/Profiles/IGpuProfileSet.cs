﻿using Lucky.Core.Profile;
using System;

namespace Lucky.Core.Profiles {
    public interface IGpuProfileSet {
        IGpuProfile GetGpuProfile(Guid coinId, int index);
        bool IsOverClockEnabled(Guid coinId);
        bool IsOverClockGpuAll(Guid coinId);
        void RemoteOverClock();
        void SetIsOverClockEnabled(Guid coinId, bool value);
        void SetIsOverClockGpuAll(Guid coinId, bool value);
    }
}