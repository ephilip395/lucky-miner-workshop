using Lucky.Core.MinerServer;

namespace Lucky.Controllers {
    /// <summary>
    /// 群控端的矿机列表接口
    /// </summary>
    public interface IClientDataController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase UpdateClient(UpdateClientRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase UpdateClients(UpdateClientsRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveClients(MinerIdsRequest request);
    }
}
