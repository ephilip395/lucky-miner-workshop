using Lucky.Core.MinerServer;
using System.Collections.Generic;

namespace Lucky.Controllers {
    public interface ICalcConfigController {
        DataResponse<List<CalcConfigData>> CalcConfigs();
        DataResponse<List<CalcConfigData>> Query(string coinCodes);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
    }
}
