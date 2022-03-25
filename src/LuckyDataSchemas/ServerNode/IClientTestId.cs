using System;

namespace Lucky.ServerNode {
    public interface IClientTestId {
        Guid MinerTweakTestId { get; }
        Guid StudioClientTestId { get; }
    }
}
