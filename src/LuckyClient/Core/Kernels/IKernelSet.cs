using System;
using System.Collections.Generic;

namespace Lucky.Core.Kernels {
    public interface IKernelSet  : ICountSet {
        bool Contains(Guid kernelId);
        bool TryGetKernel(Guid kernelId, out IKernel kernel);
        IEnumerable<IKernel> AsEnumerable();
    }
}
