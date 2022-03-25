using Lucky.Core;
using Lucky.Core.MinerServer;
using Lucky.User;
using System;
using System.Collections.Generic;

namespace Lucky.Controllers {
    public interface IUserMineWorkController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<List<UserMineWorkData>> MineWorks(object request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateMineWork(DataRequest<MineWorkData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveMineWork(DataRequest<Guid> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase ExportMineWork(ExportMineWorkRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<string> GetLocalJson(DataRequest<Guid> request);
    }
}
