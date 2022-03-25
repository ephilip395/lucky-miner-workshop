using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public interface IServerMessageSet {
        List<ServerMessageData> GetServerMessages(DateTime timeStamp);
        IEnumerable<IServerMessage> AsEnumerable();
    }
}
