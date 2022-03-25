using System;

namespace Lucky.IdGenerator {
    public class DefaultIdGenerator : IIdGenerator {
        public string Generate() {
            return Guid.NewGuid().ToString("N");
        }
    }
}
