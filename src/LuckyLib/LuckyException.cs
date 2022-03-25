
namespace Lucky {
    using System;
    using System.Runtime.Serialization;

    public class LuckyException : Exception {
        public LuckyException() : base() { }

        public LuckyException(string message) : base(message) { }

        public LuckyException(string message, Exception innerException) : base(message, innerException) { }

        public LuckyException(string format, params object[] args) : base(string.Format(format, args)) { }

        protected LuckyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
