﻿using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Controllers {
    public interface IServerMessageController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateServerMessage(DataRequest<ServerMessageData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase MarkDeleteServerMessage(DataRequest<Guid> request);
        DataResponse<List<ServerMessageData>> ServerMessages(ServerMessagesRequest request);
    }
}
