namespace Lucky.Controllers {
    public interface IMinerMonitorController {
        bool ShowMainWindow();
        ResponseBase CloseMinerMonitor(object request);
    }
}
