using Lucky.ServerNode;

namespace Lucky.Controllers {
    public interface IWsServerNodeController {
        DataResponse<string> GetNodeAddress(GetWsServerNodeAddressRequest request);
    }
}
