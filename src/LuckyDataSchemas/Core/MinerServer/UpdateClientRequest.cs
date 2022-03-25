using System.Text;

namespace Lucky.Core.MinerServer {
    public class UpdateClientRequest : IRequest, ISignableData {
        public UpdateClientRequest() { }
        public string ObjectId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
