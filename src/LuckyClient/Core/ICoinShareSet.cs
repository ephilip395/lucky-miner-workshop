using System;

namespace Lucky.Core {
    public interface ICoinShareSet {
        ICoinShare GetOrCreate(Guid coinId);
        void UpdateShare(Guid coinId, int? acceptShareCount, int? rejectShareCount, DateTime now);
    }
}
