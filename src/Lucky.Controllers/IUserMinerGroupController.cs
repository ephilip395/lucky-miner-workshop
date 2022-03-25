using Lucky.Core;
using Lucky.User;
using System;
using System.Collections.Generic;

namespace Lucky.Controllers {
    public interface IUserMinerGroupController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<List<UserMinerGroupData>> MinerGroups(object request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
    }
}
