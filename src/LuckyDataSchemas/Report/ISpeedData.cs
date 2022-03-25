using System;

namespace Lucky.Report {
    public interface ISpeedData : ISpeedDto {
        DateTime SpeedOn { get; }
    }
}
