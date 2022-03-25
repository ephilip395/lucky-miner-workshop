using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Controllers {
    public interface ILuckyFileController {
        List<LuckyFileData> LuckyFiles();
        LuckyFilesResponse GetLuckyFiles(LuckyFilesRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateLuckyFile(DataRequest<LuckyFileData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveLuckyFile(DataRequest<Guid> request);
    }
}
