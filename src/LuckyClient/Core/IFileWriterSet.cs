using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IFileWriterSet {
        bool TryGetFileWriter(Guid writerId, out IFileWriter writer);
        IEnumerable<IFileWriter> AsEnumerable();
    }
}
