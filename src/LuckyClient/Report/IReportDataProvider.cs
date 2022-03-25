using System;

namespace Lucky.Report {
    public interface IReportDataProvider {
        DateTime WsGetSpeedOn { get; set; }
        SpeedDto CreateSpeedDto();
    }
}
