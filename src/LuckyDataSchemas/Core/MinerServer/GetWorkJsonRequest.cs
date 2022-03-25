using System;
using System.Text;

namespace Lucky.Core.MinerServer {
    public class GetWorkJsonRequest : IRequest, ISignableData {
        public GetWorkJsonRequest() {
        }

        public Guid WorkId { get; set; }
        public Guid ClientId { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
