using System;

namespace Lucky.ServerNode {
    public class ClientTestIdData : IClientTestId {
        public ClientTestIdData() { }

        public Guid MinerTweakTestId { get; set; }

        public Guid StudioClientTestId { get; set; }
    }
}
