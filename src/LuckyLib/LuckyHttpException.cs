
namespace Lucky {
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class LuckyHttpException : Exception {
        public LuckyHttpException() : base() { }

        public LuckyHttpException(string message) : base(message) { }

        public LuckyHttpException(string message, Exception innerException) : base(message, innerException) { }

        public LuckyHttpException(string format, params object[] args) : base(string.Format(format, args)) { }

        protected LuckyHttpException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public HttpStatusCode StatusCode { get; set; }

        public string ReasonPhrase { get; set; }
    }
}
