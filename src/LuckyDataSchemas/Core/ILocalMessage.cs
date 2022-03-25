﻿using System;

namespace Lucky.Core {
    public interface ILocalMessage : IEntity<Guid> {
        Guid Id { get; }
        /// <summary>
        /// 频道是平的，主题是分层的。行运矿工的挖矿消息没有主题需求。
        /// </summary>
        string Channel { get; }
        string Provider { get; }
        string MessageType { get; }
        string Content { get; }
        DateTime Timestamp { get; }
    }
}
