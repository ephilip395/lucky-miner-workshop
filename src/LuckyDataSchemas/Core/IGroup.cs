﻿using System;

namespace Lucky.Core {
    public interface IGroup : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        int SortNumber { get; }
    }
}
