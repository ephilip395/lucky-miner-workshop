using System;
using System.Collections.Generic;

namespace Lucky.Core.Kernels {
    public interface ICoinKernelSet : ICountSet {
        bool Contains(Guid coinKernelId);
        bool TryGetCoinKernel(Guid coinKernelId, out ICoinKernel coinKernel);
        IEnumerable<ICoinKernel> AsEnumerable();
    }
}
