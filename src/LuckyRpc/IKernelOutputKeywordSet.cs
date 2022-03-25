using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IKernelOutputKeywordSet {
        List<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
        IEnumerable<IKernelOutputKeyword> AsEnumerable();
    }
}
