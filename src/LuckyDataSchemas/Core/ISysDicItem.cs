﻿using System;

namespace Lucky.Core {
    public interface ISysDicItem : ILevelEntity<Guid> {
        Guid DicId { get; }
        string Code { get; }
        string Value { get; }
        string Description { get; }
        int SortNumber { get; }
    }
}
