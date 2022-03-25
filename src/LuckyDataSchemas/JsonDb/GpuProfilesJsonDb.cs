﻿using Lucky.Core.Profile;
using Lucky.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.JsonDb {
    public class GpuProfilesJsonDb : IGpuProfilesJsonDb {
        public GpuType GpuType { get; set; }
        public GpuData[] Gpus { get; set; }
        public List<GpuProfileData> GpuProfiles { get; set; }
        public List<CoinOverClockData> CoinOverClocks { get; set; }

        public long TimeStamp { get; set; }

        public GpuProfilesJsonDb() {
            Gpus = new GpuData[0];
            GpuProfiles = new List<GpuProfileData>();
            CoinOverClocks = new List<CoinOverClockData>();
            this.TimeStamp = Timestamp.GetTimestamp();
        }

        public bool Exists<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().Any(a => a.GetId() == key);
        }

        public T GetByKey<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().FirstOrDefault(a => a.GetId() == key);
        }

        public IEnumerable<T> GetAll<T>() where T : IDbEntity<Guid> {
            string typeName = typeof(T).Name;
            switch (typeName) {
                case nameof(GpuData):
                    return this.Gpus.Cast<T>();
                case nameof(GpuProfileData):
                    return this.GpuProfiles.Cast<T>();
                case nameof(CoinOverClockData):
                    return this.CoinOverClocks.Cast<T>();
                default:
                    return new List<T>();
            }
        }
    }
}
