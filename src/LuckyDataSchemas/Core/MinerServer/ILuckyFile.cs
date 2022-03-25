using System;

namespace Lucky.Core.MinerServer {
    public interface ILuckyFile {
        Guid Id { get; }

        LuckyAppType AppType { get; }

        string FileName { get; }

        string Version { get; }

        string VersionTag { get; }

        DateTime CreatedOn { get; }

        DateTime PublishOn { get; }

        string Title { get; }

        string Description { get; }
    }
}
