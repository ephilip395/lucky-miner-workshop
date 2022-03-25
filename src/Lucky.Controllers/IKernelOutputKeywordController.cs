using Lucky.Core;
using Lucky.Core.MinerServer;
using System;

namespace Lucky.Controllers {
    public interface IKernelOutputKeywordController {
        KernelOutputKeywordsResponse KernelOutputKeywords(object request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveKernelOutputKeyword(DataRequest<Guid> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase ClearByExceptedOutputIds(DataRequest<Guid[]> request);
    }
}
