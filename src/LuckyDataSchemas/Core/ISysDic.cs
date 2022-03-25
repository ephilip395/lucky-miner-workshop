using System;

namespace Lucky.Core {
    public interface ISysDic : IEntity<Guid> {
        Guid Id { get; }
        string Code { get; }
        string Name { get; }
        string Description { get; }
        int SortNumber { get; }
    }
}
