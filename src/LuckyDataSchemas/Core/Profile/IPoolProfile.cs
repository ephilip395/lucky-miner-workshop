using System;

namespace Lucky.Core.Profile {
    public interface IPoolProfile {
        Guid PoolId { get; }
        string UserName { get; }
        string Password { get; }
        string Server { get; }
    }
}
