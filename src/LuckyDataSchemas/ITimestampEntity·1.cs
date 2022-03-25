using System;

namespace Lucky {
    public interface ITimestampEntity<T> : IEntity<T> {
        DateTime CreatedOn { get; }
        DateTime ModifiedOn { get; }
    }
}
