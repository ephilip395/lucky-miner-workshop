using Lucky.Core;

namespace Lucky.Gpus {
    public interface IGpu : IGpuStaticData, IOverClockInput {
        int Temperature { get; }
        int MemoryTemperature { get; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; }
    }
}
