using Lucky.Controllers;
using Lucky.ServerNode;
using System;

namespace Lucky.Services.Official {
    public class WsServerNodeService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IWsServerNodeController>();

        internal WsServerNodeService() {
        }

        public void GetNodeAddressAsync(Guid clientId, string outerUserId, Action<DataResponse<string>, Exception> callback) {
            var data = new GetWsServerNodeAddressRequest {
                ClientId = clientId,
                UserId = outerUserId
            };
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IWsServerNodeController.GetNodeAddress), 
                data, 
                callback, 
                timeountMilliseconds: 8000);
        }
    }
}
