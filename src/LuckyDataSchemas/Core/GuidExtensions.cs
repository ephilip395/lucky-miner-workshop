using System;

namespace Lucky.Core {
    public static class GuidExtensions {
        public static bool IsSelfMineWorkId(this Guid guid) {
            return guid == MineWorkData.SelfMineWorkId;
        }
    }
}
