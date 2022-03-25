using Lucky.Core.MinerServer;
#if !NoDevFee
using System;
#endif
using System.Collections.Generic;

namespace Lucky.Controllers
{
    public interface ILuckyWalletController
    {
        // 该方法不需要验证，因为挖矿端调用
        DataResponse<List<LuckyWalletData>> LuckyWallets();
#if !NoDevFee
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateLuckyWallet(DataRequest<LuckyWalletData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveLuckyWallet(DataRequest<Guid> request);
#endif
    }
}
