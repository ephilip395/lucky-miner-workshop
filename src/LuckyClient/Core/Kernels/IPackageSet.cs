using System;
using System.Collections.Generic;

namespace Lucky.Core.Kernels {
    public interface IPackageSet {
        bool Contains(Guid packageId);
        bool TryGetPackage(Guid packageId, out IPackage package);
        IEnumerable<IPackage> AsEnumerable();
    }
}
