﻿using LiteDB;
using System;

namespace Lucky.Core.Profile {
    public class PoolProfileData : IPoolProfile, IProfile, IDbEntity<Guid> {
        public PoolProfileData() { }

        public static PoolProfileData CreateDefaultData(IPool pool) {
            return new PoolProfileData() {
                PoolId = pool.GetId(),
                UserName = pool.UserName,
                Password = pool.Password,
                Server = string.Empty
            };
        }

        public Guid GetId() {
            return this.PoolId;
        }

        [BsonId]
        public Guid PoolId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        // 检测内存状态是否变更时使用
        public override string ToString() {
            return this.BuildSign().ToString();
        }
    }
}
