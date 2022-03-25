﻿using Lucky.Controllers;
using Lucky.Core;
using Lucky.Core.MinerServer;
using Lucky.User;
using System;
using System.Collections.Generic;

namespace Lucky.Services.Official {
    public class UserMineWorkService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IUserMineWorkController>();

        internal UserMineWorkService() {
        }

        #region GetMineWorksAsync
        public void GetMineWorksAsync(Action<DataResponse<List<UserMineWorkData>>, Exception> callback) {
            object request = new object();
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IUserMineWorkController.MineWorks), 
                data: request, 
                callback, 
                timeountMilliseconds: 5000);
        }
        #endregion

        #region AddOrUpdateMineWorkAsync
        public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase, Exception> callback) {
            entity.ModifiedOn = DateTime.Now;
            DataRequest<MineWorkData> request = new DataRequest<MineWorkData> {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IUserMineWorkController.AddOrUpdateMineWork), 
                data: request, 
                callback);
        }
        #endregion

        #region RemoveMineWorkAsync
        public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid> {
                Data = id
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IUserMineWorkController.RemoveMineWork), 
                data: request, 
                callback);
        }
        #endregion

        #region ExportMineWorkAsync
        public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
            ExportMineWorkRequest request = new ExportMineWorkRequest {
                MineWorkId = workId,
                LocalJson = localJson,
                ServerJson = serverJson
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IUserMineWorkController.ExportMineWork), 
                data: request, 
                callback);
        }
        #endregion

        #region GetLocalJsonAsync
        public void GetLocalJsonAsync(Guid workId, Action<DataResponse<string>, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = workId
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName, 
                nameof(IUserMineWorkController.GetLocalJson), 
                data: request, 
                callback);
        }
        #endregion
    }
}
