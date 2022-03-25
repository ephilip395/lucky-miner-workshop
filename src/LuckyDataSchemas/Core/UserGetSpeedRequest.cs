﻿using System;
using System.Collections.Generic;

namespace Lucky.Core {
    public class UserGetSpeedRequest {
        public UserGetSpeedRequest() {
            this.ClientIds = new List<Guid>();
        }

        public string LoginName { get; set; }
        public Guid StudioId { get; set; }
        public List<Guid> ClientIds { get; set; }
    }
}
