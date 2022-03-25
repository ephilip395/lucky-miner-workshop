using System;

namespace Lucky.Core.Profiles {
    public interface IKernelProfileSet {
        IKernelProfile EmptyKernelProfile { get; }

        IKernelProfile GetKernelProfile(Guid kernelId);
    }
}
