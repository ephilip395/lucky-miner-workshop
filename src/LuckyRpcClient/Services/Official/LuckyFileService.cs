using Lucky.Controllers;
using Lucky.Core.MinerServer;
using System;

namespace Lucky.Services.Official
{
    public class LuckyFileService
    {
        private readonly string _controllerName = ControllerUtil.GetControllerName<ILuckyFileController>();

        internal LuckyFileService()
        {
        }

        #region GetLuckyFilesAsync
        // ReSharper disable once InconsistentNaming
        public void GetLuckyFilesAsync(DateTime timestamp, Action<LuckyFilesResponse, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(ILuckyFileController.GetLuckyFiles),
                new LuckyFilesRequest
                {
                    Timestamp = timestamp
                },
                callback);
        }
        #endregion

        #region AddOrUpdateLuckyFileAsync
        // ReSharper disable once InconsistentNaming
        public void AddOrUpdateLuckyFileAsync(LuckyFileData entity, Action<ResponseBase, Exception> callback)
        {
            DataRequest<LuckyFileData> request = new DataRequest<LuckyFileData>()
            {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(ILuckyFileController.AddOrUpdateLuckyFile),
                data: request,
                callback);
        }
        #endregion

        #region RemoveLuckyFileAsync
        // ReSharper disable once InconsistentNaming
        public void RemoveLuckyFileAsync(Guid id, Action<ResponseBase, Exception> callback)
        {
            DataRequest<Guid> request = new DataRequest<Guid>()
            {
                Data = id
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(ILuckyFileController.RemoveLuckyFile),
                data: request,
                callback);
        }
        #endregion
    }
}
