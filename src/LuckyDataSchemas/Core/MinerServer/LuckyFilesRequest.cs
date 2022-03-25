using System;

namespace Lucky.Core.MinerServer {
    public class LuckyFilesRequest : IRequest {
        public LuckyFilesRequest() { }

        public DateTime Timestamp { get; set; }
    }
}
