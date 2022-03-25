using System;

namespace Lucky.Core {
    public interface IMinerGroup : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
    }
}
