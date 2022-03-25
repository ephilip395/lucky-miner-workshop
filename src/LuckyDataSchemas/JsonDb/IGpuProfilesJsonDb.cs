using Lucky.Core.Profile;
using Lucky.Gpus;
using System.Collections.Generic;

namespace Lucky.JsonDb {
    public interface IGpuProfilesJsonDb : IJsonDb {
        GpuType GpuType { get; }
        GpuData[] Gpus { get; }
        List<GpuProfileData> GpuProfiles { get; }
        List<CoinOverClockData> CoinOverClocks { get; }
    }
}
