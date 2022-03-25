﻿using Lucky.Controllers;
using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Services.Official {
    public partial class ClientDataService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IClientDataController>();

        internal ClientDataService() {
        }

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            UpdateClientRequest request = new UpdateClientRequest {
                ObjectId = objectId,
                PropertyName = propertyName,
                Value = value
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IClientDataController.UpdateClient), 
                data: request, 
                callback);
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            UpdateClientsRequest request = new UpdateClientsRequest {
                PropertyName = propertyName,
                Values = values
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IClientDataController.UpdateClients), 
                data: request, 
                callback);
        }
        #endregion

        #region RemoveClientsAsync
        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            var request = new MinerIdsRequest {
                ObjectIds = objectIds
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IClientDataController.RemoveClients), 
                data: request, 
                callback);
        }
        #endregion
    }
}
