using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IFragmentWriterSet {
        bool TryGetFragmentWriter(Guid writerId, out IFragmentWriter writer);
        IEnumerable<IFragmentWriter> AsEnumerable();
    }
}
