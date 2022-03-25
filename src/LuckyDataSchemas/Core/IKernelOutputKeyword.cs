using System;

namespace Lucky.Core {
    public interface IKernelOutputKeyword : ILevelEntity<Guid> {
        Guid KernelOutputId { get; }
        string MessageType { get; }
        string Keyword { get; }
        /// <summary>
        /// 大意
        /// </summary>
        string Description { get; }
    }
}
