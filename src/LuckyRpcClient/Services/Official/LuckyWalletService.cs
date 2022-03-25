using Lucky.Controllers;
using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Services.Official
{
    public class LuckyWalletService
    {
        private readonly string _controllerName = ControllerUtil.GetControllerName<ILuckyWalletController>();

        internal LuckyWalletService()
        {
        }

        #region GetLuckyWalletsAsync
        public void GetLuckyWalletsAsync(Action<DataResponse<List<LuckyWalletData>>, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(ILuckyWalletController.LuckyWallets),
                null,
                callback);
        }
        #endregion

        #region AddOrUpdateLuckyWalletAsync
        public void AddOrUpdateLuckyWalletAsync(LuckyWalletData entity, Action<ResponseBase, Exception> callback)
        {
            DataRequest<LuckyWalletData> request = new DataRequest<LuckyWalletData>()
            {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(ILuckyWalletController.AddOrUpdateLuckyWallet),
                data: request,
                callback);
        }
        #endregion

        #region RemoveLuckyWalletAsync
        public void RemoveLuckyWalletAsync(Guid id, Action<ResponseBase, Exception> callback)
        {
            DataRequest<Guid> request = new DataRequest<Guid>()
            {
                Data = id
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(ILuckyWalletController.RemoveLuckyWallet),
                data: request,
                callback);
        }
        #endregion
    }
}
