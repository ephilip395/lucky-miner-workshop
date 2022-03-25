﻿using System.ComponentModel;

namespace Lucky.Core {
    public enum PublishStatus {
        [Description("已发布")]
        Published = 0,
        [Description("未发布")]
        UnPublished = 1
    }
}
